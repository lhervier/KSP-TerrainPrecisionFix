<!-- first comment, to post right after the issue itself -->

## The fix

Freezing the frame so that the roundings at least repeat is not an option: the frame moves on its own
in flight — a suborbital hop turns it; driving shifts it every 500 m, or all at once when you leave a
parked craft behind ([Diag 3](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag3#the-measurements)) —
and according to the stock code, landed quads are placed again at every such shift
(`CelestialBody.PreciseUpdateQuadPositions`), so the ground under a base you drove away from is not the
one you left. It cannot be reset mid-flight without moving everything that lives in it. So the patch
changes the arithmetic, not the frame.

[Terrain Precision Fix](https://github.com/lhervier/KSP-TerrainPrecisionFix) is two Harmony patches in
one source file, redoing in double the two placements that go through a float at
planet scale, and leaving the frame they hang in exactly as stock moves it:

- **the origin of each quad**, moved after `PQ.SetupQuad` and `PQ.PreciseUpdateSubQuadsPosition` to
  `body.rotation * positionPlanet + body.position`, in `QuaternionD` and `Vector3d`;
- **each vertex inside its quad**, subtraction first: vertex minus quad origin, both doubles relative
  to the centre of the body, then rotated into the quad's frame.

The two 600 km vectors cancel while still in double, where a step is a tenth of a nanometre; what
reaches a float is a distance *within* the quad, a couple of kilometres at most, where a step is a
tenth of a millimetre. It is not a new way of placing terrain, it is the stock placement evaluated in
an order that does not throw away its own significant digits. The rounding is still there, and it is
still a new one on every load — it is just small enough that nothing can notice it any more.

**Why the file is longer than that description.** Most of it is a per-quad cache:
`PQS.BuildVertexSurfaceRelative` runs 225 times per quad, so what depends on the quad rather than the
vertex is worked out once per quad, and dropped when `PQS.BuildQuad` starts.

**Scope and safeguards.** Only the highest subdivision level is touched: the quads craft stand on,
the only ones with a collider (`PQSMod_QuadMeshColliders.maxLevelOffset` reads 0 in flight on Kerbin
and on the Mun) and the only ones stock moves to a container outside the body's hierarchy, so they
can hold a precise position at all. Everything else is built exactly as stock builds it. A correction
larger than 1 m is refused quad by quad, so a frame that is not the expected one leaves the terrain
where KSP puts it rather than somewhere else. If any patch fails to install, none of them does
anything.

## The readings, with the patch

Both instruments' six loads, run again in one install (KSP 1.12.5, Harmony, ModuleManager, KSPCF
1.41.1) without and with the patch. With Diag 1, where the craft rests spreads 134.5 mm on Kerbin
without it and 0.004 mm with it, and under 0.06 mm on the Mun, Minmus and Gilly. With Diag 2, the
collision surface spreads 108.1 mm on Kerbin without it and 0.004 mm with it, and under 0.02 mm on the
other three. Every reading, with the screenshots, in [Checking the culprit: loading the same save](https://github.com/lhervier/KSP-TerrainPrecisionFix/blob/master/docs/checking-the-culprit-loading.md).
Coming back to a parked craft, six round trips in one flight: the craft 21.8 mm without it and
0.094 mm with it, the surface 21.8 mm and 0.011 mm
([readings](https://github.com/lhervier/KSP-TerrainPrecisionFix/blob/master/docs/checking-the-culprit-approach.md)).
Switching to a craft 2 km away after a load: 104.5 mm and 0.022 mm, 120.4 mm and 0.003 mm
([readings](https://github.com/lhervier/KSP-TerrainPrecisionFix/blob/master/docs/checking-the-culprit-switching.md)).

It does not make the surface *better*, and is not meant to: it lands among the values stock scatters
around, and stops drawing a new one every time.

## Cost

The patch sits on a path the game runs all the time in flight, so it was measured there: the whole
frame with [KSPProfiler](https://github.com/KSPModdingLibs/KSPProfiler), and the vertex placement alone
with [PQS Bench](https://github.com/lhervier/KSP-PQSBench), which times it against the stock method on
the same quads in the same session. The placement is slightly faster than stock; in a whole frame the
difference is lost in the noise between two sessions. **It does not make the game any slower.**
Procedure, figures, logs and CSVs:
[Performance](https://github.com/lhervier/KSP-TerrainPrecisionFix/blob/master/docs/performance.md).

## What I have checked, and what I have not checked yet

This patch changes where the ground is, so everything that stands on it has to be checked against
it, one case at a time. That is in progress, and each case — Kopernicus, Breaking Ground,
existing saves… — is on
[Limits and solutions](https://github.com/lhervier/KSP-TerrainPrecisionFix/blob/master/docs/limits-and-solutions.md),
marked checked, affected or still to do. It is my list, and surely not complete: anything you can see
missing from it is exactly the help I am after.

**About `PQSOnlyStartOnce`.** I know the last PQS patch shipped here was disabled by default the same
day and is still off. This one does not change *when* the PQS starts, only *where* an already-built quad
is placed, and a frame off by more than a metre makes it a no-op. Deferred, the mod behind that
episode, is on the list and its test is next — that episode says anything touching the PQS deserves
care, this one included.

This is a work in progress, not a finished fix, and I am not asking for it to be shipped. I am asking
for a review: whether the diagnosis holds, whether the patch is correct, and what else it could break.
Any check you would want to see run, I will run.

**About #214.** Going through the open issues, I noticed that [#214](https://github.com/KSPModdingLibs/KSPCommunityFixes/issues/214), the ground anchor that
climbs a little on every load, becomes reproducible once the ground holds still. I will add what I
find there as a comment on it once I have something solid.

**About #435.** I went and read [chambm's WorldFrameDrift PR](https://github.com/KSPModdingLibs/KSPCommunityFixes/pull/435)
because it works on the very frame the ground is rounded in, and I wanted to know whether it changed
anything here. It helped me a lot to understand these angles, and Diag 3 came out of it. For the ground, though,
putting the frame back in place at a scene change would not be enough on its own: Diag 3 shows the
frame moving afterwards with nothing loaded — its angle above the inverse-rotation altitude, its
position at every floating origin shift, and in one jump when you drive away from a parked craft. What Diag 3
shows on loads does not look exactly like what the PR describes; I will post there once I am sure of
what I am seeing.
