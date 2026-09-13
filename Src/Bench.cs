using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using HarmonyLib;
using UnityEngine;

namespace com.github.lhervier.ksp.terrainprecisionfix
{
    /// <summary>What the measurement records, read from settings.cfg.</summary>
    internal enum BenchMode
    {
        /// <summary>Nothing at all: the mod behaves as if this file were not there.</summary>
        Off,

        /// <summary>What the terrain costs in flight, one line per second of game time.</summary>
        Counters,

        /// <summary>Adds, on a sample of quads, the stock formula timed against this one on the same data.</summary>
        Calibrate
    }

    /// <summary>
    /// Measures what the fix costs. Throwaway code: it exists to produce figures for the README and is
    /// meant to be deleted before the fix is proposed anywhere.
    ///
    /// Everything is accumulated in memory, in arrays allocated once, and nothing is written until it is
    /// asked for: writing to KSP.log while measuring would cost more than what is being measured.
    /// Modifier (Alt) + F8 dumps what has been recorded so far, Modifier + F7 throws it away.
    ///
    /// Deleting this file means deleting three lines in TerrainPrecisionFixMod.cs: the Bench.LoadSettings()
    /// call in Start, and the two Bench.PatchEnabled tests.
    /// </summary>
    internal static class Bench
    {
        // One sample per second of game time, so that two runs of the same flight are cut into pieces
        // covering the same stretch of trajectory even if one of them runs slower than the other.
        private const double SampleSeconds = 1.0;
        private const int MaxSamples = 4096;

        // Calibrate: how many times each formula is replayed over a quad, and how often a quad is used
        // for it. One pass over a couple of hundred vertices already lasts far longer than the timer's
        // resolution; the repeats are there to average, not to make the measurement possible.
        private const int CalibrationRounds = 8;
        private const int CalibrateOneQuadIn = 32;

        private static readonly double TicksToNanoseconds = 1e9 / Stopwatch.Frequency;

        private static BenchMode _mode = BenchMode.Off;
        private static bool _patchEnabled = true;

        /// <summary>Whether the fix corrects anything, as opposed to only counting what stock does.</summary>
        public static bool PatchEnabled { get { return _patchEnabled; } }

        private static bool Recording { get { return _mode != BenchMode.Off; } }

        /// <summary>What was accumulated over one second of game time.</summary>
        private struct Sample
        {
            public double Ut;
            public double UtSpan;
            public double RealSeconds;
            public int Frames;
            public double Altitude;
            public double Speed;
            public int Quads;
            public int PatchedQuads;
            public long Vertices;
            public long BuildTicks;
            public long UpdateTicks;
            public long SubdivisionSum;
            public int SubdivisionMax;
            public int SpeedLevelCap;
            public int MaxLevel;
        }

        private static readonly Sample[] _samples = new Sample[MaxSamples];
        private static int _sampleCount;
        private static bool _full;

        private static Sample _current;
        private static bool _open;
        private static long _currentStartTicks;
        private static int _warpedSeconds;

        private static long _stockTicks;
        private static long _fixedTicks;
        private static int _patchedQuadsSeen;
        private static int _calibratedQuads;
        private static long _calibratedVertices;
        private static Vector3[] _savedQuadVerts;
        private static Vector3d[] _savedSphereVerts;

        // Terrain spheres whose subdivision settings have been logged, so that they are logged once.
        private static readonly HashSet<PQS> _describedSpheres = new HashSet<PQS>();

