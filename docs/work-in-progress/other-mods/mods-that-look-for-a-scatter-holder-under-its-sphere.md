# Mods that look for a scatter holder under its sphere

Part of [Terrain Precision Fix](../../../README.md), one point of [Work in progress](../../work-in-progress.md), on [other mods](../../work-in-progress.md#other-mods).

**Status: planned — only with the scatter fix on; four mods read, the others unknown.** The scatter fix,
off by default, hangs each holder of terrain scatter in use from its quad, outside the body's hierarchy,
instead of the `Scatter <name>` container under the terrain sphere where stock puts it
([The fix: the scatter](../../the-fix-scatter.md)). Any mod that finds the holders through the hierarchy
sees the difference both ways: under the sphere, it no longer finds the holders in use; under a quad, it
finds children stock never puts there. This is not only a hypothesis:
[KSP Diag - Scatter](https://github.com/lhervier/KSP-Diag-Scatter) finds the holders through the
hierarchy, and has to search both places to see them with the scatter fix on.

What limits the exposure: the scatter fix is off by default, and is meant only for an install where a mod
gives the scatter colliders.

**Read.** Stock, KSP Community Fixes, Kopernicus, Parallax and TUFX: none of them finds a holder through
the hierarchy
([Other mods that look for the holders](../../the-fix-scatter.md#other-mods-that-look-for-the-holders)).

*To test:* every other mod — visual mods such as Scatterer or EVE, planet pack plugins, anything that
walks the terrain's hierarchy. Such a mod would miss the holders where stock puts them, or find
unexpected children under its quads. Nothing says one does. Nothing says none does, and that can only be
checked one mod at a time: read its source for `PQSMod_LandClassScatterQuad`, or for a walk through the
children of a terrain sphere or of a quad.
