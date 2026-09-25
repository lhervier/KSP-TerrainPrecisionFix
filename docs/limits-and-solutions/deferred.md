# Deferred

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: TBD.** Deferred replaces KSP's rendering path, and has
no reason to care where a quad is placed. It is here because it is the mod that broke KSP Community
Fixes' own `PQSOnlyStartOnce`: terrain stopped loading for some players, and that patch has been
disabled by default since. Anything touching the terrain spheres deserves the same check.

*To test:* Deferred plus this fix, the campaign of Terrain Precision Fix Diag 2, and whether the terrain
still renders.
