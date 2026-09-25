# KSP Community Fixes' own terrain patches

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: checked, in the source.** None of KSP Community Fixes' patches places a terrain quad or a
terrain vertex. The ones that touch the terrain — `PQSUpdateNoMemoryAlloc`, `PQSCoroutineLeak`,
`PQSOnlyStartOnce`, `ScatterDistribution` — patch how the terrain spheres start and update and how
scatter is distributed; none of them patches `PQS.BuildVertexSurfaceRelative`, `PQ.SetupQuad`,
`PQ.PreciseUpdateSubQuadsPosition` or `CelestialBody.PreciseUpdateQuadPositions`. `FloatingOriginPerf`
replaces `FloatingOrigin.setOffset`, but stock repositions the landed quads from the method that calls it,
not from inside it, so that path is left as stock has it.

Read in the repository as of release 1.40.1. *To do:* read it again on 1.41.1, the release every
campaign ran with.
