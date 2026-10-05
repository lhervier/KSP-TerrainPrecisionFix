# Breaking Ground's deployed experiments

Part of [Terrain Precision Fix](../../README.md), one test of [Non-regression tests: the ground](../non-regression-ground.md).

**Status: TBD.** Deployed experiments (`ModuleGroundPart` and the modules around it) are vessels,
positioned in double like any craft, so they should sit on the corrected ground like one. They are also
the parts `Vessel.GoOffRails` skips the physics hold for, like the ground anchor.

*To test:* KSP Diag - Landed Vessel on a deployed experiment, six loads, with and without this fix.
