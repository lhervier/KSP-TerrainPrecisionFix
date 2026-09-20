# What this fix costs: the runs

The logs this mod's performance figures are read from. The figures themselves, and what they say, are
in [Performance](../docs/performance.md).

Measured with [PQS Bench](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench), **whose page
carries the procedure** — the craft, the orbit, how long to fly, and what makes a run worth keeping.

## The runs

2026-09-15, KSP 1.12.5. `GameData` holding Harmony, ModuleManager, KSP Community Fixes 1.41.1, the
measuring mod and this one. A command pod on rails in a circular orbit 5 km over the Mun, 150 seconds of
game time. The save and the machine are the reference run's, both described on
[its page](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench/blob/master/perfs/README.md) —
figures from another machine are not comparable to these.

| log | mode |
|---|---|
| [`mun-05km-fix-calibrate.log`](runs/mun-05km-fix-calibrate.log) | `calibrate`, what a vertex costs |
| [`mun-05km-fix-counters.log`](runs/mun-05km-fix-counters.log) | `counters`, what a frame pays |

Two runs rather than one because a Harmony patch is installed for a whole session, and because
`calibrate` does real work of its own in the frames `counters` times.

Their `BENCH run` lines match the reference runs': same save, same craft, over the Mun from UT 54.72 and
54.70 at 5 000.0 m, for 150 seconds of game time against as much real time. The `counters` run recorded
148 samples and built **3 213 quads, of which 1 272 of the highest subdivision level** — the only ones
this mod acts on. The craft is on rails; the same save covers the same ground.

The `calibrate` run's result line, as logged:

```
BENCH calibration;quads=40;roundsPerQuad=8;verticesPerFormula=72000;stockNsPerVertex=228.5;installedNsPerVertex=64.2;differenceNsPerVertex=-164.3;harnessNsPerVertex=3.5;stockRawNsPerVertex=232.0;installedRawNsPerVertex=67.7
```

## The other two configurations

Read against runs of the same campaign, each kept with the mod that produced it:
[stock](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench/blob/master/perfs/README.md), in
PQS Bench, and [stock's arithmetic with the `Transform`s read once per
quad](https://github.com/lhervier/KSP-TerrainPrecisionFix-StockQuadCache/blob/master/perfs/README.md), in
Stock Quad Cache.
