# Checking the culprit

Part of [Terrain Precision Fix](../README.md): the measurements that check [the culprit](the-culprit.md), on stock and with this mod.

Before touching anything, two instruments measure what stock does:
[Terrain Precision Fix Diag 1](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag) measures the craft, and
[Terrain Precision Fix Diag 2](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2) measures the ground. Each has its own page, with
its method and its protocol. Then the same campaigns are run again, with this mod installed.

## Terrain Precision Fix Diag 1, on stock: the craft

Terrain Precision Fix Diag 1 measures the distance from a landed capsule to the centre of the body,
twice per load: as the save hands the capsule back (*On rails*), and once it has settled on the ground
(*Settled*). How, and why those two readings, is in
[This mod's demonstration](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag#this-mods-demonstration).

Its campaigns, detailed in [The measurements](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag#the-measurements): stock KSP 1.12.5 with
nothing in `GameData` but that instrument. On each of four worlds, a lone capsule, then the same capsule
sitting on a small flat fuel tank, saved once on flat bare ground and loaded five or six times. Each
series uses its own spot, chosen by the rules of [its protocol](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag#the-protocol): no
`Moving Vessel` line in `KSP.log`, and a craft that does not slide.

*On rails* is the same on every load, to the micrometre: KSP puts the craft back at the same place.
*Settled* is not. Here is its spread, set against the step of a float at that distance from the centre
of the body:

| series | loads | distance to the centre | float step there | spread of *Settled* | in steps |
|---|---|---|---|---|---|
| Kerbin, capsule | 6 | 600.1 km | 62.5 mm | 135.5 mm | 2.2 |
| Kerbin, 2 parts | 6 | 600.1 km | 62.5 mm | 129.9 mm | 2.1 |
| Mun, capsule | 6 | 204.1 km | 15.6 mm | 18.1 mm | 1.2 |
| Mun, 2 parts | 6 | 204.1 km | 15.6 mm | 43.4 mm | 2.8 |
| Minmus, capsule | 6 | 60.0 km | 3.9 mm | 3.9 mm | 1.0 |
| Minmus, 2 parts | 5 | 60.0 km | 3.9 mm | 7.3 mm | 1.9 |
| Gilly, capsule | 6 | 15.9 km | 0.98 mm | 1.2 mm | 1.2 |
| Gilly, 2 parts | 6 | 16.6 km | 1.95 mm | 2.3 mm | 1.2 |

From one world to the next the spread varies a hundredfold, but counted in float steps it stays between
one and three: the size the culprit predicts.

That still only shows the craft moving, which is not by itself proof that the ground moved under it.
The second instrument is there for that.

## Terrain Precision Fix Diag 2, on stock: the ground

Terrain Precision Fix Diag 2 measures the ground, with no craft in the reading at all: the collision
surface a ray pointed straight down hits, against the height KSP computes for that same spot. The second
never moves; the first is what your landing legs touch. *Difference* is the first minus the second. How
both are read is in [This mod's demonstration](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/this-mods-demonstration.md).

Its campaigns, detailed in [Six loadings of the same save](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/six-loadings-of-the-same-save.md):
the same stock install, with nothing in `GameData` but that instrument; one save on each of the four
worlds, loaded six times, following [its protocol](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/the-protocol.md). Over those six loads:

| world | spread of *Difference* | spread of the height KSP computes |
|---|---|---|
| Kerbin | 106.6 mm | 0.000 mm |
| Mun | 8.8 mm | 0.035 mm |
| Minmus | 3.1 mm | 0.000 mm |
| Gilly | 3.7 mm | 0.046 mm |

The craft never moved and the spot never changed, yet the height KSP computes held still while the
collision surface wandered by up to ten centimetres. The ground itself is not built in the same place
twice. The full readings, and what else they show, are in
[What the numbers say](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/six-loadings-of-the-same-save.md#what-the-numbers-say).

## Terrain Precision Fix Diag 1, with this mod: the craft

This fix is meant for [KSP Community Fixes](https://github.com/KSPModdingLibs/KSPCommunityFixes), so it
is measured in an install that has it: KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes
1.41.1, this mod and Terrain Precision Fix Diag 1. The same test, on the same four worlds, with the same
two craft, loaded six times per series.

What it is compared with is the same install without this mod: Terrain Precision Fix Diag 1 ran those
campaigns too, on the same spots, and published their screenshots in [`imgs/kspcf`](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag/tree/main/imgs/kspcf). Their spread
is of the same order as on a stock install with nothing else: KSP Community Fixes does not change the
defect.

With one capsule, read off the four screenshots in [`imgs/Diag1/1part`](../imgs/Diag1/1part) (in each of
them, the bottom line is the loading in progress, still live, and is not counted):

| world | spread of *Settled*, without this mod | spread of *Settled*, with this mod |
|---|---|---|
| Kerbin | 134.5 mm | 0.004 mm |
| Mun | 20.7 mm | 0.031 mm |
| Minmus | 6.7 mm | 0.038 mm |
| Gilly | 3.3 mm | 0.058 mm |

With two parts, read off the four screenshots in [`imgs/Diag1/2parts`](../imgs/Diag1/2parts):

| world | spread of *Settled*, without this mod | spread of *Settled*, with this mod |
|---|---|---|
| Kerbin | 124.7 mm | 0.025 mm |
| Mun | 11.8 mm | 0.085 mm |
| Minmus | 4.8 mm | 0.023 mm |
| Gilly | 2.5 mm | 0.208 mm |

**On Kerbin, the spread goes from more than twelve centimetres to a few hundredths of a millimetre at
most.**

On the other worlds too, what is left stays in the hundredths of a millimetre, two tenths at worst, far
below the float step at any of these distances. *On rails* is still identical on every line, so KSP put
the craft back at the same place every time, and the craft now comes to rest at the same place every
time too.

In other words: reload the same save as many times as you like, and the craft comes back to the same
place, on ground that is in the same place. The coin toss of
[Why the moving ground matters](../README.md#why-the-moving-ground-matters) is gone — there is nothing left to push
the craft out of.

## Terrain Precision Fix Diag 2, with this mod: the ground

The same install, with Terrain Precision Fix Diag 2 instead: KSP 1.12.5 with Harmony, ModuleManager, KSP
Community Fixes 1.41.1, this mod and the instrument, one save per world, loaded six times. It is compared
with the campaigns Terrain Precision Fix Diag 2 ran on the same spots in that install without this mod,
published in [`imgs/kspcf`](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/tree/master/imgs/kspcf). On the Mun, on Minmus and on Gilly, those are also the spots of its
stock campaigns; on Kerbin the spot is another one, a grassy slope.

Read off the four screenshots in [`imgs/Diag2`](../imgs/Diag2) (as before, the bottom line of each is the
loading in progress, and is not counted):

| world | *Difference*, without this mod | *Difference*, with this mod | spread, without | spread, with |
|---|---|---|---|---|
| Kerbin | +199.797 to +307.930 mm | +246.972 to +246.976 mm | 108.1 mm | 0.004 mm |
| Mun | −23.958 to −38.925 mm | −40.756 to −40.769 mm | 15.0 mm | 0.013 mm |
| Minmus | −12.291 to −16.351 mm | −14.202 to −14.204 mm | 4.1 mm | 0.002 mm |
| Gilly | +37.426 to +40.391 mm | +37.511 to +37.520 mm | 3.0 mm | 0.009 mm |

Read the table by its last two columns, not its first two. Three things to read in them.

**The column stops varying**, by a factor of three hundred on Gilly, a thousand on the Mun, two thousand
on Minmus, and more than twenty thousand on Kerbin. On Kerbin the surface under the craft came back
somewhere else over a range of eleven centimetres; it now comes back within four thousandths of a
millimetre. That is the fix, and that is all of it.

**It does not get smaller, and it is not supposed to.** It stops at a value the stock draws are scattered
around. On Kerbin and on Minmus the fixed reading falls well inside the range of the six loadings without
this mod, and on Gilly just inside it. On the Mun it falls just below: six draws are few for a spread that
wide. The stock campaign of Terrain Precision Fix Diag 2, on that same spot, drew
[down to −44.363 mm](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/six-loadings-of-the-same-save.md#the-readings), and the twelve draws together cover −24.0 to −44.4 mm, around
the −40.76 mm this mod reads. Put the loading that landed on −23.958 next to a fixed −40.765 and the fix
looks like it made things worse; it did not, that line was luck. This mod does not choose a better number
for that patch of ground; it stops drawing a new one at every loading.

**What is left is no longer the ground, and it stays.** *Ground KSP computes* is what says the same spot
was read every time: 189,650.347 mm on all six Kerbin lines, and `0.000` on the Minmus flats. On the Mun
and on Gilly, where the ground is not level, that column wanders a little by itself — 0.018 mm over the
six Gilly loadings — because a craft settling a hair to one side asks for the height of a slightly
different point. On Gilly that is more than the spread of *Difference* under it, 0.009 mm: both columns
follow the sample point together, and most of the wobble cancels between them. What remains of the
spread is the craft, not the terrain. What remains of *Difference* itself is geometry: the collision mesh
is made of flat triangles, and they miss what the ground does between two corners —
[Terrain Precision Fix Diag 2 explains why a correct reading is not zero](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/this-mods-demonstration.md#why-a-correct-reading-is-not-zero).
On that Gilly slope it is +37.5 mm, on that Kerbin slope +247.0 mm, the same on every loading. Removing
it would mean giving that mesh more triangles, which costs frames, for a gap nobody can feel.

This is also the last proof that the culprit is the right one. The fix changes where a subtraction
happens, and nothing else about the values placed; were the cause elsewhere, reordering that
subtraction would have left the spread untouched.

## A save made without this mod

This mod brings the ground back to the same height on every load. It does not bring it back to the
height it had in the loading where a save was made without it.

A craft saved on stock sits on the ground of that one draw. If that ground was lower than the corrected
one, the craft now comes back slightly inside the ground and gets pushed out — and since the ground no
longer changes, it gets pushed out **on every load of that save**, the same way each time. Reloading
the same file will not make it go away.

Let the craft settle with this mod installed and save again: from that save on, the craft and the
ground agree. This follows from the measurements above; it has not been measured on its own.

