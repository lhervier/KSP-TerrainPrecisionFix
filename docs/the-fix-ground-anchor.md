# The fix: the ground anchor

Part of [Terrain Precision Fix](../README.md): what this mod does to the ground anchor and to the loading of an anchored vessel, what it leaves alone, and what removing it does.

Two stock behaviours raise an anchored vessel when it is loaded, and the anchor holds it where they left
it; a third moves it by under a millimetre ([The culprit: the ground anchor](the-culprit-ground-anchor.md)).
This mod fixes each one on its own, with its own setting, all three on by default: `fixGroundAnchorModel`,
`fixGroundAnchorLoad` and `fixGroundAnchorRivet`. All three are in
[`Src/GroundAnchorFix.cs`](../Src/GroundAnchorFix.cs), and all three touch the stock ground anchor only.

## The anchor's model

Once KSP has loaded its parts (`GameEvents.OnPartLoaderLoaded`), the collider of the ground anchor, on the
part's prefab, is brought down to the bottom of the anchor: the vertices of its bottom face, those within
a millimetre of its lowest, 76 of its 241, are moved from 20.8 mm above the origin to the origin, on a
copy of the mesh. KSP makes every mesh collider of a part convex, so the collider then wraps the body of
the anchor down to its bottom, as the colliders of the stock ground lights do. Every anchor created from
then on has it.

The anchor then rests with its origin on the ground when it is placed, and
`Vessel.CheckGroundCollision` puts it back there: the lowest point of its colliders is its origin, and no
absolute value changes a zero. Placed, its base no longer sinks 2 cm into the ground; its screws still
go in.

No method is patched for this, and nothing is written to a save. An anchor whose colliders are not those
of the stock part, remodelled by another mod, is left as it is, and the log says so. When the collider is
lowered, the log says, once at startup:

```
[TerrainPrecisionFix] Ground anchor model fix: the anchor's collider lowered by 20.8 mm to its origin
```

## The load

A Harmony prefix on `Vessel.getCorrectedLandedAltitude`, which `Vessel.Load` calls once on a landed
vessel, after its parts are loaded: when one of the vessel's parts is a stock ground anchor, its root or
not, the method returns the altitude saved, unchanged. The vessel is loaded where it was saved, on the
ground it was saved on, and its anchor rivets it there.

At `logLevel = Debug`, the log says it for each anchored vessel loaded, with what stock would have done:

```
[TerrainPrecisionFix] [DEBUG] Ground anchor load fix: 'Stamp-O-Tron Ground Anchor' loaded at its saved altitude 152.3134 m, which stock would have raised by 273.9 mm
```

KSP also loads the part an engineer places as a new vessel, so the line shows up at placement too. The
anchor then drops onto the ground as it does on stock.

## The rivet

A Harmony postfix on `ModuleGroundPart.OnPartUnpack`: when the part going off rails is a stock ground
anchor riveted when it was saved, not one being attached in EVA construction, and its kinematic delay is
under a second, the stock anchor's 0, the case where KSP rivets it at the next frame without waiting, its
rigidbody is frozen at once, with no speed: the constraints the rivet sets a frame later. No physics step
can move it in between, at any frame rate. KSP's coroutine then rivets it as on stock.

At `logLevel = Debug`, the log says it for each anchor:

```
[TerrainPrecisionFix] [DEBUG] Ground anchor rivet fix: 'Point d'ancrage Coll-O-Tron' frozen as it is unpacked, until it is riveted again
```

What it saves is small, under a millimetre per load, and only on a game running below 50 frames per
second, or at a frame that drags. It is on by default all the same, because without it an anchored base
moves at load depending on the frame rate: not at all on a fast computer, at every load on a slow one,
the same save. A base held in the air, as stock leaves one, only sinks, by that much at every load
([Checking the culprit: anchoring a base](checking-the-culprit-anchoring.md#below-50-frames-per-second)):
if parts of it rest on legs or wheels, the anchor riveted a little lower each time presses the base onto
them. With it, the base stays where it was loaded, at any frame rate. It touches nothing else: an anchor
that KSP has to let settle before riveting it, one just placed, is left alone.

How many physics steps a slow frame lets through is capped by KSP's own setting, *Max Physics Delta-Time
per Frame* (`PHYSICS_FRAME_DT_LIMIT` in KSP's `settings.cfg`): at its default, 0.04 s, two steps of 0.02 s
at most, whatever the frame rate; raised, more.

## Only for anchored vessels

Raising a vessel to the height KSP computes is wrong for any vessel whose origin rests on a collider
below that height. But a craft on wheels or legs is never raised, its origin being well above the
ground; and a craft that is raised falls back when physics starts. Only the anchor, riveted where the
load left it, keeps the error. So only a vessel holding a stock ground anchor is loaded without it; the
rest of the fleet is raised as stock raises it. Where the collider is above the computed height,
`Math.Max` raises nothing, on stock as with this mod.

## A base already in the air

A base that stock raised into the air and that was saved there stays there. Its saved altitude is in the
air, this mod loads it there, and, being made of several parts, it no longer goes through the pass of
`CheckGroundCollision` that would set it down. This mod does not bring it back down, at any frame rate:
without [the rivet](#the-rivet), a slow game would let it sink a little at every load.

## Removing this mod

An anchor placed with this mod rests with its origin on the ground. Loaded without this mod, stock raises
it as it raises any anchor. An anchor alone is put back 20.8 mm above the ground at every load. A base
is raised to the height KSP computes at its first load without this mod, and stays there: its save
already holds the subdivision levels that make the game skip the pass. That is the situation
[the second load with the model fix alone](checking-the-culprit-anchoring.md#the-anchor-with-a-battery)
shows: the base 27.4 cm above the ground, `fixGroundAnchorLoad` being the setting that prevents it.

## Safeguards

- each setting turns its part of the fix off, or on, on its own;
- if the patch cannot be installed, vessels are loaded as stock loads them, and the log says so;
- the collider is replaced only once the new one is complete, and only on an anchor shaped like the
  stock one;
- the patch of the load acts only on a vessel holding a stock ground anchor, the patch of the rivet only on
  a stock ground anchor that KSP is about to rivet again without waiting.
