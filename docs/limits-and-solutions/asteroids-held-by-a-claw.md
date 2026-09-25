# Asteroids held by a claw

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: TBD.** An asteroid is a vessel, positioned in double like any other, so on its own it should
follow the corrected ground; neither `ModuleAsteroid` nor `ModuleGrappleNode` reads the terrain. The
fragile case is an asteroid resting on the ground and grappled by a claw: on the first load with the fix,
the ground under it moves once, by up to the stock spread, with a mass on the other end of a joint.

*To test:* a clawed asteroid on the Mun, saved and reloaded several times, with and without the fix.
