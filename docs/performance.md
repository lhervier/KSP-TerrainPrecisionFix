# Performance

Part of [Terrain Precision Fix](../README.md): what the fix costs, measured against stock.

Now that the cause is known and fixed, what does the fix cost? The vertex part replaces a computation
that stock runs for every vertex of every terrain quad it builds, so it sits on a path the game uses
continuously while flying, not only when a scene loads. **It places a vertex in under 40 % of the time
stock takes, and a frame cannot tell the difference.**

## Two instruments

- [PQS Bench](https://github.com/lhervier/KSP-PQSBench), a measuring mod that times whatever is patching
  the vertex placement against stock on the same data, in the frame the game built a quad in. **Its page
  carries the procedure**, and the rules that make a run worth keeping.
- [KSPProfiler](https://github.com/KSPModdingLibs/KSPProfiler), which times each phase of Unity's game
  loop in every frame. It is not ours, so how it was used is written [below](#how-the-frames-were-timed).

The two never run together: PQS Bench does real work inside the frames the profiler times.

## Two changes, three configurations

This fix changes two things at once: the arithmetic, done in double, and the work done once per quad
instead of once per vertex. Compared with stock alone, the two savings cannot be told apart — and the
second is not specific to the fix: stock could read its two `Transform`s once per quad too.

So the fix is measured against a third configuration as well,
[Stock Quad Cache](https://github.com/lhervier/KSP-TerrainPrecisionFix-StockQuadCache), a mod written
for this measurement alone: stock's arithmetic bit for bit, with the only difference that the two
`Transform`s are read once per quad instead of once per vertex. It carries the second change without
the first, and splits the saving between them.

Each configuration keeps its own PQS Bench logs and its own reading of them, in its own repository: the
stock reference with [the bench](https://github.com/lhervier/KSP-PQSBench/blob/master/perfs/README.md),
the middle term with [Stock Quad Cache](https://github.com/lhervier/KSP-TerrainPrecisionFix-StockQuadCache/blob/master/perfs/README.md),
and this mod's runs in [`perfs/`](../perfs/README.md), along with the profiler runs of all three.

## The campaign

KSP 1.12.5 with Harmony, ModuleManager and KSP Community Fixes 1.41.1: a command pod on rails in a
circular orbit 5 km over the Mun, low enough that the game builds the highest subdivision level — the
only one the fix acts on. Every run flies the same 70 seconds of that one save, from 30 s of mission time,
which covers the same ground every time: each PQS Bench run built the same 704 quads of the highest level.
**All runs were taken on my desktop**, described with
[the stock runs](https://github.com/lhervier/KSP-PQSBench/blob/master/perfs/README.md); figures from
another machine are not comparable to these.

Three configurations, each measured twice with each instrument, the configurations taken in turn rather
than one after the other. The reference is a KSP with this mod's folder taken out of `GameData`, since a
mod left in place still pays for its own patches on the path being timed.

## What a vertex costs

22 quads per run, 39 600 vertices per formula. Each run times what is installed against its own
measurement of stock, in the same frames, and the figure read is the difference between the two. Per
vertex, runs 1 and 2:

| installed | stock, in that run (`stockNsPerVertex`) | installed (`installedNsPerVertex`) | difference (`differenceNsPerVertex`) |
|---|---|---|---|
| nothing: stock's arithmetic, `Transform`s read on every vertex | 284.2 / 284.7 ns | 281.7 / 288.2 ns | −2.5 / +3.4 ns, the floor of the method |
| stock's arithmetic, `Transform`s read once per quad | 282.7 / 290.6 ns | 200.9 / 203.3 ns | −81.7 / −87.4 ns |
| this fix | 297.5 / 286.1 ns | 110.5 / 107.2 ns | **−187.0 / −178.9 ns**, **2.7× faster** |

Stock makes five trips into the native engine per vertex: `Transform.TransformPoint`,
`Transform.InverseTransformPoint`, and two reads of `Component.transform` — `BuildVertexSurfaceRelative`
runs once per vertex, and reads `base.transform` and `buildQuad.transform` each time. The replacement
makes none.

The middle row is not a placement the game contains. It says where the fix's 183.0 ns go, on average:
**84.6 ns** are the two `Transform` reads, 42 ns each, and **98.4 ns** are the arithmetic, the
double-precision version being that much cheaper than two native calls. Read the other way: even if stock
stopped asking Unity for a `Transform` on every vertex, the fix would still save it about 98 ns per
vertex.

The same row is what justifies [working out once per quad](the-fix-this-mod-proposes.md#once-per-quad-not-once-per-vertex) what
depends on the quad: two reads of `Component.transform` per vertex cost 84.6 ns, three quarters of what
this fix spends on a vertex altogether, and the fix needs rather more than two things per quad.

The difference is read within each run, never between two: from one session of KSP to the next the
whole replay runs a little faster or slower — the stock column above reads from 282.7 to 297.5 ns, a
5 % spread — and only a difference taken inside one session cancels that out. In the runs with neither
mod installed the two readings are of the same code reached two different ways, and they differ by
2.5 ns one way, then 3.4 ns the other: the floor of the method, which any other row carries too.

## What a frame pays

The same flight, timed frame by frame with KSPProfiler 1.0.0. It reports, for each phase of the frame,
the mean, the median, the worst 25 % and the worst 1 % of frames.

### How the frames were timed

`GameData` holding Harmony, ModuleManager, KSP Community Fixes 1.41.1, KSPProfiler with the KsmUI library
it ships with, and **one** of: nothing more (stock), Stock Quad Cache, or this mod. **PQS Bench is not
installed**: its calibration replays vertices inside the very frames being timed.

Each run is a fresh KSP:

1. Load the save, and turn the camera so that part of the Mun is in view, the same way in every run.
2. In the profiler's window: captured frames at 10 000 (the most the window accepts; the profiler stops
   by itself when it is reached), *AutoCapture* off, *AutoUpdate* left on.
3. *Start Capture* at 30 s of mission time, *Stop Capture* at 1 min 40 s, at ×1 all along — the same
   70 seconds of the same orbit as the PQS Bench runs.
4. *Export to CSV*, and read the number of captured frames off the window: the CSV does not hold it.

A run is worth keeping when fewer than 10 000 frames were captured, so that the capture ended when it
was stopped and not before. Two runs per configuration, the configurations taken in turn.

### What they read

The CSV names two rows `Update` and several `Coroutines`. Below, **Update** is the whole phase,
**Update → Coroutines** the coroutines run inside it — among them `PQS.UpdateSphere`, where the terrain
is updated and its quads built, along with whatever other coroutines ran that frame — and **Cameras
render** the drawing. In milliseconds per frame (the CSVs and logs are [with the runs](../perfs/README.md#what-a-frame-pays)):

| | stock 1 | stock 2 | `Transform`s once per quad 1 | `Transform`s once per quad 2 | this fix 1 | this fix 2 |
|---|---|---|---|---|---|---|
| frames per second, mean | 101.1 | 94.1 | 95.9 | 95.4 | 96.6 | 99.6 |
| frame time, mean | 9.89 | 10.62 | 10.43 | 10.49 | 10.35 | 10.05 |
| frame time, worst 1 % | 35.06 | 35.73 | 35.76 | 35.45 | 35.59 | 34.91 |
| Update, mean | 2.71 | 2.85 | 2.82 | 2.91 | 2.81 | 2.74 |
| Update → Coroutines, mean | 1.81 | 1.94 | 1.92 | 1.94 | 1.91 | 1.87 |
| Update → Coroutines, median | 1.16 | 1.24 | 1.23 | 1.27 | 1.22 | 1.19 |
| Update → Coroutines, worst 1 % | 24.45 | 24.73 | 24.83 | 24.61 | 24.88 | 24.23 |
| Cameras render, mean | 3.44 | 3.68 | 3.60 | 3.52 | 3.55 | 3.45 |
| VSync, mean | 0.06 | 0.06 | 0.06 | 0.06 | 0.06 | 0.06 |
| profiler overhead, mean | 0.43 | 0.45 | 0.43 | 0.45 | 0.43 | 0.44 |

**VSync** near zero says the frame waited on the processor, not on the screen or the graphics card —
which is what a frame measurement of the terrain needs. Averaged over the two runs of each configuration:

| | stock | `Transform`s read once per quad | this fix |
|---|---|---|---|
| frame time, mean | 10.26 | 10.46 | 10.20 |
| Update → Coroutines, mean | 1.88 | 1.93 | 1.89 |
| Update → Coroutines, worst 1 % | 24.59 | 24.72 | 24.56 |

**None of the three can be told from the others.** The two stock runs alone are 0.73 ms apart on the
mean frame time, and 0.13 ms apart on the coroutines — more than any two configurations differ. The
ranking is not even stable: the fix is fastest on the mean frame time, but its first run has the worst
1 % of coroutines highest of all six.

That is what the vertex figures predict. At 704 quads of the highest level in 70 seconds, the fix builds
about 10 of them a second, 2 263 vertices: 183 ns each is **0.41 ms per second of flight, 0.04 % of real
time**, or 4 µs per frame at 97 frames a second. The noise between two sessions of KSP is thirty times
that.

## What this says

The fix is faster than stock, and faster than stock with its `Transform`s read once per quad: a little
less than half of its saving is the organisation any version could adopt, the rest is the arithmetic
itself.

Nor is the saving worth having for its own sake: no frame shows it. The point is not the gain. It is that
the correction costs nothing a profiler can see.
