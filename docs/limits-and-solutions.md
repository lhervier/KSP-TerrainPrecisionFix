# Limits and solutions

Part of [Terrain Precision Fix](../README.md): what this mod makes worse, or would break without a patch
of its own, and the solution to each.

[Non-regression tests](non-regression.md) list what keeps working with this mod. This page lists the
rest: what has been checked and found worse, in stock or in another mod, one mod at a time. Each limit
says how much worse, and its solution: a patch this mod applies to another mod, a fix of its own that
is off by default, or none yet. What is still to check is in [Work in progress](work-in-progress.md).

Unless a page says otherwise, each limit is measured in KSP 1.12.5 with Harmony, ModuleManager, KSP
Community Fixes 1.41.1 and this mod, plus the instrument and the mods it names.

**The statics fix is the riskier of the two fixes on by default.** To place a static in double, this mod takes it out
of its terrain sphere ([The fix: the statics](the-fix-statics.md)). That changes the hierarchy of Unity
objects, and any mod that looks for a static where stock puts it — among the children of a body's
terrain sphere — or reads its `localPosition` as a position relative to the centre of the body, finds it
missing, or somewhere else. What limits the exposure: a static is only out of its sphere in flight, while
a craft is near it, and it is put back before every scene change. Kopernicus and Kerbal Konstructs each
look for a static under its sphere once in flight, and this mod patches both, below. Each patch stands
for a small change in that mod, given as a diff to apply to its source: with it, the patch has nothing
left to do.

## Stock

### Rocks, grass and trees

**A stock defect this mod widens on Kerbin, visual only; a fix of its own closes it, off by default.**
Stock draws terrain scatter a few centimetres off the ground, differently at every load: 94 mm apart on
Kerbin in stock, 130 mm with the terrain fix. The scatter fix closes it, and is off by default, since it
moves stock objects other mods may look for.

**→ Full chapter: [Rocks, grass and trees](limits-and-solutions/stock/rocks-grass-and-trees.md)**

### The seam between subdivision levels

**A stock crack this mod widens, visual only; no solution yet.** Where the quads this mod corrects meet
coarser ones, stock already leaves a crack. The median of the largest gap of a load goes from about
1.3 m to 1.9 m on Earth, and from 157 mm to 225 mm on Kerbin.

**→ Full chapter: [The seam between subdivision levels](limits-and-solutions/stock/the-seam-between-subdivision-levels.md)**

## Kopernicus

### The flag fix

**A problem this mod patches, the patch on by default.** Kopernicus' fix for the flags of the KSC looks
the KSC up under its sphere. In flight near it, where this mod moves the KSC and keeps the flags steady
itself, that fix would throw; the patch keeps it from running there.

**→ Full chapter: [Kopernicus: the flag fix](limits-and-solutions/kopernicus/the-flag-fix.md)**

## Kerbal Konstructs

### The group editor

**A problem this mod patches, the patch on by default.** Moving a group with the gizmo of its group
editor, in flight, Kerbal Konstructs reads the group's position from where stock hangs it. Without the
patch, the group would be sent elsewhere on its body, and saved there.

**→ Full chapter: [Kerbal Konstructs: the group editor](limits-and-solutions/kerbal-konstructs/the-group-editor.md)**
