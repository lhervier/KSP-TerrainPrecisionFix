# Principia: the bodies it moves and turns

Part of [Terrain Precision Fix](../../../README.md), one test of [Non-regression tests](../../non-regression.md), on [Principia](../../non-regression.md#principia).

**Status: checked, no problem.** [Principia](https://github.com/mockingbirdnest/Principia) replaces KSP's
orbital mechanics, and to do so it takes over the two values this fix places the terrain from: the
rotation and the position of every body. It writes them the way stock does. With it installed, a landed
craft comes back from a load, and the ground is built under a rocket in flight, as they do without it.

## What Principia touches

Read in the source of Principia Lévy, its release for KSP 1.12.5 (`ksp_plugin_adapter/ksp_plugin_adapter.cs`).

Principia has no Harmony patch and no terrain code. Two things it does reach the values this mod reads:

- **the rotation of every body, always.** Every frame, `SetBodyFrames` writes `body.rotation`, in double,
  and copies it into `bodyTransform.rotation`, in float: the same pair stock keeps, written the same way.
  This fix reads whatever rotation Principia has set, like the rest of the game;
- **the position of every body, while a craft flies.** Principia manages a craft only while it flies,
  sub-orbital, in orbit or escaping, never while it is landed (`UnmanageabilityReasons`). While the active
  craft flies, Principia moves the bodies by writing `celestial.position` directly rather than through a
  floating origin shift. The stock setter passes that on to `PQS.PrecisePosition`, which moves every
  quad by the same amount, in double, from the position this fix gave it (`PQ.FastUpdateSubQuadsPosition`):
  the correction is carried along, not rounded again.

A landed craft therefore only meets the first, and a craft in flight both. Two protocols cover them:
loading the same save, and a flight over the quads of the highest level.

## Measured

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1, Principia Lévy on the stock system,
the instruments of each protocol, and [KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer), which
plays the protocols; this mod with its defaults (at `logLevel = Debug`), or not. Each protocol is played
once without this mod and once with it.

On a save made without it, Principia starts from a state of its own: it writes
`No principia state found, creating one` to its own log, `glog/Principia/WARNING.*` in the folder of KSP,
and the game saved afterwards holds a `SCENARIO` named `PrincipiaPluginAdapter`. That line is in its log
for every session below; `KSP.log` alone does not show that Principia runs.

**Loading the same save.** [The loading protocol](../../checking-the-culprit-loading/the-protocol.md),
played by its script,
[`run-loading.py`](../../../diag/automation/run-loading.py),
on its four saves: a capsule on a small tank, on flat ground on Kerbin, the Mun, Minmus and
Gilly, each loaded six times. Spread over the six loads:

| series | *Settled*, without this mod | *Settled*, with this mod | the ground under the craft, without this mod | the ground under the craft, with this mod |
|---|---|---|---|---|
| Kerbin | 29.566 mm | 0.016 mm | 29.546 mm | 0.004 mm |
| Mun | 16.473 mm | 0.042 mm | 16.409 mm | 0.007 mm |
| Minmus | 8.002 mm | 0.021 mm | 8.011 mm | 0.017 mm |
| Gilly | 0.280 mm | 0.055 mm | 0.264 mm | 0.005 mm |

Without Principia, the same saves spread *Settled* over 43.7, 18.2, 7.3 and 2.3 mm on stock, and over
0.010 to 0.060 mm with this mod ([Checking the culprit: loading the same save](../../checking-the-culprit-loading.md#the-craft-over-six-loads)).
On stock, the spread is a draw from load to load: on Gilly, where a float's step is 1.95 mm, the six
loads of this series happened to fall within 0.28 mm.

**In flight.** [The protocol of the quads of the highest level, in flight](https://github.com/lhervier/KSP-Diag-TerrainQuads/blob/main/docs/the-protocol-flight.md)
of KSP Diag - Terrain Quads, with KSP Diag - Floating Origin, played by its script,
[`run-flight.py`](https://github.com/lhervier/KSP-Diag-TerrainQuads/blob/main/docs/the-protocol-flight.md#played-by-a-script),
and read by its `analyse-flight.py`: `Quad-Rocket` launched from the launchpad of the Space Center, one
*Log* about every second until it falls into the sea. Both flights start from the same game, which holds
no state of Principia yet. Under Principia the rocket goes a little higher and faster than without it:
84 *Logs* without this mod and 83 with it, up to 1,142 m and 1,126 m, into the sea at 738 m/s and 732 m/s,
where the flights without Principia take 72 and 73 *Logs*, up to about 955 m and 654 m/s.

For the quads of level 10, in millimetres, the median, the 90th percentile and the largest value:

| | without this mod | with this mod |
|---|---|---|
| the same quad, from one *Log* to the next, over 100 m/s | 0.059 / 0.229 / 0.757 | 0.056 / 0.240 / 0.806 |
| the step between siblings | 0.007 / 0.039 / 0.426 | 0.063 / 0.232 / 0.920 |
| the step between cousins both there from the first *Log*, built as the scene opened | 0.006 / 0.027 / 0.403 | 0.063 / 0.233 / 0.753 |
| **the step between cousins, one at least built during the flight** | **6.494 / 18.534 / 36.162** | **0.063 / 0.214 / 0.821** |

Without Principia, the last line reads 4.520 / 16.930 / 22.454 on stock and 0.060 / 0.202 / 0.674 with
this mod ([Checking the culprit: in flight](../../checking-the-culprit-flight.md#on-kerbin)): with this
mod, the same median and 90th percentile, and 0.15 mm more at the very largest. Under
100 m/s, nothing moved either way. The log shows no warning and no error from this mod.

The logs and the readings are in [`diag/non-regression/principia/the-bodies-it-moves-and-turns/`](../../../diag/README.md#with-principia).
