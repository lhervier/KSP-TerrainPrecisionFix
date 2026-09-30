# Kerbal Konstructs

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: TBD — its ground is read in the source, its statics are covered by this mod and measured on a
runway, its group editor is still to test.** [Kerbal Konstructs](https://github.com/KSP-RO/Kerbal-Konstructs)
plants statics — pads, runways, whole bases — anywhere on a body, and flattens the ground to seat them.
A fix that quietly undid those edits would be exactly the kind of side effect a player finds before
anyone else.

**Its ground.** Read in the source, and reassuring: Kerbal Konstructs writes no terrain code of its own.
`Core/MapDecals/MapDecalInstance.cs` adds a stock `PQSMod_MapDecal` to the body's `pqsController` and
calls its stock `OnSetup()`: its whole terrain editing is the stock `PQSMod_MapDecal` /
`PQSMod_MapDecalTangent`, the same components stock uses to flatten the ground around the KSC. A decal
edits `vbData.vertHeight` in `OnVertexBuildHeight`, which runs **before** `PQS.BuildVertexSurfaceRelative`
consumes it, and this fix consumes exactly the same `vbData.directionFromCenter * vbData.vertHeight`: the
height it places is the already-flattened one. Parallax is in the same position: its only decal-related
`PQSMod`, `PQSMod_MapDecalVertexRemoveScatter`, removes scatter inside a decal and does not touch height.
The safeguard would not catch a decal going wrong, since the quads would still be within a few float
steps of where they belong.

**Its statics.** Each group of statics hangs from a stock `PQSCity` of its own
(`Core/StaticGroup/GroupCenter.cs`), child of the body's terrain sphere: they carry the
[second culprit](../the-culprit.md#a-second-culprit-the-statics), as the KSC's do. Measured on stock,
with the runway protocol of both instruments, on a runway placed by Kerbal Konstructs on the Mun: it
comes back somewhere else at every load, and the step between it and the ground beside it changes
([Diag 1](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag/blob/main/docs/the-measurements-runway.md),
[Diag 2](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/the-measurements-runway.md)).
This mod takes a group out of its sphere like the KSC ([The statics](../the-fix-this-mod-proposes.md#the-statics)),
and patches the group editor of Kerbal Konstructs, which reads the position of a group from where stock
hangs it (below, [The patch of the group editor](#the-patch-of-the-group-editor)).

**Measured with this mod**, on that same runway: the deck comes back within 0.009 mm from the second
loading to the sixth, instead of 33.5 mm on stock, and the ground beside it within 0.036 mm. At the first
loading of a session, a section of the runway 21.3 mm above the deck is still active under the craft,
on stock as with this mod; it is not a rounding, and this mod does not touch it. The readings:
[Checking the culprit: the runway and the grass beside it](../checking-the-culprit-runway.md).

*To test:* why that section of the runway is only there at the first loading; the group editor in flight, near a craft —
moving a group, turning it, creating, copying and deleting one, then loading the save again; a launch
from a launch site of Kerbal Konstructs; and, for the ground, Terrain Precision Fix Diag 2 reading the
ground inside a flattened area and just outside it.

## The patch of the group editor

**Where Kerbal Konstructs looks for a static.** Read in the source of 1.12.3: it looks up statics among
the children of a body's terrain sphere only while the game loads, to find the KSC and the other stock
sites it offers as groups (`Core/StaticGroup/BuiltinCenters.cs`, `Core/LaunchSites/LaunchSiteManager.cs`),
when no static is out of its sphere. Everything else it does with a static in flight goes through world
positions, or through positions relative to the group's own `PQSCity`, which mean the same thing wherever
it hangs — except in its group editor.

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
read, the patch changes nothing, and the statics fix stays off.

**The change in Kerbal Konstructs it stands for.** In `OnMoveCallBack`, read that position whatever the
group's `PQSCity` hangs from:
`selectedGroup.CelestialBody.pqsController.transform.InverseTransformPoint(...)` of its world position,
the same frame as before.

**Checked**, with Kerbal Konstructs 1.12.3: the log says the patch is applied. The group editor itself
is in the list above, still to test.
