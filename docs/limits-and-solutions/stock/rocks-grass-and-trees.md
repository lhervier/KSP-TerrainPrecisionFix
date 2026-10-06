# Rocks, grass and trees

Part of [Terrain Precision Fix](../../../README.md), one limit of [Limits and solutions](../../limits-and-solutions.md), on [stock](../../limits-and-solutions.md#stock).

**Status: checked, a stock defect this mod widens on Kerbin, and a fix of its own that closes it, off by
default.** Terrain scatter — the rocks, and around the KSC the grass and the trees — is already drawn
off the ground in stock, differently at every load. The terrain fix does not move it, so it does not
create that gap, but the scatter does not share the ground's correction either, and on Kerbin the gap
gets wider. The scatter fix closes it (`fixScatter = true`), and is off by default: it moves stock
objects, which other mods may look for where stock puts them.

## Why the moving scatter matters

On a stock install, it barely does. Stock scatter has no collider: no craft rests on it and nothing hits
it, so a rock drawn a few centimetres higher or lower than at the last load changes nothing for the game.
Scatter is also sunk into the ground on purpose, so a shift of a few centimetres mostly moves it within the
ground, where nobody sees it.

Install a mod that gives the scatter colliders, and it does matter. The physics engine is handed the
holder's position, the pilot sees what is drawn from its matrix, and those are the two numbers that round
differently: the rock a craft hits is then up to 104 mm from the rock it can see, drawn afresh at every
load. The terrain fix alone does not settle that; with the scatter fix as well, the two agree to a
hundredth of a millimetre. See
[Scatter with colliders](../../non-regression/kopernicus/scatter-with-colliders.md).

And a fix that leaves something behind has to answer for it: in stock the scatter sometimes comes back
exactly on its quad, with the terrain fix never. The scatter fix is that answer: with both on, the ground
and the scatter on it come back at the same place at every load.

## The limit

The objects of a quad are built from its vertices, in the quad's own coordinates, and hang from a
*holder* that `PQSMod_LandClassScatterQuad.Setup` places under the terrain sphere, at
`localPosition = quad.positionPlanet`: the same 600 km vector in a float that
[the culprit](../../the-culprit-ground.md) is about. The holder
is drawn with its local to world matrix, whose translation differs from its own transform position by
whole float steps, so the objects are drawn that much above or below the ground. On stock the two land on
the same step often enough that the holder is drawn exactly on its quad about a quarter of the time on
Kerbin, and within a tenth of a millimetre of it three times out of four on the Mun. With the terrain fix
they never do: the ground is now placed in double precision and the holder is not.

[KSP Diag - Scatter](https://github.com/lhervier/KSP-Diag-Scatter) measures it, on two
saves of its own, one on Kerbin and one on the Mun, each loaded twelve times per series ([the readings](https://github.com/lhervier/KSP-Diag-Scatter/blob/main/docs/what-the-readings-show.md#the-rocks)):
the height of a measured point of an object above the ground under it comes back 94 mm apart over the
twelve loads for half of those points on Kerbin in stock, and 130 mm with the terrain fix; on the Mun,
31 mm either way. The same kind of error, of the same order and wider on Kerbin, but where stock draws
part of it from the ground moving and part from the holder, with the terrain fix all of it comes from the
holder. Stock scatter has no collider, so this is visual only — unless a mod gives it one, which is
[Scatter with colliders](../../non-regression/kopernicus/scatter-with-colliders.md).

## The solution: the scatter fix

The scatter fix hangs each holder from its own terrain quad, so that the objects are drawn in the frame
they were built in. Each part of it has a page of its own.

**The culprit.** Each holder is placed and drawn from the same 600 km vector in a float as its quad. The
quad is drawn from its position, the holder from its matrix, and the two can round to different steps,
drawn anew at every load.

**→ [The scatter fix: the culprit](the-scatter-fix/the-culprit.md)**

**Checking the culprit.** KSP Diag - Scatter, twelve loads on Kerbin and on the Mun: with the terrain fix
alone, half of the measured vertices come back 130 mm apart on Kerbin; with the scatter fix as well,
every holder is drawn on its quad, and no vertex moves by more than 0.125 mm.

**→ [The scatter fix: checking the culprit](the-scatter-fix/checking-the-culprit.md)**

**The fix.** Two Harmony patches hang each holder from its own quad, at no offset, and hand it back to
its pool when the quad goes. The stock scatter is then drawn with the very matrix of the ground it was
built on.

**→ [The scatter fix: the fix](the-scatter-fix/the-fix.md)**

**Should you turn it on?** Not on a stock install: it moves the holders, which a mod may look for where
stock puts them, against a defect nobody sees there. With a mod that gives the scatter colliders, the gap
it closes is a real one, and the trade is yours to weigh.

**→ [The scatter fix: should you turn it on?](the-scatter-fix/should-you-turn-it-on.md)**

**Performance.** Timed frame by frame in flight, three runs with the terrain fix alone and three with the
scatter fix as well: no cost these runs can resolve, the runs of one configuration differing among
themselves by more than the two configurations do.

**→ [The scatter fix: performance](the-scatter-fix/performance.md)**

## Parallax's own scatter is out of this

Read in the source of Parallax Continued, not measured: everything about its objects is expressed in the
frame of the terrain quad — where they are drawn from, the matrix they are drawn through, and the
colliders it can give them, which are children of the quad — so they follow the ground wherever this fix
places it. The scatter fix reads the same source for its own purpose and writes up what it found there
([What never touches them](the-scatter-fix/should-you-turn-it-on.md#what-never-touches-them)).
Parallax also leaves the stock `LandControl` in place on every body but Eeloo, so on a body it does not
strip, the stock scatter this page is about is still there, hanging from the same holders.

One detail this fix leaves exactly as stock has it: Parallax samples its distribution noise from
directions computed in float out of those same 600 km vectors, so an object sitting right at the cutoff
can appear on one load and not on the next. That happens without this fix too, and the fix touches
neither side of it.
