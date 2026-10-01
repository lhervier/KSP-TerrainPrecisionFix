# Kopernicus

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: checked, no problem — the ground is as stable under Kopernicus as without it; the gap of
scatter colliders is halved, not closed, and Rock Precision Fix closes it; where this mod moves the KSC,
it keeps the flags steady without Kopernicus' flag fix, which it keeps from running there; the KSC
Kopernicus moves for Real Solar System is placed where it puts it.**

Most planet packs go through [Kopernicus](https://github.com/Kopernicus/Kopernicus), and it touches
several things this mod deals with. Each is checked on its own page. The bodies here are ones Kopernicus
reconfigures: Kerbin itself, and Earth in Real Solar System, which is Kerbin given another radius,
another terrain and another place for the KSC. A body Kopernicus creates is the case of
[A body from a planet pack](a-body-from-a-planet-pack.md).

### The terrain

**Checked, no problem.** Kopernicus changes the terrain of existing bodies, so this mod has to still find
the ground where it expects it. It does: over six loads on Kerbin under Kopernicus, the height of a
terrain quad spreads over 116.0 mm (median) without this mod and 0.079 mm with it, and the safeguard
never fired; the collision surface on Earth in Real Solar System, over 693.1 mm without this mod and
0.331 mm with it.

**→ Full chapter: [The terrain](kopernicus/the-terrain.md)**

### The flag fix

**Checked, no problem — this mod does the flag fix's job where it moves the KSC.** Kopernicus comes
with a fix for the flag by the launchpad, which twitches on big home bodies, such as Earth in Real Solar
System. The glitch comes from the KSC hanging from its terrain sphere, as the moving statics do: with
Kopernicus' fix turned off, the flag twitches, and it holds as soon as this mod places the KSC in double
precision. So where this mod moves the KSC — in flight, near it — the flag fix is not needed, and this
mod keeps it from running there, where it would not find the KSC and would throw. Everywhere else, it
runs as before.

**→ Full chapter: [The flag fix](kopernicus/the-flag-fix.md)**

### Scatter with colliders

**Checked — this mod halves a stock gap, and does not close it; Rock Precision Fix closes it.** Kopernicus
can give rocks, trees and the like a collider (with the Stock Scatter Collider Enabler Patch), placed on
the ground too. The collider and the drawn rock round differently, so the rock a craft hits is not quite
the rock its pilot sees: over six loads of a kerbal on a boulder, the gap runs from −68.7 to +104.2 mm
without this mod and from −70.2 to +70.3 mm with it. Rock Precision Fix, installed next to this mod,
closes it (−0.026 to +0.022 mm).

**→ Full chapter: [Scatter with colliders](kopernicus/scatter-with-colliders.md)**

### The KSC moved to Cape Canaveral

**Checked, no problem.** Kopernicus moves the KSC to Cape Canaveral for Real Solar System, so this mod
has to handle it at its new place. It does: it takes the KSC out of its sphere there — corrected by 361,
then 266 mm, within a float step on Earth — and puts it back when the save is loaded again and when
the craft reaches orbit; the space centre then opens without error.

**→ Full chapter: [The KSC moved by Real Solar System](kopernicus/the-ksc-moved-by-real-solar-system.md)**
