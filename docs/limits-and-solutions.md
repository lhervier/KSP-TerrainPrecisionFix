# Limits and solutions

Part of [Terrain Precision Fix](../README.md): the side effects of the fix, and the ground it has not been measured on.

What this fix can make worse, and what it has not been checked against yet — each with its solution,
or with what is still missing for one. Every campaign run with this mod installed was run on KSP 1.12.5,
with Harmony, ModuleManager and KSP Community Fixes 1.41.1 — and, for the series on scatter colliders,
Kopernicus and the patch that gives them.

## Rocks, grass and trees

**Limit.** This mod does not move terrain scatter — the rocks, and around the KSC the grass and the
trees — and the scatter no longer comes back on the ground it is drawn on.

The objects of a quad are built from its vertices, in the quad's own coordinates, and hang from a
*holder* that `PQSMod_LandClassScatterQuad.Setup` places under the terrain sphere, at
`localPosition = quad.positionPlanet`: the same 600 km vector in a float this page is about. The holder
is drawn with its local to world matrix, whose translation differs from its own transform position by
whole float steps, so the objects are drawn that much above or below the ground. On stock the two land on
the same step often enough that the holder is drawn exactly on its quad about a quarter of the time on
Kerbin, and within a tenth of a millimetre of it three times out of four on the Mun. With this mod they
never do: the ground is now placed in double precision and the holder is not.

