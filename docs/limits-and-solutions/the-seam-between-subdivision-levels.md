# The seam between subdivision levels

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: checked on Earth under Real Solar System, no problem of its own — this mod does not open the
seam, but widens a stock crack.** Where a quad of the highest subdivision level meets a coarser one, the
vertices the two are supposed to share are already apart in stock, by up to three metres on Earth, and
the crack along the seam can be seen. This mod corrects the finer side only, and the gap gets wider:
about 1.9 m instead of 1.2 m for the median of the largest gap of a load, and the crack shows more often.
Visual only, since the coarser quads have no collider. A separate mod could close it, in stock and with
this one (see [A possible solution](#a-possible-solution)). On Kerbin, the crack shows in stock too, for
a largest gap of about a tenth of a metre; how this mod changes it there is still to measure.

## What goes wrong

The quads of the highest subdivision level cover the ground around the craft, and they are the only
ones this fix corrects (see [Only where a craft can stand](../the-fix-ground.md#only-where-a-craft-can-stand)).
Further away, the terrain is made of coarser quads, which stock builds and places as it always has.
Where the two meet, the edge of the finer quad is meant to run along the edge of the coarser one (see
[How stock joins two levels](#how-stock-joins-two-levels)).

It does not quite: the vertices the two quads share are not at the same place, in stock already, and
the terrain has a crack along the seam. This fix places the finer side where the ground is and leaves
the coarser side where stock puts it, so it adds its own correction to that gap.

It stays visual: the quads below the highest level have no collider (see
[Colliders below the highest subdivision level](colliders-below-the-highest-subdivision-level.md)), so
a craft never stands on that edge. It is seen from the craft, some distance away, never under it (see
[What a player sees, on Earth](#what-a-player-sees-on-earth)).

## How stock joins two levels

A quad one level coarser covers twice the width, so along the edge two quads share, the finer one has
twice as many vertices. Stock does not move the extra ones: it changes the triangles of the finer quad
along that side (`PQ.GetEdgeState`, `PQS.cacheIndices`), so that its edge runs on every other vertex,
along the same segments as the coarser edge — provided the vertices both quads share land on the same
point. [What it shows](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag4/blob/master/docs/what-it-shows.md#the-seam),
in Terrain Precision Fix Diag 4, shows how, with a figure.

## The shared vertices do not meet, even in stock

Stock places every terrain vertex in two steps, in `PQS.BuildVertexSurfaceRelative` (the whole method,
and why it rounds, is in [The culprit: the ground](../the-culprit-ground.md)):

```csharp
planetRel = base.transform.TransformPoint(vertRel);
buildQuad.verts[vertexIndex] = buildQuad.transform.InverseTransformPoint(planetRel);
```

The first line takes the vertex from the centre of the body to the world, through the matrix of the
terrain sphere: its result depends on the vertex and on that matrix, not on the quad. The second line
expresses that world position relative to the quad's own `Transform`, as rounded. Read alone, this
says that two quads built in the same frame of the sphere put a vertex they share on the same point.

Measured, they do not. [Terrain Precision Fix Diag 4](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag4)
finds every seam around the craft and measures, in double precision, the distance between the places
where the two quads draw each vertex they share. On Earth under Real Solar System, **without this
mod**, over fifteen loads of a craft on the launchpad at Cape Canaveral, the largest gap of a load went
from 1.01 m to 3.05 m, and the mean over all shared vertices from 0.31 m to 2.14 m
([the measurements](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag4/blob/master/docs/the-measurements.md#case-1-real-solar-system)).
The gap changes from one load to the next, and so does its direction: the finer quad above the coarser
one, or below.

Why the two quads do not share the frame the code above assumes has not been traced. The world origin
is far from the craft when a scene loads, and moved onto it early; quads built on either side of that
move, or of any later floating origin shift, are not placed through the same matrix. That fits what
was measured, but is not checked.

## What this fix changes

This fix places each vertex of the highest level from double-precision values, without going through
the matrix of the sphere (see [Doing the arithmetic in double](../the-fix-ground.md#doing-the-arithmetic-in-double)).
Its coarser neighbour is left to stock: the vertices it shares with the corrected quad are still where
the matrix of the sphere puts them.

Part of the stock rounding is common to every quad of the body: it comes from the rotation and the
position of the sphere, held in float, which are what change at every load (see
[Why it is different at every load](../the-culprit-ground.md#why-it-is-different-at-every-load)). Between two
stock quads built in the same frame, that part cancels out; between a corrected quad and a stock one,
it does not, and it adds to whatever gap stock already leaves.

The coarser quads cannot be corrected the same way: they hang from the terrain sphere, whose origin is
the centre of the body, and Unity would store any precise position given to them as a 600 km float
again (see [Only where a craft can stand](../the-fix-ground.md#only-where-a-craft-can-stand)).

How much wider the seam gets is measured in [The seam with this mod](#the-seam-with-this-mod).

## The seam with this mod

The same measurement as without this mod, with this mod installed: case 1 of
[the protocol of Terrain Precision Fix Diag 4](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag4/blob/master/docs/the-protocol.md),
Real Solar System as released on KSP 1.12.5 with KSP Community Fixes, and this mod; a craft on the
launchpad at Cape Canaveral, reverted to launch again and again, in two sessions, twenty loads. Every
load ended with 60 seams between 49 quads of level 11 and 23 quads of level 10, 480 shared vertices.
The last line of the log of Terrain Precision Fix Diag 4 after each load, in metres:

| Session | Load | Gap, mean | Gap, max | Finer quad at the largest gap | Beside | From the craft | Screenshot |
|---|---:|---:|---:|---|---:|---:|---|
| 1 | 1 | 2.34 | 3.39 | 2.92 below | 1.71 | 36.9 km | |
| 1 | 2 | 1.91 | 2.80 | 2.35 below | 1.53 | 38.6 km | |
| 1 | 3 | 2.68 | 3.71 | 3.18 below | 1.91 | 37.7 km | |
| 1 | 4 | 0.44 | 1.24 | 1.09 below | 0.59 | 36.3 km | |
| 1 | 5 | 1.82 | 3.15 | 2.69 below | 1.63 | 39.2 km | |
| 1 | 6 | 2.46 | 3.46 | 2.71 below | 2.16 | 37.2 km | |
| 1 | 7 | 0.69 | 1.60 | 1.39 below | 0.78 | 33.3 km | |
| 1 | 8 | 0.70 | 1.60 | 1.41 above | 0.76 | 42.6 km | |
| 1 | 9 | 0.76 | 1.72 | 1.38 below | 1.03 | 37.7 km | |
| 1 | 10 | 0.86 | 1.94 | 1.79 above | 0.74 | 33.5 km | |
| 1 | 11 | 1.18 | 2.26 | 1.90 below | 1.21 | 37.2 km | |
| 1 | 12 | 1.88 | 3.10 | 2.71 below | 1.51 | 38.5 km | |
| 1 | 13 | 0.55 | 1.50 | 1.07 below | 1.05 | 39.2 km | |
| 1 | 14 | 0.53 | 1.42 | 1.11 below | 0.88 | 36.3 km | nothing at the foot of the line |
| 1 | 15 | 0.54 | 1.48 | 1.38 above | 0.54 | 41.9 km | a crack |
| 2 | 16 | 0.79 | 1.65 | 1.41 above | 0.87 | 38.8 km | a crack |
| 2 | 17 | 0.70 | 1.88 | 1.58 above | 1.03 | 34.5 km | |
| 2 | 18 | 0.96 | 1.84 | 1.46 below | 1.12 | 40.0 km | nothing at the foot of the line |
| 2 | 19 | 0.63 | 2.00 | 1.58 above | 1.23 | 40.2 km | a crack, in the full screenshot |
| 2 | 20 | 0.60 | 1.59 | 1.14 below | 1.11 | 42.1 km | two dotted lines leaving the foot of the line |

The second session began with the craft taken back from the Space Center rather than reverted, and the
craft was destroyed as it loaded; that load is left out. The logs of both sessions are in
[the runs](../../diag/README.md#on-real-solar-system).

**Load 15, the finer quad 1.38 m above**, the camera beyond the seam, the yellow line only:

[![Load 15: flat grassland under a blue sky, the yellow line standing left of the centre](../../imgs/seam-between-levels/earth-fix-above-small.jpg)](../../imgs/seam-between-levels/earth-fix-above-8k.png)

*With this mod, on Real Solar System as released, load 15: screenshot at 7680 × 4320, the camera beyond
the seam and looking back towards the craft. Click it for the full resolution. Below, the foot of the
yellow line, cut out of the full screenshot and enlarged four times:*

![The foot of the yellow line, enlarged four times: a thin dark line runs through it, across the grass](../../imgs/seam-between-levels/earth-fix-above-zoom.png)

**Load 14, the finer quad 1.11 m below**, the same way:

[![Load 14: flat grassland under a blue sky, the yellow line standing on the left](../../imgs/seam-between-levels/earth-fix-below-small.jpg)](../../imgs/seam-between-levels/earth-fix-below-8k.png)

*With this mod, on Real Solar System as released, load 14, the same way. Below, the foot of the yellow
line, enlarged four times:*

![The foot of the yellow line, enlarged four times, on grass](../../imgs/seam-between-levels/earth-fix-below-zoom.png)

Next to the fifteen loads without this mod
([Terrain Precision Fix Diag 4, case 1](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag4/blob/master/docs/the-measurements.md#case-1-real-solar-system)):

| | Loads | Gap, max | Median of the gap max | Finer quad above | Screenshots with a crack |
|---|---:|---|---:|---:|---|
| Without this mod | 15 | 1.01 to 3.05 m | about 1.2 m | 7 of 15 | 2 of 7 |
| With this mod | 20 | 1.24 to 3.71 m | about 1.9 m | 6 of 20 | 4 of 6 |

The ranges overlap: a load with this mod can leave a smaller gap than a load without it, but on the
whole the seam is wider, and the crack shows more often. Where the finer quad was above at the largest
gap, every screenshot with this mod shows a crack, for steps of 1.4 to 1.6 m; without it, one of three
does, for steps a little under a metre. In load 15 with this mod, the crack was also seen on screen, not
only in the screenshot.

## What a player sees, on Earth

The seam runs along the edge of the zone of the highest level, which follows the active craft: it is
always some distance away from the camera, never under the craft. On Earth under Real Solar System,
where it is largest, that distance is large.

On a launch from Cape Canaveral with this mod, on Real Solar System as released plus MechJeb (the
session is [`runs/launch-earth-rss-fix.log`](../../diag/runs/launch-earth-rss-fix.log), at
`logLevel = Debug`), the quads of the highest level placed around the launchpad were 180, 14 wide by
16 long, each about 5 km across: the edge of the corrected zone ran 25 to 35 km from the pad. Seen from
high above the craft, a gap of a metre or two that far away is a fraction of a pixel, even in a
screenshot of 7680 × 4320 with the stock field of view of 60°, and nothing shows in this one:

[![With this mod, on Real Solar System: a rocket on the launchpad at Cape Canaveral, seen from high above](../../imgs/seam-between-levels/launchpad-earth-fix-small.jpg)](../../imgs/seam-between-levels/launchpad-earth-fix-8k.png)

*With this mod, on Real Solar System as released: a small rocket on the launchpad at Cape Canaveral,
the camera zoomed out, screenshot taken at 7680 × 4320 (`SCREENSHOT_SUPERSIZE = 4` in `settings.cfg`,
on a 1920 × 1080 window). Click it for the full resolution.*

The camera can be brought close to the seam, though: pulled back from the craft far enough to stand
just beyond it, and turned back towards the craft. From there, the gap shows as a thin dark line along
the seam, where the terrain is open and what lies behind it shows through; best when the finer quad,
the one further from the camera, is the higher. In stock:

![Without this mod: a thin dark straight line runs through the foot of a yellow vertical line, across grassland](https://raw.githubusercontent.com/lhervier/KSP-TerrainPrecisionFixDiag4/master/imgs/earth-stock-above-zoom.png)

![The same place, the same camera, with the triangles of the two quads drawn: the dark line runs exactly along the edge between the red quad and the green one](https://raw.githubusercontent.com/lhervier/KSP-TerrainPrecisionFixDiag4/master/imgs/earth-stock-above-triangles-zoom.png)

*Without this mod, on Real Solar System as released: a craft on the launchpad at Cape Canaveral, the
camera beyond the seam, 38.8 km from the craft; two screenshots at 7680 × 4320 from the same camera, with
Terrain Precision Fix Diag 4 drawing, first, only a yellow line on the vertex of the largest gap, then
the triangles of the coarser quad in red and of the finer one in green; the foot of the line cut out of
each.*

It takes looking for: of seven screenshots taken that way without this mod, two show the crack
([what the measurements show](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag4/blob/master/docs/what-the-measurements-show.md)),
and of six with this mod, four do (see [The seam with this mod](#the-seam-with-this-mod)).

## A possible solution

Not written, not tried: ideas to close the seam. They are read in the code of the
game, of KSP Community Fixes, of Kopernicus and of Parallax; what reading cannot tell is listed with
each. Three ways were looked at. The third one, moving the vertices of the coarser quad, is the one this
page leans to, and comes first; the two others leave every vertex where it is and change the triangles
instead.

### Move the vertices of the coarser quad

The corrected quad is left exactly as this fix builds it, collider included. Along each seam, the
vertices the coarser quad shares with it are moved onto the corrected ones, each expressed in the frame
of the coarser quad:

```csharp
coarseVertex = coarse.transform.InverseTransformPoint(corrected.transform.TransformPoint(correctedVertex));
```

Only the edge moves. The rest of the coarser quad stays where stock puts it, so its first row of cells
leans by the size of the step over the width of a cell: a metre or two over several hundred metres on
Earth. The step becomes a slope nobody can see, and the crack along the seam closes.

**Why the coarser side.** Below the highest level, a quad has no collider where the offset of the
colliders is 0 (see [Colliders below the highest subdivision level](colliders-below-the-highest-subdivision-level.md)):
moving its vertices changes what is drawn, and nothing a craft can touch. The corrected quad, on the
other side, is the one a craft stands on: its vertices and its collider are never touched.

**Precision.** The vertices of a quad are stored relative to the quad's own position, a few kilometres at
most, where a float is precise to a fraction of a millimetre: the two edges meet to that precision,
without the rounding of a position hundreds of kilometres from the origin of the world.

**Nothing changes size.** The mesh keeps its 225 vertices and stock's lists of triangles: none of the
writes listed in [Who writes the mesh of a quad](#who-writes-the-mesh-of-a-quad) fails. The vertices of a
quad are only written when it is built (`PQS.BuildQuad`, `PQS.cs:2516`); after a move, they are given
back to the mesh, and its bounds recomputed.

**When to move them again.** A moved vertex stays right only as long as nothing around it changes:

- when the coarser quad is built again, stock gives it its own vertices back;
- when the corrected quad is built again, its correction changes;
- when the zone of the highest level moves, a side that is no longer a seam has to get its stock
  vertices back, or it opens a crack with its new neighbour: the stock vertices have to be kept;
- at every floating origin shift, the coarser quad follows the new rounding of the matrix of the sphere,
  while the corrected quad keeps its own (`PQ.PreciseUpdateSubQuadsPosition`, which this fix patches
  already): the step changes, and Terrain Precision Fix Diagnostic Mod 4 shows it changing at every
  shift.

The simplest answer to all four is not to follow each event, but to reconcile: a few times a second,
and right after every shift, find the seams the way Terrain Precision Fix Diagnostic Mod 4 does, move
the shared vertices, give their stock vertices back to the sides that are no longer seams, and hand the
changed vertices to the meshes. A few dozen quads are concerned; at worst, a crack shows for one frame
after a shift.

**The corners.** A corner of the coarser quad is also the corner of the coarser quads around it, some of
which only touch it there, at the corners of the zone. Moving that vertex alone would open a crack between
two stock quads: every quad that holds the point has to move with it. Where the zone turns inwards, the
two corrected quads that meet the coarser one at a corner do not put that corner at quite the same place,
but only by millimetres: either will do.

**One mod more.** This would be a separate mod, as [Rock Precision Fix](https://github.com/lhervier/KSP-RockPrecisionFix)
is for the rocks. It closes the seam whatever its size, so it would close the crack stock already leaves
as well as the wider one this mod leaves, and it reads nothing this mod does not already rely on.

**To check, if this solution is taken:**

- that the coarser quads have no collider on the bodies it is meant for, Earth under Real Solar System
  first (the offset of the colliders is only read on Kerbin and on the Mun);
- the edge normals: stock computes them from the vertices of both neighbours (`PQS.UpdateEdgeNormals`),
  after a neighbour changes level, and a moved edge could shade a little differently from the rest;
- that no mod reads the vertices of a coarser quad for anything else: Parallax copies the meshes of the
  quads near the camera only, and the coarser ones are far from it;
- with Terrain Precision Fix Diagnostic Mod 4: the gaps down to a fraction of a millimetre, and staying
  there across a floating origin shift, a move of the zone, and the reverts of a craft on the launchpad.

### Leave the vertices, change what the edge triangles rest on

Moving the vertices of the corrected quad along the edge, to where stock would put them, would close the
step, but those vertices are also the ones of its collider, which would get the stock rounding back.
The idea goes the other way: the vertices of the corrected quad are never touched, and only the
triangles along a marked side — the ones stock rebuilds already — rest on the vertices of the coarser
neighbour instead of its own.

A triangle can only point to vertices of its own mesh, and the neighbour is another object, with its
own mesh and its own `Transform`. So the edge vertices of the neighbour have to be copied into the
corrected quad, expressed in its own frame, and the edge triangles point to the copies. Two places can
hold them:

- **A. The quad's own mesh**, as extra vertices after its 225. The lists of triangles can stay shared
  and precomputed, like stock's sixteen, if each copy has a fixed index.
- **B. A small mesh of its own**, one per marked side: a strip of triangles from the second row of
  vertices of the corrected quad to the copies, the first row of the quad being left out by lists of
  triangles of this fix. The quad's mesh keeps its 225 vertices, but each marked side adds an object,
  drawn with the quad's material, that Parallax would not know about where it replaces the meshes and
  materials of the quads near the camera.

A is the cleaner of the two, and the one this chapter looks at. Either way, the step does not vanish,
it is spread out: the first row of cells goes from the corrected vertices, a cell in, to the copies on
the edge — a slope over a fourteenth of the quad's width, in place of a step.

### Who writes the mesh of a quad

Unity refuses any array of vertex data whose size is not the vertex count of the mesh, and the mesh of a
quad is written with arrays of 225:

| When | Who | Writes |
|---|---|---|
| building the quad | `PQS.BuildQuad`, and what it calls through `Mod_OnMeshBuild` | `vertices`, `triangles`, `normals`, `tangents`, `colors`, `uv` to `uv4` |
| at the end of the build | `PQSMod_QuadMeshColliders.OnQuadBuilt` | the collider: `sharedMesh = quad.mesh`, with 225 vertices and every triangle |
| when a neighbour changes level | `PQ.UpdateVisibility` | `triangles` only, then it queues the quad for its normals |
| right after | `PQS.UpdateEdgeNormals` | `normals`, then `tangents` through `PQS.BuildTangents` |
| in that same call, with KSP Community Fixes | its prefix on `PQS.BuildTangents` | reads `normals`, writes `tangents` |

Kopernicus and Parallax never write the mesh of a quad; Parallax copies it, for the quads near the
camera only.

So the mesh cannot stay larger than 225 vertices: every one of these writes would fail, and a rebuild
too, since `mesh.vertices` cannot shrink below the vertices its triangles point to. But the writes
always come in the same order, and the copies are only needed in between:

- **after `UpdateEdgeNormals`**, grow the mesh: append the copies, extend every vertex array (a copy
  takes the normal, colour, UVs and tangent of the edge vertex it stands for), and set the triangles of
  this fix;
- **before `UpdateEdgeNormals` and before `BuildQuad`**, shrink it back, stock triangles first, then the
  225 vertices and their arrays, so that stock and KSP Community Fixes find the mesh they expect;
- `UpdateVisibility` needs neither: triangles that point below 225 are valid on a larger mesh.

The mesh is rewritten when a neighbour changes level, which is rare next to the building of quads, not
at every frame. What else it takes:

- **Finding the neighbour's vertices.** Two neighbours can belong to two faces of the cube the sphere is
  built from, with grids turned relative to each other; stock matches them already for its edge normals
  (`PQ.GetEdge`, `PQ.GetEdgeQuads`).
- **Keeping the copies fresh.** On a floating origin shift, the coarser neighbour is rounded again by the
  new matrix of the sphere, and the copies have to follow, from `PQ.PreciseUpdateSubQuadsPosition`,
  which this fix patches already.
- **A larger fix.** Four more patches, the matching of edges and the refresh: this fix would grow well
  beyond what it is today.

### The collider

The collider is given the mesh once, at the end of the build, when it has its 225 vertices and every
triangle, and those vertices are never moved. Whether it stays fully corrected then depends on one thing
the code of the game cannot show: that Unity does not rebuild a `MeshCollider` when its mesh changes
without being assigned again. If it holds, only the drawn surface differs from the collider, along the
first row of cells of a marked side. If it does not, the collider takes the edge triangles, and the stock
rounding along that row.

Either way, it should not matter to a landed craft. A landed craft only gets physics within 200 m of the
active craft: it is loaded at 2 250 m, but held in place without physics ("packed") until 200 m
(`VesselRanges` in `Physics.cfg`). The highest level reaches much further. A quad is built at that level
when its distance to the active craft, counted along the ground and multiplied by 1.3
(`PQ.UpdateTargetRelativity`), plus the height of the craft above the ground, falls below the threshold
given in [Only where a craft can stand](../the-fix-ground.md#only-where-a-craft-can-stand):
at ground level, the zone reaches about 4.8 km around the craft on the Mun and 7.2 km on Kerbin. It
follows the active craft, so a landed craft that gets physics is always well inside it.

The exception is a craft that lands while another one is active. In flight, a craft gets physics from
2 000 m and keeps it up to 25 000 m, so a dropped stage, or a capsule under parachutes while the player
flies something else, can touch down on a quad at the edge of the zone:

- with the collider left corrected, it rests on the corrected ground, and only looks a little above or
  below the drawn one until the zone comes over it;
- with a collider that took the edge triangles, it rests on the stock rounding, as it does in stock,
  and is saved there; the next time the player comes close, the ground under it is corrected, and it
  moves once by that rounding, as in [Existing saves](existing-saves.md): a few centimetres on Kerbin,
  up to about two metres on Earth under Real Solar System.

Beyond the zone, stock gives such a craft no terrain collider at all.

### The corners of the zone

Where the corrected zone turns inwards, a coarser quad meets two corrected quads of the same level, and
their common corner cannot be at the stock position and at the corrected one at once. A small crack,
one cell long, would stay there, in place of a step along the whole edge.

### To check, if this solution is taken

- First, since it costs almost nothing and decides between A and moving the vertices: that changing the
  mesh of a quad leaves its collider as it is — a raycast onto the quad, before and after.
- That no mod beyond those read here writes the mesh of a quad at other times.
- The reach of the zone on the smallest stock body, Gilly, and on the bodies of a planet pack; with the
  lower terrain presets, since the thresholds come from the player's terrain settings.
- A craft landing near the edge of the zone while another one is active.
- The cracks at the corners of the zone, and the cost of rewriting the mesh.

## Parallax

Parallax draws the terrain with shaders of its own, on the meshes stock builds. Read in its source, it
should neither create the step nor hide it. Its tessellation computes the factor of each triangle edge
from the two ends of that edge, which only avoids cracks where both sides share those ends, and it stops
a short distance from the camera. Its displacement samples a texture in world coordinates: two vertices
on the same point move the same way, two vertices apart stay apart. Not measured.

## Still to test

- **The bodies of stock KSP, with this mod**: on Kerbin without it, one load left a largest gap of
  102 mm and a crack that shows along the seam
  ([Terrain Precision Fix Diag 4, case 2](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag4/blob/master/docs/the-measurements.md#case-2-stock-ksp));
  the same measurements with this mod installed, over several loads, are still to take.
  The gap should be smaller there, since a float's step is.
- **Why the shared vertices do not meet in stock**: which builds, which shifts of the world origin, put
  the two quads of a seam in different frames.
- **A floating origin shift**: the gap changes when the origin of the world moves, which Terrain
  Precision Fix Diag 4 logs at every shift; not measured in a series yet.
