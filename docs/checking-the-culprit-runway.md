# Checking the culprit: the runway and the grass beside it

Part of [Terrain Precision Fix](../README.md): the measurements that check [the second culprit](the-culprit.md#a-second-culprit-the-statics), the statics, on the runway of the KSC and on a runway placed by Kerbal Konstructs on the Mun, each with the ground beside it, on stock and with this mod.

The ground alone is measured in [Loading the same save](checking-the-culprit-loading.md), [Coming back to a craft left parked](checking-the-culprit-approach.md) and [Switching to a craft far away](checking-the-culprit-switching.md).

Two instruments take the readings:
[Terrain Precision Fix Diag 1](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag) measures the craft, and
[Terrain Precision Fix Diag 2](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2) measures the ground. Each has its own page, with
its method and its protocol.

This fix is built on top of [KSP Community Fixes](https://github.com/KSPModdingLibs/KSPCommunityFixes),
the base most players run, so every campaign on this page is run in an install that has it: KSP 1.12.5 with Harmony, ModuleManager,
KSP Community Fixes 1.41.1 and one of the two instruments — and this mod, or not; on the Mun, Kerbal
Konstructs 1.12.3 and CustomPreLaunchChecks 1.8.1, which it requires, as well. *On stock*, below, means
that install without this mod.

Two identical craft, one on a runway and one on the ground beside it. The save is loaded, a line is
recorded on the craft on the ground, then the game's *switch vessel* key flies the craft on the runway
and a second line is recorded there. Six loadings of the same save, two lines each. It is the runway
protocol of both instruments —
[Terrain Precision Fix Diag 1](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag/blob/main/docs/the-protocol-runway.md)
and [Terrain Precision Fix Diag 2](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/the-protocol-runway.md)
— on the two saves they publish:

- **on Kerbin**, the runway of the KSC and the grass beside it, 152 m apart;
- **on the Mun**, a runway placed by [Kerbal Konstructs](https://github.com/KSP-RO/Kerbal-Konstructs) —
  the model of the KSC's runway it offers — and the ground 42 m from it. The runway hangs from a group
  of Kerbal Konstructs, itself a `PQSCity` of its own: the second culprit, placed by a mod. The ground
  there is not flat, so the step between the two spots is not the height of the deck; it does not need
  to be, as long as it does not move.

## The craft, over six loads

**On stock**
([the readings](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag/blob/main/docs/the-measurements-runway.md)).
*On rails* reads the same height on all six loadings, under every craft, within two thousandths of a
millimetre. *Moved*, once physics has the craft:

| loading | 1 | 2 | 3 | 4 | 5 | 6 |
|---|---|---|---|---|---|---|
| Kerbin, on the grass, without this mod | +69.549 mm | −18.707 mm | +68.185 mm | +51.623 mm | +43.103 mm | +38.207 mm |
| Kerbin, on the grass, with this mod | +29.385 mm | +29.387 mm | +29.403 mm | +29.359 mm | +29.408 mm | +29.358 mm |
| Kerbin, on the runway, without this mod | +37.876 mm | −47.685 mm | +26.195 mm | +82.590 mm | −7.586 mm | +12.720 mm |
| Kerbin, on the runway, with this mod | +6.228 mm | +6.241 mm | +6.263 mm | +6.308 mm | +6.385 mm | +6.397 mm |
| Mun, on the ground, without this mod | −64.377 mm | −46.294 mm | −70.422 mm | −65.027 mm | −56.567 mm | −61.993 mm |
| Mun, on the ground, with this mod | −68.167 mm | −68.195 mm | −68.140 mm | −68.131 mm | −68.129 mm | −68.111 mm |
| Mun, on the Kerbal Konstructs runway, without this mod | +9.061 mm | −15.782 mm | −19.165 mm | −24.447 mm | −20.167 mm | −19.594 mm |
| Mun, on the Kerbal Konstructs runway, with this mod | **−3.709 mm** | −25.029 mm | −25.028 mm | −25.038 mm | −25.027 mm | −25.042 mm |

**With this mod**, in that same install, on those same saves, with this mod as the only difference. The
sessions are logged in [`diag/runs/runway-diag1-fix.log`](../diag/runs/runway-diag1-fix.log) and
[`runway-mun-kk-diag1-fix.log`](../diag/runs/runway-mun-kk-diag1-fix.log). *On rails* still reads the
same height, within two thousandths of a millimetre. The odd lines are on the ground, the even lines on
the runway, after switching to it; the bottom line is the reading in progress, not a record.

On Kerbin, on the runway of the KSC:

![Six loadings on Kerbin with this mod, read by Diag 1: the craft on the grass, then the craft on the runway](../imgs/Diag1/on-runway/six-loads.png)

On the Mun, on a runway placed by Kerbal Konstructs:

![Six loadings on the Mun with this mod, read by Diag 1: the craft on the ground, then the craft on the runway placed by Kerbal Konstructs](../imgs/Diag1/on-runway/six-loads-mun-kk.png)

On Kerbin, the craft on the grass comes to rest across a spread of 88.3 mm without this mod, and
0.051 mm with it; the craft on the runway, 130.3 mm without it, and 0.170 mm with it; the step between
the two, 81.7 mm without it, 0.198 mm with it. On the Mun, the craft on the ground, 24.1 mm without this
mod, and 0.084 mm with it; the craft on the runway, 33.5 mm without it; with it, 0.015 mm from the second
loading to the sixth, the first loading apart.

## The ground, over six loads

**On stock**
([the readings](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/blob/master/docs/the-measurements-runway.md)).
The height KSP computes reads the same under each craft at every loading: 64,784.990 mm on the grass of
Kerbin and 64,785.047 mm under its runway, within four hundredths of a millimetre on the Mun. On a
runway, the ray meets the deck, above that height. *Ground under craft*:

| loading | 1 | 2 | 3 | 4 | 5 | 6 |
|---|---|---|---|---|---|---|
| Kerbin, on the grass, without this mod | 64,823.702 mm | 64,735.423 mm | 64,822.326 mm | 64,805.632 mm | 64,797.256 mm | 64,791.809 mm |
| Kerbin, on the grass, with this mod | 64,783.381 mm | 64,783.383 mm | 64,783.367 mm | 64,783.387 mm | 64,783.392 mm | 64,783.354 mm |
| Kerbin, on the runway, without this mod | 69,112.013 mm | 69,026.819 mm | 69,100.574 mm | 69,156.869 mm | 69,066.837 mm | 69,087.149 mm |
| Kerbin, on the runway, with this mod | 69,080.733 mm | 69,080.756 mm | 69,080.842 mm | 69,080.660 mm | 69,080.759 mm | 69,080.626 mm |
| Mun, on the ground, without this mod | 4,123,565.650 mm | 4,123,583.842 mm | 4,123,559.661 mm | 4,123,565.046 mm | 4,123,573.493 mm | 4,123,568.004 mm |
| Mun, on the ground, with this mod | 4,123,561.868 mm | 4,123,561.832 mm | 4,123,561.862 mm | 4,123,561.842 mm | 4,123,561.859 mm | 4,123,561.857 mm |
| Mun, on the Kerbal Konstructs runway, without this mod | 4,122,775.969 mm | 4,122,751.151 mm | 4,122,747.749 mm | 4,122,742.457 mm | 4,122,746.736 mm | 4,122,747.327 mm |
| Mun, on the Kerbal Konstructs runway, with this mod | **4,122,763.202 mm** | 4,122,741.872 mm | 4,122,741.879 mm | 4,122,741.877 mm | 4,122,741.870 mm | 4,122,741.877 mm |

**With this mod**, in that same install. The sessions are logged in
[`diag/runs/runway-diag2-fix.log`](../diag/runs/runway-diag2-fix.log) and
[`runway-mun-kk-diag2-fix.log`](../diag/runs/runway-mun-kk-diag2-fix.log). The height KSP computes reads
the same digits as on stock.

On Kerbin, on the runway of the KSC:

![Six loadings on Kerbin with this mod, read by Diag 2: the ground under the craft on the grass, then under the craft on the runway](../imgs/Diag2/on-runway/six-loads.png)

On the Mun, on a runway placed by Kerbal Konstructs:

![Six loadings on the Mun with this mod, read by Diag 2: the ground under the craft on the ground, then under the craft on the runway placed by Kerbal Konstructs](../imgs/Diag2/on-runway/six-loads-mun-kk.png)

On Kerbin, the grass spreads over 88.3 mm without this mod, and 0.038 mm with it; the deck of the runway,
over 130.1 mm without it, and 0.216 mm with it; the step between the two, over 81.7 mm without it, and
0.203 mm with it. On the Mun, the ground, over 24.2 mm without this mod, and 0.036 mm with it; the deck,
over 33.5 mm without it; with it, over 0.009 mm from the second loading to the sixth — and the first
loading stands 21.3 mm above them, as with Diag 1.

## What the measurements say

**A runway moves on its own, and this mod stops it.** On stock, the step between the deck and the ground
beside it changes by up to 81.7 mm on Kerbin and 43.0 mm on the Mun from one loading to the next: the
runway does not follow the ground, it draws a rounding of its own. That is the second culprit, a static
placed through a float at planet scale — by the game on Kerbin, by Kerbal Konstructs on the Mun. With
this mod, the deck of the KSC comes back within 0.216 mm, and the step within 0.203 mm.

**Both instruments agree.** The craft on the runway of the KSC comes to rest within 0.170 mm, the deck
under it comes back within 0.216 mm: the craft rests on the deck, and the deck no longer moves.

**The runway of the KSC keeps a larger remainder than the grass.** About two tenths of a millimetre on
the runway, four to five hundredths on the grass, with both instruments. Where that remainder comes from
is not established yet. It is still several hundred times smaller than what stock does.

**On the Mun, the first loading reads another surface.** Listing every collider under each craft at each
loading ([`diag/runs/runway-mun-kk-colliders-fix.log`](../diag/runs/runway-mun-kk-colliders-fix.log),
four loadings with this mod): at the first loading of the session, one more collider of the runway is
active under the craft, a section of the deck, `Section3_Mesh`, 21.3 mm above the deck's own
`runway_collider`; from the second loading on, it is gone, and the craft rests on `runway_collider`,
which comes back within eight thousandths of a millimetre at every loading, the first one included. The
group of Kerbal Konstructs the runway hangs from comes back at the same height every time too, within a
few micrometres. This is not a rounding, and this mod does not touch it: on stock, the first loading
stands apart as well, 24.8 mm above the highest of the five others. Why that section is only there at
the first loading is not established; on the runway of the KSC on Kerbin, nothing of the kind shows.
