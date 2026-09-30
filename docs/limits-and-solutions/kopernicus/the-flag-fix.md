# Kopernicus: the flag fix

Part of [Terrain Precision Fix](../../../README.md), one point of the case [Kopernicus](../kopernicus.md), in [Limits and solutions](../../limits-and-solutions.md).

**Status: checked, a problem this mod patches — without the patch, in one rare case only, an error is
logged and the flags of one facility are left unfixed until the next scene; their glitch was reported
on big home bodies, such as Earth in Real Solar System.** To
place the KSC in double, this mod takes it out of the home body's terrain sphere while a craft is near
([The statics](../../the-fix-this-mod-proposes.md#the-statics)), and Kopernicus looks for it there.

## Without the patch

With this mod's statics fix on and Kopernicus left unpatched (`patchKopernicus = false`), the flag fix
fails in one rare case only, and what it leaves undone is minor:

- **one rare case** — the flag fix runs a few frames after every scene opens, when the KSC is under its
  sphere, and it looks for the KSC in flight only when a facility is upgraded or repaired there. In
  play, facilities are upgraded and repaired from the space centre. The one exception is a mission of
  the Making History expansion that spawns a craft at a facility of the KSC while another craft is
  already near it: then the flag fix throws a `NullReferenceException`, which KSP catches and logs, and
  the game goes on;
- **what it leaves undone** — the flags that facility's new level brings are not fixed, until the flag
  fix runs again at the next scene. Their glitch, a flag that twitches and leaves its pole, was reported
  on big home bodies: Earth in Real Solar System, and Kerbin rescaled ten times
  ([Kopernicus issue #349](https://github.com/Kopernicus/Kopernicus/issues/349)). On a Kerbin of stock
  size, where the run of [Seeing it](#seeing-it) was taken, there is nothing to see: the flag by the
  launchpad does not move, fixed or not.

This mod patches Kopernicus all the same: the log stays clean, and the flags stay fixed.

## Where Kopernicus looks for the KSC

Kopernicus looks the KSC up among the children of the home body's terrain sphere in several places:
while the game loads (`Components/KSC.cs`); when the main menu, the space centre, the tracking station
or an editor opens (`RuntimeUtility/PreciseFloatingOrigin.cs`, `Patches/SpaceCenterCamera2_Start.cs`);
and in its flag fix. All of them but the flag fix run when this mod has put every static back under its
sphere, since it does so before every scene change.

## The patch

**What Kopernicus does.** `RuntimeUtility.FixFlags` binds the flags of the KSC to their bones again, a
few frames after every scene opens and whenever a facility is upgraded or repaired — a new level of a
facility brings flags of its own. It was written for
[Kopernicus issue #349](https://github.com/Kopernicus/Kopernicus/issues/349), a flag by the launchpad
that twitched and left its pole, reported in KSP 1.4 to 1.6, in Real Solar System and on a stock system
rescaled ten times. It looks the KSC up with
`GetComponentsInChildren<PQSCity>(true)` on the home body's terrain sphere, keeps the one named `KSC`,
and uses it without checking it was found.

**Why it matters here.** A facility can be upgraded in flight: when a mission of the Making History
expansion spawns a craft at a facility, it places the KSC again and sets that facility's level. If a
craft is near the KSC then, the KSC is out of its sphere, the lookup does not find it, and the flag fix
throws ([Without the patch](#without-the-patch)).

**What this mod does.** It replaces that one call, in `FixFlags` alone, with a lookup that returns what
`GetComponentsInChildren` returns, plus the statics this mod has taken out of a sphere hanging from the
component asked. As long as no static is out of its sphere — always, without this mod's statics fix —
the result is the same as before. If Kopernicus is installed and `FixFlags` does not hold exactly one such
call, the patch changes nothing, and the statics fix stays off. The patch can also be turned off in the
settings (`patchKopernicus = false`), to see what goes wrong without it.

**The change in Kopernicus it stands for.** In `FixFlags`, look the KSC up without assuming it hangs from
the home body's terrain sphere — among the `PQSCity` whose `sphere` is the home body's, say — and stop
there if it is not found, as the first version of `FixFlags` did with its `?.` operators.

## Seeing it

In KSP 1.12.5 with the Making History expansion, Harmony, ModuleManager, KSP Community Fixes 1.41.1 and
Kopernicus 248 with what it requires (ModularFlightIntegrator, KSPTextureLoader), and no planet pack.

The mission [`KSC flag fix`](../../../diag/kopernicus-flag-fix/Missions/): copy the `Missions` folder of
`diag/kopernicus-flag-fix/` into the folder of KSP. It starts with a pod on the runway; 30 seconds
later, it spawns a second pod on the launchpad; 30 seconds after that, it ends.

1. From the main menu, *Missions*, then play *KSC flag fix*. Switch the clock to universal time: a craft
   waiting on the runway keeps its mission elapsed time at zero.

   ![The mission started: the pod on the runway, the clock in universal time](../../../imgs/kopernicus-flags-fix/00-start-mission.png)

2. 30 seconds in, the second pod appears on the launchpad.

   ![The second pod on the launchpad, 1.6 km away](../../../imgs/kopernicus-flags-fix/10-wait-for-capsule2.png)

3. 30 seconds later, the mission ends: close its window.

   ![The end of the mission, and the button that closes its window](../../../imgs/kopernicus-flags-fix/30-end-of-mission.png)

4. Switch to the second pod, and look at the flag by the launchpad.

   ![The second pod on the launchpad, the flag by it](../../../imgs/kopernicus-flags-fix/40-switch-and-observe.png)

5. Quit KSP, and read `KSP.log`.

Three runs, each in a KSP started afresh:

| run | `settings.cfg` of this mod | in `KSP.log`, when the second pod appears | log |
|---|---|---|---|
| without this mod | — | nothing | [`kopernicus-flag-fix-without-this-mod.log`](../../../diag/runs/kopernicus-flag-fix-without-this-mod.log) |
| this mod, Kopernicus unpatched | `patchKopernicus = false`, `logLevel = Debug` | `Exception handling event OnKSCFacilityUpgraded in class RuntimeUtility: System.NullReferenceException` … `at Kopernicus.RuntimeUtility.RuntimeUtility.FixFlags ()` | [`kopernicus-flag-fix-patch-off.log`](../../../diag/runs/kopernicus-flag-fix-patch-off.log) |
| this mod, Kopernicus patched | `patchKopernicus = true`, `logLevel = Debug` | nothing | [`kopernicus-flag-fix-patch-on.log`](../../../diag/runs/kopernicus-flag-fix-patch-on.log) |

In both runs with this mod, the log shows the KSC taken out of its sphere a few seconds into the
flight, then, at the instant the second pod appears, put back under it and taken out again: the game
placed it again before spawning the pod, and the KSC was out of its sphere when the flag fix ran —
the case the patch covers. With the patch turned off, the log also warns, at startup, that Kopernicus'
flag fix will throw. In that run, where the flag fix throws, the flag by the launchpad does not move:
on a Kerbin of stock size, its glitch does not show anyway.

Under Real Solar System too, with the same Kopernicus, the log says the patch is applied.
