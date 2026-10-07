# KSP Community Fixes: its own terrain patches

Part of [Terrain Precision Fix](../../../README.md), one test of [Non-regression tests](../../non-regression.md), on [KSP Community Fixes](../../non-regression.md#ksp-community-fixes).

**Status: checked, in the source and in every campaign.** Every campaign on these pages ran with KSP Community Fixes 1.41.1 installed, the base most players run, and every measurement of this mod comes from that install. None of KSP Community Fixes' patches places a terrain quad or a
terrain vertex. The ones that touch the terrain — `PQSUpdateNoMemoryAlloc`, `PQSCoroutineLeak`,
`PQSOnlyStartOnce`, `ScatterDistribution` — patch how the terrain spheres start and update and how
scatter is distributed; none of them patches `PQS.BuildVertexSurfaceRelative`, `PQ.SetupQuad`,
`PQ.PreciseUpdateSubQuadsPosition` or `CelestialBody.PreciseUpdateQuadPositions`. `FloatingOriginPerf`
replaces `FloatingOrigin.setOffset`, but stock repositions the landed quads from the method that calls it,
not from inside it, so that path is left as stock has it.

None of them names a scatter holder either, which the scatter fix of this mod moves
([The fix: the scatter](../../the-fix-scatter.md)): `ScatterDistribution` only corrects the longitude the
scatter is spread by, in `PQSLandControl.OnVertexBuildHeight`, and `OptimizedModuleRaycasts` asks the
object a ray hits whether it is a quad, as stock does, not its parents.

Read in the repository as of release 1.40.1.
