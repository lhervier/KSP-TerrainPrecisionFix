# The fix: the statics

Part of [Terrain Precision Fix](../README.md): what the patches do for the statics, where they do it, and what they leave alone.

A static cannot be given a precise position while it hangs from its terrain sphere
([The culprit: the statics](the-culprit-statics.md)). So this mod takes it out
of the sphere, and gives it its world position in double, in the same frame as the quads
([The fix: the ground](the-fix-ground.md#doing-the-arithmetic-in-double)):
`body.rotation * planetRelativePosition + body.position`, and for its orientation, the rotation of the
body times the one stock gives it in the sphere.

- **Where it goes.** Under a container of its own, next to the one stock keeps for the quads a craft can
  stand on: outside the body's hierarchy, where a world position is kept as it is. Everything that hangs
  from the static — buildings, colliders, spawn points, the statics of a Kerbal Konstructs group — goes
  with it.
- **When.** Only in flight, and only while the static is within reach of the craft: within the farthest
  another craft can be loaded from it, 22.5 km by default, plus 5 km for the size of a static. A craft
  at the KSC takes the KSC out, and leaves the Island Airfield, 33 km away, under its sphere (logged in
  the session on Kerbin of [The KSC buildings, runway and launchpad](limits-and-solutions/the-ksc-buildings-runway-and-launchpad.md)). Only the
  statics that hang directly from their sphere are handled, as those of stock and of Kerbal Konstructs
  do; `PQSCity2` is not.
- **Following the body.** Stock moves the quads in the same call that moves the body, at every shift of
  the floating origin (the setter of `PQS.PrecisePosition`); a static out of its sphere is placed again
  there too, and whenever the body turns (`CelestialBody.CBUpdate`). If something else moves it — an
  editor, say — it stays where it was put, relative to the body.
- **Back under its sphere.** A static goes back exactly where stock left it, with its own local
  position and rotation, whenever it goes out of reach, before every scene change, and for the time
  stock code that expects it there runs: `PQSCity.Orientate`, `Start` and `ResetCelestialBody`, which
  write its local position or read its body from its parents; `PQS.SetupMods`, which lists the mods of
  a sphere from its children and would otherwise drop the static from the list; and `CommNetHome.Start`
  and `DayNightGameObjectSwitch.Setup`, which read the body of a ground station or of a light switch
  from their parents, and can be part of a static. Outside flight, a static is always where stock puts
  it.

## Other mods that look for a static under its sphere

Taking a static out of its sphere changes the hierarchy of Unity objects, and a mod may look for a
static where stock puts it. Two of the most installed ones do, in flight, once each, and this mod
patches both: the group editor of [Kerbal Konstructs](limits-and-solutions/kerbal-konstructs.md#the-patch-of-the-group-editor),
and the flag fix of [Kopernicus](limits-and-solutions/kopernicus/the-flag-fix.md) — which is not needed
where this mod moves the KSC: placed in double, the KSC keeps its flags steady, and the patch keeps the
flag fix from running there.
Each patch is described on the page of the mod it patches, along with the small change in that mod's own
code that would make the patch unnecessary. Each leaves
the original code path untouched as long as the static is under its sphere, so it changes nothing
without this mod's statics fix; if the code of the mod is not the one the patch expects, the statics fix
stays off.

## Safeguards

- a correction larger than sixteen float steps at the distance of the static is refused, and the static
  is left where stock puts it — the same limit as for the quads
  ([The fix: the ground](the-fix-ground.md#safeguards));
- a static is only taken out of its sphere if the container it goes to has the same scale as the
  sphere, so that the scale a mod gives it keeps its meaning;
- the statics fix is installed on its own, apart from [the ground fix](the-fix-ground.md), and can be
  turned off in the settings. If any of its patches fails to install, none of them does anything;
- if Kerbal Konstructs or Kopernicus is installed and its code is not the one its patch expects, the
  statics fix stays off, and every static stays where stock puts it. Each of these two patches can also
  be turned off in the settings, to see what goes wrong without it: the statics fix then stays on, and
  the log says which mod will break, and how.
