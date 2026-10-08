# The runs

Part of [Terrain Precision Fix](../README.md): the `KSP.log` of every session taken with this mod
installed. What their readings say is in
[Checking the culprit: loading the same save](../docs/checking-the-culprit-loading.md),
[Coming back to a craft left parked](../docs/checking-the-culprit-approach.md),
[Switching to a craft far away](../docs/checking-the-culprit-switching.md),
[Driving on while the world moves](../docs/checking-the-culprit-driving.md),
[Launching from a launch pad of Making History](../docs/checking-the-culprit-launch-pad.md),
[In flight](../docs/checking-the-culprit-flight.md),
[Anchoring a base](../docs/checking-the-culprit-anchoring.md),
the pages of [Non-regression tests](../docs/non-regression.md) and of
[Limits and solutions](../docs/limits-and-solutions.md).

The saves of the protocols are not here: each protocol belongs to an instrument, and its saves are in
the `diag` folder of [KSP Diag - Landed Vessel](https://github.com/lhervier/KSP-Diag-LandedVessel/tree/main/diag)
and of [KSP Diag - Terrain Height](https://github.com/lhervier/KSP-Diag-TerrainHeight/tree/main/diag),
along with the logs of the same sessions without this mod. Only the sessions taken with this mod alone
and no instrument have their saves here, with the anchor of
[Checking the culprit: anchoring a base](../docs/checking-the-culprit-anchoring.md), a protocol of no
instrument, `ground-anchor-kerbin.sfs` and its rover, `craft/Diag3-Rover.craft`; the loads on Venus, Mars and Mercury, described in
[Real Solar System: this mod's safeguard](../docs/non-regression/real-solar-system/this-mods-safeguard.md#the-saves),
the launch from Cape Canaveral, the capsule on a launch pad of Kerbal Konstructs on the Mun,
`warp-static-mun-kk.sfs`, and the launch pad in `warp-static-mun-kk/`, of
[Time warp](../docs/non-regression/stock/time-warp.md#in-flight-low-over-a-static), and the rover of
[Real Solar System: a CommNet ground station](../docs/non-regression/real-solar-system/a-commnet-ground-station.md);
the pod beside a runway of Kerbal Konstructs, `non-reg-runway-mune-kk.sfs`, and the runway in
`non-reg-runway-mune-kk/`, of [Kerbal Konstructs: the group editor](../docs/limits-and-solutions/kerbal-konstructs/the-group-editor.md);
in `kopernicus-flag-fix/`, the mission and the change that turns off the flag fix of Kopernicus, of
[Kopernicus: the flag fix](../docs/limits-and-solutions/kopernicus/the-flag-fix.md); and, in
`rss-runway-fix/`, the change to Real Solar System of
[Real Solar System: the runway fix](../docs/non-regression/real-solar-system/the-runway-fix.md).

## The loading protocol

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1, this mod, both instruments and
[KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer): the four saves of
[the loading protocol](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/docs/the-protocol-loading.md),
a capsule on a small tank on Kerbin, the Mun, Minmus and Gilly, each loaded six times by its script, `run-loading.py`. On Real Solar System, the same, plus Real Solar System 20.1.3.0
and what it requires (Kopernicus 248, Modular Flight Integrator, KSPTextureLoader, the RSS textures):
`reload-moon-rss-resave.sfs` and `reload-earth-rss-resave.sfs`. Read in
[Checking the culprit: loading the same save](../docs/checking-the-culprit-loading.md).

- [`runs/loading-fix.log`](runs/loading-fix.log) — the `KSP.log` of the session the four saves were
  played in, one after the other, together with a lone capsule on the same spots, a series no longer
  published.
- [`runs/loading-rss-fix.log`](runs/loading-rss-fix.log) — the same on Real Solar System, the Moon then
  Earth.
- `runs/reload-<save>-fix-script.txt` — what the script printed for each save, and
  `runs/reload-<save>-fix-lines.json`, every line it recorded, in both instruments: for instance
  [`runs/reload-kerbin-2parts-fix-lines.json`](runs/reload-kerbin-2parts-fix-lines.json) or
  [`runs/reload-earth-rss-resave-fix-lines.json`](runs/reload-earth-rss-resave-fix-lines.json).

## The switching protocol

The install of [The loading protocol](#the-loading-protocol), on Kerbin: the six rounds of
[the switching protocol](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/docs/the-protocol-switching.md),
played by its script, `run-switching.py`, on `switch-kerbin.sfs`. Read in
[Switching to a craft far away](../docs/checking-the-culprit-switching.md).

- [`runs/switching-fix.log`](runs/switching-fix.log) — the `KSP.log` of the session; what the script
  printed in [`runs/switching-fix-script.txt`](runs/switching-fix-script.txt), and every line it
  recorded, in both instruments, in [`runs/switching-fix-lines.json`](runs/switching-fix-lines.json).

## The approach protocol

The install of [The loading protocol](#the-loading-protocol), on Kerbin: the six round trips of
[the approach protocol](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/docs/the-protocol-approach.md),
in a single flight, played by its script, `run-approach.py`, on `approach-kerbin.sfs`. Read in
[Coming back to a craft left parked](../docs/checking-the-culprit-approach.md).

- [`runs/approach-fix.log`](runs/approach-fix.log) — the `KSP.log` of the session; what the script
  printed in [`runs/approach-fix-script.txt`](runs/approach-fix-script.txt), and every line it
  recorded, in both instruments, in [`runs/approach-fix-lines.json`](runs/approach-fix-lines.json).

## The runway protocol

The install of [The loading protocol](#the-loading-protocol): the six loadings of
[the runway protocol](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/docs/the-protocol-runway.md),
played by its script, `run-runway.py`. Read in
[Checking the culprit: loading the same save](../docs/checking-the-culprit-loading.md).

- [`runs/runway-fix.log`](runs/runway-fix.log) — on Kerbin, `runway-kerbin.sfs`; what the script
  printed in [`runs/runway-fix-script.txt`](runs/runway-fix-script.txt), and every line it recorded, in
  both instruments, in [`runs/runway-fix-lines.json`](runs/runway-fix-lines.json).
- [`runs/runway-mun-kk-fix.log`](runs/runway-mun-kk-fix.log) — on the Mun, `runway-mun-kk.sfs`, beside a
  runway placed by Kerbal Konstructs 1.12.3, added to the install with CustomPreLaunchChecks 1.8.1; what
  the script printed in [`runs/runway-mun-kk-fix-script.txt`](runs/runway-mun-kk-fix-script.txt), and
  every line it recorded in [`runs/runway-mun-kk-fix-lines.json`](runs/runway-mun-kk-fix-lines.json).

## The driving protocol

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1, this mod at `logLevel = Debug`,
KSP Diag - Terrain Height, KSP Diag - Floating Origin and KSP-MCPServer: one run of three shifts of
[the driving protocol](https://github.com/lhervier/KSP-Diag-TerrainHeight/blob/main/docs/the-protocol-driving.md),
played by its script, `run-driving.py`; on Earth, Real Solar System 20.1.3.0 and what it requires as
well. Each log holds one line per quad this mod placed, with how far it was moved. Read in
[Driving on while the world moves](../docs/checking-the-culprit-driving.md).

- [`runs/driving-diag2-fix.log`](runs/driving-diag2-fix.log) — on Kerbin, `driving-kerbin.sfs`; what the
  script printed in [`runs/driving-diag2-fix-script.txt`](runs/driving-diag2-fix-script.txt), and every
  line it recorded in [`runs/driving-diag2-fix-lines.json`](runs/driving-diag2-fix-lines.json).
- [`runs/driving-earth-rss-diag2-fix.log`](runs/driving-earth-rss-diag2-fix.log) — on Earth,
  `driving-earth-rss.sfs`; what the script printed in
  [`runs/driving-earth-rss-diag2-fix-script.txt`](runs/driving-earth-rss-diag2-fix-script.txt), and
  every line it recorded in [`runs/driving-earth-rss-diag2-fix-lines.json`](runs/driving-earth-rss-diag2-fix-lines.json).

## The protocol of the runway and the grass while the world moves

The install of [The driving protocol](#the-driving-protocol): three moves of the origin of
[the protocol of the runway and the grass while the world moves](https://github.com/lhervier/KSP-Diag-TerrainHeight/blob/main/docs/the-protocol-driving-runway.md),
played by its script, `run-driving-runway.py`, from `driving-runway-kerbin.sfs`. Read in
[Driving on while the world moves](../docs/checking-the-culprit-driving.md#on-kerbin). Earth is under
[On Real Solar System](#on-real-solar-system).

- [`runs/driving-runway-diag2-fix.log`](runs/driving-runway-diag2-fix.log) — this mod as it is
  installed; what the script printed in
  [`runs/driving-runway-diag2-fix-script.txt`](runs/driving-runway-diag2-fix-script.txt), and every
  line it recorded in [`runs/driving-runway-diag2-fix-lines.json`](runs/driving-runway-diag2-fix-lines.json).
- [`runs/driving-runway-diag2-fix-terrain-only.log`](runs/driving-runway-diag2-fix-terrain-only.log) —
  an earlier run, with an earlier version of the script, this mod with `fixStatics = false`; what the
  script printed in
  [`runs/driving-runway-diag2-fix-terrain-only-script.txt`](runs/driving-runway-diag2-fix-terrain-only-script.txt),
  and every line it recorded in
  [`runs/driving-runway-diag2-fix-terrain-only-lines.json`](runs/driving-runway-diag2-fix-terrain-only-lines.json).

## The launch pad protocol

KSP 1.12.5 with the Making History expansion, Harmony, ModuleManager, KSP Community Fixes 1.41.1, this
mod, [KSP Diag - Terrain Height](https://github.com/lhervier/KSP-Diag-TerrainHeight) and KSP-MCPServer:
six launches of `Capsule.craft` from the Desert Launch Site, one session of the game each, by the script
of [the launch pad protocol](https://github.com/lhervier/KSP-Diag-TerrainHeight/blob/main/docs/the-protocol-launch-pad.md),
`run-launch-pad.py`. Read in
[Launching from a launch pad of Making History](../docs/checking-the-culprit-launch-pad.md).

- [`runs/launch-pad-fix-1.log`](runs/launch-pad-fix-1.log) to
  [`runs/launch-pad-fix-6.log`](runs/launch-pad-fix-6.log) — the `KSP.log` of each session; what the
  script printed in [`runs/launch-pad-fix-script.txt`](runs/launch-pad-fix-script.txt), and every line
  it recorded in [`runs/launch-pad-fix-lines.json`](runs/launch-pad-fix-lines.json).

## The protocol of the quads of the highest level, in flight

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1, this mod at `logLevel = Debug`,
KSP Diag - Terrain Quads, KSP Diag - Floating Origin and KSP-MCPServer: on each body, one flight of
[the protocol of the quads of the highest level, in flight](https://github.com/lhervier/KSP-Diag-TerrainQuads/blob/main/docs/the-protocol-flight.md),
`Quad-Rocket` launched from the launchpad, played by its script, `run-flight.py`, and read by its
`analyse-flight.py`. Read in [In flight](../docs/checking-the-culprit-flight.md).

- [`runs/flight-kerbin-fix.log`](runs/flight-kerbin-fix.log) — on Kerbin, from the launchpad of the
  Space Center, 73 *Logs*; what the script printed in [`runs/flight-kerbin-fix-script.txt`](runs/flight-kerbin-fix-script.txt), what each *Log*
  answered in [`runs/flight-kerbin-fix-readings.json`](runs/flight-kerbin-fix-readings.json), the two
  files of the *Logs* in [`runs/flight-kerbin-fix-logs.csv`](runs/flight-kerbin-fix-logs.csv) and
  [`runs/flight-kerbin-fix-quads.zip`](runs/flight-kerbin-fix-quads.zip) (zipped: 43 MB once unzipped),
  and what `analyse-flight.py` printed in
  [`runs/flight-kerbin-fix-analysis.txt`](runs/flight-kerbin-fix-analysis.txt).
- [`runs/flight-earth-rss-fix.log`](runs/flight-earth-rss-fix.log) — on Earth, Real Solar System
  20.1.3.0 as released and what it requires added, from the launchpad of Cape Canaveral, 56 *Logs*; what
  the script printed in [`runs/flight-earth-rss-fix-script.txt`](runs/flight-earth-rss-fix-script.txt),
  what each *Log* answered in [`runs/flight-earth-rss-fix-readings.json`](runs/flight-earth-rss-fix-readings.json),
  the two files of the *Logs* in [`runs/flight-earth-rss-fix-logs.csv`](runs/flight-earth-rss-fix-logs.csv)
  and [`runs/flight-earth-rss-fix-quads.zip`](runs/flight-earth-rss-fix-quads.zip) (zipped: 41 MB once
  unzipped), and what `analyse-flight.py` printed in
  [`runs/flight-earth-rss-fix-analysis.txt`](runs/flight-earth-rss-fix-analysis.txt).

## The ground anchor protocol

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1,
[KSP Diag - Landed Vessel](https://github.com/lhervier/KSP-Diag-LandedVessel),
[KSP Diag - Terrain Height](https://github.com/lhervier/KSP-Diag-TerrainHeight), and this mod at
`logLevel = Debug`, or not: [`ground-anchor-kerbin.sfs`](ground-anchor-kerbin.sfs), Bill Kerman places a
ground anchor on the desert of Kerbin beside the rover of [`craft/Diag3-Rover.craft`](craft/Diag3-Rover.craft),
alone, then with one of the rover's batteries attached on top of it; quicksave and quickload twice,
played by hand. Three sessions: without this mod, with this mod and `fixGroundAnchorLoad = false`, and with
this mod as installed. Read in
[Checking the culprit: anchoring a base](../docs/checking-the-culprit-anchoring.md).

- [`runs/ground-anchor-kerbin-without-this-mod.log`](runs/ground-anchor-kerbin-without-this-mod.log), which
  holds a first placement, given up, before the two measured;
- [`runs/ground-anchor-kerbin-model-fix.log`](runs/ground-anchor-kerbin-model-fix.log), which holds a first
  series of both cases before the measured one, the anchor with a battery read without it set as target;
- [`runs/ground-anchor-kerbin-fix.log`](runs/ground-anchor-kerbin-fix.log).

## The seam between subdivision levels

[KSP Diag - Terrain Quads](https://github.com/lhervier/KSP-Diag-TerrainQuads) and KSP-MCPServer added to
this mod at `logLevel = Debug`, the craft `Diag3-Rocket` on the launchpad reverted to launch by the
script of its protocol, `run-revert.py`, until the finer quad is above at the largest gap, on land.
Read in [The seam with this mod](../docs/limits-and-solutions/stock/the-seam-between-subdivision-levels.md#the-seam-with-this-mod).

- [`runs/revert-earth-rss-fix-diag4.log`](runs/revert-earth-rss-fix-diag4.log) — on Earth, Real Solar
  System 20.1.3.0 as released and what it requires added, at Cape Canaveral, nine loads; what the
  script printed in [`runs/revert-earth-rss-fix-diag4-script.txt`](runs/revert-earth-rss-fix-diag4-script.txt),
  and every reading in [`runs/revert-earth-rss-fix-diag4-readings.json`](runs/revert-earth-rss-fix-diag4-readings.json).
- [`runs/revert-kerbin-fix-diag4.log`](runs/revert-kerbin-fix-diag4.log) — on Kerbin, at the Space
  Center, ten loads; what the script printed in
  [`runs/revert-kerbin-fix-diag4-script.txt`](runs/revert-kerbin-fix-diag4-script.txt), and every
  reading in [`runs/revert-kerbin-fix-diag4-readings.json`](runs/revert-kerbin-fix-diag4-readings.json).

## The altitude where KSP turns the body

Without this mod: KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1,
[KSP Diag - Floating Origin](https://github.com/lhervier/KSP-Diag-FloatingOrigin) and
[KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer); on the stock system, then with Real Solar
System 20.1.3.0 and what it requires, then with Outer Planets Mod 2.2.12, Kopernicus 248, Modular Flight
Integrator and KSPTextureLoader as for Real Solar System, and the Community Terrain Texture Pack 1.0.5.

- [`runs/rotation-thresholds.txt`](runs/rotation-thresholds.txt) — what
  [`automation/run-rotation-threshold.py`](automation/run-rotation-threshold.py) printed, body by body:
  a craft moved from orbit to orbit, the altitude above which KSP turns the body rather than the world.
  No log kept: the readings are the script's. Read in
  [A static turning with its body](../docs/non-regression/stock/a-static-turning-with-its-body.md).

## The scene changes protocol

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1,
[KSP Diag - Colliders](https://github.com/lhervier/KSP-Diag-Colliders), this mod at `logLevel = Debug`,
and KSP-MCPServer, which plays the test through [`automation/run-scene-changes.py`](automation/run-scene-changes.py):
a new career at the Custom difficulty (on Kerbin, the same as [`career-with-the-parts.sfs`](career-with-the-parts.sfs)),
`Diag3-Rocket.craft` on the launchpad, eleven arrivals in flight through every way of leaving
it, and `Diag3-Rover.craft` launched from the SPH onto the runway; one script per way in
[`automation/scene-changes/`](automation/scene-changes/). On Earth, Real Solar System 20.1.3.0 and what it
requires (Kopernicus 248, Modular Flight Integrator,
KSPTextureLoader, the RSS textures). Each one played again without this mod. Read in
[Scene changes](../docs/non-regression/stock/scene-changes.md).

- [`runs/scene-changes-kerbin-fix.log`](runs/scene-changes-kerbin-fix.log) and
  [`runs/scene-changes-kerbin-without-this-mod.log`](runs/scene-changes-kerbin-without-this-mod.log) — on
  Kerbin; what the script printed in
  [`runs/scene-changes-kerbin-fix-script.txt`](runs/scene-changes-kerbin-fix-script.txt) and
  [`runs/scene-changes-kerbin-without-this-mod-script.txt`](runs/scene-changes-kerbin-without-this-mod-script.txt),
  the colliders under the craft at each arrival in
  [`runs/scene-changes-kerbin-fix-readings.json`](runs/scene-changes-kerbin-fix-readings.json) and
  [`runs/scene-changes-kerbin-without-this-mod-readings.json`](runs/scene-changes-kerbin-without-this-mod-readings.json).
- [`runs/scene-changes-earth-rss-fix.log`](runs/scene-changes-earth-rss-fix.log) and
  [`runs/scene-changes-earth-rss-without-this-mod.log`](runs/scene-changes-earth-rss-without-this-mod.log)
  — on Earth; the same in
  [`runs/scene-changes-earth-rss-fix-script.txt`](runs/scene-changes-earth-rss-fix-script.txt),
  [`runs/scene-changes-earth-rss-without-this-mod-script.txt`](runs/scene-changes-earth-rss-without-this-mod-script.txt),
  [`runs/scene-changes-earth-rss-fix-readings.json`](runs/scene-changes-earth-rss-fix-readings.json) and
  [`runs/scene-changes-earth-rss-without-this-mod-readings.json`](runs/scene-changes-earth-rss-without-this-mod-readings.json).
- [`runs/scene-changes-earth-rss-without-this-mod-earlier.log`](runs/scene-changes-earth-rss-without-this-mod-earlier.log)
  — an earlier session on Earth without this mod, kept for the error of the Knowledge Base it shows.

## The destroyed buildings protocol

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1,
[KSP Diag - Colliders](https://github.com/lhervier/KSP-Diag-Colliders), this mod at `logLevel = Debug`,
and KSP-MCPServer, which plays the test through
[`automation/run-destroyed-buildings.py`](automation/run-destroyed-buildings.py): a new career at the Custom
difficulty (the same as [`career-with-the-parts.sfs`](career-with-the-parts.sfs), which a player can start
from), [`craft/VAB-Dropper.craft`](craft/VAB-Dropper.craft) dropped twice onto the VAB from the
launchpad, `Diag3-Rover.craft` launched from the SPH onto the runway around the VAB in ruins, the VAB
repaired, and `Diag3-Rocket.craft` launched from it; three arrivals on the launchpad, two on the runway.
Played again without this mod. Read in
[Destroyed buildings](../docs/non-regression/stock/destroyed-buildings.md).

- [`runs/destroyed-buildings-kerbin-fix.log`](runs/destroyed-buildings-kerbin-fix.log) and
  [`runs/destroyed-buildings-kerbin-without-this-mod.log`](runs/destroyed-buildings-kerbin-without-this-mod.log);
  what the script printed in
  [`runs/destroyed-buildings-kerbin-fix-script.txt`](runs/destroyed-buildings-kerbin-fix-script.txt) and
  [`runs/destroyed-buildings-kerbin-without-this-mod-script.txt`](runs/destroyed-buildings-kerbin-without-this-mod-script.txt),
  the colliders under the craft at each arrival in
  [`runs/destroyed-buildings-kerbin-fix-readings.json`](runs/destroyed-buildings-kerbin-fix-readings.json) and
  [`runs/destroyed-buildings-kerbin-without-this-mod-readings.json`](runs/destroyed-buildings-kerbin-without-this-mod-readings.json).

## The facility levels protocol

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1,
[KSP Diag - Colliders](https://github.com/lhervier/KSP-Diag-Colliders), this mod at `logLevel = Debug`,
and KSP-MCPServer, which plays the test through
[`automation/run-facility-levels.py`](automation/run-facility-levels.py): a new career at the Custom
difficulty (the same as [`career-with-the-parts.sfs`](career-with-the-parts.sfs)), `Diag3-Rocket.craft` launched onto the launchpad and `Diag3-Rover.craft` onto the runway, four
arrivals on each at each of their three levels. Played again without this mod. Read in
[Facility levels](../docs/non-regression/stock/facility-levels.md).

- [`runs/facility-levels-kerbin-fix.log`](runs/facility-levels-kerbin-fix.log) and
  [`runs/facility-levels-kerbin-without-this-mod.log`](runs/facility-levels-kerbin-without-this-mod.log);
  what the script printed in
  [`runs/facility-levels-kerbin-fix-script.txt`](runs/facility-levels-kerbin-fix-script.txt) and
  [`runs/facility-levels-kerbin-without-this-mod-script.txt`](runs/facility-levels-kerbin-without-this-mod-script.txt),
  the colliders under the craft at each arrival in
  [`runs/facility-levels-kerbin-fix-readings.json`](runs/facility-levels-kerbin-fix-readings.json) and
  [`runs/facility-levels-kerbin-without-this-mod-readings.json`](runs/facility-levels-kerbin-without-this-mod-readings.json).

## The time warp protocol

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1,
[KSP Diag - Colliders](https://github.com/lhervier/KSP-Diag-Colliders), this mod at `logLevel = Debug`,
and KSP-MCPServer, which plays the test through
[`automation/run-time-warp.py`](automation/run-time-warp.py): a new sandbox game, `Diag3-Rover.craft`
launched from the SPH onto the runway, three days of time warp at 100 000×, then *Warp To* the night and
the next noon at the KSC; four readings, a screenshot at each. Played again without this mod. Read in
[Time warp](../docs/non-regression/stock/time-warp.md) and
[The lights of the KSC](../docs/non-regression/stock/the-lights-of-the-ksc.md).

- [`runs/time-warp-kerbin-fix.log`](runs/time-warp-kerbin-fix.log) and
  [`runs/time-warp-kerbin-without-this-mod.log`](runs/time-warp-kerbin-without-this-mod.log);
  what the script printed in
  [`runs/time-warp-kerbin-fix-script.txt`](runs/time-warp-kerbin-fix-script.txt) and
  [`runs/time-warp-kerbin-without-this-mod-script.txt`](runs/time-warp-kerbin-without-this-mod-script.txt),
  the colliders under the craft at each reading in
  [`runs/time-warp-kerbin-fix-readings.json`](runs/time-warp-kerbin-fix-readings.json) and
  [`runs/time-warp-kerbin-without-this-mod-readings.json`](runs/time-warp-kerbin-without-this-mod-readings.json).

## The time warp flyover protocol

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1, Kerbal Konstructs 1.12.3 with the launch
pad of [`warp-static-mun-kk/`](warp-static-mun-kk/) and its CustomPreLaunchChecks,
[KSP Diag - Colliders](https://github.com/lhervier/KSP-Diag-Colliders), this mod at `logLevel = Debug`,
and KSP-MCPServer, which plays the test through
[`automation/run-time-warp-flyover.py`](automation/run-time-warp-flyover.py): the capsule of
[`warp-static-mun-kk.sfs`](warp-static-mun-kk.sfs) on that launch pad, near the highest point of the Mun's
equator, read before and after `Diag3-Rover.craft`, put on a low orbit of the Mun, passes over it in time
warp at 10×. Played again without this mod. Read in
[Time warp](../docs/non-regression/stock/time-warp.md#in-flight-low-over-a-static).

- [`runs/time-warp-flyover-mun-kk-fix.log`](runs/time-warp-flyover-mun-kk-fix.log) and
  [`runs/time-warp-flyover-mun-kk-without-this-mod.log`](runs/time-warp-flyover-mun-kk-without-this-mod.log);
  the session without this mod is also the one the launch pad was placed in, with Kerbal Konstructs' editor,
  and holds a first try of the script that launched from the launchpad, where KSP recovered the capsule;
  what the script printed in
  [`runs/time-warp-flyover-mun-kk-fix-script.txt`](runs/time-warp-flyover-mun-kk-fix-script.txt) and
  [`runs/time-warp-flyover-mun-kk-without-this-mod-script.txt`](runs/time-warp-flyover-mun-kk-without-this-mod-script.txt),
  the colliders under the capsule at each reading in
  [`runs/time-warp-flyover-mun-kk-fix-readings.json`](runs/time-warp-flyover-mun-kk-fix-readings.json) and
  [`runs/time-warp-flyover-mun-kk-without-this-mod-readings.json`](runs/time-warp-flyover-mun-kk-without-this-mod-readings.json).

## On the stock system

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1, this mod and one instrument.

- [`runs/runway-mun-kk-colliders-fix.log`](runs/runway-mun-kk-colliders-fix.log) — four loadings of
  `runway-mun-kk.sfs`, every collider under each craft listed at each loading, with its height above the terrain
  KSP computes there.

## With Deferred

The install of [On the stock system](#on-the-stock-system) plus [Deferred](https://github.com/LGhassen/Deferred)
1.3.5 and [Shabby](https://github.com/KSPModdingLibs/Shabby) 0.4.2, both instruments at once, this mod at
`logLevel = Debug`, and [KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer), which plays the
protocols through the scripts of [`automation/`](automation). Read in
[Deferred](../docs/non-regression/deferred/drawing-the-ground.md).

- [`automation/run-runway.py`](automation/run-runway.py) — plays
  [the runway protocol](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/docs/the-protocol-runway.md).
- [`automation/run-approach.py`](automation/run-approach.py) — plays
  [the approach protocol](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/docs/the-protocol-approach.md).
- [`runs/runway-deferred-fix.log`](runs/runway-deferred-fix.log) — the six loadings of `runway-kerbin.sfs`;
  the lines read are in [`runs/runway-deferred-fix-lines.json`](runs/runway-deferred-fix-lines.json).
- [`runs/runway-without-deferred-fix.log`](runs/runway-without-deferred-fix.log) — the same script on the
  same install, Deferred and Shabby taken out; the lines read are in
  [`runs/runway-without-deferred-fix-lines.json`](runs/runway-without-deferred-fix-lines.json).
- [`runs/approach-deferred-fix.log`](runs/approach-deferred-fix.log) — the six round trips of
  `approach-kerbin.sfs`, in a single flight; the lines read are in
  [`runs/approach-deferred-fix-lines.json`](runs/approach-deferred-fix-lines.json).

## With Kerbal Konstructs

KSP 1.12.5 with the Making History expansion, Harmony, ModuleManager, KSP Community Fixes 1.41.1,
[Kerbal Konstructs](https://github.com/KSP-RO/Kerbal-Konstructs) 1.12.3 and CustomPreLaunchChecks 1.8.1,
which it requires, no instrument, this mod at `logLevel = Debug`. Read in
[Kerbal Konstructs: the group editor](../docs/limits-and-solutions/kerbal-konstructs/the-group-editor.md).

- [`runs/kk-group-editor-fix.log`](runs/kk-group-editor-fix.log) — `non-reg-runway-mune-kk.sfs`, its
  runway in `GameData/KerbalKonstructs/NewInstances`: the group moved with the gizmo of the group editor,
  saved with *Save&Close*, then the save loaded again.

## With Kopernicus

KSP 1.12.5 with the Making History expansion, Harmony, ModuleManager, KSP Community Fixes 1.41.1 and
[Real Solar System](https://github.com/KSP-RO/RealSolarSystem) 20.1.3.0 with what it requires (Kopernicus
248, Modular Flight Integrator, KSPTextureLoader, the RSS textures), no instrument. Each
session plays the mission [`kopernicus-flag-fix/Missions/KSC flag fix`](kopernicus-flag-fix/Missions)
once, as described in [Seeing the patch](../docs/limits-and-solutions/kopernicus/the-flag-fix.md#seeing-the-patch).

- [`runs/kopernicus-flag-fix-without-this-mod.log`](runs/kopernicus-flag-fix-without-this-mod.log) —
  without this mod.
- [`runs/kopernicus-flag-fix-patch-off.log`](runs/kopernicus-flag-fix-patch-off.log) — this mod at
  `logLevel = Debug`, with `patchKopernicus = false`.
- [`runs/kopernicus-flag-fix-patch-on.log`](runs/kopernicus-flag-fix-patch-on.log) — this mod at
  `logLevel = Debug`, with its defaults.

## On Real Solar System

The same install plus [Real Solar System](https://github.com/KSP-RO/RealSolarSystem) 20.1.3.0 and what
it requires (Kopernicus, Modular Flight Integrator, KSPTextureLoader, the RSS textures), as released
unless said otherwise. This mod ran at `logLevel = Debug`, so each log also holds one line per quad
placed, with how far it was moved. The sessions marked *first safeguard* ran with its first version,
a fixed metre, and hold the line where it refused part of Earth's terrain.

- [`runs/reload-moon-rss-fix.log`](runs/reload-moon-rss-fix.log) — Diag LandedVessel, Real Solar System's
  component turned off by an empty assembly named `WorldStabilizer`: one load of
  `reload-moon-rss.sfs`, the save made without this mod, then that save taken again as
  `reload-moon-rss-resave.sfs` and loaded six times. *First safeguard.*
- [`runs/reload-moon-rss-fix-vgpe-on.log`](runs/reload-moon-rss-fix-vgpe-on.log) — Diag LandedVessel, Real Solar
  System as released: the loads of `reload-moon-rss-resave.sfs`, six of them recorded. *First
  safeguard.*
- [`runs/reload-moon-rss-fix-diag2.log`](runs/reload-moon-rss-fix-diag2.log) — Diag TerrainHeight: six loads of
  `reload-moon-rss-resave.sfs`.
- [`runs/reload-moon-rss-fix-24loads.log`](runs/reload-moon-rss-fix-24loads.log) — no instrument:
  24 loads of `reload-moon-rss-resave.sfs` in a row. *First safeguard.*
- [`runs/reload-earth-rss-fix.log`](runs/reload-earth-rss-fix.log) — Diag LandedVessel: six loads of
  `reload-earth-rss-resave.sfs`, the craft in *prelaunch*.
- [`runs/reload-earth-rss-fix-diag2.log`](runs/reload-earth-rss-fix-diag2.log) — Diag TerrainHeight: the same six
  loads.
- [`runs/reload-earth-rss-fix-chain.log`](runs/reload-earth-rss-fix-chain.log) — no instrument, one
  session: 27 loads of `reload-earth-rss-landed.sfs`, the craft *landed*, then, after going back to the
  space centre, 24 of `reload-earth-rss-resave.sfs`, the craft in *prelaunch*.
- [`runs/kopernicus-flag-glitch-without-this-mod.log`](runs/kopernicus-flag-glitch-without-this-mod.log),
  [`runs/kopernicus-flag-glitch-statics-fix.log`](runs/kopernicus-flag-glitch-statics-fix.log) and
  [`runs/kopernicus-flag-glitch-statics-fix-off.log`](runs/kopernicus-flag-glitch-statics-fix-off.log) —
  MechJeb2 2.15.3 added to the first and the third, and Kopernicus built from the sources of its release
  248 with the change
  [`kopernicus-flag-fix/kopernicus-248-without-its-flag-fix.diff`](kopernicus-flag-fix/kopernicus-248-without-its-flag-fix.diff),
  which turns its flag fix off: a craft on the launchpad at Cape Canaveral, without this mod, then with
  it, then with its statics fix turned off. Read in
  [The flag glitch](../docs/limits-and-solutions/kopernicus/the-flag-fix.md#the-flag-glitch).
- [`runs/reload-venus-mars-rss-fix.log`](runs/reload-venus-mars-rss-fix.log) — no instrument, one
  session: six loads of [`reload-venus-rss.sfs`](reload-venus-rss.sfs), then six of
  [`reload-mars-rss.sfs`](reload-mars-rss.sfs). The six loads after them, on a second site of Mars,
  are not used.
- [`runs/reload-mercury-rss-fix.log`](runs/reload-mercury-rss-fix.log) — no instrument: six loads of
  [`reload-mercury-rss.sfs`](reload-mercury-rss.sfs). Each one logs KSP moving the craft down 22.4 m,
  the terrain detail its save describes.
- [`runs/launch-earth-rss-fix.log`](runs/launch-earth-rss-fix.log) — no instrument, MechJeb2 2.15.0.0
  added to the install: one session, a small rocket launched from the VAB onto the launchpad at Cape
  Canaveral, then flown towards orbit, the flight started over three times. The launch was saved
  afterwards as [`rss-launch-to-earth-orbit.sfs`](rss-launch-to-earth-orbit.sfs), which needs MechJeb2
  to load. Used in [The seam between subdivision levels](../docs/limits-and-solutions/stock/the-seam-between-subdivision-levels.md).
- [`runs/runway-earth-rss-without-runway-fix.log`](runs/runway-earth-rss-without-runway-fix.log) —
  without this mod,
  [KSP Diag - Colliders](https://github.com/lhervier/KSP-Diag-Colliders) added, and
  Real Solar System built from the sources of its release 20.1.3.0 with the change
  [`rss-runway-fix/rss-20.1.3-without-its-runway-fix.diff`](rss-runway-fix/rss-20.1.3-without-its-runway-fix.diff),
  which keeps its runway fix from doing anything: one session, the rover of KSP Diag - Terrain Height
  launched from the SPH onto the runway at Cape Canaveral several times, and reloaded many times
  between, 23 entries in flight. Read in
  [Seeing it](../docs/non-regression/real-solar-system/the-runway-fix.md#seeing-it).
- [`runs/runway-earth-rss-without-runway-fix-fix.log`](runs/runway-earth-rss-without-runway-fix-fix.log) —
  the same install, with this mod: one session, the protocol of
  [Seeing it](../docs/non-regression/real-solar-system/the-runway-fix.md#seeing-it), 37 entries in flight.
- [`runs/driving-runway-earth-rss-diag2-fix.log`](runs/driving-runway-earth-rss-diag2-fix.log) —
  [KSP Diag - Terrain Height](https://github.com/lhervier/KSP-Diag-TerrainHeight), Diag FloatingOrigin and
  [KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer) added, Real Solar System built without its
  runway fix as above: the protocol of the runway and the grass while the world moves, played by its
  script from `driving-runway-earth-rss.sfs`, two moves of the origin; what the script printed in
  [`runs/driving-runway-earth-rss-diag2-fix-script.txt`](runs/driving-runway-earth-rss-diag2-fix-script.txt),
  and every line it recorded in
  [`runs/driving-runway-earth-rss-diag2-fix-lines.json`](runs/driving-runway-earth-rss-diag2-fix-lines.json).
  Read in [Driving on while the world moves](../docs/checking-the-culprit-driving.md#on-earth).
- [`runs/station-kourou-rss-fix.log`](runs/station-kourou-rss-fix.log) —
  [KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer) added: one session, a rover driven by its `drive_to` tool
  in 500 m legs toward the CommNet ground station of Kourou, from 28.86 km to 27.30 km, then
  [`station-kourou-rss.sfs`](station-kourou-rss.sfs) loaded and driven from 27.83 km to 27.30 km by
  [`automation/run-station-approach.py`](automation/run-station-approach.py). Read in
  [A CommNet ground station from a planet pack](../docs/non-regression/real-solar-system/a-commnet-ground-station.md).

## The scatter fix

The readings of [KSP Diag - Scatter](https://github.com/lhervier/KSP-Diag-Scatter) with
[the scatter fix](../docs/the-fix-scatter.md)
on, read in [Checking the culprit: loading the same save](../docs/checking-the-culprit-loading.md#the-scatter-over-twelve-loads),
[Checking the culprit: in flight](../docs/checking-the-culprit-flight.md#the-rocks-along-a-flight),
[The scatter holders](../docs/non-regression/stock/the-scatter-holders.md) and
[Kopernicus: scatter with colliders](../docs/non-regression/kopernicus/scatter-with-colliders.md),
kept as they were logged, copied out of `KSP.log`, in [`runs/scatter-fix/`](runs/scatter-fix): for the
rocks after a load, one file per load, each holding the last record taken after that load; for the rocks
over a flight and for the holder pools, one file per flight or session, holding every record taken
during it. The scatter fix was then a mod of its own, Rock Precision Fix 0.1.0, installed next to this
mod: the same two patches, whose lines in these logs are tagged `[RockPrecisionFix]`.

The procedure, the saves and the format of a record belong to the instrument:
[its protocol](https://github.com/lhervier/KSP-Diag-Scatter/blob/main/docs/measuring-the-rocks.md#load-after-load),
[the saves](https://github.com/lhervier/KSP-Diag-Scatter/blob/main/diag/README.md#the-saves) and
[the log](https://github.com/lhervier/KSP-Diag-Scatter/blob/main/docs/measuring-the-rocks.md#the-log).

### The scatter fix: the rocks, over twelve loads

KSP 1.12.5 on Windows. `GameData` holding Harmony, ModuleManager, KSP Community Fixes 1.41.1,
this mod, 0.1.0, KSP Diag - Scatter and Rock Precision Fix 0.1.0. Both fixes at `logLevel = Info`. Terrain scatter on, the log flushed at once. KSP was
started once, and in that session `reference-kerbin.sfs` then `reference-mune.sfs` were each loaded twelve
times, with `Alt+F6` pressed after each load once the scene had settled.

Both fixes wrote to `KSP.log` that they were installed, and that they had acted on each body before its
first record:

```
[RockPrecisionFix] Version 0.1.0.0 installed, log level Info
[TerrainPrecisionFix] Version 0.1.0.0 installed, log level Info
[TerrainPrecisionFix] Kerbin: terrain placed in double precision (first quad corrected by 16.50 mm)
[RockPrecisionFix] Kerbin: scatter drawn from its terrain quads (first holder was -27.51 mm off)
[TerrainPrecisionFix] Mun: terrain placed in double precision (first quad corrected by 9.18 mm)
[RockPrecisionFix] Mun: scatter drawn from its terrain quads (first holder was +8.51 mm off)
```

| load | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11 | 12 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Kerbin | [`1`](runs/scatter-fix/kerbin-both-load1.log) | [`2`](runs/scatter-fix/kerbin-both-load2.log) | [`3`](runs/scatter-fix/kerbin-both-load3.log) | [`4`](runs/scatter-fix/kerbin-both-load4.log) | [`5`](runs/scatter-fix/kerbin-both-load5.log) | [`6`](runs/scatter-fix/kerbin-both-load6.log) | [`7`](runs/scatter-fix/kerbin-both-load7.log) | [`8`](runs/scatter-fix/kerbin-both-load8.log) | [`9`](runs/scatter-fix/kerbin-both-load9.log) | [`10`](runs/scatter-fix/kerbin-both-load10.log) | [`11`](runs/scatter-fix/kerbin-both-load11.log) | [`12`](runs/scatter-fix/kerbin-both-load12.log) |
| the Mun | [`1`](runs/scatter-fix/mun-both-load1.log) | [`2`](runs/scatter-fix/mun-both-load2.log) | [`3`](runs/scatter-fix/mun-both-load3.log) | [`4`](runs/scatter-fix/mun-both-load4.log) | [`5`](runs/scatter-fix/mun-both-load5.log) | [`6`](runs/scatter-fix/mun-both-load6.log) | [`7`](runs/scatter-fix/mun-both-load7.log) | [`8`](runs/scatter-fix/mun-both-load8.log) | [`9`](runs/scatter-fix/mun-both-load9.log) | [`10`](runs/scatter-fix/mun-both-load10.log) | [`11`](runs/scatter-fix/mun-both-load11.log) | [`12`](runs/scatter-fix/mun-both-load12.log) |

Every record of Kerbin ends on the same line, but for its number:

```
End of record 1: 64 quads with rocks, 118 holders (0 not built yet); nearest quad 'Kerbin Zn3010000130': 218 rocks, 1780 vertices measured, 0 without ground under them
```

and every record of the Mun on this one:

```
End of record 13: 128 quads with rocks, 128 holders (0 not built yet); nearest quad 'Mun Zp211333000': 20 rocks, 200 vertices measured, 0 without ground under them
```

So every record was taken once all the holders were built, and all the records of a body name the same
nearest quad: its objects and their measured vertices compare one by one from one load to the next.

### The scatter fix: the rocks, over a flight

Where the rocks are drawn once the terrain has kept building and destroying quads under a moving craft,
with the world origin following it.

The same install as above, with the build of KSP Diag - Scatter of
[the holder pools](#the-scatter-fix-the-holder-pools-over-a-flight) below. `ref-mune-5km.sfs` loaded once, then `Alt+F6`
pressed 30 s into the flight, again every two minutes, and once more after the pod crashed, following
[its protocol](https://github.com/lhervier/KSP-Diag-Scatter/blob/main/docs/measuring-the-rocks.md#over-a-flight).

Both fixes wrote to `KSP.log` that they had acted on the Mun before the first record:

```
[TerrainPrecisionFix] Mun: terrain placed in double precision (first quad corrected by 17.46 mm)
[RockPrecisionFix] Mun: scatter drawn from its terrain quads (first holder was -15.15 mm off)
```

| flight | records |
|---|---|
| with both fixes | [`mun-5km-both-rocks.log`](runs/scatter-fix/mun-5km-both-rocks.log) |

The file holds the six records and the line of `KSP.log` reporting the crash, between the fifth and the
sixth. Every record has all its holders built, and 200 vertices measured on its nearest quad, all of them
with ground under them:

| record | quads with rocks | holders | nearest quad |
|---|---|---|---|
| 1 | 344 | 344 | `Mun Zp200000011` |
| 2 | 168 | 168 | `Mun Xn231111111` |
| 3 | 144 | 144 | `Mun Xn211311311` |
| 4 | 224 | 224 | `Mun Xn122020000` |
| 5 | 152 | 152 | `Mun Xn013331113` |
| 6, after the crash | 568 | 568 | `Mun Zn200000011` |

Those are, record for record, the quads of the same flight with this mod without the scatter fix, and the same
nearest quads: their vertices compare one by one from one flight to the other.

### The scatter fix: the holder pools, over a flight

The scatter fix takes each holder out of the pool's container while its quad is in use, and hangs it back there
when the quad is handed back. This series checks that every holder does go back, and that none is lost on
the way.

The same install as above, with a later build of KSP Diag - Scatter: the first one
with the holder record. `ref-mune-5km.sfs` loaded once, then `Alt+Shift+F6` pressed 30 s into the flight,
again about every two minutes, and once more after the pod crashed, following
[its protocol](https://github.com/lhervier/KSP-Diag-Scatter/blob/main/docs/checking-the-holder-pools.md#over-a-flight).

Both fixes wrote to `KSP.log` that they had acted on the Mun before the first record:

```
[TerrainPrecisionFix] Mun: terrain placed in double precision (first quad corrected by 14.82 mm)
[RockPrecisionFix] Mun: scatter drawn from its terrain quads (first holder was -1.34 mm off)
```

| flight | records |
|---|---|
| with both fixes | [`mun-5km-both-holders.log`](runs/scatter-fix/mun-5km-both-holders.log) |

The file holds the six records and the line of `KSP.log` reporting the crash, between the fifth and the
sixth. Every record ends on `0 broken rules`:

| record | holders in use | free | broken rules |
|---|---|---|---|
| 1 | 344 | 40 | 0 |
| 2 | 168 | 216 | 0 |
| 3 | 144 | 240 | 0 |
| 4 | 224 | 160 | 0 |
| 5 | 152 | 232 | 0 |
| 6, after the crash | 568 | 40 | 0 |

Those counts are, record for record, those of the flight with this mod without the scatter fix: the flight
takes as many holders out of the pool, at the same moments, with the scatter fix as without it.

### The scatter fix: the holder pools, across scene switches

The scatter fix hangs a holder from its quad, and the quads of the most detailed level are shared by every body.
Leaving a body switches its terrain off: this series checks that its holders go back to their pools
before stock destroys them, and that none travels on a quad to the next body.

The same install and build as the flight above. One session, following
[its protocol](https://github.com/lhervier/KSP-Diag-Scatter/blob/main/docs/checking-the-holder-pools.md#across-scene-switches):
`reference-mune.sfs` loaded from the Space Center, back to the Space Center, then `reference-kerbin.sfs`,
with `Alt+Shift+F6` pressed in each of the three scenes.

Both fixes wrote to `KSP.log` that they had acted on each body before its first record:

```
[TerrainPrecisionFix] Kerbin: terrain placed in double precision (first quad corrected by 16.50 mm)
[RockPrecisionFix] Kerbin: scatter drawn from its terrain quads (first holder was -27.51 mm off)
[TerrainPrecisionFix] Mun: terrain placed in double precision (first quad corrected by 1.68 mm)
[RockPrecisionFix] Mun: scatter drawn from its terrain quads (first holder was -0.40 mm off)
```

| session | records |
|---|---|
| with both fixes | [`scenes-both-holders.log`](runs/scatter-fix/scenes-both-holders.log) |

The file holds the three records, the line of `KSP.log` marking the arrival in each scene, and those of
both fixes. Every record ends on `0 in no pool` and `0 broken rules`:

| record | scene | pools | holders in use | free |
|---|---|---|---|---|
| 1 | the Mun | the Mun's `Rock00` | 128 | 32 |
| 2 | the Space Center | Kerbin's `Tree00`, `Grass00`, `boulder`, `Pine00`, `cactus` | 4 | 316 |
| 3 | Kerbin | the same five | 118 | 202 |

Those are, pool for pool, the counts of the same session with this mod without the scatter fix. The Mun's pool
is gone from the second record on, and no holder of the Mun turned up under a quad of Kerbin.

### The scatter fix: the colliders

A different install, because stock scatter has no collider: the one above, plus
[Kopernicus](https://github.com/Kopernicus/Kopernicus) 1.12.1.247 and the
[Stock Scatter Collider Enabler Patch](https://github.com/Poodmund/Stock-Scatter-Collider-Enabler-Patch)
1.0.1, which gives every stock object of scatter a collision mesh of its own shape.

One series, following
[its protocol](https://github.com/lhervier/KSP-Diag-Scatter/blob/main/docs/measuring-the-rocks.md#with-colliders-on-the-scatter):
`ref-kerbin-scatter-collider-eva.sfs`, a kerbal standing on a boulder in a desert of Kerbin, loaded six
times in one session, each load with a record and a picture of the kerbal's feet.

| load | 1 | 2 | 3 | 4 | 5 | 6 |
|---|---|---|---|---|---|---|
| with both fixes | [`1`](runs/scatter-fix/collider-both-load1.log) | [`2`](runs/scatter-fix/collider-both-load2.log) | [`3`](runs/scatter-fix/collider-both-load3.log) | [`4`](runs/scatter-fix/collider-both-load4.log) | [`5`](runs/scatter-fix/collider-both-load5.log) | [`6`](runs/scatter-fix/collider-both-load6.log) |
| its pictures | [`1`](../imgs/scatter-fix/collider-both-load1.png) | [`2`](../imgs/scatter-fix/collider-both-load2.png) | [`3`](../imgs/scatter-fix/collider-both-load3.png) | [`4`](../imgs/scatter-fix/collider-both-load4.png) | [`5`](../imgs/scatter-fix/collider-both-load5.png) | [`6`](../imgs/scatter-fix/collider-both-load6.png) |

Every record ends on the same line, but for its number:

```
End of record 1: 173 quads with rocks, 306 holders (0 not built yet); nearest quad 'Kerbin Xn0131000203': 6 rocks, 60 vertices measured, 0 without ground under them, 6 colliders measured
```

The same nearest quad throughout, carrying the same six objects with a collider — one `boulder` and five
`cactus` — so they compare one by one from one load to the next, and with the two series taken without the scatter fix.

### The scatter fix: the other configurations

Stock and this mod without the scatter fix, on the same saves, are kept with
[KSP Diag - Scatter](https://github.com/lhervier/KSP-Diag-Scatter/blob/main/diag/README.md),
the colliders included.
