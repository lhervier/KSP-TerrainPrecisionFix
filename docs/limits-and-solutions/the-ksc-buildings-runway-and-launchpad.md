# The KSC buildings, runway and launchpad

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: covered by this mod, measured on the runway of Kerbin; the rest still to test.** The statics
of the KSC are placed by a `PQSCity`, through a float `Transform` at planet scale, like the terrain:
[A second culprit: the statics](../the-culprit.md#a-second-culprit-the-statics). This mod takes a static
out of its terrain sphere in flight, while a craft is near it, and places it in double:
[The statics](../the-fix-this-mod-proposes.md#the-statics).

**Measured on the runway of Kerbin**, with the runway protocol of both instruments, six loadings, a
craft on the grass and a craft on the runway: the deck of the runway spreads over 130.1 mm without this
mod and 0.216 mm with it, and the step between it and the grass beside it over 81.7 mm and 0.203 mm.
The readings are in
[Checking the culprit: the runway and the grass beside it](../checking-the-culprit-runway.md).

**Checked in flight**, near the KSC on Kerbin, and on Earth under Real Solar System: the KSC is taken out
of its sphere when a craft is loaded near it, put back under it when the save is loaded again, when the
craft is sent to orbit, and before the space centre opens, which then opens normally. Its 39 destructible
buildings and 9 upgradeable facilities stay registered under their usual names, which KSP builds from
their place in the hierarchy below the KSC, and nothing is logged as an error.

`PQSCity2`, which places the launch sites of the Making History expansion, carries the same defect and
is not covered.

*To test:*

- a shift of the floating origin under a craft parked on the runway;
- the other ways a scene is left or reloaded: revert, quickload, recovering a craft;
- launching from the runway and from the launchpad;
- flying to another body and back;
- destroying a building in flight, then repairing it from the space centre; every level of every
  facility;
- the ground station of the KSC, in a game that starts in flight;
- the cost per frame, with many statics near a craft;
- the runway of the KSC on Earth in Real Solar System, and Real Solar System's own runway fix, which
  keeps the floating origin from moving while a craft rolls on it.
