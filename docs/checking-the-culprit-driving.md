# Checking the culprit: driving on while the world moves

Part of [Terrain Precision Fix](../README.md): the measurements that check [the culprit](the-culprit.md) under a rover that keeps driving, while the game moves its whole world, on stock and with this mod.

The ground under a craft the game sets down is measured in [Loading the same save](checking-the-culprit-loading.md), [Coming back to a craft left parked](checking-the-culprit-approach.md) and [Switching to a craft far away](checking-the-culprit-switching.md).

[Terrain Precision Fix Diag 2](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2) measures the
ground, and [Terrain Precision Fix Diag 3](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag3),
which only reads, says when the world moves. Each has its own page, with its method.

This fix is built on top of [KSP Community Fixes](https://github.com/KSPModdingLibs/KSPCommunityFixes),
the base most players run, so every campaign on this page is run in an install that has it: KSP 1.12.5
with Harmony, ModuleManager, KSP Community Fixes 1.41.1 and the two instruments — and this mod, or not.
*On stock*, below, means that install without this mod.

Every 500 m the craft you fly travels, KSP moves the floating origin back onto it, and with it the
terrain sphere: the translation of the frame the ground is converted through
([Why it is different at every load](the-culprit.md)). Stock places the landed quads again at each of
those shifts (`CelestialBody.PreciseUpdateQuadPositions`), through that new frame. A rover alone on the
flat grass south of the runway of the KSC drives due south; at each shift, three lines: just before it,
a few metres after it, and the same few metres farther on with no shift, which measures what the few
metres do on their own. Three shifts per run, since the grass is flat for a kilometre and a half only.
It is [the driving protocol](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/the-protocol-driving.md)
of Terrain Precision Fix Diag 2, on the save it publishes.

## The ground, over the shifts of a drive

**On stock**, two runs
([the readings](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/the-measurements-driving.md)).
*Difference* — the ground under the rover minus the height KSP computes for that spot — changes, in
millimetres:

| run, shift | across the shift | same distance, no shift |
|---|---|---|
| 1, 1 | **−4.197** | +1.166 |
| 1, 2 | **−11.692** | −0.740 |
| 1, 3 | **+9.588** | +0.009 |
| 2, 1 | **−11.782** | −0.228 |
| 2, 2 | **+10.749** | −0.476 |

**With this mod**, in that same install, on that same save, with this mod as the only difference. The
session is logged in [`diag/runs/driving-diag2-fix.log`](../diag/runs/driving-diag2-fix.log). On the
three lines of each shift kept, **Ground KSP computes** reads the same digits to within six
thousandths of a millimetre, and Diag 3 reads one shift of 500.0 m, to within five centimetres, on each
line taken just after a shift, and none on each line taken with no shift. In *Difference*, in
millimetres:

| shift | just before | just after | same distance again | across the shift | same distance, no shift |
|---|---|---|---|---|---|
| 1 | −2.349 | −2.134 | −1.776 | **+0.215** | +0.358 |
| 2 | −1.725 | −1.494 | −1.272 | **+0.231** | +0.222 |

![One run with this mod, read by Diag 3: nine lines, three shifts](../imgs/Diag2/on-driving/diag3.png)

![One run with this mod, read by Diag 2: nine lines, three shifts](../imgs/Diag2/on-driving/diag2.png)

The third shift is left out, as the protocol says, by its own third line: *Difference* changes by
+4.427 mm across the shift and by +5.193 mm over the same few metres with no shift. That is the same
spot, about a kilometre and a half south of the runway, that left out a shift of the second run on
stock; see [What the measurements say](#what-the-measurements-say).

## What the measurements say

**On stock, the ground moves under a rover in the middle of a drive.** Across a shift of the floating
origin, *Difference* changes by 4.2 to 11.8 mm, upwards or downwards, five times out of five; across the
same few metres with no shift, by 1.2 mm at most. Nothing is loaded and the scene does not change: the
quads under the rover are placed again through a new rounding of the frame, and land somewhere else.

**With this mod, a shift moves nothing.** Across the two shifts kept, *Difference* changes by 0.215 and
0.231 mm, no more than over the same few metres with no shift, 0.358 and 0.222 mm: what is left is what
the few metres do, the flat triangles of the ground following its curve. The log shows this mod placing
the quads again at every shift, a burst of up to 152 quads within the same second, which stock would
have placed somewhere else.

**On flat grass, the ground with this mod reads 1.3 to 2.3 mm below the height KSP computes**, where on
stock it reads 36 to 111 mm above it. That remainder is geometry, not a rounding: the collision mesh is
made of flat triangles, which miss what the ground does between two vertices
([Terrain Precision Fix Diag 2 explains why a correct reading is not zero](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/this-mods-demonstration.md#why-a-correct-reading-is-not-zero)).

**One spot, about a kilometre and a half south of the runway, reads far below the computed height**,
on stock (−292.7 and −284.9 mm in the two runs) and with this mod (−508.8 mm), and a few metres change
the reading there by up to 20 mm with no shift. The computed height is flat there, to a few thousandths
of a millimetre. Where that comes from is not established yet; it is not a shift of the origin, and the
shifts read there are left out.
