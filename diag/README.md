# The runs

Part of [Terrain Precision Fix](../README.md): the `KSP.log` of every session taken with this mod
installed. What their readings say is in
[Checking the culprit: loading the same save](../docs/checking-the-culprit-loading.md),
[Coming back to a craft left parked](../docs/checking-the-culprit-approach.md),
[Switching to a craft far away](../docs/checking-the-culprit-switching.md),
[Driving on while the world moves](../docs/checking-the-culprit-driving.md),
[Launching from a launch pad of Making History](../docs/checking-the-culprit-launch-pad.md),
[In flight](../docs/checking-the-culprit-flight.md),
[Rescaled systems: Real Solar System](../docs/limits-and-solutions/rescaled-systems-real-solar-system.md)
and its pages, and [Deferred](../docs/limits-and-solutions/deferred.md).

The saves of the protocols are not here: each protocol belongs to an instrument, and its saves are in
the `diag` folder of [KSP Diag - Landed Vessel](https://github.com/lhervier/KSP-Diag-LandedVessel/tree/main/diag)
and of [KSP Diag - Terrain Height](https://github.com/lhervier/KSP-Diag-TerrainHeight/tree/main/diag),
along with the logs of the same sessions without this mod. Only the sessions taken with this mod alone
and no instrument have their saves here: the loads on Venus, Mars and Mercury, described in
[Real Solar System: what this mod corrected](../docs/limits-and-solutions/rss/what-this-mod-corrected.md#the-saves),
the launch from Cape Canaveral, and the rover of
[A CommNet ground station from a planet pack](../docs/non-regression/a-commnet-ground-station-from-a-planet-pack.md);
the pod beside a runway of Kerbal Konstructs, `non-reg-runway-mune-kk.sfs`, and the runway in
`non-reg-runway-mune-kk/`, of [Kerbal Konstructs](../docs/limits-and-solutions/kerbal-konstructs.md#the-patch-of-the-group-editor);
in `kopernicus-flag-fix/`, the mission and the change to Kopernicus of
[Kopernicus: the flag fix](../docs/limits-and-solutions/kopernicus/the-flag-fix.md); and, in
`rss-runway-fix/`, the change to Real Solar System of
[Real Solar System: the runway fix](../docs/limits-and-solutions/rss/the-runway-fix.md).

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

## The seam between subdivision levels

[KSP Diag - Terrain Quads](https://github.com/lhervier/KSP-Diag-TerrainQuads) and KSP-MCPServer added to
this mod at `logLevel = Debug`, the craft `Diag3-Rocket` on the launchpad reverted to launch by the
script of its protocol, `run-revert.py`, until the finer quad is above at the largest gap, on land.
Read in [The seam with this mod](../docs/non-regression/the-seam-between-subdivision-levels.md#the-seam-with-this-mod).

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
  [Non-regression tests: the statics](../docs/non-regression-statics.md#a-static-turning-with-its-body).

## On the stock system

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1, this mod and one instrument.

- [`runs/statics-kerbin-fix.log`](runs/statics-kerbin-fix.log) — no instrument, this mod with its
  statics fix, at `logLevel = Debug`: `runway-kerbin.sfs` loaded six times, the craft sent to a 200 km
  orbit with `Alt+F12 → Cheats → Set Orbit`, then the space centre. At every step, the `PQSCity` of
  Kerbin and where they hang, and the destructible buildings and upgradeable facilities of the KSC, are
  listed. Read in [Non-regression tests: the statics](../docs/non-regression-statics.md).
- [`runs/runway-mun-kk-colliders-fix.log`](runs/runway-mun-kk-colliders-fix.log) — four loadings of
  that save, every collider under each craft listed at each loading, with its height above the terrain
  KSP computes there.

## With Deferred

The install of [On the stock system](#on-the-stock-system) plus [Deferred](https://github.com/LGhassen/Deferred)
1.3.5 and [Shabby](https://github.com/KSPModdingLibs/Shabby) 0.4.2, both instruments at once, this mod at
`logLevel = Debug`, and [KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer), which plays the
protocols through the scripts of [`automation/`](automation/). Read in
[Deferred](../docs/limits-and-solutions/deferred.md).

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
[Kerbal Konstructs: the patch of the group editor](../docs/limits-and-solutions/kerbal-konstructs.md#the-patch-of-the-group-editor).

- [`runs/kk-group-editor-fix.log`](runs/kk-group-editor-fix.log) — `non-reg-runway-mune-kk.sfs`, its
  runway in `GameData/KerbalKonstructs/NewInstances`: the group moved with the gizmo of the group editor,
  saved with *Save&Close*, then the save loaded again.

## With Kopernicus

KSP 1.12.5 with the Making History expansion, Harmony, ModuleManager, KSP Community Fixes 1.41.1 and
[Real Solar System](https://github.com/KSP-RO/RealSolarSystem) 20.1.3.0 with what it requires (Kopernicus
248, Modular Flight Integrator, KSPTextureLoader, the RSS textures), no instrument. Each
session plays the mission [`kopernicus-flag-fix/Missions/KSC flag fix`](kopernicus-flag-fix/Missions/)
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
- [`runs/statics-earth-rss-fix.log`](runs/statics-earth-rss-fix.log) — no instrument, this mod with its
  statics fix: two loads of `reload-earth-rss-landed.sfs`, the craft sent to a 200 km orbit with
  `Alt+F12 → Cheats → Set Orbit`, then the space centre. At every step, the `PQSCity` of Earth and where
  they hang, and the destructible buildings and upgradeable facilities of the KSC, are listed. Read in
  [Non-regression tests: the statics](../docs/non-regression-statics.md)
  and [Kopernicus: the KSC moved by Real Solar System](../docs/limits-and-solutions/kopernicus/the-ksc-moved-by-real-solar-system.md).
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
  to load. Used in [The seam between subdivision levels](../docs/non-regression/the-seam-between-subdivision-levels.md).
- [`runs/runway-earth-rss-without-runway-fix.log`](runs/runway-earth-rss-without-runway-fix.log) —
  without this mod,
  [KSP Diag - Colliders](https://github.com/lhervier/KSP-Diag-Colliders) added, and
  Real Solar System built from the sources of its release 20.1.3.0 with the change
  [`rss-runway-fix/rss-20.1.3-without-its-runway-fix.diff`](rss-runway-fix/rss-20.1.3-without-its-runway-fix.diff),
  which keeps its runway fix from doing anything: one session, the rover of KSP Diag - Terrain Height
  launched from the SPH onto the runway at Cape Canaveral several times, and reloaded many times
  between, 23 entries in flight. Read in
  [Seeing it](../docs/limits-and-solutions/rss/the-runway-fix.md#seeing-it).
- [`runs/runway-earth-rss-without-runway-fix-fix.log`](runs/runway-earth-rss-without-runway-fix-fix.log) —
  the same install, with this mod: one session, the protocol of
  [Seeing it](../docs/limits-and-solutions/rss/the-runway-fix.md#seeing-it), 37 entries in flight.
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
  [A CommNet ground station from a planet pack](../docs/non-regression/a-commnet-ground-station-from-a-planet-pack.md).
