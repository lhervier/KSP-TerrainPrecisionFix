# The terrain collision surface is rebuilt at a different height on every load

**Every time a save is loaded, KSP builds the collision mesh under your craft at a different height.** Load one save six
times: it puts the capsule back to the micrometre, yet where the capsule settles spreads 13.5 cm on
Kerbin. A small instrument is all it takes to read.

**How this was made.** The diagnosis and the patch were written with Claude, Anthropic's AI assistant.
I could not have produced them on my own, so I worked at it with the AI until the result was one I
understand myself, and nothing here was written down until a measurement agreed with it. I say so up
front, because a contribution made with an AI deserves a closer look than others — the instruments,
the quoted stock code and, in the first comment, the list of what I have and have not checked are
there for exactly that.

**Where I stand.** I write Java backends for a living, I am not a Unity developer, and this is the
first time I have gone this far into KSP's internals. I have been playing with the patch daily on a
save several years old, and kraken events on landed craft have all but disappeared — but whether that
makes it the right fix or a plausible-looking mistake is what I cannot tell on my own. That is what I
am asking you for.

## Repro

What it looks like:

![A craft jumping on its own the moment a save is reloaded](https://github.com/lhervier/KSP-TerrainPrecisionFix/raw/master/imgs/Booing-scaled.gif)

*Stock 1.12.5 with KSPCF as the only mod, `PartStartStability` included. A pod on an empty fuel tank, parked
in the grass at the KSC, saved, then reloaded from the pause menu, several times if needed.*

Here it is the ground coming back higher than it was saved, high enough to push the craft out — which
it does not do on every load.

To measure it, stock 1.12.5 with KSPCF and one more mod, and it is not the fix:
[Terrain Precision Fix Diag 1](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag) — a measuring
instrument, no Harmony, no patching, about 200 lines. It reads the distance from the craft to the
centre of the body twice: as the save hands it back, and once it has settled. Save a lone capsule on
flat bare ground, load that save six times, and record both readings each time (step by step, with
screenshots, in its README).

The first repeats to within three micrometres — KSP puts the craft back where it was. The second spreads
**134.5 mm on Kerbin**, 20.7 mm on the Mun, 6.7 mm on Minmus, 3.3 mm on Gilly
([every reading](https://github.com/lhervier/KSP-TerrainPrecisionFix/blob/master/docs/checking-the-culprit-loading.md)).

**And with no craft in the reading at all.** [Diag 2](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2),
just as inert, rays straight down and compares the collision surface it hits with the height
`pqsController` computes there. Same protocol: over six loads on Kerbin the surface moves 108.1 mm,
the computed height 0.039 mm
([every reading](https://github.com/lhervier/KSP-TerrainPrecisionFix/blob/master/docs/checking-the-culprit-loading.md)).
The two need not be equal — a triangle mesh is not the analytic surface — but only one of them changes
on every load.

**And with no load at all.** Leave the pod parked, drive a rover past 2.5 km until the game unloads
it, and come back: six round trips in one flight, and the surface under the pod comes back over
21.8 mm, the pod settling over 21.8 mm too
([every reading](https://github.com/lhervier/KSP-TerrainPrecisionFix/blob/master/docs/checking-the-culprit-approach.md)).
Loading the save while the pod sits 2 km from the craft you fly, then switching to it, shows when the
jump happens: the surface is already in place as the scene opens, the switch does not move it, and the
pod, held at its saved height until then, settles onto it
([every reading](https://github.com/lhervier/KSP-TerrainPrecisionFix/blob/master/docs/checking-the-culprit-switching.md)).

## What's going on: float rounding, and randomness

Every terrain vertex is placed in `PQS.BuildVertexSurfaceRelative` (`vertRel` and `planetRel` are
`Vector3d` fields):

```csharp
private void BuildVertexSurfaceRelative(VertexBuildData data)
{
    vertRel = vbData.directionFromCenter * vbData.vertHeight;
    planetRel = base.transform.TransformPoint(vertRel);
    verts[vertexIndex] = vertRel;
    buildQuad.verts[vertexIndex] = buildQuad.transform.InverseTransformPoint(planetRel);
}
```

`vertRel` is the vertex relative to the centre of the body, in double: 600 km on Kerbin. It goes
through two `Transform`s, and the quad's own holds a 600 km vector built the same way:

```csharp
// PQ.PreciseUpdateSubQuadsPosition; the same two values are computed in PQ.Subdivide
positionPlanetRelative = positionPlanePosition.normalized;              // Vector3d
positionPlanet = positionPlanetRelative * sphereRoot.GetSurfaceHeight(positionPlanetRelative);
quadTransform.localPosition = positionPlanet;
```

Its parent, the sphere's quad storage, sits at the centre of the body with no offset, so a 600 km
double is stored in a float `localPosition`. The quad origin is rounded, then each of its 225
vertices separately, so the mesh is displaced and slightly bent. What survives once the two 600 km
vectors cancel is a few centimetres of noise — one float step is 62.5 mm on Kerbin, 15.6 mm on the
Mun, 3.9 mm on Minmus and 1.95 mm on Gilly, and every spread above is one or two steps.

**On its own, that would be tolerable.** A rounding is deterministic: same numbers in, same answer
out, and a surface a few centimetres off but always in the same place is something nobody would
notice.

**But it is not the same answer twice.** The doubles going in *are* identical on every load —
`vertRel` and `positionPlanet` are recomputed from the same constants. What changes is the frame they
are rounded into: the world matrix of the terrain sphere, itself held in float.

- Its **rotation** is the body's orientation in the world frame, `CelestialBody.directRotAngle`.
  `CelestialBody.CBUpdate` splits the body's rotation between that angle and
  `Planetarium.InverseRotAngle`, and the save holds neither. On a load, the clock's jump back to the
  save goes into `directRotAngle`, which comes back off by the body's rotation over however long you
  played since that save, or since the previous load.
- Its **translation** is the body's position relative to the floating origin, which shifts every
  500 m the active craft travels — except while another landed craft is loaded
  (`Krakensbane.SafeToEngage`): the origin then waits, and catches up in one shift when that craft is
  unloaded.

Neither is saved, and neither stays put once loaded: the angle follows the clock whenever the craft is
above the inverse-rotation altitude (100 km on Kerbin), and the translation moves at every floating
origin shift. [Diag 3](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag3), inert like the other two, shows both, on a quickload, on a suborbital
hop, on a drive away from a parked craft and on a drive down the runway and back. The same quad, rebuilt from the same doubles, lands on a
different set of roundings every load — which is what the spreads above measure. The same reasoning, at more length:
[The culprit](https://github.com/lhervier/KSP-TerrainPrecisionFix/blob/master/docs/the-culprit.md).
