# TODO

What is left to do, in three parts: the stock bugs still to test, what the fix still has to be checked
against, and the probes still to publish so that every figure in the README can be reproduced.

## Before opening the KSPCF issue

Not everything below has to be done first. What does, in the order a reviewer will ask for it:

1. **Kopernicus.** Most planet packs go through it; the fix is not worth proposing until it is measured
   with it, on a stock body and on a planet pack body.
2. **Existing saves.** Already tested, not yet written: the fix has been run on the author's own
   years-old save, and none of its bases broke. What is left is to say it in the README together with
   what it does not prove — those bases are built in a way that resists this defect (see below) — and
   to say plainly that the fix also takes away the reload lottery players use as an escape hatch.
   The KSPCF campaigns are done, checked and linked from all three READMEs.
3. **Breaking Ground surface features.** They have colliders and are placed like the rocks. Enough to
   know whether the fix introduces a physical offset there, even if the answer is "yes, and Rock
   Precision Fix handles it".
4. ~~**The README reorganised**~~ — done 2026-09-16: thesis, why it matters, the culprit (faulty code
   and why the draw differs), checking it with both probes (stock, then with the fix), the fix and the
   way out not taken, performance, limits and solutions.

Everything else — Parallax measured rather than read, `PQSCity`, rocks measured, the ground moving in
flight, the `cos α` bug — can be listed as open in the README without holding the issue back.

## Stock bugs still to test

The terrain defect itself, where the README still relies on the code alone; everything else that sits on
the ground and is placed with the same float rounding, which the aim is to cover too, in this mod or next
to it (see the rocks below); and one unrelated stock bug read in the code.

### What draws a new rounding — settled, no test mod needed

The quad's `localPosition` is not what varies: the same `Vector3d` always rounds to the same `float`.
What reaches the collider is `M × localPosition`, where `M` is the world matrix of the terrain sphere,
held in float. So what draws a new number is `M` — its rotation and its translation — and both move by
design:

- **rotation**, the orientation of the world frame. Measured: `Planetarium.InverseRotAngle` differs on
  every load (237.17 / 236.74 / 239.36 / 236.67 / 241.77 on five launches of KSP), while the body's
  `rotationAngle` is identical on every load of the same save. It advances with the body's rotation
  whenever the game runs in the rotating frame, which includes sitting at the space centre, so it
  carries the time spent playing between two loads.
- **translation**, the world position of the body, which changes every time the floating origin shifts
  — every 500 m the active craft travels (`FloatingOrigin.threshold`). Quads built while a rover
  approaches a base, and quads rebuilt by `CelestialBody.PreciseUpdateQuadPositions` after a shift
  (`FloatingOrigin.cs:423`, when the craft is landed or under 100 m/s), are rounded against a matrix
  that has moved since the scene opened.

That is enough to explain the draw, whether or not anything else feeds it, and it closes the other way
out, now "Two ways out, one taken" in the README, without a measurement: pinning the angle cannot make stock terrain
reproducible, because the translation keeps moving, and pinning the translation means removing the
floating origin, which is what lets KSP run in float at all. The planned throwaway probe that would
have pinned `InverseRotAngle` is dropped (decided 2026-09-13), and so is the third public instrument it
could have become.

Still worth measuring, for itself rather than for that argument: whether stock ground visibly moves
**during a single flight**, which would widen the defect beyond loading. A capsule and a rover side by
side on Kerbin, Terrain Precision Fix Diag 2 reading the ground under the capsule, the rover driven
beyond 500 m and back. Expected: *Difference* jumps at every origin shift in stock, and does not with
the fix, which patches that path too.

### Rocks (terrain scatter) — handled by Rock Precision Fix, not measured yet

`PQSMod_LandClassScatterQuad.Setup` places the holder of a quad's rocks with
`base.transform.localPosition = quad.positionPlanet;`, under a parent attached to the terrain sphere,
whose origin is the centre of the body. The rocks themselves are built from the quad's vertices, in the
quad's own coordinates, and written into the holder's mesh as they are.

