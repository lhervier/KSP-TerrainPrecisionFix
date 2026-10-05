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

- [Loading, an orbit and the space centre](#loading-an-orbit-and-the-space-centre) — checked.
- [Launching from the VAB and the SPH](#launching-from-the-vab-and-the-sph) — checked on Kerbin.
- [The foot of the launchpad](#the-foot-of-the-launchpad) — to test.
- [Scene changes](#scene-changes) — to test.
- [A trip to another body](#a-trip-to-another-body) — to test.
- [Time warp](#time-warp) — to test.
- [Destroyed buildings](#destroyed-buildings) — to test.
- [Facility levels](#facility-levels) — to test.
- [The ground station](#the-ground-station) — to test.
- [A mission spawning a craft](#a-mission-spawning-a-craft) — to test.
- [A launch pad placed by a mission](#a-launch-pad-placed-by-a-mission) — not tested: no way found on stock.
- [The other stock statics](#the-other-stock-statics) — to test.
- [A static turning with its body](#a-static-turning-with-its-body) — cannot happen on stock, in Real
  Solar System or with Outer Planets Mod.
- [What this page does not cover](#what-this-page-does-not-cover).

## Loading, an orbit and the space centre

**Checked.**

*Why.* This mod takes the KSC out of its sphere when a craft comes near it, and has to put it back
before the code that expects it there runs. KSP finds the buildings of the KSC by their place in the
hierarchy below it.

*The test.* Load a save with a craft 1.4 km from the KSC, several times, then send the craft to a
200 km orbit (`Alt+F12 → Cheats → Set Orbit`), then go back to the space centre. On Kerbin,
[`runway-kerbin.sfs`](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/diag/runway-kerbin.sfs),
six loadings; on Earth, in [Real Solar System](limits-and-solutions/rescaled-systems-real-solar-system.md),
[`reload-earth-rss-landed.sfs`](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/diag/reload-earth-rss-landed.sfs),
two.

*The result.* At every loading, the KSC is taken out of its sphere, and nothing else is: on Kerbin, the
Island Airfield, 33 km away, stays under its sphere. The KSC is put back when the save is loaded again
and when the craft reaches orbit, and the space centre opens normally. Its 39 destructible buildings and
9 upgradeable facilities stay registered under their usual names. The logs show no error this mod
causes. Logs: [Kerbin](../diag/runs/statics-kerbin-fix.log), [Earth](../diag/runs/statics-earth-rss-fix.log).

## Launching from the VAB and the SPH

**Checked on Kerbin.**

*Why.* KSP places a new craft on a spawn point that hangs from the KSC.

*The test.* From the VAB, launch a craft on the launchpad; from the SPH, a craft on the runway.

*The result.* Each stands on the launchpad and on the runway.

## The foot of the launchpad

**To test.**

*Why.* This mod places the ground and the launchpad each in double, so the edge between the two changes.
The edge of the runway is measured in [Checking the culprit](checking-the-culprit-loading.md).

*The test.* Look at the foot of the launchpad, where it meets the grass, over several loadings, with
this mod and without it.

*What should happen.* No grass should come through the edge, and no step should show at its foot that
stock does not show.

## Scene changes

**To test.**

*Why.* This mod has to put the KSC back under its sphere before every scene change.

*The test.* With a craft on the launchpad, in turn: revert to launch, quicksave and quickload (F5, F9),
go to the space centre and back through the tracking station, revert to the VAB, recover the craft.

*What should happen.* In flight, the KSC should be where it was; at the space centre, every building
should open.

## A trip to another body

**To test.**

*Why.* Away from Kerbin, the KSC goes back under its sphere; switching back to a craft near it takes it
out again.

*The test.* With a craft on the launchpad and a second one parked on the runway, send the first around
the Mun (`Alt+F12 → Cheats → Set Orbit`), then switch to the second from the map view.

*What should happen.* The KSC should be there, at its full detail, and the second craft should stand on
the runway.

## Time warp

**To test.**

*Why.* A static out of its sphere has to follow its body as it turns.

*The test.* With a craft on the runway, time warp at the highest rate on rails for a few days of game
time, then stop.

*What should happen.* The craft and the KSC should be where they were.

## Destroyed buildings

**To test.**

*Why.* KSP finds a destructible building by its place in the hierarchy below the KSC, which this mod
changes in flight.

*The test.* In flight, crash a craft into a destructible building of the KSC, such as the water tower of
the launchpad; repair it at the space centre.

*What should happen.* It should show as destroyed at the space centre, and be back at its place in the
next flight once repaired.

## Facility levels

**To test.**

*Why.* Upgradeable facilities are found the same way as destructible buildings.

*The test.* In a career game, launch from the launchpad and from the runway at each of their three
levels, upgraded from the space centre.

*What should happen.* Each facility should show the buildings of its level, at their place.

## The ground station

**To test.**

*Why.* The ground station of the KSC finds its body by looking up the hierarchy from where it hangs.

*The test.* CommNet on, a probe on the launchpad; then the same in a game that starts in flight, from a
stock scenario.

*What should happen.* The signal indicator should show a connection to the KSC.

## A mission spawning a craft

**To test.**

*Why.* A mission spawns a craft on a spawn point of the KSC, as a launch does.

*The test.* With the Making History expansion and no other mod, play the mission
[`KSC flag fix`](../diag/kopernicus-flag-fix/Missions/): 30 seconds in, it spawns a pod on the launchpad.

*What should happen.* The pod should stand on its spawn point, the launchpad at its level. Seen once,
with Kopernicus and Real Solar System, in
[Seeing the patch](limits-and-solutions/kopernicus/the-flag-fix.md#seeing-the-patch): the pod appears
on the launchpad; whether it stands on its spawn point was not checked.

## A launch pad placed by a mission

**Not tested: we found no way to launch a craft from such a launch pad on stock.**

*Why.* A mission of Making History can place a launch pad of its own, anywhere on a body: a copy of the
mobile launch pad of the desert sites, built while the game runs rather than with the planets. Like
them, it is a static this mod takes out of its sphere in flight. Unlike them, stock code calls one more
method on it, and only there: the mission places the pad, waits one frame, then calls
`PQSCity2.SetBody`, which finds the body by looking up the hierarchy from the pad
(`LaunchSiteSituation.cs:385-386`), and sets the pad's `localPosition` in the frame of the sphere.
Taken out of its sphere in that frame, the pad would find no body, and be sent elsewhere. This mod
puts a static back under its sphere for the time of that call, as it does for every stock method that
reads its parents.

Read in the code, that frame never comes in flight: a mission places its launch pads while its game is
being set up, from the main menu (`MissionSystem.cs:869`), and a launch pad node only works linked to
the start node of the mission (`ActionCreateLaunchSite.cs`); this mod takes statics out in flight only.

*The test it would take.* A mission whose start node places a launch pad (a *Create Launch Site* node)
and opens the VAB; played from the main menu (*Play Missions*) with this mod, a craft launched from that
pad. The craft should stand on the pad, the feet of the pad on the ground, and `KSP.log` should not say
that the pad *is not parented to a valid CelestialBody*.

*Why it could not be played.* KSP places the pad and adds it to its launch sites, but never finds it by
its name. `PSystemSetup.GetLaunchSite` passes over a launch site whose `BundleName` is neither empty,
`stock` nor the one of Making History (`PSystemSetup.cs:1800-1838`), and the launch site a mission
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
[the mission we used](../diag/mission-launch-pad/Missions/) places the pad on Kerbin, at latitude
3.1597°, longitude −141.1279°, and opens the VAB.

*What we did see.* With this mod and a craft brought next to the pad in flight, `KSP.log` shows the pad
taken out of its sphere only then, never while the mission is set up, and no line saying that it *is
not parented to a valid CelestialBody*. That, and the code read above, is why we trust this mod does not
break such a launch pad.

## The other stock statics

**To test.**

*Why.* The KSC 2, the Island Airfield, the pyramids and the anomalies are placed by `PQSCity` like the
KSC, and this mod takes them out of their sphere the same way.

*The test.* A craft landed by each of them, loaded twice, then flown away and back.

*What should happen.* The static should be where it was, and the craft should stand as it did.

## A static turning with its body

**Cannot happen on stock, in Real Solar System or with Outer Planets Mod.**

*Why.* Low over a body, KSP turns the world around the craft, and the body stays still in the game's
world (`OrbitPhysicsManager.cs:1181`). Above an altitude set for each body, KSP turns the body instead.
A static under its sphere turns with it on its own; a static this mod has taken out of its sphere does
not, and this mod has to turn it
([Keeping a static out of its sphere in place](the-fix-statics.md#keeping-a-static-out-of-its-sphere-in-place)).
If it failed to, the static would slide over the ground, by some 9 m every second at the Mun's equator.

*When it could be tested.* Two conditions at once: a static out of its sphere, so within 27.5 km of the
craft, where this mod takes it out, and no farther than 30.25 km, where it puts it back
([The fix: the statics](the-fix-statics.md)); and the craft above the altitude where KSP turns the
body. A test needs a body that KSP turns from less than 30 km up.

*The readings.* That altitude was read body by body, without this mod, with
[KSP Diag - Floating Origin](https://github.com/lhervier/KSP-Diag-FloatingOrigin), whose *Frame* column
says whether KSP turns the world (*Rotating*) or the body (*Inertial*): a craft is put in a circular
equatorial orbit with `Alt+F12 → Cheats → Set Orbit`, first above the body's relief and the top of its
atmosphere, then twice as high until the frame is inertial, never past the body's sphere of influence,
then halving the interval down to 500 m. The script
[`run-rotation-threshold.py`](../diag/automation/run-rotation-threshold.py) plays these steps through
[KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer); what it printed is in
[`rotation-thresholds.txt`](../diag/runs/rotation-thresholds.txt). Stock, then Real Solar System
20.1.3.0, then Outer Planets Mod 2.2.12:

- **no body turns lower than 100 km.** Every solid body without an atmosphere that was read turns from
  100 km; Earth in Real Solar System from 145 km;
- **a body with an atmosphere never turns below its top**: KSP does not let that altitude go lower
  (`CelestialBody.cs:464`). Venus, Mars, Titan, Triton and Pluto in Real Solar System were already
  inertial 2 km above it;
- **the smallest moons never turn while a craft is around them**: on Phobos and Deimos in Real Solar
  System, Hale and Ovok in Outer Planets Mod, the world still turns at the edge of their sphere of
  influence, past which the craft is around their planet.

Real Solar System loads asteroids from farther away than stock, craft by craft (its
`Harmony/Vessel.cs`); the ranges every other craft gets, which this mod reads, stay as they are.

<details>
<summary>The readings, body by body</summary>

| system | body | KSP turns the body from |
|---|---|---|
| stock | the Mun | between 99.92 and 100.00 km |
| stock | Minmus | between 99.80 and 100.20 km |
| stock | Gilly | between 99.84 and 100.00 km |
| stock | Moho, Ike, Dres, Tylo, Vall, Bop, Pol, Eeloo | above 30 km: still rotating there, one reading each |
| stock | Kerbin, Eve, Duna, Laythe | not read: at least the top of their atmosphere |
| Real Solar System | the Moon, Mercury, Vesta | between 99.84 and 100.31 km |
| Real Solar System | Ceres, Io, Europa, Ganymede, Callisto, Mimas, Enceladus, Tethys, Dione, Rhea, Iapetus, Miranda, Ariel, Umbriel, Titania, Oberon, Charon | between 99.69 and 100.00 km |
| Real Solar System | Earth | between 144.77 and 145.05 km |
| Real Solar System | Venus | between 145 km, the top of its atmosphere, and 147 km |
| Real Solar System | Mars | between 125 km, the top of its atmosphere, and 127 km |
| Real Solar System | Titan | between 600 km, the top of its atmosphere, and 602 km |
| Real Solar System | Triton, Pluto | between 110 km, the top of their atmosphere, and 112 km |
| Real Solar System | Phobos, Deimos | never: still rotating at 38 km, the edge of their sphere of influence is 39.5 to 39.8 km up |
| Outer Planets Mod | Slate, Eeloo, Wal, Nissee, Plock, Karen | between 99.69 and 100.00 km |
| Outer Planets Mod | Polta, Priax | between 99.61 and 100.00 km |
| Outer Planets Mod | Tekto | between 99.65 and 100.03 km, above its 95 km of atmosphere |
| Outer Planets Mod | Thatmo | between 99.73 and 100.02 km |
| Outer Planets Mod | Tal | between 99.69 and 100.16 km |
| Outer Planets Mod | Hale | never: still rotating at 33 km, the edge of its sphere of influence is 35 km up |
| Outer Planets Mod | Ovok | never: still rotating at 65 km, the edge of its sphere of influence is 68 km up |

The gas giants and the Sun, where nothing stands, were not read.

</details>

*Why that altitude is so high.* It has to be: KSP never turns the terrain quads of the highest level.
It keeps them out of the sphere, as this mod does with a static, and only ever moves them along
(`PQ.cs:273`), or places them again without touching how they are turned (`PQ.cs:279`). Were KSP to turn
the body while they are there, they would stay as they were while the rest of the body turned, and the
ground would tear apart. KSP keeps the world turning with the body as long as the craft is low enough
for them to exist. A body set to turn lower is therefore no way to test this mod: stock breaks first.
Kopernicus can set that altitude for a body; with it installed, this ModuleManager patch makes KSP turn
the Mun from 5 km up:

```
@Kopernicus:AFTER[Kopernicus]
{
	@Body[Mun]
	{
		@Properties
		{
			inverseRotThresholdAltitude = 5000
		}
	}
}
```

![The ground of the Mun torn apart between two quads](../imgs/non-regression/the-mun-turning-from-5-km.png)

*KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1, Kopernicus 248 and the patch above,
KSP Diag - Floating Origin at the top, without this mod: a craft moved from orbit to orbit of the Mun
with Set Orbit, 8, 5.2 and 5.05 km up for a few seconds each, where KSP turns the Mun, then 4.95 and
4.8 km up, where it turns the world again; the game paused at 4.8 km.*

## What this page does not cover

The cost per frame of the statics fix is no test a player can play by hand: it belongs to
[Performance](performance.md), where it is still to measure.
