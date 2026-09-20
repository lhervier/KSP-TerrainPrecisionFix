# Performance

Part of [Terrain Precision Fix](../README.md): what the fix costs, measured in flight against stock.

Now that the cause is known and fixed, what does the fix cost? The vertex part replaces a computation
that stock runs for every vertex of every terrain quad it builds, so it sits on a path the game uses
continuously while flying, not only when a scene loads. **It places a vertex in less than a third of
the time stock takes.**

## The instrument

Everything below was taken with
[PQS Bench](https://github.com/lhervier/KSP-PQSBench), a measuring mod that counts
what building the stock terrain costs and times whatever is patching the vertex placement against stock
on the same data, in the frame the game built a quad in. **Its page carries the procedure**, and the
rules that make a run worth keeping.

## Two changes, three configurations

This fix changes two things at once: the arithmetic, done in double, and the work done once per quad
instead of once per vertex. Compared with stock alone, the two savings cannot be told apart — and the
second is not specific to the fix: stock could read its two `Transform`s once per quad too.

So the fix is measured against a third configuration as well,
[Stock Quad Cache](https://github.com/lhervier/KSP-TerrainPrecisionFix-StockQuadCache), a mod written
for this measurement alone: stock's arithmetic bit for bit, with the only difference that the two
`Transform`s are read once per quad instead of once per vertex. It carries the second change without
the first, and splits the saving between them.

Each configuration keeps its own logs and its own reading of them, in its own repository: the stock
reference with [the bench](https://github.com/lhervier/KSP-PQSBench/blob/master/perfs/README.md),
the middle term with [Stock Quad Cache](https://github.com/lhervier/KSP-TerrainPrecisionFix-StockQuadCache/blob/master/perfs/README.md),
and this mod's two runs in [`perfs/`](../perfs/).

## The campaign

KSP 1.12.5 with Harmony, ModuleManager and KSP Community Fixes 1.41.1: a command pod on rails in a
circular orbit 5 km over the Mun, low enough that the game builds the highest subdivision level — the
only one the fix acts on — and six runs of two and a half minutes of that one save, which covers the
same ground every time. **All six were taken on my laptop**, described with
[the stock run](https://github.com/lhervier/KSP-PQSBench/blob/master/perfs/README.md);
figures from another machine are not comparable to these.

Three configurations, each measured twice, once for what a vertex costs and once for what a frame pays.
The reference is a KSP with this mod's folder taken out of `GameData`, since a mod left in place still
pays for its own patches on the path being timed.

Every `counters` run built exactly 1 272 quads of the highest subdivision level over 148 samples (3 211
to 3 213 quads in all) — the craft is on rails, so the same save covers the same ground.

## What a vertex costs

40 quads per run, 72 000 vertices per formula. Each run times what is installed against its own
measurement of stock, in the same frames, and the figure read is the difference between the two. Per
vertex:

| installed | stock, in that run (`stockNsPerVertex`) | installed (`installedNsPerVertex`) | difference (`differenceNsPerVertex`) |
|---|---|---|---|
| nothing: stock's arithmetic, `Transform`s read on every vertex | 229.6 ns | 228.5 ns | −1.1 ns, the floor of the method |
| stock's arithmetic, `Transform`s read once per quad | 232.3 ns | 149.5 ns | −82.8 ns |
| this fix | 228.5 ns | 64.2 ns | **−164.3 ns**, **3.56× faster** |

Stock makes five trips into the native engine per vertex: `Transform.TransformPoint`,
`Transform.InverseTransformPoint`, and two reads of `Component.transform` — `BuildVertexSurfaceRelative`
runs once per vertex, and reads `base.transform` and `buildQuad.transform` each time. The replacement
makes none.

The middle row is not a placement the game contains. It says where the 164.3 ns go: **82.8 ns** are the
two `Transform` reads, 41.4 ns each, and **81.5 ns** are the arithmetic, the double-precision version
being that much cheaper than two native calls. Read the other way: even if stock stopped asking Unity
for a `Transform` on every vertex, the fix would still be more than twice as fast.

The same row is what justifies [working out once per quad](the-fix-this-mod-proposes.md#once-per-quad-not-once-per-vertex) what
depends on the quad: two reads of `Component.transform` per vertex cost 82.8 ns, more than this fix
spends on a vertex altogether, and the fix needs rather more than two things per quad.

The difference is read within each run, never between two: from one session of KSP to the next the
whole replay runs a little faster or slower — the stock column above reads 229.6, 232.3 and 228.5 ns,
a 1.7 % spread — and only a difference taken inside one session cancels that out.
In the run with neither mod installed the two readings are of the same code reached two different ways,
and they differ by 1.1 ns: the floor of the method, which any other row carries too.

Placing a vertex is a small part of building one: a quad of the highest level takes 1.7 ms to build,
about 7.6 µs per vertex, nearly all of it spent in the `PQSMod`s that compute height and colour. The
230 ns stock spends placing it are 3.0 % of that.

## In flight

The same save and the same stretch of orbit, three times, 150 seconds each, counted second by second.

| | stock | `Transform`s read once per quad | this fix |
|---|---|---|---|
| frames per second | 77.04 | 75.82 | 79.16 |
| ms per quad the fix acts on | 1.716 | 1.682 | **1.650** |
| terrain per frame | 1.195 ms | 1.170 ms | 1.182 ms |
| terrain share of real time | 9.21 % | 8.87 % | 9.36 % |

Per quad, the fix is ahead by 3.8 %, where the calibration predicts 2.2 % — 164.3 ns × 225 vertices is
37.0 µs per quad. **That should not be read as a measurement.** The middle column is the reason: by the
same reckoning it should be 1.1 % ahead, and it is 2.0 % ahead; per frame it is ahead of the fix; and
the fix, faster per quad, takes a larger share of real time than stock. The noise between two sessions
of KSP is worth about as much as the effect being looked for at this scale. What these three runs
establish is a bound — nothing degrades at the scale of a frame — and the figure worth publishing is the
calibration.

## What this says

The fix is faster than stock, and faster than stock with its `Transform`s read once per quad: half of
its saving is the organisation any version could adopt, the other half is the arithmetic itself.

Nor is the saving worth having for its own sake. At 5 km over the Mun the game builds 8.5 of these quads
per second, so 1 917 vertices: 164.3 ns each is 0.31 ms per second of flight, 0.031 % of real time. The
point is not the gain. It is that the correction is free.

