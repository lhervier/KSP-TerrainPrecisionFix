# Kerbal Konstructs: the ground it flattens

Part of [Terrain Precision Fix](../../../README.md), one point of [Work in progress](../../work-in-progress.md), on [Kerbal Konstructs](../../work-in-progress.md#kerbal-konstructs).

**Status: planned — read in the source, not measured.** [Kerbal Konstructs](https://github.com/KSP-RO/Kerbal-Konstructs)
flattens the ground to seat its statics. A fix that quietly undid those edits would be exactly the kind
of side effect a player finds before anyone else.

**Read in the source, and reassuring.** Kerbal Konstructs writes no terrain code of its own.
`Core/MapDecals/MapDecalInstance.cs` adds a stock `PQSMod_MapDecal` to the body's `pqsController` and
calls its stock `OnSetup()`: its whole terrain editing is the stock `PQSMod_MapDecal` /
`PQSMod_MapDecalTangent`, the same components stock uses to flatten the ground around the KSC. A decal
edits `vbData.vertHeight` in `OnVertexBuildHeight`, which runs **before** `PQS.BuildVertexSurfaceRelative`
consumes it, and this fix consumes exactly the same `vbData.directionFromCenter * vbData.vertHeight`: the
height it places is the already-flattened one. Parallax is in the same position: its only decal-related
`PQSMod`, `PQSMod_MapDecalVertexRemoveScatter`, removes scatter inside a decal and does not touch height.
The safeguard would not catch a decal going wrong, since the quads would still be within a few float
steps of where they belong.

*To test:* [KSP Diag - Terrain Height](https://github.com/lhervier/KSP-Diag-TerrainHeight) reading the
ground inside a flattened area and just outside it, over several loads, with and without this mod. If
this mod undid the flattening, the ground inside would come back at the height of the ground outside,
and a static would stand on a step.
