# Limits and solutions

Part of [Terrain Precision Fix](../README.md): what this fix does to other mods.

This fix changes where the ground and the statics are, so every mod that places something on the
ground, reads it, or changes the bodies it is built from, has to be checked against it, one mod at a
time. What it does to stock itself is in the non-regression tests of
[the ground](non-regression-ground.md) and of [the statics](non-regression-statics.md). **This is a
work in progress.** Each case below has a chapter of its own, and the cases are grouped by where they
stand:

- **[Checked, no problem](#checked-no-problem)** — measured or read, and this fix breaks nothing there.
  The summary still says what changes, when something does;
- **[Checked, a problem this mod patches](#checked-a-problem-this-mod-patches)** — this fix would break
  something in another mod, and patches that mod itself. The chapter says what the patch changes, the
  change in that mod it stands for, and what is still to test. None today;
- **[Checked, a real problem](#checked-a-real-problem)** — this fix breaks something there. None today;
- **[Still to test](#still-to-test)** — not done yet: the chapter holds what is already known and what
  is planned to test it.

Every campaign run with this mod installed was run on KSP 1.12.5, with Harmony, ModuleManager and KSP
Community Fixes 1.41.1, plus, of course, the mods the case is about: each chapter names them.

## Checked, no problem

### KSP Community Fixes' own terrain patches

**Checked, in the source.** None of KSP Community Fixes' patches places a terrain quad or a terrain
vertex; the ones that touch the terrain patch how the spheres start and update, and how scatter is
distributed. Read on release 1.40.1, still to read again on 1.41.1.

**→ Full chapter: [KSP Community Fixes' own terrain patches](limits-and-solutions/ksp-community-fixes-own-terrain-patches.md)**

### Rescaled systems: Real Solar System

**Checked: this mod breaks nothing, and leaves Real Solar System's workarounds nothing to correct for
this defect.** The defect grows with the body — a float's step is 125 mm on the Moon, 500 mm on Earth —
and Real Solar System ships two workarounds for it. One moves a landed craft back onto the ground when
it goes off rails, only when it is more than 10 cm off: without this mod, the craft still jumps now and
then; with it, the craft and the ground come back within a fraction of a millimetre, and dozens of
reloads on the Moon and on Earth did not make it jump once. The other turns off the colliders of the
runway's sections and holds the floating origin while a craft rolls on it: without it, the pieces of the
runway are rounded each on their own, and a craft can rest above the deck or sunk into it, or meet a
step, and the runway jumps by up to 232 mm with the grass at each move of the floating origin; with
this mod, the runway is where it is drawn, in one piece, and a move of the origin moves nothing. The
safeguard grows with the body too, so none of the terrain is left
uncorrected, on Earth as on Venus, Mars and Mercury. Still to test: a flight, not only loads.

**→ Full chapter: [Rescaled systems: Real Solar System](limits-and-solutions/rescaled-systems-real-solar-system.md)**

### Kopernicus

**Checked, no problem.** Most planet packs go through Kopernicus, and it touches four things this mod
deals with:

- **the terrain** — Kopernicus changes the terrain of existing bodies, so this mod has to still find
  the ground where it expects it. It does: on Kerbin and on Earth in Real Solar System, the ground comes
  back to the same height at every load, as without Kopernicus;
- **the flag fix** — Kopernicus comes with a fix for the flag by the launchpad, which twitches on big
  home bodies, such as Earth in Real Solar System. The glitch comes from the KSC hanging from its terrain
  sphere, as the moving statics do: where this mod moves the KSC, in flight near it, it holds the flag
  without Kopernicus' fix, and keeps that fix from running there, where it would not find the KSC and
  would throw. Everywhere else, Kopernicus' fix runs as before;
- **scatter with colliders** — Kopernicus can give rocks, trees and the like a collider, placed on the
  ground too. This mod does not make them worse: the gap between a rock and its collider, already there
  without this mod, is halved, and Rock Precision Fix closes it;
- **the KSC moved to Cape Canaveral** — Kopernicus moves the KSC for Real Solar System. This mod handles
  it at its new place, and the space centre opens without error after a flight.

**→ Full chapter: [Kopernicus](limits-and-solutions/kopernicus.md)**

### Deferred

**Checked, no problem.** Deferred replaces the way KSP draws everything, the terrain included, and it is
the mod that broke KSP Community Fixes' own `PQSOnlyStartOnce`, so anything touching the terrain spheres
gets checked against it. It draws the ground wherever it is placed and patches nothing this mod patches:
with it installed, the craft and the ground come back as they do without it, after a load and after a
trip out of range.

**→ Full chapter: [Deferred](limits-and-solutions/deferred.md)**

## Checked, a problem this mod patches

None today.

## Checked, a real problem

None today.

## Still to test

### Mods that look for a static under its sphere

**Two mods read and patched, the others unknown.** Taking a static out of its terrain sphere changes the
hierarchy of Unity objects, and a mod may look for it where stock puts it. It only happens in flight,
near a craft. Kerbal Konstructs and Kopernicus each do so once in flight, and this mod patches both;
each patch stands for a small change the mod itself could make. The group editor of Kerbal Konstructs is
still to test in game.

**→ Full chapter: [Mods that look for a static under its sphere](limits-and-solutions/mods-that-look-for-a-static-under-its-sphere.md)**

### A body from a planet pack

**TBD — nothing has been measured on one.** A planet pack builds new bodies from cloned, reconfigured
terrain spheres, and the frame this fix computes in has to still be theirs. The campaign is prepared on
Outer Planets Mod: Slate first, whose float step is Kerbin's; Eeloo, reconfigured rather than created;
Ovok, the edge case.

**→ Full chapter: [A body from a planet pack](limits-and-solutions/a-body-from-a-planet-pack.md)**

### Kerbal Konstructs

**TBD — its ground is read in the source, its statics are covered by this mod and measured on a runway,
its group editor is still to test.** Kerbal Konstructs flattens the ground with the stock
`PQSMod_MapDecal`, which edits the height before this fix places it, so the flattening is kept. Its
statics hang from a stock `PQSCity` and carry the same defect as the KSC's. On a runway it placed on the
Mun, this mod brings the deck back within 0.015 mm instead of 17.7 mm, except at the first loading of a
session, when a section of the runway 21.3 mm higher is still active. Open:
that section, the group editor in flight, and a decal gone wrong, which the safeguard would not catch.

**→ Full chapter: [Kerbal Konstructs](limits-and-solutions/kerbal-konstructs.md)**

### Principia

**TBD — read in the source, not measured.** Principia writes the rotation and position of every body,
the two values this fix places the terrain from, and writes them the way stock does, so the fix reads
them like the rest of the game. Open: whether the quads are placed again often enough to follow a
rotation Principia computes.

**→ Full chapter: [Principia](limits-and-solutions/principia.md)**

### Parallax

**TBD — read in the source, not measured.** Parallax draws the terrain with shaders of its own, on the
meshes stock builds, and its scatter is expressed in the frame of the terrain quad, colliders included,
so it should follow the ground wherever this fix places it. The stock scatter it leaves in place keeps
the gap this fix widens on Kerbin. Open: the loading and approach campaigns with Parallax installed, and
an instrument for its scatter.

**→ Full chapter: [Parallax](limits-and-solutions/parallax.md)**

### Tilt'Em

**TBD — read in the source, not measured.** Tilt'Em gives the planets an axial tilt by rewriting the
rotation of every body, one of the two values this fix places the terrain from, in a stock method this
fix also patches. It writes that rotation the way stock does, so the fix reads it like the rest of the
game, and the fix's own patch on that method still runs. Open: whether the ground follows a tilted body
when a craft comes down to the altitude where the world starts turning with it.

**→ Full chapter: [Tilt'Em](limits-and-solutions/tilt-em.md)**

### KAS

**TBD.** A static KAS attachment either rides on a vessel and follows the corrected ground, or is pinned
to the terrain sphere and does not. To read first.

**→ Full chapter: [KAS](limits-and-solutions/kas.md)**