        /// <summary>
        /// Reads benchMode and patchEnabled from PluginData/settings.cfg, next to the DLL. A missing file
        /// or an unknown value leaves the mod as it ships: no measurement, fix on.
        /// </summary>
        public static void LoadSettings()
        {
            string folder = Path.GetDirectoryName(typeof(Bench).Assembly.Location);
            string path = Path.Combine(Path.Combine(folder, "PluginData"), "settings.cfg");
            if (!File.Exists(path))
            {
                return;
            }

            ConfigNode node = ConfigNode.Load(path);
            if (node == null)
            {
                return;
            }

            string mode = node.GetValue("benchMode");
            if (!string.IsNullOrEmpty(mode))
            {
                BenchMode parsed;
                if (Enum.TryParse(mode, true, out parsed) && Enum.IsDefined(typeof(BenchMode), parsed))
                {
                    _mode = parsed;
                }
                else
                {
                    Log.Warning("Unknown benchMode '" + mode + "' in " + path + ", measuring nothing");
                }
            }

            string enabled = node.GetValue("patchEnabled");
            bool parsedEnabled;
            if (!string.IsNullOrEmpty(enabled) && bool.TryParse(enabled, out parsedEnabled))
            {
                _patchEnabled = parsedEnabled;
            }

            if (!_patchEnabled)
            {
                Log.Warning("patchEnabled = false: the terrain is left exactly as stock builds it."
                    + " This is the reference run of a performance campaign, not a working fix.");
            }
            if (Recording)
            {
                Log.Info($"Measuring performance, benchMode {_mode}, {PQS.cacheVertCount} vertices per"
                    + " quad. Alt+F8 dumps what has been recorded, Alt+F7 throws it away.");
                if (Log.IsTraceEnabled)
                {
                    Log.Warning("logLevel is Trace, which writes to KSP.log on the very path being"
                        + " measured: the figures would be worthless. Measure at Info.");
                }
            }
        }

        // ==========================================================================
        // Recording
        // ==========================================================================

        /// <summary>Counts a frame, and closes the current sample once a second of game time has passed.</summary>
        public static void OnFrame()
        {
            if (!Recording || _full || !HighLogic.LoadedSceneIsFlight)
            {
                return;
            }

            // Time warp is not measured at all: the craft crosses the ground far too fast for a sample to
            // mean anything, and a sample per second of game time would be hundreds of samples per second.
            if (TimeWarp.CurrentRate > 1.05f)
            {
                if (_open)
                {
                    _open = false;
                    _warpedSeconds++;
                }
                return;
            }

            Vessel vessel = FlightGlobals.ActiveVessel;
            if (vessel == null)
            {
                return;
            }
            double ut = Planetarium.GetUniversalTime();

            if (!_open)
            {
                Open(ut);
                return;
            }

            _current.Frames++;
            if (ut - _current.Ut < SampleSeconds)
            {
                return;
            }

            // Where the craft was is read at the end of the sample rather than averaged over it: across a
            // second of a ballistic pass it barely moves, and its only job is to say where this was taken.
            _current.UtSpan = ut - _current.Ut;
            _current.RealSeconds = (Stopwatch.GetTimestamp() - _currentStartTicks) / (double)Stopwatch.Frequency;
            _current.Altitude = vessel.altitude;
            _current.Speed = vessel.srfSpeed;
            Close();
            Open(ut);
        }

        private static void Open(double ut)
        {
            _current = new Sample();
            _current.Ut = ut;
            _current.SpeedLevelCap = int.MaxValue;

            // The frame a sample opens on is counted here, since the clock starts on it too. The frame two
            // samples straddle therefore counts in both, which is what it costs: it spans both.
            _current.Frames = 1;
            _currentStartTicks = Stopwatch.GetTimestamp();
            _open = true;
        }

        private static void Close()
        {
            if (_sampleCount >= MaxSamples)
            {
                if (!_full)
                {
                    _full = true;
                    Log.Warning(MaxSamples + " samples recorded, no more room: dump them (Alt+F8) and"
                        + " start again (Alt+F7)");
                }
                _open = false;
                return;
            }
            _samples[_sampleCount++] = _current;
        }

