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

**Read, and patched.** Each of these two mods looks for a static under its sphere once in flight, and
this mod patches that one place; the patch, and the small change in the mod it stands for, are
described with the mod:

- [Kerbal Konstructs](kerbal-konstructs.md#the-patch-of-the-group-editor), its group editor, when a
  group is moved with its gizmo;
- [Kopernicus](kopernicus/the-flag-fix.md), its flag fix, when a facility is upgraded: not needed
  while the KSC is out of its sphere, which keeps its flags steady, so the patch keeps it from running
  then.

*To test:* the other mods. Any mod that places things relative to the KSC, or looks up a `PQSCity`, is
to read.
