# What this fix costs: the runs

The logs this mod's performance figures are read from. The figures themselves, and what they say, are
in [Performance](../docs/performance.md).

Two instruments, for two questions: what placing one terrain vertex costs, and what a whole frame pays.
Every run was flown on the same save, on the same machine, and in the same session of runs as the stock
reference runs kept with PQS Bench, which describe
[the save and the machine](https://github.com/lhervier/KSP-PQSBench/blob/master/perfs/README.md) —
figures from another machine are not comparable to these.

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
[stock](https://github.com/lhervier/KSP-PQSBench/blob/master/perfs/README.md), in PQS Bench, and
[stock's arithmetic with the `Transform`s read once per
quad](https://github.com/lhervier/KSP-TerrainPrecisionFix-StockQuadCache/blob/master/perfs/README.md), in
Stock Quad Cache.

## What a frame pays

Measured with [KSPProfiler](https://github.com/KSPModdingLibs/KSPProfiler) 1.0.0, **by the procedure
written in [Performance](../docs/performance.md#how-the-frames-were-timed)**, which also reads the
figures. The three configurations are kept here together, because they are only ever read against each
other.

Six runs, two per configuration, in this order: Stock Quad Cache, this mod, stock, and again. A seventh,
the first one flown, is not here: its save was reloaded in flight without restarting KSP.

| configuration | run | frames captured | CSV | `KSP.log` |
|---|---|---|---|---|
| stock | 1 | 7 084 | [csv](runs/profiler/mun-05km-stock-1.csv) | [log](runs/profiler/mun-05km-stock-1.log) |
| stock | 2 | 6 606 | [csv](runs/profiler/mun-05km-stock-2.csv) | [log](runs/profiler/mun-05km-stock-2.log) |
| Stock Quad Cache | 1 | 6 724 | [csv](runs/profiler/mun-05km-stockquadcache-1.csv) | [log](runs/profiler/mun-05km-stockquadcache-1.log) |
| Stock Quad Cache | 2 | 6 714 | [csv](runs/profiler/mun-05km-stockquadcache-2.csv) | [log](runs/profiler/mun-05km-stockquadcache-2.log) |
| this mod | 1 | 6 785 | [csv](runs/profiler/mun-05km-fix-1.csv) | [log](runs/profiler/mun-05km-fix-1.log) |
| this mod | 2 | 6 988 | [csv](runs/profiler/mun-05km-fix-2.csv) | [log](runs/profiler/mun-05km-fix-2.log) |

Every run's frame count is under the profiler's 10 000 ceiling, and matches 70 seconds at its mean
frame rate. Each `KSP.log` says which mods were loaded, and in the runs with this mod, that it placed
the Mun's terrain in double precision.