Measured with [Rock Precision Fix Diag](https://github.com/lhervier/KSP-RockPrecisionFixDiag) on Kerbin
(see the README): in stock, rocks are already drawn off the ground, because the holder's local to world
matrix does not round like its transform position (−85.5 to +64.7 mm, standard deviation 29.7 mm). With
this fix, the stock error on the quad origin adds to it (−66.3 to +152.0 mm, standard deviation
44.4 mm). Same kind of error, about 1.5 times wider, and visual only: stock rocks have no collider.

Handled by a separate mod, [Rock Precision Fix](https://github.com/lhervier/KSP-RockPrecisionFix): it
hangs each holder from its own quad, so the holder's position no longer goes through a 600 km float,
which removes both roundings at once. It works with or without this mod. Written, not measured yet.

### Breaking Ground

- **Surface features (ROC)**, the ones studied in EVA or with the robotic arms: placed like the rocks,
  `PQSMod_ROCScatterQuad.Setup` does the same `localPosition = quad.positionPlanet`. Unlike the rocks,
  they have colliders. The rock measurement covers where they are drawn. Where the physics puts their
  colliders, whether with the transform position or with the matrix, is not measured, and decides
  whether this fix introduces a physical offset. To measure before deciding. Rock Precision Fix could
  later handle them the same way as the rocks: `PQSMod_ROCScatterQuad` has the same kind of holder,
  which could hang from its quad too, for where they are drawn and for their colliders alike.
- **Deployed experiments** (`ModuleGroundPart` and related modules): they are vessels, positioned in
  double like any craft, so a priori they already benefit from the corrected ground. To check with
  Terrain Precision Fix Diag rather than assume. They are also the parts `Vessel.GoOffRails` skips the physics hold for.

### KSC buildings (`PQSCity`, `PQSCity2`)

Both do `base.transform.localPosition = planetRelativePosition;` with a `Vector3d` measured from the
centre of the body (`PQSCity` twice, `PQSCity2` three times). A capsule on the runway spreads over
117 mm on six loads, as much as on the grass next to it. The runway sits on top of terrain that
`PQSCity` flattens, so that measurement cannot yet tell the two apart. Differential test: with this
fix installed, the terrain becomes stable; if the capsule on the runway still moves, it is the static.

### Craft pushed into slopes on load — a separate stock bug, not measured

Read in the code, not measured. Not a float rounding issue, and not something this fix touches. It is
the reason Terrain Precision Fix Diag asks for flat ground.

On unpacking, `Vessel.CheckGroundCollision` puts the craft back onto the ground. It runs on every load
for a craft made of a single part (`Vessel.GoOffRails`), and in a few other cases. It compares two
distances:

- `D`, from the root down to the ground, along the **vertical**: a raycast straight down through the
  root;
- `L`, from the root to the lowest point of the craft, along the **ground normal**
  (`getLowestPoint` turns the craft so that the normal of the hit points along `z`, and takes the lowest
  `z` of its colliders).

The move applied is `L' − D`, where `L'` comes from:

```csharp
float num5 = Mathf.Cos(Mathf.Abs((float)Vector3d.Angle(groundCollisionHit.normal, vector3d2)) * ((float)Math.PI / 180f)) * num4;
if (Mathf.Abs(num4 - num5) > 0.1f)
    num4 = num5;
```

`num4` is `L`, and `vector3d2` the vertical. For a craft resting on a flat slope of angle `α`, the root
is `L` from the plane measured along the normal, so `L / cos α` measured along the vertical: that is
what `D` finds, and the move should be zero. The code multiplies by `cos α` where it should divide, and
only does so above 10 cm of difference; below, it keeps `L` as it is. Either way `L' < D`, so the craft
is always moved **down**, by:

| branch | taken when | move |
|---|---|---|
| `L` kept | `L (1 − cos α) ≤ 0.1 m` | `L (1/cos α − 1)` |
| `L cos α` | `L (1 − cos α) > 0.1 m` | `L sin²α / cos α` |

For `L = 1 m`: 15 mm at 10°, 64 mm at 20°, 103 mm at 25°, 289 mm at 30°. Moves under 10 cm are not
applied, except when the root carries a `ModuleGroundPart`. Zero on flat ground, growing with the height
of the craft and quickly with the slope.

Dividing, always, would give zero on a flat slope. The residual would then only come from the ground
not being a plane between the root and the lowest point.

To measure: a single-part craft on a slope of 30° or more, saved and loaded three times. Expected, if
the reading is right: a `ground contact! - error. Moving Vessel down` line, the same value on every load,
matching the second formula with `α` taken from the normal under the craft. Counter-test: the same
craft with a second part, which skips the pass, and the line should disappear. On uneven ground, a
`down` line can also come from `Vessel.Load` raising the craft to the analytic terrain height first;
check that before reading it as this bug.

## What the fix still has to be checked against

### Existing saves — the one risk a player can feel

The fix takes away the draw, and the draw was also an escape hatch. In stock, a base that comes back
buried and tears itself apart can be reloaded: the ground is drawn somewhere else, and it may survive.
With the fix, the ground comes back to the same place every time, so a base that breaks on loading
breaks on every loading.

What is true, and has to be in the README rather than discovered by a player:

- **The window is one loading per landed craft.** The craft was saved sitting on the ground of one
  particular draw; it comes back on the corrected ground, which differs from that draw by up to the
  stock spread. Once it has been loaded and saved again with the fix installed, both sides agree and
  the question never comes back.
- **The corrected ground is not a worse draw than average, it is among them.** Measured in
  [Terrain Precision Fix Diag 2, with this mod](README.md#terrain-precision-fix-diag-2-with-this-mod-the-ground):
  the fixed reading falls inside the six draws without the fix on Kerbin, Minmus and Gilly (near the
  lower edge on Gilly), and just below them on the Mun, where it is inside the twelve draws of both
  stock campaigns on that spot. So a given base is no more likely to come back buried than it
  was on any given stock loading — what changes is that the outcome no longer differs at each try.
- **The failure becomes repairable, which it was not in stock.** Raising a craft by a few centimetres
  in the `.sfs` is a permanent repair once the ground is stable. In stock the same edit fixes nothing:
  the next loading redraws the ground under it anyway.
- **KSPCF patches are individually switchable**, so a player whose years-old base does not survive the
  transition can turn this one off, load, raise the craft, and turn it back on. To confirm against the
  KSPCF settings mechanism before putting it in the README.

**Tried, on a real save (2026-09-13).** Install: KSP 1.12.5, Harmony, KSPCommunityFixes — the target
itself — and this fix. Nothing else. The save is years old and carries large permanent bases; several
*other* vessels were dropped by KSP on loading for missing modded parts (MechJeb, ScanSat), none of
them a base. Screenshots in `imgs/bases`.

Loaded one by one, the bases on the Mun, Minmus and Gilly are stable. **None broke, on any body.**
The base on Eve settles: some of its feet end up in the ground, both on loading and on leaving time
warp. It does not break.

Eve is not this fix, and the mechanism is known. Time warp repacks the vessel and its joints are
recreated without the elastic deformation they had: the base visibly returns to the shape it was
**built** in, and in that shape some of its feet are below the surface. Once settled they are not,
because Unity has pushed them out. Nothing to do with where the ground is — the ground does not move
at an unpack, since no save is reloaded. Same family as the Disclaimer’s “craft bent to fit the
ground”, seen at a warp exit instead of a loading.

What it does say is that the base has a construction defect: parts were placed below the surface while
building it in EVA. Fixing that belongs to the EVA construction tool, not here.

Also from this run:
- **Does KSPCF already patch the PQS?** The campaign ran with KSPCF installed, which is the right
  install to test against, but any patch of theirs touching terrain is an interaction to document
  before proposing this one.
- **How these bases were built.** They were assembled in EVA construction with a personal tool
  (KSP-EvaCMGroundPlugin), which is where this defect was found in the first place. They load and work
  without it — the campaign above did not have it installed — so the vessels tested are stock vessels.
  Decided 2026-09-13: say that in one sentence under `How this was made`, and do **not** open the
  second bug that tool addresses. One issue, one bug; the question can be answered if it is asked.
- **One screenshot in the README, not four.** They prove nothing on their own — the defect is not
  visible in them — their only job is to show the scale of what was loaded.

That save is a sample of one player, whose bases are built in a way that resists this defect more than
most:

- an anchor (a Clamp-o-Tron placed by an engineer on the ground), then "rails" of girders built on
  that anchor in EVA construction, **following the curvature of the terrain** rather than imposing a
  flat shape on it;
- base modules brought by a crane and docked onto those rails, then struts added by an engineer.

Rails that follow the ground mean the craft is not fighting the terrain's shape to begin with, and
struts mean the assembly barely flexes. What that save does *not* sample is the configuration that
suffers most, and which the author reports as having caused the worst incidents in stock: modules
docked to each other and standing on **landing legs**, where the legs come back extended and the
docking joints are soft. The fix removes one of the causes there, not the others.

Still worth doing, and cheap, because it turns "nothing broke" into a bound: on a copy of that
save — never the original — read how far each landed base actually moved at the first loading with
the fix, rather than only whether it survived. A range in millimetres over real bases says how much
transition there is to absorb, where "none of mine broke" says only that this player's bases absorb it.

Also to decide, and it costs more than it looks: the README's `Disclaimer` is part of the block copied
**word for word** into the two Diag READMEs. Adding docked assemblies and landing legs to it means
editing all three. Either do that, or put it in the correcting mod's own closing paragraph, which is
the part of the block that is allowed to differ.

Open, and only worth doing if that bound shows a real transition cost: a migration helper that
walks a `.sfs` and raises landed vessels onto the corrected ground. It would be a separate tool, not
part of the fix, and it is not a prerequisite for the issue.

### Measured on stock, replayed under KSPCF — done and linked (2026-09-13)

The patch is proposed to KSPCF, so every campaign has been run a second time in an install that has
KSPCF (1.41.1), and the screenshots are published: `imgs/kspcf` in each Diag repository (without the
fix), `imgs/Diag1` and `imgs/Diag2` here (with it).

**This repository changed on 2026-09-16**: its tables now come from the KSPCF campaigns, with the fix
against without it in that same install, and the earlier campaigns with the fix and without KSPCF were
removed (screenshots and tables; they are in the git history). Its "on stock" sections still recap the
bare-KSP campaigns of the two Diags. What follows is about the Diag repositories.

The analysis of the Diags stays on the stock figures, deliberately. The campaigns exist to show the defect is in
bare KSP: measured with forty patches installed, they invite the one answer the issue must not get,
"how do you know it is the game and not one of ours?". The KSPCF run is there to show the defect does
not disappear, or change nature, in the install the patch is aimed at.

So the READMEs **link** to those screenshots and do not tabulate them. No value from the KSPCF run is
carried into a table, and no comparison is drawn figure by figure: the campaigns were shot on their
own spots, so only the spread is comparable, and the spread is of the same order on every world
(checked image by image, 2026-09-13, twelve campaigns without the fix). Keeping them as a link also
keeps the READMEs short.

Verified separately, and worth one sentence next to the claim: no KSPCF patch touches the placement of
PQS quads. Record the version or commit checked.

### Cost of the vertex patch — measured, 2026-09-13

Measured, published in [Performance](README.md#performance), material and logs in [perfs/](perfs/):
**260.0 ns per vertex in stock against 87.8 ns with the fix**, replaying the placements over the
vertices of a quad the game had just built, in the frame that built it. Two flights of the same save,
one of them without the mod installed at all, differ by less than the noise between two KSP sessions.

A third formula, stock with the two `Transform`s hoisted out of the loop, reads **174.8 ns** and splits
the saving in half: **85.2 ns** is reading `base.transform` and `buildQuad.transform` on every vertex,
**87.0 ns** is the arithmetic. Even against a stock that stopped asking Unity for a `Transform` per
vertex, the fix would still be twice as fast.

The expectation recorded here — "the cost should be small, and it may well be negative" — turned out to
be true only after the patch was rewritten. As first written it read **500.4 ns**: it redid for each of
the 225 vertices of a quad what only depends on the quad, and reading a handful of Unity transforms over
and over costs far more than the arithmetic the fix exists for. Worked out once per quad, a vertex is
left with a `Vector3d` subtraction and two rotations. Both figures are in the README: the first one is
what justifies the second being written that way.

What is left, and it is not a prerequisite: the same measurement on Kerbin rather than the Mun, where
quads are four times larger and the craft can be made to fly low for much longer.

**Settled by the same campaign**: an earlier one read 167.3 ns for stock, because its replay kept
`sphere.transform` and `quad.transform` in locals across the 225 vertices of a quad — which
`PQS.BuildVertexSurfaceRelative`, called once per vertex, never does. Timed as its own formula, that
hoisted stock reads 174.8 ns, within a few nanoseconds of what the old method reported in three separate
sessions (167.3, 168.6, 172.2). The old campaign was measuring a hoisted stock and calling it stock; its
figures are out of the README, and its logs out of `perfs/runs/` — they are in the git history. The one
kept is the run that carries the 500.4 ns, still quoted and no longer reproducible.

### Colliders below the highest level

The fix only acts on quads of the highest subdivision level. `PQSMod_QuadMeshColliders` gives a
collider to every quad at or above `sphere.maxLevel - |maxLevelOffset|`: with an offset of 0, only the
highest level has colliders; with 2, the two levels below it have them too. Those quads hang from the
sphere and would stay uncorrected.

**Settled, 2026-09-13.** The value is logged by the mod itself while measuring performance, and read in
flight: `maxLevelOffset` is **0** on Kerbin and on the Mun, so the lowest level with a collider is the
highest level itself — 10 and 9 respectively. **The fix covers every quad a craft can stand on.**

Still worth reading on the other bodies as they are visited, since nothing says a body cannot ship a
different offset. A single line in the log gives it, and the two bodies measured so far agree with the
`Reset()` default.

### Parallax scatters

Read in the Parallax Continued source (tag 1.0.4), not measured. Unlike the rocks, the scatters should
follow the corrected quad, because everything about them is expressed in the quad's own frame:

- positions are drawn inside the triangles of `quad.mesh.vertices`, and kept as quad-local positions;
- they are drawn every frame through the quad's `meshRenderer.localToWorldMatrix`;
- their colliders are child GameObjects of the quad, with a short `localPosition`, and only exist on
  quads of the highest level, the ones this fix corrects;
- Parallax reads the mesh when the quad becomes visible, in `PQ.SetVisible` after `PQ.Build`, so after
  both patches have run;
- the terrain shader's replacement mesh is a child of the quad too, with no offset.

To confirm in game, with Parallax installed: the offset between a scatter collider and its quad should
be the same on every load.

One thing the fix leaves as it is: Parallax samples its distribution noise with directions from the
centre of the body computed in float, from 600 km vectors, and computes the altitude the same way. An
object right at the noise cutoff can therefore appear on one load and not on the next. That happens in
stock too, and has nothing to do with the terrain's placement.

### Kopernicus compatibility — required

Most planet packs go through Kopernicus, so the fix is not worth proposing until it works with it. To
check:

- Kopernicus rebuilds the terrain of every body it touches. The frame the fix computes positions in
  (`body.rotation`, `body.position`) has to still be the one the quads hang from. The 1 m safeguard
  would catch a mismatch, but the result would be no fix, silently, apart from a warning in the log.
- `maxLevelOffset`: the one Kopernicus lets a config set belongs to the scatter
  (`Configuration/ModLoader/LandControl.cs`), not to the colliders. On the terrains it creates,
  Kopernicus sets the colliders' `maxLevelOffset` to 0 (`Configuration/PQSLoader.cs`). On the bodies it
  only modifies, it keeps the stock value, which is still unknown (see above).
- `DisableFarAwayColliders` (`RuntimeUtility/SinkingBugFix.cs`) disables every collider of a body whose
  centre is more than 10,000 km from the world origin, to work around a PhysX raycast bug. That never
  includes the terrain under the craft, so it should not interact with the fix.
- Kopernicus replaces the stock scatter holder with its own subclass,
  `PQSMod_KopernicusLandClassScatterQuad`, and can give scatter objects colliders (`scatterColliders`).
  Same open question as the ROCs above.
- Measure it, with Terrain Precision Fix Diag, on at least one stock body and one body from a planet
  pack.

## Where the mechanism figures come from — nothing to publish

Not a task: a record of which figures of the README rest on a published instrument, which do not, and
how to check the second kind without one.

Two instruments are published, and between them they carry the whole demonstration:

- [Terrain Precision Fix Diag](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag) reads the
  craft — **On rails** and **Settled** — and shows that it does not come back to the same height;
- [Terrain Precision Fix Diag 2](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2), written on
  2026-09-12, reads the ground under it: the collision surface found by a raycast straight down against
  `CelestialBody.TerrainAltitude` at the same point, one line per loading, frozen by a *Record* button
  like the first one. It is a mod of its own rather than a reading folded into that instrument because
  it measures the ground and not the craft, so it needs none of that protocol: no settling, no waiting.
  Four stock campaigns, on Kerbin, the Mun, Minmus and Gilly, are on its page; the same four saves with
  the fix installed are in [Terrain Precision Fix Diag 2, with this mod](README.md#terrain-precision-fix-diag-2-with-this-mod-the-ground).

The second one is the one that matters. `TerrainAltitude` is computed in double by stock code,
independently of the frame the fix uses, so it is the only reading that shows the ground comes back to
the **right** place and not merely to the **same** place.

The figures that came from a development probe that was never released — observations 2 to 4 of the old
"Why it happens" section, and the "What happens underneath" table — **left the README on 2026-09-16**,
when it was reorganised: no figure without a protocol a reader can follow. The probe stays unreleased
(decided 2026-09-12), and nothing is lost by it:

- **The mechanism is shorter to read than to measure.** `quadTransform.localPosition = positionPlanet`
  in `PQ.SetupQuad`, and the four lines of `PQS.BuildVertexSurfaceRelative`, are quoted in
  [The culprit](README.md#the-culprit), which says in words, without measured values, that the origin
  and each vertex are rounded on their own.
- **Diag 2 carries the proof on the ground**, with numbers read off screenshots on both sides.
- **The fix closes the case without any probe**: it changes where a subtraction happens, and the spread
  falls by three orders of magnitude.
- **Observations 2 and 3 can still be reproduced** if a reader asks: with `logLevel = Debug` the fix
  logs how far it moved each quad origin, which is the stock error as a distance; with `Trace`, how far
  the vertices moved within each quad.

The two rejected frames lost their figures on the same day (about 750 km for `PQS.GetWorldPosition`,
36 mm for the float rotation on a 600 km vector): the README gives the reason in words only. They are
still quoted in a code comment of `WorldPosition`; a Trace line could log both for the first quad of
each body, should they ever be wanted back.

For the record, what the unreleased probe read, should it ever be wanted again. None of it needs
Harmony: the quad under the craft is the collider a raycast straight down hits, and everything below is
public.

| probe | what it read | README figures it backs | how to check it now |
|---|---|---|---|
| **Quad origin** | the altitude of `PQ.positionPlanet` and of the quad's transform, and the distance between that transform and `body.rotation * positionPlanet + body.position` | former observation 2: 64.7851 m, and −87.8 to +145.7 mm in stock; former "What happens underneath": 0.00 mm with the fix, on Kerbin and on the Mun | `logLevel = Debug`: one line per quad, giving the same error as a distance |
| **Mesh deformation** | the same raycast at three points 100 m apart on that quad, with the triangle each one hits, and the height differences between them | former observation 3: the same triangles on every load, and differences that change by −7 to +53 mm | `logLevel = Trace`: the largest distance a vertex of the quad moved from where stock put it. The same claim — the vertices are rounded one by one — read from the other side, as a shift from stock rather than as a difference between two loads |
| **World frame angle** | the game time, the body's `rotationAngle` and `directRotAngle`, and `Planetarium.InverseRotAngle` | former observation 4 | nothing, and nothing is needed: the README now gives the draw as a hypothesis read in the stock code, without the measured angles |

