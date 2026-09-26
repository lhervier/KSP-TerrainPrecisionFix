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

A craft is set down on bare ground, saved once, and that same save is loaded six times over. The
craft never changes, the spot never changes, and nothing is touched between two loads.

## The craft, over six loads

Terrain Precision Fix Diag 1 measures the distance from a landed capsule to the centre of the body,
twice per load: as the save hands the capsule back (*On rails*), and once it has settled on the ground
(*Settled*). How, and why those two readings, is in
[This mod's demonstration](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag#this-mods-demonstration).

**On stock.** Its campaigns, detailed in
[The measurements: loading the same save](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag/blob/main/docs/the-measurements-loading.md).
On each of four worlds, a lone capsule, then the same capsule sitting on a small flat fuel tank, saved
once and loaded six times. Each series uses its own spot, chosen by the rules of
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

**With this mod.** The same test, in the same install, on the same four worlds, with the same two
craft, loaded six times per series. The eight series, read off the screenshots in
[`imgs/Diag1/on-load/1part`](../imgs/Diag1/on-load/1part) and
[`imgs/Diag1/on-load/2parts`](../imgs/Diag1/on-load/2parts) — in each of them, the bottom line is the
loading in progress, still live, and is not counted:

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

## The ground, over six loads

Terrain Precision Fix Diag 2 measures the ground, with no craft in the reading at all: the collision
surface a ray pointed straight down hits, against the height KSP computes for that same spot. The second
never moves; the first is what your landing legs touch. *Difference* is the first minus the second. How
both are read is in [This mod's demonstration](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/this-mods-demonstration.md).

**On stock.** Its campaigns, detailed in [The measurements: loading the same save](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/the-measurements-loading.md):
the same install, with that instrument; one save on each of the four worlds, loaded six times,
following [its protocol](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/the-protocol-loading.md). Over those six loads:

| world | spread of *Difference* | spread of the height KSP computes |
|---|---|---|
| Kerbin | 108.1 mm | 0.039 mm |
| Mun | 15.0 mm | 0.015 mm |
| Minmus | 4.1 mm | 0.000 mm |
| Gilly | 3.0 mm | 0.018 mm |

**With this mod.** The same install, the same saves, on the same spots, loaded six times; this mod
is the only difference.

Read off the four screenshots in [`imgs/Diag2`](../imgs/Diag2) (as before, the bottom line of each is the
loading in progress, and is not counted):

| world | *Difference*, without this mod | *Difference*, with this mod | spread, without | spread, with |
|---|---|---|---|---|
| Kerbin | +199.797 to +307.930 mm | +246.972 to +246.976 mm | 108.1 mm | 0.004 mm |
| Mun | −23.958 to −38.925 mm | −40.756 to −40.769 mm | 15.0 mm | 0.013 mm |
| Minmus | −12.291 to −16.351 mm | −14.202 to −14.204 mm | 4.1 mm | 0.002 mm |
| Gilly | +37.426 to +40.391 mm | +37.511 to +37.520 mm | 3.0 mm | 0.009 mm |

## On larger worlds: Real Solar System

The culprit predicts a spread of one or two float steps, and a float's step doubles each time the
distance to the centre of the body crosses a power of two. [Real Solar System](https://github.com/KSP-RO/RealSolarSystem)
replaces the planets with the real ones, much larger, which puts that prediction to the test.

The install is the one above, plus Real Solar System 20.1.3.0 and what it requires (Kopernicus, Modular
Flight Integrator, KSPTextureLoader, the RSS textures), with both instruments. The craft is a capsule on
an empty fuel tank. Real Solar System ships a workaround of its own for the symptom: at every load, it
moves a landed craft back onto the ground when it is more than 10 cm off, and leaves it where it is
below that. Every series below keeps it on, as players have it, unless said otherwise; near the KSC on
Earth the craft is in *prelaunch*, where stock KSP runs the same pass instead. The saves, the
screenshots, the logs and the full readings are in
[Rescaled systems: Real Solar System](limits-and-solutions/rescaled-systems-real-solar-system.md).

**The craft, with Diag 1**, over six loads:

| series | distance to the centre | float step there | spread of *Settled*, without this mod | in steps | with this mod |
|---|---|---|---|---|---|
| the Moon | 1 744.4 km | 125 mm | 262.6 mm | 2.1 | 0.395 mm |
| Earth, near the KSC | 6 371.1 km | 500 mm | 301.1 mm | 0.6 | 0.178 mm |

**The ground, with Diag 2**, over six loads:

| series | spread of *Difference*, without this mod | with this mod | spread of the height KSP computes, without / with |
|---|---|---|---|
| the Moon | 241.2 mm | 0.226 mm | 0.541 / 0.002 mm |
| Earth, near the KSC | 301.2 mm | 0.289 mm | 2.067 / 0.036 mm |

**Reloading until something happens**, the same saves, watching the craft. Without this mod,
fourteen loads: the workaround moved the craft up in one block six times, and three times the craft came
back buried by less than its 10 cm and jumped. With its workaround turned off, the craft tipped over at
the very first load. With this mod, twenty-four loads in a row without an instrument: the craft never
moved, and the workaround, which still runs at every load, never had anything to move. On Earth, with
this mod, fifty-one loads in a row, with the workaround running on a landed craft and with stock's own
pass on a craft in *prelaunch*: the craft never moved either.

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
place twice. The full readings of the stock worlds, and what else they show, are in
[What the numbers say](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/the-measurements-loading.md#what-the-numbers-say).

**With this mod, both stop moving.** On Kerbin, the craft's spread goes from more than twelve
centimetres to a few hundredths of a millimetre at most; on the other stock worlds too, what is left
stays in the hundredths of a millimetre, two tenths at worst, and on the Moon and Earth of Real Solar
System within a few tenths — far below the float step at any of these distances. *On rails* is still
identical on every line, so KSP put the craft back at the same place every time, and the craft now
comes to rest at the same place every time too. Under the craft, the ground reading does the same.
Three things to read in its column, *Difference*:

- **It stops varying**, by a factor of three hundred on Gilly, a thousand on the Mun, two thousand on
  Minmus, and more than twenty thousand on Kerbin. On Kerbin the surface under the craft came back
  somewhere else over a range of eleven centimetres; it now comes back within four thousandths of a
  millimetre. That is the fix, and that is all of it.
- **It does not get smaller, and it is not supposed to.** It stops at a value the stock draws are
  scattered around. On Kerbin and on Minmus the fixed reading falls well inside the range of the six
  loadings without this mod, and on Gilly just inside it. On the Mun it falls just below: six draws are
  few for a spread that wide, and a seventh could as well have landed under −40.76 mm. Put the loading
  that landed on −23.958 next to a fixed −40.765 and the fix looks like it made things worse; it did
  not, that line was luck. This mod does not choose a better number for that patch of ground; it stops
  drawing a new one at every loading.
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
  On that Gilly slope it is +37.5 mm, on that Kerbin slope +247.0 mm, on the Moon −114 mm, the same on
  every loading. Removing it would mean giving that mesh more triangles, which costs frames, for a gap
  nobody can feel.

**On the larger worlds, the stock spread is enough to make a craft jump, and Real Solar System's own
workaround does not catch it all.** It only acts beyond 10 cm, which leaves room for a craft buried by
a few centimetres to be pushed out: three jumps in fourteen loads on the Moon. Turned off, the craft
tipped over at the first load. With this mod, nothing moves any more, and the workaround, which still
runs at every load, never has anything to correct in this scenario.

**This is also the last proof that the culprit is the right one.** The fix changes where a subtraction
happens, and nothing else about the values placed; were the cause elsewhere, reordering that
subtraction would have left the spread untouched.
