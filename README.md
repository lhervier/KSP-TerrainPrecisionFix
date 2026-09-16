# Terrain Precision Fix

A fix for stock KSP 1.12, meant as a proposal for
[KSP Community Fixes](https://github.com/KSPModdingLibs/KSPCommunityFixes) and kept as small as
possible for that reason: two Harmony patches, in one source file. Here is what they fix:

> **The ground KSP builds under you is never built at the same height twice.** Load the same save five
> times, and the surface your craft is standing on comes back a little higher or a little lower each
> time — a few centimetres apart on Kerbin, less on smaller worlds.

**How this was made.** The investigation and the code were written with Claude, Anthropic's AI
assistant. Everything here was reviewed and validated by a human — me — who very much enjoyed
learning along the way how KSP builds the ground you land on. I am saying so up front, because
contributions made with an AI deserve a closer look than others, and because some people would
rather stop reading here. That look is what this page is built for: every figure on it comes from an
in-game measurement, the instrument behind the headline figures is public and runs on a stock
install, the stock code quoted here is a handful of lines anyone can check, and the fix fits in one
file you can read in a few minutes.

## Why the moving ground matters

Every time you load, it is a coin toss between two outcomes.

**The ground comes back lower than it was when you saved.** Your craft is now hovering a couple of
centimetres above it, so it drops those two centimetres. You never notice, and nothing breaks.

**The ground comes back higher than it was when you saved.** Your craft is now *inside* the ground —
and the physics engine will not leave two solid things overlapping. It pushes them apart, hard, in
the only direction available: up. Your craft gets launched.

![A craft jumping on its own the moment a save is reloaded](imgs/Booing-scaled.gif)

*KSP 1.12 without this mod, with [KSP Community Fixes](https://github.com/KSPModdingLibs/KSPCommunityFixes)
as the only mod installed. A pod on a fuel tank, parked in the grass at the KSC, saved, then reloaded
from the pause menu — nothing touched in between.*

That second case is the symptom everybody already knows. The lander that twitches, hops or flips the
moment the scene finishes loading. The base that sat perfectly flush yesterday and is buried up to
the hatches today. The big base that tears itself apart the very first time you load it, and never
again afterwards. A craft with many parts spread over a wide area gives the coin toss more chances
to land the wrong way up.

**Loading is not the only time the coin is tossed.** A landed craft you fly towards is loaded long
before you reach it, but held still at the position it was left at; its physics only starts once you
are within 200 m. The ground under it was not built when that position was recorded, so the same toss
happens there. From 200 m away you see much less of it — and it does just as much damage.

Loading is the moment that can be repeated at will, though: reload the same save, and the coin is
tossed again. So it is the case this page measures, and the only one it deals with from here on.

### Disclaimer: it is not the only cause

The ground moving is one cause among several, and this page does not claim it is the only one. Plenty
of other things move a craft when a scene opens. Two well-known examples, among others:

- **suspensions.** Landing legs and wheels come back fully extended, because that is the only state
  KSP can restore them to. They then compress under the weight of the craft, and the craft moves
  while they do.
- **a craft bent to fit the ground.** While you play, physics twists the joints between parts so the
  craft settles onto the shape of the ground beneath it. That twisting is not saved. On loading, the
  craft comes back in its original, unbent shape — and if the ground is not flat, part of it really
  *is* underground, with no measurement error involved.

This mod removes that one cause, and only that one: a lander that hops because its legs are still
unfolding will go on hopping once it is installed. What it takes away is the part that should never
have been there at all — a surface that is not where the game's own formulas say it is, and is not in
the same place twice. It takes it away down to a hundredth of a millimetre, measured below.

## The culprit

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

### Why it is different at every load

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
[The fix this mod proposes](#the-fix-this-mod-proposes). First, the hypothesis has to hold.

## Checking the culprit

Before touching anything, two instruments measure what stock does:
[Terrain Precision Fix Diag 1](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag) measures the craft, and
[Terrain Precision Fix Diag 2](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2) measures the ground. Each has its own page, with
its method and its protocol. Then the same campaigns are run again, with this mod installed.

### Terrain Precision Fix Diag 1, on stock: the craft

Terrain Precision Fix Diag 1 measures the distance from a landed capsule to the centre of the body,
twice per load: as the save hands the capsule back (*On rails*), and once it has settled on the ground
(*Settled*). How, and why those two readings, is in
[This mod's demonstration](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag#this-mods-demonstration).

Its campaigns, detailed in [The measurements](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag#the-measurements): stock KSP 1.12.5 with
nothing in `GameData` but that instrument. On each of four worlds, a lone capsule, then the same capsule
sitting on a small flat fuel tank, saved once on flat bare ground and loaded five or six times. Each
series uses its own spot, chosen by the rules of [its protocol](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag#the-protocol): no
`Moving Vessel` line in `KSP.log`, and a craft that does not slide.

*On rails* is the same on every load, to the micrometre: KSP puts the craft back at the same place.
*Settled* is not. Here is its spread, set against the step of a float at that distance from the centre
of the body:

| series | loads | distance to the centre | float step there | spread of *Settled* | in steps |
|---|---|---|---|---|---|
| Kerbin, capsule | 6 | 600.1 km | 62.5 mm | 135.5 mm | 2.2 |
| Kerbin, 2 parts | 6 | 600.1 km | 62.5 mm | 129.9 mm | 2.1 |
| Mun, capsule | 6 | 204.1 km | 15.6 mm | 18.1 mm | 1.2 |
| Mun, 2 parts | 6 | 204.1 km | 15.6 mm | 43.4 mm | 2.8 |
| Minmus, capsule | 6 | 60.0 km | 3.9 mm | 3.9 mm | 1.0 |
| Minmus, 2 parts | 5 | 60.0 km | 3.9 mm | 7.3 mm | 1.9 |
| Gilly, capsule | 6 | 15.9 km | 0.98 mm | 1.2 mm | 1.2 |
| Gilly, 2 parts | 6 | 16.6 km | 1.95 mm | 2.3 mm | 1.2 |

From one world to the next the spread varies a hundredfold, but counted in float steps it stays between
one and three: the size the culprit predicts.

That still only shows the craft moving, which is not by itself proof that the ground moved under it.
The second instrument is there for that.

### Terrain Precision Fix Diag 2, on stock: the ground

Terrain Precision Fix Diag 2 measures the ground, with no craft in the reading at all: the collision
surface a ray pointed straight down hits, against the height KSP computes for that same spot. The second
never moves; the first is what your landing legs touch. *Difference* is the first minus the second. How
both are read is in [This mod's demonstration](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2#this-mods-demonstration).

Its campaigns, detailed in [Six loadings of the same save](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2#six-loadings-of-the-same-save):
the same stock install, with nothing in `GameData` but that instrument; one save on each of the four
worlds, loaded six times, following [its protocol](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2#the-protocol). Over those six loads:

| world | spread of *Difference* | spread of the height KSP computes |
|---|---|---|
| Kerbin | 106.6 mm | 0.000 mm |
| Mun | 8.8 mm | 0.035 mm |
| Minmus | 3.1 mm | 0.000 mm |
| Gilly | 3.7 mm | 0.046 mm |

The craft never moved and the spot never changed, yet the height KSP computes held still while the
collision surface wandered by up to ten centimetres. The ground itself is not built in the same place
twice. The full readings, and what else they show, are in
[What the numbers say](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2#what-the-numbers-say).

### Terrain Precision Fix Diag 1, with this mod: the craft

This fix is meant for [KSP Community Fixes](https://github.com/KSPModdingLibs/KSPCommunityFixes), so it
is measured in an install that has it: KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes
1.41.1, this mod and Terrain Precision Fix Diag 1. The same test, on the same four worlds, with the same
two craft, loaded six times per series.

What it is compared with is the same install without this mod: Terrain Precision Fix Diag 1 ran those
campaigns too, on the same spots, and published their screenshots in [`imgs/kspcf`](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag/tree/main/imgs/kspcf). Their spread
is of the same order as on a stock install with nothing else: KSP Community Fixes does not change the
defect.

With one capsule, read off the four screenshots in [`imgs/Diag1/1part`](imgs/Diag1/1part) (in each of
them, the bottom line is the loading in progress, still live, and is not counted):

| world | spread of *Settled*, without this mod | spread of *Settled*, with this mod |
|---|---|---|
| Kerbin | 134.5 mm | 0.004 mm |
| Mun | 20.7 mm | 0.031 mm |
| Minmus | 6.7 mm | 0.038 mm |
| Gilly | 3.3 mm | 0.058 mm |

With two parts, read off the four screenshots in [`imgs/Diag1/2parts`](imgs/Diag1/2parts):

| world | spread of *Settled*, without this mod | spread of *Settled*, with this mod |
|---|---|---|
| Kerbin | 124.7 mm | 0.025 mm |
| Mun | 11.8 mm | 0.085 mm |
| Minmus | 4.8 mm | 0.023 mm |
| Gilly | 2.5 mm | 0.208 mm |

**On Kerbin, the spread goes from more than twelve centimetres to a few hundredths of a millimetre at
most.**

On the other worlds too, what is left stays in the hundredths of a millimetre, two tenths at worst, far
below the float step at any of these distances. *On rails* is still identical on every line, so KSP put
the craft back at the same place every time, and the craft now comes to rest at the same place every
time too.

In other words: reload the same save as many times as you like, and the craft comes back to the same
place, on ground that is in the same place. The coin toss of
[Why the moving ground matters](#why-the-moving-ground-matters) is gone — there is nothing left to push
the craft out of.

### Terrain Precision Fix Diag 2, with this mod: the ground

The same install, with Terrain Precision Fix Diag 2 instead: KSP 1.12.5 with Harmony, ModuleManager, KSP
Community Fixes 1.41.1, this mod and the instrument, one save per world, loaded six times. It is compared
with the campaigns Terrain Precision Fix Diag 2 ran on the same spots in that install without this mod,
published in [`imgs/kspcf`](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/tree/master/imgs/kspcf). On the Mun, on Minmus and on Gilly, those are also the spots of its
stock campaigns; on Kerbin the spot is another one, a grassy slope.

Read off the four screenshots in [`imgs/Diag2`](imgs/Diag2) (as before, the bottom line of each is the
loading in progress, and is not counted):

| world | *Difference*, without this mod | *Difference*, with this mod | spread, without | spread, with |
|---|---|---|---|---|
| Kerbin | +199.797 to +307.930 mm | +246.972 to +246.976 mm | 108.1 mm | 0.004 mm |
| Mun | −23.958 to −38.925 mm | −40.756 to −40.769 mm | 15.0 mm | 0.013 mm |
| Minmus | −12.291 to −16.351 mm | −14.202 to −14.204 mm | 4.1 mm | 0.002 mm |
| Gilly | +37.426 to +40.391 mm | +37.511 to +37.520 mm | 3.0 mm | 0.009 mm |

Read the table by its last two columns, not its first two. Three things to read in them.

**The column stops varying**, by a factor of three hundred on Gilly, a thousand on the Mun, two thousand
on Minmus, and more than twenty thousand on Kerbin. On Kerbin the surface under the craft came back
somewhere else over a range of eleven centimetres; it now comes back within four thousandths of a
millimetre. That is the fix, and that is all of it.

**It does not get smaller, and it is not supposed to.** It stops at a value the stock draws are scattered
around. On Kerbin and on Minmus the fixed reading falls well inside the range of the six loadings without
this mod, and on Gilly just inside it. On the Mun it falls just below: six draws are few for a spread that
wide. The stock campaign of Terrain Precision Fix Diag 2, on that same spot, drew
[down to −44.363 mm](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2#the-readings), and the twelve draws together cover −24.0 to −44.4 mm, around
the −40.76 mm this mod reads. Put the loading that landed on −23.958 next to a fixed −40.765 and the fix
looks like it made things worse; it did not, that line was luck. This mod does not choose a better number
for that patch of ground; it stops drawing a new one at every loading.

**What is left is no longer the ground, and it stays.** *Ground KSP computes* is what says the same spot
was read every time: 189,650.347 mm on all six Kerbin lines, and `0.000` on the Minmus flats. On the Mun
and on Gilly, where the ground is not level, that column wanders a little by itself — 0.018 mm over the
six Gilly loadings — because a craft settling a hair to one side asks for the height of a slightly
different point. On Gilly that is more than the spread of *Difference* under it, 0.009 mm: both columns
follow the sample point together, and most of the wobble cancels between them. What remains of the
spread is the craft, not the terrain. What remains of *Difference* itself is geometry: the collision mesh
is made of flat triangles, and they miss what the ground does between two corners —
[Terrain Precision Fix Diag 2 explains why a correct reading is not zero](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2#why-a-correct-reading-is-not-zero).
On that Gilly slope it is +37.5 mm, on that Kerbin slope +247.0 mm, the same on every loading. Removing
it would mean giving that mesh more triangles, which costs frames, for a gap nobody can feel.

This is also the last proof that the culprit is the right one. The fix changes where a subtraction
happens, and nothing else about the values placed; were the cause elsewhere, reordering that
subtraction would have left the spread untouched.

### A save made without this mod

This mod brings the ground back to the same height on every load. It does not bring it back to the
height it had in the loading where a save was made without it.

A craft saved on stock sits on the ground of that one draw. If that ground was lower than the corrected
one, the craft now comes back slightly inside the ground and gets pushed out — and since the ground no
longer changes, it gets pushed out **on every load of that save**, the same way each time. Reloading
the same file will not make it go away.

Let the craft settle with this mod installed and save again: from that save on, the craft and the
ground agree. This follows from the measurements above; it has not been measured on its own.

## The fix this mod proposes

### Two ways out, one taken

The culprit leaves two ways out: make the roundings come out the same on every load, or stop rounding
at planet scale.

**Freezing the frame** would mean giving the world frame the same orientation and the same position
relative to the body on every load. It was set aside, on a reading of the stock code rather than on a
measurement:

- the matrix that draws the rounding has a rotation **and** a translation. Pinning
  `Planetarium.InverseRotAngle` would only pin part of the rotation — the orientation of the body in the
  world frame also depends on the date of the save — and the translation moves at every floating origin
  shift. According to the stock code, landed quads are placed again, the same way, at every such shift
  (`CelestialBody.PreciseUpdateQuadPositions`), so the ground would be rounded anew without any reload.
  Pinning the translation too would mean removing the floating origin, which is what lets KSP run in
  float at all;
- and a rounding that repeats is still a rounding: the ground would come back to the same place, but
  that place would still be off the height the game computes by as much as a few centimetres, as the
  stock readings above show.

**Doing the arithmetic in double** removes the error instead of freezing it, at the place where the
precision is actually lost. That is what this mod does.

### Doing the arithmetic in double

The two stock placements that go through a float at planet scale are redone in double:

- **The origin of each quad.** After `PQ.SetupQuad` and `PQ.PreciseUpdateSubQuadsPosition`, the quad
  is moved to `body.rotation * positionPlanet + body.position`, computed with `QuaternionD` and
  `Vector3d`. The result is a world position, relative to the floating origin: near the craft it is a
  short vector, which a float holds precisely.
- **Each vertex, inside its quad.** `PQS.BuildVertexSurfaceRelative` is replaced by the same
  computation with the subtraction done first: the vertex minus the quad origin, both doubles relative
  to the centre of the body, then rotated into the quad's frame.

A double holding 600 km changes in steps of about a tenth of a nanometre, so nothing is lost while the
long vectors are handled. What finally reaches a float is a distance within the quad, a couple of
kilometres at most, where a float step is about a tenth of a millimetre instead of 62.5 mm. That float
is still there, and its rounding may well still differ from one load to the next — but at that scale.
It is the order of what the tables above still show with the fix.

This is not a new way of placing the terrain. It is the stock placement, evaluated in an order that
does not throw away its own significant digits.

Two frames look like more obvious choices, and both are wrong. `PQS.GetWorldPosition` does not give a
position in the frame the quads hang in. The rotation of the body's transform is the same rotation as
`body.rotation`, but held in float: applied to a 600 km vector, it brings back the very rounding being
removed.

### Once per quad, not once per vertex

`PQS.BuildVertexSurfaceRelative` runs for every vertex of every quad the game builds, 225 per quad, and
the game builds quads all the time while flying low. Stock already asks Unity for two `Transform`s on
every one of those vertices, and calls into them twice.

The replacement needs more than that, and all of it depends on the quad rather than on the vertex:
whether the fix applies to it at all, the frame it hangs in, the inverse of its rotation. Worked out
again for every vertex, that would cost more than the placement it is part of. So it is worked out
once, on the first vertex of a quad, and kept for the others; it is forgotten whenever `PQS.BuildQuad`
starts, since a quad can be rebuilt after having moved. What is left per vertex is a `Vector3d`
subtraction, a rotation in double, a rotation in float and two writes.

What that saves, and how much of it comes from the double-precision arithmetic rather than from the
work done once per quad, is measured in [Performance](#performance).

### Only where a craft can stand

Only the quads of the highest subdivision level are corrected. Those are the ones craft stand on, the
only ones with a collider on Kerbin and on the Mun, where `PQSMod_QuadMeshColliders.maxLevelOffset` has
been read in flight and is 0 (see [TODO.md](TODO.md)), and the only ones that can keep a precise
position: stock moves them to a container of their own, outside the body's hierarchy. Every other quad
hangs from the body's terrain sphere, whose origin is the centre of the body, so Unity would store any
position given to it as a 600 km float again. Those are left exactly as stock builds them.

The game only builds that level close to the ground: below 6 250 m over the Mun and 9 375 m over
Kerbin, read in flight from each sphere's own `subdivisionThresholds` and written to the log by
[PQS Bench](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench), once per sphere, in every
run of [Performance](#performance). Higher up, the patched code decides once per quad that it does not
apply, and each vertex is left with a reference comparison before stock runs untouched.

### Safeguards

- a correction larger than 1 m is refused, quad by quad, and that quad is left as stock builds it: if
  the frame is ever not the expected one, the terrain stays where KSP puts it instead of going
  somewhere else;
- if any patch fails to install, none of them does anything.

## Performance

Now that the cause is known and fixed, what does the fix cost? The vertex part replaces a computation
that stock runs for every vertex of every terrain quad it builds, so it sits on a path the game uses
continuously while flying, not only when a scene loads. **It places a vertex in less than a third of
the time stock takes.**

### The instrument

Everything below was taken with
[PQS Bench](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench), a measuring mod that counts
what building the stock terrain costs and times whatever is patching the vertex placement against stock
on the same data, in the frame the game built a quad in. **Its page carries the procedure**, and the
rules that make a run worth keeping.

### Two changes, three configurations

This fix changes two things at once: the arithmetic, done in double, and the work done once per quad
instead of once per vertex. Compared with stock alone, the two savings cannot be told apart — and the
second is not specific to the fix: stock could read its two `Transform`s once per quad too.

So the fix is measured against a third configuration as well,
[Stock Quad Cache](https://github.com/lhervier/KSP-TerrainPrecisionFix-StockQuadCache), a mod written
for this measurement alone: stock's arithmetic bit for bit, with the only difference that the two
`Transform`s are read once per quad instead of once per vertex. It carries the second change without
the first, and splits the saving between them.

Each configuration keeps its own logs and its own reading of them, in its own repository: the stock
reference with [the bench](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench/blob/master/perfs/README.md),
the middle term with [Stock Quad Cache](https://github.com/lhervier/KSP-TerrainPrecisionFix-StockQuadCache/blob/master/perfs/README.md),
and this mod's two runs in [`perfs/`](perfs/).

### The campaign

KSP 1.12.5 with Harmony, ModuleManager and KSP Community Fixes 1.41.1: a command pod on rails in a
circular orbit 5 km over the Mun, low enough that the game builds the highest subdivision level — the
only one the fix acts on — and six runs of two and a half minutes of that one save, which covers the
same ground every time. **All six were taken on my laptop**, described with
[the stock run](https://github.com/lhervier/KSP-TerrainPrecisionFix-PQSBench/blob/master/perfs/README.md);
figures from another machine are not comparable to these.

Three configurations, each measured twice, once for what a vertex costs and once for what a frame pays.
The reference is a KSP with this mod's folder taken out of `GameData`, since a mod left in place still
pays for its own patches on the path being timed.

Every `counters` run built exactly 1 272 quads of the highest subdivision level over 148 samples (3 211
to 3 213 quads in all) — the craft is on rails, so the same save covers the same ground.

### What a vertex costs

40 quads per run, 72 000 vertices per formula. Each run times what is installed against its own
measurement of stock, in the same frames, and the figure read is the difference between the two. Per
vertex:

| installed | stock, in that run (`stockNsPerVertex`) | installed (`installedNsPerVertex`) | difference (`differenceNsPerVertex`) |
|---|---|---|---|
| nothing: stock's arithmetic, `Transform`s read on every vertex | 229.6 ns | 228.5 ns | −1.1 ns, the floor of the method |
| stock's arithmetic, `Transform`s read once per quad | 232.3 ns | 149.5 ns | −82.8 ns |
| this fix | 228.5 ns | 64.2 ns | **−164.3 ns**, **3.56× faster** |

Stock makes five trips into the native engine per vertex: `Transform.TransformPoint`,
`Transform.InverseTransformPoint`, and two reads of `Component.transform` — `BuildVertexSurfaceRelative`
runs once per vertex, and reads `base.transform` and `buildQuad.transform` each time. The replacement
makes none.

The middle row is not a placement the game contains. It says where the 164.3 ns go: **82.8 ns** are the
two `Transform` reads, 41.4 ns each, and **81.5 ns** are the arithmetic, the double-precision version
being that much cheaper than two native calls. Read the other way: even if stock stopped asking Unity
for a `Transform` on every vertex, the fix would still be more than twice as fast.

The same row is what justifies [working out once per quad](#once-per-quad-not-once-per-vertex) what
depends on the quad: two reads of `Component.transform` per vertex cost 82.8 ns, more than this fix
spends on a vertex altogether, and the fix needs rather more than two things per quad.

The difference is read within each run, never between two: from one session of KSP to the next the
whole replay runs a little faster or slower — the stock column above reads 229.6, 232.3 and 228.5 ns,
a 1.7 % spread — and only a difference taken inside one session cancels that out.
In the run with neither mod installed the two readings are of the same code reached two different ways,
and they differ by 1.1 ns: the floor of the method, which any other row carries too.

Placing a vertex is a small part of building one: a quad of the highest level takes 1.7 ms to build,
about 7.6 µs per vertex, nearly all of it spent in the `PQSMod`s that compute height and colour. The
230 ns stock spends placing it are 3.0 % of that.

### In flight

The same save and the same stretch of orbit, three times, 150 seconds each, counted second by second.

| | stock | `Transform`s read once per quad | this fix |
|---|---|---|---|
| frames per second | 77.04 | 75.82 | 79.16 |
| ms per quad the fix acts on | 1.716 | 1.682 | **1.650** |
| terrain per frame | 1.195 ms | 1.170 ms | 1.182 ms |
| terrain share of real time | 9.21 % | 8.87 % | 9.36 % |

Per quad, the fix is ahead by 3.8 %, where the calibration predicts 2.2 % — 164.3 ns × 225 vertices is
37.0 µs per quad. **That should not be read as a measurement.** The middle column is the reason: by the
same reckoning it should be 1.1 % ahead, and it is 2.0 % ahead; per frame it is ahead of the fix; and
the fix, faster per quad, takes a larger share of real time than stock. The noise between two sessions
of KSP is worth about as much as the effect being looked for at this scale. What these three runs
establish is a bound — nothing degrades at the scale of a frame — and the figure worth publishing is the
calibration.

### What this says

The fix is faster than stock, and faster than stock with its `Transform`s read once per quad: half of
its saving is the organisation any version could adopt, the other half is the arithmetic itself.

Nor is the saving worth having for its own sake. At 5 km over the Mun the game builds 8.5 of these quads
per second, so 1 917 vertices: 164.3 ns each is 0.31 ms per second of flight, 0.031 % of real time. The
point is not the gain. It is that the correction is free.

## Limits and solutions

What this fix can make worse, and what it has not been checked against yet — each with its solution,
or with what is still missing for one. Every campaign run with this mod installed was run on KSP 1.12.5,
with Harmony, ModuleManager and KSP Community Fixes 1.41.1.

### Rocks, grass and trees

**Limit.** This mod does not move terrain scatter, and scatter is drawn further off the ground with it
than without it. Visual only.

Another instrument, [Rock Precision Fix Diag](https://github.com/lhervier/KSP-RockPrecisionFixDiag),
measures where scatter is drawn: for every object of the terrain quad nearest to the craft, the height
of its lowest point above the ground right under it.

The offset exists in stock: scatter is already not drawn exactly on the ground. The objects of a quad
are built from the quad's vertices, in the quad's own coordinates, and hang from a holder that
`PQSMod_LandClassScatterQuad.Setup` places under the terrain sphere, at
`localPosition = quad.positionPlanet`: a vector hundreds of kilometres long, in a float. Unity draws the
holder with its local to world matrix, and the translation of that matrix differs from the holder's own
transform position by whole float steps. The objects are drawn that much above or below the ground,
differently on each quad and on each load. This mod inherits that offset and widens it: the ground is
now placed in double precision and the holders are not, so the stock error on the quad origin adds to
the stock error of the holder.

Kerbin, next to the KSC, the same save loaded six times per series, over the 118 holders of 64 quads:

| offset of the scatter from the ground | stock | with this mod |
|---|---|---|
| range | −85.5 to +64.7 mm | −66.3 to +152.0 mm |
| standard deviation | 29.7 mm | 44.4 mm |

The same kind of error, about one and a half times wider. On the quad nearest to the craft, the height
of the objects minus those two errors is the same on every load of both series, to 1.5 mm: nothing else
moves them. Stock scatter has no collider, so the offset, with or without this mod, is visual only.

**Solution.** [Rock Precision Fix](https://github.com/lhervier/KSP-RockPrecisionFix), a separate mod,
corrects the stock placement of scatter: it hangs each holder from its own terrain quad, so that the
objects are drawn in the frame they were built in. It works with or without this mod. It has not been
measured yet.

### Scatter with colliders

**Limit.** Breaking Ground's surface features, and Kopernicus scatter with `scatterColliders`, are
placed the same way as the rocks, but they have colliders, so the offset would be physical there: wider
than in stock if the physics takes the holder's pose from the same matrix the renderer uses, new if it
takes it from the transform position, which matches the stock ground. Not measured.

**Solution.** Not there yet. First measure where the physics puts those colliders; Rock Precision Fix
could then hang these holders from their quads the same way (see [TODO.md](TODO.md)).

### Not checked yet

- **Kopernicus.** Not measured, and required before this goes anywhere: most planet packs go through
  it. If Kopernicus places the quads in another frame, the 1 m safeguard should leave its terrain as
  stock builds it, with one warning per body in the log. *Solution:* measure it with Terrain Precision
  Fix Diag 1, on a stock body and on a planet pack body (see [TODO.md](TODO.md)).
- **Parallax scatters.** According to its source, they should follow the corrected ground: their
  positions and colliders are expressed relative to the quad, and a collider is a child of its quad, so
  they move with it. *Solution:* confirm it in game (see [TODO.md](TODO.md)).
- **Colliders below the highest subdivision level.** `PQSMod_QuadMeshColliders` gives a collider to
  every quad at or above `maxLevel - |maxLevelOffset|`, and with an offset other than 0 the fix would
  leave those lower quads uncorrected. The offset is **0** on Kerbin and on the Mun, so there the fix
  covers every quad a craft can stand on. *Solution:* read it on the other bodies (see
  [TODO.md](TODO.md)).
- **The map view**, where quads are built and destroyed all the time. *Solution:* measure it.
- **Breaking Ground's deployed experiments.** They are vessels, positioned in double like any craft, so
  they should sit on the corrected ground like one. *Solution:* check it with Terrain Precision Fix Diag 1.
- **The KSC buildings, runway and launchpad.** Not covered: `PQSCity` and `PQSCity2` both do
  `base.transform.localPosition = planetRelativePosition;`, where `planetRelativePosition` is a
  `Vector3d` measured from the centre of the body. With Terrain Precision Fix Diag 1 on stock, a capsule
  parked on the runway spreads over 117 mm on six loads, as on the grass next to it; whether that comes
  from the runway itself or from the terrain underneath has not been separated yet. *Solution:* the same
  measurement with this mod installed tells the two apart — if the capsule on the runway still moves,
  it is the static (see [TODO.md](TODO.md)).

## Install

Requires KSP 1.12 and [HarmonyKSP](https://github.com/KSPModdingLibs/HarmonyKSP) (the usual
`GameData/000_Harmony`, also installed by KSP Community Fixes).

Copy `GameData/TerrainPrecisionFixMod` into the `GameData` of KSP. Nothing is written to your saves:
removing the folder gives you the stock terrain back.

## Settings

`GameData/TerrainPrecisionFixMod/PluginData/settings.cfg` holds a single value, read when KSP starts:

| `logLevel` | what goes to `KSP.log` |
|---|---|
| `Info` (default) | one line at startup, then one line per body the first time its terrain is corrected |
| `Debug` | adds one line per quad placed, with how far it was moved |
| `Trace` | adds, per quad, how far its vertices were moved within it — slower, meant for measuring |

`Error` and `Warning` are accepted too. To change it: quit KSP, edit the file, start KSP again.

## Build

Set `KSPDIR` to your KSP install folder, which must contain `GameData/000_Harmony`, and run `build.bat`.
It needs the .NET SDK, and produces `GameData/TerrainPrecisionFixMod/TerrainPrecisionFixMod.dll`.

## License

MIT
