# The culprit: the ground anchor

Part of [Terrain Precision Fix](../README.md): the two stock behaviours that raise a vessel held by a ground anchor when it is loaded, and why the anchor then keeps it in the air.

Here they are straight away. Neither has to do with the precision of the ground: both are there on any
ground, moving or not.

The ground anchor, the Stamp-O-Tron of EVA construction, is riveted to the ground once it is down:
`ModuleGroundPart` makes it kinematic and frozen, and holds everything attached to it with it. That is
what it is for, and it is also why these two behaviours matter: whatever moves an anchored vessel when it
is loaded, before the anchor is riveted, stays. An anchored vessel never falls back.

## The anchor's model

`Vessel.CheckGroundCollision` puts a landed vessel back on the ground when it goes off rails. It measures
how far the vessel's origin stands above the lowest point of its colliders, and sets the origin that far
above the ground. KSP 1.12.5:

```csharp
float num4 = Mathf.Abs(getLowestPoint());
```

For an ordinary vessel, the lowest point of its colliders is below its origin, and the absolute value
changes nothing. The ground anchor's origin is the bottom of the anchor, and its model reaches down to it;
its collider does not. In `groundAnchor.mu`, the body of the anchor goes from 0 to 149.6 mm above the
origin, its collider from 20.8 to 149.6 mm, and its four screws have no collider. So `getLowestPoint`
returns −20.8 mm, the absolute value turns it into +20.8 mm, and the anchor's origin is set 20.8 mm
**above** the ground. Once placed, the anchor rests on its collider, its origin 20.8 mm **below** the
ground: the pass puts it 4.2 cm higher than it was placed. The other stock parts built on
`ModuleGroundPart`, the two ground lights and the deployed experiments of Breaking Ground, have the bottom
of their collider at their origin, below it, or 1 mm above it at most.

When the pass runs:

- **at every load, for a vessel of a single part**: an anchor alone is put back 20.8 mm above the ground
  each time, wherever it was saved;
- **at the first load only, for a vessel of several parts**: a part placed in EVA construction is saved
  without the subdivision levels of the ground under it, and once a load has written them,
  `Vessel.GoOffRails` skips the pass for good (`skipGroundPositioning`).

The pass leaves a vessel where it is when the correction is under 10 cm, but not when its root part
carries a `ModuleGroundPart`: an anchor at the root takes the 4.2 cm.

## The height KSP computes

`Vessel.Load` raises a landed vessel whose origin is below the height the game computes for the terrain.
KSP 1.12.5, `Vessel.getCorrectedLandedAltitude`:

```csharp
double val = body.pqsController.GetSurfaceHeight(relSurfaceNVector, overrideQuadBuildCheck: true) - body.Radius;
return Math.Max(alt, val);
```

The ground a vessel rests on is not that height. It is the collider of the terrain: flat triangles
stretched between vertices placed at that height, which pass under the computed relief between them, or
over it. [KSP Diag - Terrain Height](https://github.com/lhervier/KSP-Diag-TerrainHeight) reads both
under a craft. Where the anchor is checked, on the desert of Kerbin, the collider is 27.3 cm below the
height KSP computes ([Checking the culprit: anchoring a base](checking-the-culprit-anchoring.md)).

A craft on wheels or legs has its origin well above the ground, and is not raised. An ordinary craft that
is raised falls back when physics starts. The ground anchor has its origin on the ground: it is raised by
the whole gap, and riveted there.

- **An anchor alone** is then put back down by the pass of `CheckGroundCollision`, which runs at every
  load for a vessel of one part, to its 20.8 mm above the ground.
- **A base of several parts** is put back down at its first load only. From the second load on, the pass
  is skipped and nothing puts it down: the base stays at the height KSP computes, held in the air by its
  anchor, for good. It goes no higher at the next loads: it is already at that height.

## What the player sees

An anchor alone comes back 4 cm higher than it was placed, its screws out of the ground, at its first
load, or on coming back from the tracking station, or out of time warp — at its first time off rails.
That is what KSP Community Fixes' issue [#214](https://github.com/KSPModdingLibs/KSPCommunityFixes/issues/214),
*Ground anchor levitates after a scene load*, reports, with an anchor alone.

A base on an anchor comes back 4 cm higher at its first load, then, at the second, as high as the
computed terrain: from nothing to tens of centimetres, depending on where it stands, with no way back.
It looks very much like the issue, only higher.

| | |
|---|---|
| ![On stock, an anchor alone at its second load](../imgs/anchor/no-fix/050-quicksave-and-reload-again.png) | ![On stock, an anchor with a battery at its second load](../imgs/anchor/no-fix/150-quicksave-and-reload-again.png) |
| an anchor alone, placed by Bill Kerman on the desert of Kerbin, then saved and loaded twice | an anchor with one of the rover's batteries on top, placed and loaded the same way |

On stock, KSP 1.12.5 with KSP Community Fixes. The protocol and the readings are in
[Checking the culprit: anchoring a base](checking-the-culprit-anchoring.md).

## With the terrain fix

The terrain fix changes neither: the ground no longer moves, and the anchor and the base are raised all
the same. Without it, the ground itself comes back higher or lower at each load
([The culprit: the ground](the-culprit-ground.md)): an anchor alone follows it, 20.8 mm above it every
time, and the gap between the collider and the computed height changes with it.

Each behaviour has its own fix in this mod, in [The fix: the ground anchor](the-fix-ground-anchor.md).
Both are checked in [Checking the culprit: anchoring a base](checking-the-culprit-anchoring.md).
