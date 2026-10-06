# Colliders below the highest subdivision level

Part of [Terrain Precision Fix](../../../README.md), one test of [Non-regression tests](../../non-regression.md), on [stock](../../non-regression.md#stock).

**Status: not a regression, and not corrected either — on stock, no such collider exists.** This mod
only corrects the quads of the highest subdivision level. A quad of a lower level that had a collider
would be left exactly as stock builds it: a craft resting on it would jump at every load, as it does
without this mod.

## Where colliders are

`PQSMod_QuadMeshColliders` gives a collider to every quad whose subdivision is at least
`maxLevel - |maxLevelOffset|` (KSP 1.12.5):

```csharp
// PQSMod_QuadMeshColliders.OnSetup
minLevel = sphere.maxLevel - Mathf.Abs(maxLevelOffset);

// PQSMod_QuadMeshColliders.OnQuadBuilt
if (quad.subdivision < minLevel)
    return;
```

With an offset of 0, only the highest level has colliders, and those are the quads this mod corrects.

## The offset on every stock body

Read in game, from the main menu, through the game's own API: for each body of
`FlightGlobals.Bodies`, its `pqsController`, the `maxLevel` of that sphere and the `maxLevelOffset` of
every `PQSMod_QuadMeshColliders` found on it or below it. Terrain detail preset High.

| Body | `maxLevel` | `maxLevelOffset` | Lowest level with a collider |
|---|---|---|---|
| Kerbin | 10 | 0 | 10 |
| Mun | 9 | 0 | 9 |
| Minmus | 7 | 0 | 7 |
| Moho | 9 | 0 | 9 |
| Eve | 10 | 0 | 10 |
| Duna | 9 | 0 | 9 |
| Ike | 7 | 0 | 7 |
| Laythe | 10 | 0 | 10 |
| Vall | 9 | 0 | 9 |
| Bop | 6 | 0 | 6 |
| Tylo | 9 | 0 | 9 |
| Gilly | 7 | 0 | 7 |
| Pol | 9 | 0 | 9 |
| Dres | 9 | 0 | 9 |
| Eeloo | 9 | 0 | 9 |

The Sun and Jool have no terrain. Each body has a single `PQSMod_QuadMeshColliders`, on its terrain
sphere; the oceans have none. `maxLevel` follows the player's terrain preset, the offset does not: with
an offset of 0, the colliders stay on the highest level whatever the preset.

So the case above is only supposed: no body has been found where a quad below the highest level carries
a collider.

## Kopernicus

Kopernicus cannot change this offset either. Its configs have no node for `PQSMod_QuadMeshColliders`;
the `maxLevelOffset` a config can set belongs to the scatter (`Configuration/ModLoader/LandControl.cs`),
not to the colliders. On a sphere it creates without a template, it sets the offset to 0
(`Configuration/PQSLoader.cs`); with a template, the stock value of that template comes along, which is
0 on every body in the table.

## A craft flying very fast over a landed one

One stock case remains, read in the code and not measured. Above a certain speed of the craft it
follows, the game stops subdividing the terrain up to the highest level:

```csharp
// PQS.UpdateVisual: the highest level allowed at the target's angular speed, per frame
if (angularTargetSpeed < 1.5707963f / Mathf.Pow(2f, itr) * maxQuadLenghtsPerFrame)   // 0.03
{
    maxLevelAtCurrentTgtSpeed = itr;
    break;
}

// PQ.UpdateSubdivision: a quad only splits below that level
if (subdivision < sphereRoot.maxLevelAtCurrentTgtSpeed)
```

That is no more than 3 % of a quad's width per frame: on Kerbin, with the preset High, some 830 m/s at
30 frames per second, and twice that at 60. A game slowed down by a large base does not bring it much
lower: below 25 frames per second, each frame moves the game on by no more than *Max Physics Delta-Time
per Frame* (0.04 s by default), so the speed never falls under some 690 m/s on Kerbin. Terrain that was not at the highest level already stays
below it, without any collider. A landed craft that the fast one passes within 200 m of still leaves the
rails: `OrbitPhysicsManager.LateUpdate` only compares its distance with the `unpack` range of its
situation. It could find no ground under it.

If it is real, it is a stock bug, the same with this mod and without it, and outside what this mod
fixes.
