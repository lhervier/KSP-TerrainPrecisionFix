# Scatter with colliders

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: checked, no problem — this mod halves a stock gap, and does not close it.** Stock scatter has no collider, but
a mod can give it one — [Kopernicus](https://github.com/Kopernicus/Kopernicus) with the
[Stock Scatter Collider Enabler Patch](https://github.com/Poodmund/Stock-Scatter-Collider-Enabler-Patch),
both on CKAN, do. The offset described in
[Rocks, grass and trees](rocks-grass-and-trees.md) then stops being visual: the physics engine is
handed the holder's position, while the pilot sees what is drawn from the holder's matrix, and those are
the two numbers that round differently. The rock a craft hits is not the rock its pilot sees.

Rock Precision Fix Diag measures the gap between a collider and the object it belongs to, over six loads
of a kerbal standing on a boulder in a desert of Kerbin, in each configuration
([the readings](https://github.com/lhervier/KSP-RockPrecisionFix/blob/main/docs/checking-the-culprit.md#rock-precision-fix-diag-the-colliders)):
it runs from −68.7 to +104.2 mm on stock, and from −70.2 to +70.3 mm with this mod — halved, and drawn
afresh at every load. On the stock loads, the six pictures taken with the readings show it: the kerbal's
boots sink into the boulder at one load and stand clear of it at the next.

**Solution.** [Rock Precision Fix](https://github.com/lhervier/KSP-RockPrecisionFix) again, and only with
this mod as well: the same series reads −0.026 to +0.022 mm with both installed. The collider and the
object are the same object again.
