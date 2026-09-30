using System.Collections.Generic;
using CommNet;
using HarmonyLib;
using UnityEngine;

namespace com.github.lhervier.ksp.terrainprecisionfix
{
    /// <summary>
    /// Places the statics — the buildings of the KSC, and whatever a mod such as Kerbal Konstructs places
    /// through a PQSCity — the same way at every load, where their own double precision coordinates say
    /// they are.
    ///
    /// A PQSCity hangs from its body's terrain sphere, whose origin is the centre of the body, at a
    /// localPosition hundreds of kilometres long: the same float rounding as the terrain, and for the same
    /// reason the same static comes back a few centimetres higher or lower at every load. Unlike a terrain
    /// quad, a static cannot be given a precise position while it hangs from the sphere: Unity would store
    /// it as a 600 km float again.
    ///
    /// So in flight, while a static is within reach of the craft, it is moved out of the sphere, next to
    /// the terrain quads that carry colliders, and given its world position in double precision. It is
    /// kept there, following its body, and put back under the sphere exactly as stock left it whenever it
    /// goes out of reach, while stock code that expects it under the sphere runs, and before every scene
    /// change: outside flight, and to any code looking for it outside those moments, it is where stock put
    /// it.
    /// </summary>
    internal static class StaticsFix
    {
        /// <summary>Name of the object statics hang from while they are out of their sphere.</summary>
        private const string StorageName = "TerrainPrecisionFix PQSCity storage";

        /// <summary>
        /// Added to the farthest a loaded craft can be from the active one, to reach the far end of the
        /// largest statics from their origin: the KSC is about 3 km across.
        /// </summary>
        private const float StaticExtent = 5000f;

        /// <summary>
        /// A static out of its sphere is put back only this much farther than where it was taken out, so
        /// that a craft hovering at the limit does not move it back and forth at every frame.
        /// </summary>
        private const float PutBackRangeFactor = 1.1f;

        /// <summary>A static out of its sphere, and what it takes to keep it where it belongs.</summary>
        private sealed class TakenOut
        {
            public PQSCity City;
            public Transform Transform;
            public PQS Sphere;
            public CelestialBody Body;

            // Its pose in the frame of the sphere, the one stock gives it: the position in double, the
            // rotation in float, which is harmless over the size of a static.
            public Vector3d SpherePosition;
            public Quaternion SphereRotation;

            // The pose this fix last gave it, as read back from its transform, and the pose of the body it
            // was worked out from.
            public Vector3 PlacedPosition;
            public Quaternion PlacedRotation;
            public Vector3d PlacedBodyPosition;
            public QuaternionD PlacedBodyRotation;
        }

        // Set once every patch is in place, including those that make the mods known to look for statics
        // under their sphere cope with statics that are not. Each patch checks it.
        private static bool _active;

        // Whether the flight scene is ready and not being left: the only time statics are moved out.
        private static bool _inFlight;

        private static AccessTools.FieldRef<PQSCity, Vector3d> _planetRelativePosition;

        // Every static seen, and those out of their sphere right now. Lists rather than sets: a destroyed
        // Unity object compares equal to any other, and a static can be destroyed at any time (a Kerbal
        // Konstructs group deleted in flight).
        private static readonly List<PQSCity> _known = new List<PQSCity>();
        private static readonly List<TakenOut> _out = new List<TakenOut>();
        private static readonly List<PQSCity> _scratch = new List<PQSCity>();

        // Bodies already reported, so that each of these messages appears once per body and per session.
        private static readonly HashSet<CelestialBody> _announced = new HashSet<CelestialBody>();
        private static readonly HashSet<CelestialBody> _refused = new HashSet<CelestialBody>();
        private static readonly HashSet<CelestialBody> _noStorage = new HashSet<CelestialBody>();

