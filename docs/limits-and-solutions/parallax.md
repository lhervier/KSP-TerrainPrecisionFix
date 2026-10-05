# Parallax

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: TBD — read in the source, not measured.** [Parallax Continued](https://github.com/Gameslinx/Parallax-Continued)
draws the terrain with shaders of its own and replaces its scatter with its own objects, two things that
sit right on the ground this fix places.

Read in its source, and reassuring so far:

- **the terrain** — Parallax draws it on the meshes stock builds. Its tessellation and its displacement
  should neither create nor hide the crack stock already has where two subdivision levels meet, and
  that this fix widens
  ([The seam between subdivision levels](../non-regression/the-seam-between-subdivision-levels.md#parallax));
- **its scatter** — everything about its objects is expressed in the frame of the terrain quad: where
  they are drawn from, the matrix they are drawn through, and the colliders it can give them, which are
  children of the quad. So they follow the ground wherever this fix places it
  ([Rocks, grass and trees](../non-regression/rocks-grass-and-trees.md));
- **the stock scatter** — Parallax leaves the stock `LandControl` in place on every body but Eeloo, so
  the stock scatter is still there, with the gap this fix widens on Kerbin
  ([Rocks, grass and trees](../non-regression/rocks-grass-and-trees.md)).

One detail this fix leaves exactly as stock has it: Parallax samples its distribution noise from
directions computed in float out of 600 km vectors, so an object sitting right at the cutoff can appear
on one load and not on the next, with or without this fix.

*To test:* the loading and approach campaigns of KSP Diag - Landed Vessel and Diag TerrainHeight, with
Parallax installed, with and without this fix; and, for its scatter, that an object stands where it
did on the ground at every load, without this fix and with it. Parallax's scatter is not the stock one,
so it needs an instrument or a protocol of its own, still to design.
