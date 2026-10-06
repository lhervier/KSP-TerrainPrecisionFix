# Real Solar System: the runway fix

Part of [Terrain Precision Fix](../../../README.md), one point of [Work in progress](../../work-in-progress.md), on [Real Solar System](../../work-in-progress.md#real-solar-system).

**Status: planned.** With this mod, neither half of Real Solar System's runway fix has anything left to
correct for this defect, checked with Real Solar System built without it
([The runway fix](../../non-regression/real-solar-system/the-runway-fix.md)).

*To test:* the runway fix getting in the way of this mod. In the install of
[Seeing it](../../non-regression/real-solar-system/the-runway-fix.md#seeing-it), with this mod at
`logLevel = Debug`, then a second time without this mod to tell what stock already does: with Real Solar
System as released, launch the rover from the SPH and reload it several times, then taxi it along the
runway. It should stand on the deck at every load, and `KSP.log` should show no error from Real Solar
System.
