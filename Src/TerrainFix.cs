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
    internal static class TerrainFix
    {
        // Set once every patch is in place. Each patch checks it, so that a partial install — one patch
        // applied, the next one refused — never mixes corrected vertices with an uncorrected quad origin.
        private static bool _active;

        // Private fields of stock classes, read and written the same way stock does. Bound once: the vertex
        // patch runs for every vertex of every quad.
        private static AccessTools.FieldRef<PQS, PQ> _buildQuad;
        private static AccessTools.FieldRef<PQS, int> _vertexIndex;
        private static AccessTools.FieldRef<PQ, Vector3d> _precisePosition;

        // Bodies already reported, so that each of these messages appears once per body and per session.
        private static readonly HashSet<CelestialBody> _announced = new HashSet<CelestialBody>();
        private static readonly HashSet<CelestialBody> _refused = new HashSet<CelestialBody>();

        // Trace only: over the quad being built, the largest distance between a vertex as placed here and
        // as stock would have placed it.
        private static float _maxVertexShift;

        /// <summary>
        /// Applies the terrain patches, and turns them on once all of them are in place. Throws when one of
        /// them cannot be applied: the terrain is then left exactly as stock builds it.
        /// </summary>
        public static void Install(Harmony harmony)
        {
            _buildQuad = AccessTools.FieldRefAccess<PQS, PQ>("buildQuad");
            _vertexIndex = AccessTools.FieldRefAccess<PQS, int>("vertexIndex");
            _precisePosition = AccessTools.FieldRefAccess<PQ, Vector3d>("PrecisePosition");
            harmony.CreateClassProcessor(typeof(BuildVertexSurfaceRelativePatch)).Patch();
            harmony.CreateClassProcessor(typeof(BuildQuadPatch)).Patch();
            harmony.CreateClassProcessor(typeof(SetupQuadPatch)).Patch();
            harmony.CreateClassProcessor(typeof(PreciseUpdateSubQuadsPositionPatch)).Patch();
            _active = true;
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
            // body's PQSMod_QuadMeshColliders.maxLevelOffset is 0, which it is on every stock body, read
            // in game.
            return quad.transform.parent == sphere.LocalSpacePQStorage.transform;
        }

        /// <summary>
        /// Whether moving <paramref name="quad"/> by <paramref name="correction"/> metres is a rounding
        /// correction rather than a move to a different place. Reports the first refusal on each body.
        /// </summary>
        private static bool IsRoundingCorrection(CelestialBody body, PQ quad, double correction)
        {
            double maxCorrection;
            if (PlanetFrame.IsRoundingCorrection(quad.positionPlanet.magnitude, correction, out maxCorrection))
            {
                return true;
            }
            if (_refused.Add(body))
            {
                Log.Warning($"{body.bodyName}: a correction of {correction:0.000} m is more than"
                    + $" {PlanetFrame.MaxCorrectionInFloatSteps:0} float steps ({maxCorrection:0.000} m at that"
                    + " distance), too large to be a rounding error, the quads concerned are left as stock"
                    + " builds them");
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
                if (!_active)
                {
                    return true;
                }
                PQ quad = _buildQuad(__instance);
                if (quad == null)
                {
                    return true;
                }

                // The vertex relative to the centre of the body, in double. Stock keeps it as is, and so
                // does this: the normals are computed from it.
                Vector3d vertex = data.directionFromCenter * data.vertHeight;
                return !PlaceVertex(__instance, quad, _vertexIndex(__instance), vertex);
            }
        }

        /// <summary>
        /// Puts one terrain vertex where its own double precision coordinates say it is, inside its quad.
        /// Returns whether it did: a vertex the fix does not apply to is left to stock, untouched.
        /// </summary>
        private static bool PlaceVertex(PQS sphere, PQ quad, int index, Vector3d vertex)
        {
            // Everything that depends on the quad rather than on the vertex is worked out once and reused
            // for its couple of hundred vertices. Measured: without this, a vertex costs three times what
            // stock spends on it, almost all of it in reading Unity transforms over and over.
            if (!ReferenceEquals(quad, _contextQuad))
            {
                BuildQuadContext(sphere, quad);
            }
            if (!_contextApplies)
            {
                return false;
            }
            if (quad.verts == null || PQS.verts == null
                || index < 0 || index >= quad.verts.Length || index >= PQS.verts.Length)
            {
                return false;
            }

            PQS.verts[index] = vertex;

            // The fix itself. The vertex and the quad origin are both doubles, in the same frame, and their
            // difference — a quad is a couple of kilometres wide at most — is the only thing a float ever
            // holds. Stock converts each of them to float first, 600 km from the centre where the step is
            // 62.5 mm, and subtracts afterwards.
            Vector3d offsetInQuad = vertex - _contextPositionPlanet;

            // Into world orientation in double, then into the quad's own frame. The quad's rotation is a
            // float, which is harmless on a vector this short: a tenth of a millimetre at most.
            Vector3 localVertex = _contextInverseQuadRotation * (Vector3)(_contextBodyRotation * offsetInQuad);
            if (Log.IsTraceEnabled)
            {
                TraceVertexShift(sphere, quad, _contextBody, index, vertex, localVertex);
            }
            quad.verts[index] = localVertex;
            return true;
        }

        // What placing a vertex needs to know about the quad it belongs to. Only ever read after
        // BuildQuadContext has run for that same quad.
        private static PQ _contextQuad;
        private static bool _contextApplies;
        private static CelestialBody _contextBody;
        private static Vector3d _contextPositionPlanet;
        private static QuaternionD _contextBodyRotation;
        private static Quaternion _contextInverseQuadRotation;

        /// <summary>
        /// Works out whether the fix applies to this quad, and the frame its vertices are placed in.
        /// </summary>
        private static void BuildQuadContext(PQS sphere, PQ quad)
        {
            _contextQuad = quad;
            _contextApplies = false;

            if (!AppliesTo(quad))
            {
                return;
            }

            // Vertices are placed relative to where the quad patch below puts the quad, so they only hold
            // for a quad that patch accepts: same test, on the same numbers.
            CelestialBody body = PlanetFrame.BodyOf(sphere);
            if (body == null)
            {
                return;
            }
            Vector3d quadOrigin = PlanetFrame.WorldPosition(body, quad.positionPlanet);
            if (!IsRoundingCorrection(body, quad, (quadOrigin - (Vector3d)quad.transform.position).magnitude))
            {
                return;
            }

            _contextBody = body;
            _contextPositionPlanet = quad.positionPlanet;
            _contextBodyRotation = body.rotation;
            _contextInverseQuadRotation = Quaternion.Inverse(quad.transform.rotation);
            _contextApplies = true;
        }

        /// <summary>Forgets what was worked out for a quad, so that the next vertex works it out again.</summary>
        private static void ForgetQuadContext()
        {
            _contextQuad = null;
        }

        /// <summary>
        /// A quad can be rebuilt after having been moved, so whatever was worked out for it last time has
        /// to be worked out again.
        /// </summary>
        [HarmonyPatch(typeof(PQS), "BuildQuad")]
        private static class BuildQuadPatch
        {
            private static void Prefix()
            {
                ForgetQuadContext();
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
            CelestialBody body = PlanetFrame.BodyOf(quad.sphereRoot);
            if (body == null)
            {
                return;
            }

            // positionPlanet is the quad's origin relative to the centre of the body, in double. Stock
            // assigns it, at full length, to the float localPosition of the quad's transform.
            Vector3d origin = PlanetFrame.WorldPosition(body, quad.positionPlanet);
            double correction = (origin - (Vector3d)quad.transform.position).magnitude;
            if (!IsRoundingCorrection(body, quad, correction))
            {
                return;
            }

            // A world position, so relative to the floating origin: a short vector near the craft, which a
            // float holds precisely.
            quad.transform.position = origin;

            // The quad just moved, so anything worked out from its transform is stale.
            ForgetQuadContext();

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
