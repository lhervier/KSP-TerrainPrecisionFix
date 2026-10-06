# The scatter fix without the terrain fix

Part of [Terrain Precision Fix](../../../README.md), one point of [Work in progress](../../work-in-progress.md), on [stock](../../work-in-progress.md#stock).

**Status: planned.** The scatter fix, off by default, is installed whether the terrain fix is on or
not, but it has only been measured with it
([The scatter fix: checking the culprit](../../limits-and-solutions/stock/the-scatter-fix/checking-the-culprit.md)).
On its own, it should keep the scatter on the stock ground, which still comes back at a different height
at every load.

*To test:* the series of [KSP Diag - Scatter](https://github.com/lhervier/KSP-Diag-Scatter), on its two
saves, twelve loads each, with `fixTerrain = false` and `fixScatter = true`. Every holder should be
drawn on its quad, and the objects should move with the ground under them, by as much as the stock
ground moves, and no more.