        /// <summary>
        /// Applies the statics patches, those that make other mods cope with a static out of its sphere,
        /// and turns the fix on once all of them are in place. Throws when a stock patch cannot be applied;
        /// leaves the fix off, with a warning, when a mod installed here is not the version its patch
        /// expects. Either way the statics are then left exactly where stock puts them.
        /// </summary>
        public static void Install(Harmony harmony)
        {
            _planetRelativePosition = AccessTools.FieldRefAccess<PQSCity, Vector3d>("planetRelativePosition");
            harmony.CreateClassProcessor(typeof(OrientatePatch)).Patch();
            harmony.CreateClassProcessor(typeof(StartPatch)).Patch();
            harmony.CreateClassProcessor(typeof(ResetCelestialBodyPatch)).Patch();
            harmony.CreateClassProcessor(typeof(SetupModsPatch)).Patch();
            harmony.CreateClassProcessor(typeof(CommNetHomeStartPatch)).Patch();
            harmony.CreateClassProcessor(typeof(DayNightSetupPatch)).Patch();
            harmony.CreateClassProcessor(typeof(SphereMovedPatch)).Patch();
            harmony.CreateClassProcessor(typeof(BodyRotatedPatch)).Patch();

            if (!KerbalKonstructsCompat.Install(harmony) || !KopernicusCompat.Install(harmony))
            {
                Log.Warning("Statics fix turned off: a mod installed here would not cope with a static out of"
                    + " its terrain sphere. The statics are left where stock places them");
                return;
            }
            _active = true;
        }

        // ==========================================================================
        // Scene changes and the per frame check, called by the addon
        // ==========================================================================

        /// <summary>The flight scene is ready: statics within reach may now be moved out.</summary>
        public static void OnFlightReady()
        {
            _inFlight = true;
        }

        /// <summary>
        /// A scene change is starting: every static goes back under its sphere, and stays there until the
        /// next flight scene is ready.
        /// </summary>
        public static void OnSceneChangeRequested()
        {
            _inFlight = false;
            PutAllBack();
        }

        /// <summary>
        /// Moves out of their sphere the statics that came within reach, and puts back those that went out
        /// of it. Called once per frame.
        /// </summary>
        public static void Update()
        {
            if (!_active || _known.Count == 0)
            {
                return;
            }
            if (!_inFlight || !HighLogic.LoadedSceneIsFlight)
            {
                PutAllBack();
                return;
            }

            // Taking a static out or putting it back changes neither list being read.
            _scratch.Clear();
            _scratch.AddRange(_known);
            foreach (PQSCity city in _scratch)
            {
                TakenOut record = Find(city);
                if (city == null)
                {
                    // Destroyed, and whatever hung from our storage with it.
                    _known.Remove(city);
                    if (record != null)
                    {
                        _out.Remove(record);
                    }
                    continue;
                }
                bool wanted = IsWithinReach(city, record != null);
                if (wanted && record == null)
                {
                    TakeOut(city);
                }
                else if (!wanted && record != null)
                {
                    PutBack(record);
                }
                else if (record != null)
                {
                    // Normally already done when the body moved; this catches any other way it did.
                    Place(record, false);
                }
            }
        }

        // ==========================================================================
        // Taking a static out of its sphere, keeping it placed, putting it back
        // ==========================================================================

        /// <summary>Whether a static should be out of its sphere, given whether it already is.</summary>
        private static bool IsWithinReach(PQSCity city, bool isOut)
        {
            if (!_inFlight || !HighLogic.LoadedSceneIsFlight)
            {
                return false;
            }
            PQS sphere = city.sphere;
            if (sphere == null || !sphere.isActive || sphere.target == null || !sphere.gameObject.activeInHierarchy)
            {
                return false;
            }
            if (!city.gameObject.activeSelf)
            {
                return false;
            }

            // Only the statics that hang directly from their sphere, as those of stock and of Kerbal
            // Konstructs do: the localPosition of any other is not in the frame of the sphere.
            if (!isOut && city.transform.parent != sphere.transform)
            {
                return false;
            }

            float range = ReachRange();
            if (isOut)
            {
                range *= PutBackRangeFactor;
            }
            return (sphere.target.position - city.transform.position).sqrMagnitude < range * range;
        }

