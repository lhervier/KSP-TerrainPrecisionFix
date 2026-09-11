# TODO

Everything else that sits on the ground and is placed with the same float rounding. The aim is for
this mod to cover all of it, not only the terrain.

## Rocks (terrain scatter) — measured, no fix planned

`PQSMod_LandClassScatterQuad.Setup` places the holder of a quad's rocks with
`base.transform.localPosition = quad.positionPlanet;`, under a parent attached to the terrain sphere,
whose origin is the centre of the body. The rocks themselves are built from the quad's vertices, in the
quad's own coordinates, and written into the holder's mesh as they are.

Measured with [Rock Offset Probe](https://github.com/lhervier/KSP-GroundFix-Mod2) on Kerbin (see the
README): in stock, rocks are already drawn off the ground, because the holder's local to world matrix
does not round like its transform position (−85.5 to +64.7 mm, standard deviation 29.7 mm). With this
fix, the stock error on the quad origin adds to it (−66.3 to +152.0 mm, standard deviation 44.4 mm).
Same kind of error, about 1.5 times wider, and visual only: stock rocks have no collider.

A fix would take the holder out of the sphere's hierarchy, under its quad for instance, which removes
both roundings at once. Only worth doing if the extra width turns out to be visible.

## Breaking Ground

- **Surface features (ROC)**, the ones studied in EVA or with the robotic arms: placed like the rocks,
  `PQSMod_ROCScatterQuad.Setup` does the same `localPosition = quad.positionPlanet`. Unlike the rocks,
  they have colliders. The rock measurement covers where they are drawn. Where the physics puts their
  colliders, whether with the transform position or with the matrix, is not measured, and decides
  whether this fix introduces a physical offset. To measure before deciding.
- **Deployed experiments** (`ModuleGroundPart` and related modules): they are vessels, positioned in
  double like any craft, so a priori they already benefit from the corrected ground. To check with the
  probe rather than assume. They are also the parts `Vessel.GoOffRails` skips the physics hold for.

## KSC buildings (`PQSCity`, `PQSCity2`)

Both do `base.transform.localPosition = planetRelativePosition;` with a `Vector3d` measured from the
centre of the body (`PQSCity` twice, `PQSCity2` three times). A capsule on the runway spreads over
117 mm on six loads, as much as on the grass next to it. The runway sits on top of terrain that
`PQSCity` flattens, so that measurement cannot yet tell the two apart. Differential test: with this
fix installed, the terrain becomes stable; if the capsule on the runway still moves, it is the static.

## Parallax scatters

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

## Colliders below the highest level

The fix only acts on quads of the highest subdivision level. `PQSMod_QuadMeshColliders` gives a
collider to every quad at or above `sphere.maxLevel - |maxLevelOffset|`: with an offset of 0, only the
highest level has colliders; with 2, the two levels below it have them too. Those quads hang from the
sphere and would stay uncorrected.

The actual value for stock bodies is not known. The only default visible in code is set in `Reset()`,
which Unity only calls in its editor; what the game uses is serialized in its assets. Every measurement
so far hit quads of the highest level, which fits an offset of 0 without proving it. To settle it: log
`maxLevelOffset` and the resulting lowest collider level for each body at startup, at the Debug level.

## Kopernicus compatibility — required

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
- Measure it, with the probe, on at least one stock body and one body from a planet pack.
