# The fix this mod proposes

Part of [Terrain Precision Fix](../README.md): what the two patches do, where they do it, and what they leave alone.

## Two ways out, one taken

The culprit leaves two ways out: make the roundings come out the same on every load, or stop rounding
at planet scale.

**Freezing the frame** would mean giving the world frame the same orientation and the same position
relative to the body on every load. It was set aside, on a reading of the stock code rather than on a
measurement:

- the matrix that draws the rounding has a rotation **and** a translation. Pinning
  `Planetarium.InverseRotAngle` would only pin part of the rotation — the orientation of the body in the
  world frame also depends on the date of the save — and the translation moves at every floating origin
  shift. According to the stock code, landed quads are placed again, the same way, at every such shift
  (`CelestialBody.PreciseUpdateQuadPositions`), so the ground would be rounded anew without any reload.
  Pinning the translation too would mean removing the floating origin, which is what lets KSP run in
  float at all;
- and a rounding that repeats is still a rounding: the ground would come back to the same place, but
  that place would still be off the height the game computes by as much as a few centimetres, as the
  stock readings above show.

**Doing the arithmetic in double** removes the error instead of freezing it, at the place where the
precision is actually lost. That is what this mod does.

## Doing the arithmetic in double

The two stock placements that go through a float at planet scale are redone in double:

- **The origin of each quad.** After `PQ.SetupQuad` and `PQ.PreciseUpdateSubQuadsPosition`, the quad
  is moved to `body.rotation * positionPlanet + body.position`, computed with `QuaternionD` and
  `Vector3d`. The result is a world position, relative to the floating origin: near the craft it is a
  short vector, which a float holds precisely.
- **Each vertex, inside its quad.** `PQS.BuildVertexSurfaceRelative` is replaced by the same
  computation with the subtraction done first: the vertex minus the quad origin, both doubles relative
  to the centre of the body, then rotated into the quad's frame.

A double holding 600 km changes in steps of about a tenth of a nanometre, so nothing is lost while the
long vectors are handled. What finally reaches a float is a distance within the quad, a couple of
kilometres at most, where a float step is about a tenth of a millimetre instead of 62.5 mm. That float
is still there, and its rounding may well still differ from one load to the next — but at that scale.
It is the order of what the tables above still show with the fix.

This is not a new way of placing the terrain. It is the stock placement, evaluated in an order that
does not throw away its own significant digits.

Two frames look like more obvious choices, and both are wrong. `PQS.GetWorldPosition` does not give a
position in the frame the quads hang in. The rotation of the body's transform is the same rotation as
`body.rotation`, but held in float: applied to a 600 km vector, it brings back the very rounding being
removed.

## Once per quad, not once per vertex

`PQS.BuildVertexSurfaceRelative` runs for every vertex of every quad the game builds, 225 per quad, and
the game builds quads all the time while flying low. Stock already asks Unity for two `Transform`s on
every one of those vertices, and calls into them twice.

The replacement needs more than that, and all of it depends on the quad rather than on the vertex:
whether the fix applies to it at all, the frame it hangs in, the inverse of its rotation. Worked out
again for every vertex, that would cost more than the placement it is part of. So it is worked out
once, on the first vertex of a quad, and kept for the others; it is forgotten whenever `PQS.BuildQuad`
starts, since a quad can be rebuilt after having moved. What is left per vertex is a `Vector3d`
subtraction, a rotation in double, a rotation in float and two writes.

What that saves, and how much of it comes from the double-precision arithmetic rather than from the
work done once per quad, is measured in [Performance](performance.md).

## Only where a craft can stand

Only the quads of the highest subdivision level are corrected. Those are the ones craft stand on, the
only ones with a collider on Kerbin and on the Mun, where `PQSMod_QuadMeshColliders.maxLevelOffset` has
been read in flight and is 0 (see [TODO.md](../TODO.md)), and the only ones that can keep a precise
position: stock moves them to a container of their own, outside the body's hierarchy. Every other quad
hangs from the body's terrain sphere, whose origin is the centre of the body, so Unity would store any
position given to it as a 600 km float again. Those are left exactly as stock builds them.

The game only builds that level close to the ground: below 6 250 m over the Mun and 9 375 m over
Kerbin, read in flight from each sphere's own `subdivisionThresholds` and written to the log by
[PQS Bench](https://github.com/lhervier/KSP-PQSBench), once per sphere, in every
run of [Performance](performance.md). Higher up, the patched code decides once per quad that it does not
apply, and each vertex is left with a reference comparison before stock runs untouched.

## Safeguards

- a correction larger than 1 m is refused, quad by quad, and that quad is left as stock builds it: if
  the frame is ever not the expected one, the terrain stays where KSP puts it instead of going
  somewhere else;
- if any patch fails to install, none of them does anything.

