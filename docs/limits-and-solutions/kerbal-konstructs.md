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
hangs it ([Mods that look for a static under its sphere](mods-that-look-for-a-static-under-its-sphere.md)).

**Measured with this mod**, on that same runway: the deck comes back within 0.009 mm from the second
loading to the sixth, instead of 33.5 mm on stock, and the ground beside it within 0.036 mm. At the first
loading of a session, a section of the runway 21.3 mm above the deck is still active under the craft,
on stock as with this mod; it is not a rounding, and this mod does not touch it. The readings:
[Checking the culprit: the runway and the grass beside it](../checking-the-culprit-runway.md).

*To test:* why that section of the runway is only there at the first loading; the group editor in flight, near a craft —
moving a group, turning it, creating, copying and deleting one, then loading the save again; a launch
from a launch site of Kerbal Konstructs; and, for the ground, Terrain Precision Fix Diag 2 reading the
ground inside a flattened area and just outside it.
