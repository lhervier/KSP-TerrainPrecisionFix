# The fix: the scatter

Part of [Terrain Precision Fix](../README.md): what the patches do for the terrain scatter, where they do it, what they leave alone, and why they are off by default.

The objects of a quad's scatter hang from a holder that is placed and drawn from a 600 km float, and
that does not share the correction the terrain fix gives the quad
([The culprit: the scatter](the-culprit-scatter.md)). So this mod hangs each holder from its own quad.

The scatter fix is **off by default**. `fixScatter = true` in the settings turns it on; it is installed
when KSP starts, whether the terrain fix is on or not. It is two Harmony patches, in
[`Src/ScatterFix.cs`](../Src/ScatterFix.cs). Why it is off is in
[Off by default: should you turn it on?](#off-by-default-should-you-turn-it-on)

## Hanging each holder from its quad

Each holder hangs from its own quad, at no offset. The objects are then drawn in the frame they were
built in, with the very matrix the ground is drawn with, and no transform in the chain holds a 600 km
vector any more.

- **After `PQSMod_LandClassScatterQuad.Setup`**, which gives a holder its quad: the holder is re-parented
  under the quad, with zero position, identity rotation and unit scale. From then on it follows the quad
  through everything stock does to it, floating origin shifts (`PQ.FastUpdateSubQuadsPosition`) and
  re-placements (`PQ.PreciseUpdateSubQuadsPosition`) included, with nothing more to do
  ([measured along a flight](checking-the-culprit-flight.md#the-rocks-along-a-flight)).
- **Before `PQSLandControl.LandClassScatter.DestroyQuad`**, which returns a holder to its pool when its
  quad is destroyed: the holder goes back under the pool's container, where stock keeps its free holders,
  before the quad itself goes back to the PQS cache to be reused elsewhere. A prefix, because the stock
  method starts by clearing the holder's quad.

This is not a new way of drawing scatter. It is the stock scatter, drawn from the frame stock built it in.

## Why the holder has to move at all

The fix that would leave every other mod alone is the one that keeps the holder under its `Scatter <name>`
container and merely places it better. Reading the stock code, and the way Unity stores a transform, that
way looks closed. Three reasons, from the plainest to the heaviest:

- **The correction is smaller than the step of the number that would carry it.** Unity keeps a child's
  position in its parent's frame, in single precision, whichever call writes it: `position`,
  `localPosition` and `SetPositionAndRotation` all end in that same float. Under the sphere, that number is
  the 600 km vector, where a float changes in steps of 62.5 mm on Kerbin, while the offsets to be taken out
  are the ones measured in
  [Checking the culprit: loading the same save](checking-the-culprit-loading.md#the-scatter-over-twelve-loads),
  up to 110 mm. No value that can be written puts the holder where it belongs.
- **The frame it hangs in is single precision as well.** Even given a perfect local position, the holder is
  drawn through the sphere's matrix, over that same 600 km. That product is what rounds differently at
  every load: it is the defect itself, not a way around it.
- **Out of the sphere, the holder has to be placed by hand before every frame.** Placing it in double
  precision needs a parent near the world origin, and stock then stops carrying it: the world position of a
  quad of the highest level is rewritten whenever the body moves in the game's local space, which in flight
  is every frame, and its rotation whenever stock places the quad again. The holder would have to follow
  both — its objects are built in the quad's frame, so its rotation counts as much as its position — and a
  frame missed would leave them not centimetres but metres behind the ground.

**This is read, not measured.** No build of the scatter fix has tried that way, and nothing else on this
page rests on it: the measurements stand on their own. It is written down because it is the first question
to ask of a fix that moves stock objects, and a reader who sees a way through should say so.

What is left is to take the holder out of the sphere, as the scatter fix does, or to stop drawing the
scatter from the holder's transform altogether and draw it from somewhere else, which changes far more
than where an object hangs. Of the two, hanging the holder from its quad is the smaller change: the quad
is the one object stock already keeps in step, for nothing, with the ground the scatter is built from.
Parallax reaches the same conclusion for its own scatter: the colliders it creates hang from the quad
they belong to, in the quad's frame, so that the object a craft hits is the object the quad draws.

## Only where scatter grows

Only the holders of quads that hang from `LocalSpacePQStorage` are moved. In stock, scatter only exists
on those quads, and any other quad hangs from the sphere with the same kind of 600 km local position as
the holder, so hanging the holder from it would gain nothing.

Holders on a sphere whose quads are not surface relative are left alone too: there, stock places the
holder at the centre of the body, not at a long vector.

## Why moving the holder is safe for stock

Read in the stock code:

- a destroyed quad calls its `onDestroy` delegates, which release its holder, before it goes to the PQS
  cache, so no holder travels with a recycled quad;
- `PQS.ResetSphere` destroys the quads before the scatter destroys its holders;
- the visibility of a holder is switched with `obj.SetActive` from the quad's `onVisible` and
  `onInvisible` delegates, not through the hierarchy, so hanging it from the quad does not change when it
  is shown.

Checked in game as well, over a whole flight and across scene switches: no holder is lost, and every pool
works as in stock ([Non-regression tests: the scatter holders](non-regression/stock/the-scatter-holders.md)).

## Other mods that look for the holders

Safe for stock is not safe for every mod. What the fix changes for other mods is one thing: where a
holder hangs. Its pool (`cacheAssigned`, `cacheUnassigned`), its `quad`, its delegates and the way it is
shown and hidden stay stock. But a holder in use no longer hangs from the `Scatter <name>` container
under the sphere: it hangs from its quad, under `LocalSpacePQStorage`, outside the body's hierarchy. Code
that finds holders through the hierarchy sees the difference both ways:

- looking under the sphere, `sphere.GetComponentsInChildren<PQSMod_LandClassScatterQuad>()` no longer
  finds the holders in use;
- looking under a quad, it finds children that stock never puts there.

This is not only a hypothesis: [KSP Diag - Scatter](https://github.com/lhervier/KSP-Diag-Scatter)
finds the holders through the hierarchy, and has to search both places to see them with the scatter fix
on.

Only one thing decides whether a piece of code sees the move: whether it goes looking for the holders.
What has been read, mod by mod:

- **Stock** relies on neither the hierarchy nor where a holder hangs:
  [The scatter holders](non-regression/stock/the-scatter-holders.md).
- **KSP Community Fixes** never names a holder:
  [its own terrain patches](non-regression/ksp-community-fixes/its-own-terrain-patches.md).
- **Kopernicus**, the only mod read here that uses the holders, reaches them through the scatter and its
  pool, not through the hierarchy, and its optional scatter colliders are the ones this fix brings back
  onto their objects: [Kopernicus: scatter with colliders](non-regression/kopernicus/scatter-with-colliders.md).
- **Parallax** has its own scatter system and never names the stock holders:
  [Parallax: its terrain and its scatter](work-in-progress/parallax/its-terrain-and-its-scatter.md).
- **TUFX** (1.1.1), read in its source, works on the camera's image, not on the scene: it has no Harmony
  patch and never looks for a terrain object. Its only walk through the scene lists the objects of the
  main menu into its debug log.

Every other mod is unread:
[Mods that look for a scatter holder under its sphere](work-in-progress/other-mods/mods-that-look-for-a-scatter-holder-under-its-sphere.md).

## Off by default: should you turn it on?

On one side, on a stock install, a defect nobody sees: stock scatter has no collider, and is sunk into the
ground on purpose (see
[Why the moving scatter matters](limits-and-solutions/stock/rocks-grass-and-trees.md#why-the-moving-scatter-matters)).
On the other, a change to where stock objects hang, which any mod installed along with it may rely on. Its
cost weighs on neither side: none shows in the measurement (see
[Performance: the scatter fix](performance.md#the-scatter-fix)).

What tips the first side is another mod. Give the scatter colliders — the
[Stock Scatter Collider Enabler Patch](https://github.com/Poodmund/Stock-Scatter-Collider-Enabler-Patch)
does, on top of Kopernicus, and it is on CKAN — and the defect stops being invisible: the rock a craft
hits stands up to 104 mm from the rock its pilot sees, and a kerbal left on a boulder sinks into it at one
load and stands clear of it at the next. Only the scatter fix closes that gap; the terrain fix halves it
(see [Kopernicus: scatter with colliders](non-regression/kopernicus/scatter-with-colliders.md)).
So the answer below is not the same for everyone: it holds for a stock install, and a player whose mods
give the scatter colliders is weighing something else.

And it is not a change that can be traded away: nothing in
[Why the holder has to move at all](#why-the-holder-has-to-move-at-all) leaves a fix that keeps the
holders where stock puts them.

So, should you turn it on? On a stock install, no, and that is why it is off by default. Nobody turning
it on there would remember that the holders moved, and a mod tripping over it would fail with nothing to
point at this fix — a bad trade against a defect that changes nothing in play. With a mod that gives the
scatter colliders, the trade is yours to make: the gap it closes is then a real one, between the rock
your craft hits and the rock you see, and no other fix closes it.

Either way, the scatter fix stays what it is: the measured answer to what the terrain fix leaves behind,
and the proof that the culprit is the right one.

It is meant to go with the terrain fix, and has only been measured with it: on its own, it would keep the
scatter on the stock ground, which still comes back at a different height at every load
([The scatter fix without the terrain fix](work-in-progress/stock/the-scatter-fix-without-the-terrain-fix.md)).

## If it went into KSP Community Fixes

Then the re-parenting stops being a technical risk and becomes a matter of agreement between modders. A
KSP Community Fixes patch is documented, and can be turned off by a line of its `Settings.cfg`: the move
would be known, and a player hit by it could undo it without removing anything else.

But it would still be a change that other mods have to follow. A mod that finds holders through the
hierarchy would have to change its code: search under the terrain quads as well, as KSP Diag - Scatter
does, or read the holders from their scatter's pool (`LandClassScatter.cacheAssigned`, a private
list), which does not depend on where they hang. Whether that is worth asking of other modders is not a
technical question, and this page does not answer it. What can be said is what is being asked for: not
only a defect nobody sees, but also the one players already meet when their mods give the scatter
colliders, where the rock hit is not the rock seen.

## Safeguards

- if either patch fails to install, no holder is ever moved;
- a holder that cannot be hung from its quad stays where stock placed it;
- the release only acts on a holder that hangs from its own quad, so it never moves a holder the fix did
  not move;
- with Rock Precision Fix installed, the mod this fix was first published as, the scatter fix is not
  installed and the log says so: both would hang every holder twice.