        /// <summary>Records one terrain quad actually built, and the time it took.</summary>
        private static void RecordQuad(PQS sphere, PQ quad, long ticks)
        {
            if (_describedSpheres.Add(sphere))
            {
                DescribeSphere(sphere);
            }

            bool patched = TerrainPrecisionFixMod.AppliesTo(quad);
            if (_open)
            {
                _current.Quads++;
                _current.Vertices += PQS.cacheVertCount;
                _current.BuildTicks += ticks;
                _current.SubdivisionSum += quad.subdivision;
                if (quad.subdivision > _current.SubdivisionMax)
                {
                    _current.SubdivisionMax = quad.subdivision;
                }
                if (patched)
                {
                    _current.PatchedQuads++;
                }

                // The lowest ceiling the sphere put on subdivision while this sample lasted. Below maxLevel,
                // the craft is crossing the ground too fast for the game to build the quads that carry a
                // collider — which are the only ones this mod touches.
                if (sphere.maxLevelAtCurrentTgtSpeed < _current.SpeedLevelCap)
                {
                    _current.SpeedLevelCap = sphere.maxLevelAtCurrentTgtSpeed;
                }
                _current.MaxLevel = sphere.maxLevel;
            }
            if (_mode != BenchMode.Calibrate || !patched)
            {
                return;
            }
            if (_patchedQuadsSeen % CalibrateOneQuadIn == 0)
            {
                Calibrate(sphere, quad);
            }
            _patchedQuadsSeen++;
        }

        /// <summary>
        /// Logs, once per terrain sphere, what decides how far it subdivides: the altitude the highest
        /// level appears under, which levels carry a collider, and how fast the craft may cross the ground
        /// before the game stops building that highest level at all.
        /// </summary>
        private static void DescribeSphere(PQS sphere)
        {
            int maxLevel = sphere.maxLevel;

            // A quad of level L splits when the craft is closer than subdivisionThresholds[L], so the
            // highest level appears under the threshold of the level below it.
            double highestLevelUnder = sphere.subdivisionThresholds != null
                && maxLevel - 1 >= 0 && maxLevel - 1 < sphere.subdivisionThresholds.Length
                ? sphere.subdivisionThresholds[maxLevel - 1]
                : 0.0;

            // PQS.UpdateVisual refuses to subdivide past the level whose quads are wider than what the
            // craft crosses between two samples, which is a real time interval: the slower the game runs,
            // the lower this ceiling falls.
            double angleCap = 1.5707963 / Math.Pow(2.0, maxLevel) * sphere.maxQuadLenghtsPerFrame;

            Log.Info($"BENCH sphere;name={sphere.name};radius={F(sphere.radius, 0)}"
                + $";minLevel={sphere.minLevel};maxLevel={maxLevel}"
                + $";highestLevelUnder={F(highestLevelUnder, 0)}"
                + $";subdivisionOffAbove={F(sphere.maxDetailDistance * sphere.radius, 0)}"
                + $";maxAnglePerSample={angleCap.ToString("E3", CultureInfo.InvariantCulture)}"
                + $";speedCapAt60Fps={F(angleCap * sphere.radius * 60.0, 0)}");

            PQSMod_QuadMeshColliders[] colliders =
                sphere.GetComponentsInChildren<PQSMod_QuadMeshColliders>(true);
            if (colliders == null || colliders.Length == 0)
            {
                Log.Info($"BENCH colliders;name={sphere.name};none");
                return;
            }
            foreach (PQSMod_QuadMeshColliders collider in colliders)
            {
                // Anything below maxLevel has a collider the fix leaves where stock puts it.
                Log.Info($"BENCH colliders;name={sphere.name};maxLevelOffset={collider.maxLevelOffset}"
                    + $";lowestLevelWithCollider={maxLevel - Math.Abs(collider.maxLevelOffset)}");
            }
        }

        // ==========================================================================
        // Calibrate: the two formulas, same data, same frame
        // ==========================================================================

