# The culprit: the statics

Part of [Terrain Precision Fix](../README.md): the stock code that places the statics standing on the ground, and why it places them somewhere else at every load.

Here it is straight away.

The runway, the launchpad and the buildings of the KSC are not terrain. They are statics, placed by a
`PQSCity`, and so are the pads, runways and whole bases that a mod such as
[Kerbal Konstructs](https://github.com/KSP-RO/Kerbal-Konstructs) plants anywhere on a body. Their
placement repeats the terrain's, in `PQSCity.Orientate` (KSP 1.12.5; `planetRelativePosition` is a
`Vector3d`):

```csharp
planetRelativePosition = vector3d * (sphere.radius + repositionRadiusOffset);
base.transform.localPosition = planetRelativePosition;
```

The `Transform` of a static hangs straight from the body's terrain sphere, whose origin is the centre of
the body. So this is again a 600 km double stored in a float `localPosition`, with the same 62.5 mm
step on Kerbin, converted through the same frame as the ground, which moves as described in
[Why it is different at every load](the-culprit-ground.md#why-it-is-different-at-every-load).
`PQSCity2`, which places the launch sites of the Making History expansion, does the same.

One thing sets a static apart: it has nowhere else to go. Stock moves the quads a craft can stand on
into a container outside the body's hierarchy, where a world position given to them is kept as it is.
It gives statics no such place: a precise position given to a static under its sphere would be stored
as a 600 km float again.

That a static comes back somewhere else at every load, and not together with the ground around it, is
checked on the runway of the KSC in
[Checking the culprit: the runway and the grass beside it](checking-the-culprit-runway.md).
