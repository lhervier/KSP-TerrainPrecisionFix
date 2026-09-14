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
`calibrate` does real work of its own in the frames `counters` times.

Their `BENCH run` lines match the reference runs': same save, same craft, over the Mun from UT 335.5 at
4 999.8 m, for 150 seconds of game time against as much real time. The `counters` run recorded 146
samples, one short of the others, and built **2 442 quads, of which 952 of the highest subdivision
level** — the only ones this mod acts on. Every figure read from it is a rate, which a second less does
not move. The craft is on rails; the same save covers the same ground.

## The three configurations

Each has its own repository, its own logs and its own reading of them. All six runs are the same save,
the same day, the same machine. Each is read against its own `stock` yardstick, since from one session
of KSP to the next the whole replay runs a little faster or slower — [PQS Bench](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench/blob/main/perfs/README.md)
shows by how much:

| installed | `differenceNsPerVertex` | where it is measured |
|---|---|---|
| nothing | +1.2, the floor of the method | [PQS Bench](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench/blob/main/perfs/README.md) |
| stock's arithmetic, `Transform`s read once per quad | −76.1 | [Stock Quad Cache](https://github.com/lhervier/KSP-TerrainPrecisionFix-StockQuadCache/blob/main/perfs/README.md) |
| **this fix** | **−180.0** | here |

The middle row is a mod written for this measurement alone, which runs stock's arithmetic bit for bit
and differs only in reading the two `Transform`s once per quad. It is there because this fix changes
**two** things at once, and it splits the saving between them:

| | per vertex |
|---|---|
| reading the two `Transform`s on every vertex | **−76.1 ns**, 38 ns each |
| the arithmetic | **−103.9 ns** |
| total | **−180.0 ns**, 288.0 → 107.9 in this run, **2.67× faster** |

## What the calibrate run says

30 quads, 54 000 vertices per formula:

| | |
|---|---|
| `stockNsPerVertex` (this run's own yardstick) | 288.0 |
| `installedNsPerVertex` | 107.9 |
| `differenceNsPerVertex` | **−180.0** |
| `differingQuads` | **30 / 30** |

`differingQuads` is the one number that should *not* be zero here. It counts calibrated quads where a
vertex landed somewhere other than stock puts it, compared exactly — thirty out of thirty, which is the
whole purpose of the mod. The other two configurations read zero.

In the run with nothing installed, the two columns are the same code reached two different ways and
differ by 1.2 ns; that is the floor of the method, and it makes the saving above slightly conservative.

## In flight

| | this fix | [nothing](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench/blob/main/perfs/README.md) | [`Transform`s hoisted](https://github.com/lhervier/KSP-TerrainPrecisionFix-StockQuadCache/blob/main/perfs/README.md) |
|---|---|---|---|
| frames per second | 114.58 | 114.56 | 114.49 |
| ms per quad of the highest level | **2.736** | 2.775 | 2.748 |
| terrain per frame | 0.941 ms | 0.937 ms | 0.940 ms |
| terrain share of real time | 10.78 % | 10.74 % | 10.76 % |

Per quad, the fix is ahead by 1.4 %, and the calibration predicts 1.5 % — 180.0 ns × 225 vertices is
40.5 µs per quad. **That agreement should not be read as a measurement.** The third column is the
reason: by the same reckoning it should be 0.6 % ahead, and it is 1.0 % ahead; and per frame, both mods
come out slightly behind stock. At this scale the noise between two sessions of KSP is worth about as
much as the effect being looked for.

What these runs establish is a bound — **nothing degrades at the scale of a frame** — and the figure
worth publishing is the calibration.

Nor is the saving worth having for its own sake. At 5 km over the Mun the game builds 6.4 of these quads
per second, so 1 440 vertices: 180.0 ns each is 0.26 ms per second of flight, 0.026 % of real time. And
placing a vertex is a small part of building one — a quad takes 2.8 ms, nearly all of it in the
`PQSMod`s that compute height and colour. The point is not the gain. It is that the correction is free.

## Why the per-quad work is worked out once

The saving is not in the arithmetic alone. Of the 180.0 ns, **76.1 come from not asking Unity for a
`Transform` on every vertex** — that is what the middle configuration measures, and it is two reads.
This fix works out rather more than two things per quad: whether it applies at all, the frame the quad
hangs in, and the inverse of its rotation. Doing any of that per vertex instead would cost several
times what the placement itself costs, which is why the code is written the way it is.
