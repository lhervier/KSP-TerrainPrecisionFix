# Rescaled systems: Real Solar System

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: checked, no problem — this mod works with Real Solar System and breaks nothing, measured on
the Moon. On Earth, the 1 m safeguard leaves part of the terrain as stock builds it: nothing breaks, but
that part stays uncorrected.**

The defect grows with the radius of the body. A float's step doubles every time a distance crosses a
power of two, so the rounding this fix removes is not the same size on a rescaled body:

| body | radius | float step at that distance |
|---|---|---|
| Kerbin | 600 000 m | 62.5 mm |
| the Moon, in [Real Solar System](https://github.com/KSP-RO/RealSolarSystem) | 1 737 100 m | 125 mm |
| Earth, in Real Solar System | 6 371 000 m | 500 mm |

On Kerbin, the spreads measured in
[Checking the culprit: loading the same save](../checking-the-culprit-loading.md) are about two steps.
At the same number of steps, the ground of the Moon should move by about 25 cm from one load to the
next, and Earth's by about a metre.

**Real Solar System already works around the symptom.** Its source ships a component of its own,
`VesselGroundPositionEnhancer`
([its source](https://github.com/KSP-RO/RealSolarSystem/blob/master/Source/VesselGroundPositionEnhancer.cs)),
added to *"mostly prevent vessels clipping into the ground and as a result flung into the air"*
([pull request #257](https://github.com/KSP-RO/RealSolarSystem/pull/257)). Whenever a landed craft goes
off rails, it runs the stock `Vessel.CheckGroundCollision`, which moves the craft onto the ground before
its physics starts whenever it is more than 10 cm off, and logs `ground contact! - error. Moving Vessel
up X.XXXm`. A craft that comes back more than 10 cm inside the ground is then not launched but moved,
in one block. Under 10 cm, the stock method leaves the craft where it is, inside the ground or not. The
component turns itself off when an assembly named `WorldStabilizer` is loaded, the mod it was written to
stand in for. The repository also has an option to force it off, which release 20.1.3.0 does not have
yet.

**This fix has a safeguard sized for Kerbin.** It refuses any correction larger than **1 m**, a fixed
value chosen against a rounding of a few centimetres. On Earth, a correction of two steps already
exceeds it.

## Checking the culprit

**The install.** KSP 1.12.5 with Harmony, ModuleManager and the same release of KSP Community Fixes as
every other campaign, plus Real Solar System 20.1.3.0 and what it requires (Kopernicus 248, Modular
Flight Integrator, KSPTextureLoader, the RSS textures), and
[Terrain Precision Fix Diag 1](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag). Nothing else.
The series marked *RSS's pass off* add an empty assembly named `WorldStabilizer` in `GameData`, with
nothing in it, which is what turns Real Solar System's component off.

### Terrain Precision Fix Diag 1, on the Moon

The protocol of
[Terrain Precision Fix Diag 1, over six loads](../checking-the-culprit-loading.md#the-craft-over-six-loads):
a Mk1 pod on an empty FL-T100, on flat ground on the Moon (latitude 28.61°, longitude −80.62°), saved
landed, then loaded six times from the pause menu, each reading recorded once *Settled* has frozen.
The first two rows are the same save. The last two are that save loaded once with this mod, then saved
again (see [Existing saves](existing-saves.md)).

| install | what the craft does | spread of *Settled* over six loads |
|---|---|---|
| without this mod, Real Solar System as released | moved up by RSS's pass at three loads out of six, by 0.111 to 0.169 m | 262.6 mm (2.1 float steps) |
| without this mod, RSS's pass off | tips over at the first load | — |
| with this mod, RSS's pass off | stays put, no `Moving Vessel` line | 0.364 mm |
| with this mod, Real Solar System as released | stays put; RSS's pass runs at every load and never moves it | 0.395 mm |

![Six loads on the Moon, without this mod](../../imgs/Diag1/on-load/2parts/rss/10-moon-stock.png)

*Without this mod, Real Solar System as released: six loads of the save, each recorded.*

![The first load on the Moon, without this mod and without Real Solar System's pass](../../imgs/Diag1/on-load/2parts/rss/20-moon-stock-no-ground-positioning.png)

*Without this mod, RSS's pass off: the first load of the same save.*

![Six loads on the Moon, with this mod and without Real Solar System's pass](../../imgs/Diag1/on-load/2parts/rss/30-moon-fix.png)

*With this mod, RSS's pass off: six loads of the save taken again with this mod.*

![Six loads on the Moon, with this mod and Real Solar System as released](../../imgs/Diag1/on-load/2parts/rss/40-moon-fix-rss-as-released.png)

*With this mod, Real Solar System as released: six loads of that same save.*

The two sessions without this mod are logged in
[Diag 1's `diag/runs`](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag/tree/main/diag/runs),
files `reload-moon-rss-stock.log` and `reload-moon-rss-vgpeoff-stock.log`, with the save as
`diag/reload-moon-rss.sfs`; the two sessions with it in
[`diag/runs/reload-moon-rss-fix.log`](../../diag/runs/reload-moon-rss-fix.log) (RSS's pass off) and
[`diag/runs/reload-moon-rss-fix-vgpe-on.log`](../../diag/runs/reload-moon-rss-fix-vgpe-on.log) (as released),
with their save as [`diag/reload-moon-rss-resave.sfs`](../../diag/reload-moon-rss-resave.sfs).

### What this mod corrected

The sessions with this mod ran at `logLevel = Debug`, which logs how far each quad was moved. On the
Moon, 2 464 quads were corrected, by 443 mm at most, 3.5 float steps. Earth's terrain was built too
during those sessions: 180 quads corrected, by 966 mm at most, and one refused:

```
[TerrainPrecisionFix] Earth: a correction of 1.094 m is too large to be a rounding error, the quads concerned are left as stock builds them
```

### Terrain Precision Fix Diag 2, on the Moon

The protocol of
[Terrain Precision Fix Diag 2, over six loads](../checking-the-culprit-loading.md#the-ground-over-six-loads),
which reads the ground itself rather than the craft: a ray straight down under the craft, against the
height KSP computes for that same spot. Same install with
[Terrain Precision Fix Diag 2](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2) added, Real
Solar System as released, the save taken again with this mod, loaded six times.

| install | spread of the ground under the craft over six loads | *Difference* |
|---|---|---|
| without this mod | 241.2 mm (1.9 float steps) | −140.2 to +101.0 mm |
| with this mod | 0.226 mm | −114.10 to −113.88 mm |

Diag 1, read in the same two sessions, agrees: the craft comes back over 241.1 mm without this mod,
moved up by RSS's pass at three loads out of six (0.106 to 0.215 m), and over 0.232 mm with it, never
moved.

![Six loads on the Moon, without this mod, both instruments](../../imgs/Diag2/rss/10-moon-stock.png)

*Without this mod, Real Solar System as released: six loads of the save taken again with this mod.*

![Six loads on the Moon, with this mod, both instruments](../../imgs/Diag2/rss/20-moon-fix.png)

*With this mod, Real Solar System as released: six loads of the same save.*

The session without this mod is logged in
[Diag 2's `diag/runs`](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/tree/master/diag/runs),
file `reload-moon-rss-stock.log`; the session with it in
[`diag/runs/reload-moon-rss-fix-diag2.log`](../../diag/runs/reload-moon-rss-fix-diag2.log).

### Reloading until something happens

No instrument is needed for this one: the same craft, Real Solar System as released, reloaded from the
pause menu again and again, watching whether it jumps or tips over. Each load leaves one
`[RSS-VGPE] CheckGroundCollision()` line in `KSP.log`, which counts them, and a `Moving Vessel` line
whenever RSS's pass moved the craft.

**Without this mod**, the save `diag/reload-moon-rss.sfs`, made without it, loaded 14 times (both
instruments were open, and read the craft and the ground over a spread of 322.6 mm):

| loads | what the craft does | *Moved*, from Diag 1 |
|---|---|---|
| 3 of 14 | **jumps**: it came back 17 to 38 mm inside the ground, under the 10 cm RSS's pass acts on | +17.1 to +38.2 mm |
| 6 of 14 | moved up by RSS's pass before its physics starts, by 0.101 to 0.234 m | +100.2 to +233.5 mm |
| 5 of 14 | nothing visible: the ground came back lower | −89.1 to +0.7 mm |

![Fourteen loads on the Moon, without this mod](../../imgs/Diag1/on-load/2parts/rss/50-moon-stock-14-loads.png)

*Without this mod, Real Solar System as released: fourteen loads of the save made without it.*

**With this mod**, the save `diag/reload-moon-rss-resave.sfs`, taken again with it, loaded 24 times with
no instrument installed: the craft never moved, and the log holds 24 `[RSS-VGPE]` lines and no
`Moving Vessel` line.

The session without this mod is logged in
[Diag 1's `diag/runs`](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag/tree/main/diag/runs),
file `reload-moon-rss-stock-14loads.log`; the session with it in
[`diag/runs/reload-moon-rss-fix-24loads.log`](../../diag/runs/reload-moon-rss-fix-24loads.log).

## What the results show

**This mod works with Real Solar System, and breaks nothing.** With both installed, as players would
have them, Real Solar System's own stabiliser still runs at every load and never finds anything to
correct; the craft stays put, load after load. The rest of this section details why.

**The defect is there, at the size the float step predicts.** Without this mod, the ground itself comes
back over 241.2 mm, and the craft resting on it over 241.1 to 262.6 mm, about two steps of 125 mm: the
same number of steps as on Kerbin, on a step twice as large. With this mod, the ground comes back over
0.226 mm. The *Difference* of about −114 mm that stays is the same at every load: it is how far the
collision mesh lies from the computed height at that spot, not a rounding.

**Real Solar System's pass catches part of it, and hides it.** A craft that comes back more than 10 cm
inside the ground is moved up in one block, here at six loads out of fourteen: nothing is launched, but
a structure resting on several points is lifted by its lowest one. A craft that comes back less than
10 cm inside the ground is left there, and the physics engine pushes it out: three jumps in fourteen
loads, on a save made without this mod. On the Moon, 10 cm is less than one float step. With the pass
off, the craft tips over at the very first load.

**This mod makes the pass unnecessary, and does not fight it.** With this mod, the craft stays put over
a few tenths of a millimetre, with the pass off as with it on, and 24 loads in a row did not make it jump
once. The pass still runs at every load, and
never has anything to move: the ground comes back within a millimetre, well inside the 10 cm below which
the pass leaves a craft where it is. What is left is wider than on Kerbin's campaigns, and 0.3 % of a
float step at that distance.

**On Earth, the safeguard is too tight.** A correction of 1.094 m was refused, and of 1.318 m in the
24-load session, so part of Earth's
terrain is corrected and part is left as stock builds it. On the Moon, the largest correction, 443 mm,
stays under it.

**Solution:** a limit proportional to the float step at the body's radius, rather than a fixed metre, so
that it keeps the same meaning — a correction no rounding can produce — on any body. It needs room
above the 3.5 steps already seen on the Moon. Not done yet.

*Still to test:* Earth itself, once the safeguard has changed.
