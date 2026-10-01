# The fix this mod proposes

Part of [Terrain Precision Fix](../README.md): what the patches do, for the ground and for the statics, where they do it, and what they leave alone.

## Two ways out, one taken

The culprit leaves two ways out: make the roundings come out the same on every load, or stop rounding
at planet scale.

**Freezing the frame** would mean giving the world frame the same orientation and the same position
relative to the body every time the ground is built. It was set aside, for reasons that
[Terrain Precision Fix Diag 3](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag3) measures:

- the frame moves with nothing loaded. Its orientation, `CelestialBody.directRotAngle`, follows the
  clock whenever the body is in the inertial frame — above 100 km on Kerbin — so a craft coming down
  from orbit finds it wherever its trajectory left it. Its position moves at every floating origin
  shift, so a craft driven away and back finds the terrain sphere elsewhere than where it left it
  ([cases 2 and 4](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag3/blob/master/docs/the-measurements.md)). According to the stock code, landed quads are placed again,
  the same way, at every such shift (`CelestialBody.PreciseUpdateQuadPositions`), so the ground would
  be rounded anew without any reload. Pinning the position would mean removing the floating origin,
  which is what lets KSP run in float at all;
- pinning the frame when a save is loaded would not be enough anyway. The game builds and subdivides
  quads under a craft as it approaches, not only when a save is loaded, and by then the frame is
  whatever the flight made of it; it cannot be reset in mid-flight without moving everything else that
  lives in it. The same quad would be rounded one way when built on approach, and another when built
  at load;
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
been read in flight and is 0 (see [Colliders below the highest subdivision level](limits-and-solutions/colliders-below-the-highest-subdivision-level.md)), and the only ones that can keep a precise
position: stock moves them to a container of their own, outside the body's hierarchy. Every other quad
hangs from the body's terrain sphere, whose origin is the centre of the body, so Unity would store any
position given to it as a 600 km float again. Those are left exactly as stock builds them.

The game only builds that level close to the ground: below 6 250 m over the Mun and 9 375 m over
Kerbin, read in flight from each sphere's own `subdivisionThresholds` and written to the log by
[PQS Bench](https://github.com/lhervier/KSP-PQSBench), once per sphere, in every
run of [Performance](performance.md). Higher up, the patched code decides once per quad that it does not
apply, and each vertex is left with a reference comparison before stock runs untouched.

## The statics

A static cannot be given a precise position while it hangs from its terrain sphere
([A second culprit: the statics](the-culprit.md#a-second-culprit-the-statics)). So this mod takes it out
of the sphere, and gives it its world position in double, in the same frame as the quads:
`body.rotation * planetRelativePosition + body.position`, and for its orientation, the rotation of the
body times the one stock gives it in the sphere.

- **Where it goes.** Under a container of its own, next to the one stock keeps for the quads a craft can
  stand on: outside the body's hierarchy, where a world position is kept as it is. Everything that hangs
  from the static — buildings, colliders, spawn points, the statics of a Kerbal Konstructs group — goes
  with it.
- **When.** Only in flight, and only while the static is within reach of the craft: within the farthest
  another craft can be loaded from it, 22.5 km by default, plus 5 km for the size of a static. A craft
  at the KSC takes the KSC out, and leaves the Island Airfield, 33 km away, under its sphere (logged in
  the session on Kerbin of [The KSC buildings, runway and launchpad](limits-and-solutions/the-ksc-buildings-runway-and-launchpad.md)). Only the
  statics that hang directly from their sphere are handled, as those of stock and of Kerbal Konstructs
  do; `PQSCity2` is not.
- **Following the body.** Stock moves the quads in the same call that moves the body, at every shift of
  the floating origin (the setter of `PQS.PrecisePosition`); a static out of its sphere is placed again
  there too, and whenever the body turns (`CelestialBody.CBUpdate`). If something else moves it — an
  editor, say — it stays where it was put, relative to the body.
- **Back under its sphere.** A static goes back exactly where stock left it, with its own local
  position and rotation, whenever it goes out of reach, before every scene change, and for the time
  stock code that expects it there runs: `PQSCity.Orientate`, `Start` and `ResetCelestialBody`, which
  write its local position or read its body from its parents; `PQS.SetupMods`, which lists the mods of
  a sphere from its children and would otherwise drop the static from the list; and `CommNetHome.Start`
  and `DayNightGameObjectSwitch.Setup`, which read the body of a ground station or of a light switch
  from their parents, and can be part of a static. Outside flight, a static is always where stock puts
  it.

## Other mods that look for a static under its sphere

Taking a static out of its sphere changes the hierarchy of Unity objects, and a mod may look for a
static where stock puts it. Two of the most installed ones do, in flight, once each, and this mod
patches both: the group editor of [Kerbal Konstructs](limits-and-solutions/kerbal-konstructs.md#the-patch-of-the-group-editor),
and the flag fix of [Kopernicus](limits-and-solutions/kopernicus/the-flag-fix.md) — which is not needed
where this mod moves the KSC: placed in double, the KSC keeps its flags steady, and the patch keeps the
flag fix from running there.
Each patch is described with its mod, along with the small change in that mod it stands for. Each leaves
the original code path untouched as long as the static is under its sphere, so it changes nothing
without this mod's statics fix; if the code of the mod is not the one the patch expects, the statics fix
stays off.

## Safeguards

- a correction larger than sixteen float steps is refused, quad by quad, and that quad is left as stock
  builds it: if the frame is ever not the expected one, the terrain stays where KSP puts it instead of
  going somewhere else. The step is taken at the distance of the quad from the centre of the body, so
  the limit is 1 m on Kerbin and grows with the body as the rounding does — 8 m on Earth in Real Solar
  System — while a wrong frame misses by kilometres. The largest correction measured so far is 4.0
  steps, 1 998 mm on Earth in Real Solar System (3.6 steps on Venus, 3.5 on the Moon, about one on
  Mars and Mercury), a quarter of the limit:
  [What this mod corrected](limits-and-solutions/rescaled-systems-real-solar-system.md#what-this-mod-corrected).
  The same limit applies to each static;
- a static is only taken out of its sphere if the container it goes to has the same scale as the
  sphere, so that the scale a mod gives it keeps its meaning;
- the ground and the statics are two fixes, each installed on its own, and each can be turned off in
  the settings. If any patch of one fix fails to install, none of that fix's patches does anything;
- if Kerbal Konstructs or Kopernicus is installed and its code is not the one its patch expects, the
  statics fix stays off, and every static stays where stock puts it. Each of these two patches can also
  be turned off in the settings, to see what goes wrong without it: the statics fix then stays on, and
  the log says which mod will break, and how.

