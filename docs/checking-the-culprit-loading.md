# Checking the culprit: loading the same save

Part of [Terrain Precision Fix](../README.md): the measurements that check [the culprit](the-culprit.md) on a craft handed back by a save, on stock and with this mod.

The two other ways a craft is put back onto the ground are measured in [Coming back to a craft left parked](checking-the-culprit-approach.md) and [Switching to a craft far away](checking-the-culprit-switching.md).

Two instruments take the readings:
[Terrain Precision Fix Diag 1](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag) measures the craft, and
[Terrain Precision Fix Diag 2](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2) measures the ground. Each has its own page, with
its method and its protocol.

This fix is built on top of [KSP Community Fixes](https://github.com/KSPModdingLibs/KSPCommunityFixes),
the base most players run, so every campaign on this page is run in an install that has it: KSP 1.12.5 with Harmony, ModuleManager,
KSP Community Fixes 1.41.1 and one of the two instruments — and this mod, or not. *On stock*, below,
means that install without this mod.

The culprit predicts a spread of one or two float steps, and a float's step doubles each time the
distance to the centre of the body crosses a power of two. So besides the four stock worlds, the same
test is run on the Moon and on Earth of [Real Solar System](https://github.com/KSP-RO/RealSolarSystem),
which replaces the planets with the real ones, much larger. There, the install is the one above plus
Real Solar System 20.1.3.0 and what it requires (Kopernicus, Modular Flight Integrator,
KSPTextureLoader, the RSS textures), with one instrument or both, and Real Solar System as released:
it ships a workaround of its own for the symptom, which stays on, as players have it. What that
workaround does, and what it leaves, is in
[Real Solar System's own workaround](#real-solar-systems-own-workaround).

A craft is set down on bare ground, saved once, and that same save is loaded six times over. The
craft never changes, the spot never changes, and nothing is touched between two loads.

## The craft, over six loads

Terrain Precision Fix Diag 1 measures the distance from a landed capsule to the centre of the body,
twice per load: as the save hands the capsule back (*On rails*), and once it has settled on the ground
(*Settled*). How, and why those two readings, is in
[This mod's demonstration](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag#this-mods-demonstration).

**On stock.** Its campaigns, detailed in
[The measurements: loading the same save](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag/blob/main/docs/the-measurements-loading.md).
On each of the four stock worlds, a lone capsule, then the same capsule sitting on a small flat fuel
tank; on the Moon and Earth, the capsule on its tank. Each series is saved once and loaded six times,
and uses its own spot, chosen by the rules of
[its protocol](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag/blob/main/docs/the-protocol-loading.md):
no `Moving Vessel` line in `KSP.log`, and a craft that does not slide.

*On rails* is the same on every load, to within three micrometres: KSP puts the craft back at the same
place. *Settled* is not. Here is its spread, set against the step of a float at that distance from the
centre of the body:

| series | loads | distance to the centre | float step there | spread of *Settled* | in steps |
|---|---|---|---|---|---|
| Kerbin, capsule | 6 | 600.2 km | 62.5 mm | 134.5 mm | 2.2 |
| Kerbin, 2 parts | 6 | 600.1 km | 62.5 mm | 124.7 mm | 2.0 |
| Mun, capsule | 6 | 202.1 km | 15.6 mm | 20.7 mm | 1.3 |
| Mun, 2 parts | 6 | 202.6 km | 15.6 mm | 11.8 mm | 0.8 |
| Minmus, capsule | 6 | 60.0 km | 3.9 mm | 6.7 mm | 1.7 |
| Minmus, 2 parts | 6 | 60.0 km | 3.9 mm | 4.8 mm | 1.2 |
| Gilly, capsule | 6 | 16.7 km | 1.95 mm | 3.3 mm | 1.7 |
| Gilly, 2 parts | 6 | 16.7 km | 1.95 mm | 2.5 mm | 1.3 |
| the Moon, 2 parts | 6 | 1 744.4 km | 125 mm | 262.6 mm | 2.1 |
| Earth, 2 parts | 6 | 6 371.1 km | 500 mm | 301.1 mm | 0.6 |

On the Moon and Earth, the first rule of the protocol cannot be kept: at three of the six loads of each
series, the craft came back more than 10 cm inside the ground and was moved up before its physics
started, by Real Solar System's workaround on the Moon, and by stock's own pass on Earth, where the craft
is in *prelaunch* (see [Real Solar System's own workaround](#real-solar-systems-own-workaround)).
*Settled* is read all the same, as a player gets it. The Moon series loads `reload-moon-rss.sfs`, the
Earth series `reload-earth-rss-resave.sfs`, both in
[Diag 1's `diag` folder](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag/tree/main/diag).

**With this mod.** The same test, in the same installs, on the same six worlds, with the same craft,
loaded six times per series. On the Moon, the series loads the same save taken again once with this mod,
`reload-moon-rss-resave.sfs` (see [Existing saves](limits-and-solutions/existing-saves.md)). The ten
series, read off the screenshots in
[`imgs/Diag1/on-load/1part`](../imgs/Diag1/on-load/1part),
[`imgs/Diag1/on-load/2parts`](../imgs/Diag1/on-load/2parts) and
[`imgs/Diag1/on-load/2parts/rss`](../imgs/Diag1/on-load/2parts/rss) — in each of them, the bottom line
is the loading in progress, still live, and is not counted:

| series | spread of *Settled*, without this mod | spread of *Settled*, with this mod |
|---|---|---|
| Kerbin, capsule | 134.5 mm | 0.004 mm |
| Kerbin, 2 parts | 124.7 mm | 0.025 mm |
| Mun, capsule | 20.7 mm | 0.031 mm |
| Mun, 2 parts | 11.8 mm | 0.085 mm |
| Minmus, capsule | 6.7 mm | 0.038 mm |
| Minmus, 2 parts | 4.8 mm | 0.023 mm |
| Gilly, capsule | 3.3 mm | 0.058 mm |
| Gilly, 2 parts | 2.5 mm | 0.208 mm |
| the Moon, 2 parts | 262.6 mm | 0.395 mm |
| Earth, 2 parts | 301.1 mm | 0.178 mm |

With this mod, no load of the Moon or Earth series has a `Moving Vessel` line.

## The ground, over six loads

Terrain Precision Fix Diag 2 measures the ground, with no craft in the reading at all: the collision
surface a ray pointed straight down hits, against the height KSP computes for that same spot. The second
never moves; the first is what your landing legs touch. *Difference* is the first minus the second. How
both are read is in [This mod's demonstration](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/this-mods-demonstration.md).

**On stock.** Its campaigns, detailed in [The measurements: loading the same save](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/the-measurements-loading.md):
the same installs, with that instrument; one save on each of the six worlds, loaded six times,
following [its protocol](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/the-protocol-loading.md).
On the Moon, the save is `reload-moon-rss-resave.sfs`; on Earth, `reload-earth-rss-resave.sfs`, the
same session as the Earth series of Diag 1. Over those six loads:

| world | spread of *Difference* | spread of the height KSP computes |
|---|---|---|
| Kerbin | 108.1 mm | 0.039 mm |
| Mun | 15.0 mm | 0.015 mm |
| Minmus | 4.1 mm | 0.000 mm |
| Gilly | 3.0 mm | 0.018 mm |
| the Moon | 241.2 mm | 0.541 mm |
| Earth | 301.2 mm | 2.067 mm |

**With this mod.** The same installs, the same saves, on the same spots, loaded six times; this mod
is the only difference.

Read off the screenshots in [`imgs/Diag2`](../imgs/Diag2) and, for the Moon,
[`imgs/Diag2/rss`](../imgs/Diag2/rss); those of Earth show both instruments and are in
[`imgs/Diag1/on-load/2parts/rss`](../imgs/Diag1/on-load/2parts/rss) (as before, the bottom line of each
is the loading in progress, and is not counted):

| world | *Difference*, without this mod | *Difference*, with this mod | spread, without | spread, with |
|---|---|---|---|---|
| Kerbin | +199.797 to +307.930 mm | +246.972 to +246.976 mm | 108.1 mm | 0.004 mm |
| Mun | −23.958 to −38.925 mm | −40.756 to −40.769 mm | 15.0 mm | 0.013 mm |
| Minmus | −12.291 to −16.351 mm | −14.202 to −14.204 mm | 4.1 mm | 0.002 mm |
| Gilly | +37.426 to +40.391 mm | +37.511 to +37.520 mm | 3.0 mm | 0.009 mm |
| the Moon | −140.221 to +101.001 mm | −114.103 to −113.877 mm | 241.2 mm | 0.226 mm |
| Earth | +79.086 to +380.241 mm | +121.758 to +122.064 mm | 301.2 mm | 0.306 mm |

## Real Solar System's own workaround

Real Solar System already works around the symptom. It ships a component of its own,
`VesselGroundPositionEnhancer`
([its source](https://github.com/KSP-RO/RealSolarSystem/blob/master/Source/VesselGroundPositionEnhancer.cs)),
added to *"mostly prevent vessels clipping into the ground and as a result flung into the air"*
([pull request #257](https://github.com/KSP-RO/RealSolarSystem/pull/257)). Whenever a landed craft goes
off rails, it runs the stock `Vessel.CheckGroundCollision`, which moves the craft onto the ground before
its physics starts whenever it is more than 10 cm off, in one block, and logs `ground contact! - error.
Moving Vessel up X.XXXm`. Under 10 cm, the stock method leaves the craft where it is, inside the ground
or not. The component only acts on a *landed* craft; for a craft in *prelaunch*, as on Earth near the
KSC, stock KSP runs that same method itself at every load. The component turns itself off when an
assembly named `WorldStabilizer` is loaded, which is how it is turned off below.

**Reloading until something happens.** No instrument is needed for this one: the same saves, Real Solar
System as released, reloaded from the pause menu again and again, watching whether the craft jumps or
tips over. Real Solar System leaves one line in `KSP.log` each time the craft goes off rails, which
counts the loads, and a `Moving Vessel` line is added whenever either pass moved the craft. The two
series with its workaround turned off add an empty assembly named `WorldStabilizer` in `GameData`.

| install | save | loads | what the craft does |
|---|---|---|---|
| without this mod, workaround off | `reload-moon-rss.sfs` | 1 | **tips over** at the first load |
| without this mod | `reload-moon-rss.sfs` | 14 | moved up by the workaround at 6 loads, by 0.101 to 0.234 m; **jumps** at 3, having come back 17 to 38 mm inside the ground, under the 10 cm the workaround acts on; nothing visible at 5 |
| without this mod | `reload-earth-rss-resave.sfs` | 6 | moved up by stock's pass at 3 loads, by 0.150 to 0.259 m; **jumps** at 1 (+88.8 mm) |
| with this mod, workaround off | `reload-moon-rss-resave.sfs` | 6 | stays put, over a spread of 0.364 mm; no `Moving Vessel` line |
| with this mod | `reload-moon-rss-resave.sfs` | 24 | never moves; 24 lines of the workaround, no `Moving Vessel` line |
| with this mod | `reload-earth-rss-landed.sfs` | 27 | never moves; the craft is *landed*, the workaround runs 27 times, no `Moving Vessel` line |
| with this mod | `reload-earth-rss-resave.sfs` | 24 | never moves; the craft is in *prelaunch*, stock's pass runs 24 times, no `Moving Vessel` line |

`reload-earth-rss-landed.sfs` is `reload-earth-rss-resave.sfs` with one line changed in the file: the
situation of the craft, from `PRELAUNCH` to `LANDED`, the situation in which the workaround runs. The
Earth series without this mod is the six loads of Diag 1 above. The saves, the screenshots and the logs
of every series are in
[Rescaled systems: Real Solar System](limits-and-solutions/rescaled-systems-real-solar-system.md).

**The workaround catches part of the defect, and hides it.** A craft that comes back more than 10 cm
inside the ground is moved up in one block: nothing is launched, but a structure resting on several
points is lifted by its lowest one. A craft that comes back less than 10 cm inside the ground is left
there, and the physics engine pushes it out: it jumps. On the Moon, 10 cm is less than one float step,
and on Earth a fifth of one, so the draws that fall under it are not rare: three jumps in fourteen loads
on the Moon, one in six on Earth. With the workaround off, the craft tips over at the very first load.

**With this mod, it never has anything to move.** The ground comes back within a millimetre, well inside
the 10 cm below which the pass leaves a craft where it is. Over 75 loads in a row, 24 on the Moon and 51
on Earth, the craft never moved, and the pass, which still runs at every load, found nothing to correct
for this defect, without getting in the way. It may well have other uses, outside the scope of this fix.
With the workaround off, this mod keeps the craft in place on its own.

## What the measurements say

**KSP puts the craft back at the same place, and the craft does not come to rest there.** *On rails* is
the same on every load of every series, to within three micrometres; *Settled* is not, once, on any of
the six bodies.

**The spread is the size the culprit predicts.** From Gilly to Earth it goes from 2.5 mm to 301.1 mm,
more than a hundredfold, but counted in float steps at that distance from the centre of the body it
stays around one or two: 0.6 to 2.2 steps, series after series.

**It is the ground that moves, not only the craft.** The craft never moved and the spot never changed,
yet the height KSP computes held still while the collision surface wandered: by up to eleven
centimetres on Kerbin, thirty on Earth in Real Solar System. The ground itself is not built in the same
place twice. The full readings, and what else they show, are in
[What the numbers say](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/the-measurements-loading.md#what-the-numbers-say).

**With this mod, both stop moving.** On Kerbin, the craft's spread goes from more than twelve
centimetres to a few hundredths of a millimetre at most; on the other stock worlds too, what is left
stays in the hundredths of a millimetre, two tenths at worst, and on the Moon and Earth of Real Solar
System within a few tenths — far below the float step at any of these distances. *On rails* is still
identical on every line, so KSP put the craft back at the same place every time, and the craft now
comes to rest at the same place every time too. Under the craft, the ground reading does the same.
Three things to read in its column, *Difference*:

- **It stops varying**, by a factor of three hundred on Gilly, a thousand on the Mun, on the Moon and on
  Earth, two thousand on Minmus, and more than twenty thousand on Kerbin. On Kerbin the surface under
  the craft came back somewhere else over a range of eleven centimetres; it now comes back within four
  thousandths of a millimetre. That is the fix, and that is all of it.
- **It does not get smaller, and it is not supposed to.** It stops at a value the stock draws are
  scattered around. On Kerbin, on Minmus, on the Moon and on Earth the fixed reading falls well inside
  the range of the six loadings without this mod, and on Gilly just inside it. On the Mun it falls just
  below: six draws are few for a spread that wide, and a seventh could as well have landed under
  −40.76 mm. Put the loading that landed on −23.958 next to a fixed −40.765 and the fix looks like it
  made things worse; it did not, that line was luck. This mod does not choose a better number for that
  patch of ground; it stops drawing a new one at every loading.
- **What is left is no longer the ground, and it stays.** *Ground KSP computes* is what says the same
  spot was read every time: 189,650.347 mm on all six Kerbin lines, and `0.000` on the Minmus flats. On
  the Mun and on Gilly, where the ground is not level, that column wanders a little by itself — 0.018 mm
  over the six Gilly loadings — because a craft settling a hair to one side asks for the height of a
  slightly different point; on Real Solar System, without this mod, a craft pushed out of the ground
  lands elsewhere, and the column follows. On Gilly that is more than the spread of *Difference* under
  it, 0.009 mm: both columns follow the sample point together, and most of the wobble cancels between
  them. What remains of the spread is the craft, not the terrain. What remains of *Difference* itself is
  geometry: the collision mesh is made of flat triangles, and they miss what the ground does between two
  corners —
  [Terrain Precision Fix Diag 2 explains why a correct reading is not zero](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/this-mods-demonstration.md#why-a-correct-reading-is-not-zero).
  On that Gilly slope it is +37.5 mm, on that Kerbin slope +247.0 mm, on the Moon −114.0 mm, on Earth
  +121.9 mm, the same on every loading. Removing it would mean giving that mesh more triangles, which
  costs frames, for a gap nobody can feel.

**On the larger worlds, the stock spread is enough to make a craft jump, and Real Solar System's own
workaround does not catch it all.** It only acts beyond 10 cm, which leaves room for a craft buried by
a few centimetres to be pushed out: three jumps in fourteen loads on the Moon, one in six on Earth.
Turned off, the craft tipped over at the first load. With this mod, the craft did not move once in 75
loads, and the workaround, which still runs at every load, never had anything to correct — see
[Real Solar System's own workaround](#real-solar-systems-own-workaround).

**This is also the last proof that the culprit is the right one.** The fix changes where a subtraction
happens, and nothing else about the values placed; were the cause elsewhere, reordering that
subtraction would have left the spread untouched.
