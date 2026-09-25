# Colliders below the highest subdivision level

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: TBD on every body but Kerbin and the Mun.** `PQSMod_QuadMeshColliders` gives a collider to
every quad at or above `maxLevel - |maxLevelOffset|`, and this fix only acts on the highest level: with
an offset other than 0, the levels below it would have colliders and stay uncorrected. The offset is
**0** on Kerbin and on the Mun, read in flight, so there the fix covers every quad a craft can stand on.

Kopernicus sets the colliders' offset to 0 only on a sphere it creates without a template
(`Configuration/PQSLoader.cs`); with a template, the stock value of that template comes along. The
`maxLevelOffset` a Kopernicus config can set belongs to the scatter
(`Configuration/ModLoader/LandControl.cs`), not to the colliders, and Outer Planets Mod never sets it.

*To test:* read it on the other bodies — [PQS Bench](https://github.com/lhervier/KSP-PQSBench) prints it
for the body being flown over — and a pack that sets a non-zero one stays to be found.
