# What this fix costs

The runs this mod's published figures are read from. Two of them, plus the two references they are read
against, which live where they belong: with the mods that produced them.

Measured with [PQS Bench](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench), **whose page
carries the procedure** — the craft, the orbit, how long to fly, and what makes a run worth keeping.

## The runs

2026-09-14, KSP 1.12.5. `GameData` holding Harmony, ModuleManager, KSP Community Fixes, the measuring
mod and this one. A command pod on rails in a circular orbit 5 km over the Mun, 150 seconds of game
time.

| log | mode |
|---|---|
| [`mun-05km-fix-calibrate.log`](runs/mun-05km-fix-calibrate.log) | `calibrate`, what a vertex costs |
| [`mun-05km-fix-counters.log`](runs/mun-05km-fix-counters.log) | `counters`, what a frame pays |

Two runs rather than one because a Harmony patch is installed for a whole session, and because
`calibrate` does real work of its own in the frame it measures, so frame times have to come from a run
without it.

Both built **2 484 quads, of which exactly 960 of the highest subdivision level** — the only ones this
mod acts on — over 147 samples, which is what the reference runs built too. The craft is on rails; the
same save covers the same ground.

## The three configurations

Each has its own repository, its own logs and its own reading of them. All six runs are the same save,
the same day, the same machine.

| installed | ns per vertex | where it is measured |
|---|---|---|
| nothing | 285.2 | [PQS Bench](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench/blob/main/perfs/README.md) |
| stock's arithmetic, `Transform`s read once per quad | 206.6 | [Stock Quad Cache](https://github.com/lhervier/KSP-TerrainPrecisionFix-StockQuadCache/blob/main/perfs/README.md) |
| **this fix** | **103.7** | here |

The middle row is a mod written for this measurement alone, which runs stock's arithmetic bit for bit
and differs only in reading the two `Transform`s once per quad. It is there because this fix changes
**two** things at once, and it splits the saving between them:

| | per vertex |
|---|---|
| reading the two `Transform`s on every vertex | **−78.6 ns**, 39.3 ns each |
| the arithmetic | **−102.9 ns** |
| total | **−181.5 ns**, 285.2 → 103.7, **2.75× faster** |

## What the calibrate run says

30 quads, 54 000 vertices per formula:

| | |
|---|---|
| `installedNsPerVertex` | **103.7** |
| `stockNsPerVertex` (this run's own yardstick) | 281.1 |
| `differingQuads` | **30 / 30** |

`differingQuads` is the one number that should *not* be zero here. It counts calibrated quads where a
vertex landed somewhere other than stock puts it, compared exactly — thirty out of thirty, which is the
whole purpose of the mod. The other two configurations read zero.

The three runs' own `stock` readings — 281.1 here, 283.0 and 283.6 in the references — agree within
0.9 % across three KSP sessions, which is what comparing the `installed` column from one to the next
rests on. In the run with nothing installed the two columns are the same code reached two different
ways and agree to 2.2 ns; that is the floor of the method, and it makes the saving above slightly
conservative.

## In flight

| | this fix | [nothing](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench/blob/main/perfs/README.md) | [`Transform`s hoisted](https://github.com/lhervier/KSP-TerrainPrecisionFix-StockQuadCache/blob/main/perfs/README.md) |
|---|---|---|---|
| frames per second | 114.49 | 114.72 | 114.09 |
| ms per quad of the highest level | **2.735** | 2.771 | 2.795 |
| terrain per frame | **0.942 ms** | 0.947 ms | 0.984 ms |
| terrain share of real time | **10.79 %** | 10.86 % | 11.22 % |

The fix is ahead by 1.3 %, and the calibration predicts 1.5 % — 181.5 ns × 225 vertices is 40.8 µs per
quad. **That agreement should not be read as a measurement.** The third column is the reason: by the
same reckoning it should be 0.6 % ahead, and it is 0.9 % behind. At this scale the noise between two
sessions of KSP is worth about as much as the effect being looked for.

What these runs establish is a bound — **nothing degrades at the scale of a frame** — and the figure
worth publishing is the calibration.

Nor is the saving worth having for its own sake. At 5 km over the Mun the game builds 6.4 of these quads
per second, so 1 440 vertices: 181.5 ns each is 0.26 ms per second of flight, 0.026 % of real time. And
placing a vertex is a small part of building one — a quad takes 2.8 ms, nearly all of it in the
`PQSMod`s that compute height and colour. The point is not the gain. It is that the correction is free.

## Why the per-quad work is worked out once

The saving is not in the arithmetic alone. Of the 181.5 ns, **78.6 come from not asking Unity for a
`Transform` on every vertex** — that is what the middle configuration measures, and it is two reads.
This fix works out rather more than two things per quad: whether it applies at all, the frame the quad
hangs in, and the inverse of its rotation. Doing any of that per vertex instead would cost several
times what the placement itself costs, which is why the code is written the way it is.