[Rock Precision Fix Diag](https://github.com/lhervier/KSP-RockPrecisionFixDiag) measures it, on two
saves of its own, one on Kerbin and one on the Mun, each loaded twelve times per series ([the readings](https://github.com/lhervier/KSP-RockPrecisionFixDiag/blob/main/docs/what-the-readings-show.md#the-rocks)):
the height of a measured point of an object above the ground under it comes back 94 mm apart over the
twelve loads for half of those points on Kerbin in stock, and 130 mm with this mod; on the Mun, 31 mm
either way. The same kind of error, of the same order, but where stock draws part of it from the ground
moving and part from the holder, with this mod all of it comes from the holder. Stock scatter has no
collider, so this is visual only — unless a mod gives it one, which is the next section.

**Solution.** [Rock Precision Fix](https://github.com/lhervier/KSP-RockPrecisionFix), a separate mod,
hangs each holder from its own terrain quad, so that the objects are drawn in the frame they were built
in. Measured on those same series, with this mod installed next to it: every holder is drawn exactly on
its quad, and no measured point of an object moves by more than 0.125 mm over the twelve loads on Kerbin,
0.042 mm on the Mun
([its measurements](https://github.com/lhervier/KSP-RockPrecisionFix/blob/main/docs/checking-the-culprit.md#rock-precision-fix-diag-with-this-mod)).
It works with or without this mod, and it is not a mod to install lightly: it moves stock objects, which
other mods may look for where stock puts them, and its page weighs that trade
([Should you install it?](https://github.com/lhervier/KSP-RockPrecisionFix/blob/main/docs/should-you-install-it.md)).

## Scatter with colliders

**Limit, measured: this mod halves it, and does not close it.** Stock scatter has no collider, but a mod
can give it one — [Kopernicus](https://github.com/Kopernicus/Kopernicus) with the
[Stock Scatter Collider Enabler Patch](https://github.com/Poodmund/Stock-Scatter-Collider-Enabler-Patch),
both on CKAN, do. The offset above then stops being visual: the physics engine is handed the holder's
position, while the pilot sees what is drawn from the holder's matrix, and those are the two numbers that
round differently. The rock a craft hits is not the rock its pilot sees.

Rock Precision Fix Diag measures the gap between a collider and the object it belongs to, over six loads
of a kerbal standing on a boulder in a desert of Kerbin, in each configuration
([the readings](https://github.com/lhervier/KSP-RockPrecisionFix/blob/main/docs/checking-the-culprit.md#rock-precision-fix-diag-the-colliders)):
it runs from −68.7 to +104.2 mm on stock, and from −70.2 to +70.3 mm with this mod — halved, and drawn
afresh at every load. On the stock loads, the six pictures taken with the readings show it: the kerbal's
boots sink into the boulder at one load and stand clear of it at the next.

**Solution.** [Rock Precision Fix](https://github.com/lhervier/KSP-RockPrecisionFix) again, and only with
this mod as well: the same series reads −0.026 to +0.022 mm with both installed. The collider and the
object are the same object again.

**Still open: Breaking Ground's surface features.** `PQSMod_ROCScatterQuad.Setup` places them the same
way, and they carry a collider without any mod being needed. Whether the physics takes their pose from
the holder's matrix, as it does for the scatter colliders above, or from its transform position, is not
measured, and it decides whether this mod widens a physical offset there or leaves it alone. *Solution:*
measure it first; a fix of their own would then hang those holders from their quads the same way (see
[TODO.md](../TODO.md)).

## Kopernicus, on a stock body

**Measured: the fix still places the terrain, and the ground is as stable under
Kopernicus as without it.** Most planet packs go through
[Kopernicus](https://github.com/Kopernicus/Kopernicus), which rebuilds the terrain of every body it
touches, so the frame this fix computes in has to still be the frame the quads hang from. If it were
not, the 1 m safeguard would leave that terrain as stock builds it, with a warning per body in the log
and nothing else.

The series on scatter colliders above answers that for a stock body, because Rock Precision Fix Diag
also logs, at every load, the distance from the centre of Kerbin to the transform of each terrain quad
around the kerbal — the very number this fix places. On Kopernicus 1.12.1.247 with the collider patch,
over the six loads of each install, for the 173 quads present in all six of them
([the logs](https://github.com/lhervier/KSP-RockPrecisionFixDiag/tree/main/diag/runs), files
`collider-stock-load*.log` and `collider-tpf-load*.log`):

| install | spread of a quad's height over six loads, median | at worst |
|---|---|---|
| under Kopernicus, without this mod | 116.0 mm | 200.8 mm |
| under Kopernicus, with this mod | 0.079 mm | 0.265 mm |

The safeguard never fired, the log reported Kerbin's terrain placed in double precision, and what is
left is in the tenths of a millimetre — the same order as the campaigns without Kopernicus. On a stock
body, Kopernicus leaves the quads hanging in the frame this fix computes.

**Still open, on any Kopernicus install: the collision surface itself.** The readings above are the
position the quads are placed at — the cause — not the surface a craft rests on, which only
[Terrain Precision Fix Diag 2](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2) reads, and
which has never been read with Kopernicus installed. *Solution:* the campaign of
[Terrain Precision Fix Diag 2](checking-the-culprit.md#terrain-precision-fix-diag-2-with-this-mod-the-ground),
run on a Kopernicus install (see [TODO.md](../TODO.md)).

The body a planet pack adds is a case of its own, and the next chapter is about it.

## A body from a planet pack

**Open: nothing has been measured on one.** On a stock body Kopernicus rebuilds a terrain that already
exists; for a planet pack it does more. Each added body is built by cloning the terrain sphere of a
stock template, which is then reconfigured — another radius, another `maxLevel`, PQSMods added and
removed — and a pack can also rewrite the `PQS` of a stock body. The frame this fix computes in
(`body.rotation`, `body.position`) has to still be the one those quads hang from. If it is not, the 1 m
safeguard leaves that terrain as stock builds it: no fix, silently, apart from one warning per body in
the log.

*Solution:* the campaign of
[Terrain Precision Fix Diag 1](checking-the-culprit.md#terrain-precision-fix-diag-1-with-this-mod-the-craft),
run on such a body (see [TODO.md](../TODO.md)). A single landing already answers half of it: with this
mod installed, the log carries either `<body>: terrain placed in double precision` or the safeguard's
warning.

**Which body to run it on.** [Outer Planets Mod](https://github.com/Poodmund/Outer-Planets-Mod) covers
both cases, and its Kopernicus configs say what to expect before the game is even started:

| body | built from the template | radius | `maxLevel` |
|---|---|---|---|
| Slate | Moho | 540 000 m | 8 |
| Wal | Moho | 370 000 m | 8 |
| Thatmo | Moho | 286 000 m | 8 |
| Tekto | Laythe | 280 000 m | Laythe's |
| Ovok | Minmus | 26 000 m | 1 |
| Eeloo | the stock body, moved into orbit of Sarnus | 210 000 m (stock) | 8 |

For comparison, read in flight by
[PQS Bench](https://github.com/lhervier/KSP-PQSBench/blob/master/README.md#how-the-terrain-of-that-body-is-set-up):
Kerbin subdivides to level 10 and the Mun to 9 — so the other `maxLevel` of this chapter is really
exercised there.

**Slate is the body to measure on.** Its radius, 540 km, falls between the same two powers of two as
Kerbin's 600 km, so a float's step is the same 62.5 mm there: the defect has the amplitude of the
campaigns already run, and the readings compare directly. It has no atmosphere, so the landing of the
protocol is a landing and nothing more; the flat ground Diag 1 asks for is then a matter of picking the
spot. **Eeloo is the other case**, a stock body the pack reconfigures
rather than creates. **Ovok is the edge case**: at `maxLevel` 1 the quads of its highest level are
enormous, and whether this fix acts on them at all — it only moves the quads the game parents to
`LocalSpacePQStorage` — is one line of log to read.

**What such a campaign would not settle: the colliders' `maxLevelOffset`.** That pack never sets it;
every `maxLevelOffset` in its configs belongs to a scatter, not to `PQSMod_QuadMeshColliders`.
Kopernicus sets the colliders' offset to 0 only on a sphere it creates without a template
(`Configuration/PQSLoader.cs`); with a template, the stock value of that template comes along. A
campaign there therefore reads that offset rather than choosing it — PQS Bench prints it for the
body being flown over — and a pack that does set a non-zero one stays untested.

## Not checked yet

- **Parallax scatters.** According to its source, they should follow the corrected ground: their
  positions and colliders are expressed relative to the quad, and a collider is a child of its quad, so
  they move with it. *Solution:* confirm it in game (see [TODO.md](../TODO.md)).
- **Colliders below the highest subdivision level.** `PQSMod_QuadMeshColliders` gives a collider to
  every quad at or above `maxLevel - |maxLevelOffset|`, and with an offset other than 0 the fix would
  leave those lower quads uncorrected. The offset is **0** on Kerbin and on the Mun, so there the fix
  covers every quad a craft can stand on. *Solution:* read it on the other bodies (see
  [TODO.md](../TODO.md)).
- **The map view**, where quads are built and destroyed all the time. *Solution:* measure it.
- **Breaking Ground's deployed experiments.** They are vessels, positioned in double like any craft, so
  they should sit on the corrected ground like one. *Solution:* check it with Terrain Precision Fix Diag 1.
- **The KSC buildings, runway and launchpad.** Not covered: `PQSCity` and `PQSCity2` both do
  `base.transform.localPosition = planetRelativePosition;`, where `planetRelativePosition` is a
  `Vector3d` measured from the centre of the body. With Terrain Precision Fix Diag 1 on stock, a capsule
  parked on the runway spreads over 117 mm on six loads, as on the grass next to it; whether that comes
  from the runway itself or from the terrain underneath has not been separated yet. *Solution:* the same
  measurement with this mod installed tells the two apart — if the capsule on the runway still moves,
  it is the static (see [TODO.md](../TODO.md)).

