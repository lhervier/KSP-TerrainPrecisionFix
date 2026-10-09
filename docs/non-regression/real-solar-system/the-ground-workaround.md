# Real Solar System: the ground workaround

Part of [Terrain Precision Fix](../../../README.md), one test of [Non-regression tests](../../non-regression.md), on [Real Solar System](../../non-regression.md#real-solar-system).

**Status: checked on the Moon and on Earth — with this mod, Real Solar System's ground workaround has
nothing left to correct for this defect, and does not get in the way; without this mod, it does not
always keep a craft from being thrown, and it never runs while a craft is already on its wheels.**

## What it does

Real Solar System already works around the symptom. It ships a component of its own,
`VesselGroundPositionEnhancer`
([its source](https://github.com/KSP-RO/RealSolarSystem/blob/master/Source/VesselGroundPositionEnhancer.cs)),
added to *"mostly prevent vessels clipping into the ground and as a result flung into the air"*
([pull request #257](https://github.com/KSP-RO/RealSolarSystem/pull/257)). Whenever a landed craft goes
off rails, it runs the stock `Vessel.CheckGroundCollision`, which moves the craft onto the ground before
its physics starts whenever it is more than 10 cm off, inside the ground or above it, in one block, and
logs `ground contact! - error. Moving Vessel up X.XXXm` (or `down`). Under 10 cm, the stock method
leaves the craft where it is, inside the ground or not. The component only acts on a *landed* craft;
for a craft in *prelaunch*, as on Earth near the KSC, stock KSP runs that same method itself at every
load: `Vessel.GoOffRails` spares a landed craft whose saved terrain levels match the current ones, but
never a craft in *prelaunch*. The component turns itself off when an assembly named `WorldStabilizer`
is loaded, the mod it was written to stand in for; an empty assembly of that name in `GameData` is how
it was turned off below. Real Solar System's repository also has an option to force it off, which
release 20.1.3.0 does not have yet.

**When it runs.** It runs whenever a landed craft goes off rails, which a craft does wherever the game
sets it down: when a save is loaded, when a craft comes back within 200 m of a craft left parked, when
the player switches to a craft far away. It never runs while a craft is already on its wheels.

**It hides the symptom, not the defect.** It moves the craft, never the ground: with it on, as in
every series of
[Checking the culprit: loading the same save](../../checking-the-culprit-loading.md), the ground is still
built somewhere else at each load, by 48.7 mm on the Moon and 292.3 mm on Earth, and the craft still
comes to rest somewhere else, by 49.2 mm and 292.4 mm. All it can do is keep the craft from being
thrown, and it does not always manage that.

## Reloading until something happens

No instrument is needed for this one: the same saves, Real Solar
System as released, reloaded from the pause menu again and again, watching whether the craft jumps or
tips over; some series are the six loads of an instrument in
[Checking the culprit: loading the same save](../../checking-the-culprit-loading.md). Real Solar System
leaves one line in `KSP.log` each time the craft goes off rails, which counts the loads:
`[RSS-VGPE] CheckGroundCollision()` for a landed craft, where its workaround runs,
`[RSS-VGPE] Vessel going off rails in PRELAUNCH` for a craft in *prelaunch*, where stock's pass runs
instead. A `Moving Vessel` line is added whenever either pass moved the craft.

| install | save | loads | what the craft does |
|---|---|---|---|
| without this mod, workaround off | `reload-moon-rss.sfs` | 1 | **tips over** at the first load |
| without this mod | `reload-moon-rss.sfs` | 14 | moved up by the workaround at 6 loads, by 0.115 to 0.261 m; **tips over** at 2 and **jumps** at 1, having come back less than the 10 cm the workaround acts on; nothing visible at 5 |
| without this mod | `reload-moon-rss-resave.sfs` | 1 | **tips over** at the first load of a session, the workaround running, no `Moving Vessel` line |
| without this mod, Diag TerrainHeight | `reload-moon-rss-resave.sfs` | 6 | moved up by the workaround at 2 loads, by 0.178 and 0.216 m; **jumps** at 1; **tips over** at 1, having come back inside the ground by less than the 10 cm the workaround acts on |
| without this mod, Diag LandedVessel | `reload-earth-rss-resave.sfs` | 6 | moved by stock's pass at 5 loads, up by 0.251 to 0.456 m at 3 and down by 0.143 to 0.283 m at 2; **jumps** at 1, having come back 83 mm inside the ground |
| without this mod, Diag TerrainHeight | `reload-earth-rss-resave.sfs` | 6 | moved up by stock's pass at 4 loads, by 0.102 to 0.678 m; **jumps** at 1 |
| with this mod, workaround off | `reload-moon-rss-resave.sfs` | 6 | stays put, over a spread of 0.364 mm; no `Moving Vessel` line |
| with this mod | `reload-moon-rss-resave.sfs` | 24 | never moves; 24 lines of the workaround, no `Moving Vessel` line |
| with this mod | `reload-earth-rss-landed.sfs` | 27 | never moves; the craft is *landed*, the workaround runs 27 times, no `Moving Vessel` line |
| with this mod | `reload-earth-rss-resave.sfs` | 24 | never moves; the craft is in *prelaunch*, stock's pass runs 24 times, no `Moving Vessel` line |

## Its limits

**Its limit: under 10 cm it does nothing, and without this mod the craft still jumps or tips over.**
A craft that comes back more than 10 cm inside the ground is moved up in one block, and one that comes
back more than 10 cm above it is moved down: nothing is launched, but a structure resting on several
points is set on its lowest one. A craft that comes back less than 10 cm inside the ground is left
there, and the physics engine pushes it out: a few centimetres are enough to throw it up, and it jumps,
or comes down on its side and tips over. On the Moon, 10 cm is less than one float step, and on Earth a fifth of one, so the draws
that fall under it are not rare: on the Moon, one jump and two tip-overs in fourteen loads, and two
more tip-overs in seven loads of the save taken again; on Earth, one jump in each of the two series of
six. The workaround ran at every one of those loads. With the workaround off, the craft tipped over at
the very first load.

**It never sees the ground move under a craft that is already rolling.** Every 500 m a craft travels,
KSP moves the floating origin back onto it, and stock places the ground under it again, somewhere else.
On Earth, across such a move, the ground under a rover driving on the grass by the KSC moved by 159 to
243 mm without this mod, and the rover was seen to jump; with this mod, a move changes nothing
([Checking the culprit: driving on while the world moves](../../checking-the-culprit-driving.md#on-earth)).
The craft does not go off rails there, so the workaround does not run.

## With this mod

**With this mod, it has nothing left to do for this defect.** The ground comes back within a
millimetre, well inside the 10 cm below which the pass leaves a craft where it is. Over 75 loads in a
row, 24 on the Moon and 51 on Earth, the craft never moved, and the pass, which still runs at every
load, found nothing to correct, without getting in the way. With the workaround off, this mod keeps
the craft in place on its own, over six loads read by Diag LandedVessel:

![Six loads on the Moon, with this mod and without Real Solar System's workaround](../../../imgs/non-regression/real-solar-system/the-ground-workaround/30-moon-fix.png)

*With this mod, Real Solar System's workaround off, Diag LandedVessel: six loads of `reload-moon-rss-resave.sfs`,
a spread of 0.364 mm, against 0.395 mm with it on.*

The workaround may well have other uses, outside the scope of this fix.

## The saves

The saves on the Moon and on Earth are in the `diag` folder of each of the three Diags (here, Diag LandedVessel's):

- [`reload-moon-rss.sfs`](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/diag/reload-moon-rss.sfs)
  and [`reload-moon-rss-resave.sfs`](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/diag/reload-moon-rss-resave.sfs) —
  the Moon, latitude 28.61°, longitude −80.62°, on flat ground;
- [`reload-earth-rss-resave.sfs`](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/diag/reload-earth-rss-resave.sfs)
  and [`reload-earth-rss-landed.sfs`](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/diag/reload-earth-rss-landed.sfs) —
  Earth, latitude 28.611°, longitude −80.619°, on the grass about 1.4 km west of the KSC, 74 m above
  sea level.
  `reload-earth-rss-landed.sfs` is `reload-earth-rss-resave.sfs` with one line changed in the file:
  the situation of the craft, from `PRELAUNCH` to `LANDED`, the situation in which the workaround runs.

Copy a save into the folder of a sandbox game and load it from that game.
