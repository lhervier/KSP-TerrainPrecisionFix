# Principia

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: TBD — read in the source, not measured.** [Principia](https://github.com/mockingbirdnest/Principia)
replaces KSP's orbital mechanics, and to do so it takes over the two values this fix places the terrain
from: the rotation and the position of every body. A fix that reads them could be expected to disagree
with a mod that writes them.

Read in the source (`ksp_plugin_adapter/ksp_plugin_adapter.cs`), and reassuring so far. Principia has no
Harmony patch and no terrain code. Every frame, `SetBodyFrames` writes `body.rotation`, in double, and
copies it into `bodyTransform.rotation`, in float — the same pair stock keeps, written the same way —
so this fix reads whatever rotation Principia has set, like the rest of the game. It moves the bodies
by writing `celestial.position` directly rather than through a floating origin shift; the stock setter
passes that on to `PQS.PrecisePosition`, which moves every quad by the same amount, in double, from the
position this fix gave it (`PQ.FastUpdateSubQuadsPosition`). The correction is carried along, not
rounded again.

What the source does not say is whether the quads are placed again often enough to follow a rotation
that Principia computes rather than KSP.

*To test:* the loading campaign of Terrain Precision Fix Diag 1 and Diag 2 on Kerbin, with Principia
installed, with and without this fix.
