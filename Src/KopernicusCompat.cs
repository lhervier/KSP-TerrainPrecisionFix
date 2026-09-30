using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace com.github.lhervier.ksp.terrainprecisionfix
{
    /// <summary>
    /// Makes Kopernicus find the KSC when it is out of its terrain sphere.
    ///
    /// Kopernicus fixes the KSC's flags whenever a facility is upgraded or repaired, and looks the KSC up
    /// among the PQSCity under the home body's sphere, then uses it without checking it was found. In
    /// flight, a facility is upgraded when a mission of the Making History expansion spawns a craft; the
    /// KSC may then be out of its sphere, the lookup finds nothing, and the fix throws.
    ///
    /// The change this patch stands for, in Kopernicus.RuntimeUtility.RuntimeUtility.FixFlags(), looks the
    /// KSC up without assuming it hangs from the sphere, for instance among the PQSCity whose sphere is the
    /// home body's. The patch itself returns what the stock lookup returns, plus the statics this fix moved
    /// out of the sphere: without this fix moving statics, the result is the same as unpatched.
    ///
    /// Kopernicus' other lookups of the KSC under the home body's sphere run when the main menu, the space
    /// centre, the tracking station or an editor is entered, and statics are back under their sphere before
    /// any scene change; the rest of its code about PQSCity runs while loading.
    /// </summary>
    internal static class KopernicusCompat
    {
        private const string RuntimeUtilityTypeName = "Kopernicus.RuntimeUtility.RuntimeUtility";

        // Set by the transpiler once it has found and replaced the one lookup it expects.
        private static bool _patched;

        /// <summary>
        /// Applies the patch when Kopernicus is installed and <paramref name="enabled"/>. Returns false when
        /// it is installed, the patch is wanted, and its code is not the one the patch expects; true
        /// otherwise, including when the patch is turned off, which is reported as breaking the flag fix.
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
                    + " of the Making History expansion does when it spawns a craft there");
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
            if (_patched)
            {
                Log.Info("Kopernicus: flag fix patched to cope with statics out of their sphere");
            }
            return _patched;
        }

        [HarmonyPatch]
        private static class FixFlagsPatch
        {
            private static MethodBase TargetMethod()
            {
                // The overload without parameters: the two others, event handlers, call it.
                return AccessTools.Method(AccessTools.TypeByName(RuntimeUtilityTypeName), "FixFlags", Type.EmptyTypes);
            }

            /// <summary>
            /// Replaces the one call to GetComponentsInChildren&lt;PQSCity&gt;(bool) with
            /// <see cref="StaticsFix.CitiesUnder"/>. Leaves the method unchanged, and the patch reported as
            /// not applied, unless there is exactly one such call.
            /// </summary>
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> code = instructions.ToList();
                List<int> lookups = new List<int>();
                for (int i = 0; i < code.Count; i++)
                {
                    if (IsCitiesLookup(code[i]))
                    {
                        lookups.Add(i);
                    }
                }
                if (lookups.Count != 1)
                {
                    Log.Warning($"Kopernicus: expected one lookup of PQSCity in RuntimeUtility.FixFlags,"
                        + $" found {lookups.Count}, flag fix left as is");
                    return code;
                }

                // Same stack before and after: the component and the flag in, the array out. Opcode and
                // operand only, so that the instruction keeps its labels and blocks.
                CodeInstruction lookup = code[lookups[0]];
                lookup.opcode = OpCodes.Call;
                lookup.operand = AccessTools.Method(typeof(StaticsFix), nameof(StaticsFix.CitiesUnder));
                _patched = true;
                return code;
            }

            /// <summary>Whether the instruction calls Component.GetComponentsInChildren&lt;PQSCity&gt;(bool).</summary>
            private static bool IsCitiesLookup(CodeInstruction instruction)
            {
                MethodInfo method = instruction.operand as MethodInfo;
                if (method == null || method.Name != nameof(Component.GetComponentsInChildren)
                    || method.DeclaringType != typeof(Component) || !method.IsGenericMethod)
                {
                    return false;
                }
                Type[] typeArguments = method.GetGenericArguments();
                ParameterInfo[] parameters = method.GetParameters();
                return typeArguments.Length == 1 && typeArguments[0] == typeof(PQSCity)
                    && parameters.Length == 1 && parameters[0].ParameterType == typeof(bool);
            }
        }
    }
}
