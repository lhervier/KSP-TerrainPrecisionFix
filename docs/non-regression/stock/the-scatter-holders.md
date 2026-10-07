# The scatter holders

Part of [Terrain Precision Fix](../../../README.md), one test of [Non-regression tests](../../non-regression.md), on [stock](../../non-regression.md#stock).

**Status: checked with the scatter fix on, no problem — no holder lost, and every pool works as in
stock, over a flight and across scene switches.** The scatter fix, off by default, hangs each holder of
terrain scatter from its own quad, and hangs it back in its pool's container when the quad is destroyed
([The fix: the scatter](../../the-fix-scatter.md)). A holder missed on the way would be left on a quad that
stock sends back to its cache of quads, to be reused elsewhere, or on another body: nothing would show on
screen.

**The series with the scatter fix were taken when it was a mod of its own**, Rock Precision Fix 0.1.0,
installed next to Terrain Precision Fix 0.1.0: the same two patches as this mod's scatter fix, whose lines
in these logs are tagged `[RockPrecisionFix]` instead of `[TerrainPrecisionFix]`.

## What stock does with the holders

Read in the stock code: stock relies neither on the hierarchy nor on where a holder hangs.

- **The pool** is kept in two lists of the scatter, `cacheAssigned` and `cacheUnassigned`, not in the
  hierarchy; a holder in use is released by the `onDestroy` delegates of its quad, before the quad goes to
  the PQS cache.
- **A holder is shown and hidden** with `obj.SetActive`, a quad with `meshRenderer.enabled`, so the
  holder does not inherit anything from the quad's object.
- **When a collision or a raycast hits the ground**, stock asks the object hit whether it is a quad
  (`GetComponent<PQ>()` in `Part`, `ModuleWheelDamage`, `ModuleDeployableSolarPanel`,
  `ModuleGroundSciencePart`), not its parents: a holder or an object of scatter hanging from a quad is not
  taken for the ground.

## The holder pools, over a flight

[KSP Diag - Scatter](https://github.com/lhervier/KSP-Diag-Scatter) keeps a second reading,
[the holder pools](https://github.com/lhervier/KSP-Diag-Scatter/blob/main/docs/checking-the-holder-pools.md).
It reads every pool of holders through stock's own bookkeeping, and counts the holders that break a rule
stock keeps: a holder in use that no longer exists or stands on a quad that is not active, a free holder
that does not hang from its pool's container or still has a quad, a count that disagrees with its list, a
holder in no pool.

It is taken during a flight, where the terrain keeps building quads ahead of the craft and destroying
those behind it, in KSP 1.12.5 with Harmony, ModuleManager and KSP Community Fixes 1.41.1: the save
[`ref-mune-5km.sfs`](https://github.com/lhervier/KSP-Diag-Scatter/blob/main/diag/README.md#the-saves),
a Mk1 command pod in a circular equatorial orbit 5 km over the Mun, loaded once, one record 30 s into the
flight, then about every two minutes, and one after the pod crashed into the relief. Once with the
terrain fix alone, once with both fixes. The records with both fixes are in
[`diag/runs`](../../../diag/README.md#the-scatter-fix-the-holder-pools-over-a-flight), and those with the
terrain fix alone with
[the instrument](https://github.com/lhervier/KSP-Diag-Scatter/blob/main/diag/README.md#the-holder-pools-over-a-flight).

| record | holders in use | free | broken rules, the terrain fix alone | broken rules, both fixes |
|---|---|---|---|---|
| 1 | 344 | 40 | 0 | 0 |
| 2 | 168 | 216 | 0 | 0 |
| 3 | 144 | 240 | 0 | 0 |
| 4 | 224 | 160 | 0 | 0 |
| 5 | 152 | 232 | 0 | 0 |
| 6, after the crash | 568 | 40 | 0 | 0 |

**No holder is lost.** With both fixes, every holder handed back hangs in its pool's container, without a
quad, and every holder in use stands on a live quad of the Mun, at every record, the one after the crash
included.

**The pool works as in stock.** The counts of holders in use and free are the same, record for record, in
both flights: the flight takes as many holders out of the pool, at the same moments, with the scatter fix
as without it. The pool of the Mun's `Rock00` grows from 384 holders to 608 after the crash, as stock makes
new ones when it has none free left, and those go through the fix as well.

## The holder pools, across scene switches

The quads of the most detailed level are shared by every body, and leaving a body switches its terrain
off. This series checks that its holders go back to their pools before stock destroys them, and that
none travels on a quad to the next body.

The same install, with both fixes. One session, following
[the instrument's protocol](https://github.com/lhervier/KSP-Diag-Scatter/blob/main/docs/checking-the-holder-pools.md#across-scene-switches):
`reference-mune.sfs` loaded from the Space Center, back to the Space Center, then `reference-kerbin.sfs`,
with a record in each of the three scenes. The records are in
[`diag/runs`](../../../diag/README.md#the-scatter-fix-the-holder-pools-across-scene-switches). Every record
ends on `0 in no pool` and `0 broken rules`:

| record | scene | pools | holders in use | free |
|---|---|---|---|---|
| 1 | the Mun | the Mun's `Rock00` | 128 | 32 |
| 2 | the Space Center | Kerbin's `Tree00`, `Grass00`, `boulder`, `Pine00`, `cactus` | 4 | 316 |
| 3 | Kerbin | the same five | 118 | 202 |

Those are, pool for pool, the counts of the same session with the terrain fix alone. The Mun's pool is
gone from the second record on, and no holder of the Mun turned up under a quad of Kerbin.
