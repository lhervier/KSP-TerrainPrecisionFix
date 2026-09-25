# Rocks, grass and trees

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: checked, no problem — this mod does not move them, but widens a stock defect on Kerbin.** Terrain scatter — the rocks, and
around the KSC the grass and the trees — is already drawn off the ground in stock, differently at every
load. This mod does not move it, so it does not create that gap, but it does not share the ground's
correction either, and on Kerbin the gap gets wider.

The objects of a quad are built from its vertices, in the quad's own coordinates, and hang from a
*holder* that `PQSMod_LandClassScatterQuad.Setup` places under the terrain sphere, at
`localPosition = quad.positionPlanet`: the same 600 km vector in a float that
[the culprit](../the-culprit.md) is about. The holder
is drawn with its local to world matrix, whose translation differs from its own transform position by
whole float steps, so the objects are drawn that much above or below the ground. On stock the two land on
the same step often enough that the holder is drawn exactly on its quad about a quarter of the time on
Kerbin, and within a tenth of a millimetre of it three times out of four on the Mun. With this mod they
never do: the ground is now placed in double precision and the holder is not.

[Rock Precision Fix Diag](https://github.com/lhervier/KSP-RockPrecisionFixDiag) measures it, on two
saves of its own, one on Kerbin and one on the Mun, each loaded twelve times per series ([the readings](https://github.com/lhervier/KSP-RockPrecisionFixDiag/blob/main/docs/what-the-readings-show.md#the-rocks)):
the height of a measured point of an object above the ground under it comes back 94 mm apart over the
twelve loads for half of those points on Kerbin in stock, and 130 mm with this mod; on the Mun, 31 mm
either way. The same kind of error, of the same order and wider on Kerbin, but where stock draws part
of it from the ground moving and part from the holder, with this mod all of it comes from the holder. Stock scatter has no
collider, so this is visual only — unless a mod gives it one, which is
[Scatter with colliders](scatter-with-colliders.md).

**Solution.** [Rock Precision Fix](https://github.com/lhervier/KSP-RockPrecisionFix), a separate mod,
hangs each holder from its own terrain quad, so that the objects are drawn in the frame they were built
in. Measured on those same series, with this mod installed next to it: every holder is drawn exactly on
its quad, and no measured point of an object moves by more than 0.125 mm over the twelve loads on Kerbin,
0.042 mm on the Mun
([its measurements](https://github.com/lhervier/KSP-RockPrecisionFix/blob/main/docs/checking-the-culprit.md#rock-precision-fix-diag-with-this-mod)).
It works with or without this mod, and it is not a mod to install lightly: it moves stock objects, which
other mods may look for where stock puts them, and its page weighs that trade
([Should you install it?](https://github.com/lhervier/KSP-RockPrecisionFix/blob/main/docs/should-you-install-it.md)).

**Parallax's own scatter is out of this.** Read in the source of Parallax Continued, not measured:
everything about its objects is expressed in the frame of the terrain quad — where they are drawn from,
the matrix they are drawn through, and the colliders it can give them, which are children of the quad —
so they follow the ground wherever this fix places it. Rock Precision Fix reads the same source for its
own purpose and writes up what it found there
([What never touches them](https://github.com/lhervier/KSP-RockPrecisionFix/blob/main/docs/should-you-install-it.md#what-never-touches-them)).
Parallax also leaves the stock `LandControl` in place on every body but Eeloo, so on a body it does not
strip, the stock scatter this chapter is about is still there, hanging from the same holders.

One detail this fix leaves exactly as stock has it: Parallax samples its distribution noise from
directions computed in float out of those same 600 km vectors, so an object sitting right at the cutoff
can appear on one load and not on the next. That happens without this fix too, and the fix touches
neither side of it.
