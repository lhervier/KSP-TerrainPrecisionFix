# Kopernicus: scatter with colliders

Part of [Terrain Precision Fix](../../../README.md), one test of [Non-regression tests](../../non-regression.md), on [Kopernicus](../../non-regression.md#kopernicus).

**Status: checked, no problem — the terrain fix halves a stock gap, and does not close it; the scatter
fix of this mod, off by default, closes it.** Stock scatter has no collider, but Kopernicus can give it one: Kopernicus with the
[Stock Scatter Collider Enabler Patch](https://github.com/Poodmund/Stock-Scatter-Collider-Enabler-Patch),
both on CKAN, do. The offset described in [The culprit: the scatter](../../the-culprit-scatter.md) then
stops being visual: the physics engine is handed the holder's position, while the pilot sees what is
drawn from the holder's matrix, and those are the two numbers that round differently. The rock a craft
hits is not the rock its pilot sees. Kopernicus replaces the stock scatter holder with its own subclass,
`PQSMod_KopernicusLandClassScatterQuad`, which is the one measured here.

## The gap between a rock and its collider

[KSP Diag - Scatter](https://github.com/lhervier/KSP-Diag-Scatter) measures, object by object, where the
physics engine holds the collider of an object of scatter against where the object is drawn, following
[its protocol](https://github.com/lhervier/KSP-Diag-Scatter/blob/main/docs/measuring-the-rocks.md#with-colliders-on-the-scatter),
in KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1, Kopernicus 1.12.1.247 and the Stock
Scatter Collider Enabler Patch 1.0.1: one save, a kerbal standing on a boulder in a desert of Kerbin,
loaded six times per configuration, one record and one picture of the kerbal's feet per load. Six objects
carry a collider on the quad read: one `boulder` and five `cactus`.

Three configurations: on stock, with the terrain fix alone — this mod as installed by default — and with
the scatter fix as well, off by default. **The series with the scatter fix was taken when it was a mod of
its own**, Rock Precision Fix 0.1.0, installed next to Terrain Precision Fix 0.1.0: the same two patches,
whose lines in its logs are tagged `[RockPrecisionFix]`. The records taken with both fixes are in
[`diag/runs`](../../../diag/README.md#the-scatter-fix-the-colliders); those taken without the scatter fix
belong to [the instrument](https://github.com/lhervier/KSP-Diag-Scatter/blob/main/docs/what-the-readings-show.md#the-colliders),
which this table quotes.

| the collider against the object drawn, over 36 readings | on stock | with the terrain fix alone | with both fixes |
|---|---|---|---|
| *up* | −68.7 to +104.2 mm | −70.2 to +70.3 mm | −0.026 to +0.022 mm |
| *up*, root mean square | 48.1 mm | 31.6 mm | 0.010 mm |
| *across*, at most | 83.3 mm | 44.5 mm | 0.058 mm |

**With the terrain fix alone, the gap is halved, not closed.** The collider and the object stay
centimetres apart, drawn afresh at every load: the terrain fix leaves the holder hanging from the sphere,
over the 600 km vector the culprit is about.

**The gap does not follow the holder.** In one of the stock loads, the holder of those six objects sits
exactly on its quad, *up* and *across* 0.000 mm, and its colliders still stand up to 73 mm from the
objects they belong to. Whatever separates them happens below the holder, where each object carries its
own position under it — which is why hanging the holder from its quad settles it for the objects as well.

**And it is visible.** On the boulder the kerbal stands on, the gap reads −4.3, −48.1, +48.2, +10.2, −9.1
and +53.7 mm over the stock loads: its boots sink to the ankles at one load and stand clear of the rock at
the next. With both fixes, the six pictures are interchangeable. They are illustrations, not measurements:
the viewpoint is not exactly the same twice, and a kerbal sinks a little into whatever it stands on.

## What Kopernicus does with the holders

Read in its source. Kopernicus is the only mod read that uses the holders, and the scatter fix moves them
([Other mods that look for the holders](../../the-fix-scatter.md#other-mods-that-look-for-the-holders)).
It reaches them through the scatter (`scatterParent`, which the scatter fix reads at every call, and the
pool), not through the hierarchy, and reaches the objects of a holder through the holder's own children,
which follow it. Its lethal and heat emitting scatter computes world positions from the holder's matrix
and positions in the quad's frame, which the scatter fix makes exact. Its optional scatter colliders end
up below the quad: they are the ones measured above, and the fix brings each of them onto the object it
belongs to. Stock's `GetComponent<PQ>()` still finds no quad on them, which is what the instrument relies
on to tell the ground from a rock when it casts a ray at them.

## Solution

The scatter fix of this mod, off by default (`fixScatter = true` turns it on), and only with the terrain
fix as well: the same series reads −0.026 to +0.022 mm with both. The collider and the object are the
same object again, to a hundredth of a millimetre, at every load. Why it is off by default, and when to
turn it on: [Off by default: should you turn it on?](../../the-fix-scatter.md#off-by-default-should-you-turn-it-on)
