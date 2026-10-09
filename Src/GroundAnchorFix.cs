using HarmonyLib;
using UnityEngine;

namespace com.github.lhervier.ksp.terrainprecisionfix
{
    /// <summary>
    /// Keeps the stock ground anchor at the height it was placed at, load after load.
    ///
    /// Two stock behaviours move an anchored vessel at load, and an anchor riveted to the ground then holds it
    /// where they left it.
    ///
    /// The first one puts a landed vessel back on the ground (Vessel.CheckGroundCollision), at each load of a
    /// vessel made of a single part, and at the first load of any other: its origin ends at the height of its
    /// lowest collider point above that origin, taken as an absolute value. The ground anchor's collider
    /// stops 20.8 mm above the bottom of the anchor, its origin: once placed, the anchor rests with its origin
    /// 20.8 mm below the ground, and the first load puts it 20.8 mm above, 4.2 cm higher. Here the collider of
    /// the part's prefab reaches down to the origin, as those of the stock ground lights do.
    ///
    /// The second one, at every load, raises a landed vessel whose root is below the height the terrain is
    /// computed at (Vessel.getCorrectedLandedAltitude). The ground a vessel rests on is the terrain's collider,
    /// made of flat triangles between points at that height, and it can be centimetres below it: tens of
    /// centimetres at places on Kerbin. A vessel with wheels or legs has its root well above the ground and is
    /// never raised; an anchor has its origin on the ground and is raised by the whole gap, then riveted in the
    /// air. Here a vessel holding a stock ground anchor is not raised: it is loaded where it was saved.
    ///
    /// A third one is much smaller. At load, an anchor already riveted is riveted again one rendered frame
    /// after its vessel is unpacked (ModuleGroundPart.MakePartKinematic); when that frame lasts longer than a
    /// physics step, as it does below 50 frames per second, the anchor meanwhile moves freely for a step or
    /// two, by up to a millimetre or so, and is riveted there. Here such an anchor is frozen from the moment
    /// it is unpacked.
    /// </summary>
    internal static class GroundAnchorFix
    {
        // The stock ground anchor, Stamp-O-Tron. Ground parts of other mods are left alone.
        private const string PartName = "groundAnchor";

        // Below this, in metres, a collider already reaches the part's origin; and the vertices within this of
        // the lowest one make the bottom face that is lowered.
        private const float Tolerance = 0.001f;

        /// <summary>
        /// Applies the patch that keeps a vessel holding a stock ground anchor where it was saved, at load.
        /// Throws when the patch cannot be applied: vessels are then raised as stock raises them.
        /// </summary>
        public static void Install(Harmony harmony)
        {
            harmony.CreateClassProcessor(typeof(CorrectedLandedAltitudePatch)).Patch();
        }

        /// <summary>
        /// Applies the patch that keeps a stock ground anchor, riveted when its vessel was saved, frozen from
        /// the moment it is unpacked until KSP rivets it again. Throws when the patch cannot be applied: the
        /// anchor is then riveted as stock rivets it.
        /// </summary>
        public static void InstallRivet(Harmony harmony)
        {
            harmony.CreateClassProcessor(typeof(UnpackRivetPatch)).Patch();
        }

        /// <summary>Whether the vessel holds a stock ground anchor.</summary>
        private static bool HoldsAnchor(Vessel vessel)
        {
            if (vessel == null || vessel.parts == null)
            {
                return false;
            }
            for (int i = 0; i < vessel.parts.Count; i++)
            {
                Part part = vessel.parts[i];
                if (part != null && part.partInfo != null && part.partInfo.name == PartName)
                {
                    return true;
                }
            }
            return false;
        }

        // Vessel.Load calls this once, for a landed vessel whose parts are loaded, and moves the vessel to the
        // altitude it returns.
        [HarmonyPatch(typeof(Vessel), "getCorrectedLandedAltitude")]
        private static class CorrectedLandedAltitudePatch
        {
            private static bool Prefix(Vessel __instance, double lat, double lon, double alt, CelestialBody body,
                ref double __result)
            {
                if (!HoldsAnchor(__instance))
                {
                    return true;
                }
                if (Log.IsDebugEnabled && body != null && body.pqsController != null)
                {
                    // What stock would have done, for the record: the height the skipped method compares with,
                    // through the public overload (the one it calls is internal).
                    double terrain = body.pqsController.GetSurfaceHeight(body.GetRelSurfaceNVector(lat, lon))
                        - body.Radius;
                    Log.Debug($"Ground anchor load fix: '{__instance.GetDisplayName()}' loaded at its saved altitude"
                        + $" {alt:F4} m, which stock would have raised by {System.Math.Max(0.0, terrain - alt) * 1000.0:F1} mm");
                }
                __result = alt;
                return false;
            }
        }

