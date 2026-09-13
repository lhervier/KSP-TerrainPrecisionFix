# Performance measurement

What the fix costs, measured, with the logs it was read from.

## The instrument

The measuring code used to live in this mod, as `Src/Bench.cs` and two extra settings. It is now a mod
of its own, [PQS Bench](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench), which measures
what building the stock terrain costs and knows how to time this fix against stock on the same data.

**The procedure is there, not here**: which craft, which orbit, which runs, and how to read the dump.
What follows is only what it produced.

Two things changed with the move, and they are worth knowing when comparing a new run with the logs
below:

- **The reference run is now a KSP without this mod installed**, rather than the same DLL with its
  correction switched off. The old way left this mod's Harmony prefix on `BuildVertexSurfaceRelative`
  in place, so the run it called "stock" still paid for an indirection on every vertex — a bias against
  stock, in the direction of the difference reported below.
- **The stock formula being replayed gained a write** it had been missing (`verts[vertexIndex] =
  vertRel`, which `PQS.BuildVertexSurfaceRelative` does and the replay did not). Another bias in
  stock's favour, of one array write per vertex.

Both were small and both flattered the fix. The figures below were taken before either was corrected,
so they are a lower bound on what the fix saves, not an upper one.

The column `patchedQuads` in the logs below is called `topLevelQuads` in the current instrument: it
counts the same thing, quads of the highest subdivision level, which is a property of the terrain rather
than of this mod.

## Results

Measured on 2026-09-13, KSP 1.12.5 with Harmony, ModuleManager and KSPCommunityFixes, on a craft in a
5 km circular orbit of the Mun. The three flights are in [runs/](runs/). Only the "before caching" line
comes from an earlier build of the mod, and cannot be taken again without putting that code back.

### What a vertex costs

| | stock | this fix | difference |
|---|---|---|---|
| before caching, 41 quads | 168.6 ns | 500.4 ns | **+331.8 ns** |
| after caching, 30 quads | 167.3 ns | 82.7 ns | **−84.6 ns** |

The first run is why `BuildQuadContext` exists. `PlaceVertex` used to redo, for each of the 225 vertices
of a quad, everything that only depends on the quad: `AppliesTo` (which reads `quad.transform.parent`
and `sphere.LocalSpacePQStorage`), `BodyOf`, the one-metre safeguard, and `quad.transform.position` /
`.rotation` with its inverse. That is several trips into the Unity engine per vertex, against the two
stock makes — hence three times the cost. Worked out once per quad, a vertex is left with a `Vector3d`
subtraction, a rotation in double, a rotation in float and two writes.

**So the fix now places a vertex in half the time stock takes**, which was the expectation recorded in
the TODO and had never been checked: stock calls `Transform.TransformPoint` and
`InverseTransformPoint`, two native calls, where the replacement is managed arithmetic with none.

Stock was measured three times — 167.3, 168.6 and 172.2 ns — in three separate KSP sessions, two of
them on an orbit set up by hand and one on an orbit written into the save by a script. They sit within
1.5 % of each other, which is what the method's own reproducibility is worth.

Neither the cost nor the saving is visible while playing. At 5 km over the Mun the game builds 6.4
patched quads per second, so 1 440 vertices: 84.6 ns each amounts to 0.12 ms saved per second of flight,
0.01 % of real time. The point is not the gain — it is that the correction is free.

### The same flight, in flight

Same save, same two minutes, same build of the mod; the correction on, then off.

| | fix | stock |
|---|---|---|
| samples / duration | 146 / 149.0 s | 147 / 150.1 s |
| frames per second | 114.59 | 114.21 |
| quads built per second | 16.38 | 16.55 |
| of which patched | 6.39 (39 %) | 6.40 (39 %) |
| ms per quad | 2.689 | 2.703 |
| **ms per patched quad** | **2.743** | **2.786** |
| terrain per frame | 0.938 ms | 0.958 ms |
| terrain share of real time | 10.75 % | 10.94 % |

The two runs built 2 442 and 2 484 quads, 1.7 % apart, and 952 against 960 patched ones, 0.8 % apart:
two loads of one save really do cover the same ground.

The fix is ahead on every line, and **that must not be read as a 1.5 % gain**. The calibration says the
saving is 84.6 ns × 225 vertices = 19.0 µs per quad, which is 0.7 % of 2.786 ms — half of the 1.5 %
between these two columns. What the pair establishes is a bound: at the scale of a frame, one session
against another, nothing degrades, and what is left is the noise between two runs of KSP. The figure
worth publishing is the calibration; this pair is what shows it changes nothing a player can feel.

### Where the fix runs at all

The fix only acts on the highest subdivision level, and the game only builds that level close to the
ground. Both limits are logged by PQS Bench, once per sphere:

| sphere | minLevel | maxLevel | highest level appears under | lowest level with a collider | max angle per sample |
|---|---|---|---|---|---|
| Kerbin | 2 | 10 | 9 375 m | 10 | 4.60e-5 rad |
| Mun | 2 | 9 | 6 250 m | 9 | 9.20e-5 rad |
| KerbinOcean | 2 | 7 | 75 000 m | none | 3.68e-4 rad |

**`maxLevelOffset` is 0** on Kerbin and on the Mun, so only the highest level carries a collider — the
very level the fix acts on. It covers every quad a craft can stand on.

### Also worth knowing

Building a quad costs 2.7 ms whether or not the fix touches it — about 12 µs per vertex, nearly all of
it spent in the `PQSMod`s that compute height and colour. Both placements, stock's and this one, are a
fraction of a percent of that.
