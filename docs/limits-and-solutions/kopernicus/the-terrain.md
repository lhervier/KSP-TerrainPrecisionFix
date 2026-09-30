# Kopernicus: the terrain

Part of [Terrain Precision Fix](../../../README.md), one point of the case [Kopernicus](../kopernicus.md), in [Limits and solutions](../../limits-and-solutions.md).

**Status: checked, no problem — the ground is as stable under Kopernicus as without it.** Kopernicus
rebuilds the terrain of every body it touches, so the frame this fix computes in has to still be the
frame the quads hang from. If it were not, the safeguard would leave that terrain as stock builds it,
with a warning per body in the log and nothing else.

## Where the quads are placed

Whether the quads still hang in the frame this fix computes: the series of
[Scatter with colliders](scatter-with-colliders.md) answers that for a stock body, because Rock
Precision Fix Diag also logs, at every load, the distance from the centre of Kerbin to the transform of
each terrain quad around the kerbal — the very number this fix places. On Kopernicus 1.12.1.247 with the
collider patch, over the six loads of each install, for the 173 quads present in all six of them
([the logs](https://github.com/lhervier/KSP-RockPrecisionFixDiag/tree/main/diag/runs), files
`collider-stock-load*.log` and `collider-tpf-load*.log`):

| install | spread of a quad's height over six loads, median | at worst |
|---|---|---|
| under Kopernicus, without this mod | 116.0 mm | 200.8 mm |
| under Kopernicus, with this mod | 0.079 mm | 0.265 mm |

The safeguard never fired, the log reported Kerbin's terrain placed in double precision, and what is
left is in the tenths of a millimetre — the same order as the campaigns without Kopernicus. On a stock
body, Kopernicus leaves the quads hanging in the frame this fix computes.

## The collision surface itself

The readings above are the position the quads are placed at — the cause — not the surface a craft rests
on, which only [Terrain Precision Fix Diag 2](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2)
reads. That surface was read on Earth in [Real Solar System](../rescaled-systems-real-solar-system.md),
a stock body rebuilt by Kopernicus: Real Solar System's Earth is Kerbin, reconfigured by Kopernicus 248
to another radius and another terrain. The protocol is the one of
[Checking the culprit: loading the same save](../../checking-the-culprit-loading.md#the-ground-over-six-loads):
one save, loaded six times, with and without this fix. Under the craft, the collision surface spreads
over 693.1 mm without this mod and 0.331 mm with it, and the craft, loaded again and again with this
mod, never moved.

## Read in the source

`DisableFarAwayColliders` (`RuntimeUtility/SinkingBugFix.cs`) disables every collider of a body whose
centre is more than 10,000 km from the world origin, which never includes the terrain under the craft:
not expected to interact with this fix.
