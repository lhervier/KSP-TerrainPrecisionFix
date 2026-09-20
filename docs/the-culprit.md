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
600 km pointing the other way, and keeps the difference. `InverseTransformPoint` then expresses that
point relative to the quad, whose own position was obtained the same way:

```csharp
// PQ.SetupQuad and PQ.PreciseUpdateSubQuadsPosition — positionPlanet is a Vector3d
quadTransform.localPosition = positionPlanet;
```

A 600 km double stored in a float `localPosition`, under a parent that sits at the centre of the body.

**That is the error.** A `Transform` works in `float`, and a float holding 600 km can only change in
steps of 62.5 mm. Every one of those values is rounded to the nearest step on the way, and the
roundings pile up in the few centimetres that are left once the two 600 km vectors cancel out. The
quad origin is rounded, and each vertex inside it is rounded on its own, so the mesh is not only moved
but also slightly bent.

## Why it is different at every load

A rounding only depends on the value being rounded, and `vertRel` and `positionPlanet` are exactly the
same doubles on every load of a save. What changes is the frame they are converted into: the world
matrix of the terrain sphere, itself held in float.

- Its **rotation** follows the orientation of KSP's world frame, `Planetarium.InverseRotAngle`. According
  to the stock code (`CelestialBody.CBUpdate`), that angle advances with the rotation of the body while
  the game runs in the rotating frame, which includes sitting at the space centre, so it carries the
  time played between two loads.
- Its **translation** is the position of the body relative to the floating origin, which moves every
  time the active craft travels 500 m.

At 600 km, turning the frame by a thousandth of a degree moves a point by more than 10 m, some 170
float steps: the slightest change draws a whole new set of roundings.

That this is what draws a new rounding at every load is a hypothesis, read from the stock code and
consistent with every measurement below; it has not been tested on its own. It does not need to be:
the fix does not care whether a rounding is still drawn at every load, since it shrinks that rounding
to a size where drawing it again no longer matters.

That fix follows from the code above: do the subtraction in double first, and only give a float the
short distance that is left. How exactly, and why the other obvious way out was not taken, is in
[The fix this mod proposes](the-fix-this-mod-proposes.md). First, the hypothesis has to hold.

