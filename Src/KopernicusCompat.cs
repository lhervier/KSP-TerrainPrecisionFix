using System;
using System.Reflection;
using HarmonyLib;

namespace com.github.lhervier.ksp.terrainprecisionfix
{
    /// <summary>
    /// Keeps Kopernicus' flag fix from running while the KSC is out of its terrain sphere.
    ///
    /// Kopernicus fixes the KSC's flags a few frames after every scene opens, and whenever a facility is
    /// upgraded or repaired. It looks the KSC up among the PQSCity under the home body's sphere, then uses
    /// it without checking it was found. In flight, a facility is upgraded when a mission of the Making
    /// History expansion spawns a craft; the KSC may then be out of its sphere, the lookup finds nothing,
    /// and the fix throws. The flags do not need it there: the glitch it fixes comes from the KSC hanging
    /// from the sphere, and a KSC out of it, placed in double precision, keeps them steady.
    ///
    /// The change this patch stands for, in Kopernicus.RuntimeUtility.RuntimeUtility.FixFlags(), stops
    /// when the KSC is not found, as its first version did. Without this fix moving statics, the KSC is
    /// always under its sphere, and the flag fix runs as unpatched.
    ///
    /// Kopernicus' other lookups of the KSC under the home body's sphere run when the main menu, the space
    /// centre, the tracking station or an editor is entered, and statics are back under their sphere before
    /// any scene change; the rest of its code about PQSCity runs while loading.
    /// </summary>
    internal static class KopernicusCompat
    {
        private const string RuntimeUtilityTypeName = "Kopernicus.RuntimeUtility.RuntimeUtility";

        // The name Kopernicus looks the KSC up by.
        private const string KscName = "KSC";

        /// <summary>
        /// Applies the patch when Kopernicus is installed and <paramref name="enabled"/>. Returns false when
        /// it is installed, the patch is wanted, and its code is not the one the patch expects; true
        /// otherwise, including when the patch is turned off, which is reported as making the flag fix throw.
        /// </summary>
        public static bool Install(Harmony harmony, bool enabled)
        {
            if (AccessTools.TypeByName(RuntimeUtilityTypeName) == null)
            {
                return true;
            }
            if (!enabled)
            {
                Log.Warning("Kopernicus: its patch is turned off in the settings while the statics fix is on."
                    + " Its flag fix will throw when a facility is upgraded in flight near the KSC, as a mission"
                    + " of the Making History expansion does when it spawns a craft there: an error in the log,"
                    + " the statics fix keeping the flags steady there");
                return true;
            }
            try
            {
                harmony.CreateClassProcessor(typeof(FixFlagsPatch)).Patch();
            }
            catch (Exception e)
            {
                Log.Warning($"Kopernicus: could not patch its flag fix: {e.Message}");
                return false;
            }
            Log.Info("Kopernicus: flag fix skipped while the KSC is out of its sphere");
            return true;
        }

        [HarmonyPatch]
        private static class FixFlagsPatch
        {
            private static MethodBase TargetMethod()
            {
                // The overload without parameters: the two others, event handlers, call it. Null, and the
                // patch fails, if Kopernicus no longer has it.
                return AccessTools.Method(AccessTools.TypeByName(RuntimeUtilityTypeName), "FixFlags", Type.EmptyTypes);
            }

            /// <summary>Runs the flag fix only while the KSC is under its sphere.</summary>
            private static bool Prefix()
            {
                return !StaticsFix.IsOut(KscName);
            }
        }
    }
}