        /// <summary>
        /// How close to the craft a static has to be for a loaded craft to possibly touch it, in metres.
        /// </summary>
        private static float ReachRange()
        {
            // Loaded craft are all within their unload range of the active one, the largest of which is
            // that of a flying craft: 22.5 km by default.
            VesselRanges ranges = PhysicsGlobals.Instance != null
                ? PhysicsGlobals.Instance.VesselRangesDefault
                : new VesselRanges();
            float unload = Mathf.Max(ranges.prelaunch.unload, ranges.landed.unload, ranges.splashed.unload,
                ranges.flying.unload, ranges.subOrbital.unload, ranges.orbit.unload);
            return unload + StaticExtent;
        }

        /// <summary>
        /// Moves a static out of its sphere, to where its own double precision coordinates say it is. Leaves
        /// it under its sphere when this cannot be done safely.
        /// </summary>
        private static void TakeOut(PQSCity city)
        {
            PQS sphere = city.sphere;
            CelestialBody body = PlanetFrame.BodyOf(sphere);
            if (body == null)
            {
                return;
            }
            Transform storage = StorageFor(sphere, body);
            if (storage == null)
            {
                return;
            }

            // Stock sets the localPosition from planetRelativePosition, in double, whenever it places the
            // static; when it does not, the localPosition of the prefab is all there is.
            Transform transform = city.transform;
            Vector3 localPosition = transform.localPosition;
            Vector3d planetRelativePosition = _planetRelativePosition(city);
            Vector3d spherePosition = ((Vector3)planetRelativePosition).Equals(localPosition)
                ? planetRelativePosition
                : (Vector3d)localPosition;

            double correction = (PlanetFrame.WorldPosition(body, spherePosition) - (Vector3d)transform.position).magnitude;
            if (!IsRoundingCorrection(body, spherePosition.magnitude, correction))
            {
                return;
            }

            TakenOut record = new TakenOut
            {
                City = city,
                Transform = transform,
                Sphere = sphere,
                Body = body,
                SpherePosition = spherePosition,
                SphereRotation = transform.localRotation
            };

            // Without keeping the world pose: it is given right after, in double.
            transform.SetParent(storage, false);
            _out.Add(record);
            Place(record, true);

            if (_announced.Add(body))
            {
                Log.Info($"{body.bodyName}: statics placed in double precision"
                    + $" (first one, '{city.name}', corrected by {correction * 1000.0:0.00} mm)");
            }
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"{body.bodyName} static '{city.name}': out of its sphere,"
                    + $" corrected by {correction * 1000.0:0.00} mm");
            }
        }

        /// <summary>
        /// Gives a static out of its sphere the world pose its coordinates in the sphere and the current
        /// pose of its body make. Unless <paramref name="force"/>, does nothing when neither changed since.
        /// </summary>
        private static void Place(TakenOut record, bool force)
        {
            Transform transform = record.Transform;
            if (transform == null)
            {
                return;
            }
            CelestialBody body = record.Body;
            if (!force)
            {
                bool movedByOthers = IsMovedByOthers(record);
                if (!movedByOthers && body.position == record.PlacedBodyPosition
                    && body.rotation == record.PlacedBodyRotation)
                {
                    return;
                }
                if (movedByOthers)
                {
                    AdoptPose(record);
                }
            }

            // A world position, so relative to the floating origin: a short vector near the craft, which a
            // float holds precisely.
            Vector3d position = PlanetFrame.WorldPosition(body, record.SpherePosition);
            Quaternion rotation = (Quaternion)body.rotation * record.SphereRotation;
            transform.SetPositionAndRotation(position, rotation);

            // Read back rather than kept as given: that is what the next comparison will read.
            record.PlacedPosition = transform.position;
            record.PlacedRotation = transform.rotation;
            record.PlacedBodyPosition = body.position;
            record.PlacedBodyRotation = body.rotation;
        }

        /// <summary>Whether something else moved a static out of its sphere since this fix last placed it.</summary>
        private static bool IsMovedByOthers(TakenOut record)
        {
            return !record.Transform.position.Equals(record.PlacedPosition)
                || !record.Transform.rotation.Equals(record.PlacedRotation);
        }

        /// <summary>
        /// Takes the current world pose of a static, which something else gave it, as its new pose in the
        /// frame of its sphere: it stays where it was put, relative to its body.
        /// </summary>
        private static void AdoptPose(TakenOut record)
        {
            // Whoever moved it did so in the world frame this fix last placed it in.
            QuaternionD inverseBodyRotation = QuaternionD.Inverse(record.PlacedBodyRotation);
            record.SpherePosition = inverseBodyRotation * ((Vector3d)record.Transform.position - record.PlacedBodyPosition);
            record.SphereRotation = (Quaternion)inverseBodyRotation * record.Transform.rotation;
        }

        /// <summary>
        /// Puts a static back under its sphere, with the local pose stock gave it, or the one matching
        /// wherever something else moved it since.
        /// </summary>
        private static void PutBack(TakenOut record)
        {
            _out.Remove(record);
            Transform transform = record.Transform;
            if (transform == null || record.Sphere == null)
            {
                return;
            }
            if (IsMovedByOthers(record))
            {
                AdoptPose(record);
            }
            transform.SetParent(record.Sphere.transform, false);
            transform.localPosition = (Vector3)record.SpherePosition;
            transform.localRotation = record.SphereRotation;
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"{record.Body.bodyName} static '{record.City.name}': back under its sphere");
            }
        }

        /// <summary>Puts every static out of its sphere back under it.</summary>
        private static void PutAllBack()
        {
            while (_out.Count > 0)
            {
                PutBack(_out[_out.Count - 1]);
            }
        }

        /// <summary>The record of a static out of its sphere, or null when it is under it.</summary>
        private static TakenOut Find(PQSCity city)
        {
            foreach (TakenOut record in _out)
            {
                if (ReferenceEquals(record.City, city))
                {
                    return record;
                }
            }
            return null;
        }

        /// <summary>
        /// The object statics of this sphere hang from while they are out of it, or null when there is no
        /// safe one.
        /// </summary>
        private static Transform StorageFor(PQS sphere, CelestialBody body)
        {
            // Next to the quads of the highest subdivision level: stock moves them out of the sphere to the
            // same place, which is not attached to the body, so a world position given there is kept as it
            // is. Stock falls back to the sphere itself when there is no such place, and a static would then
            // still hang from the body.
            Transform localSpace = sphere.LocalSpacePQStorage != null ? sphere.LocalSpacePQStorage.transform.parent : null;
            if (localSpace == null || localSpace.IsChildOf(sphere.transform) || localSpace.IsChildOf(body.transform))
            {
                ReportNoStorage(body, "no place out of the sphere to put them");
                return null;
            }

            Transform storage = localSpace.Find(StorageName);
            if (storage == null)
            {
                storage = new GameObject(StorageName).transform;
                storage.SetParent(localSpace, false);
            }

            // Statics keep their localScale when they change parent, and mods set it: it only means the same
            // thing under both parents if they have the same scale.
            if ((storage.lossyScale - sphere.transform.lossyScale).sqrMagnitude > 1e-12f)
            {
                ReportNoStorage(body, $"the sphere has a scale of {sphere.transform.lossyScale}"
                    + $" and the place out of it {storage.lossyScale}");
                return null;
            }
            return storage;
        }

        private static void ReportNoStorage(CelestialBody body, string reason)
        {
            if (_noStorage.Add(body))
            {
                Log.Warning($"{body.bodyName}: {reason}, the statics are left where stock places them");
            }
        }

        /// <summary>
        /// Whether moving a static <paramref name="distance"/> metres from the centre of its body by
        /// <paramref name="correction"/> metres is a rounding correction rather than a move to a different
        /// place. Reports the first refusal on each body.
        /// </summary>
        private static bool IsRoundingCorrection(CelestialBody body, double distance, double correction)
        {
            double maxCorrection;
            if (PlanetFrame.IsRoundingCorrection(distance, correction, out maxCorrection))
            {
                return true;
            }
            if (_refused.Add(body))
            {
                Log.Warning($"{body.bodyName}: a correction of {correction:0.000} m is more than"
                    + $" {PlanetFrame.MaxCorrectionInFloatSteps:0} float steps ({maxCorrection:0.000} m at that"
                    + " distance), too large to be a rounding error, the statics concerned are left where"
                    + " stock places them");
            }
            return false;
        }

        // ==========================================================================
        // For the patches of stock code, and of mods, that expect statics under their sphere
        // ==========================================================================

        /// <summary>
        /// Puts a static back under its sphere, for the time code that expects it there runs. Returns
        /// whether it was out, to be handed to <see cref="GiveBack"/>.
        /// </summary>
        private static bool Borrow(PQSCity city)
        {
            TakenOut record = Find(city);
            if (record == null)
            {
                return false;
            }
            PutBack(record);
            return true;
        }

        /// <summary>
        /// Takes a static borrowed by <see cref="Borrow"/> out of its sphere again, from the pose stock code
        /// just gave it.
        /// </summary>
        private static void GiveBack(PQSCity city, bool wasOut)
        {
            if (wasOut && city != null && IsWithinReach(city, false))
            {
                TakeOut(city);
            }
        }

        /// <summary>The static out of its sphere that <paramref name="transform"/> belongs to, or null.</summary>
        private static PQSCity OutAncestor(Transform transform)
        {
            for (Transform current = transform; current != null; current = current.parent)
            {
                foreach (TakenOut record in _out)
                {
                    if (ReferenceEquals(record.Transform, current))
                    {
                        return record.City;
                    }
                }
            }
            return null;
        }

        /// <summary>Remembers a static, so that it can be moved out when within reach.</summary>
        private static void Register(PQSCity city)
        {
            if (!_known.Contains(city))
            {
                _known.Add(city);
            }
        }

        /// <summary>
        /// The position of <paramref name="transform"/> in the frame of the terrain sphere it belongs to:
        /// its localPosition while it hangs from the sphere, worked out from its world position when it is a
        /// static out of it.
        /// </summary>
        public static Vector3 SphereLocalPosition(Transform transform)
        {
            if (_active)
            {
                foreach (TakenOut record in _out)
                {
                    if (ReferenceEquals(record.Transform, transform))
                    {
                        return (Vector3)PlanetFrame.SpherePosition(record.Body, transform.position);
                    }
                }
            }
            return transform.localPosition;
        }

        /// <summary>
        /// The statics under <paramref name="root"/>, as GetComponentsInChildren returns them, plus those out
        /// of a sphere that hangs from it.
        /// </summary>
        public static PQSCity[] CitiesUnder(Component root, bool includeInactive)
        {
            PQSCity[] found = root.GetComponentsInChildren<PQSCity>(includeInactive);
            if (!_active || _out.Count == 0)
            {
                return found;
            }
            List<PQSCity> all = null;
            foreach (TakenOut record in _out)
            {
                if (record.Transform == null || record.Sphere == null
                    || !record.Sphere.transform.IsChildOf(root.transform))
                {
                    continue;
                }
                // Under the sphere, it would be inactive along with it.
                if (!includeInactive && !record.Sphere.gameObject.activeInHierarchy)
                {
                    continue;
                }
                if (all == null)
                {
                    all = new List<PQSCity>(found);
                }
                all.AddRange(record.Transform.GetComponentsInChildren<PQSCity>(includeInactive));
            }
            return all == null ? found : all.ToArray();
        }

        // ==========================================================================
        // Stock code that finds a static's frame or body through the hierarchy
        // ==========================================================================

        /// <summary>
        /// Places the static from its coordinates in the sphere: it writes its localPosition and reads its
        /// body from its parents, so it runs with the static under its sphere. Also where statics become
        /// known: every one of them goes through it at least once, from its Start.
        /// </summary>
        [HarmonyPatch(typeof(PQSCity), nameof(PQSCity.Orientate))]
        private static class OrientatePatch
        {
            private static void Prefix(PQSCity __instance, out bool __state)
            {
                __state = _active && Borrow(__instance);
            }

            private static void Postfix(PQSCity __instance, bool __state)
            {
                if (_active)
                {
                    Register(__instance);
                    GiveBack(__instance, __state);
                }
            }
        }

        /// <summary>Reads the static's body from its parents.</summary>
        [HarmonyPatch(typeof(PQSCity), "Start")]
        private static class StartPatch
        {
            private static void Prefix(PQSCity __instance, out bool __state)
            {
                __state = _active && Borrow(__instance);
            }

            private static void Postfix(PQSCity __instance, bool __state)
            {
                GiveBack(__instance, __state);
            }
        }

        /// <summary>Reads the static's body from its parents.</summary>
        [HarmonyPatch(typeof(PQSCity), nameof(PQSCity.ResetCelestialBody))]
        private static class ResetCelestialBodyPatch
        {
            private static void Prefix(PQSCity __instance, out bool __state)
            {
                __state = _active && Borrow(__instance);
            }

            private static void Postfix(PQSCity __instance, bool __state)
            {
                GiveBack(__instance, __state);
            }
        }

        /// <summary>
        /// Lists the mods of a sphere, statics included, from its children: a static out of the sphere
        /// would drop out of the list, and no longer be placed nor shown.
        /// </summary>
        [HarmonyPatch(typeof(PQS), "SetupMods")]
        private static class SetupModsPatch
        {
            private static void Prefix(PQS __instance, out List<PQSCity> __state)
            {
                __state = null;
                if (!_active)
                {
                    return;
                }
                for (int i = _out.Count - 1; i >= 0; i--)
                {
                    TakenOut record = _out[i];
                    if (ReferenceEquals(record.Sphere, __instance))
                    {
                        if (__state == null)
                        {
                            __state = new List<PQSCity>();
                        }
                        __state.Add(record.City);
                        PutBack(record);
                    }
                }
            }

            private static void Postfix(List<PQSCity> __state)
            {
                if (__state == null)
                {
                    return;
                }
                foreach (PQSCity city in __state)
                {
                    GiveBack(city, true);
                }
            }
        }

        /// <summary>
        /// Reads the body of a ground station from its parents, once, and a ground station can be part of a
        /// static (the KSC's).
        /// </summary>
        [HarmonyPatch(typeof(CommNetHome), "Start")]
        private static class CommNetHomeStartPatch
        {
            private static void Prefix(CommNetHome __instance, out PQSCity __state)
            {
                __state = null;
                if (_active)
                {
                    __state = OutAncestor(__instance.transform);
                    Borrow(__state);
                }
            }

            private static void Postfix(PQSCity __state)
            {
                GiveBack(__state, __state != null);
            }
        }

        /// <summary>
        /// Reads the body of a day and night switch from the parents of its first object, once, and those
        /// objects can be part of a static.
        /// </summary>
        [HarmonyPatch(typeof(DayNightGameObjectSwitch), "Setup")]
        private static class DayNightSetupPatch
        {
            private static void Prefix(DayNightGameObjectSwitch __instance, out PQSCity __state)
            {
                __state = null;
                if (_active && __instance.objects != null && __instance.objects.Length > 0
                    && __instance.objects[0] != null)
                {
                    __state = OutAncestor(__instance.objects[0].transform);
                    Borrow(__state);
                }
            }

            private static void Postfix(PQSCity __state)
            {
                GiveBack(__state, __state != null);
            }
        }

        // ==========================================================================
        // Following the body
        // ==========================================================================

        /// <summary>
        /// The sphere moved, with its body, as it does at every shift of the floating origin: stock moves
        /// the quads out of the sphere in the same call, and the statics out of it have to follow before
        /// anything else, physics included, sees them.
        /// </summary>
        [HarmonyPatch(typeof(PQS), nameof(PQS.PrecisePosition), MethodType.Setter)]
        private static class SphereMovedPatch
        {
            private static void Postfix(PQS __instance)
            {
                if (!_active || _out.Count == 0)
                {
                    return;
                }
                foreach (TakenOut record in _out)
                {
                    if (ReferenceEquals(record.Sphere, __instance))
                    {
                        Place(record, false);
                    }
                }
            }
        }

        /// <summary>
        /// The body may have turned, as it does at every frame when the craft is too high for the world to
        /// turn with the body instead.
        /// </summary>
        [HarmonyPatch(typeof(CelestialBody), nameof(CelestialBody.CBUpdate))]
        private static class BodyRotatedPatch
        {
            private static void Postfix(CelestialBody __instance)
            {
                if (!_active || _out.Count == 0)
                {
                    return;
                }
                foreach (TakenOut record in _out)
                {
                    if (ReferenceEquals(record.Body, __instance))
                    {
                        Place(record, false);
                    }
                }
            }
        }
    }
}
