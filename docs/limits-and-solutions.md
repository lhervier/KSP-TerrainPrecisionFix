# Limits and solutions

Part of [Terrain Precision Fix](../README.md): the side effects of the fix, and the ground it has not been measured on.

This fix changes where the ground is, so everything that stands on the ground, or is placed from it, has
to be checked against it, one case at a time. **This is a work in progress.** Each chapter below is one
case, and opens on where it stands:

- **checked** — measured or read, and nothing to report;
- **affected** — the fix changes something there, and the chapter says what, and what solves it;
- **TBD** — not done yet: the chapter holds what is already known and what is planned to test it.

Every campaign run with this mod installed was run on KSP 1.12.5, with Harmony, ModuleManager and KSP
Community Fixes 1.41.1 — and, for the series on scatter colliders, Kopernicus and the patch that gives
them.

## KSP Community Fixes' own terrain patches

**Status: checked, in the source.** None of KSP Community Fixes' patches places a terrain quad or a
terrain vertex. The ones that touch the terrain — `PQSUpdateNoMemoryAlloc`, `PQSCoroutineLeak`,
`PQSOnlyStartOnce`, `ScatterDistribution` — patch how the terrain spheres start and update and how
scatter is distributed; none of them patches `PQS.BuildVertexSurfaceRelative`, `PQ.SetupQuad`,
`PQ.PreciseUpdateSubQuadsPosition` or `CelestialBody.PreciseUpdateQuadPositions`. `FloatingOriginPerf`
replaces `FloatingOrigin.setOffset`, but stock repositions the landed quads from the method that calls it,
not from inside it, so that path is left as stock has it.

Read in the repository as of release 1.40.1. *To do:* read it again on 1.41.1, the release every
campaign ran with.

## Rocks, grass and trees

**Status: affected.** This mod does not move terrain scatter — the rocks, and around the KSC the grass
and the trees — and the scatter no longer comes back on the ground it is drawn on.

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
collider, so this is visual only — unless a mod gives it one, which is the next chapter.

