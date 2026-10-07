# Time warp

Part of [Terrain Precision Fix](../../../README.md), one test of [Non-regression tests](../../non-regression.md), on [stock](../../non-regression.md#stock).

**Status: checked on Kerbin.** A craft on the runway, three days of time warp at the highest rate, then a
night and a day: the KSC stays out of its sphere throughout, and the runway stays within 0.04 mm under the
craft, as without this mod.

*Why.* Time warp does not turn the body under a static this mod has taken out of its sphere. Whether KSP
turns the body or the world around the craft depends on the craft's altitude alone, not on the time warp
(`OrbitPhysicsManager.checkReferenceFrame`); on Kerbin, the world turns as long as the craft is in the
atmosphere, and a static is out of its sphere only within 27.5 km of the craft
([A static turning with its body](a-static-turning-with-its-body.md)). Nor does the floating origin move:
at the highest rates KSP moves it at every frame, but not for a craft landed or splashed
(`FloatingOrigin.FixedUpdate`). So on the ground, time warp gives this mod nothing to do. What it does
change: the craft goes on rails, then off rails onto the runway; and days and nights go by, which switch
lights of the KSC on and off ([The lights of the KSC](the-lights-of-the-ksc.md)). The test checks that
nothing in them moves the KSC, or puts it back under its sphere.

## The test

A new sandbox game, its universal time set to 3 600 s, in daylight at the KSC. Then:

1. [`Diag3-Rover.craft`](https://github.com/lhervier/KSP-Diag-FloatingOrigin/blob/main/craft/Diag3-Rover.craft)
   launched from the SPH onto the runway, its brakes put on at once;
2. time warp on rails at the highest rate, 100 000×, with the `.` key, for three days of Kerbin, then back
   to normal time with the `,` key;
3. *Warp To* the night at the KSC, nine tenths into its day (noon being half);
4. *Warp To* the next noon at the KSC.

At the launch and at the end of each warp, once the craft has settled,
[KSP Diag - Colliders](https://github.com/lhervier/KSP-Diag-Colliders) logs the colliders under it (its
button *Log the colliders under each craft*): the runway's, with its height above the terrain the game
computes there.

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1, KSP Diag - Colliders, and this mod
with its defaults at `logLevel = Debug`. Then the same without this mod.

*Played by a script.* [KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer) plays the test through
[`diag/automation/run-time-warp.py`](../../../diag/automation/run-time-warp.py), the rover in the
`Ships/SPH` folder of the game: it starts the game itself from the main menu. *Warp To* is the one of the
map view, or of an alarm. Every step is one a player can take: the test plays just as well by hand.

## The result

In the log with this mod, the KSC is taken out of its sphere once, as the rover arrives on the runway, and
goes back under it only when the game leaves the flight for the space centre, after the test. Nothing in
between, through the three days at 100 000× and the two *Warp To*:

```
[TerrainPrecisionFix] [DEBUG] Kerbin static 'KSC': out of its sphere, corrected by 106.87 mm
Warping to UT:137318.0. Max Rate Allowed: 5.0x.
Real-time resumed. Current UT is: 137318.0. TgtUT Error is -0.0063s. (Undershot)
Warping to UT:150278.0. Max Rate Allowed: 5.0x.
Real-time resumed. Current UT is: 150278.0. TgtUT Error is -0.0085s. (Undershot)
[TerrainPrecisionFix] [DEBUG] Kerbin static 'KSC': back under its sphere
```

The height of the runway under the rover, lowest and highest over its four readings:

| | without this mod | with this mod |
|---|---|---|
| runway | 4 299.942 to 4 299.984 mm (0.042 mm) | 4 301.354 to 4 301.385 mm (0.031 mm) |

The logs with this mod and without it show no error.

The logs, what the script printed and every reading of KSP Diag - Colliders, for both sessions, are in
[`diag/runs/`](../../../diag/README.md#the-time-warp-protocol).
