# Tilt'Em: a tilted body

Part of [Terrain Precision Fix](../../../README.md), one point of [Work in progress](../../work-in-progress.md), on [Tilt'Em](../../work-in-progress.md#tiltem).

**Status: planned — read in the source, not measured.** [Tilt'Em Continued](https://github.com/ballisticfox/TiltEm-Continued)
gives the planets an axial tilt. To do so it rewrites the rotation of every body, one of the two values
this fix places the terrain from, and it takes over the stock method that sets it, which this fix also
patches. A fix that reads that rotation could be expected to disagree with a mod that writes it.

Read in the source (`TiltEm/Harmony/Frames/CelestialBody_CBUpdate.cs`), and reassuring so far. Tilt'Em
replaces `CelestialBody.CBUpdate` with a prefix that skips the stock method. Every frame, it writes
`body.rotation`, in double, and copies it into `bodyTransform.rotation`, in float — the same pair stock
keeps, written the same way — so this fix reads whatever rotation Tilt'Em has set, tilt included, like
the rest of the game. It has no terrain code. The position of the bodies still comes from the stock
orbit code, which Tilt'Em feeds with its tilted frame, and reaches the terrain sphere the stock way.

The statics fix follows the rotation of a body with a postfix on that same `CelestialBody.CBUpdate`.
Harmony still runs a postfix when a prefix skips the original method, so a static taken out of its
sphere should keep following a tilted body.

What the source does not say is whether the ground follows a tilted body when a craft comes down to the
altitude where the world starts turning with it, a switch that Tilt'Em handles with an anchor of its
own.

*To test:* the loading campaign of KSP Diag - Landed Vessel and Diag TerrainHeight on a tilted Kerbin,
with Tilt'Em and Kopernicus installed, with and without this fix; then, with this fix, a descent from
orbit down to a craft landed near the KSC.
