# Real Solar System: descents onto Venus, Mars and Mercury

Part of [Terrain Precision Fix](../../../README.md), one point of [Work in progress](../../work-in-progress.md), on [Real Solar System](../../work-in-progress.md#real-solar-system).

**Status: planned.** On Venus, Mars and Mercury, this mod corrected the terrain by up to 3.6 float steps
and refused nothing, over six loads each
([What this mod corrected](../../non-regression/real-solar-system/what-this-mod-corrected.md)). Every
one of those series loads a landed craft again, while a player builds terrain continuously during a
descent.

*To test:* descents onto the same sites, flown by an autopilot so they can be replayed, with this mod
at `logLevel = Debug`. The log should show no correction refused; if one were, the terrain of that
body would be left as stock builds it, with a warning in the log.
