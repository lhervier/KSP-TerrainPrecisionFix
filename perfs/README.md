# What this fix costs

The runs this mod's published figures are read from. Two of them, plus the two references they are read
against, which live where they belong: with the mods that produced them.

Measured with [PQS Bench](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench), **whose page
carries the procedure** — the craft, the orbit, how long to fly, and what makes a run worth keeping.

## The runs

2026-09-15, KSP 1.12.5. `GameData` holding Harmony, ModuleManager, KSP Community Fixes, the measuring
mod and this one. A command pod on rails in a circular orbit 5 km over the Mun, 150 seconds of game
time. The save and the machine are the reference run's, both described on
[its page](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench/blob/main/perfs/README.md) —
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

## The three configurations

Each has its own repository, its own logs and its own reading of them. All six runs are the same save,
the same day, the same machine. Each is read against its own `stock` yardstick, since from one session
of KSP to the next the whole replay runs a little faster or slower — [PQS Bench](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench/blob/main/perfs/README.md)
shows by how much:

| installed | `differenceNsPerVertex` | where it is measured |
|---|---|---|
| nothing | −1.1, the floor of the method | [PQS Bench](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench/blob/main/perfs/README.md) |
| stock's arithmetic, `Transform`s read once per quad | −82.8 | [Stock Quad Cache](https://github.com/lhervier/KSP-TerrainPrecisionFix-StockQuadCache/blob/main/perfs/README.md) |
| **this fix** | **−164.3** | here |

The middle row is a mod written for this measurement alone, which runs stock's arithmetic bit for bit
and differs only in reading the two `Transform`s once per quad. It is there because this fix changes
**two** things at once, and it splits the saving between them:

| | per vertex |
|---|---|
| reading the two `Transform`s on every vertex | **−82.8 ns**, 41 ns each |
| the arithmetic | **−81.5 ns** |
| total | **−164.3 ns**, 228.5 → 64.2 in this run, **3.56× faster** |

## What the calibrate run says

40 quads, 72 000 vertices per formula:

| | |
|---|---|
| `stockNsPerVertex` (this run's own yardstick) | 228.5 |
| `installedNsPerVertex` | 64.2 |
| `differenceNsPerVertex` | **−164.3** |

In the run with nothing installed, the two columns are the same code reached two different ways and
differ by 1.1 ns; that is the floor of the method.

## In flight

| | this fix | [nothing](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench/blob/main/perfs/README.md) | [`Transform`s hoisted](https://github.com/lhervier/KSP-TerrainPrecisionFix-StockQuadCache/blob/main/perfs/README.md) |
|---|---|---|---|
| frames per second | 79.16 | 77.04 | 75.82 |
| ms per quad of the highest level | **1.650** | 1.716 | 1.682 |
| terrain per frame | 1.182 ms | 1.195 ms | 1.170 ms |
| terrain share of real time | 9.36 % | 9.21 % | 8.87 % |

Per quad, the fix is ahead by 3.8 %, where the calibration predicts 2.2 % — 164.3 ns × 225 vertices is
37.0 µs per quad. **That should not be read as a measurement.** The third column is the reason: by the
same reckoning it should be 1.1 % ahead, and it is 2.0 % ahead; per frame it is ahead of the fix; and
the fix, faster per quad, takes a larger share of real time than stock. At this scale the noise between
two sessions of KSP is worth as much as the effect being looked for.

What these runs establish is a bound — **nothing degrades at the scale of a frame** — and the figure
worth publishing is the calibration.

Nor is the saving worth having for its own sake. At 5 km over the Mun the game builds 8.5 of these quads
per second, so 1 917 vertices: 164.3 ns each is 0.31 ms per second of flight, 0.031 % of real time. And
placing a vertex is a small part of building one — a quad takes 1.7 ms, nearly all of it in the
`PQSMod`s that compute height and colour. The point is not the gain. It is that the correction is free.

## Why the per-quad work is worked out once

The saving is not in the arithmetic alone. Of the 164.3 ns, **82.8 come from not asking Unity for a
`Transform` on every vertex** — that is what the middle configuration measures, and it is two reads.
This fix works out rather more than two things per quad: whether it applies at all, the frame the quad
hangs in, and the inverse of its rotation. Doing any of that per vertex instead would cost several
times what the placement itself costs, which is why the code is written the way it is.
