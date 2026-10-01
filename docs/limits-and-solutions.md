# Limits and solutions

Part of [Terrain Precision Fix](../README.md): the side effects of the fix, and the ground it has not been measured on.

This fix changes where the ground is, so everything that stands on the ground, or is placed from it, has
to be checked against it, one case at a time. **This is a work in progress.** Each case below has a
chapter of its own, and the cases are grouped by where they stand:

- **[Checked, no problem](#checked-no-problem)** — measured or read, and this fix breaks nothing there.
  The summary still says what changes, when something does;
- **[Checked, a problem this mod patches](#checked-a-problem-this-mod-patches)** — this fix would break
  something in another mod, and patches that mod itself. The chapter says what the patch changes, the
  change in that mod it stands for, and what is still to test. None today;
- **[Checked, a real problem](#checked-a-real-problem)** — this fix breaks something there. None today;
- **[Still to test](#still-to-test)** — not done yet: the chapter holds what is already known and what
  is planned to test it.

Every campaign run with this mod installed was run on KSP 1.12.5, with Harmony, ModuleManager and KSP
Community Fixes 1.41.1 — and, for the series on scatter colliders, Kopernicus and the patch that gives
them; the Real Solar System series adds what its chapter lists.

## Checked, no problem

### KSP Community Fixes' own terrain patches

**Checked, in the source.** None of KSP Community Fixes' patches places a terrain quad or a terrain
vertex; the ones that touch the terrain patch how the spheres start and update, and how scatter is
distributed. Read on release 1.40.1, still to read again on 1.41.1.

**→ Full chapter: [KSP Community Fixes' own terrain patches](limits-and-solutions/ksp-community-fixes-own-terrain-patches.md)**

### Rescaled systems: Real Solar System

**Checked on the Moon and on Earth: this mod breaks nothing, and improves on what Real Solar System
does.** The defect grows with the body — a float's step is 125 mm on the Moon, 500 mm on Earth — and
Real Solar System ships a workaround of its own, which moves a landed craft back onto the ground when it
loads. That workaround does not always work: it only acts when the craft is more than 10 cm off, so
without this mod the craft still jumps now and then, and is moved up in one block the rest of the time.
With this mod, nothing moves any more: the craft and the ground come back within a fraction of a
millimetre on both bodies, the workaround never has anything to correct here — it may well have other
uses, outside the scope of this fix — and dozens of reloads on each body did not make the craft jump
once. The safeguard grows with the body
too, so none of the terrain is left uncorrected, on Earth as on Venus, Mars and Mercury, where it was
checked as well.

**→ Full chapter: [Rescaled systems: Real Solar System](limits-and-solutions/rescaled-systems-real-solar-system.md)**

### Rocks, grass and trees

**Checked — this mod does not move them, but widens a stock defect on Kerbin.** Stock already draws
scatter off the ground, differently at every load, from a holder placed through the same float
`Transform` as the ground; this mod corrects the ground and not the holder, so the gap between the two
gets wider. Over twelve loads with Rock Precision Fix Diag, the height of a measured point above the
ground comes back 94 mm apart (median) on Kerbin in stock and 130 mm with this mod, 31 mm either way on
the Mun. Visual only, since stock scatter has no collider. A separate mod,
[Rock Precision Fix](https://github.com/lhervier/KSP-RockPrecisionFix), corrects it, at a cost: it
changes the hierarchy of Unity objects, hanging each holder from its terrain quad, and other mods may
look for them where stock puts them. Parallax's own scatter follows the corrected ground already.

**→ Full chapter: [Rocks, grass and trees](limits-and-solutions/rocks-grass-and-trees.md)**

### The seam between subdivision levels

**Checked on Earth under Real Solar System — this mod does not open the seam, but widens a stock crack.**
Where a quad of the highest level meets a coarser one, the vertices they are supposed to share are
already apart in stock, and the terrain has a crack along the seam that can be seen, though it takes
looking for. This mod corrects the finer side only, so it adds its own correction to that gap. Over
fifteen loads without it and twenty with it, measured with Terrain Precision Fix Diag 4, the median of
the largest gap of a load goes from about 1.2 m to about 1.9 m, and the crack shows more often. Visual
only, since the coarser quads have no collider. A separate mod could close it, in stock and with this
one; it is proposed, not written. On Kerbin, the crack shows in stock too; with this mod, still to
measure.

**→ Full chapter: [The seam between subdivision levels](limits-and-solutions/the-seam-between-subdivision-levels.md)**

### Existing saves

**Checked — a landed craft goes through one more draw, always the same one.** A craft saved in stock
was saved on the ground of one random draw. At its first load with this mod, it comes back on the
corrected ground, which is just one of the draws stock could have given. The difference: that draw no
longer changes. If it happens to be one that breaks the base, it breaks it at every load, where stock
would let the player reload until it survives. Loading the craft once and saving it again with this mod
ends it for good, and raising a craft by a few centimetres in the `.sfs` becomes a permanent repair.
Seen on the Moon under Real Solar System: a craft saved in stock, loaded once and saved again with this
mod, then never moved in 42 loads.

**→ Full chapter: [Existing saves](limits-and-solutions/existing-saves.md)**

### The ground during a flight

**Checked, on the craft and on the ground.** Driving away from a landed craft and back, with no save
loaded, draws the ground again too; this mod patches that path. Six round trips on Kerbin: the craft
comes to rest over 21.8 mm without this mod and 0.094 mm with it, the ground over 21.8 mm and 0.011 mm.

**→ Full chapter: [The ground during a flight](limits-and-solutions/the-ground-during-a-flight.md)**

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

## Checked, a problem this mod patches

None today.

## Checked, a real problem

None today.

## Still to test

### The KSC buildings, runway and launchpad

**Covered by this mod, measured on the runway of Kerbin; the rest still to test.** `PQSCity` places the
statics through the same kind of float `Transform` as the terrain; this mod takes them out of their
sphere in flight to place them in double. On six loadings, the deck of the runway spreads over
130.1 mm without this mod and 0.216 mm with it. Still to test: a floating origin shift under a craft on
the runway, the other ways a scene is left or reloaded, launches, destroyed buildings, the other stock
statics, the runway of Real Solar System. `PQSCity2`, the launch sites of Making History, is not covered.

**→ Full chapter: [The KSC buildings, runway and launchpad](limits-and-solutions/the-ksc-buildings-runway-and-launchpad.md)**

### Mods that look for a static under its sphere

**Two mods read and patched, the others unknown.** Taking a static out of its terrain sphere changes the
hierarchy of Unity objects, and a mod may look for it where stock puts it. It only happens in flight,
near a craft. Kerbal Konstructs and Kopernicus each do so once in flight, and this mod patches both;
each patch stands for a small change the mod itself could make. The group editor of Kerbal Konstructs is
still to test in game.

**→ Full chapter: [Mods that look for a static under its sphere](limits-and-solutions/mods-that-look-for-a-static-under-its-sphere.md)**

### Ground anchors

**TBD.** The anchor is the part most exposed to the moment the ground is drawn — its physics starts on
the very first frame and it is frozen after one — and it has causes of its own that this fix does not
touch (KSP Community Fixes' issue #214). A stable ground should make its behaviour repeatable, not fix
it. The test is planned.

**→ Full chapter: [Ground anchors](limits-and-solutions/ground-anchors.md)**

### A body from a planet pack

**TBD — nothing has been measured on one.** A planet pack builds new bodies from cloned, reconfigured
terrain spheres, and the frame this fix computes in has to still be theirs. The campaign is prepared on
Outer Planets Mod: Slate first, whose float step is Kerbin's; Eeloo, reconfigured rather than created;
Ovok, the edge case.

**→ Full chapter: [A body from a planet pack](limits-and-solutions/a-body-from-a-planet-pack.md)**

### Breaking Ground's surface features

**TBD.** Surface features are placed like the rocks, and carry a collider without any mod. Whether the
physics takes their pose from the holder's matrix or from its transform decides whether this fix widens
a physical offset there; that is to read first, then to measure.

**→ Full chapter: [Breaking Ground's surface features](limits-and-solutions/breaking-grounds-surface-features.md)**

### Breaking Ground's deployed experiments

**TBD.** Deployed experiments are vessels, positioned in double, and should sit on the corrected ground
like any craft; like the ground anchor, they skip the physics hold. Diag 1 on one, with and without this
fix.

**→ Full chapter: [Breaking Ground's deployed experiments](limits-and-solutions/breaking-grounds-deployed-experiments.md)**

### Kerbal Konstructs

**TBD — its ground is read in the source, its statics are covered by this mod and measured on a runway,
its group editor is still to test.** Kerbal Konstructs flattens the ground with the stock
`PQSMod_MapDecal`, which edits the height before this fix places it, so the flattening is kept. Its
statics hang from a stock `PQSCity` and carry the same defect as the KSC's. On a runway it placed on the
Mun, this mod brings the deck back within 0.009 mm instead of 33.5 mm, except at the first loading of a
session, when a section of the runway 21.3 mm higher is still active, on stock as with this mod. Open:
that section, the group editor in flight, and a decal gone wrong, which the safeguard would not catch.

**→ Full chapter: [Kerbal Konstructs](limits-and-solutions/kerbal-konstructs.md)**

### Principia

**TBD — read in the source, not measured.** Principia writes the rotation and position of every body,
the two values this fix places the terrain from, and writes them the way stock does, so the fix reads
them like the rest of the game. Open: whether the quads are placed again often enough to follow a
rotation Principia computes.

**→ Full chapter: [Principia](limits-and-solutions/principia.md)**

### Sloped ground

**TBD.** Every campaign so far is on flat ground. On a slope, a separate stock bug, read in the code and
not measured, moves a single-part craft down into the ground at every load; this fix does not touch it.
The campaign on a 30° slope is what tells the two apart.

**→ Full chapter: [Sloped ground](limits-and-solutions/sloped-ground.md)**

### Asteroids held by a claw

**TBD.** An asteroid follows the corrected ground like any vessel; the fragile case is one resting on the
ground and grappled by a claw, when the ground moves once under it at the first load with this fix.

**→ Full chapter: [Asteroids held by a claw](limits-and-solutions/asteroids-held-by-a-claw.md)**

### KAS

**TBD.** A static KAS attachment either rides on a vessel and follows the corrected ground, or is pinned
to the terrain sphere and does not. To read first.

**→ Full chapter: [KAS](limits-and-solutions/kas.md)**

### The map view

**TBD.** Quads are built and dropped all the time in the map view, and nothing has been measured there.

**→ Full chapter: [The map view](limits-and-solutions/the-map-view.md)**

### Deferred

**TBD.** Deferred has no reason to care where a quad is placed, but it is the mod that broke KSP
Community Fixes' own `PQSOnlyStartOnce`, so anything touching the terrain spheres gets the same check.

**→ Full chapter: [Deferred](limits-and-solutions/deferred.md)**

### Colliders below the highest subdivision level

**TBD on every body but Kerbin and the Mun.** With a non-zero collider offset, levels below the highest
would carry colliders and stay uncorrected. The offset is 0 on Kerbin and the Mun, read in flight; to
read on the other bodies.

**→ Full chapter: [Colliders below the highest subdivision level](limits-and-solutions/colliders-below-the-highest-subdivision-level.md)**
