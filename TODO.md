# TODO

What is left to do, in three parts: the stock bugs still to test, what the fix still has to be checked
against, and the probes still to publish so that every figure in the README can be reproduced.

## Stock bugs still to test

The terrain defect itself, where the README still relies on the code alone; everything else that sits on
the ground and is placed with the same float rounding, which the aim is to cover too, in this mod or next
to it (see the rocks below); and one unrelated stock bug read in the code.

### What draws a new rounding — read in the code, not measured

The README's observation 4 and its section "Another lead, not followed" rest on this. According to the
stock code, two inputs of the rounding change without the terrain itself changing:

- **The orientation of the world frame.** Measured: `Planetarium.InverseRotAngle` differs on every load,
  while the body's `rotationAngle` does not. Not measured: that this is what draws a new rounding. To
  settle it, a throwaway test mod, never published, that sets `Planetarium.InverseRotAngle` to a fixed
  value before `FlightDriver.Start` places the bodies, on a stock install without the fix. With the same
  save, hence the same date, the orientation of the body in the world frame is then fixed too. Expected:
  **Settled** identical on every load; with the angle moved by 0.01°, a different value, identical on
  every load again. If **Settled** still spreads with the angle fixed, something else changes too, and
  observation 4 has to be rewritten.
- **The floating origin.** When the active craft gets further than `FloatingOrigin.threshold` (500 m in
  the code) from the world origin, and it is landed or under 100 m/s, `FloatingOrigin` calls
  `CelestialBody.PreciseUpdateQuadPositions`. Every quad of the highest level then goes through
  `PQ.PreciseUpdateSubQuadsPosition` again, which assigns `positionPlanet` to the float `localPosition`
  again, under a body whose world position has just changed. In stock, that would be a new rounding
  without any reload: a base could change height when a rover drives away from it and back. The fix
  covers it, since it patches that method. To measure with the quad origin probe (see below): a capsule
  and a rover side by side on Kerbin, the rover driven beyond 500 m and back. Expected: a jump at every
  shift in stock, none with the fix.

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

### Colliders below the highest level

The fix only acts on quads of the highest subdivision level. `PQSMod_QuadMeshColliders` gives a
collider to every quad at or above `sphere.maxLevel - |maxLevelOffset|`: with an offset of 0, only the
highest level has colliders; with 2, the two levels below it have them too. Those quads hang from the
sphere and would stay uncorrected.

The actual value for stock bodies is not known. The only default visible in code is set in `Reset()`,
which Unity only calls in its editor; what the game uses is serialized in its assets. Every measurement
so far hit quads of the highest level, which fits an offset of 0 without proving it. To settle it: log
`maxLevelOffset` and the resulting lowest collider level for each body at startup, at the Debug level.

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
  the fix installed are in [The ground itself](README.md#the-ground-itself).

The second one is the one that matters. `TerrainAltitude` is computed in double by stock code,
independently of the frame the fix uses, so it is the only reading that shows the ground comes back to
the **right** place and not merely to the **same** place.

Everything the README says about the mechanism — observations 2 to 4, and the quad origin line of
"What happens underneath" — comes from a development probe that was never released. It stays
unreleased (decided 2026-09-12). A third instrument would cost a public repository, a README bound by
the same rules as the other two, a third copy of `FormatUtils` to keep in step and a campaign on four
bodies, and it would add nothing to the case:

- **Observations 2 and 3 are already reproducible, without any instrument.** With `logLevel = Debug`,
  the fix logs for every quad it places how far it moved its origin, which is the stock error, as a
  distance rather than an altitude. With `Trace`, it logs how far the vertices moved within each quad.
- **The mechanism is shorter to read than to measure.** `quadTransform.localPosition = positionPlanet`
  in `PQ.SetupQuad`, a `Vector3d` of 600 km assigned to a float field, and the four lines of
  `PQS.BuildVertexSurfaceRelative`, are both quoted in
  [Where it happens](README.md#where-it-happens). Whoever doubts the figures can read the code
  that produces them.
- **A quad origin probe would be circular once the fix is installed.** The fix sets
  `transform.position = body.rotation * positionPlanet + body.position`, and a rotation preserves the
  length of a vector, so the altitude of the transform matches the altitude of `positionPlanet` by
  construction. The 0.00 mm such a probe reads says the fix does what it claims, and nothing more; only
  its stock reading carries information.
- **And what closes the case needs no probe at all**: the fix changes nothing but the order of the
  arithmetic, and the spread falls by three orders of magnitude. Were the cause elsewhere, reordering a
  subtraction would leave it untouched.

What this leaves without a published counterpart is the three lines of the
[What happens underneath](README.md#what-happens-underneath) table: development measurements, taken on
a spot no published campaign covers. The same demonstration with numbers read off screenshots on both
sides is the section below it, "The ground itself".

For the record, what the unreleased probe read, should it ever be wanted again. None of it needs
Harmony: the quad under the craft is the collider a raycast straight down hits, and everything below is
public.

| probe | what it read | README figures it backs | how to check it now |
|---|---|---|---|
| **Quad origin** | the altitude of `PQ.positionPlanet` and of the quad's transform, and the distance between that transform and `body.rotation * positionPlanet + body.position` | observation 2: 64.7851 m, and −87.8 to +145.7 mm in stock. "What happens underneath": 0.00 mm with the fix, on Kerbin and on the Mun | `logLevel = Debug`: one line per quad, giving the same error as a distance |
| **Mesh deformation** | the same raycast at three points 100 m apart on that quad, with the triangle each one hits, and the height differences between them | observation 3: the same triangles on every load, and differences that change by −7 to +53 mm | `logLevel = Trace`: the largest distance a vertex of the quad moved from where stock put it. The same claim — the vertices are rounded one by one — read from the other side, as a shift from stock rather than as a difference between two loads |
| **World frame angle** | the game time, the body's `rotationAngle` and `directRotAngle`, and `Planetarium.InverseRotAngle` | observation 4 | nothing, and nothing is needed: that the angle differs at every load is the measurement, and the README already gives what it concludes from it as a hypothesis read in the stock code, not as a result |

Not reproducible either, and about the fix rather than about stock: the two frames the README rejects
("Two frames look like more obvious choices", about 750 km and 36 mm). If they stay in the README, a
Trace line in the fix could log both for the first quad of each body.
