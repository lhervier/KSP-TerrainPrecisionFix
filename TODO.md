# TODO

Everything else that sits on the ground and is placed with the same float rounding. The aim is for
this mod to cover all of it, not only the terrain.

## Rocks (terrain scatter)

`PQSMod_LandClassScatterQuad.Setup` places the container of a quad's rocks with
`base.transform.localPosition = quad.positionPlanet;`, under a parent attached to the terrain sphere,
whose origin is the centre of the body. The rocks themselves are placed inside that container from the
quad's vertices.

In stock, the container and the quad get the same rounding, so rocks stay on the ground. With this fix
the quad is exact and the container is not: rocks should be offset from the ground by the stock error
of the quad origin, up to 15 cm on Kerbin. To measure first, then fix. The container would have to be
taken out of the sphere's hierarchy, as stock does for the quads of the highest level. Otherwise a
precise position given to it is lost again.

## Breaking Ground

- **Surface features (ROC)**, the ones studied in EVA or with the robotic arms: same mechanism,
  `PQSMod_ROCScatterQuad.Setup` does the same `localPosition = quad.positionPlanet`. They have
  colliders, so an offset here is a physical one, not only a visual one.
- **Deployed experiments** (`ModuleGroundPart` and related modules): they are vessels, positioned in
  double like any craft, so a priori they already benefit from the corrected ground. To check with the
  probe rather than assume. They are also the parts `Vessel.GoOffRails` skips the physics hold for.

## KSC buildings (`PQSCity`, `PQSCity2`)

Both do `base.transform.localPosition = planetRelativePosition;` with a `Vector3d` measured from the
centre of the body (`PQSCity` twice, `PQSCity2` three times). A capsule on the runway spreads over
117 mm on six loads, as much as on the grass next to it. The runway sits on top of terrain that
`PQSCity` flattens, so that measurement cannot yet tell the two apart. Differential test: with this
fix installed, the terrain becomes stable; if the capsule on the runway still moves, it is the static.

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
- Kopernicus exposes `maxLevelOffset` (its DLL has a `MaxLevelOffset` property next to
  `PQSMod_QuadMeshColliders`; its source is not at hand), so a planet pack can give colliders to lower
  levels, see above.
- Its DLL also mentions `DisableFarAwayColliders`: find out what it does to terrain colliders.
- Measure it, with the probe, on at least one stock body and one body from a planet pack.
