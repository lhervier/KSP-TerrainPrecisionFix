# Non-regression tests: the ground

Part of [Terrain Precision Fix](../README.md), one half of its non-regression tests, with
[the statics](non-regression-statics.md).

This mod places in double the terrain quads a craft can stand on: [The fix: the ground](the-fix-ground.md).
What it fixes there is measured in [Checking the culprit](../README.md#checking-the-culprit). This page
checks the other side: what stock places on the ground, or around it, is not made worse.

Unless it says otherwise, each test is played in KSP 1.12.5 with Harmony, ModuleManager, KSP Community
Fixes 1.41.1 and this mod, plus the instrument it names, and a second time without this mod, to tell
what stock already does. Each test has a chapter of its own, linked from its first column.

| Test | Why | Result |
|---|---|---|
| [Rocks, grass and trees](non-regression/rocks-grass-and-trees.md): two saves of [KSP Diag - Scatter](https://github.com/lhervier/KSP-Diag-Scatter), on Kerbin and on the Mun, each loaded twelve times. | Stock scatter hangs from a holder placed through the same float as the ground, and this mod corrects the ground, not the holder. | **Checked — not moved, but a stock defect widened on Kerbin.** The height of a measured point above the ground comes back 94 mm apart (median) on Kerbin in stock, 130 mm with this mod; 31 mm either way on the Mun. Visual only: stock scatter has no collider. [Rock Precision Fix](https://github.com/lhervier/KSP-RockPrecisionFix) corrects it. |
| [The seam between subdivision levels](non-regression/the-seam-between-subdivision-levels.md): loads with [KSP Diag - Quad Seams](https://github.com/lhervier/KSP-Diag-QuadSeams), on Earth in Real Solar System and on Kerbin. | This mod corrects the quads of the highest level only, not the coarser ones they meet. | **Checked — the seam is not opened, but a stock crack widened.** The median of the largest gap of a load goes from about 1.3 m to about 1.9 m on Earth (79 loads without this mod, nine with it), and from about 157 mm to about 225 mm on Kerbin (seven and ten loads). Visual only: the coarser quads have no collider. |
| [Existing saves](non-regression/existing-saves.md): a craft saved without this mod, loaded once and saved again with it, then loaded again. | A craft saved in stock was saved on the ground of one random draw, and comes back on the corrected one. | **Checked.** The corrected ground is one of the draws stock could have given, and it no longer changes: loading the craft once and saving it again ends the transition. On the Moon in Real Solar System, the craft then did not move in 42 loads. Still to test on Kerbin, on the save of the switching protocol. |
| [Colliders below the highest subdivision level](non-regression/colliders-below-the-highest-subdivision-level.md): the collider offset of each body, read in flight. | This mod corrects the highest level only; with a non-zero offset, lower levels would carry colliders and stay uncorrected. | **Checked on Kerbin and the Mun**, where the offset is 0. To test on the other bodies. |
| [Sloped ground](non-regression/sloped-ground.md): a craft on a 30° slope, loaded several times. | Every campaign so far is on flat ground; on a slope, a separate stock bug, read in the code, moves a single-part craft into the ground at every load, and this mod does not touch it. | To test. |
| [Ground anchors](non-regression/ground-anchors.md): an anchored base, loaded several times. | The anchor starts its physics on the very first frame and is frozen after one, at whatever height the ground is then. | To test. A stable ground should make its behaviour repeatable, not fix it. |
| [Asteroids held by a claw](non-regression/asteroids-held-by-a-claw.md): a clawed asteroid on the Mun, saved and reloaded several times. | At the first load with this mod, the ground under the asteroid moves once, with a mass on the other end of a joint. | To test. |
| [Breaking Ground's surface features](non-regression/breaking-grounds-surface-features.md): where their collider sits against what is drawn, over several loads. | They are placed like the rocks, and carry a collider. | To test. |
| [Breaking Ground's deployed experiments](non-regression/breaking-grounds-deployed-experiments.md): [KSP Diag - Landed Vessel](https://github.com/lhervier/KSP-Diag-LandedVessel) on a deployed experiment, six loads. | They are vessels and should sit on the corrected ground like a craft, but skip the physics hold, like the ground anchor. | To test. |
| The ocean: a craft splashing down near a coast, with this mod at `logLevel = Debug`. | Read in the logs of Earth in Real Solar System, this mod corrected terrain quads only, never the ocean, which fits its check on `surfaceRelativeQuads`; the value of that flag on the ocean sphere has not been read. | To test. The log should show no quad of the ocean corrected, and the craft should float as in stock. |
| [The map view](non-regression/the-map-view.md): the log, and the terrain on returning to flight. | Quads are built and dropped all the time in the map view. | To test. |
