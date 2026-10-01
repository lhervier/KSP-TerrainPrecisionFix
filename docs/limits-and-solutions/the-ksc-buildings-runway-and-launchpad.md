# The KSC buildings, runway and launchpad

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: covered by this mod, measured on the runway of Kerbin; the rest still to test.** The statics
of the KSC are placed by a `PQSCity`, through a float `Transform` at planet scale, like the terrain:
[The culprit: the statics](../the-culprit-statics.md). This mod takes a static
out of its terrain sphere in flight, while a craft is near it, and places it in double:
[The fix: the statics](../the-fix-statics.md).

**Measured on the runway of Kerbin**, with the runway protocol of both instruments, six loadings, a
craft on the grass and a craft on the runway: the deck of the runway spreads over 130.1 mm without this
mod and 0.216 mm with it, and the step between it and the grass beside it over 81.7 mm and 0.203 mm.
**And while a rover drives by it**, with the protocol of the runway and the grass while the world
moves, played by a script: at each move of the floating origin, on stock, the deck moves by −8.85 and
+48.47 mm, together with the grass; with this mod, the deck moves by 0.04 mm at most over three moves.
A craft rolling on the runway gets no bump from the runway at a move of the origin.
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

## Still to test

Each case below is what a player would see if this mod broke something there, then how to check it by
hand, in game. Unless it says otherwise, each is played in KSP 1.12.5 with Harmony, ModuleManager, KSP
Community Fixes 1.41.1 and this mod, with `logLevel = Debug` in its settings so that `KSP.log` shows each
static it takes out of its sphere and puts back; and each is played a second time without this mod, to
tell what stock already does.

- **A craft spawned in the wrong place.** From the VAB, launch a pod on the launchpad; from the SPH, a
  plane on the runway. Each settles where stock puts it, without a drop or a jolt.
- **The KSC missing or misplaced after a scene change.** With a craft on the launchpad, in turn: revert
  to launch, quicksave and quickload (F5, F9), go to the space centre and back through the tracking
  station, revert to the VAB, recover the craft. In flight, the KSC is where it was; at the space centre,
  every building opens.
- **The KSC missing or blurred after a trip to another body.** With a craft on the launchpad and a second
  one parked on the runway, send the first around the Mun (`Alt+F12 → Cheats → Set Orbit`), then switch
  to the second from the map view. The KSC is there, at its full detail, and the second craft stands on
  the runway.
- **The KSC drifting during time warp.** With a craft on the runway, time warp at the highest rate on
  rails for a few days of game time, then stop. The craft and the KSC are where they were.
- **A destroyed building that does not stay destroyed, or will not come back.** In flight, crash a craft
  into a destructible building of the KSC, such as the water tower of the launchpad. At the space centre,
  it shows as destroyed; repaired there, it is back at its place in the next flight.
- **A facility drawn at the wrong level.** In a career game, launch from the launchpad and from the
  runway at each of their three levels, upgraded from the space centre. In flight, each facility shows
  the buildings of its level, at their place.
- **No connection to the KSC's ground station.** CommNet on, a probe on the launchpad: the signal
  indicator shows a connection to the KSC. The same in a game that starts in flight, from a stock
  scenario.
- **A craft spawned by a mission away from its spawn point.** With the Making History expansion and no
  other mod, play the mission [`KSC flag fix`](../../diag/kopernicus-flag-fix/Missions/): 30 seconds
  in, it spawns a pod on the launchpad. The pod stands on its spawn point, the launchpad at its level.
  Seen once, with Kopernicus and Real Solar System, in
  [Seeing the patch](kopernicus/the-flag-fix.md#seeing-the-patch): the pod appears on the launchpad;
  whether it stands on its spawn point was not checked.
- **The same failures at the other statics of stock.** The KSC 2, the Island Airfield, the pyramids and
  the anomalies: a craft landed by each, loaded twice, then flown away and back. The static is where it
  was, and the craft stands as it did.
- **A static sliding over the ground of a body without an atmosphere.** Low over a body, KSP turns the
  world around the craft; above an altitude set for each body, it turns the body, and this mod has to turn the statics
  with it. A craft climbing from the Mun's surface to 25 km above an anomaly of the Mun: from up there,
  the anomaly does not slide over the ground.
- **A bump under a craft rolling on the runway of the KSC on Earth, in Real Solar System.** Real
  Solar System keeps the floating origin from moving while a craft rolls on that runway, and no setting
  turns that lock off. In the install of
  [Rescaled systems: Real Solar System](rescaled-systems-real-solar-system.md), taxi a plane from one
  end of the runway to the other, with Real Solar System as released, then built from its sources with
  the lock turned off; each without this mod and with it. As released, the plane rolls without a bump;
  with the lock off, it does so with this mod.

The cost per frame of the statics fix is no impact a player can check by hand: it belongs to
[Performance](../performance.md), where it is still to measure.