**Solution.** [Rock Precision Fix](https://github.com/lhervier/KSP-RockPrecisionFix), a separate mod,
hangs each holder from its own terrain quad, so that the objects are drawn in the frame they were built
in. Measured on those same series, with this mod installed next to it: every holder is drawn exactly on
its quad, and no measured point of an object moves by more than 0.125 mm over the twelve loads on Kerbin,
0.042 mm on the Mun
([its measurements](https://github.com/lhervier/KSP-RockPrecisionFix/blob/main/docs/checking-the-culprit.md#rock-precision-fix-diag-with-this-mod)).
It works with or without this mod, and it is not a mod to install lightly: it moves stock objects, which
other mods may look for where stock puts them, and its page weighs that trade
([Should you install it?](https://github.com/lhervier/KSP-RockPrecisionFix/blob/main/docs/should-you-install-it.md)).

**Parallax's own scatter is out of this.** Read in the source of Parallax Continued, not measured:
everything about its objects is expressed in the frame of the terrain quad — where they are drawn from,
the matrix they are drawn through, and the colliders it can give them, which are children of the quad —
so they follow the ground wherever this fix places it. Rock Precision Fix reads the same source for its
own purpose and writes up what it found there
([What never touches them](https://github.com/lhervier/KSP-RockPrecisionFix/blob/main/docs/should-you-install-it.md#what-never-touches-them)).
Parallax also leaves the stock `LandControl` in place on every body but Eeloo, so on a body it does not
strip, the stock scatter this chapter is about is still there, hanging from the same holders.

One detail this fix leaves exactly as stock has it: Parallax samples its distribution noise from
directions computed in float out of those same 600 km vectors, so an object sitting right at the cutoff
can appear on one load and not on the next. That happens without this fix too, and the fix touches
neither side of it.

## Scatter with colliders

**Status: affected — this mod halves the gap, and does not close it.** Stock scatter has no collider, but
a mod can give it one — [Kopernicus](https://github.com/Kopernicus/Kopernicus) with the
[Stock Scatter Collider Enabler Patch](https://github.com/Poodmund/Stock-Scatter-Collider-Enabler-Patch),
both on CKAN, do. The offset of the previous chapter then stops being visual: the physics engine is
handed the holder's position, while the pilot sees what is drawn from the holder's matrix, and those are
the two numbers that round differently. The rock a craft hits is not the rock its pilot sees.

Rock Precision Fix Diag measures the gap between a collider and the object it belongs to, over six loads
of a kerbal standing on a boulder in a desert of Kerbin, in each configuration
([the readings](https://github.com/lhervier/KSP-RockPrecisionFix/blob/main/docs/checking-the-culprit.md#rock-precision-fix-diag-the-colliders)):
it runs from −68.7 to +104.2 mm on stock, and from −70.2 to +70.3 mm with this mod — halved, and drawn
afresh at every load. On the stock loads, the six pictures taken with the readings show it: the kerbal's
boots sink into the boulder at one load and stand clear of it at the next.

**Solution.** [Rock Precision Fix](https://github.com/lhervier/KSP-RockPrecisionFix) again, and only with
this mod as well: the same series reads −0.026 to +0.022 mm with both installed. The collider and the
object are the same object again.

## The KSC buildings, runway and launchpad

**Status: affected — not covered by this fix.** `PQSCity` and `PQSCity2` both do
`base.transform.localPosition = planetRelativePosition;`, where `planetRelativePosition` is a `Vector3d`
measured from the centre of the body (`PQSCity` twice, `PQSCity2` three times). They carry the same
defect as the terrain, through the same kind of float `Transform`, and this fix does not touch them: a
static has no quad of its own to hang from.

With Terrain Precision Fix Diag 1 on stock, a capsule parked on the runway spreads over 117 mm on six
loads, as on the grass next to it. The runway sits on terrain that `PQSCity` flattens, so that reading
cannot tell the runway from the ground under it.

*To test:* the same measurement with this mod installed. The terrain is then stable, so if the capsule on
the runway still moves, it is the static.

## Ground anchors

**Status: TBD.** The ground anchor, the `ModuleGroundPart` part an engineer places in EVA construction, is
the part most exposed to the moment the ground is drawn, for two stock reasons read in the code: it is
the one thing that makes `Vessel.GoOffRails` skip the physics hold, so an anchored craft starts its
physics on the very first frame; and `groundAnchor.cfg` sets `kinematicDelay = 0`, so the anchor is
frozen (`PermanentGroundContact`, `FreezeAll`) after a single frame, at whatever height it is at that
instant.

It also has causes of its own, which this fix does not touch and does not claim:

- a part dropped in EVA construction is saved with `PQSMin`/`PQSMax` at `0/0` (written literally in
  `EVAConstructionModeEditor`), and `Vessel.GoOffRails` only skips the repositioning pass when those
  levels are non-zero and match the current `pqsController`, so the pass runs on every load;
- `Vessel.CheckGroundCollision` puts the lowest point of the craft's **colliders** on the terrain, and
  the anchor's collider stops above the part origin while its model reaches it, so the pass lifts it;
- its 10 cm dead zone is disabled when the root part carries a `ModuleGroundPart`, so even a millimetric
  correction is applied.

A stable ground should make the anchor's behaviour repeatable, not fix it. That is the subject of
KSP Community Fixes' issue [#214](https://github.com/KSPModdingLibs/KSPCommunityFixes/issues/214).

*To test,* on KSP + Harmony + ModuleManager + KSP Community Fixes + Terrain Precision Fix Diag 1, with and
without this fix: an anchor placed in EVA construction, **saved after each load** — the re-save is what
lets a lift accumulate. To read: the `Moving Vessel` lines of each load, the `PQSMin`/`PQSMax` of the
anchor in the `.sfs`, and the gap between the anchor's collider and its origin in `groundAnchor.mu`.

## Kopernicus, on a stock body

**Status: checked — the fix still places the terrain, and the ground is as stable under Kopernicus as
without it.** Most planet packs go through [Kopernicus](https://github.com/Kopernicus/Kopernicus), which
rebuilds the terrain of every body it touches, so the frame this fix computes in has to still be the
frame the quads hang from. If it were not, the 1 m safeguard would leave that terrain as stock builds it,
with a warning per body in the log and nothing else.

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

**Still TBD: the collision surface itself.** The readings above are the position the quads are placed
at — the cause — not the surface a craft rests on, which only
[Terrain Precision Fix Diag 2](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2) reads, and which
has never been read with Kopernicus installed. *To test:* the campaigns of
[Checking the culprit](checking-the-culprit.md) — Diag 1 and Diag 2, six loads on Kerbin, with and
without this fix — run on a Kopernicus install with nothing else, so that the result stands on its own
protocol rather than on a campaign about scatter.

Two things read in Kopernicus' source, and not expected to interact with this fix:
`DisableFarAwayColliders` (`RuntimeUtility/SinkingBugFix.cs`) disables every collider of a body whose
centre is more than 10,000 km from the world origin, which never includes the terrain under the craft;
and Kopernicus replaces the stock scatter holder with its own subclass,
`PQSMod_KopernicusLandClassScatterQuad`, which is the case measured under
[Scatter with colliders](#scatter-with-colliders).

## A body from a planet pack

**Status: TBD — nothing has been measured on one.** On a stock body Kopernicus rebuilds a terrain that
already exists; for a planet pack it does more. Each added body is built by cloning the terrain sphere of
a stock template, which is then reconfigured — another radius, another `maxLevel`, PQSMods added and
removed — and a pack can also rewrite the `PQS` of a stock body. The frame this fix computes in
(`body.rotation`, `body.position`) has to still be the one those quads hang from. If it is not, the 1 m
safeguard leaves that terrain as stock builds it: no fix, silently, apart from one warning per body in
the log.

*To test:* the campaign of
[Terrain Precision Fix Diag 1](checking-the-culprit.md#terrain-precision-fix-diag-1-with-this-mod-the-craft),
run on such a body. A
single landing already answers half of it: with this mod installed, the log carries either
`<body>: terrain placed in double precision` or the safeguard's warning.

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
spot. **Eeloo is the other case**, a stock body the pack reconfigures rather than creates. **Ovok is the
edge case**: at `maxLevel` 1 the quads of its highest level are enormous, and whether this fix acts on
them at all — it only moves the quads the game parents to `LocalSpacePQStorage` — is one line of log to
read.

What such a campaign would not settle, the colliders' `maxLevelOffset`, is in
[its own chapter](#colliders-below-the-highest-subdivision-level).

## Rescaled systems: Real Solar System

**Status: TBD — nothing has been measured on one, and the 1 m safeguard may switch the fix off there.**
The defect grows with the radius of the body. A float's step doubles every time a distance crosses a
power of two, so the rounding this fix removes is not the same size on a rescaled body:

| body | radius | float step at that distance |
|---|---|---|
| Kerbin | 600 000 m | 62.5 mm |
| Earth, in [Real Solar System](https://github.com/KSP-RO/RealSolarSystem) | 6 371 000 m | 500 mm |

That is arithmetic, not a measurement. On Kerbin, the spreads measured in
[Checking the culprit](checking-the-culprit.md) are one to three steps; at the same number of steps,
Earth's terrain would move by half a metre to a metre and a half from one load to the next.

The fix itself should hold there: what it hands a float is a distance within a quad, whatever the size of
the body. The safeguard is what may not. It refuses any correction larger than **1 m**, a fixed value
chosen against a rounding of a few centimetres, and on Earth a correction of one to three steps is
0.5 to 1.5 m. Some quads would then be corrected and others left as stock builds them, each refusal
reported once per body in the log.

*To test:* a landing on Earth in Real Solar System, with this mod installed, and the log read for
`Earth: terrain placed in double precision` or the safeguard's warning; then the campaign of
[Terrain Precision Fix Diag 1](checking-the-culprit.md#terrain-precision-fix-diag-1-with-this-mod-the-craft)
on flat ground, with and without this fix.

**Solution, if the safeguard does fire:** a limit proportional to the float step at the body's radius,
rather than a fixed metre, so that it keeps the same meaning — a correction no rounding can produce — on
any body.

## Breaking Ground's surface features

**Status: TBD.** The surface features studied in EVA or with the robotic arms are placed like the rocks:
`PQSMod_ROCScatterQuad.Setup` does the same `localPosition = quad.positionPlanet`, and their holder hangs
from a `rocParent` that `LandClassROC` creates as a child of the terrain sphere. Unlike stock rocks, they
carry a collider without any mod being needed.

Whether the physics takes their pose from the holder's matrix, as it does for the scatter colliders
above, or from its transform position, is not measured, and it decides whether this fix widens a
physical offset there or leaves it alone. The identifier of a surface feature depends on its position
within the quad (`rocPOS`, taken from `quad.verts`), not on the pose of its holder, so moving the holder
would not change it.

*To test:* first read where their collider sits relative to what is drawn, the way Rock Precision Fix
Diag does for scatter colliders; then measure the gap over several loads, with and without this fix. If
there is a gap, a fix of their own would hang those holders from their quads, the way Rock Precision Fix
does for scatter.

## Breaking Ground's deployed experiments

**Status: TBD.** Deployed experiments (`ModuleGroundPart` and the modules around it) are vessels,
positioned in double like any craft, so they should sit on the corrected ground like one. They are also
the parts `Vessel.GoOffRails` skips the physics hold for, like the ground anchor.

*To test:* Terrain Precision Fix Diag 1 on a deployed experiment, six loads, with and without this fix.

## Kerbal Konstructs

**Status: TBD — read in the source, not measured.** [Kerbal Konstructs](https://github.com/KSP-RO/Kerbal-Konstructs)
plants statics — pads, runways, whole bases — anywhere on a body, and flattens the ground to seat them.
A fix that quietly undid those edits would be exactly the kind of side effect a player finds before
anyone else.

Read in the source, and reassuring: Kerbal Konstructs writes no terrain code of its own.
`Core/MapDecals/MapDecalInstance.cs` adds a stock `PQSMod_MapDecal` to the body's `pqsController` and
calls its stock `OnSetup()`: its whole terrain editing is the stock `PQSMod_MapDecal` /
`PQSMod_MapDecalTangent`, the same components stock uses to flatten the ground around the KSC. A decal
edits `vbData.vertHeight` in `OnVertexBuildHeight`, which runs **before** `PQS.BuildVertexSurfaceRelative`
consumes it, and this fix consumes exactly the same `vbData.directionFromCenter * vbData.vertHeight`: the
height it places is the already-flattened one. Parallax is in the same position: its only decal-related
`PQSMod`, `PQSMod_MapDecalVertexRemoveScatter`, removes scatter inside a decal and does not touch height.

Two questions stay open. Its statics may be placed like the KSC's, from the centre of the body through a
float `Transform`, in which case they carry the same defect and this fix does not cover them. And the 1 m
safeguard would not catch a decal going wrong, since the quads would still be within a metre of where
they belong.

*To test:* a Kerbal Konstructs site, with and without this fix, Terrain Precision Fix Diag 2 reading the
ground inside the flattened area and just outside it, and a craft parked on one of its pads.

## Sloped ground

**Status: TBD.** Every campaign so far is on flat ground, because Terrain Precision Fix Diag 1 asks for
it. On a slope, a separate stock bug, read in the code and not measured, puts a single-part craft down
into the ground at every load. This fix does not touch it, and a reading taken there would show it.

On unpacking, `Vessel.CheckGroundCollision` puts the craft back onto the ground. It runs on every load
for a craft made of a single part (`Vessel.GoOffRails`), and in a few other cases. It compares `D`, the
distance from the root down to the ground along the **vertical**, with `L`, the distance from the root
to the lowest point of the craft along the **ground normal**, and moves the craft by `L' − D`:

```csharp
float num5 = Mathf.Cos(Mathf.Abs((float)Vector3d.Angle(groundCollisionHit.normal, vector3d2)) * ((float)Math.PI / 180f)) * num4;
if (Mathf.Abs(num4 - num5) > 0.1f)
    num4 = num5;
```

`num4` is `L`, and `vector3d2` the vertical. For a craft resting on a flat slope of angle `α`, the root
is `L / cos α` above the ground measured along the vertical, which is what `D` finds, so the move should
be zero. The code multiplies by `cos α` where it should divide, and only above 10 cm of difference.
Either way `L' < D`, so the craft is always moved **down**:

| branch | taken when | move |
|---|---|---|
| `L` kept | `L (1 − cos α) ≤ 0.1 m` | `L (1/cos α − 1)` |
| `L cos α` | `L (1 − cos α) > 0.1 m` | `L sin²α / cos α` |

For `L = 1 m`: 15 mm at 10°, 64 mm at 20°, 103 mm at 25°, 289 mm at 30°. Moves under 10 cm are not
applied, except when the root carries a `ModuleGroundPart`.

*To test:* the campaign already run on flat ground, run again on a slope of 30° or more — the same
install, the same lone capsule, the same six loads, without and with this fix. It is the one case where
the two defects can be told apart: the fix makes the ground stop moving and leaves the slope move as it
is. Expected, if the reading is right: a `ground contact! - error. Moving Vessel down` line at every
load, whose value changes from load to load without the fix, and comes back the same with it. The same
craft with a second part, which skips the pass, is the counter-test.

## Existing saves

**Status: TBD.** The fix takes away the draw, and the draw was also an escape hatch: in stock, a base
that comes back buried and tears itself apart can be reloaded until it survives. With the fix, it breaks
the same way every time. What a player with a long-running save should expect:

- **the window is one loading per landed craft.** The craft was saved on the ground of one particular
  draw, and comes back on the corrected ground; once it has been loaded and saved again with the fix
  installed, both sides agree and the question never comes back;
- **the corrected ground is among the stock draws, not a worse one** — the readings are in
  [Terrain Precision Fix Diag 2, with this mod](checking-the-culprit.md#terrain-precision-fix-diag-2-with-this-mod-the-ground);
- **the failure becomes repairable**: raising a craft by a few centimetres in the `.sfs` is a permanent
  repair once the ground is stable, where in stock the next loading draws the ground under it again.

*To test:* on a copy of a long-running save, how far each landed base actually moves at its first
loading with the fix — a range in millimetres over real bases, to say how much transition there is to
absorb. The configuration expected to suffer most is modules docked to each other and standing on
landing legs.

## Asteroids held by a claw

**Status: TBD.** An asteroid is a vessel, positioned in double like any other, so on its own it should
follow the corrected ground; neither `ModuleAsteroid` nor `ModuleGrappleNode` reads the terrain. The
fragile case is an asteroid resting on the ground and grappled by a claw: on the first load with the fix,
the ground under it moves once, by up to the stock spread, with a mass on the other end of a joint.

*To test:* a clawed asteroid on the Mun, saved and reloaded several times, with and without the fix.

## KAS

**Status: TBD.** Kerbal Attachment System attaches parts to each other and to the ground with joints of
its own. A static attachment either rides on a vessel, and follows the corrected ground for free, or is
pinned to the terrain sphere, and does not.

*To test:* read how KAS anchors a statically attached part, and only then decide whether there is
anything to measure.

## The ground during a flight

**Status: TBD.** Loading is the moment every campaign measures, because it is the one that can be
repeated at will. The same draw should happen without any reload: the world matrix of the terrain
sphere moves at every floating origin shift, and stock places the landed quads again at each of them
(`CelestialBody.PreciseUpdateQuadPositions`). This fix patches that path too, so with it the ground
should stay put.

*To test:* a capsule and a rover side by side on Kerbin, Terrain Precision Fix Diag 2 reading the ground
under the capsule, the rover driven beyond 500 m and back, with and without the fix. Expected: the
reading jumps at every origin shift in stock, and not with the fix.

## The map view

**Status: TBD.** In the map view, quads are built and destroyed all the time, and nothing has been
measured there.

*To test:* whether the fix's per-quad work and its safeguard behave as in flight while the map view
builds and drops quads — the log, and the terrain on returning to flight.

## Deferred

**Status: TBD.** Deferred replaces KSP's rendering path, and has
no reason to care where a quad is placed. It is here because it is the mod that broke KSP Community
Fixes' own `PQSOnlyStartOnce`: terrain stopped loading for some players, and that patch has been
disabled by default since. Anything touching the terrain spheres deserves the same check.

*To test:* Deferred plus this fix, the campaign of Terrain Precision Fix Diag 2, and whether the terrain
still renders.

## Colliders below the highest subdivision level

**Status: TBD on every body but Kerbin and the Mun.** `PQSMod_QuadMeshColliders` gives a collider to
every quad at or above `maxLevel - |maxLevelOffset|`, and this fix only acts on the highest level: with
an offset other than 0, the levels below it would have colliders and stay uncorrected. The offset is
**0** on Kerbin and on the Mun, read in flight, so there the fix covers every quad a craft can stand on.

Kopernicus sets the colliders' offset to 0 only on a sphere it creates without a template
(`Configuration/PQSLoader.cs`); with a template, the stock value of that template comes along. The
`maxLevelOffset` a Kopernicus config can set belongs to the scatter
(`Configuration/ModLoader/LandControl.cs`), not to the colliders, and Outer Planets Mod never sets it.

*To test:* read it on the other bodies — [PQS Bench](https://github.com/lhervier/KSP-PQSBench) prints it
for the body being flown over — and a pack that sets a non-zero one stays to be found.
