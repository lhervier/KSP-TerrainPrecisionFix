# Parallax: its terrain and its scatter

Part of [Terrain Precision Fix](../../../README.md), one point of [Work in progress](../../work-in-progress.md), on [Parallax](../../work-in-progress.md#parallax).

**Status: planned — read in the source, not measured.** [Parallax Continued](https://github.com/Gameslinx/Parallax-Continued)
draws the terrain with shaders of its own and replaces its scatter with its own objects, two things that
sit right on the ground this fix places.

Read in its source, and reassuring so far:

- **the terrain** — Parallax draws it on the meshes stock builds. Its tessellation and its displacement
  should neither create nor hide the crack stock already has where two subdivision levels meet, and
  that this fix widens
  ([The seam between subdivision levels](../../limits-and-solutions/stock/the-seam-between-subdivision-levels.md#parallax));
- **its scatter** — everything about its objects is expressed in the frame of the terrain quad: where
  they are drawn from (`quad.meshRenderer.localToWorldMatrix`), the matrix they are drawn through, and
  the colliders it can give them, which are children of the quad and placed in the quad's frame. So what
  it draws and what a craft hits share the ground's frame, and follow the ground wherever this fix places
  it: neither the gap of the stock scatter nor the scatter fix reaches them, and Parallax never names a
  stock holder
  ([Other mods that look for the holders](../../the-fix-scatter.md#other-mods-that-look-for-the-holders));
- **the stock scatter** — Parallax leaves the stock `LandControl` in place on every body but Eeloo, so
  on a body it does not strip, the stock scatter is still there, hanging from the same holders, with the
  gap this fix widens on Kerbin
  ([Rocks, grass and trees](../../limits-and-solutions/stock/rocks-grass-and-trees.md)), and the scatter
  fix acts on it as without Parallax.

One detail this fix leaves exactly as stock has it: Parallax samples its distribution noise from
directions computed in float out of 600 km vectors, so an object sitting right at the cutoff can appear
on one load and not on the next, with or without this fix.

*To test:* the loading and approach campaigns of KSP Diag - Landed Vessel and Diag TerrainHeight, with
Parallax installed, with and without this fix; and, for its scatter, that an object stands where it
did on the ground at every load, without this fix and with it. Parallax's scatter is not the stock one,
so it needs an instrument or a protocol of its own, still to design.
