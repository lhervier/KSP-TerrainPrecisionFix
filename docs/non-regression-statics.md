# Non-regression tests: the statics

Part of [Terrain Precision Fix](../README.md), one half of its non-regression tests, with
[the ground](non-regression-ground.md).

In flight, while a craft is near it, this mod takes a static out of its terrain sphere and places it in
double: [The fix: the statics](the-fix-statics.md). What it fixes there is measured in
[Checking the culprit](../README.md#checking-the-culprit). This page checks the other side: what works
in stock around the KSC still works with this mod.

Unless it says otherwise, each test is played in KSP 1.12.5 with Harmony, ModuleManager, KSP Community
Fixes 1.41.1 and this mod, with `logLevel = Debug` in its settings so that `KSP.log` shows each static it
takes out of its sphere and puts back; and, when something looks wrong, a second time without this mod,
to tell what stock already does.

| Test | Why | Result |
|---|---|---|
| Load a save with a craft 1.4 km from the KSC, several times, then send the craft to a 200 km orbit (`Alt+F12 → Cheats → Set Orbit`), then go back to the space centre. On Kerbin, [`runway-kerbin.sfs`](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/diag/runway-kerbin.sfs), six loadings; on Earth, in [Real Solar System](limits-and-solutions/rescaled-systems-real-solar-system.md), [`reload-earth-rss-landed.sfs`](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/diag/reload-earth-rss-landed.sfs), two. | This mod takes the KSC out of its sphere when a craft comes near it, and has to put it back before the code that expects it there runs. KSP finds the buildings of the KSC by their place in the hierarchy below it. | **Checked.** At every loading, the KSC is taken out of its sphere, and nothing else is: on Kerbin, the Island Airfield, 33 km away, stays under its sphere. The KSC is put back when the save is loaded again and when the craft reaches orbit, and the space centre opens normally. Its 39 destructible buildings and 9 upgradeable facilities stay registered under their usual names. The logs show no error this mod causes. Logs: [Kerbin](../diag/runs/statics-kerbin-fix.log), [Earth](../diag/runs/statics-earth-rss-fix.log). |
| From the VAB, launch a craft on the launchpad; from the SPH, a craft on the runway. | KSP places a new craft on a spawn point that hangs from the KSC. | **Checked** on Kerbin: each stands on the launchpad and on the runway. |
| With a craft on the launchpad, in turn: revert to launch, quicksave and quickload (F5, F9), go to the space centre and back through the tracking station, revert to the VAB, recover the craft. | This mod has to put the KSC back under its sphere before every scene change. | To test. In flight, the KSC should be where it was; at the space centre, every building should open. |
| With a craft on the launchpad and a second one parked on the runway, send the first around the Mun (`Alt+F12 → Cheats → Set Orbit`), then switch to the second from the map view. | Away from Kerbin, the KSC goes back under its sphere; the switch takes it out again. | To test. The KSC should be there, at its full detail, and the second craft should stand on the runway. |
| With a craft on the runway, time warp at the highest rate on rails for a few days of game time, then stop. | A static out of its sphere has to follow its body as it turns. | To test. The craft and the KSC should be where they were. |
| In flight, crash a craft into a destructible building of the KSC, such as the water tower of the launchpad; repair it at the space centre. | KSP finds a destructible building by its place in the hierarchy below the KSC, which this mod changes in flight. | To test. It should show as destroyed at the space centre, and be back at its place in the next flight once repaired. |
| In a career game, launch from the launchpad and from the runway at each of their three levels, upgraded from the space centre. | Upgradeable facilities are found the same way as destructible buildings. | To test. Each facility should show the buildings of its level, at their place. |
| CommNet on, a probe on the launchpad; then the same in a game that starts in flight, from a stock scenario. | The ground station of the KSC finds its body by looking up the hierarchy from where it hangs. | To test. The signal indicator should show a connection to the KSC. |
| With the Making History expansion and no other mod, play the mission [`KSC flag fix`](../diag/kopernicus-flag-fix/Missions/): 30 seconds in, it spawns a pod on the launchpad. | A mission spawns a craft on a spawn point of the KSC, as a launch does. | To test. The pod should stand on its spawn point, the launchpad at its level. Seen once, with Kopernicus and Real Solar System, in [Seeing the patch](limits-and-solutions/kopernicus/the-flag-fix.md#seeing-the-patch): the pod appears on the launchpad; whether it stands on its spawn point was not checked. |
| A craft landed by each of the other stock statics — the KSC 2, the Island Airfield, the pyramids, the anomalies — loaded twice, then flown away and back. | They are placed by `PQSCity` like the KSC, and this mod takes them out of their sphere the same way. | To test. The static should be where it was, and the craft should stand as it did. |
| A craft climbing from the Mun's surface to 25 km above an anomaly of the Mun. | Low over a body, KSP turns the world around the craft; above an altitude set for each body, it turns the body, and this mod has to turn the statics with it. | To test. From up there, the anomaly should not slide over the ground. |

`PQSCity2`, which places the launch sites of the Making History expansion, carries the same defect and
is not covered.

The cost per frame of the statics fix is no test a player can play by hand: it belongs to
[Performance](performance.md), where it is still to measure.
