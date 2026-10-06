# Deferred: drawing the ground

Part of [Terrain Precision Fix](../../../README.md), one test of [Non-regression tests](../../non-regression.md), on [Deferred](../../non-regression.md#deferred).

**Status: checked, no problem.** [Deferred](https://github.com/LGhassen/Deferred) replaces the way KSP
draws everything, the terrain included, and it is the mod that broke KSP Community Fixes' own
`PQSOnlyStartOnce`: terrain stopped loading for some players, and that patch has been disabled by default
since. So anything touching the terrain spheres gets checked against it. Deferred draws the ground wherever
it is placed and patches nothing this mod patches; with it installed, the craft and the ground come back
as they do without it, after a load and after a trip out of range.

## What Deferred and Shabby touch

Read in the source of Deferred 1.3.5 and of [Shabby](https://github.com/KSPModdingLibs/Shabby) 0.4.2,
which Deferred requires.

**Deferred has no Harmony patch.** It turns the cameras to deferred shading, gives the stock materials
shaders that have a deferred pass, and adds its own passes to the cameras. What of it comes near the
terrain:

- **its terrain shaders** take the place of the stock `PQS` shaders. They compute the texturing from the
  world position of each vertex and the floating origin offset, as the stock ones do, and the normals from
  the renderer's matrices. They draw a quad wherever it is: one placed by stock and one placed by this mod
  are the same to them;
- **the fade of the terrain** into the distant view of the planet, `DeferredPQSFade`, reads the opacity
  KSP sets on the surface material of the terrain sphere, and composes the image. No position is read;
- **`KSCModelNormalsFixer`** recomputes the normals of the ground meshes of the KSC facilities and of the
  Island Airfield. It runs once, at the main menu, on the shared meshes. This mod only moves statics in
  flight, and never touches a mesh;
- **its reflection probe for the KSC** only exists in the space centre scene, where the statics fix of
  this mod does nothing.

**Shabby** patches how part icons and part models get their materials (`PartLoader.SetPartIconMaterials`,
`PartLoader.CompileModel`, and KSP Community Fixes' own icon shader lookup), and turns the calls to
`Shader.Find` in the mods' assemblies into a lookup of its own. Nothing of the terrain, nor of the statics.

Neither of them patches any of the methods this mod patches.

**Why `PQSOnlyStartOnce` is not the same case.** That patch removes the second call to
`PSystemSetup.SetPQSActive()` in `FlightDriver.setStartupNewVessel`: it changes when the terrain spheres
are activated on a launch. This mod changes nothing of that order. It changes where a quad and a static
are placed, and Deferred only sees that through the positions it draws.

## Measured

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1, Deferred 1.3.5, Shabby 0.4.2, this mod
with its defaults (at `logLevel = Debug`), both instruments, and
[KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer), which plays the protocols: each campaign
below is a script of [`diag/automation/`](../../../diag/automation) that does, in the same order, what the
protocol asks a player to do, and records in both instruments at every stop. The terrain is drawn as
usual, on the screenshots taken along the way.

**Loading the same save.** [The runway protocol](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/docs/the-protocol-runway.md),
played by [`run-runway.py`](../../../diag/automation/run-runway.py): `runway-kerbin.sfs` loaded six times,
the craft on the grass read first, then the one on the runway, switched to as the `[` key does. The same
script was played once more on the same install with Deferred and Shabby taken out. Spread over the six
loads:

| | without Deferred | with Deferred |
|---|---|---|
| the craft on the grass, *Settled* | 0.247 mm | 0.259 mm |
| the craft on the runway, *Settled* | 0.215 mm | 0.147 mm |
| the ground under the craft on the grass | 0.058 mm | 0.036 mm |
| the ground under the craft on the runway | 0.268 mm | 0.171 mm |

Hundredths to tenths of a millimetre either way, where stock spreads the same readings over 88 to 130 mm
([Checking the culprit: loading the same save](../../checking-the-culprit-loading.md)). In both series, one
load out of six sets the craft on the grass down 0.23 mm higher than the five others, while the ground
under it does not move: the craft's own settling, with or without Deferred.

**Coming back to a craft left parked.** [The approach protocol](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/docs/the-protocol-approach.md),
played by [`run-approach.py`](../../../diag/automation/run-approach.py): `approach-kerbin.sfs`, six round
trips of the rover to 3.1 km south of the parked craft and back, in a single flight. The rover drives with
the cheat *Infinite Electricity* on, so that close to forty kilometres of driving do not depend on its
batteries.

| round trip | 1 | 2 | 3 | 4 | 5 | 6 |
|---|---|---|---|---|---|---|
| *Moved* | −0.107 mm | −0.010 mm | +0.020 mm | −0.020 mm | +0.039 mm | −0.039 mm |
| *Difference* moved by | −0.009 mm | +0.007 mm | −0.004 mm | −0.001 mm | −0.004 mm | +0.010 mm |

Over the six, the craft comes to rest across a spread of 0.039 mm, and the ground under it across
0.011 mm, against 0.094 mm and 0.011 mm with this mod and no Deferred, and 21.8 mm for both in stock
([Checking the culprit: coming back to a craft left parked](../../checking-the-culprit-approach.md)).

The logs and the lines read are in [`diag/runs/`](../../../diag/README.md#with-deferred).
