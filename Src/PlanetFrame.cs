using System;
using UnityEngine;

namespace com.github.lhervier.ksp.terrainprecisionfix
{
    /// <summary>
    /// The frame of a body's terrain sphere, worked out in double precision, and the limits of what a float
    /// can hold in it. Shared by the terrain and the statics fixes.
    /// </summary>
    internal static class PlanetFrame
    {
        /// <summary>
        /// Largest correction applied, in float steps at the distance of the corrected object from the
        /// centre of its body. What is being corrected is a rounding error of a few steps; anything larger
        /// means the frame computed here is not the one the object hangs from, and the object is then left
        /// as stock places it rather than moved somewhere else. Sixteen steps is 1 m on Kerbin, and grows
        /// with the body as the rounding does.
        /// </summary>
        public const double MaxCorrectionInFloatSteps = 16.0;

        // The body a sphere belongs to, remembered between calls: the quads of one sphere are built in
        // bursts, so a single slot spares a scan of FlightGlobals.Bodies for every vertex.
        private static PQS _lastSphere;
        private static CelestialBody _lastBody;

        /// <summary>The body whose terrain this sphere is, or null when there is none.</summary>
        public static CelestialBody BodyOf(PQS sphere)
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
        public static Vector3d WorldPosition(CelestialBody body, Vector3d spherePosition)
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
        /// The position in the frame of a body's terrain sphere, in double precision, of a world position.
        /// The inverse of <see cref="WorldPosition"/>.
        /// </summary>
        public static Vector3d SpherePosition(CelestialBody body, Vector3d worldPosition)
        {
            return QuaternionD.Inverse(body.rotation) * (worldPosition - body.position);
        }

        /// <summary>
        /// Whether moving an object <paramref name="distance"/> metres from the centre of its body by
        /// <paramref name="correction"/> metres is a rounding correction rather than a move to a different
        /// place. <paramref name="maxCorrection"/> receives the limit that applied.
        /// </summary>
        public static bool IsRoundingCorrection(double distance, double correction, out double maxCorrection)
        {
            // The rounding being removed happens on the object's position relative to the centre of the
            // body, so its size is set by the float step at that distance.
            maxCorrection = MaxCorrectionInFloatSteps * FloatStep(distance);
            return correction <= maxCorrection;
        }

        /// <summary>The gap between two consecutive float values around <paramref name="distance"/>, in metres.</summary>
        public static double FloatStep(double distance)
        {
            // A float has 24 significant bits: between 2^n and 2^(n+1), consecutive values are 2^(n-23)
            // apart. Below a metre the value no longer matters here, and the logarithm would not be defined
            // at zero.
            int exponent = (int)Math.Floor(Math.Log(Math.Max(distance, 1.0), 2.0));
            return Math.Pow(2.0, exponent - 23);
        }
    }
}
