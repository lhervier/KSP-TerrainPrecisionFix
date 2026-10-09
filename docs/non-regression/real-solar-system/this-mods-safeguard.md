# Real Solar System: this mod's safeguard

Part of [Terrain Precision Fix](../../../README.md), one test of [Non-regression tests](../../non-regression.md), on [Real Solar System](../../non-regression.md#real-solar-system).

**Status: checked on the Moon, Earth, Venus, Mars and Mercury — the safeguard never refused a
correction. The largest one was 4.0 float steps, a quarter of the limit.**

*The rule.* This mod refuses any correction larger than sixteen float steps, taken at the distance from
the centre of the body: the quad, or the static, is then left where stock puts it
([Safeguards](../../the-fix-ground.md#safeguards)). The limit grows with the body, as the rounding does:
1 m on Kerbin, 8 m on Earth.

*Why it is a non-regression test.* A correction refused by mistake puts the defect back where it was
refused. On Earth, ten times the size of Kerbin, a float step is eight times as large: the bodies of
Real Solar System are where a limit set too low would show.

## The margin

The sessions with this mod ran at `logLevel = Debug`, which logs how far each quad and each static was
moved. The quads placed and the largest correction on each body, over every log of the
[runs](../../../diag/README.md) taken with this mod in Real Solar System:

| body | float step | limit | quads placed | largest correction |
|---|---|---|---|---|
| Earth | 500 mm | 8 m | 39 971 | 1 998 mm, 4.0 steps |
| Venus | 500 mm | 8 m | 2 112 | 1 792 mm, 3.6 steps |
| the Moon | 125 mm | 2 m | 17 600 | 456 mm, 3.6 steps |
| Mars | 250 mm | 4 m | 5 140 | 308 mm, 1.2 steps |
| Mercury | 250 mm | 4 m | 1 532 | 259 mm, 1.0 step |

The statics of Earth stay further from it: the KSC was corrected by 1 541 mm at most, 3.1 steps, over
72 placements, and the tracking station of Cape Canaveral by 1 317 mm, 2.6 steps, over 12.

## A limit that does not grow: the first version

The first version of the safeguard was a fixed metre, set against roundings of a few centimetres on
Kerbin. On Earth, a correction of two float steps already exceeds it. In the sessions marked *first
safeguard* in [the runs](../../../diag/README.md#on-real-solar-system), played on the Moon, Earth's
terrain was built too, and that version refused corrections of 1.094 m and 1.318 m:

```
[TerrainPrecisionFix] Earth: a correction of 1.094 m is too large to be a rounding error, the quads concerned are left as stock builds them
```

Those quads stayed as stock builds them, with the defect, and nothing else showed in the logs: no error
besides those every session of that install shows. This is the line to look for in a log, in its
current wording, given once per body, at the first refusal:

```
[TerrainPrecisionFix] <body>: a correction of … m is more than 16 float steps (… m at that distance), too large to be a rounding error, the quads concerned are left as stock builds them
```

For a static, the line ends with *the statics concerned are left where stock places them*.

## The saves

The loads on Venus, Mars and Mercury, taken with this mod alone, have their saves in this repository's
[`diag/non-regression/real-solar-system/this-mods-safeguard`](../../../diag/non-regression/real-solar-system/this-mods-safeguard) folder: the same pod on its tank, placed with *Set Position* in the debug menu
(Alt+F12, *Cheats*).

- [`reload-venus-rss.sfs`](../../../diag/non-regression/real-solar-system/this-mods-safeguard/reload-venus-rss.sfs) — Venus (latitude −13.05°, longitude
  −49.81°, the plain where Venera 14 landed), the craft *landed*;
- [`reload-mars-rss.sfs`](../../../diag/non-regression/real-solar-system/this-mods-safeguard/reload-mars-rss.sfs) — Mars (latitude 47.64°, longitude 134.29°,
  Utopia Planitia, where Viking 2 landed), the craft in *prelaunch*;
- [`reload-mercury-rss.sfs`](../../../diag/non-regression/real-solar-system/this-mods-safeguard/reload-mercury-rss.sfs) — Mercury (latitude 73.40°, longitude
  −79.50°, Borealis Planitia), the craft *landed*. Real Solar System roughens Mercury's terrain
  everywhere with two layers of noise, finer than the collision mesh can follow: at each load, KSP
  first lifts the craft to the height it computes for that spot, 22.4 m above the mesh, then its
  ground pass moves it back down, to where it was saved. That gap is terrain detail, far beyond any
  rounding.

Copy a save into the folder of a sandbox game and load it from that game.