        /// <summary>
        /// Times the stock vertex placement against this mod's, replaying both over the vertices of a quad
        /// that has just been built. Leaves the quad exactly as it found it.
        /// </summary>
        private static void Calibrate(PQS sphere, PQ quad)
        {
            int count = PQS.cacheVertCount;
            if (quad.verts == null || PQS.verts == null
                || quad.verts.Length < count || PQS.verts.Length < count)
            {
                return;
            }

            // Both formulas write where the real build wrote, so what the terrain ends up looking like
            // would otherwise depend on which one happened to run last.
            if (_savedQuadVerts == null || _savedQuadVerts.Length < count)
            {
                _savedQuadVerts = new Vector3[count];
                _savedSphereVerts = new Vector3d[count];
            }
            Array.Copy(quad.verts, _savedQuadVerts, count);
            Array.Copy(PQS.verts, _savedSphereVerts, count);

            // One round of each before the clock starts: the first pass over an array that is not in cache
            // would otherwise be charged to whichever formula runs first.
            RunStock(sphere, quad, count);
            RunFixed(sphere, quad, count);

            // And the order alternates from one calibrated quad to the next, so that whatever is left of
            // that effect does not always land on the same side.
            if ((_calibratedQuads & 1) == 0)
            {
                _stockTicks += TimeStock(sphere, quad, count);
                _fixedTicks += TimeFixed(sphere, quad, count);
            }
            else
            {
                _fixedTicks += TimeFixed(sphere, quad, count);
                _stockTicks += TimeStock(sphere, quad, count);
            }

            Array.Copy(_savedQuadVerts, quad.verts, count);
            Array.Copy(_savedSphereVerts, PQS.verts, count);

            _calibratedQuads++;
            _calibratedVertices += (long)count * CalibrationRounds;
        }

        private static long TimeStock(PQS sphere, PQ quad, int count)
        {
            long start = Stopwatch.GetTimestamp();
            for (int round = 0; round < CalibrationRounds; round++)
            {
                RunStock(sphere, quad, count);
            }
            return Stopwatch.GetTimestamp() - start;
        }

        private static long TimeFixed(PQS sphere, PQ quad, int count)
        {
            long start = Stopwatch.GetTimestamp();
            for (int round = 0; round < CalibrationRounds; round++)
            {
                RunFixed(sphere, quad, count);
            }
            return Stopwatch.GetTimestamp() - start;
        }

        /// <summary>The stock placement, as PQS.BuildVertexSurfaceRelative does it.</summary>
        private static void RunStock(PQS sphere, PQ quad, int count)
        {
            Transform sphereTransform = sphere.transform;
            Transform quadTransform = quad.transform;
            for (int index = 0; index < count; index++)
            {
                Vector3d vertex = PQS.verts[index];
                quad.verts[index] = quadTransform.InverseTransformPoint(
                    sphereTransform.TransformPoint((Vector3)vertex));
            }
        }

        /// <summary>
        /// This mod's placement. The real method, not a copy of its arithmetic: what a vertex costs here
        /// includes deciding, for every single one of them, that the fix applies at all.
        /// </summary>
        private static void RunFixed(PQS sphere, PQ quad, int count)
        {
            for (int index = 0; index < count; index++)
            {
                TerrainPrecisionFixMod.PlaceVertex(sphere, quad, index, PQS.verts[index]);
            }
        }

        // ==========================================================================
        // Reading it back
        // ==========================================================================

        /// <summary>Writes everything recorded so far to KSP.log, as semicolon separated lines.</summary>
        public static void Dump()
        {
            Log.Info($"BENCH begin;mode={_mode};patchEnabled={_patchEnabled};samples={_sampleCount}"
                + $";verticesPerQuad={PQS.cacheVertCount};warpedSeconds={_warpedSeconds}");
            Log.Info("BENCH;sample;ut;utSpan;realSeconds;frames;fps;altitude;speed;quads;patchedQuads"
                + ";vertices;buildMs;updateMs;subdivisionAvg;subdivisionMax;speedLevelCap;maxLevel");
            for (int i = 0; i < _sampleCount; i++)
            {
                Sample s = _samples[i];
                double buildMs = s.BuildTicks * 1000.0 / Stopwatch.Frequency;
                double updateMs = s.UpdateTicks * 1000.0 / Stopwatch.Frequency;
                double fps = s.RealSeconds > 0.0 ? s.Frames / s.RealSeconds : 0.0;
                double subdivisionAvg = s.Quads > 0 ? s.SubdivisionSum / (double)s.Quads : 0.0;
                Log.Info(string.Join(";", new[]
                {
                    "BENCH",
                    I(i),
                    F(s.Ut, 2), F(s.UtSpan, 3), F(s.RealSeconds, 3),
                    I(s.Frames), F(fps, 1),
                    F(s.Altitude, 1), F(s.Speed, 1),
                    I(s.Quads), I(s.PatchedQuads), L(s.Vertices),
                    F(buildMs, 3), F(updateMs, 3),
                    F(subdivisionAvg, 2), I(s.SubdivisionMax),
                    I(s.SpeedLevelCap == int.MaxValue ? -1 : s.SpeedLevelCap), I(s.MaxLevel)
                }));
            }

            if (_calibratedVertices > 0)
            {
                double stock = _stockTicks * TicksToNanoseconds / _calibratedVertices;
                double patched = _fixedTicks * TicksToNanoseconds / _calibratedVertices;
                Log.Info($"BENCH calibration;quads={_calibratedQuads}"
                    + $";verticesPerFormula={_calibratedVertices}"
                    + $";stockNsPerVertex={F(stock, 1)};fixedNsPerVertex={F(patched, 1)}"
                    + $";differenceNsPerVertex={F(patched - stock, 1)}");
            }
            Log.Info("BENCH end");
        }

