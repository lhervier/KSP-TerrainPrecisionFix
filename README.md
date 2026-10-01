# Terrain Precision Fix

**⚠️ Work in progress.** This is an active investigation, not a finished mod. The figures, the code and the conclusions on this page can still change, and several questions are still open — they are listed in [Limits and solutions](docs/limits-and-solutions.md).

A fix for stock KSP 1.12, kept as small as possible: a handful of Harmony patches, in a few short source
files. It is a mod of its own, built and measured on top of
[KSP Community Fixes](https://github.com/KSPModdingLibs/KSPCommunityFixes), the base most players run.
Here is what it fixes, on the ground and on the runways, launchpads and buildings that stand on it:

> **The ground KSP builds under you is never built at the same height twice.** Load a save, come back
> to a craft left parked, drive a few hundred metres: each time the ground is built again, the surface
> your craft is standing on comes back a little higher or a little lower — a few centimetres apart on
> Kerbin, less on smaller worlds, and up to seventy on Earth in Real Solar System.

**How this was made.** The investigation and the code were written with Claude, Anthropic's AI
assistant. Everything here was reviewed and validated by a human — me — who very much enjoyed
learning along the way how KSP builds the ground you land on. I am saying so up front, because
contributions made with an AI deserve a closer look than others, and because some people would
rather stop reading here. That look is what this page is built for: every figure on it comes from an
in-game measurement, the instrument behind the headline figures is public and runs on a stock
install, the stock code quoted here is a handful of lines anyone can check, and the fix is a few short
files you can read in one sitting.

## Why the moving ground matters

KSP does not keep the ground it built: it builds it again, over and over, while you play. And every
time it does, the surface lands at a slightly different height — a little higher or a little lower,
at random, a few centimetres on Kerbin and up to seventy on Earth in Real Solar System. Loading a save
is only the moment everybody notices. It happens just as well:

- **when a save is loaded**, or the scene changes: the whole ground is built anew;
- **when you come back to a craft left parked.** A landed craft you fly towards is loaded long before
  you reach it, but held still at the position it was left at; its physics only starts once you are
  within 200 m, on a ground built since that position was recorded;
- **when you switch to a craft far away**, which is put down the same way, on a ground it was not
  standing on when it was left;
- **while you drive.** Every 500 m the craft you fly travels, KSP moves its whole world back onto it,
  and builds the ground under you again — in the middle of the drive, with nothing loaded and no scene
  changed;
- **under the runway, the launchpad and the buildings of the KSC**, and the bases a mod such as Kerbal
  Konstructs plants on a body: they are placed the same way, and move on their own, at every load and
  while a rover drives by them.

Each time, it is a coin toss between two outcomes.

**The ground comes back lower than it was.** Your craft is now hovering a couple of centimetres above
it, so it drops those two centimetres. You never notice, and nothing breaks.

**The ground comes back higher than it was.** Your craft is now *inside* the ground — and the physics
engine will not leave two solid things overlapping. It pushes them apart, hard, in the only direction
available: up. Your craft gets launched.

![A craft jumping on its own the moment a save is reloaded](imgs/Booing-scaled.gif)

*KSP 1.12 without this mod, with [KSP Community Fixes](https://github.com/KSPModdingLibs/KSPCommunityFixes)
as the only mod installed. A pod on an empty fuel tank, parked in the grass at the KSC, saved, then reloaded
from the pause menu, several times if needed — nothing touched in between.*

That second case is the symptom everybody already knows. The lander that twitches, hops or flips the
moment the scene finishes loading. The base that sat perfectly flush yesterday and is buried up to
the hatches today. The big base that tears itself apart the very first time you load it, and never
again afterwards. A craft with many parts spread over a wide area gives the coin toss more chances
to land the wrong way up. The other moments do the same damage, only less visibly: from 200 m away you
see little of a parked craft jumping, and a rover that hops while driving looks like a bump in the
road — on Earth, it was seen to jump when the ground rose under it.

Every one of these moments can be repeated at will — reload the same save, drive away from a craft left
parked and come back to it, switch to a craft far away, drive past a few shifts of the world — and each
is measured on this page.

### Disclaimer: it is not the only cause

The ground moving is one cause among several, and this page does not claim it is the only one. Plenty
of other things move a craft when a scene opens. Two well-known examples, among others:

- **suspensions.** Landing legs and wheels come back fully extended, because that is the only state
  KSP can restore them to. They then compress under the weight of the craft, and the craft moves
  while they do.
- **a craft bent to fit the ground.** While you play, physics twists the joints between parts so the
  craft settles onto the shape of the ground beneath it. That twisting is not saved. On loading, the
  craft comes back in its original, unbent shape — and if the ground is not flat, part of it really
  *is* underground, with no measurement error involved.

This mod removes that one cause, and only that one: a lander that hops because its legs are still
unfolding will go on hopping once it is installed. What it takes away is the part that should never
have been there at all — a surface that is not where the game's own formulas say it is, and is not in
the same place twice. It takes it away down to a hundredth of a millimetre on the ground, and two tenths
on the runway of the KSC, measured below.

## The culprit

Stock places a terrain quad, and every vertex inside it, through a Unity `Transform`: a vector 600 km
long stored in a float, where a step is 62.5 mm. Each value is rounded on its own. What draws a new set
of roundings at every load is the frame they go through, whose rotation and translation both move while
you play.

The statics are a second culprit: the runway, the launchpad and the buildings of the KSC, and the bases
a mod such as Kerbal Konstructs plants anywhere on a body. `PQSCity` places them the same way, a 600 km
vector in a float `Transform` hanging from the body, through the same frame. Unlike the quads a craft
stands on, stock gives them no place outside the body where a precise position would be kept.

**→ Full chapter: [The culprit](docs/the-culprit.md)**

## Checking the culprit

Each situation where a craft meets the ground is measured twice, without this mod and with it, with two
instruments: one reads the landed craft, the other the ground itself. In each summary below, the first
figure is without this mod, the second with it.

**Loading the same save**, six times, on Kerbin, the Mun, Minmus, Gilly, and the Moon and Earth of
[Real Solar System](https://github.com/KSP-RO/RealSolarSystem): the ground comes back over 108.1 mm on
Kerbin and 693.1 mm on Earth, and within 0.4 mm everywhere with this mod.

**→ Full chapter: [Checking the culprit: loading the same save](docs/checking-the-culprit-loading.md)**

**Coming back to a craft left parked**, driving away until it unloads, then back, six times in one
flight: its ground comes back over 21.8 mm, and over 0.011 mm with this mod.

**→ Full chapter: [Checking the culprit: coming back to a craft left parked](docs/checking-the-culprit-approach.md)**

**Switching to a craft far away**, six loadings: its ground spreads over 120.4 mm, and 0.003 mm with this
mod; the switch itself moves nothing.

**→ Full chapter: [Checking the culprit: switching to a craft far away](docs/checking-the-culprit-switching.md)**

**Driving on while the world moves**, every 500 m a rover drives: the ground under it jumps by 4 to 12 mm
on Kerbin and 14 to 42 cm on Earth, where the rover jumps with it; with this mod, no jump is left.

**→ Full chapter: [Checking the culprit: driving on while the world moves](docs/checking-the-culprit-driving.md)**

**The runway and the grass beside it**, for the second culprit, the statics: at every loading, the runway
comes back over 130.1 mm, on its own, and over 0.216 mm with this mod; while a rover drives by it, the
runway moves with the ground by up to 48.5 mm, and within 0.05 mm with this mod — though it still moves
with its terrain fix alone.

**→ Full chapter: [Checking the culprit: the runway and the grass beside it](docs/checking-the-culprit-runway.md)**

## What moves the frame

The rounding is drawn anew because the frame the ground is converted through — how the body is turned
in Unity's world, and where it sits in it — does not stay put. An instrument,
[Terrain Precision Fix Diag 3](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag3), reads that
frame on a stock install: it moves at every load, on the way to orbit and back, and on the ground as a
craft is driven, none of which the save records. That is why putting it back in place at loading would
not be enough ([why, in The fix this mod proposes](docs/the-fix-this-mod-proposes.md#two-ways-out-one-taken)).

**→ Full chapter: [What the measurements show](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag3/blob/master/docs/what-the-measurements-show.md), on the page of Diag 3**

## The fix this mod proposes

For the ground, two Harmony patches redo in double the two placements that go through a float at planet
scale: the origin of each quad, and each vertex inside it. The two 600 km vectors cancel before anything
reaches a float, which is then only asked to hold a distance within the quad. Only the quads a craft can
stand on are touched.

For the statics, which cannot hold a precise position under their sphere, a static is taken out of it
in flight, while a craft is near it, and placed in double in the same frame. It follows its body, and it
goes back exactly where stock left it before every scene change, and whenever stock code that expects it
there runs. Kerbal Konstructs and Kopernicus each look for a static under its sphere once in flight: the
mod patches both, each patch standing for a small change these mods could make themselves. The one in
Kopernicus is its fix for a flag that twitches on big home bodies — and that glitch comes from the KSC
hanging from its sphere: placed in double, the KSC holds its flag without that fix.

A correction larger than sixteen float steps at the distance of what is placed (1 m on Kerbin) is
refused. The ground and the statics are two fixes: each can be turned off in the settings, and if one of
its patches fails to install, none of them does anything.

**→ Full chapter: [The fix this mod proposes](docs/the-fix-this-mod-proposes.md)**

## Performance

Measured with [PQS Bench](https://github.com/lhervier/KSP-PQSBench), against stock and against
stock with its two `Transform`s read once per quad: the fix places a vertex in about 110 ns where stock
takes about 290 ns in the same run, 183 ns less on average, 2.7× faster. A little more than half of that
saving is the double-precision arithmetic, the rest the work done once per quad instead of once per
vertex. Timed frame by frame with [KSPProfiler](https://github.com/KSPModdingLibs/KSPProfiler), the
three configurations cannot be told apart. These figures are the ground's: what the statics fix costs is
not measured yet.

**→ Full chapter: [Performance](docs/performance.md)**

## Limits and solutions

Everything that stands on the ground, or is placed from it, has to be checked against this fix, one
case at a time — a work in progress, with a chapter per case. The cases go from other mods (KSP
Community Fixes, Kopernicus and planet packs, Parallax, Kerbal Konstructs, Deferred…) to what stock
itself places on the ground (scatter, the KSC statics, Breaking Ground) and to situations (rescaled
systems such as Real Solar System, slopes, existing saves, the map view…). Taking the statics out of
their sphere adds a case of its own: mods that look for a static where stock puts it. So far, every
case checked shows no problem caused by this fix — including Real Solar System, where the defect is at
its largest, and Kopernicus, whose fix for a twitching flag the statics fix makes unneeded where it
moves the KSC. Some cases say what changes, and what can go further; the others say how they will be
tested.

**→ Full chapter: [Limits and solutions](docs/limits-and-solutions.md)**

## Install

Requires KSP 1.12 and [HarmonyKSP](https://github.com/KSPModdingLibs/HarmonyKSP) (the usual
`GameData/000_Harmony`, also installed by KSP Community Fixes).

Copy `GameData/TerrainPrecisionFixMod` into the `GameData` of KSP. Nothing is written to your saves:
removing the folder gives you the stock terrain and statics back.

## Settings

`GameData/TerrainPrecisionFixMod/PluginData/settings.cfg` holds five values, read when KSP starts. To
change one: quit KSP, edit the file, start KSP again.

| setting | what it does |
|---|---|
| `fixTerrain` | `true` (default) places the terrain in double precision; `false` leaves it as stock builds it |
| `fixStatics` | `true` (default) places the statics in double precision; `false` leaves them where stock places them |
| `patchKopernicus` | `true` (default) patches Kopernicus, when installed, to cope with the statics fix. With `false` and the statics fix on, Kopernicus' flag fix throws when a facility is upgraded in flight near the KSC, which only a Making History mission does: an error in the log, and nothing else, since the statics fix keeps the flags steady there. Meant only to see what the patch is for |
| `patchKerbalKonstructs` | `true` (default) patches Kerbal Konstructs, when installed, to cope with the statics fix. With `false` and the statics fix on, moving a group with its group editor in flight sends it elsewhere on its body, and saves it there: meant only to see what the patch is for |
| `logLevel` | what goes to `KSP.log`, below |

| `logLevel` | what goes to `KSP.log` |
|---|---|
| `Info` (default) | a few lines at startup, then one line per body the first time its terrain, then its statics, are corrected |
| `Debug` | adds one line per quad placed, with how far it was moved, and one line per static taken out of its sphere or put back under it |
| `Trace` | adds, per quad, how far its vertices were moved within it — slower, meant for measuring |

`Error` and `Warning` are accepted too.

## Build

Set `KSPDIR` to your KSP install folder, which must contain `GameData/000_Harmony`, and run `build.bat`.
It needs the .NET SDK, and produces `GameData/TerrainPrecisionFixMod/TerrainPrecisionFixMod.dll`.

## License

MIT
