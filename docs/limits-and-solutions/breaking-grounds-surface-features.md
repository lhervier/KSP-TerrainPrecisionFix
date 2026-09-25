# Breaking Ground's surface features

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: TBD.** The surface features studied in EVA or with the robotic arms are placed like the rocks:
`PQSMod_ROCScatterQuad.Setup` does the same `localPosition = quad.positionPlanet`, and their holder hangs
from a `rocParent` that `LandClassROC` creates as a child of the terrain sphere. Unlike stock rocks, they
carry a collider without any mod being needed.

Whether the physics takes their pose from the holder's matrix, as it does for
[scatter colliders](scatter-with-colliders.md), or from its transform position, is not measured, and it decides whether this fix widens a
physical offset there or leaves it alone. The identifier of a surface feature depends on its position
within the quad (`rocPOS`, taken from `quad.verts`), not on the pose of its holder, so moving the holder
would not change it.

*To test:* first read where their collider sits relative to what is drawn, the way Rock Precision Fix
Diag does for scatter colliders; then measure the gap over several loads, with and without this fix. If
there is a gap, a fix of their own would hang those holders from their quads, the way Rock Precision Fix
does for scatter.