        /// <summary>Throws away everything recorded, to start another run without restarting KSP.</summary>
        public static void Reset()
        {
            _sampleCount = 0;
            _full = false;
            _open = false;
            _warpedSeconds = 0;
            _stockTicks = 0L;
            _fixedTicks = 0L;
            _patchedQuadsSeen = 0;
            _calibratedQuads = 0;
            _calibratedVertices = 0L;
            _describedSpheres.Clear();
            Log.Info("BENCH reset");
        }

        // The log is read by a spreadsheet or a script, so numbers are written with a dot whatever the
        // machine's locale says.
        private static string F(double value, int decimals)
        {
            return value.ToString("F" + decimals, CultureInfo.InvariantCulture);
        }

        private static string I(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        private static string L(long value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        // ==========================================================================
        // Where the time is taken
        // ==========================================================================

        /// <summary>
        /// Times one terrain quad being built. PQS.BuildQuad is the loop over the vertices of a single
        /// quad, so it runs once per quad actually built, and what it costs includes this mod's vertex
        /// patch.
        /// </summary>
        [HarmonyPatch(typeof(PQS), "BuildQuad")]
        private static class BuildQuadPatch
        {
            private static void Prefix(out long __state)
            {
                __state = Recording ? Stopwatch.GetTimestamp() : 0L;
            }

            private static void Postfix(PQS __instance, PQ quad, bool __result, long __state)
            {
                // A false result is a call that returned without building anything.
                if (__state == 0L || !__result || quad == null)
                {
                    return;
                }
                RecordQuad(__instance, quad, Stopwatch.GetTimestamp() - __state);
            }
        }

        /// <summary>
        /// Times the whole terrain update of one sphere for one frame, which contains the quad builds
        /// above along with the subdivision decisions and the normals. This is what a frame pays.
        /// </summary>
        [HarmonyPatch(typeof(PQS), "UpdateQuads")]
        private static class UpdateQuadsPatch
        {
            private static void Prefix(out long __state)
            {
                __state = Recording ? Stopwatch.GetTimestamp() : 0L;
            }

            private static void Postfix(long __state)
            {
                if (__state == 0L || !_open)
                {
                    return;
                }
                _current.UpdateTicks += Stopwatch.GetTimestamp() - __state;
            }
        }
    }

    /// <summary>Counts frames and listens for the dump and reset keys.</summary>
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class BenchRunner : MonoBehaviour
    {
        // Read through KSP's own key bindings rather than UnityEngine.Input, which lives in a module this
        // mod does not reference: a throwaway measurement is not a reason to add one.
        private static readonly KeyBinding _dump = new KeyBinding(KeyCode.F8);
        private static readonly KeyBinding _reset = new KeyBinding(KeyCode.F7);

        private void Update()
        {
            Bench.OnFrame();
            if (!GameSettings.MODIFIER_KEY.GetKey())
            {
                return;
            }
            if (_dump.GetKeyDown())
            {
                Bench.Dump();
            }
            else if (_reset.GetKeyDown())
            {
                Bench.Reset();
            }
        }
    }
}
