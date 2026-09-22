# The culprit

Part of [Terrain Precision Fix](../README.md): the stock code that places the ground, and why it places it somewhere else at every load.

Here it is straight away. The next chapter checks it before anything is changed.

This is how every terrain vertex is placed, in `PQS.BuildVertexSurfaceRelative`, decompiled from
KSP 1.12.5 (`vertRel` and `planetRel` are `Vector3d` fields):

```csharp
private void BuildVertexSurfaceRelative(VertexBuildData data)
{
    vertRel = vbData.directionFromCenter * vbData.vertHeight;
    planetRel = base.transform.TransformPoint(vertRel);
    verts[vertexIndex] = vertRel;
    buildQuad.verts[vertexIndex] = buildQuad.transform.InverseTransformPoint(planetRel);
}
```

`vertRel` is the vertex relative to the centre of the body, computed in double: a vector 600 km long
on Kerbin. `TransformPoint` turns it into a world position, which in KSP is close to the craft. To get
there, it adds that 600 km vector to the position of the centre of the body, another vector of about
600 km pointing the other way, and keeps the difference.

`InverseTransformPoint` then expresses that world position relative to `buildQuad.transform`, the
`Transform` of the quad the vertex belongs to. That one holds a 600 km vector of its own, built
exactly like `vertRel` — a direction from the centre of the body, times a height, this time the
height of the centre of the quad:

```csharp
// PQ.PreciseUpdateSubQuadsPosition; the same two values are computed in PQ.Subdivide
// when a quad creates its children, and assigned to the Transform in PQ.SetupQuad
positionPlanetRelative = positionPlanePosition.normalized;              // Vector3d
positionPlanet = positionPlanetRelative * sphereRoot.GetSurfaceHeight(positionPlanetRelative);
quadTransform.localPosition = positionPlanet;
```

The parent of that `Transform` is the sphere's quad storage, which hangs off the body without an
offset of its own. So this is a 600 km double stored in a float `localPosition`, under a parent that
sits at the centre of the body.

**That is the error.** A `Transform` works in `float`, and a float holding 600 km can only change in
steps of 62.5 mm. Every one of those values is rounded to the nearest step on the way, and the
roundings pile up in the few centimetres that are left once the two 600 km vectors cancel out. The
quad origin is rounded, and each vertex inside it is rounded on its own, so the mesh is not only moved
but also slightly bent.

## Why it is different at every load

A rounding only depends on the value being rounded, and `vertRel` and `positionPlanet` are exactly the
same doubles on every load of a save. What changes is the frame they are converted into: the world
matrix of the terrain sphere, itself held in float.

- Its **rotation** is the orientation of the body in the world frame, `CelestialBody.directRotAngle`.
  Stock splits the rotation of a body between that angle and `Planetarium.InverseRotAngle`
  (`CelestialBody.CBUpdate`), and saves neither. When a save is loaded, the jump of the clock back to
  the date of the save goes into `directRotAngle`, which comes back off by the rotation of the body over
  the time played since that save, or since the previous load
  ([measured with Terrain Precision Fix Diag 3](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag3#the-measurements)).
- Its **translation** is the position of the body relative to the floating origin, which moves every
  time the active craft travels 500 m.

At 600 km, turning the frame by a thousandth of a degree moves a point by more than 10 m, some 170
float steps: the slightest change draws a whole new set of roundings.

That the frame changes at every load is measured. That this change is what draws a new rounding is
read from the stock code and consistent with every measurement below; it has not been tested on its
own. It does not need to be:
the fix does not care whether a rounding is still drawn at every load, since it shrinks that rounding
to a size where drawing it again no longer matters.

That fix follows from the code above: do the subtraction in double first, and only give a float the
short distance that is left. How exactly, and why the other obvious way out was not taken, is in
[The fix this mod proposes](the-fix-this-mod-proposes.md). First, the hypothesis has to hold.

