# The fix: the statics

Part of [Terrain Precision Fix](../README.md): what the patches do for the statics, where they do it, and what they leave alone.

A static cannot be given a precise position while it hangs from its terrain sphere
([The culprit: the statics](the-culprit-statics.md)). So this mod takes it out of the sphere, with
code of its own run at every frame in flight, not with a patch, and gives it its world position in
double, in the same frame as the quads ([The fix: the ground](the-fix-ground.md#doing-the-arithmetic-in-double)):
`body.rotation * planetRelativePosition + body.position`, and for its orientation, the rotation of the
body times the one stock gives it in the sphere.

- **Where it goes.** Under a container of its own, next to the one stock keeps for the quads a craft can
  stand on: outside the body's hierarchy, where a world position is kept as it is. Everything that hangs
  from the static — buildings, colliders, spawn points, the statics of a Kerbal Konstructs group — goes
  with it.
- **When.** Only in flight, and only while the static is within reach of the craft: within the farthest
  another craft can be loaded from it, 22.5 km by default, plus 5 km for the size of a static. A craft
  at the KSC takes the KSC out, and leaves the Island Airfield, 33 km away, under its sphere. Only the
  statics that hang directly from their sphere are handled, as those of stock, of the Making History
  expansion and of Kerbal Konstructs do: the KSC and the other stock statics, placed by `PQSCity`, and
  the launch sites of Making History, placed by `PQSCity2`.
- **Following the body.** Out of its sphere, a static no longer moves with its body: the fix places it
  again whenever the body moves or turns, in the same call where stock moves the quads. If something
  else moves it — an editor, say — it stays where it was put, relative to the body.
- **Back under its sphere.** A static goes back exactly where stock left it whenever it goes out of
  reach, before every scene change, and for the time stock code that expects it there runs. Outside
  flight, a static is always where stock puts it.

## The stock methods involved

What each stock method this fix patches is for in the game, as of KSP 1.12.5, in the order the fix
installs its patches, with the name of its patch in the code.

- **`PQSCity.Orientate`** (`OrientatePatch`) — places a static on its body: writes its position and
  rotation in the frame of the sphere. Called by `Start`, and before stock puts a craft on a launch site.
- **`PQSCity.Start`** (`StartPatch`) — starts a static: finds its body, then calls `Orientate`.
- **`PQSCity.ResetCelestialBody`** (`ResetCelestialBodyPatch`) — finds the body of a static again.
- **`PQSCity2.Orientate`** (`Orientate2Patch`) — the same as `PQSCity.Orientate`, for a launch site of
  Making History: places it on its body, and has its launch pad, if it has one, set itself on the ground.
  Called by `Start`, when the site comes into view, and before stock puts a craft on it.
- **`PQSCity2.Start`** (`Start2Patch`) — starts a launch site of Making History: finds its body, then
  calls `Orientate`.
- **`PQSCity2.SetBody`** (`SetBody2Patch`) — finds the body of a launch site of Making History again; a
  mission calls it on the launch pad it places.
- **`PositionMobileLaunchPad.CompleteOrientation`** (`CompleteOrientationPatch`) — sets a launch pad of
  Making History, such as the Desert Launch Site, on the ground: lifts it if one of its feet is under the
  ground, then stretches its legs down to the ground, both by casting rays from where it stands. Called
  by `PQSCity2.Orientate`.
- **`PQS.SetupMods`** (`SetupModsPatch`) — builds the list of the mods of a terrain sphere, `PQSCity`
  and `PQSCity2` included; the sphere calls the mods on this list as the terrain updates.
- **`CommNetHome.Start`** (`CommNetHomeStartPatch`) — starts a CommNet ground station, such as the one
  of the KSC: finds its body, to place it in the network.
- **`DayNightGameObjectSwitch.Setup`** (`DayNightSetupPatch`) — sets up a component that switches
  objects on and off with day and night: finds their body, to know the time of day where they are.
- **The setter of `PQS.PrecisePosition`** (`SphereMovedPatch`) — the position of a terrain sphere in
  the world, set whenever its body moves, such as when the floating origin shifts. It moves the quads a
  craft can stand on with it.
- **`CelestialBody.CBUpdate`** (`BodyRotatedPatch`) — updates a body at every frame: turns it, and
  moves it along its orbit.

## How the fix works

The base of the fix is not a patch: it is code of this mod, run at every frame. The patches of the
methods of [The stock methods involved](#the-stock-methods-involved) only come around it.

### At every frame: taking the statics out, putting them back

At every frame, from the `Update` of its addon, this mod goes through every static it knows of
(`StaticsFix.Update`). In flight — from the moment the flight scene is ready until a scene change is
asked for — it:

- takes out of its sphere a static that has come within reach of the craft, and places it in double;
- puts back under its sphere, exactly where stock left it, a static that has gone out of reach — 10 %
  farther than where it was taken out, so that a craft hovering at the limit does not move it back and
  forth at every frame;
- places again a static already out, if its body moved since it was last placed in a way none of the
  patches below saw.

When a scene change is asked for, every static goes back under its sphere, and none is taken out again
until the next flight scene is ready.

The patches are there for four things around this. None of the methods they patch is changed: the fix
only acts just before or just after them.

### Learning of the statics

- **`PQSCity.Orientate`** (`OrientatePatch`) and **`PQSCity2.Orientate`** (`Orientate2Patch`) — just
  after it, the static is added to those the code run at every frame goes through. Every static goes
  through one of these methods at least once, from its `Start`, so none is missed. This is the only
  part of the patches that runs while no static is out of its sphere, with the patch of
  [Measuring a launch pad against the ground](#measuring-a-launch-pad-against-the-ground).

### Keeping a static out of its sphere in place

Under its sphere, a static moves and turns with its body without anyone doing anything. Out of it, it no
longer does: the fix places it itself, in double, from its position in the frame of the sphere and the
current position and rotation of the body. These two patches are part of the fix itself: nothing in
stock would break without them, but the statics taken out would stay behind as their body moves.

- **The setter of `PQS.PrecisePosition`** (`SphereMovedPatch`) — just after it, every static out of
  this sphere is placed again. When the floating origin shifts, stock moves the quads a craft can stand
  on in this very call: placing the statics in it too moves them together with the ground, before
  physics, or anything else, sees one without the other.
- **`CelestialBody.CBUpdate`** (`BodyRotatedPatch`) — just after it, every static out of the sphere of
  this body is placed again, if the body turned or moved since it was last placed: it does at every
  frame while the craft is too high for the world to turn with the body.

Should the body move some other way, the code run at every frame places the static again.

### Lending the static back to stock code that expects it under its sphere

These nine methods would break with a static out of its sphere. Only one of them would lose the static
itself; the others would lose its body, or write its position in the wrong frame:

- **`PQS.SetupMods`** (`SetupModsPatch`) builds the list of the mods of a sphere from its children: a
  static out of it would drop off the list, and with it the calls that show it and load its levels of
  detail, until the list is built again.
- **`PQSCity.Start`** (`StartPatch`), **`PQSCity.ResetCelestialBody`** (`ResetCelestialBodyPatch`),
  **`PQSCity2.Start`** (`Start2Patch`), **`PQSCity2.SetBody`** (`SetBody2Patch`),
  **`CommNetHome.Start`** (`CommNetHomeStartPatch`) and **`DayNightGameObjectSwitch.Setup`**
  (`DayNightSetupPatch`) look for the body as the `CelestialBody` among the parents of the static, or of
  an object inside it: out of the sphere, there is none. The static would lose its body, a ground station
  could not be placed in the network, and the objects of a day and night switch would not follow day and
  night.
- **`PQSCity.Orientate`** (`OrientatePatch`) and **`PQSCity2.Orientate`** (`Orientate2Patch`) write
  the static's position and rotation in the frame of the sphere, as local values: out of the sphere, they
  would send the static far away. `PQSCity.Orientate` also finds its body among its parents when it has
  none.

So for each of them, the fix **lends the static back to stock** for the time of the call. Just before
the method runs, the static is put back under its sphere, with the local position and rotation stock
gave it (or, if something else moved it meanwhile, those of where it was moved). Just after, if it is
still within reach of a craft, it is taken out again, from the pose the method just gave it: a static
that stock placed again with `Orientate` — before a craft is spawned on its launch site, for instance —
keeps its new place. For `SetupMods`, every static out of the sphere is lent back; for
`CommNetHome.Start` and `DayNightGameObjectSwitch.Setup`, the static the station or the switch belongs
to. `PQSCity.Start` and `PQSCity2.Start` are normally run before a static can ever be taken out: their
patches cover the case where they are not. A mission calls `PQSCity2.SetBody` a frame after it has placed
a launch pad, by which time the pad may be out of its sphere.

### Measuring a launch pad against the ground

A launch pad of Making History, such as the Desert Launch Site, sets itself on the ground by casting
rays from where it stands: it lifts itself if one of its feet is under the ground, then stretches its
legs down to it (`PositionMobileLaunchPad.CompleteOrientation`). It does so while a craft is being
launched from it, before the flight scene is ready, so before the fix takes any static out of its sphere:
under its sphere, the pad would measure the ground from a rounded position, and once taken out, its
feet would end above the ground or inside it by as much.

- **`PositionMobileLaunchPad.CompleteOrientation`** (`CompleteOrientationPatch`) — just before it, in
  flight, the launch pad is taken out of its sphere and placed in double, even though the flight scene
  is not ready yet; just after it, it is put back under its sphere, and the height it lifted itself to
  is kept in double, as its position in the frame of the sphere: when it is taken out next, it goes back
  to that height exactly.

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
