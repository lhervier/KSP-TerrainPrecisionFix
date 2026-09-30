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
    /// Makes Kerbal Konstructs' group editor work on a group whose static is out of its terrain sphere.
    ///
    /// Moving a group with the editor's gizmo, in flight, sets the world position of the group's static,
    /// then reads its transform.localPosition as its position relative to the centre of the body. That only
    /// holds while the static hangs directly from the sphere; out of it, the group would be sent somewhere
    /// else on the body, and saved there.
    ///
    /// The change this patch stands for, in KerbalKonstructs.UI.GroupEditor.OnMoveCallBack, reads that
    /// position whatever the static hangs from:
    ///   selectedGroup.RadialPosition = selectedGroup.CelestialBody.pqsController.transform
    ///       .InverseTransformPoint(selectedGroup.gameObject.transform.position);
    /// The patch itself reads the localPosition as before, and only works the position out otherwise when
    /// the static is out of its sphere: without this fix moving statics, the group editor runs exactly as
    /// it does unpatched.
    ///
    /// Everything else Kerbal Konstructs does with a static in flight goes through world positions, or
    /// through positions relative to the group's static, which do not depend on where it hangs from. Its
    /// searches for statics under a sphere only run at the main menu, when no static is out.
    /// </summary>
    internal static class KerbalKonstructsCompat
    {
        private const string GroupEditorTypeName = "KerbalKonstructs.UI.GroupEditor";
        private const string GroupCenterTypeName = "KerbalKonstructs.Core.GroupCenter";

        // Set by the transpiler once it has found and replaced the one read it expects.
        private static bool _patched;

        /// <summary>
        /// Applies the patch when Kerbal Konstructs is installed. Returns false when it is installed and
        /// its code is not the one the patch expects, true otherwise.
        /// </summary>
        public static bool Install(Harmony harmony)
        {
            if (AccessTools.TypeByName(GroupEditorTypeName) == null)
            {
                return true;
            }
            try
            {
                harmony.CreateClassProcessor(typeof(OnMoveCallBackPatch)).Patch();
            }
            catch (Exception e)
            {
                Log.Warning($"Kerbal Konstructs: could not patch its group editor: {e.Message}");
                return false;
            }
            if (_patched)
            {
                Log.Info("Kerbal Konstructs: group editor patched to cope with statics out of their sphere");
            }
            return _patched;
        }

        [HarmonyPatch]
        private static class OnMoveCallBackPatch
        {
            private static MethodBase TargetMethod()
            {
                return AccessTools.Method(AccessTools.TypeByName(GroupEditorTypeName), "OnMoveCallBack");
            }

            /// <summary>
            /// Replaces the one read of a transform's localPosition stored as a group's RadialPosition with
            /// <see cref="StaticsFix.SphereLocalPosition"/>. Leaves the method unchanged, and the patch
            /// reported as not applied, unless there is exactly one such read.
            /// </summary>
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> code = instructions.ToList();
                FieldInfo radialPosition = AccessTools.Field(AccessTools.TypeByName(GroupCenterTypeName), "RadialPosition");
                MethodInfo getLocalPosition = AccessTools.PropertyGetter(typeof(Transform), nameof(Transform.localPosition));
                if (radialPosition == null || getLocalPosition == null)
                {
                    Log.Warning("Kerbal Konstructs: GroupCenter.RadialPosition not found, group editor left as is");
                    return code;
                }

                // In the IL, the read is the call to the getter immediately followed by the store into
                // RadialPosition.
                List<int> reads = new List<int>();
                for (int i = 0; i + 1 < code.Count; i++)
                {
                    if (code[i].Calls(getLocalPosition) && code[i + 1].StoresField(radialPosition))
                    {
                        reads.Add(i);
                    }
                }
                if (reads.Count != 1)
                {
                    Log.Warning($"Kerbal Konstructs: expected one read of a group's localPosition in"
                        + $" GroupEditor.OnMoveCallBack, found {reads.Count}, group editor left as is");
                    return code;
                }

                // Same stack before and after: the transform in, a Vector3 out. Opcode and operand only,
                // so that the instruction keeps its labels and blocks.
                CodeInstruction read = code[reads[0]];
                read.opcode = OpCodes.Call;
                read.operand = AccessTools.Method(typeof(StaticsFix), nameof(StaticsFix.SphereLocalPosition));
                _patched = true;
                return code;
            }
        }
    }
}
