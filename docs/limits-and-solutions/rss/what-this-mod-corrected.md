# Real Solar System: what this mod corrected

Part of [Terrain Precision Fix](../../../README.md), one point of the case [Real Solar System](../rescaled-systems-real-solar-system.md), in [Limits and solutions](../../limits-and-solutions.md).

**Status: checked on five bodies — the safeguard grows with the body, and none of the terrain built on
the Moon, Earth, Venus, Mars and Mercury was left as stock builds it.**

The sessions with this mod ran at `logLevel = Debug`, which logs how far each quad was moved.

This mod refuses any correction too large to be a rounding (see
[Safeguards](../../the-fix-ground.md#safeguards)). Its first version was a fixed **1 m**, chosen
against a rounding of a few centimetres, which a correction of two steps already exceeds on Earth. The
sessions marked *first safeguard* in [`diag/runs`](../../../diag/README.md#on-real-solar-system) ran with
it: on the Moon, 2 464 quads were corrected, by 443 mm at most, 3.5 float steps. Earth's terrain was
built too during those sessions, and that first version refused corrections of 1.094 m and 1.318 m,
leaving part of it as stock builds it:

```
[TerrainPrecisionFix] Earth: a correction of 1.094 m is too large to be a rounding error, the quads concerned are left as stock builds them
```

With the current safeguard, sixteen float steps at the distance of the quad from the centre of the
body, none of the terrain built on these five bodies was refused:

| series | placements of a quad corrected | largest correction |
|---|---|---|
| Earth, the six loads of Diag LandedVessel | 1 564 | 1 998 mm, 4.0 float steps |
| Earth, the six loads of Diag TerrainHeight | 1 576 | 1 230 mm, 2.5 float steps |
| Earth, the 51 reloads in a row | 12 128 | 1 992 mm, 4.0 float steps |
| the Moon, the six loads of Diag TerrainHeight | 2 112 | 347 mm, 2.8 float steps |
| Venus, six loads | 2 112 | 1 792 mm, 3.6 float steps |
| Mars, six loads | 3 604 | 308 mm, 1.2 float steps |
| Mercury, six loads | 1 532 | 259 mm, 1.0 float step |

Venus, where the float step is 500 mm as on Earth, comes next after Earth at 3.6 steps. On Mars and
Mercury, where it is 250 mm, the largest correction stays around one step. Why these two bodies need
fewer steps than Earth, Venus and the Moon is not explained.

*Still to test:* descents onto the sites of Venus, Mars and Mercury, flown by an autopilot so they can
be replayed. Every series on this page loads a landed craft again, while a player builds terrain
continuously during a descent.

## The saves

The loads on Venus, Mars and Mercury, taken with this mod alone, have their saves in this repository's
[`diag`](../../../diag) folder: the same pod on its tank, placed with *Set Position* in the debug menu
(Alt+F12, *Cheats*).

- [`reload-venus-rss.sfs`](../../../diag/reload-venus-rss.sfs) — Venus (latitude −13.05°, longitude
  −49.81°, the plain where Venera 14 landed), the craft *landed*;
- [`reload-mars-rss.sfs`](../../../diag/reload-mars-rss.sfs) — Mars (latitude 47.64°, longitude 134.29°,
  Utopia Planitia, where Viking 2 landed), the craft in *prelaunch*;
- [`reload-mercury-rss.sfs`](../../../diag/reload-mercury-rss.sfs) — Mercury (latitude 73.40°, longitude
  −79.50°, Borealis Planitia), the craft *landed*. Real Solar System roughens Mercury's terrain
  everywhere with two layers of noise, finer than the collision mesh can follow: at each load, KSP
  first lifts the craft to the height it computes for that spot, 22.4 m above the mesh, then its
  ground pass moves it back down, to where it was saved. That gap is terrain detail, far beyond any
  rounding.

Copy a save into the folder of a sandbox game and load it from that game.
