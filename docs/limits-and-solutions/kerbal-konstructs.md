# Kerbal Konstructs

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: TBD — read in the source, not measured.** [Kerbal Konstructs](https://github.com/KSP-RO/Kerbal-Konstructs)
plants statics — pads, runways, whole bases — anywhere on a body, and flattens the ground to seat them.
A fix that quietly undid those edits would be exactly the kind of side effect a player finds before
anyone else.

Read in the source, and reassuring: Kerbal Konstructs writes no terrain code of its own.
`Core/MapDecals/MapDecalInstance.cs` adds a stock `PQSMod_MapDecal` to the body's `pqsController` and
calls its stock `OnSetup()`: its whole terrain editing is the stock `PQSMod_MapDecal` /
`PQSMod_MapDecalTangent`, the same components stock uses to flatten the ground around the KSC. A decal
edits `vbData.vertHeight` in `OnVertexBuildHeight`, which runs **before** `PQS.BuildVertexSurfaceRelative`
consumes it, and this fix consumes exactly the same `vbData.directionFromCenter * vbData.vertHeight`: the
height it places is the already-flattened one. Parallax is in the same position: its only decal-related
`PQSMod`, `PQSMod_MapDecalVertexRemoveScatter`, removes scatter inside a decal and does not touch height.

Two questions stay open. Its statics may be placed like the KSC's, from the centre of the body through a
float `Transform`, in which case they carry the same defect and this fix does not cover them. And the
safeguard would not catch a decal going wrong, since the quads would still be within a few float steps of where
they belong.

*To test:* a Kerbal Konstructs site, with and without this fix, Terrain Precision Fix Diag 2 reading the
ground inside the flattened area and just outside it, and a craft parked on one of its pads.
