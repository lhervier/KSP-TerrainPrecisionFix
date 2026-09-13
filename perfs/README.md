# Performance measurement

What the fix costs, and how to measure it again from nothing. Throwaway, like `Src/Bench.cs`: both go
once the figures have settled into the main README.

## The instrument

`Src/Bench.cs` accumulates in memory and writes nothing until asked, so that writing to `KSP.log` never
lands in the middle of what is being timed. Two settings in
`GameData/TerrainPrecisionFixMod/PluginData/settings.cfg`, read once when KSP starts:

- `benchMode` — `off`, `counters` (what the terrain costs, one line per second of game time) or
  `calibrate` (adds, on one quad in thirty-two, the stock formula timed against this one on the same
  data, in the frame that just built that quad, alternating which runs first).
- `patchEnabled` — `false` leaves the terrain exactly as stock builds it while still counting. That is
  the reference run of an A/B: same DLL, same save, one line changed.

In flight, **Alt+F8** dumps everything recorded to `KSP.log`, **Alt+F7** throws it away. The bench is
static: loading another save does not reset it.

Measure at `logLevel = Info`. `Debug` writes a line per quad and `Trace` a line per vertex, on the very
path being measured.

## Where the fix runs at all

The fix only acts on the highest subdivision level, and the game only builds that level close to the
ground. Both limits are logged by the mod itself, once per sphere, whenever `benchMode` is not `off`:

| sphere | minLevel | maxLevel | highest level appears under | lowest level with a collider | max angle per sample |
|---|---|---|---|---|---|
| Kerbin | 2 | 10 | 9 375 m | 10 | 4.60e-5 rad |
| Mun | 2 | 9 | 6 250 m | 9 | 9.20e-5 rad |
| KerbinOcean | 2 | 7 | 75 000 m | none | 3.68e-4 rad |

Two consequences, and the first one matters beyond this folder:

- **`maxLevelOffset` is 0** on Kerbin and on the Mun, so only the highest level carries a collider — the
  very level the fix acts on. It covers every quad a craft can stand on.
- **Above 6 250 m over the Mun, the patched code never runs.** A flight higher than that measures
  nothing: `patchedQuads` stays at 0 and `subdivisionMax` never reaches 9.

The second limit fights the first. The lower the orbit, the faster the craft crosses the ground, and
past `max angle per sample` the game declines to subdivide that far (`PQ.UpdateSubdivision` only splits
a quad while `subdivision < sphereRoot.maxLevelAtCurrentTgtSpeed`). That angle is sampled at the
`FixedUpdate` rate, so a single long frame can cross it: watch the `speedLevelCap` column, which holds
the lowest ceiling seen during each second.

## Setting up the flight

Nothing to fly, nothing to time by hand — the craft is on rails, so loading the same save twice covers
the same ground twice.

1. A new craft carrying **a command pod and nothing else**. Any craft works; one part keeps it obvious.
2. Launch it. From the VAB or the SPH, it makes no difference.
3. Debug menu (Alt+F12) → *Cheats* → *Set Orbit*. Pick the Mun, then set **Semi-Major Axis to
   205000** and leave every other field at 0. The semi-major axis is measured from the **centre of the
   body**, not from the ground: 5 km up is the Mun's 200 km radius plus 5 000, so 205 000. Zero
   eccentricity and zero inclination give a circular equatorial orbit, which keeps the craft at that one
   altitude and away from the higher ground off the equator. Tick the box that skips the safety checks,
   then *Set Orbit*.

   ![Set Orbit, with a semi-major axis of 205 000 m](../imgs/00-perfs-diag-cheat.png)

4. Save. The craft is now 5 000 m up, crossing the ground at 554 m/s:

   ![The craft in a 5 km orbit of the Mun](../imgs/10-perfs-diag-mun-orbit.png)

The screenshots are from a French install; the fields are in the order above whatever the language.

5 km is a compromise: below the 6 250 m the Mun needs, high enough not to hit a ridge, and low enough
that about 40 % of the quads built are ones the fix acts on. On another body, read `highestLevelUnder`
in the log first and aim well below it.

## Taking the measurements

Three runs, each one a fresh KSP — `settings.cfg` is only read at startup. Copy `KSP.log` between two
runs, KSP overwrites it.

