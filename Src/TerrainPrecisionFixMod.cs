using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace com.github.lhervier.ksp.terrainprecisionfix
{
    /// <summary>
    /// Places the terrain the same way at every load, where its own double precision coordinates say it is.
    ///
    /// Stock KSP knows the position of every terrain vertex in double precision, relative to the centre of
    /// the body, then places the terrain through Unity transforms, which are single precision, while those
    /// vectors are still hundreds of kilometres long: 600 km on Kerbin, where a float can only represent
    /// every 62.5 mm. The rounding depends on the orientation of the world frame, which differs at every
    /// load, so the same piece of ground comes back a few centimetres higher or lower each time.
    ///
    /// The two placements involved — of each terrain quad, and of each vertex inside its quad — are redone
    /// here with the large vectors subtracted in double, so that a float only ever holds a distance within
    /// a quad. Only on the quads of the highest subdivision level, which are the ones craft stand on.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class TerrainPrecisionFixMod : MonoBehaviour
    {
        private const string HarmonyId = "com.github.lhervier.ksp.terrainprecisionfix";

        /// <summary>
        /// Largest correction applied, in metres. What is being corrected is a rounding error of a few
        /// centimetres; anything beyond a metre means the frame computed here is not the one the quad hangs
        /// from, and the quad is then left as stock builds it rather than moved somewhere else.
        /// </summary>
        private const double MaxCorrection = 1.0;

        // Set once every patch is in place. Each patch checks it, so that a partial install — one patch
        // applied, the next one refused — never mixes corrected vertices with an uncorrected quad origin.
        private static bool _active;

        // Private fields of stock classes, read and written the same way stock does. Bound once: the vertex
        // patch runs for every vertex of every quad.
        private static AccessTools.FieldRef<PQS, PQ> _buildQuad;
        private static AccessTools.FieldRef<PQS, int> _vertexIndex;
        private static AccessTools.FieldRef<PQ, Vector3d> _precisePosition;

        // The body a sphere belongs to, remembered between calls: the quads of one sphere are built in
        // bursts, so a single slot spares a scan of FlightGlobals.Bodies for every vertex.
        private static PQS _lastSphere;
        private static CelestialBody _lastBody;

        // Bodies already reported, so that each of these messages appears once per body and per session.
        private static readonly HashSet<CelestialBody> _announced = new HashSet<CelestialBody>();
        private static readonly HashSet<CelestialBody> _refused = new HashSet<CelestialBody>();

        // Trace only: over the quad being built, the largest distance between a vertex as placed here and
        // as stock would have placed it.
        private static float _maxVertexShift;

        private void Start()
        {
            Log.LoadLevel();
            try
            {
                _buildQuad = AccessTools.FieldRefAccess<PQS, PQ>("buildQuad");
                _vertexIndex = AccessTools.FieldRefAccess<PQS, int>("vertexIndex");
                _precisePosition = AccessTools.FieldRefAccess<PQ, Vector3d>("PrecisePosition");
                new Harmony(HarmonyId).PatchAll(typeof(TerrainPrecisionFixMod).Assembly);
                _active = true;
                Log.Info($"Version {typeof(TerrainPrecisionFixMod).Assembly.GetName().Version} installed,"
                    + $" log level {Log.Level}");
            }
            catch (Exception e)
            {
                // With _active left false, whatever patch did get applied does nothing: the terrain is
                // exactly stock.
                Log.Error($"Could not install the patches, the terrain is left as stock builds it: {e}");
            }
        }

        /// <summary>Whether the fix acts on this quad.</summary>
        private static bool AppliesTo(PQ quad)
        {
            PQS sphere = quad.sphereRoot;

            // Stock has two ways of placing vertices, and only the surface relative one goes through the
            // transforms this corrects.
            if (!_active || sphere == null || !sphere.surfaceRelativeQuads || sphere.LocalSpacePQStorage == null)
            {
                return false;
            }

            // Only the quads of the highest subdivision level are moved to this storage, which is not
            // attached to the body: a position given to them is kept as it is. Every other quad hangs from
            // the sphere, whose origin is the centre of the body, so Unity would store any position given to
            // it as a 600 km float again. Those are also the quads without a collider, as long as the
            // body's PQSMod_QuadMeshColliders.maxLevelOffset is 0.
            return quad.transform.parent == sphere.LocalSpacePQStorage.transform;
        }

        /// <summary>The body whose terrain this sphere is, or null when there is none.</summary>
        private static CelestialBody BodyOf(PQS sphere)
        {
            if (ReferenceEquals(sphere, _lastSphere))
            {
                return _lastBody;
            }
            if (FlightGlobals.Bodies == null)
            {
                return null;
            }
            foreach (CelestialBody candidate in FlightGlobals.Bodies)
            {
                if (candidate != null && ReferenceEquals(candidate.pqsController, sphere))
                {
                    _lastSphere = sphere;
                    _lastBody = candidate;
                    return candidate;
                }
            }
            return null;
        }

        /// <summary>
        /// The world position, in double precision, of a point given in the frame of a body's terrain
        /// sphere.
        /// </summary>
        private static Vector3d WorldPosition(CelestialBody body, Vector3d spherePosition)
        {
            // Of the frames that could be used here, this is the one the actual positions of the quads agree
            // with, to within the rounding being removed. Two look more obvious and are wrong:
            // - PQS.GetWorldPosition: measured, it misses by about 750 km on the body being flown over;
            // - the rotation and position of the body's transform: they are the float versions of these
            //   two values, and on a 600 km vector the float rotation alone is worth 36 mm, the very size
            //   of the defect.
            return body.rotation * spherePosition + body.position;
        }

        /// <summary>
        /// Whether moving a quad by <paramref name="correction"/> metres is a rounding correction rather
        /// than a move to a different place. Reports the first refusal on each body.
        /// </summary>
        private static bool IsRoundingCorrection(CelestialBody body, double correction)
        {
            if (correction <= MaxCorrection)
            {
                return true;
            }
            if (_refused.Add(body))
            {
                Log.Warning($"{body.bodyName}: a correction of {correction:0.000} m is too large to be a"
                    + " rounding error, the quads concerned are left as stock builds them");
            }
            return false;
        }

        // ==========================================================================
        // Where each vertex goes inside its quad
        // ==========================================================================

        /// <summary>
        /// Replaces the stock placement of a terrain vertex when the fix applies: the vertex lands at the
        /// same place, without being rounded at planet scale on the way.
        /// </summary>
        [HarmonyPatch(typeof(PQS), "BuildVertexSurfaceRelative")]
        private static class BuildVertexSurfaceRelativePatch
        {
            private static bool Prefix(PQS __instance, PQS.VertexBuildData data)
            {
                PQ quad = _active ? _buildQuad(__instance) : null;
                if (quad == null || !AppliesTo(quad))
                {
                    return true;
                }

                int index = _vertexIndex(__instance);
                if (quad.verts == null || PQS.verts == null
                    || index < 0 || index >= quad.verts.Length || index >= PQS.verts.Length)
                {
                    return true;
                }

                // The vertex is placed relative to where the quad patch below puts the quad, so it only
                // holds for a quad that patch accepts: same test, on the same numbers.
                CelestialBody body = BodyOf(__instance);
                if (body == null)
                {
                    return true;
                }
                Vector3d quadOrigin = WorldPosition(body, quad.positionPlanet);
                if (!IsRoundingCorrection(body, (quadOrigin - (Vector3d)quad.transform.position).magnitude))
                {
                    return true;
                }

                // The vertex relative to the centre of the body, in double. Stock keeps it as is, and so does
                // this: the normals are computed from it.
                Vector3d vertex = data.directionFromCenter * data.vertHeight;
                PQS.verts[index] = vertex;

                // The fix itself. The vertex and the quad origin are both doubles, in the same frame, and
                // their difference — a quad is a couple of kilometres wide at most — is the only thing a
                // float ever holds. Stock converts each of them to float first, 600 km from the centre where
                // the step is 62.5 mm, and subtracts afterwards.
                Vector3d offsetInQuad = vertex - quad.positionPlanet;

                // Into world orientation in double, then into the quad's own frame. The quad's rotation is a
                // float, which is harmless on a vector this short: a tenth of a millimetre at most.
                Vector3 localVertex = Quaternion.Inverse(quad.transform.rotation)
                    * (Vector3)(body.rotation * offsetInQuad);
                if (Log.IsTraceEnabled)
                {
                    TraceVertexShift(__instance, quad, body, index, vertex, localVertex);
                }
                quad.verts[index] = localVertex;
                return false;
            }
        }

        /// <summary>
        /// Trace only: compares a vertex with where stock would have put it, and logs the largest difference
        /// over the quad once its last vertex is done.
        /// </summary>
        private static void TraceVertexShift(PQS sphere, PQ quad, CelestialBody body, int index,
            Vector3d vertex, Vector3 localVertex)
        {
            if (index == 0)
            {
                _maxVertexShift = 0f;
            }
            Vector3 stock = quad.transform.InverseTransformPoint(sphere.transform.TransformPoint((Vector3)vertex));
            _maxVertexShift = Mathf.Max(_maxVertexShift, (stock - localVertex).magnitude);
            if (index == quad.verts.Length - 1)
            {
                Log.Trace($"{body.bodyName} quad '{quad.name}' (subdivision {quad.subdivision}):"
                    + $" vertices moved by up to {_maxVertexShift * 1000.0:0.00} mm within the quad");
            }
        }

        // ==========================================================================
        // Where the quad itself goes
        // ==========================================================================

        /// <summary>
        /// Moves a quad to the position its own double precision coordinates give, when the fix applies.
        /// Runs after the stock placement, which it overrides, and after the quad has been moved to its
        /// final parent.
        /// </summary>
        private static void PlaceQuad(PQ quad)
        {
            if (quad == null || !AppliesTo(quad))
            {
                return;
            }
            CelestialBody body = BodyOf(quad.sphereRoot);
            if (body == null)
            {
                return;
            }

            // positionPlanet is the quad's origin relative to the centre of the body, in double. Stock
            // assigns it, at full length, to the float localPosition of the quad's transform.
            Vector3d origin = WorldPosition(body, quad.positionPlanet);
            double correction = (origin - (Vector3d)quad.transform.position).magnitude;
            if (!IsRoundingCorrection(body, correction))
            {
                return;
            }

            // A world position, so relative to the floating origin: a short vector near the craft, which a
            // float holds precisely.
            quad.transform.position = origin;

            // Stock keeps the quad's world position in double alongside the transform, and later moves the
            // quad from it: it has to hold the corrected value too.
            _precisePosition(quad) = origin;

            if (_announced.Add(body))
            {
                Log.Info($"{body.bodyName}: terrain placed in double precision"
                    + $" (first quad corrected by {correction * 1000.0:0.00} mm)");
            }
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"{body.bodyName} quad '{quad.name}' (subdivision {quad.subdivision}):"
                    + $" origin moved by {correction * 1000.0:0.00} mm");
            }
        }

        [HarmonyPatch(typeof(PQ), "SetupQuad")]
        private static class SetupQuadPatch
        {
            private static void Postfix(PQ __instance)
            {
                PlaceQuad(__instance);
            }
        }

        [HarmonyPatch(typeof(PQ), "PreciseUpdateSubQuadsPosition")]
        private static class PreciseUpdateSubQuadsPositionPatch
        {
            private static void Postfix(PQ __instance)
            {
                PlaceQuad(__instance);
            }
        }
    }
}
