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

**Checked in flight**, in two sessions logged with this mod at `logLevel = Debug`, where it logs every
static it takes out of its sphere, with the correction it applied, and every static it puts back. Each
session loads a save several times, then sends the craft to a 200 km orbit with
`Alt+F12 → Cheats → Set Orbit`, then goes back to the space centre. At every loading and at each of the
last two steps, the same log lists every `PQSCity` of the body, where it hangs and how far it is from the
craft, and the destructible buildings and upgradeable facilities of the KSC, with whether KSP has them
registered.

- **On Kerbin**, in the install of [Checking the culprit](../checking-the-culprit-runway.md):
  [`runway-kerbin.sfs`](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag/blob/main/diag/runway-kerbin.sfs)
  loaded six times, the craft on the grass 1.4 km from the origin of the KSC
  ([`diag/runs/statics-kerbin-fix.log`](../../diag/runs/statics-kerbin-fix.log)).
- **On Earth**, in the install of [Real Solar System](rescaled-systems-real-solar-system.md):
  [`reload-earth-rss-landed.sfs`](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag/blob/main/diag/reload-earth-rss-landed.sfs)
  loaded twice, the craft 1.4 km from the KSC at Cape Canaveral
  ([`diag/runs/statics-earth-rss-fix.log`](../../diag/runs/statics-earth-rss-fix.log)).

At every loading, the KSC is taken out of its sphere — corrected by 36 to 100 mm on Kerbin, by 361 and
266 mm on Earth: under two float steps at that distance from the centre of the body (62.5 mm on Kerbin,
500 mm on Earth), far from the sixteen the safeguard allows — and nothing else is:
on Kerbin, the Island Airfield, 33 km from the craft, stays under its sphere. The KSC is put back under
it when the save is loaded again and when the craft reaches orbit, and it is under it at the space
centre, which opens normally. Its 39 destructible buildings and 9 upgradeable facilities stay registered
under their usual names, which KSP builds from their place in the hierarchy below the KSC. The only
errors in the logs are the stock loader's intended one and, on Earth, four launch sites of the Making
History expansion that Real Solar System does not place, which its log shows without this mod too.

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
- a mission of the Making History expansion that spawns a craft on the launchpad, without any other mod:
  stock finds the KSC from the launchpad, places it again, sets the launchpad's level and puts the craft
  on its spawn point;
- the cost per frame, with many statics near a craft;
- the runway of the KSC on Earth in Real Solar System, and Real Solar System's own runway fix, which
  keeps the floating origin from moving while a craft rolls on it.
