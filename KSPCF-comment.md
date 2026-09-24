<!-- first comment, to post right after the issue itself -->

## How the mod works

**Why not freeze the frame.** Making the roundings repeat is not an option: the frame moves on its own
in flight — a suborbital hop turns it, driving shifts it every 500 m, or all at once when you leave a
parked craft behind — and landed quads are placed again at every shift
(`CelestialBody.PreciseUpdateQuadPositions`). It cannot be reset mid-flight without moving everything
that lives in it. So the mod changes the arithmetic, not the frame.

**Two Harmony patches**, redoing in double the two placements that go through a float at planet scale:

- **the origin of each quad**, after `PQ.SetupQuad` and `PQ.PreciseUpdateSubQuadsPosition`:
  `body.rotation * positionPlanet + body.position`, in `QuaternionD` and `Vector3d`;
- **each vertex inside its quad**, subtraction first: vertex minus quad origin, both doubles relative to
  the centre of the body, then rotated into the quad's frame.

The two 600 km vectors cancel while still in double; what reaches a float is a distance within the
quad, where a step is a tenth of a millimetre. It is the stock placement evaluated in an order that
keeps its significant digits. It does not make the surface *better*: it lands among the values stock
scatters around, and stops drawing a new one every time.

**Scope and safeguards.** Only the highest subdivision level is touched: the quads craft stand on, the
only ones with a collider, and the only ones stock moves out of the body's hierarchy, so they can hold
a precise position at all. Everything else is built as stock builds it. A correction larger than 1 m
is refused quad by quad, and logged, so an unexpected frame leaves the terrain where KSP puts it. If
any patch fails to install, none of them does anything.

**Cost.** Measured with [KSPProfiler](https://github.com/KSPModdingLibs/KSPProfiler) on whole frames,
and with [PQS Bench](https://github.com/lhervier/KSP-PQSBench) on the vertex placement alone: slightly
faster than stock, lost in the noise over a whole frame
([performance](https://github.com/lhervier/KSP-TerrainPrecisionFix/blob/master/docs/performance.md)).

**What I already know it does not cover.** The KSC buildings (`PQSCity`) and the rocks are placed
from the centre of the body too, and keep their own rounding; the rocks even end up slightly further
from the corrected ground on Kerbin. Both are on
[Limits and solutions](https://github.com/lhervier/KSP-TerrainPrecisionFix/blob/master/docs/limits-and-solutions.md),
with everything else still to check.

**Two related issues.** [#214](https://github.com/KSPModdingLibs/KSPCommunityFixes/issues/214), the
ground anchor that climbs a little on every load, becomes readable once the ground holds still; I will
comment there once I have something solid. And
[#435](https://github.com/KSPModdingLibs/KSPCommunityFixes/pull/435) works on the same frame: putting
it back in place at a scene change would not be enough for the ground, which also moves in flight with
nothing loaded.
