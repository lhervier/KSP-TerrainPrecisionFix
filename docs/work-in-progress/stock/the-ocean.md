# The ocean

Part of [Terrain Precision Fix](../../../README.md), one point of [Work in progress](../../work-in-progress.md), on [stock](../../work-in-progress.md#stock).

**Status: planned.** Read in the logs of Earth in Real Solar System, this mod corrected terrain quads
only, never the ocean, which fits its check on `surfaceRelativeQuads`; the value of that flag on the
ocean sphere has not been read.

*To test:* a craft splashing down near a coast, with this mod at `logLevel = Debug`. The log should show
no quad of the ocean corrected, and the craft should float as in stock. If this mod touched the ocean, a
floating craft would bob or sink differently from one load to the next.