| run | `benchMode` | `patchEnabled` |
|---|---|---|
| what a vertex costs | `calibrate` | `true` |
| in flight, with the fix | `counters` | `true` |
| in flight, reference | `counters` | `false` |

Each run: load the save, **Alt+F7** once in flight so that the scene load is not in the samples, fly a
couple of minutes at ×1 — time warp is discarded — then **Alt+F8**. The runs above used Alt+F7 at two
minutes of flight and Alt+F8 at four and a half.

The exact timing does not matter to what is measured: everything is a rate or a ratio. It matters
between the two `counters` runs, which are compared to each other — use the same two marks in both, so
that they cover the same stretch of orbit.

A run is valid when `patchedQuads` is above zero and `speedLevelCap` sits at `maxLevel`. The third run
should announce itself in the log with `patchEnabled = false: the terrain is left exactly as stock
builds it`.

## Results

Measured on 2026-09-13 by the procedure above, KSP 1.12.5 with Harmony, ModuleManager and
KSPCommunityFixes. The three flights are in [runs/](runs/). Only the "before caching" line comes from an
earlier build of the mod, and cannot be taken again without putting that code back.

### What a vertex costs

| | stock | this fix | difference |
|---|---|---|---|
| before caching, 41 quads | 168.6 ns | 500.4 ns | **+331.8 ns** |
| after caching, 30 quads | 167.3 ns | 82.7 ns | **−84.6 ns** |

The first run is why `BuildQuadContext` exists. `PlaceVertex` used to redo, for each of the 225 vertices
of a quad, everything that only depends on the quad: `AppliesTo` (which reads `quad.transform.parent`
and `sphere.LocalSpacePQStorage`), `BodyOf`, the one-metre safeguard, and `quad.transform.position` /
`.rotation` with its inverse. That is several trips into the Unity engine per vertex, against the two
stock makes — hence three times the cost. Worked out once per quad, a vertex is left with a `Vector3d`
subtraction, a rotation in double, a rotation in float and two writes.

**So the fix now places a vertex in half the time stock takes**, which was the expectation recorded in
the TODO and had never been checked: stock calls `Transform.TransformPoint` and
`InverseTransformPoint`, two native calls, where the replacement is managed arithmetic with none.

Stock was measured three times — 167.3, 168.6 and 172.2 ns — in three separate KSP sessions, two of
them on an orbit set up by hand and one on an orbit written into the save by a script. They sit within
1.5 % of each other, which is what the method's own reproducibility is worth.

Neither the cost nor the saving is visible while playing. At 5 km over the Mun the game builds 6.4
patched quads per second, so 1 440 vertices: 84.6 ns each amounts to 0.12 ms saved per second of flight,
0.01 % of real time. The point is not the gain — it is that the correction is free.

### The same flight, in flight

Same save, same two minutes, same build of the mod; `patchEnabled` is the only line that differs.

| | fix | stock |
|---|---|---|
| samples / duration | 146 / 149.0 s | 147 / 150.1 s |
| frames per second | 114.59 | 114.21 |
| quads built per second | 16.38 | 16.55 |
| of which patched | 6.39 (39 %) | 6.40 (39 %) |
| ms per quad | 2.689 | 2.703 |
| **ms per patched quad** | **2.743** | **2.786** |
| terrain per frame | 0.938 ms | 0.958 ms |
| terrain share of real time | 10.75 % | 10.94 % |

The two runs built 2 442 and 2 484 quads, 1.7 % apart, and 952 against 960 patched ones, 0.8 % apart:
two loads of one save really do cover the same ground.

The fix is ahead on every line, and **that must not be read as a 1.5 % gain**. The calibration says the
saving is 84.6 ns × 225 vertices = 19.0 µs per quad, which is 0.7 % of 2.786 ms — half of the 1.5 %
between these two columns. What the pair establishes is a bound: at the scale of a frame, one session
against another, nothing degrades, and what is left is the noise between two runs of KSP. The figure
worth publishing is the calibration; this pair is what shows it changes nothing a player can feel.

### Also worth knowing

Building a quad costs 2.7 ms whether or not the fix touches it — about 12 µs per vertex, nearly all of
it spent in the `PQSMod`s that compute height and colour. Both placements, stock's and this one, are a
fraction of a percent of that.
