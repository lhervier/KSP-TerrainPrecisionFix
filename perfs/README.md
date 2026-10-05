# What this fix costs: the runs

The logs this mod's performance figures are read from. The figures themselves, and what they say, are
in [Performance](../docs/performance.md).

Two instruments, for two questions: what placing one terrain vertex costs, and what a whole frame pays.
Every run was flown on the same save and on the same machine as the stock reference runs kept with PQS
Bench, which describe [the save and the machine](https://github.com/lhervier/KSP-PQSBench/blob/main/perfs/README.md) —
figures from another machine are not comparable to these. The PQS Bench runs belong to the same session
of runs as those, KSP in a 1280×720 window; the profiler runs, to a later one, KSP full screen at
1280×720.

## What a vertex costs

Measured with [PQS Bench](https://github.com/lhervier/KSP-PQSBench), **whose page carries the
procedure** — the craft, the orbit, how long to fly, and what makes a run worth keeping.

KSP 1.12.5. `GameData` holding Harmony, ModuleManager, KSP Community Fixes 1.41.1, the measuring mod and
this one. A command pod on rails in a circular orbit 5 km over the Mun, 70 seconds of game time from 30 s
of mission time.

| log | starts at | game time | real time | quads of the highest level built |
|---|---|---|---|---|
| [`mun-05km-fix-calibrate-1.log`](runs/mun-05km-fix-calibrate-1.log) | UT 54.64 | 70.08 s | 70.22 s | 704 |
| [`mun-05km-fix-calibrate-2.log`](runs/mun-05km-fix-calibrate-2.log) | UT 54.76 | 70.16 s | 70.28 s | 704 |

The figures come from their `BENCH run` lines, and match the stock runs': same save, same craft, same
stretch of the same orbit, the same quads built. Their result lines, as logged:

```
BENCH calibration;quads=22;roundsPerQuad=8;verticesPerFormula=39600;stockNsPerVertex=297.5;installedNsPerVertex=110.5;differenceNsPerVertex=-187.0;harnessNsPerVertex=6.8;stockRawNsPerVertex=304.3;installedRawNsPerVertex=117.3
BENCH calibration;quads=22;roundsPerQuad=8;verticesPerFormula=39600;stockNsPerVertex=286.1;installedNsPerVertex=107.2;differenceNsPerVertex=-178.9;harnessNsPerVertex=4.7;stockRawNsPerVertex=290.7;installedRawNsPerVertex=111.9
```

The two other configurations are kept with the mod that produced them:
[stock](https://github.com/lhervier/KSP-PQSBench/blob/main/perfs/README.md), in PQS Bench, and
[stock's arithmetic with the `Transform`s read once per
quad](https://github.com/lhervier/KSP-TerrainPrecisionFix-StockQuadCache/blob/main/perfs/README.md), in
Stock Quad Cache.

## What a frame pays

Measured with [KSPProfiler](https://github.com/KSPModdingLibs/KSPProfiler) 1.0.0, through
[a fork of it](https://github.com/lhervier/KSP-ExtMod-KSPProfiler) that
[KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer) can drive, **by the procedure written in
[Performance](../docs/performance.md#how-the-frames-were-timed)**, which also reads the figures. The runs
were flown by [this script](https://github.com/lhervier/KSP-PQSBench/blob/main/perfs/automation/run-perfs.py).
The three configurations are kept here together, because they are only ever read against each other.

Nine runs, three per configuration, each in a fresh KSP, in this order: stock, Stock Quad Cache, this
mod, and again, three rounds. They belong to one session of runs that also measured, in each round,
Terrain Precision Fix with [Rock Precision Fix](https://github.com/lhervier/KSP-RockPrecisionFix), for
that mod's page. The mission time at the start and the stop of each capture is as the script recorded
it; it also recorded, at both, that KSP's window was in front of the others, in every run.

| configuration | run | captured from | to | frames captured | CSV | `KSP.log` |
|---|---|---|---|---|---|---|
| stock | 1 | 30.026 s | 100.04 s | 6 612 | [csv](runs/profiler/mun-05km-stock-1.csv) | [log](runs/profiler/mun-05km-stock-1.log) |
| stock | 2 | 30.037 s | 100.02 s | 6 574 | [csv](runs/profiler/mun-05km-stock-2.csv) | [log](runs/profiler/mun-05km-stock-2.log) |
| stock | 3 | 30.030 s | 100.02 s | 6 707 | [csv](runs/profiler/mun-05km-stock-3.csv) | [log](runs/profiler/mun-05km-stock-3.log) |
| Stock Quad Cache | 1 | 29.994 s | 100.10 s | 6 649 | [csv](runs/profiler/mun-05km-stockquadcache-1.csv) | [log](runs/profiler/mun-05km-stockquadcache-1.log) |
| Stock Quad Cache | 2 | 29.981 s | 100.10 s | 6 737 | [csv](runs/profiler/mun-05km-stockquadcache-2.csv) | [log](runs/profiler/mun-05km-stockquadcache-2.log) |
| Stock Quad Cache | 3 | 29.993 s | 100.12 s | 6 758 | [csv](runs/profiler/mun-05km-stockquadcache-3.csv) | [log](runs/profiler/mun-05km-stockquadcache-3.log) |
| this mod | 1 | 30.020 s | 100.10 s | 6 730 | [csv](runs/profiler/mun-05km-fix-1.csv) | [log](runs/profiler/mun-05km-fix-1.log) |
| this mod | 2 | 30.000 s | 100.08 s | 6 654 | [csv](runs/profiler/mun-05km-fix-2.csv) | [log](runs/profiler/mun-05km-fix-2.log) |
| this mod | 3 | 30.016 s | 100.12 s | 6 749 | [csv](runs/profiler/mun-05km-fix-3.csv) | [log](runs/profiler/mun-05km-fix-3.log) |

Every run's frame count is under the profiler's 10 000 ceiling, so each capture ended on *Stop*, and
matches 70 seconds at its mean frame rate. Each `KSP.log` says which mods were loaded, in its
`Mod DLLs found` list; the runs with Stock Quad Cache log
`[StockQuadCache] Version 0.1.0.0 installed, log level Info`, and the runs with this mod
`[TerrainPrecisionFix] Statics fix installed` and `[TerrainPrecisionFix] Mun: terrain placed in double
precision`.
