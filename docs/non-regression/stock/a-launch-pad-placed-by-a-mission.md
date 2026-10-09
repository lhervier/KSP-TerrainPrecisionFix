# A launch pad placed by a mission

Part of [Terrain Precision Fix](../../../README.md), one test of [Non-regression tests](../../non-regression.md), on [stock](../../non-regression.md#stock).

**Status: impossible to reproduce — we found no way to launch a craft from such a launch pad on stock.**

*Why.* A mission of Making History can place a launch pad of its own, anywhere on a body: a copy of the
mobile launch pad of the desert sites, built while the game runs rather than with the planets. Like
them, it is a static this mod takes out of its sphere in flight. Unlike them, stock code calls one more
method on it, and only there: the mission places the pad, waits one frame, then calls
`PQSCity2.SetBody`, which finds the body by looking up the hierarchy from the pad
(`LaunchSiteSituation.createLaunchSiteObject`: `yield return null; pqsCity2.SetBody();`), and sets the pad's `localPosition` in the frame of the sphere.
Taken out of its sphere in that frame, the pad would find no body, and be sent elsewhere. This mod
puts a static back under its sphere for the time of that call, as it does for every stock method that
reads its parents.

Read in the code, that frame never comes in flight: a mission places its launch pads while its game is
being set up, from the main menu (`MissionSystem.SetupMissionGame`, through `GenerateMissionLaunchSites`), and a launch pad node only works linked to
the start node of the mission (`ActionCreateLaunchSite.cs`); this mod takes statics out in flight only.

*The test it would take.* A mission whose start node places a launch pad (a *Create Launch Site* node)
and opens the VAB; played from the main menu (*Play Missions*) with this mod, a craft launched from that
pad. The craft should stand on the pad, the feet of the pad on the ground, and `KSP.log` should not say
that the pad *is not parented to a valid CelestialBody*.

*Why it could not be played.* KSP places the pad and adds it to its launch sites, but never finds it by
its name. `PSystemSetup.GetLaunchSite` passes over a launch site whose `BundleName` is neither empty,
`stock` nor the one of Making History, and the launch site a mission
builds leaves it unset (`LaunchSiteSituation.cs`, `createLaunchSiteObject`). So:

- the pad is missing from the launch site selector of the VAB;
- a mission that spawns a craft on it does not start: `KSP.log` says *Unable to find LaunchSite for
  Vessel*;
- a launch asked for by the name of the pad opens the flight scene, and KSP goes back to the space
  centre as soon as the craft is loaded.

All three were seen in KSP 1.12.5 with Making History; the first and the last also without this mod or
KSP Community Fixes. To
reproduce the first: in the Mission Builder, link a *Create Launch Site* node to the start node, open the
VAB in the settings of the mission, then play it and open the launch site selector of the VAB. We see
no other way to put a craft on that pad. If you find one, an issue or a pull request is welcome;
[the mission we used](../../../diag/non-regression/stock/a-launch-pad-placed-by-a-mission/Missions) places the pad on Kerbin, at latitude
3.1597°, longitude −141.1279°, and opens the VAB.

*What we did see.* With this mod and a craft brought next to the pad in flight, `KSP.log` shows the pad
taken out of its sphere only then, never while the mission is set up, and no line saying that it *is
not parented to a valid CelestialBody*. That, and the code read above, is why we trust this mod does not
break such a launch pad.
