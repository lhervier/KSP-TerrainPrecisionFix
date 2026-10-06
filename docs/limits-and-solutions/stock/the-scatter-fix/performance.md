# The scatter fix: performance

Part of [Terrain Precision Fix](../../../../README.md), one page of the solution to
[Rocks, grass and trees](../rocks-grass-and-trees.md), in [Limits and solutions](../../../limits-and-solutions.md):
the protocol and the figures.

What the scatter fix adds runs once each time a holder is given a quad or released, not once per vertex: a
re-parenting and three transform assignments. A holder is given its quad within `PQS.BuildQuad`, when the
`PQSMod`s are told the quad is built (`Mod_OnQuadBuilt` → `AddScatterMeshController` → `Setup`), and
handed back to its pool within `PQS.UpdateQuads`, when a quad collapses (`UpdateSubdivision` →
`Collapse` → `onDestroy`). In flight, both happen inside `PQS.UpdateQuads`, where the terrain subdivides
and collapses its quads, and which the terrain's coroutine `PQS.UpdateSphere` calls each time it runs.

So whatever the scatter fix costs is paid in the frames the terrain is updated in. Timed frame by frame, with
the terrain fix alone and with the scatter fix added: **no cost these runs can resolve.** The runs with the scatter fix differ more among themselves than the two configurations do.

## The instrument

[KSPProfiler](https://github.com/KSPModdingLibs/KSPProfiler) times each phase of Unity's game loop in
every frame, and reports for each the mean, the median, the worst 25 % and the worst 1 % of the frames
it captured.

The runs used [a fork of it](https://github.com/lhervier/KSP-ExtMod-KSPProfiler) which only adds a
remote control of its window's buttons — open, *Start capture*, *Stop capture*, *Export to CSV* —
through [KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer), a mod that answers HTTP requests on
127.0.0.1. Nothing in what the profiler measures was changed: the same procedure can be played by hand
with KSPProfiler 1.0.0. Here, the runs were flown by
[a script](https://github.com/lhervier/KSP-PQSBench/blob/main/perfs/automation/run-perfs.py), in
Python and nothing else.

[PQS Bench](https://github.com/lhervier/KSP-PQSBench), the bench the terrain fix is measured with ([Performance](../../../performance.md)),
does not answer this question: its `calibrate` mode only replays the vertex placement
(`PQS.BuildVertexSurfaceRelative`), which the scatter fix does not touch.

## The campaign

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1, KSP-MCPServer, the profiler and
Terrain Precision Fix 0.1.0 in every run, and, in half of them, the scatter fix, then a mod of its own:
Rock Precision Fix 0.1.0, the same two patches. The terrain fix is in both configurations because the
scatter fix is meant to go with it. KSP full screen at 1280×720
(`FULLSCREEN = True` in its `settings.cfg`), so that no other window can come in front of it; terrain
scatter on at full density, terrain detail High. PQS Bench is not installed.

A command pod on rails in a circular equatorial orbit 5 km over the Mun, from
[the save PQS Bench provides](https://github.com/lhervier/KSP-PQSBench/blob/main/perfs/ref-mune-5km.sfs),
low enough that the game builds the highest subdivision level of the terrain. Every run times the same
70 seconds of that orbit, from 30 s of mission time.

Three runs per configuration, the two configurations taken in turn. **All of them were taken on my
desktop, in one session of runs**, described with the logs in [`perfs/`](../../../../perfs/README.md#the-scatter-fix); figures
from another machine, or another session of runs, are not comparable to these.

### How the frames were timed

Each run is a fresh KSP:

1. Load the save. Right after the load, about 3.5 s of mission time, turn the camera to look ahead along
   the orbit, with the Mun's ground on the left two thirds of the screen. In orbit, KSP's camera is in
   the orbit's frame, so the Mun's limb stands vertical: heading 204° in that frame, pitch 0, default
   distance and field of view.
2. Open the profiler's window, as a player has it, KSP's interface shown. Captured frames at 10 000 (the
   most the window accepts; the profiler stops by itself when it is reached), *AutoCapture* off. Bring
   KSP's window in front of the others, and move the mouse pointer away from the craft, to the middle of
   the screen's left edge.
3. *Start capture* at 30 s of mission time, *Stop capture* at 1 min 40 s, at ×1 all along. Nothing is
   asked of the game in between: the script sleeps through the window, and KSP-MCPServer shows no message
   on screen.
4. *Export to CSV*. The CSV does not hold the number of captured frames: by hand, read it off the window.

A run is worth keeping when fewer than 10 000 frames were captured, so that the capture ended on *Stop*
and not before. All six did, with 6 584 to 6 749 frames each. The script checked at the start and at
the stop of every capture that KSP's window was in front: it was, every time.

## What a frame pays

The CSV names two rows `Update` and several `Coroutines`. Below, **Update → Coroutines** is the
coroutines run inside the `Update` phase — among them `PQS.UpdateSphere`, along with whatever other
coroutines ran that frame. That is the row the scatter fix's work is counted in. In milliseconds per frame,
*alone* for the terrain fix alone (every row of every run is
[with the runs](../../../../perfs/README.md#the-scatter-fix-the-figures)):

| | alone 1 | with the scatter fix 1 | alone 2 | with the scatter fix 2 | alone 3 | with the scatter fix 3 |
|---|---|---|---|---|---|---|
| frame time, mean | 10.42 | 10.50 | 10.55 | 10.45 | 10.40 | 10.65 |
| frame time, worst 1 % | 35.16 | 35.53 | 35.33 | 35.24 | 35.54 | 35.67 |
| Update → Coroutines, mean | 1.93 | 1.95 | 1.94 | 1.93 | 1.93 | 2.00 |
| Update → Coroutines, median | 1.28 | 1.28 | 1.29 | 1.27 | 1.28 | 1.28 |
| Update → Coroutines, worst 1 % | 24.39 | 24.53 | 24.32 | 24.41 | 24.89 | 24.98 |
| VSync, mean | 0.06 | 0.06 | 0.06 | 0.06 | 0.06 | 0.06 |

**VSync** near zero says the frame waited on the processor, not on the screen or the graphics card —
which is what a frame measurement of the terrain needs. Averaged over the three runs of each
configuration:

| | the terrain fix alone | with the scatter fix |
|---|---|---|
| frame time, mean | 10.46 | 10.53 |
| Update → Coroutines, mean | 1.93 | 1.96 |
| Update → Coroutines, worst 1 % | 24.53 | 24.64 |

On Update → Coroutines, the terrain fix alone spends 1.93 to 1.94 ms per frame on average over its
three runs, 1.93 ms on average, and 1.93 to 2.00 ms with the scatter fix, 1.96 ms on average: 0.03 ms more,
made by one run, the third, at 2.00 ms; the other two, at 1.95 and 1.93 ms, are within 0.02 ms of the
runs without it. The three runs with the scatter fix spread by 0.07 ms, more than the two averages differ. The
mean frame time leans the same way, and for the same run: 10.46 ms alone and 10.53 ms with the scatter fix on
average, the third run with it at 10.65 ms.

## What this says

**No cost these runs can resolve.** With the scatter fix, the coroutines the terrain is updated in average
0.03 ms more per frame, but one run of the three makes that difference, and the runs with the scatter fix spread
by 0.07 ms, more than it. Whatever the scatter fix costs is at most of the order of that spread; these runs show
neither a gain nor a cost.
