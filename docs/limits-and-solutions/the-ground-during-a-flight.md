# The ground during a flight

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: checked, on the craft and on the ground.** A craft does not only meet the
ground when a save hands it back. Drive away from a landed craft until the game unloads it, past
2500 m, then come back within 200 m: its physics starts again on ground that was built while you were
away, and no save was loaded at any point. The world matrix of the terrain sphere moves at every
floating origin shift, and stock places the landed quads again at each of them
(`CelestialBody.PreciseUpdateQuadPositions`). This fix patches that path too.

Six round trips in a row, in a single flight on Kerbin: without this fix the craft comes to rest 7.5
to 19.2 mm from the height it was handed back at, upwards as often as downwards, over a spread of
21.8 mm; with it, over 0.094 mm. The readings are in
[The craft, over six round trips](../checking-the-culprit-approach.md#the-craft-over-six-round-trips).

Terrain Precision Fix Diag 2, which reads the ground itself rather than the craft resting on it,
ran the same protocol: without this fix the ground comes back somewhere else on every round trip, over
a spread of 21.8 mm, and has already moved by the time the craft is back in range; with it, over
0.011 mm
([The ground, over six round trips](../checking-the-culprit-approach.md#the-ground-over-six-round-trips)).
