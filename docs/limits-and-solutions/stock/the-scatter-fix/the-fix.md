# The scatter fix: the fix

Part of [Terrain Precision Fix](../../../../README.md), one page of the solution to
[Rocks, grass and trees](../rocks-grass-and-trees.md), in [Limits and solutions](../../../limits-and-solutions.md).

The scatter fix is off by default. `fixScatter = true` in the settings turns it on; it is installed when
KSP starts, whether the terrain fix is on or not. It is two Harmony patches, in
[`Src/ScatterFix.cs`](../../../../Src/ScatterFix.cs).

## Hanging each holder from its quad

Each holder hangs from its own quad, at no offset. The objects are then drawn in the frame they were
built in, with the very matrix the ground is drawn with, and no transform in the chain holds a 600 km
vector any more.

- **After `PQSMod_LandClassScatterQuad.Setup`**, which gives a holder its quad: the holder is re-parented
  under the quad, with zero position, identity rotation and unit scale. From then on it follows the quad
  through everything stock does to it, floating origin shifts (`PQ.FastUpdateSubQuadsPosition`) and
  re-placements (`PQ.PreciseUpdateSubQuadsPosition`) included, with nothing more to do
  ([measured along a flight](checking-the-culprit.md#ksp-diag---scatter-the-rocks-over-a-flight)).
- **Before `PQSLandControl.LandClassScatter.DestroyQuad`**, which returns a holder to its pool when its
  quad is destroyed: the holder goes back under the pool's container, where stock keeps its free holders,
  before the quad itself goes back to the PQS cache to be reused elsewhere. A prefix, because the stock
  method starts by clearing the holder's quad.

This is not a new way of drawing scatter. It is the stock scatter, drawn from the frame stock built it in.

## Only where scatter grows

Only the holders of quads that hang from `LocalSpacePQStorage` are moved. In stock, scatter only exists
on those quads, and any other quad hangs from the sphere with the same kind of 600 km local position as
the holder, so hanging the holder from it would gain nothing.

Holders on a sphere whose quads are not surface relative are left alone too: there, stock places the
holder at the centre of the body, not at a long vector.

## Why moving the holder is safe for stock

Read in the stock code:

- a destroyed quad calls its `onDestroy` delegates, which release its holder, before it goes to the PQS
  cache, so no holder travels with a recycled quad. Measured as well, over a whole flight: see
  [the holder pools over a flight](checking-the-culprit.md#ksp-diag---scatter-the-holder-pools-over-a-flight);
- `PQS.ResetSphere` destroys the quads before the scatter destroys its holders;
- the visibility of a holder is switched with `obj.SetActive` from the quad's `onVisible` and
  `onInvisible` delegates, not through the hierarchy, so hanging it from the quad does not change when it
  is shown.

Safe for stock is not safe for every mod: a mod that looks for the holders where stock puts them would
miss them. That is why the fix is off by default: [Should you turn it on?](should-you-turn-it-on.md)

## Safeguards

- if either patch fails to install, no holder is ever moved;
- a holder that cannot be hung from its quad stays where stock placed it;
- the release only acts on a holder that hangs from its own quad, so it never moves a holder the fix did
  not move;
- with Rock Precision Fix installed, the mod this fix was first published as, the scatter fix is not
  installed and the log says so: both would hang every holder twice.
