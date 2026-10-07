# Scene changes

Part of [Terrain Precision Fix](../../../README.md), one test of [Non-regression tests](../../non-regression.md), on [stock](../../non-regression.md#stock).

**Status: checked on Kerbin and on Earth in Real Solar System.** With this mod, after every way of
leaving a flight and coming back to the craft on the launchpad, the launchpad is where it was, within
0.13 mm; without it, it moves by up to 140 mm on Kerbin and 1 127 mm on Earth. Every building of the
space centre opens.

*Why.* This mod takes the KSC out of its terrain sphere in flight, near a craft, and has to put it back
before every scene change: the next scene, and the next flight, look for it under its sphere
([The fix: the statics](../../the-fix-statics.md)). A craft leaving for another body takes the KSC out of
its reach: it goes back under its sphere on the way, and has to come out again when the player switches
back to a craft near it.

## The test

A new career game, at the Normal difficulty, a craft on the launchpad. In turn, leaving the flight and
coming back to the craft each time:

1. *Revert to Launch*, twice;
2. a quicksave (F5), then a quickload of it (F9), twice;
3. the space centre: the Astronaut Complex, the Research and Development, Mission Control and the
   Administration, each opened and closed; then the Tracking Station, entered through its building, and
   the craft flown from there;
4. the space centre again, the Vehicle Assembly Building entered through its building, and a launch;
5. *Revert to Vehicle Assembly Building*, and a launch;
6. *Recover*, and a launch from the space centre;
7. *Quit to Main Menu*, and the game loaded back into the flight of the craft;
8. a second craft launched onto the runway from the space centre, then put on a circular orbit 100 km
   above the Mun (`Alt+F12 → Cheats → Set Orbit`; on Earth, the Moon), and the player switched back to
   the craft on the launchpad from the map view (*Switch To*).

That makes eleven arrivals in flight, with the launch that starts the game. At each one, once the craft has
settled, [KSP Diag - Colliders](https://github.com/lhervier/KSP-Diag-Colliders) logs the colliders under
each craft (its button *Log the colliders under each craft*): the launchpad's, with its height above the
terrain the game computes there. In a sandbox game, three of the buildings of step 3 only show a dialog
saying they are closed in that mode: hence the career.

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1, KSP Diag - Colliders, and this mod
with its defaults at `logLevel = Debug`; on Earth, Real Solar System 20.1.3.0 and what it requires
(Kopernicus 248, Modular Flight Integrator, KSPTextureLoader, the RSS textures). Then the same without this
mod.

*Played by a script.* [KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer) plays the test through
[`diag/automation/run-scene-changes.py`](../../../diag/automation/run-scene-changes.py), with
[`Diag3-Rocket.craft`](https://github.com/lhervier/KSP-Diag-FloatingOrigin/blob/main/craft/Diag3-Rocket.craft)
in the `Ships/VAB` folder of the game, for both craft: it starts the career itself from the main menu, and closes the
guide of a new career by its button wherever it shows. Each way of leaving the flight is a script of its
own, in [`diag/automation/scene-changes/`](../../../diag/automation/scene-changes/), that can also be played
alone. Every step is one a player can take: the test
plays just as well by hand, pressing *Log the colliders under each craft* at each arrival.

## The result

The height of the launchpad under the craft, lowest and highest over the eleven arrivals:

| | without this mod | with this mod |
|---|---|---|
| Kerbin | 2 675.643 to 2 815.769 mm (140.126 mm) | 2 747.132 to 2 747.254 mm (0.122 mm) |
| Earth | 3 102.290 to 4 229.275 mm (1 126.984 mm) | 3 581.683 to 3 581.792 mm (0.109 mm) |

Back from the other body, the launchpad reads 2 730.054 mm on Kerbin and 3 752.959 mm on Earth without
this mod, 2 747.197 mm and 3 581.792 mm with it: the same as the arrival before it on Kerbin, within
0.071 mm on Earth.

Without this mod, KSP places the KSC again at every arrival, rounded each time in a different way
([The culprits: the statics](../../the-culprit-statics.md)). With it, the KSC comes back to the same place,
whichever way the flight was left.

Before ten of the eleven exits from the flight, the log shows the KSC put back, just before the scene
change; after each arrival, taken out again:

```
[TerrainPrecisionFix] [DEBUG] Kerbin static 'KSC': back under its sphere
[HighLogic]: =========================== Scene Change : From FLIGHT to SPACECENTER =====================
```

Before the last one, the switch back from the other body, the KSC is already under its sphere: it went
back as soon as the second craft reached the Mun, out of its reach. It comes out again once the flight
is open on the craft on the launchpad. Stripped of the rest:

```
setting new dominant body: Mun
[TerrainPrecisionFix] [DEBUG] Kerbin static 'KSC': back under its sphere
[HighLogic]: =========================== Scene Change : From FLIGHT to FLIGHT =====================
[TerrainPrecisionFix] [DEBUG] Kerbin static 'KSC': out of its sphere, corrected by 45.43 mm
```

The logs show the same errors with this mod as without it, but for one that comes in some runs and not in
others: in two of the visits to the space centre, KSP's camera there does not find its place
(`SpaceCenterCamera: Cannot find transform of name 'KSC/SpaceCenter/SpaceCenterCameraPosition'`), then
throws a `NullReferenceException` at every frame from `SpaceCenterCamera2.UpdateTransformOverview`. On
Kerbin it shows without this mod as with it; on Earth, only with it. At the space centre, every building
opens and closes as without this mod, and the recovery report places the craft on the launchpad.

The logs, what the script printed and every reading of KSP Diag - Colliders are in
[`diag/runs/`](../../../diag/README.md#the-scene-changes-protocol).
