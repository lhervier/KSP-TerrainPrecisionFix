# A body from a planet pack

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: TBD — nothing has been measured on one.** On a stock body Kopernicus rebuilds a terrain that
already exists; for a planet pack it does more. Each added body is built by cloning the terrain sphere of
a stock template, which is then reconfigured — another radius, another `maxLevel`, PQSMods added and
removed — and a pack can also rewrite the `PQS` of a stock body. The frame this fix computes in
(`body.rotation`, `body.position`) has to still be the one those quads hang from. If it is not, the 1 m
safeguard leaves that terrain as stock builds it: no fix, silently, apart from one warning per body in
the log.

*To test:* the campaign of
[Terrain Precision Fix Diag 1](../checking-the-culprit-loading.md#the-craft-over-six-loads),
run on such a body. A
single landing already answers half of it: with this mod installed, the log carries either
`<body>: terrain placed in double precision` or the safeguard's warning.

**Which body to run it on.** [Outer Planets Mod](https://github.com/Poodmund/Outer-Planets-Mod) covers
both cases, and its Kopernicus configs say what to expect before the game is even started:

| body | built from the template | radius | `maxLevel` |
|---|---|---|---|
| Slate | Moho | 540 000 m | 8 |
| Wal | Moho | 370 000 m | 8 |
| Thatmo | Moho | 286 000 m | 8 |
| Tekto | Laythe | 280 000 m | Laythe's |
| Ovok | Minmus | 26 000 m | 1 |
| Eeloo | the stock body, moved into orbit of Sarnus | 210 000 m (stock) | 8 |

For comparison, read in flight by
[PQS Bench](https://github.com/lhervier/KSP-PQSBench/blob/master/README.md#how-the-terrain-of-that-body-is-set-up):
Kerbin subdivides to level 10 and the Mun to 9 — so the other `maxLevel` of this chapter is really
exercised there.

**Slate is the body to measure on.** Its radius, 540 km, falls between the same two powers of two as
Kerbin's 600 km, so a float's step is the same 62.5 mm there: the defect has the amplitude of the
campaigns already run, and the readings compare directly. It has no atmosphere, so the landing of the
protocol is a landing and nothing more; the flat ground Diag 1 asks for is then a matter of picking the
spot. **Eeloo is the other case**, a stock body the pack reconfigures rather than creates. **Ovok is the
edge case**: at `maxLevel` 1 the quads of its highest level are enormous, and whether this fix acts on
them at all — it only moves the quads the game parents to `LocalSpacePQStorage` — is one line of log to
read.

What such a campaign would not settle, the colliders' `maxLevelOffset`, is in
[its own chapter](colliders-below-the-highest-subdivision-level.md).
