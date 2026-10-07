using UnityEngine;

namespace com.github.lhervier.ksp.terrainprecisionfix
{
    /// <summary>
    /// Keeps the stock ground anchor at the height it was placed at, load after load.
    ///
    /// At each load of a landed vessel whose root carries a ModuleGroundPart, stock KSP puts the vessel back
    /// on the ground (Vessel.CheckGroundCollision): its origin ends at the height of its lowest collider
    /// point above that origin, taken as an absolute value. The ground anchor's collider stops 20.8 mm above
    /// the bottom of the anchor, its origin: once placed, the anchor rests with its origin 20.8 mm below the
    /// ground, and the first load puts it 20.8 mm above, 4.2 cm higher. Here the collider of the part's
    /// prefab reaches down to the origin, as those of the stock ground lights do: placed or loaded, the
    /// anchor then rests with its origin on the ground.
    /// </summary>
    internal static class GroundAnchorFix
    {
        // The stock ground anchor, Stamp-O-Tron. Ground parts of other mods are left alone.
        private const string PartName = "groundAnchor";

        // Below this, in metres, a collider already reaches the part's origin; and the vertices within this of
        // the lowest one make the bottom face that is lowered.
        private const float Tolerance = 0.001f;

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
                Log.Info("Ground anchor fix: no stock ground anchor in this game, nothing to fix");
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
                    Log.Warning("Ground anchor fix: the ground anchor's colliders are not those of the stock part,"
                        + " it is left as is");
                    return;
                }
                collider = mesh;
            }
            if (collider == null)
            {
                Log.Warning("Ground anchor fix: the ground anchor has no collider, it is left as is");
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
                Log.Info($"Ground anchor fix: the anchor's collider already reaches its origin ({lowest * 1000f:F1} mm),"
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

            Log.Info($"Ground anchor fix: the anchor's collider lowered by {lowest * 1000f:F1} mm to its origin");
            Log.Debug($"Ground anchor fix: {lowered} of the {vertices.Length} vertices of '{collider.name}' lowered");
        }
    }
}
