# The other stock statics

Part of [Terrain Precision Fix](../../../README.md), one point of [Work in progress](../../work-in-progress.md), on [stock](../../work-in-progress.md#stock).

**Status: planned.**

*Why.* The KSC 2, the Island Airfield, the pyramids, the monoliths and the easter eggs are placed by
`PQSCity` like the KSC, and this mod takes them out of their sphere the same way. They are not scatter:
the anomalies KerbNet shows, and the ones a craft discovers by coming close, are the
`PQSSurfaceObject`s of the body, and `PQSCity` and `PQSCity2` are the only kinds of them.

Some monoliths are placed at random, a different place on each body in each game: their `PQSCity`
(`randomizeOnSphere`) waits one frame after the sphere is set up, picks a latitude and a longitude from
the seed of the game (`PQSCity.Randomize`), then places itself with `PQSCity.Orientate`, which this mod
patches. Read in the code, they should follow the corrected ground like the others; not seen in game.

*The test.* A craft landed by each of them, loaded twice, then flown away and back. For a monolith
placed at random, the same on two bodies, in one game loaded with this mod and without it, to see that
it stands in the same place.

*What should happen.* The static should be where it was, and the craft should stand as it did.
