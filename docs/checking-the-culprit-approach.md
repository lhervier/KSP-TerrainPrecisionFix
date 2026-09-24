# Checking the culprit: coming back to a craft left parked

Part of [Terrain Precision Fix](../README.md): the measurements that check [the culprit](the-culprit.md) on a craft left parked and come back to, in the middle of a flight, on stock and with this mod.

The two other ways a craft is put back onto the ground are measured in [Loading the same save](checking-the-culprit-loading.md) and [Switching to a craft far away](checking-the-culprit-switching.md).

Two instruments take the readings:
[Terrain Precision Fix Diag 1](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag) measures the craft, and
[Terrain Precision Fix Diag 2](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2) measures the ground. Each has its own page, with
its method and its protocol.

This fix is built on top of [KSP Community Fixes](https://github.com/KSPModdingLibs/KSPCommunityFixes),
the base most players run, so every campaign on this page is run in an install that has it: KSP 1.12.5 with Harmony, ModuleManager,
KSP Community Fixes 1.41.1 and one of the two instruments — and this mod, or not. *On stock*, below,
means that install without this mod.

The other way a craft meets the ground, and the one you cannot avoid by never quitting: a craft is left
parked while a rover drives away from it, past 2500 m, where the game unloads it — then comes back
within 200 m, where physics takes the parked craft over again. No save is loaded at any point and the
scene is never changed: one single flight, six round trips in a row, on Kerbin. It is
[the second protocol](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag#the-protocol) of Terrain
Precision Fix Diag 1.

## The craft, over six round trips

**On stock**, in an install with KSP Community Fixes and that instrument
([the readings](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag#the-measurements)). *Moved* —
how far the craft ends up from the height it was handed back at — reads:

| round trip | 1 | 2 | 3 | 4 | 5 | 6 |
|---|---|---|---|---|---|---|
| *Moved* | +12.570 mm | −17.549 mm | +10.969 mm | −7.749 mm | −7.484 mm | +19.170 mm |

Same craft, same spot, same flight: 7.5 to 19.2 mm every time, upwards as often as downwards, and
never the same twice. Over the six, the craft comes to rest across a spread of 21.8 mm — a fraction
of a float step, where six loads of a save spread it over two of them. The height it is handed back
at, read before each round trip and after it, never moves by more than six thousandths of a
millimetre: what changes is what it settles onto.

**With this mod**, in that same install, on that same save, with this mod as the only difference. The
six screenshots are in [`imgs/Diag1/on-approach`](../imgs/Diag1/on-approach), and the session is logged in
[`diag/runs/approach-diag1-fix.log`](../diag/runs/approach-diag1-fix.log).

| round trip | 1 | 2 | 3 | 4 | 5 | 6 |
|---|---|---|---|---|---|---|
| *Moved*, without this mod | +12.570 mm | −17.549 mm | +10.969 mm | −7.749 mm | −7.484 mm | +19.170 mm |
| *Moved*, with this mod | −0.023 mm | −0.020 mm | −0.043 mm | −0.041 mm | +0.086 mm | −0.022 mm |

**Over the six round trips, the craft comes to rest across a spread of 21.8 mm without this mod and
0.094 mm with it.** What is left is of the same order as after a load: hundredths of a millimetre,
against a float step of 62.5 mm there.

In other words: reload the same save as many times as you like, or leave a craft parked and come back
to it in the middle of a flight — either way it comes back to the same place, on ground that is in the
same place. The coin toss of
[Why the moving ground matters](../README.md#why-the-moving-ground-matters) is gone — there is nothing left to push
the craft out of.

## The ground, over six round trips

**On stock**, with Terrain Precision Fix Diag 2 and
[its own approach protocol](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/the-protocol-approach.md)
— the same one, on the same spot
([the readings](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/the-measurements-approach.md)).
The height KSP computes reads the same digits on every line of the six round trips. *Difference*,
across each round trip:

| round trip | 1 | 2 | 3 | 4 | 5 | 6 |
|---|---|---|---|---|---|---|
| *Difference* moved by | −21.782 mm | +5.904 mm | +4.563 mm | +4.689 mm | −6.096 mm | +2.676 mm |

The ground itself comes back somewhere else on every round trip, over a spread of 21.8 mm. And it has
already moved by the time the craft is back in range, while it is still packed: from then to the
moment physics takes the craft over, it moves by 0.040 mm at most.

**With this mod**, in that same install, on that same save, with this mod as the only difference. The
six screenshots are in [`imgs/Diag2/on-approach`](../imgs/Diag2/on-approach), and the session is logged in
[`diag/runs/approach-diag2-fix.log`](../diag/runs/approach-diag2-fix.log).

| round trip | 1 | 2 | 3 | 4 | 5 | 6 |
|---|---|---|---|---|---|---|
| *Difference* moved by, without this mod | −21.782 mm | +5.904 mm | +4.563 mm | +4.689 mm | −6.096 mm | +2.676 mm |
| *Difference* moved by, with this mod | +0.006 mm | −0.003 mm | +0.006 mm | +0.001 mm | +0.001 mm | −0.002 mm |

On the fifth round trip, the last line was not recorded: the value used is the one the reading in
progress shows under 200 m, −2.260 mm, and the first line of the sixth round trip reads the same.

**Over the whole series, the ground comes back within a spread of 21.8 mm without this mod, and
0.011 mm with it.** The ground under the craft stays where it is while the craft is away.
