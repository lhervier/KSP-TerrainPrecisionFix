# Limits and solutions

Part of [Terrain Precision Fix](../README.md): the side effects of the fix, and the ground it has not been measured on.

This fix changes where the ground is, so everything that stands on the ground, or is placed from it, has
to be checked against it, one case at a time. **This is a work in progress.** Each case below has a
chapter of its own, and the cases are grouped by where they stand:

- **[Checked, no problem](#checked-no-problem)** — measured or read, and this fix breaks nothing there.
  The summary still says what changes, when something does;
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

### Kopernicus, on a stock body

**Checked — the fix still places the terrain, and the ground is as stable under Kopernicus as without
it.** Over six loads on Kerbin under Kopernicus, the height of a terrain quad spreads over 116.0 mm
(median) without this mod and 0.079 mm with it, and the safeguard never fired. Still to read: the
collision surface itself, with Diag 2.

**→ Full chapter: [Kopernicus, on a stock body](limits-and-solutions/kopernicus-on-a-stock-body.md)**

### Rescaled systems: Real Solar System

**Checked on the Moon and on Earth: this mod breaks nothing, and improves on what Real Solar System
does.** The defect grows with the body — a float's step is 125 mm on the Moon, 500 mm on Earth — and
Real Solar System ships a workaround of its own, which moves a landed craft back onto the ground when it
loads. That workaround does not always work: it only acts when the craft is more than 10 cm off, so
without this mod the craft still jumps now and then, and is moved up in one block the rest of the time.
With this mod, nothing moves any more: the craft and the ground come back within a fraction of a
millimetre on both bodies, the workaround never has anything to correct here — it may well have other
uses, outside the scope of this fix — and dozens of reloads on the Moon did not make the craft jump
once. The safeguard grows with the body
too, so none of Earth's terrain is left uncorrected.

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

### Scatter with colliders

**Checked — this mod halves a stock defect, and does not close it.** When a mod gives scatter a
collider (Kopernicus with the Stock Scatter Collider Enabler Patch), the collider and the drawn rock
round differently, so the rock a craft hits is not quite the rock its pilot sees. Over six loads of a
kerbal on a boulder, the gap between them runs from −68.7 to +104.2 mm on stock and from −70.2 to
+70.3 mm with this mod. Rock Precision Fix, installed next to this mod, closes it (−0.026 to
+0.022 mm), with the same cost on the hierarchy of Unity objects.

**→ Full chapter: [Scatter with colliders](limits-and-solutions/scatter-with-colliders.md)**

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

## Checked, a real problem

None today.

## Still to test

### The KSC buildings, runway and launchpad

**Not covered by this fix, and to test with it.** `PQSCity` and `PQSCity2` place the statics through the same kind
of float `Transform`, and this fix does not touch them. A capsule parked on the runway spreads over
117 mm on six loads on stock, which cannot yet tell the runway from the ground under it; the same
reading with this mod will.

**→ Full chapter: [The KSC buildings, runway and launchpad](limits-and-solutions/the-ksc-buildings-runway-and-launchpad.md)**

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

**TBD — read in the source, not measured.** Kerbal Konstructs flattens the ground with the stock
`PQSMod_MapDecal`, which edits the height before this fix places it, so the flattening is kept. Open:
whether its statics carry the same defect as the KSC's, and a decal gone wrong, which the safeguard
would not catch.

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
