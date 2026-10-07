# Scene changes

Part of [Terrain Precision Fix](../../../README.md), one test of [Non-regression tests](../../non-regression.md), on [stock](../../non-regression.md#stock).

**Status: checked on Kerbin and on Earth in Real Solar System.** With this mod, after every way of
leaving a flight and coming back to the craft on the launchpad, the launchpad is where it was, within
0.2 mm; without it, it moves by up to 149 mm on Kerbin and 1 127 mm on Earth. A craft launched from the
VAB stands on the launchpad, one launched from the SPH on the runway, and every building of the space
centre opens.

*Why.* This mod takes the KSC out of its terrain sphere in flight, near a craft, and has to put it back
before every scene change: the next scene, and the next flight, look for it under its sphere
([The fix: the statics](../../the-fix-statics.md)): KSP finds the buildings of the KSC by their place
in the hierarchy below it, and places a new craft on a spawn point that hangs from it, on the launchpad or
on the runway. A craft leaving for another body takes the KSC out of
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
8. the space centre again, the Space Plane Hangar entered through its building, and a second craft, a
   rover, launched from it onto the runway; the rover then put on a circular orbit 100 km above the Mun
   (`Alt+F12 → Cheats → Set Orbit`; on Earth, the Moon), and the player switched back to the craft on
   the launchpad from the map view (*Switch To*).

That makes eleven arrivals of the craft on the launchpad, with the launch that starts the game, and one of
the rover on the runway. At each one, once the craft has settled,
[KSP Diag - Colliders](https://github.com/lhervier/KSP-Diag-Colliders) logs the colliders under each craft
(its button *Log the colliders under each craft*): the launchpad's or the runway's, with its height above
the terrain the game computes there. In a sandbox game, three of the buildings of step 3 only show a dialog
saying they are closed in that mode: hence the career.

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1, KSP Diag - Colliders, and this mod
with its defaults at `logLevel = Debug`; on Earth, Real Solar System 20.1.3.0 and what it requires
(Kopernicus 248, Modular Flight Integrator, KSPTextureLoader, the RSS textures). Then the same without this
mod.

*Played by a script.* [KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer) plays the test through
[`diag/automation/run-scene-changes.py`](../../../diag/automation/run-scene-changes.py), with
[`Diag3-Rocket.craft`](https://github.com/lhervier/KSP-Diag-FloatingOrigin/blob/main/craft/Diag3-Rocket.craft)
in the `Ships/VAB` folder of the game and
[`Diag3-Rover.craft`](https://github.com/lhervier/KSP-Diag-FloatingOrigin/blob/main/craft/Diag3-Rover.craft)
in its `Ships/SPH` folder: it starts the career itself from the main menu, and closes the
guide of a new career by its button wherever it shows. Each way of leaving the flight is a script of its
own, in [`diag/automation/scene-changes/`](../../../diag/automation/scene-changes/), that can also be played
alone. Every step is one a player can take: the test
plays just as well by hand, pressing *Log the colliders under each craft* at each arrival.

## The result

The height of the launchpad under the craft, lowest and highest over its eleven arrivals:

| | without this mod | with this mod |
|---|---|---|
| Kerbin | 2 687.927 to 2 837.068 mm (149.140 mm) | 2 747.128 to 2 747.241 mm (0.113 mm) |
| Earth | 3 102.290 to 4 229.274 mm (1 126.984 mm) | 3 581.669 to 3 581.862 mm (0.192 mm) |

Back from the other body, the launchpad reads 2 825.781 mm on Kerbin and 3 650.495 mm on Earth without
this mod, 2 747.128 mm and 3 581.690 mm with it: within 0.047 mm of the arrival before it on Kerbin, and
0.008 mm on Earth.

The rover, launched from the SPH, stands on the runway: the runway is under it, 4 149.754 mm above the
terrain on Kerbin and 3 798.226 mm on Earth without this mod, 4 062.424 mm and 3 861.214 mm with it. One
launch from the SPH per session gives one reading, not a spread.

Without this mod, KSP places the KSC again at every arrival, rounded each time in a different way
([The culprits: the statics](../../the-culprit-statics.md)). With it, the KSC comes back to the same place,
whichever way the flight was left.

Only the statics within reach of the craft are taken out: on Kerbin, the KSC alone, the Island Airfield,
33 km away, staying under its sphere; on Earth, the KSC and the tracking station of Cape Canaveral.
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
[TerrainPrecisionFix] [DEBUG] Kerbin static 'KSC': out of its sphere, corrected by 68.76 mm
```

The logs with this mod show no error that the logs without it do not show. At the space centre, every
building opens and closes as without this mod, and the recovery report places the craft on the
launchpad.

The logs, what the script printed and every reading of KSP Diag - Colliders are in
[`diag/runs/`](../../../diag/README.md#the-scene-changes-protocol).
