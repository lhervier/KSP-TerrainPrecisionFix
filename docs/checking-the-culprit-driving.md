# Checking the culprit: driving on while the world moves

Part of [Terrain Precision Fix](../README.md): the measurements that check the two culprits, [the ground](the-culprit-ground.md) and [the statics](the-culprit-statics.md), under a rover that keeps driving, while the game moves its whole world, on stock and with this mod, on Kerbin and on Earth in Real Solar System: on the grass, and on the runway of the KSC with the grass beside it.

The ground under a craft the game sets down is measured in [Loading the same save](checking-the-culprit-loading.md), [Coming back to a craft left parked](checking-the-culprit-approach.md) and [Switching to a craft far away](checking-the-culprit-switching.md).

[KSP Diag - Terrain Height](https://github.com/lhervier/KSP-Diag-TerrainHeight) measures the
ground, and [KSP Diag - Floating Origin](https://github.com/lhervier/KSP-Diag-FloatingOrigin),
which only reads, says when the world moves. Each has its own page, with its method.

This fix is built on top of [KSP Community Fixes](https://github.com/KSPModdingLibs/KSPCommunityFixes),
the base most players run, so every campaign on this page is run in an install that has it: KSP 1.12.5
with Harmony, ModuleManager, KSP Community Fixes 1.41.1, the two instruments and
[KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer), which drives the rover — and this mod, or
not; on Earth, [Real Solar System](https://github.com/KSP-RO/RealSolarSystem) 20.1.3.0 and what it
requires as well. *On stock*, below, means that install without this mod.

Every 500 m the craft you fly travels, KSP moves the floating origin back onto it, and with it the
terrain sphere: the translation of the frame the ground is converted through
([Why it is different at every load](the-culprit-ground.md#why-it-is-different-at-every-load)). Stock places the landed quads again at each of
those shifts (`CelestialBody.PreciseUpdateQuadPositions`), through that new frame. A rover alone on the
grass south of the runway of the KSC drives due south; at each shift, three lines: just before it, a few
metres after it, and the same few metres farther on with no shift, which measures what the few metres
do on their own. A shift counts when the first change is at least three times the second. It is
[the driving protocol](https://github.com/lhervier/KSP-Diag-TerrainHeight/blob/master/docs/the-protocol-driving.md)
of KSP Diag - Terrain Height, on the two saves it publishes, played by its script,
[`run-driving.py`](https://github.com/lhervier/KSP-Diag-TerrainHeight/blob/master/docs/the-protocol-driving.md#played-by-a-script):
one run of three shifts on each body, without this mod and with it.

The terrain sphere carries the runway of the KSC too. A rover alone by it reads a spot on the grass, G,
and a spot on the deck, P, two or three times each before a move of the origin and twice after it, nine
lines a move. A script drives the rover through
[KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer) and parks it on the same spots to within a
centimetre. It is the
[protocol of the runway and the grass while the world moves](https://github.com/lhervier/KSP-Diag-TerrainHeight/blob/master/docs/the-protocol-driving-runway.md)
of KSP Diag - Terrain Height, on the saves it publishes, in the install above with KSP-MCPServer
added; on Earth, Real Solar System is built without its runway fix, which keeps the floating origin from
moving at 500 m once a craft has rolled onto the deck
([`rss-20.1.3-without-its-runway-fix.diff`](../diag/rss-runway-fix/rss-20.1.3-without-its-runway-fix.diff);
see [Real Solar System: the runway fix](limits-and-solutions/rss/the-runway-fix.md)). This mod corrects
the terrain and the statics separately, each with its own setting (`fixTerrain`, `fixStatics`), so on
Kerbin the runway is read three ways: on stock, with the terrain fix alone, and with both. On Earth, on
stock and with this mod as it is installed. Across each move, the mean of the lines after minus the mean
of the lines before.

## On Kerbin

**On stock**, one run
([the readings](https://github.com/lhervier/KSP-Diag-TerrainHeight/blob/master/docs/the-measurements-driving.md#on-kerbin)).
*Difference* — the ground under the rover minus the height KSP computes for that spot — changes, in
millimetres:

| shift | across the shift | same distance, no shift | counts |
|---|---|---|---|
| 1 | +1.216 | +1.356 | no |
| 2 | **+26.586** | −4.535 | yes |
| 3 | **−24.665** | +5.656 | yes |

**With this mod**, in that same install, on that same save, with this mod as the only difference. The
session is logged in [`diag/runs/driving-diag2-fix.log`](../diag/runs/driving-diag2-fix.log); what the
script printed is in [`driving-diag2-fix-script.txt`](../diag/runs/driving-diag2-fix-script.txt), and
every line it recorded in [`driving-diag2-fix-lines.json`](../diag/runs/driving-diag2-fix-lines.json).
On the three lines of each shift, **Ground KSP computes** reads the same digits to within six
thousandths of a millimetre, and Diag FloatingOrigin reads one shift of 500.0 m, to within two
centimetres, on each line taken just after a shift, and none on each line taken with no shift. In
*Difference*, in millimetres:

| shift | just before | just after | same distance again | across the shift | same distance, no shift |
|---|---|---|---|---|---|
| 1 | −2.392 | −2.216 | −1.975 | +0.176 | +0.241 |
| 2 | −2.100 | −1.606 | −1.536 | +0.493 | +0.070 |
| 3 | −517.280 | −514.190 | −511.705 | +3.089 | +2.485 |

![One run on Kerbin with this mod, read by Diag FloatingOrigin: nine lines, three shifts](../imgs/Diag2/on-driving/diag3.png)

![One run on Kerbin with this mod, read by Diag TerrainHeight: nine lines, three shifts](../imgs/Diag2/on-driving/diag2.png)

The third shift is read at a spot about a kilometre and a half south of the runway that reads far below
the computed height, with this mod as on stock; see [What the measurements say](#what-the-measurements-say).

**By the runway of the KSC.** Across each move, in millimetres:

| | move | the grass, G | the deck, P | the step, P − G | spread at a spot, at most |
|---|---|---|---|---|---|
| on stock | 1 | **−54.833** | **−54.847** | −0.014 | 0.073 |
| | 2 | **+53.816** | **+53.776** | −0.041 | 0.062 |
| with the terrain fix alone | 1 | +0.006 | **+41.587** | **+41.581** | 0.019 |
| | 2 | +0.004 | **−22.311** | **−22.315** | 0.017 |
| with both fixes | 1 | +0.007 | −0.019 | −0.026 | 0.034 |
| | 2 | −0.002 | −0.068 | −0.066 | 0.102 |

**On stock**
([the readings](https://github.com/lhervier/KSP-Diag-TerrainHeight/blob/master/docs/the-measurements-driving-runway.md)),
one run, a third move left out where the grass is no longer flat.

**With the terrain fix alone**, an earlier run, played with an earlier version of the script, `fixStatics = false` in this mod's settings, the only line changed. The
session is logged in
[`diag/runs/driving-runway-diag2-fix-terrain-only.log`](../diag/runs/driving-runway-diag2-fix-terrain-only.log),
what the script printed in
[`driving-runway-diag2-fix-terrain-only-script.txt`](../diag/runs/driving-runway-diag2-fix-terrain-only-script.txt)
and every line it recorded in
[`driving-runway-diag2-fix-terrain-only-lines.json`](../diag/runs/driving-runway-diag2-fix-terrain-only-lines.json).
At the second move, the rover stalled against the lip of the deck on its way to P and stopped 0.79 m
from the spot, at the same place each time: its readings there agree to 0.017 mm. On the last visit to
P, after the move, it stopped 9.7 m away, and that line is left out: P has one line after that move.

![With the terrain fix alone, the first move, read by Diag TerrainHeight](../imgs/Diag2/on-driving-runway/terrain-only-move1-diag2.png)

**With both fixes**, this mod as it is installed, the same script as on stock. The session is logged in
[`diag/runs/driving-runway-diag2-fix.log`](../diag/runs/driving-runway-diag2-fix.log), what the script
printed in [`driving-runway-diag2-fix-script.txt`](../diag/runs/driving-runway-diag2-fix-script.txt) and
every line it recorded in
[`driving-runway-diag2-fix-lines.json`](../diag/runs/driving-runway-diag2-fix-lines.json). G stood 476 to
482 m from the origin before each move, and the rover stopped 12 to 17 cm from each spot. The third
move is left out, as on stock: the lines taken at a spot there spread over up to 26.7 mm.

![With both fixes, the first move, read by Diag TerrainHeight](../imgs/Diag2/on-driving-runway/fix-move1-diag2.png)

## On Earth

The ground around the KSC of Real Solar System is not flat to the millimetre: the height KSP computes
changes by about a centimetre per metre, so the rover stops within two metres of each shift, and again
two metres on.

**On stock**, one run
([the readings](https://github.com/lhervier/KSP-Diag-TerrainHeight/blob/master/docs/the-measurements-driving.md#on-earth)).
In millimetres:

| shift | across the shift | same distance, no shift | counts |
|---|---|---|---|
| 1 | **−159.139** | −24.276 | yes |
| 2 | −242.981 | −81.124 | no, 2.995 times |
| 3 | +231.530 | +82.033 | no, 2.8 times |

**With this mod**, in that same install, on that same save. The session is logged in
[`diag/runs/driving-earth-rss-diag2-fix.log`](../diag/runs/driving-earth-rss-diag2-fix.log); what the
script printed is in [`driving-earth-rss-diag2-fix-script.txt`](../diag/runs/driving-earth-rss-diag2-fix-script.txt),
and every line it recorded in [`driving-earth-rss-diag2-fix-lines.json`](../diag/runs/driving-earth-rss-diag2-fix-lines.json).
Diag FloatingOrigin reads one shift of 500.0 m, to within two centimetres, on each line taken just
after a shift, and none on each line taken with no shift; its first line reads two, the moves the game
makes as the scene opens on Earth. In *Difference*, in millimetres:

| shift | just before | just after | same distance again | across the shift | same distance, no shift |
|---|---|---|---|---|---|
| 1 | −94.249 | −85.967 | −118.103 | +8.283 | −32.136 |
| 2 | +128.235 | +180.144 | +126.215 | +51.910 | −53.929 |
| 3 | −18.674 | +28.258 | +64.241 | +46.933 | +35.983 |

![One run on Earth with this mod, read by Diag FloatingOrigin: nine lines, three shifts](../imgs/Diag2/on-driving/earth-diag3.png)

![One run on Earth with this mod, read by Diag TerrainHeight: nine lines, three shifts](../imgs/Diag2/on-driving/earth-diag2.png)

None of the three shifts counts: each changes *Difference* about as much as the same few metres do with
no shift, or less. On that slope, the height KSP computes changes between the lines of a shift by up to
90 mm, with this mod as on stock.

**By the runway of the KSC at Cape Canaveral.** Across each move, in millimetres:

| | move | the grass, G | the deck, P | the step, P − G | spread at a spot, at most |
|---|---|---|---|---|---|
| on stock | 1 | **−231.864** | **−231.835** | +0.028 | 0.223 |
| | 2 | **+34.188** | **+33.838** | −0.350 | 0.273 |
| with this mod | 1 | +0.053 | +0.034 | −0.020 | 0.137 |
| | 2 | +0.119 | −0.001 | −0.119 | 0.282 |

**On stock**
([the readings](https://github.com/lhervier/KSP-Diag-TerrainHeight/blob/master/docs/the-measurements-driving-runway.md#on-earth)),
one run, two moves.

**With this mod**, on the same save. The session is logged in
[`diag/runs/driving-runway-earth-rss-diag2-fix.log`](../diag/runs/driving-runway-earth-rss-diag2-fix.log),
what the script printed in
[`driving-runway-earth-rss-diag2-fix-script.txt`](../diag/runs/driving-runway-earth-rss-diag2-fix-script.txt)
and every line it recorded in
[`driving-runway-earth-rss-diag2-fix-lines.json`](../diag/runs/driving-runway-earth-rss-diag2-fix-lines.json).
The rover stopped 13 to 16 cm from each spot, and G stood 475 to 477 m from the origin before each move;
Diag FloatingOrigin reads a move of 500.0 m, to within three centimetres, on the first line after each. The log shows
this mod taking the KSC out of its sphere once, as the save loads, and holding it there through both
moves.

![On Earth, with this mod, the first move, read by Diag TerrainHeight](../imgs/Diag2/on-driving-runway/earth-fix-move1-diag2.png)

The screenshots of every move by the runway, on Kerbin and on Earth, read by both instruments, are in
[`imgs/Diag2/on-driving-runway/`](../imgs/Diag2/on-driving-runway/).

## What the measurements say

**On stock, the ground moves under a rover in the middle of a drive.** Across a shift of the floating
origin, *Difference* changes by 26.6 and 24.7 mm on Kerbin, two shifts kept out of three, against
5.7 mm at most over the same few metres with no shift; on Earth, by 159 to 243 mm, against 24 to 82 mm
with no shift on a slope, one shift kept and the two others just short of the rule. Nothing is loaded
and the scene does not change: the quads under the rover are placed again through a new rounding of the
frame, and land somewhere else. On Earth, in earlier runs played by hand, the rover was seen to jump
when the ground rose under it
([KSP Diag - Terrain Height](https://github.com/lhervier/KSP-Diag-TerrainHeight/blob/master/docs/the-measurements-driving.md#on-earth)).

**With this mod, a shift moves nothing.** On Kerbin, on the flat grass, *Difference* changes by 0.18 and
0.49 mm across the two shifts, against 26.6 mm on stock at the second; on Earth, by 8 to 52 mm, against
32 to 54 mm over the same few metres with no shift, on a slope: no shift stands out from what the few
metres do. The logs show this mod placing the quads again at every shift, a burst of 140 to 152 quads
within the same second on Kerbin, 184 to 188 on Earth, which stock would have placed somewhere else.

**The runway moves too, and correcting the terrain is not enough.** On stock, at every move of the
floating origin, the deck of the KSC moves — by −54.85 and +53.78 mm on Kerbin — together with the grass
beside it, to within four hundredths of a millimetre: both hang from the terrain sphere, whose position
is written in float anew at each move, and both are carried by that same new rounding. With the terrain
fix alone, the grass holds, within six thousandths of a millimetre, and the deck still moves on its own,
by +41.59 and −22.31 mm: the terrain fix does not reach a static. With both fixes, neither moves, within
seven hundredths of a millimetre on the deck over two moves: the statics fix holds the runway through a
move of the origin as it does through a loading.

**On Earth, where a float's step is eight times larger, the same.** On stock, the deck and the grass
move together, by −231.84 and +33.84 mm: a rover rolling on the runway is no better off than one on the
grass, which moves by up to 243 mm across a move. With this mod, the deck moves by 0.034 mm at most, and
the grass by 0.119 mm, within the spread of the lines taken at the same spot, 0.282 mm: the origin
moves, and nothing under the rover does.

**On flat grass, the ground with this mod reads 1.5 to 2.4 mm below the height KSP computes**, where on
stock it reads anywhere from 16 mm below it to 10 mm above it. That remainder is geometry, not a
rounding: the collision mesh is made of flat triangles, which miss what the ground does between two
vertices
([KSP Diag - Terrain Height explains why a correct reading is not zero](https://github.com/lhervier/KSP-Diag-TerrainHeight/blob/master/docs/this-mods-demonstration.md#why-a-correct-reading-is-not-zero)).

**One spot on Kerbin, about a kilometre and a half south of the runway, reads far below the computed
height**, on stock (−437 to −462 mm) and with this mod (−512 to −517 mm), and a few metres change the
reading there by 2.5 mm with this mod, 5.7 mm on stock, with no shift. The computed height is flat
there, to a few thousandths of a millimetre. Where that comes from is not established yet; it is not a
shift of the origin.
