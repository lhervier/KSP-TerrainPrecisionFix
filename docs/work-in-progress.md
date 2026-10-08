# Work in progress

Part of [Terrain Precision Fix](../README.md): what is still to check, one mod at a time, stock first.

Each point is something this mod could break, and is not checked yet: why it is a question, what is
already known, and the test planned to answer it, played by hand in game, with this mod and without
it. In each chapter, the checks in progress come first, then those planned. Once played, a point moves
to [Non-regression tests](non-regression.md) if nothing is worse, or to
[Limits and solutions](limits-and-solutions.md) if something is.

## Stock

### Sloped ground

**Planned.** Every campaign so far is on flat ground. On a slope, a separate stock bug, read in the code,
moves a single-part craft into the ground at every load; this mod should leave it as it is, and make it
repeatable.

**→ Full chapter: [Sloped ground](work-in-progress/stock/sloped-ground.md)**

### Asteroids held by a claw

**Planned.** At the first load with this mod, the ground under an asteroid resting on it moves once, with
a mass on the other end of a joint.

**→ Full chapter: [Asteroids held by a claw](work-in-progress/stock/asteroids-held-by-a-claw.md)**

### Breaking Ground's surface features

**Planned.** They are placed like the rocks, and carry a collider: this mod could widen a physical gap
there, as it widens a visual one for the rocks.

