# Terrain Precision Fix

**⚠️ Work in progress.** This is an active investigation, not a finished mod. The figures, the code and the conclusions on this page can still change, and several questions are still open — they are listed in [Limits and solutions](docs/limits-and-solutions.md).

A fix for stock KSP 1.12, kept as small as possible: a handful of Harmony patches, in a few short source
files. It is a mod of its own, built and measured on top of
[KSP Community Fixes](https://github.com/KSPModdingLibs/KSPCommunityFixes), the base most players run.
Here is what it fixes, on the ground and on the runways, launchpads and buildings that stand on it:

> **The ground KSP builds under you is never built at the same height twice, and neither are the
> runways and buildings it places on it.** Load a save, come back to a craft left parked, drive a few
> hundred metres: each time the ground is built again, the surface your craft is standing on comes back
> a little higher or a little lower — a few centimetres apart on Kerbin, less on smaller worlds, and up
> to seventy on Earth in Real Solar System.

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
at random, a few centimetres on Kerbin and up to seventy on Earth in Real Solar System. The runway, the
launchpad and the buildings of the KSC, and the bases a mod such as Kerbal Konstructs plants on a body,
are placed the same way, each at a height of its own, apart from the ground beside it. Loading a save
is only the moment everybody notices. It happens just as well:

- **when a save is loaded**, or the scene changes: the whole ground is built anew, and every runway and
  building is placed again;
- **when you come back to a craft left parked.** Its position was recorded while it rested on the
  ground as it stood then. As you fly back towards it, KSP builds that ground again, at a different
  height, and puts the craft back at its recorded position — on a ground that is no longer the one it
  was resting on;
- **when you switch to a craft far away**, which is put down the same way, on a ground it was not
  standing on when it was left;
- **while you drive.** Every 500 m the craft you fly travels, KSP moves its whole world back onto it,
  builds the ground under you again, and moves the runway beside you with it — in the middle of the
  drive, with nothing loaded and no scene changed.

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
is measured on this page, on the ground, and on a runway as well when a save is loaded and while you
drive.

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

## The culprits

**The ground.** Stock places a terrain quad, and every vertex inside it, through a Unity `Transform`: a
vector 600 km long stored in a float, where a step is 62.5 mm. Each value is rounded on its own. What
draws a new set of roundings at every load is the frame they go through, whose rotation and translation
both move while you play.

**→ Full chapter: [The culprit: the ground](docs/the-culprit-ground.md)**

**The statics.** The runway, the launchpad and the buildings of the KSC, and the bases a mod such as
Kerbal Konstructs plants anywhere on a body. `PQSCity` places them the same way, a 600 km vector in a
float `Transform` hanging from the body, through the same frame. Unlike the quads a craft stands on,
stock gives them no place outside the body where a precise position would be kept.

**→ Full chapter: [The culprit: the statics](docs/the-culprit-statics.md)**

## Checking the culprit

Each situation where a craft meets the ground is measured twice, without this mod and with it, with two
instruments: one reads the landed craft, the other the ground itself. In each summary below, the first
figure is without this mod, the second with it.

**Loading the same save**, six times, on Kerbin, the Mun, Minmus, Gilly, and the Moon and Earth of
[Real Solar System](https://github.com/KSP-RO/RealSolarSystem): the ground comes back over 73.7 mm on
Kerbin and 292.3 mm on Earth, and within 0.3 mm everywhere with this mod. For the second culprit, the
statics, on the runway of the KSC and on one placed by Kerbal Konstructs on the Mun: the runway comes back
over 116.5 mm, on its own, and over 0.177 mm with this mod.

**→ Full chapter: [Checking the culprit: loading the same save](docs/checking-the-culprit-loading.md)**

**Coming back to a craft left parked**, driving away until it unloads, then back, six times in one
flight: its ground comes back over 49.3 mm, and over 0.010 mm with this mod.

**→ Full chapter: [Checking the culprit: coming back to a craft left parked](docs/checking-the-culprit-approach.md)**

**Switching to a craft far away**, six loadings: its ground spreads over 112.1 mm, and 0.002 mm with this
mod; the switch itself moves nothing.

**→ Full chapter: [Checking the culprit: switching to a craft far away](docs/checking-the-culprit-switching.md)**

**Driving on while the world moves**, every 500 m a rover drives: the ground under it jumps by about
25 mm on Kerbin and 16 cm on Earth, where the rover jumps with it; with this mod, no jump is left. The
runway of the KSC moves with the ground by up to 54.8 mm on Kerbin and 232 mm on Earth, and within 0.07
and 0.12 mm with this mod — though it still moves with its terrain fix alone.

**→ Full chapter: [Checking the culprit: driving on while the world moves](docs/checking-the-culprit-driving.md)**

## What moves the frame

The rounding is drawn anew because the frame the ground is built in — how the body is turned in
Unity's world, and where its terrain sphere sits in it — does not stay put.
[KSP Diag - Floating Origin](https://github.com/lhervier/KSP-Diag-FloatingOrigin) shows the
values of KSP's floating origin, on a stock install. Its measurements show that a save does not give
that frame back, and that it changes during a flight with nothing loaded.

**→ Full chapter: [What the measurements show](https://github.com/lhervier/KSP-Diag-FloatingOrigin/blob/master/docs/what-the-measurements-show.md), on the page of Diag FloatingOrigin**

## The fix this mod proposes

**The ground.** Two Harmony patches redo in double the two placements that go through a float at planet
scale: the origin of each quad, and each vertex inside it. The two 600 km vectors cancel before anything
reaches a float, which is then only asked to hold a distance within the quad. Only the quads a craft can
stand on are touched.

**→ Full chapter: [The fix: the ground](docs/the-fix-ground.md)**

**The statics.** A static cannot hold a precise position under its sphere, so it is taken out of it in
flight, while a craft is near it, and placed in double in the same frame. It follows its body, and it
goes back exactly where stock left it before every scene change, and whenever stock code that expects it
there runs. Kerbal Konstructs and Kopernicus each look for a static under its sphere once in flight: **this
mod has to patch both**, each patch standing for a small change these mods could make themselves.

**→ Full chapter: [The fix: the statics](docs/the-fix-statics.md)**

## Performance

**The fix does not slow the game down.** Timed frame by frame with
[KSPProfiler](https://github.com/KSPModdingLibs/KSPProfiler), on the same flight, stock and this fix cannot
be told apart: their runs average the same, and three runs of the fix alone differ more than it differs
from stock. Measured vertex by vertex with [PQS Bench](https://github.com/lhervier/KSP-PQSBench), the fix
even places a vertex faster than stock, but that saving is about 0.04 % of the time played, seven to
twenty-five times below the spread among runs of one configuration: no frame shows it. What the statics
fix costs with statics near a craft is not measured yet.

**→ Full chapter: [Performance](docs/performance.md)**

## Limits and solutions

Everything that stands on the ground, or is placed from it, has to be checked against this fix, one
case at a time: other mods (Kopernicus, Parallax, Kerbal Konstructs…), what stock places on the ground
(scatter, the KSC statics, Breaking Ground) and situations (rescaled systems, slopes, existing saves…).
This is a work in progress, with a chapter per case. **The statics fix is the riskier of the two**: it
takes a static out of the place where stock, and any mod, expects to find it.

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
