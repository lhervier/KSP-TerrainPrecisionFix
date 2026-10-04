# Performance

Part of [Terrain Precision Fix](../README.md): what the fix costs, measured against stock — the ground measured, the statics not yet.

Now that the cause is known and fixed, what does the fix cost? The vertex part replaces a computation
that stock runs for every vertex of every terrain quad it builds, so it sits on a path the game uses
continuously while flying, not only when a scene loads. **It does not slow the game down: timed frame
by frame, it came out no dearer than stock.** It even places a vertex faster than stock, but that saving
is too small for a frame to show. The statics fix runs on another path, a check per
frame of the statics the game knows of; **its cost with statics near a craft is not measured yet**.

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

Three configurations, taken in turn rather than one after the other: each measured twice with PQS Bench,
and three times with the profiler, in a later session of runs. The reference is a KSP with this mod's
folder taken out of `GameData`, since a mod left in place still pays for its own patches on the path
being timed.

The PQS Bench runs measure the ground only: they were taken before this mod placed the statics. The
profiler runs were taken with the statics fix installed, but 5 km over the Mun no static is near enough
for it to take one out of its sphere: all it did there was its check of every static, once per frame,
and that check is in their figures.

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

The same row is what justifies [working out once per quad](the-fix-ground.md#once-per-quad-not-once-per-vertex) what
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

`GameData` holding Harmony, ModuleManager, KSP Community Fixes 1.41.1, KSP-MCPServer, KSPProfiler with
the KsmUI library it ships with, and **one** of: nothing more (stock), Stock Quad Cache 0.1.0, or this mod
0.1.0. **PQS Bench is not installed**: its calibration replays vertices inside the very frames being
timed.

The same desktop as the PQS Bench runs, with KSP this time **full screen** at 1280×720 (`FULLSCREEN = True`
in its `settings.cfg`), so that no other window can come in front of it; vertical sync off
(`SYNC_VBL = 0`), no frame limit, terrain detail High, terrain scatter on at full density.

The runs used [a fork of KSPProfiler](https://github.com/lhervier/KSP-ExtMod-KSPProfiler) which only adds
a remote control of its window's buttons — open, *Start capture*, *Stop capture*, *Export to CSV* —
through [KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer), a mod that answers HTTP requests on
127.0.0.1. Nothing in what the profiler measures was changed: the same procedure can be played by hand
with KSPProfiler 1.0.0. Here, the runs were flown by
[a script](https://github.com/lhervier/KSP-PQSBench/blob/master/perfs/automation/run-perfs.py), in
Python and nothing else. Whether driving the game through KSP-MCPServer changes what a frame costs was
checked on [its own page](https://github.com/lhervier/KSP-MCPServer/blob/master/docs/performance.md):
driving it by script costs at most about 0.05 ms per frame, on the coroutines the terrain is updated in
(Update → Coroutines, [below](#what-they-read)).

Each run is a fresh KSP:

1. Load the save. Right after the load, turn the camera to look ahead along the orbit, with the Mun's
   ground on the left two thirds of the screen: the framing of
   [PQS Bench's runs](https://github.com/lhervier/KSP-PQSBench/blob/master/docs/measuring-a-terrain-mod.md#the-runs),
   pictured there. In orbit, KSP's camera turns in the orbit's frame, so the Mun's edge stands upright:
   heading 204° in that frame, pitch 0.
2. Open the profiler's window, where it opens, KSP's interface shown. Captured frames at 10 000 (the
   most the window accepts; the profiler stops by itself when it is reached), *AutoCapture* off. Bring
   KSP's window in front of the others, and move the mouse pointer away from the craft, to the middle of
   the screen's left edge.
3. *Start capture* at 30 s of mission time, *Stop capture* at 1 min 40 s, at ×1 all along — the same
   70 seconds of the same orbit as the PQS Bench runs. Nothing is asked of the game in between: the
   script sleeps through the window, and KSP-MCPServer's on-screen messages are turned off.
4. *Export to CSV*. The CSV does not hold the number of captured frames: by hand, read it off the window.

A run is worth keeping when fewer than 10 000 frames were captured, so that the capture ended on *Stop*
and not before. Three runs per configuration, in one session of runs, the configurations taken in turn;
all nine ended on *Stop*, with 6 574 to 6 758 frames each. The script checked at the start and at the
stop of every capture that KSP's window was in front: it was, every time.

### What they read

The CSV names two rows `Update` and several `Coroutines`. Below, **Update** is the whole phase,
**Update → Coroutines** the coroutines run inside it — among them `PQS.UpdateSphere`, where the terrain
is updated and its quads built, along with whatever other coroutines ran that frame — and **Cameras
render** the drawing. In milliseconds per frame (the CSVs and logs are [with the runs](../perfs/README.md#what-a-frame-pays)):

| | stock 1 | stock 2 | stock 3 | `Transform`s once per quad 1 | `Transform`s once per quad 2 | `Transform`s once per quad 3 | this fix 1 | this fix 2 | this fix 3 |
|---|---|---|---|---|---|---|---|---|---|
| frames per second, mean | 94.3 | 93.8 | 95.7 | 94.7 | 96.0 | 96.2 | 95.9 | 94.8 | 96.2 |
| frame time, mean | 10.61 | 10.66 | 10.45 | 10.55 | 10.42 | 10.39 | 10.42 | 10.55 | 10.40 |
| frame time, worst 1 % | 35.58 | 35.77 | 35.35 | 35.71 | 35.36 | 35.26 | 35.16 | 35.33 | 35.54 |
| Update, mean | 2.92 | 2.91 | 2.89 | 2.89 | 2.88 | 2.88 | 2.89 | 2.88 | 2.89 |
| Update → Coroutines, mean | 1.96 | 2.00 | 1.96 | 1.95 | 1.94 | 1.94 | 1.93 | 1.94 | 1.93 |
| Update → Coroutines, median | 1.30 | 1.30 | 1.30 | 1.30 | 1.30 | 1.29 | 1.28 | 1.29 | 1.28 |
| Update → Coroutines, worst 1 % | 24.67 | 24.77 | 24.64 | 24.70 | 24.65 | 24.33 | 24.39 | 24.32 | 24.89 |
| Cameras render, mean | 3.56 | 3.63 | 3.54 | 3.55 | 3.50 | 3.49 | 3.53 | 3.59 | 3.51 |
| VSync, mean | 0.06 | 0.07 | 0.06 | 0.06 | 0.06 | 0.06 | 0.06 | 0.06 | 0.06 |
| profiler overhead, mean | 0.44 | 0.44 | 0.45 | 0.46 | 0.46 | 0.45 | 0.46 | 0.48 | 0.46 |

**VSync** near zero says the frame waited on the processor, not on the screen or the graphics card —
which is what a frame measurement of the terrain needs. Averaged over the three runs of each
configuration:

| | stock | `Transform`s read once per quad | this fix |
|---|---|---|---|
| frame time, mean | 10.57 | 10.45 | 10.46 |
| Update → Coroutines, mean | 1.97 | 1.94 | 1.93 |
| Update → Coroutines, worst 1 % | 24.69 | 24.56 | 24.53 |

**The fix is no dearer than stock.** On Update → Coroutines, stock spends 1.96 to 2.00 ms per frame, run
by run, 1.97 ms over its three runs, and this fix 1.93 to 1.94 ms, 1.93 ms over its three: each
run of the fix a little below each run of stock, by 0.02 ms between the closest two. The mean frame time
leans the same way, less cleanly: 10.57 ms for stock and 10.46 ms for the fix on average, but the fix's
second run, at 10.55 ms, is above stock's third, at 10.45 ms.

Stock's arithmetic with its `Transform`s read once per quad sits between the two: 1.94 to 1.95 ms,
1.94 ms on average, each of its runs below each run of stock too — by 0.01 ms between the closest two —
and none below the fix's. Cameras render, the drawing, which neither mod touches, also averages a little
less for the two than for stock: 3.51 and 3.54 ms, against 3.58.

The vertex figures predict none of this. At 704 quads of the highest level in 70 seconds, the game builds
about 10 of them a second, 2 263 vertices: the fix's 183 ns each is **0.41 ms per second of flight,
0.04 % of real time**, or 4 µs per frame at 95 frames a second, and the 84.6 ns saved by reading the
`Transform`s once per quad, 2 µs per frame. Both are below the 0.01 ms the CSV gives its figures to, and
below the spread of the three runs of one configuration on Update → Coroutines: 0.04 ms for stock,
0.01 ms for each of the other two. And the 0.04 ms by which the fix comes out below stock on average is
ten times its own saving, the 0.03 ms of the middle configuration fifteen times its own: whatever makes
those runs cheaper, it is not the vertex placement, and three runs per configuration do not say what it
is.

## What this says

**The fix does not slow the game down.** That is the only conclusion the frames support about it. In
this session, its runs and those of stock with its `Transform`s read once per quad came out a little
cheaper than stock's, every one of them — by 0.03 to 0.04 ms on average, ten to fifteen times more than
their vertex saving can pay for: three runs per configuration show it, and do not say why. It is not a
gain to claim for the fix.

The bench does show the fix placing a vertex faster than stock, and faster than stock with its
`Transform`s read once per quad: a little less than half of that saving is the organisation any version
could adopt, the rest is the arithmetic itself. But it is not a gain worth claiming: at 4 µs per frame,
below the 0.01 ms the profiler reports to and below the spread among the runs of one configuration, no
frame shows it.

The statics are hardly in these figures: in the profiler runs, no static was near the craft. What their
fix does grows with the number of statics, not of vertices: once per frame, it checks every static the
game knows of to see which ones a craft is near, and a static out of its sphere is placed again whenever
its body moves. That is still to measure, with many statics near a craft — a base of Kerbal
Konstructs — and is listed in [The KSC buildings, runway and launchpad](limits-and-solutions/the-ksc-buildings-runway-and-launchpad.md).
