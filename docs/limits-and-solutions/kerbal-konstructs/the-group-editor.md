# Kerbal Konstructs: the group editor

Part of [Terrain Precision Fix](../../../README.md), one limit of [Limits and solutions](../../limits-and-solutions.md), on [Kerbal Konstructs](../../limits-and-solutions.md#kerbal-konstructs).

**Status: checked, a problem this mod patches — moved with the gizmo of its group editor, in flight beside
a craft, a group stays where it is let go, and is saved there. Without the patch, it would be sent
elsewhere on its body, and saved there. The patch is on by default (`patchKerbalKonstructs = true`).**
[Kerbal Konstructs](https://github.com/KSP-RO/Kerbal-Konstructs) plants statics — pads, runways, whole
bases — anywhere on a body. Each group of statics hangs from a stock `PQSCity` of its own
(`Core/StaticGroup/GroupCenter.cs`), child of the body's terrain sphere, and this mod takes it out of
its sphere in flight, near a craft, like the KSC ([The fix: the statics](../../the-fix-statics.md)).

## Where Kerbal Konstructs looks for a static

Read in the source of 1.12.3: it looks up statics among the children of a body's terrain sphere only
while the game loads, to find the KSC and the other stock sites it offers as groups
(`Core/StaticGroup/BuiltinCenters.cs`, `Core/LaunchSites/LaunchSiteManager.cs`), when no static is out of
its sphere. Everything else it does with a static in flight goes through world positions, or through
positions relative to the group's own `PQSCity`, which mean the same thing wherever it hangs — except in
its group editor.

## The patch

**What Kerbal Konstructs does.** Moving a group with the gizmo of its group editor, in flight,
`GroupEditor.OnMoveCallBack` sets the world position of the group's `PQSCity`, then reads its
`transform.localPosition` as the position of the group relative to the centre of the body.

**Why it matters here.** That only holds while the `PQSCity` hangs directly from the terrain sphere. The
group editor is used in flight, near a craft, where this mod has taken the group out of its sphere: the
local position is then relative to something else, and the group would be sent elsewhere on the body,
and saved there.

**What this mod does.** It replaces that one read, in `OnMoveCallBack` alone, with one that returns the
`localPosition` as before while the static hangs from its sphere, and works the same position out from
its world position, in double, while it is out. Without this mod's statics fix, the group editor runs
exactly as it did. If Kerbal Konstructs is installed and `OnMoveCallBack` does not hold exactly one such
read, the patch changes nothing, and the statics fix stays off. The patch can be turned off in the
settings (`patchKerbalKonstructs = false`), to see what goes wrong without it.

## The change in Kerbal Konstructs it stands for

In `OnMoveCallBack`, read that position whatever the group's `PQSCity` hangs from: its world position,
brought into the frame of the body's terrain sphere, the same frame as before. Against the source of
Kerbal Konstructs 1.12.3, in
[`upstream/kerbal-konstructs-1.12.3-group-editor.diff`](../../../upstream/kerbal-konstructs-1.12.3-group-editor.diff):

```diff
                 selectedGroup.gameObject.transform.position = EditorGizmo.moveGizmo.transform.position;
-                selectedGroup.RadialPosition = selectedGroup.gameObject.transform.localPosition;
+                // Relative to the centre of the body, whatever the group's PQSCity hangs from: a mod may have
+                // taken it out of the terrain sphere, in flight, to place it in double precision.
+                selectedGroup.RadialPosition = selectedGroup.CelestialBody.pqsController.transform
+                    .InverseTransformPoint(selectedGroup.gameObject.transform.position);
```

With that change in Kerbal Konstructs, this mod's patch has nothing left to do.

## Checked

With Kerbal Konstructs 1.12.3 and CustomPreLaunchChecks 1.8.1, which it requires, and this mod at
`logLevel = Debug`. The save, [`non-reg-runway-mune-kk.sfs`](../../../diag/limits-and-solutions/kerbal-konstructs/the-group-editor/non-reg-runway-mune-kk.sfs),
is a pod landed on the Mun beside a runway of Kerbal Konstructs, whose two files go in
`GameData/KerbalKonstructs/NewInstances` ([`diag/limits-and-solutions/kerbal-konstructs/the-group-editor/non-reg-runway-mune-kk/GameData`](../../../diag/limits-and-solutions/kerbal-konstructs/the-group-editor/non-reg-runway-mune-kk/GameData)).
In flight, `Ctrl+K` → *Edit Groups* → `MuneBase` opens the group editor, in *Group* mode. Dragging the
arrows of its gizmo moves the runway, *Save&Close* saves it, then the save is loaded again. The runway
follows the gizmo and stays where it is let go, the latitude and longitude the editor shows change by
no more than the distance dragged, and once the save is loaded again the runway is where it was left.
The log says the patch is applied, and the group stays out of its sphere throughout, put back under it
only while Kerbal Konstructs places it anew after each drag:
[`kk-group-editor-fix.log`](../../../diag/limits-and-solutions/kerbal-konstructs/the-group-editor/kk-group-editor-fix.log). Only the gizmo goes
through the read this mod patches: the keys and the arrows of the editor's window move a group by
latitude and longitude, which mean the same thing wherever it hangs.
