# The runs

Part of [Terrain Precision Fix](../README.md): the `KSP.log` of every session taken with this mod
installed. What their readings say is in
[Checking the culprit: loading the same save](../docs/checking-the-culprit-loading.md),
[Coming back to a craft left parked](../docs/checking-the-culprit-approach.md),
[Switching to a craft far away](../docs/checking-the-culprit-switching.md),
[The runway and the grass beside it](../docs/checking-the-culprit-runway.md) and
[Rescaled systems: Real Solar System](../docs/limits-and-solutions/rescaled-systems-real-solar-system.md).

The saves of the protocols are not here: each protocol belongs to an instrument, and its saves are in
the `diag` folder of [Terrain Precision Fix Diag 1](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag/tree/main/diag)
and of [Terrain Precision Fix Diag 2](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag2/tree/master/diag),
along with the logs of the same sessions without this mod. Only the sessions taken with this mod alone
and no instrument have their saves here: the loads on Venus, Mars and Mercury, described in
[Rescaled systems: Real Solar System](../docs/limits-and-solutions/rescaled-systems-real-solar-system.md#the-saves),
and the launch from Cape Canaveral; and, in `kopernicus-flag-fix/`, the mission and the change to
Kopernicus of [Kopernicus: the flag fix](../docs/limits-and-solutions/kopernicus/the-flag-fix.md).

## On the stock system

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1, this mod and one instrument.

- [`runs/approach-diag1-fix.log`](runs/approach-diag1-fix.log) — the six round trips of
  [the approach protocol](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag/blob/main/docs/the-protocol-approach.md)
  on Kerbin, in a single flight, read by Diag 1.
- [`runs/approach-diag2-fix.log`](runs/approach-diag2-fix.log) — the same, read by Diag 2.
- [`runs/switching-diag1-fix.log`](runs/switching-diag1-fix.log) — the six rounds of
  [the switching protocol](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag/blob/main/docs/the-protocol-switching.md)
  on Kerbin, read by Diag 1.
- [`runs/switching-diag2-fix.log`](runs/switching-diag2-fix.log) — the same, read by Diag 2.
- [`runs/runway-diag1-fix.log`](runs/runway-diag1-fix.log) — the six loadings of
  [the runway protocol](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag/blob/main/docs/the-protocol-runway.md)
  on Kerbin, read by Diag 1.
- [`runs/runway-diag2-fix.log`](runs/runway-diag2-fix.log) — the same, read by Diag 2.
- [`runs/runway-mun-kk-diag1-fix.log`](runs/runway-mun-kk-diag1-fix.log) — the six loadings of the
  same protocol on the Mun, beside a runway placed by Kerbal Konstructs 1.12.3 (added to the install with
  CustomPreLaunchChecks 1.8.1), read by Diag 1.
- [`runs/runway-mun-kk-diag2-fix.log`](runs/runway-mun-kk-diag2-fix.log) — the same, read by Diag 2.
- [`runs/statics-kerbin-fix.log`](runs/statics-kerbin-fix.log) — no instrument, this mod with its
  statics fix, at `logLevel = Debug`: `runway-kerbin.sfs` loaded six times, the craft sent to a 200 km
  orbit with `Alt+F12 → Cheats → Set Orbit`, then the space centre. At every step, the `PQSCity` of
  Kerbin and where they hang, and the destructible buildings and upgradeable facilities of the KSC, are
  listed. Read in [The KSC buildings, runway and launchpad](../docs/limits-and-solutions/the-ksc-buildings-runway-and-launchpad.md).
- [`runs/runway-mun-kk-colliders-fix.log`](runs/runway-mun-kk-colliders-fix.log) — four loadings of
  that save, every collider under each craft listed at each loading, with its height above the terrain
  KSP computes there.

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

- [`runs/reload-moon-rss-fix.log`](runs/reload-moon-rss-fix.log) — Diag 1, Real Solar System's
  component turned off by an empty assembly named `WorldStabilizer`: one load of
  `reload-moon-rss.sfs`, the save made without this mod, then that save taken again as
  `reload-moon-rss-resave.sfs` and loaded six times. *First safeguard.*
- [`runs/reload-moon-rss-fix-vgpe-on.log`](runs/reload-moon-rss-fix-vgpe-on.log) — Diag 1, Real Solar
  System as released: the loads of `reload-moon-rss-resave.sfs`, six of them recorded. *First
  safeguard.*
- [`runs/reload-moon-rss-fix-diag2.log`](runs/reload-moon-rss-fix-diag2.log) — Diag 2: six loads of
  `reload-moon-rss-resave.sfs`.
- [`runs/reload-moon-rss-fix-24loads.log`](runs/reload-moon-rss-fix-24loads.log) — no instrument:
  24 loads of `reload-moon-rss-resave.sfs` in a row. *First safeguard.*
- [`runs/reload-earth-rss-fix.log`](runs/reload-earth-rss-fix.log) — Diag 1: six loads of
  `reload-earth-rss-resave.sfs`, the craft in *prelaunch*.
- [`runs/reload-earth-rss-fix-diag2.log`](runs/reload-earth-rss-fix-diag2.log) — Diag 2: the same six
  loads.
- [`runs/reload-earth-rss-fix-chain.log`](runs/reload-earth-rss-fix-chain.log) — no instrument, one
  session: 27 loads of `reload-earth-rss-landed.sfs`, the craft *landed*, then, after going back to the
  space centre, 24 of `reload-earth-rss-resave.sfs`, the craft in *prelaunch*.
- [`runs/statics-earth-rss-fix.log`](runs/statics-earth-rss-fix.log) — no instrument, this mod with its
  statics fix: two loads of `reload-earth-rss-landed.sfs`, the craft sent to a 200 km orbit with
  `Alt+F12 → Cheats → Set Orbit`, then the space centre. At every step, the `PQSCity` of Earth and where
  they hang, and the destructible buildings and upgradeable facilities of the KSC, are listed. Read in
  [The KSC buildings, runway and launchpad](../docs/limits-and-solutions/the-ksc-buildings-runway-and-launchpad.md)
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
  to load. Used in [The seam between subdivision levels](../docs/limits-and-solutions/the-seam-between-subdivision-levels.md).
- [`runs/revert-earth-rss-fix-diag4-1-lines.txt`](runs/revert-earth-rss-fix-diag4-1-lines.txt) —
  [Terrain Precision Fix Diag 4](https://github.com/lhervier/KSP-TerrainPrecisionFixDiag4), MechJeb2
  added: the first session of [The seam with this mod](../docs/limits-and-solutions/the-seam-between-subdivision-levels.md#the-seam-with-this-mod),
  a craft on the launchpad at Cape Canaveral reverted to launch fifteen times. Its `KSP.log` was
  overwritten when the game was started again: this file holds every line Diag 4 wrote in it, copied
  before it was lost; the lines of this mod are lost with it.
- [`runs/revert-earth-rss-fix-diag4-2.log`](runs/revert-earth-rss-fix-diag4-2.log) — the same install:
  the second session, whole. It begins with the craft taken back from the Space Center, which KSP
  moved 8.3 m down as it loaded (`Moving Vessel down -8.346m`), and which was destroyed; then five
  reverts to launch, loads 16 to 20 of the chapter.
