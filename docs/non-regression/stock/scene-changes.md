# Scene changes

Part of [Terrain Precision Fix](../../../README.md), one test of [Non-regression tests](../../non-regression.md), on [stock](../../non-regression.md#stock).

**Status: checked on Kerbin and on Earth in Real Solar System.** With this mod, after every way of
leaving a flight and coming back to the craft on the launchpad, the launchpad is where it was, within
0.14 mm; without it, it moves by up to 115 mm on Kerbin and 963 mm on Earth. Every building of the space
centre opens.

*Why.* This mod takes the KSC out of its terrain sphere in flight, near a craft, and has to put it back
before every scene change: the next scene, and the next flight, look for it under its sphere
([The fix: the statics](../../the-fix-statics.md)).

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
7. *Quit to Main Menu*, and the game loaded back into the flight of the craft.

That makes ten arrivals in flight, with the launch that starts the game. At each one, once the craft has
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
in the `Ships/VAB` folder of the game: it starts the career itself from the main menu, and closes the
guide of a new career by its button wherever it shows. Every step is one a player can take: the test
plays just as well by hand, pressing *Log the colliders under each craft* at each arrival.

## The result

The height of the launchpad under the craft, lowest and highest over the ten arrivals:

| | without this mod | with this mod |
|---|---|---|
| Kerbin | 2 663.639 to 2 778.298 mm (114.659 mm) | 2 747.131 to 2 747.241 mm (0.110 mm) |
| Earth | 3 102.291 to 4 065.248 mm (962.957 mm) | 3 581.656 to 3 581.792 mm (0.136 mm) |

Without this mod, KSP places the KSC again at every arrival, rounded each time in a different way
([The culprits: the statics](../../the-culprit-statics.md)). With it, the KSC comes back to the same place,
whichever way the flight was left.

Before each of the nine exits from the flight, the log shows the KSC put back, just before the scene
change; after each arrival, taken out again:

```
[TerrainPrecisionFix] [DEBUG] Kerbin static 'KSC': back under its sphere
[HighLogic]: =========================== Scene Change : From FLIGHT to SPACECENTER =====================
```

The logs show the same errors with this mod as without it. At the space centre, every building opens
and closes as without this mod, and the recovery report places the craft on the launchpad.

The logs, what the script printed and every reading of KSP Diag - Colliders are in
[`diag/runs/`](../../../diag/README.md#the-scene-changes-protocol).