**→ Full chapter: [Breaking Ground's surface features](work-in-progress/stock/breaking-grounds-surface-features.md)**

### Breaking Ground's deployed experiments

**Planned.** They are vessels and should sit on the corrected ground like a craft, but skip the physics
hold, like the ground anchor.

**→ Full chapter: [Breaking Ground's deployed experiments](work-in-progress/stock/breaking-grounds-deployed-experiments.md)**

### The ocean

**Planned.** Read in the logs, this mod corrected terrain quads only, never the ocean; how the ocean
sphere is flagged has not been read, and a craft floating near a coast has not been watched.

**→ Full chapter: [The ocean](work-in-progress/stock/the-ocean.md)**

### The ground station

**Planned.** The ground station of the KSC finds its body by looking up from where it hangs, and a game
that starts in flight starts it while the KSC may be out of its sphere.

**→ Full chapter: [The ground station](work-in-progress/stock/the-ground-station.md)**

### A mission spawning a craft

**Planned.** A mission of Making History spawns a craft on a spawn point of the KSC, as a launch does.

**→ Full chapter: [A mission spawning a craft](work-in-progress/stock/a-mission-spawning-a-craft.md)**

### The other stock statics

**Planned.** The KSC 2, the Island Airfield, the pyramids, the monoliths and the easter eggs are placed
like the KSC, and taken out of their sphere the same way; some monoliths are placed at random, from the
seed of the game.

**→ Full chapter: [The other stock statics](work-in-progress/stock/the-other-stock-statics.md)**

### The scatter fix without the terrain fix

**Planned.** The scatter fix, off by default, can be turned on with the terrain fix off, but has only
been measured with it.

**→ Full chapter: [The scatter fix without the terrain fix](work-in-progress/stock/the-scatter-fix-without-the-terrain-fix.md)**

## KSP Community Fixes

### Its own terrain patches, on 1.41.1

**Planned.** Its terrain patches were read on release 1.40.1; every campaign ran with 1.41.1.

**→ Full chapter: [KSP Community Fixes: its own terrain patches, on 1.41.1](work-in-progress/ksp-community-fixes/its-own-terrain-patches-on-1-41-1.md)**

## Kopernicus

### A body from a planet pack

**Planned — nothing has been measured on one.** A planet pack builds new bodies from cloned,
reconfigured terrain spheres, and the frame this fix computes in has to still be theirs. The campaign is
prepared on Outer Planets Mod: Slate first, whose float step is Kerbin's.

**→ Full chapter: [Kopernicus: a body from a planet pack](work-in-progress/kopernicus/a-body-from-a-planet-pack.md)**

## Real Solar System

### Coming back to a craft left parked, on the Moon

**In progress.** Every measurement under Real Solar System loads a landed craft again; coming back to a
craft left parked, where its quads are built again after the origin has moved, is still to play.

**→ Full chapter: [Real Solar System: coming back to a craft left parked, on the Moon](work-in-progress/real-solar-system/coming-back-to-a-craft-left-parked-on-the-moon.md)**

### The ground workaround

**In progress.** Real Solar System's ground workaround has nothing left to correct with this mod on the
Moon and on Earth; the rest of the series, a craft *landed* on Earth and the other moments it runs, is
still to play.

**→ Full chapter: [Real Solar System: the ground workaround](work-in-progress/real-solar-system/the-ground-workaround.md)**

### The runway fix

**Planned.** Checked with Real Solar System built without its runway fix; with the runway fix as
released, a craft reloaded and taxied on the runway is still to watch.

**→ Full chapter: [Real Solar System: the runway fix](work-in-progress/real-solar-system/the-runway-fix.md)**

### Descents onto Venus, Mars and Mercury

**Planned.** This mod refused no correction on these bodies over loads; a descent, where terrain is built
all the way down, is still to fly.

**→ Full chapter: [Real Solar System: descents onto Venus, Mars and Mercury](work-in-progress/real-solar-system/descents-onto-venus-mars-and-mercury.md)**

## Kerbal Konstructs

Its statics carry the same defect as the KSC's, and this mod corrects them: measured on a runway it
placed on the Mun, in [Checking the culprit: loading the same save](checking-the-culprit-loading.md).
Its group editor needs a patch of this mod: [Limits and solutions](limits-and-solutions.md#kerbal-konstructs).

### A section of the runway at the first loading

**Planned — seen, not explained.** At the first loading of a session, a section of the runway 21.3 mm
above the deck is still active under the craft; not a rounding, and this mod does not touch it.

**→ Full chapter: [Kerbal Konstructs: a section of the runway at the first loading](work-in-progress/kerbal-konstructs/a-section-of-the-runway-at-the-first-loading.md)**

### The rest of the group editor

**Planned.** Turning, creating, copying and deleting a group in flight, near a craft, then loading the
save again.

**→ Full chapter: [Kerbal Konstructs: the rest of the group editor](work-in-progress/kerbal-konstructs/the-rest-of-the-group-editor.md)**

### A launch from its launch sites

**Planned.** A craft launched from a launch site of Kerbal Konstructs is placed on a static this mod
moves in flight.

**→ Full chapter: [Kerbal Konstructs: a launch from its launch sites](work-in-progress/kerbal-konstructs/a-launch-from-its-launch-sites.md)**

### The ground it flattens

**Planned — read in the source, not measured.** Kerbal Konstructs flattens the ground with stock decals,
which edit the height before this fix places it, so the flattening should be kept.

**→ Full chapter: [Kerbal Konstructs: the ground it flattens](work-in-progress/kerbal-konstructs/the-ground-it-flattens.md)**

## Principia

### The bodies it moves and turns

**Planned — read in the source, not measured.** Principia writes the rotation and position of every
body, the two values this fix places the terrain from, the way stock does. Open: whether the quads follow
a rotation Principia computes.

**→ Full chapter: [Principia: the bodies it moves and turns](work-in-progress/principia/the-bodies-it-moves-and-turns.md)**

## Parallax

### Its terrain and its scatter

**Planned — read in the source, not measured.** Parallax draws the terrain on the meshes stock builds,
and its scatter in the frame of the terrain quad, so both should follow the ground wherever this fix
places it.

**→ Full chapter: [Parallax: its terrain and its scatter](work-in-progress/parallax/its-terrain-and-its-scatter.md)**

## Tilt'Em

### A tilted body

**Planned — read in the source, not measured.** Tilt'Em rewrites the rotation of every body, in a stock
method this fix also patches, the way stock does. Open: the ground of a tilted body when a craft comes
down to where the world turns with it.

**→ Full chapter: [Tilt'Em: a tilted body](work-in-progress/tilt-em/a-tilted-body.md)**

## KAS

### A part attached to the ground

**Planned.** A static KAS attachment either rides on a vessel and follows the corrected ground, or is
pinned to the terrain sphere and does not. To read first.

**→ Full chapter: [KAS: a part attached to the ground](work-in-progress/kas/a-part-attached-to-the-ground.md)**

## Other mods

### Mods that look for a static under its sphere

**Planned.** Any mod that looks for a static where stock puts it would miss it in flight, near a craft.
Kopernicus and Kerbal Konstructs are read and patched; the others are not read.

**→ Full chapter: [Mods that look for a static under its sphere](work-in-progress/other-mods/mods-that-look-for-a-static-under-its-sphere.md)**

### Mods that look for a scatter holder under its sphere

**Planned.** With the scatter fix on, off by default, any mod that looks for a scatter holder where stock
puts it would miss it. Stock, KSP Community Fixes, Kopernicus, Parallax and TUFX are read; the others are
not.

**→ Full chapter: [Mods that look for a scatter holder under its sphere](work-in-progress/other-mods/mods-that-look-for-a-scatter-holder-under-its-sphere.md)**
