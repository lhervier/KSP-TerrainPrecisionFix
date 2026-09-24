# Checking the culprit: switching to a craft far away

Part of [Terrain Precision Fix](../README.md): the measurements that check [the culprit](the-culprit.md) on a craft switched to from two kilometres away, on stock and with this mod.

The two other ways a craft is put back onto the ground are measured in [Loading the same save](checking-the-culprit-loading.md) and [Coming back to a craft left parked](checking-the-culprit-approach.md).

Two instruments take the readings:
[Terrain Precision Fix Diag 1](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag) measures the craft, and
[Terrain Precision Fix Diag 2](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2) measures the ground. Each has its own page, with
its method and its protocol.

This fix is meant for [KSP Community Fixes](https://github.com/KSPModdingLibs/KSPCommunityFixes), so
every campaign on this page is run in an install that has it: KSP 1.12.5 with Harmony, ModuleManager,
KSP Community Fixes 1.41.1 and one of the two instruments — and this mod, or not. *On stock*, below,
means that install without this mod.

Two craft landed 1.97 km apart on Kerbin. The save is loaded while flying the rover: the capsule is
loaded too, but packed, held where the save put it. Then the game's *switch vessel* key flies the
capsule, and physics takes it over. Six rounds, each starting by loading the same save. It is the
switching protocol of both instruments —
[Terrain Precision Fix Diag 1](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag/blob/main/docs/the-protocol-switching.md)
and [Terrain Precision Fix Diag 2](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/the-protocol-switching.md) —
with a save made without this mod.

## The craft, over six rounds

**On stock**
([the readings](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag/blob/main/docs/the-measurements-switching.md)).
As the save opens, *On rails* reads the same height on all six rounds, within two thousandths of a
millimetre. *Moved*, once the switch has handed the capsule to physics:

| round | 1 | 2 | 3 | 4 | 5 | 6 |
|---|---|---|---|---|---|---|
| *Moved*, without this mod | +10.827 mm | −44.514 mm | −83.507 mm | −84.189 mm | +20.268 mm | +18.322 mm |
| *Moved*, with this mod | −31.836 mm | −31.858 mm | −31.854 mm | −31.856 mm | −31.842 mm | −31.843 mm |

**With this mod**, in that same install, on that same save, with this mod as the only difference: the
screenshot is [`imgs/Diag1/on-switch/six-rounds.png`](../imgs/Diag1/on-switch/six-rounds.png), and the
session is logged in [`diag/runs/switching-diag1-fix.log`](../diag/runs/switching-diag1-fix.log). *On
rails* still reads the same height, within three thousandths of a millimetre.

**The capsule comes to rest across a spread of 104.5 mm without this mod, and 0.022 mm with it.**

The −31.8 mm left is the same on every round, so it is not the defect. The save was made without this
mod: the height it holds the capsule at was taken on one of the grounds stock builds, and this mod
builds the ground in one place every time — not that one. It is the case of
[Existing saves](limits-and-solutions.md#existing-saves).

## The ground, over six rounds

**On stock**
([the readings](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/the-measurements-switching.md)).
The height KSP computes reads 64,784.828 mm on all twelve lines. *Difference*, as the save opens:

| round | 1 | 2 | 3 | 4 | 5 | 6 |
|---|---|---|---|---|---|---|
| *Difference*, without this mod | −54.655 mm | +44.378 mm | −57.166 mm | +63.188 mm | −26.893 mm | +8.032 mm |
| *Difference*, with this mod | −1.696 mm | −1.695 mm | −1.695 mm | −1.696 mm | −1.696 mm | −1.697 mm |

On stock, the switch that follows does not move it: the line taken after it reads the same ground to
within five thousandths of a millimetre, the capsule settling a hair and moving the spot the ray is
fired at.

**With this mod**, in that same install: the screenshot is
[`imgs/Diag2/on-switch/six-rounds.png`](../imgs/Diag2/on-switch/six-rounds.png), and the session is
logged in [`diag/runs/switching-diag2-fix.log`](../diag/runs/switching-diag2-fix.log). All twelve lines,
before and after the switch, read between −1.694 and −1.697 mm.

**The ground spreads over 120.4 mm without this mod, and 0.003 mm with it.**

## What the two say together

The ground under the capsule is built when the save is loaded, while the capsule is two kilometres
from the craft being flown and not flown itself — and switching to it does not move it. Meanwhile the
capsule is held at the height the save recorded. So until the switch, it sits inside that ground or
above it, and it is when physics takes it over that it comes to rest on it: by as much as the ground
moved. With this mod, the ground is built in the same place at every loading, and the capsule comes to
rest by the same amount every time.
