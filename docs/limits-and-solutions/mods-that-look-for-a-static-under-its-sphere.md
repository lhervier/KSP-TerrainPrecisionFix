# Mods that look for a static under its sphere

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: two mods read and patched, the others unknown.** To place a static in double, this mod takes
it out of its terrain sphere ([The statics](../the-fix-this-mod-proposes.md#the-statics)). That changes
the hierarchy of Unity objects, and any mod that looks for a static where stock puts it — among the
children of a body's terrain sphere — or reads its `localPosition` as a position relative to the
centre of the body, will find it missing, or somewhere else.

What limits the exposure: a static is only out of its sphere in flight, while a craft is near it, and it
is put back before every scene change. A mod that looks for it at the main menu, at the space centre or
in an editor finds it where stock puts it.

**Read, and patched.**
[Kerbal Konstructs](https://github.com/KSP-RO/Kerbal-Konstructs) (1.12.3) and
[Kopernicus](https://github.com/Kopernicus/Kopernicus) each look for a static under its sphere in
flight once: the group editor of Kerbal Konstructs, when a group is moved with its gizmo, and the flag
fix of Kopernicus, when a facility is upgraded. This mod patches both; what each patch changes, and the
small change in each mod it stands for, are in
[Other mods that look for a static under its sphere](../the-fix-this-mod-proposes.md#other-mods-that-look-for-a-static-under-its-sphere).
Every other lookup of a static under its sphere in their source runs at the main menu or when a scene
opens. If either mod is installed and its code is not the one the patch expects, the statics fix stays
off.

**Checked in game**: under Real Solar System, the patch of Kopernicus applies to the version installed,
and the space centre opens without error after a flight near the KSC.

*To test:* the group editor of Kerbal Konstructs in flight, near a craft — moving a group, turning it,
creating, copying and deleting one, then loading the save again (see
[Kerbal Konstructs](kerbal-konstructs.md)); the flag fix of Kopernicus, in a mission of the Making
History expansion that spawns a craft at the KSC. And the other mods: any mod that places things
relative to the KSC, or looks up a `PQSCity`, is to read.
