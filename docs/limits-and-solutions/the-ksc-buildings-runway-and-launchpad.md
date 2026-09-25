# The KSC buildings, runway and launchpad

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: affected — not covered by this fix.** `PQSCity` and `PQSCity2` both do
`base.transform.localPosition = planetRelativePosition;`, where `planetRelativePosition` is a `Vector3d`
measured from the centre of the body (`PQSCity` twice, `PQSCity2` three times). They carry the same
defect as the terrain, through the same kind of float `Transform`, and this fix does not touch them: a
static has no quad of its own to hang from.

With Terrain Precision Fix Diag 1 on stock, a capsule parked on the runway spreads over 117 mm on six
loads, as on the grass next to it. The runway sits on terrain that `PQSCity` flattens, so that reading
cannot tell the runway from the ground under it.

*To test:* the same measurement with this mod installed. The terrain is then stable, so if the capsule on
the runway still moves, it is the static.
