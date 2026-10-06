# The seam between subdivision levels

Part of [Terrain Precision Fix](../../../README.md), one point of [Work in progress](../../work-in-progress.md), on [stock](../../work-in-progress.md#stock).

**Status: planned.** Where a quad of the highest subdivision level meets a coarser one, the vertices
the two are supposed to share are already apart in stock, and this mod widens the crack: measured on
Earth under Real Solar System and on Kerbin, in
[The seam between subdivision levels](../../limits-and-solutions/stock/the-seam-between-subdivision-levels.md).
Still open:

- **The other bodies of stock KSP, with this mod**: Kerbin is measured (see
  [The seam with this mod](../../limits-and-solutions/stock/the-seam-between-subdivision-levels.md#the-seam-with-this-mod));
  the Mun, Minmus and the others are not. The gap should be smaller there, since a float's step is.
- **Why the shared vertices do not meet in stock**: which builds, which shifts of the world origin, put
  the two quads of a seam in different frames.
- **A floating origin shift**: the gap changes when the origin of the world moves, which KSP Diag -
  Terrain Quads logs at every shift; not measured in a series yet.
