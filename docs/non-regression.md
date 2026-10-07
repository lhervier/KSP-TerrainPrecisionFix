# Non-regression tests

Part of [Terrain Precision Fix](../README.md): what works without this mod still works with it.

[Checking the culprit](../README.md#checking-the-culprit) shows what this mod fixes. These tests check
the other side, one mod at a time, stock first: what this mod could break, played in game with it, and
compared with the game without it where the two have to be told apart. Only what has been checked is
here; what is still to check is in [Work in progress](work-in-progress.md), and what this mod does make
worse, with its solution, in [Limits and solutions](limits-and-solutions.md).

Unless a page says otherwise, each test is played in KSP 1.12.5 with Harmony, ModuleManager, KSP
Community Fixes 1.41.1 and this mod, plus the instrument and the mods it names. Each test has a page of
its own.

## Stock

This mod places the terrain quads a craft stands on in double, and takes the statics out of their
terrain sphere in flight, near a craft ([The fix: the ground](the-fix-ground.md),
[The fix: the statics](the-fix-statics.md)); with its scatter fix on, off by default, it hangs each
holder of terrain scatter from its quad ([The fix: the scatter](the-fix-scatter.md)). What stock places
on the ground, or finds the statics and the holders by, has to keep working.

### Existing saves

**Checked, no problem — a landed craft goes through one more draw, always the same one.** A craft saved
without this mod comes back once on the corrected ground; saved again with it, on the Moon in Real Solar
System, the workaround that moves a landed craft back onto the ground never had to move it, in 36 loads.

**→ Full chapter: [Existing saves](non-regression/stock/existing-saves.md)**

### Colliders below the highest subdivision level

**Not a regression, not corrected either; on stock, no such collider exists.** This mod corrects the
quads of the highest level only: a lower-level quad with a collider would jump at every load as in
stock. The collider offset that would allow one is 0 on every stock body, read in game, and Kopernicus
cannot change it. A craft flying very fast over a landed one may leave it on terrain without any
collider: a stock case, read in the code, outside what this mod fixes.

**→ Full chapter: [Colliders below the highest subdivision level](non-regression/stock/colliders-below-the-highest-subdivision-level.md)**

### Scene changes

**Checked on Kerbin and on Earth in Real Solar System.** This mod has to put the KSC back before every
scene change. Through every way of leaving a flight and coming back, a trip to another body included,
the launchpad stays within 0.2 mm, against up to 938 mm without this mod. A craft launched from the
VAB stands on the launchpad, one from the SPH on the runway, and every building opens.

**→ Full chapter: [Scene changes](non-regression/stock/scene-changes.md)**

### Destroyed buildings

**Checked on Kerbin.** KSP finds a destructible building by its path below the KSC, which this mod moves in
flight. The VAB, brought down by a heavy craft dropped onto it while the KSC is out of its sphere,
collapses, shows as destroyed at the space centre, stays in ruins through the next flights, and stands again
once repaired, as without this mod; the launchpad and the runway stay within 0.2 mm, against 53 mm
for the launchpad without it.

**→ Full chapter: [Destroyed buildings](non-regression/stock/destroyed-buildings.md)**

### Facility levels

**Checked on Kerbin.** Each level of a facility is a model of its own, built below the KSC. At each of
their three levels, a craft stands on the launchpad and on the runway of that level, as without this mod,
and stays within 0.25 mm through its arrivals, against up to 145 mm without it.

**→ Full chapter: [Facility levels](non-regression/stock/facility-levels.md)**

### A launch pad placed by a mission

**Impossible to reproduce.** A mission of Making History can place a launch pad of its own, which stock
looks for under its sphere once. Read in the code, that never happens in flight, where this mod moves
statics; no way was found to launch from such a pad on stock.

**→ Full chapter: [A launch pad placed by a mission](non-regression/stock/a-launch-pad-placed-by-a-mission.md)**

### A static turning with its body

**Cannot happen on stock, in Real Solar System or with Outer Planets Mod.** This mod has to turn a static
out of its sphere when KSP turns the body. Measured body by body, no body turns lower than 100 km, far
above any craft near a static this mod has moved.

**→ Full chapter: [A static turning with its body](non-regression/stock/a-static-turning-with-its-body.md)**

### The map view

**Nothing to test, read in the code.** The map draws the bodies from scaled space, which this mod does
not touch. While it is open, the terrain keeps being built around the active craft exactly as in
flight, and the markers of the launch sites follow their statics wherever this mod places them.

**→ Full chapter: [The map view](non-regression/stock/the-map-view.md)**

### The scatter holders

**Checked with the scatter fix on, no problem — no holder lost.** The scatter fix takes each holder out
of its pool's container while its quad is in use. Over a flight over the Mun and across scene switches,
every holder went back to its pool, none was left on a quad, and the pools counted as in stock.

**→ Full chapter: [The scatter holders](non-regression/stock/the-scatter-holders.md)**

## KSP Community Fixes

KSP Community Fixes is the base most players run, and it patches the terrain too.

### Its own terrain patches

**Checked, in the source and in every campaign.** None of its patches places a terrain quad or a terrain
vertex, or names a scatter holder, and every measurement of this mod was taken with KSP Community Fixes
1.41.1 installed.

**→ Full chapter: [KSP Community Fixes: its own terrain patches](non-regression/ksp-community-fixes/its-own-terrain-patches.md)**

## Kopernicus

Most planet packs go through [Kopernicus](https://github.com/Kopernicus/Kopernicus), and it touches
several things this mod deals with. The bodies here are ones Kopernicus reconfigures: Kerbin itself, and
Earth in Real Solar System, which is Kerbin given another radius, another terrain and another place for
the KSC. Its flag fix needs a patch of this mod: it is in
[Limits and solutions](limits-and-solutions.md#kopernicus).

### The terrain

**Checked, no problem.** Kopernicus changes the terrain of existing bodies, so this mod has to still find
the ground where it expects it. Over six loads on Kerbin, a terrain quad spreads over 116.0 mm (median)
without this mod and 0.079 mm with it.

**→ Full chapter: [Kopernicus: the terrain](non-regression/kopernicus/the-terrain.md)**

### Scatter with colliders

**Checked — the terrain fix halves a stock gap; the scatter fix closes it.** Kopernicus can give rocks a
collider, which stock places apart from the rock drawn: over six loads, −68.7 to +104.2 mm without this
mod, −70.2 to +70.3 mm with the terrain fix, −0.026 to +0.022 mm with the scatter fix too.

**→ Full chapter: [Kopernicus: scatter with colliders](non-regression/kopernicus/scatter-with-colliders.md)**

### The KSC moved to Cape Canaveral

**Checked, no problem.** Kopernicus moves the KSC for Real Solar System, so this mod has to handle it at
its new place. It takes the KSC out of its sphere there and puts it back, and the launchpad comes back
to the same place through every way of leaving a flight, within 0.2 mm.

**→ Full chapter: [Kopernicus: the KSC moved by Real Solar System](non-regression/kopernicus/the-ksc-moved-by-real-solar-system.md)**

## Real Solar System

The defect grows with the body: a float's step is 62.5 mm at the radius of Kerbin, 125 mm on the Moon,
500 mm on Earth. [Real Solar System](https://github.com/KSP-RO/RealSolarSystem) ships two workarounds for
what it does to a craft, and adds statics of its own. The measurements on the Moon and on Earth, and
their install, are in [Checking the culprit: loading the same save](checking-the-culprit-loading.md) and
[Checking the culprit: driving on while the world moves](checking-the-culprit-driving.md); the logs of
every session with this mod in [`diag/runs`](../diag/README.md#on-real-solar-system).

### The ground workaround

**Checked on the Moon and on Earth — with this mod, it has nothing left to correct for this defect.** Real
Solar System moves a landed craft back onto the ground when it is more than 10 cm off. Over 75 loads in
a row with this mod, the craft never moved, and the workaround never had anything to correct.

**→ Full chapter: [Real Solar System: the ground workaround](non-regression/real-solar-system/the-ground-workaround.md)**

### The runway fix

**Checked on Earth — with this mod, the runway is where it is drawn, with no step.** Real Solar System
turns off the colliders of the runway's sections and holds the floating origin on it. Built without that
fix, with this mod, 37 entries in flight showed no step, and a move of the origin moved nothing.

**→ Full chapter: [Real Solar System: the runway fix](non-regression/real-solar-system/the-runway-fix.md)**

### This mod's safeguard

**Checked on the Moon, Earth, Venus, Mars and Mercury — it never refused a correction.** This mod
refuses any correction larger than sixteen float steps, a limit that grows with the body. The largest
correction, on Earth, was four steps, 2 m, a quarter of the limit; a first version, a fixed metre,
refused part of Earth's terrain.

**→ Full chapter: [Real Solar System: this mod's safeguard](non-regression/real-solar-system/this-mods-safeguard.md)**

### A CommNet ground station

**Checked on Earth, no problem.** Real Solar System adds 63 CommNet ground stations, statics this mod
takes out of their sphere. Driven toward the one of Kourou, a rover sees it taken out on the way, and
CommNet keeps linking the rover to it.

**→ Full chapter: [Real Solar System: a CommNet ground station](non-regression/real-solar-system/a-commnet-ground-station.md)**

## Deferred

[Deferred](https://github.com/LGhassen/Deferred) replaces the way KSP draws everything, the terrain
included, and it is the mod that broke KSP Community Fixes' own `PQSOnlyStartOnce`: anything touching the
terrain spheres gets checked against it.

### Drawing the ground

**Checked, no problem.** Deferred draws the ground wherever it is placed and patches nothing this mod
patches. With it installed, the craft and the ground come back as they do without it, after a load and
after a trip out of range.

**→ Full chapter: [Deferred: drawing the ground](non-regression/deferred/drawing-the-ground.md)**
