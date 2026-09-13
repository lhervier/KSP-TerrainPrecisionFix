# Performance measurement

What the fix costs, measured, with the logs it was read from.

## The instrument

The measuring code used to live in this mod, as `Src/Bench.cs` and two extra settings. It is now a mod
of its own, [PQS Bench](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench), which measures
what building the stock terrain costs and knows how to time this fix against stock on the same data.

**The procedure is there, not here**: which craft, which orbit, which runs, and how to read the dump.
What follows is only what it produced.

## Results

Measured on 2026-09-13, KSP 1.12.5 with Harmony, ModuleManager and KSPCommunityFixes, on a craft in a
5 km circular orbit of the Mun. Four runs of one save, in [runs/](runs/):

| | |
|---|---|
| `mun-05km-calibrate.log` | the three placements timed against each other. **The figures below come from this one** |
| `mun-05km-calibrate-two-formulas.log` | the same, half an hour earlier, before the hoisted formula was added: the second reading of stock and of the fix |
| `mun-05km-counters-fix.log` | what the terrain costs in flight, mod installed |
| `mun-05km-counters-stock.log` | the same, mod taken out of `GameData` |

`runs/` holds only what a published figure is read from. One log of the earlier campaign is kept,
`mun-05km-calibrate-before-quad-caching.log`, because the 500.4 ns below is still quoted and cannot be
taken again; the rest of that campaign is in the git history.

### What a vertex costs

Runs 1 and 4, 30 quads each, 54 000 vertices per formula. Figures from run 4.

| | per vertex | |
|---|---|---|
| stock | 260.0 ns | as `PQS.BuildVertexSurfaceRelative` does it |
| stock, `Transform`s hoisted | 174.8 ns | the same arithmetic, the two `Transform`s read once per quad |
| this fix | **87.8 ns** | |

**The fix places a vertex in a third of the time stock takes**, and the saving splits almost exactly in
two:

| | | |
|---|---|---|
| reading the two `Transform`s | **−85.2 ns** | 42.6 ns each, paid on every vertex |
| the arithmetic itself | **−87.0 ns** | double-precision subtraction and rotations against stock's two native calls |
| total | **−172.2 ns** | |

`BuildVertexSurfaceRelative` is called once per vertex and reads `base.transform` and
`buildQuad.transform` every time, then calls `Transform.TransformPoint` and
`Transform.InverseTransformPoint` — five trips into the native engine per vertex. The fix makes none:
the frame a quad hangs in is worked out once for its 225 vertices, and a vertex is left with a
`Vector3d` subtraction, a rotation in double, a rotation in float and two writes.

The hoisted formula is not a placement the game contains; it exists to put a number on each half. How it
is built so that the comparison is fair is on the PQS Bench page, not here.

Two `calibrate` runs of the same flight, taken half an hour apart, read 262.0 and 260.0 ns for stock,
88.8 and 87.8 for the fix: 0.8 % and 1.1 % apart, which is what this method's reproducibility is worth.

### In flight

Same save, same 150 seconds, `counters` mode. The reference run is a KSP with the mod's folder taken out
of `GameData`, so it pays for no patch of this mod at all.

| | fix | stock |
|---|---|---|
| samples / duration | 147 / 150.0 s | 147 / 150.0 s |
| frames per second | 114.28 | 114.10 |
| quads built per second | 16.61 | 16.51 |
| of which patched | 6.40 (39 %) | 6.40 (39 %) |
| ms per quad | 2.711 | 2.759 |
| **ms per patched quad** | **2.762** | **2.834** |
| terrain per frame | 0.953 ms | 0.975 ms |
| terrain share of real time | 10.89 % | 11.12 % |

The two runs built 2 492 and 2 476 quads, 0.6 % apart, and **exactly 960 patched ones each**: two loads
of one save really do cover the same ground.

The fix is ahead on every line, and **that must not be read as a 2.5 % gain**. The calibration says the
saving is 172.2 ns × 225 vertices = 38.7 µs per quad, which is 1.4 % of 2.834 ms — half of the 2.5 %
between these two columns. What the pair establishes is a bound: at the scale of a frame, one session
against another, nothing degrades, and what is left is the noise between two runs of KSP. The figure
worth publishing is the calibration; this pair is what shows it changes nothing a player can feel.

Nor is the gain worth having for its own sake. At 5 km over the Mun the game builds 6.4 patched quads
per second, so 1 440 vertices: 172.2 ns each amounts to 0.25 ms saved per second of flight, 0.025 % of
real time. The point is not the gain — it is that the correction is free.

### Where the fix runs at all

The fix only acts on the highest subdivision level, and the game only builds that level close to the
ground — below 9 375 m on Kerbin, 6 250 m on the Mun. PQS Bench writes a line per terrain sphere with
those limits in every run; what they are and how to read them is on its page, and the table it produced
is there too.

The one that matters here: **`maxLevelOffset` is 0** on both bodies, so only the highest level carries a
collider — the very level the fix acts on. **It covers every quad a craft can stand on.**

### Also worth knowing

Building a quad costs 2.8 ms whether or not the fix touches it — about 12 µs per vertex, nearly all of
it spent in the `PQSMod`s that compute height and colour. Both placements, stock's and this one, are a
fraction of a percent of that.

## The campaign of the integrated bench, and why its figures are gone

An earlier campaign, taken the same day with the measuring code still inside this mod, read **167.3 ns
for stock against 82.7 ns for the fix**. Those numbers are not in the tables above, and should not be
quoted: the stock side was not stock.

- **It hoisted two reads of `Component.transform` out of the loop**, keeping `sphere.transform` and
  `quad.transform` in locals for the 225 vertices of a quad. The real `PQS.BuildVertexSurfaceRelative`
  is called once per vertex and reads `base.transform` and `buildQuad.transform` every time. The replay
  now does the same, and the stock side went from 167.3 ns to 262.0 ns.
- **It omitted a write** the real method does, `verts[vertexIndex] = vertRel`.
- **Its reference run was not stock either.** `patchEnabled = false` left this mod's Harmony prefix on
  `BuildVertexSurfaceRelative` in place, so the run it called "stock" still paid for an indirection on
  every vertex. It is now a KSP without the mod.

All three flattered the fix, so the published saving was an underestimate: 84.6 ns per vertex where the
corrected method reads 172.2.

**The hoisted formula settles it, and closes the loop.** Timed as a third formula, it reads 174.8 ns —
which is what the old method read for "stock" in three separate sessions: 167.3, 168.6 and 172.2 ns. The
two agree to within a few nanoseconds, the difference being the write that was missing and the delegate
that is new. The old campaign was measuring a hoisted stock and calling it stock.

The fix moved less: 82.7 ns to 87.8 ns. The calibration now reaches it through a delegate, since it
lives in another assembly, **and puts the stock formulas behind delegates of the same type** so that the
comparison is between formulas rather than between ways of reaching one. All three figures carry that
indirection.

The column `patchedQuads` in the `-pqsbench` logs is called `topLevelQuads`: it counts the same thing,
quads of the highest subdivision level, which is a property of the terrain rather than of this mod. The
four older logs keep the old name.