        // Unpacking a ground part that was riveted when saved starts the coroutine that rivets it again
        // (MakePartKinematic). With a kinematic delay under a second, the stock anchor's 0, that coroutine
        // neither waits for the part to settle nor checks that it touches the ground: it rivets it at the
        // next rendered frame, wherever the physics steps in between have taken it.
        [HarmonyPatch(typeof(ModuleGroundPart), nameof(ModuleGroundPart.OnPartUnpack))]
        private static class UnpackRivetPatch
        {
            private static readonly AccessTools.FieldRef<ModuleGroundPart, bool> DeployedOnGround =
                AccessTools.FieldRefAccess<ModuleGroundPart, bool>("deployedOnGround");

            private static readonly AccessTools.FieldRef<ModuleCargoPart, bool> BeingAttached =
                AccessTools.FieldRefAccess<ModuleCargoPart, bool>("beingAttached");

            // Whether the part is being attached in EVA construction, which stock unpacks without riveting.
            private static void Prefix(ModuleGroundPart __instance, out bool __state)
            {
                __state = BeingAttached(__instance);
            }

            private static void Postfix(ModuleGroundPart __instance, bool __state)
            {
                Part part = __instance.part;
                if (__state || !DeployedOnGround(__instance) || __instance.kinematicDelay >= 1f
                    || part == null || part.partInfo == null || part.partInfo.name != PartName)
                {
                    return;
                }
                Rigidbody rb = part.Rigidbody;
                if (rb == null)
                {
                    return;
                }
                // The constraints the rivet sets, set one frame early; the rivet sets them again.
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.constraints = RigidbodyConstraints.FreezeAll;
                Log.Debug($"Ground anchor rivet fix: '{__instance.vessel?.GetDisplayName()}' frozen as it is unpacked,"
                    + " until it is riveted again");
            }
        }

        /// <summary>
        /// Lowers the collider of the stock ground anchor's prefab to the part's origin, so that every anchor
        /// created from then on rests on the ground where KSP puts it back at each load. Leaves the part as it
        /// is, and says so, when it is missing, shaped otherwise than the stock one, or when its collider
        /// already reaches the origin.
        /// </summary>
        public static void Apply()
        {
            AvailablePart info = PartLoader.getPartInfoByName(PartName);
            if (info == null || info.partPrefab == null)
            {
                Log.Info("Ground anchor model fix: no stock ground anchor in this game, nothing to fix");
                return;
            }
            Part prefab = info.partPrefab;

            // The stock anchor has a single solid collider, a mesh. Any other shape means another mod has
            // remodelled it: whatever it did is left as is.
            MeshCollider collider = null;
            foreach (Collider candidate in prefab.GetComponentsInChildren<Collider>(true))
            {
                if (candidate.isTrigger)
                {
                    continue;
                }
                MeshCollider mesh = candidate as MeshCollider;
                if (mesh == null || mesh.sharedMesh == null || collider != null)
                {
                    Log.Warning("Ground anchor model fix: the ground anchor's colliders are not those of the stock part,"
                        + " it is left as is");
                    return;
                }
                collider = mesh;
            }
            if (collider == null)
            {
                Log.Warning("Ground anchor model fix: the ground anchor has no collider, it is left as is");
                return;
            }

            // The vertices in part space, where the origin is the bottom of the anchor and y points up.
            Matrix4x4 toPart = prefab.transform.worldToLocalMatrix * collider.transform.localToWorldMatrix;
            Vector3[] vertices = collider.sharedMesh.vertices;
            float lowest = float.PositiveInfinity;
            for (int i = 0; i < vertices.Length; i++)
            {
                lowest = Mathf.Min(lowest, toPart.MultiplyPoint3x4(vertices[i]).y);
            }
            if (lowest <= Tolerance)
            {
                Log.Info($"Ground anchor model fix: the anchor's collider already reaches its origin ({lowest * 1000f:F1} mm),"
                    + " it is left as is");
                return;
            }

            // Bring the bottom face down to the origin. The collider is convex (KSP makes every mesh collider
            // of a part convex), so the rest of its hull follows.
            Matrix4x4 toCollider = toPart.inverse;
            int lowered = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 point = toPart.MultiplyPoint3x4(vertices[i]);
                if (point.y - lowest < Tolerance)
                {
                    point.y = 0f;
                    vertices[i] = toCollider.MultiplyPoint3x4(point);
                    lowered++;
                }
            }

            // A copy: the mesh read from the model may be shared, and only the anchor's collider is meant.
            Mesh copy = Object.Instantiate(collider.sharedMesh);
            copy.name = collider.sharedMesh.name + " (reaching the origin)";
            copy.vertices = vertices;
            copy.RecalculateBounds();
            collider.sharedMesh = copy;

            Log.Info($"Ground anchor model fix: the anchor's collider lowered by {lowest * 1000f:F1} mm to its origin");
            Log.Debug($"Ground anchor model fix: {lowered} of the {vertices.Length} vertices of '{collider.name}' lowered");
        }
    }
}
