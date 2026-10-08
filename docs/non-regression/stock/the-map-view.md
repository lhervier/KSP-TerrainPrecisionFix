# The map view

Part of [Terrain Precision Fix](../../../README.md), one test of [Non-regression tests](../../non-regression.md), on [stock](../../non-regression.md#stock).

**Status: nothing to test, read in the code — the map view changes nothing in what this mod works on.**
The map draws the bodies from scaled space, which this mod does not touch. The terrain keeps being
built around the active craft while the map is open, exactly as in flight, and the markers of the
launch sites follow their statics wherever this mod places them.

## The map does not draw the terrain

The map view draws every body as a scaled-down model of its own, in scaled space. The terrain quads,
which this mod places, belong to the terrain sphere of the body, which is not what the map shows. This
mod patches nothing in scaled space.

## The terrain keeps going as in flight

The terrain sphere of each body is switched on and off by `PQSMod_CelestialBodyTransform.OnPreUpdate`,
by the altitude of the camera. While the map is open, that switch is held: an active sphere is not
switched off, an inactive one is not switched on, whatever the altitude. Stripped of the rest:

```csharp
// PQSMod_CelestialBodyTransform.OnPreUpdate
if (sphere.isActive)
{
    if (!MapView.MapIsEnabled && sphere.visibleAltitude > deactivateAltitude)
        sphere.DeactivateSphere();
}
else if (!MapView.MapIsEnabled && sphere.visibleAltitude < deactivateAltitude)
    sphere.ActivateSphere();
```

The same method keeps the target of the sphere on the active craft, map or not
(`sphere.SetTarget(FlightGlobals.fetch.activeVessel.transform)`). So an active sphere goes on
subdividing around the craft, as it does in flight: the map builds and drops no quad that flight would
not, and the quads of the highest subdivision level, those this mod corrects, are the ones around the
craft, as in flight. What this mod does there is what the checks of [the ground](../../../README.md#the-ground)
measure.

## The launch sites on the map

The map marks each launch site with a node, placed from the site's `Transform`
(`SiteNode`, through `LaunchSite.GetWorldPos`). That `Transform` belongs to a static, which this mod
takes out of its sphere in flight, near a craft (see [The fix: the statics](../../the-fix-statics.md)).
`LaunchSite.GetWorldPos` would look for it by its path under the sphere, and not find it there; but it
only does so when nothing is cached, and `LaunchSite.Setup` caches it when the game loads the planetary
system, long before any flight. The node then follows the `Transform` itself, wherever this mod
places it, and this mod keeps the static at its place on the ground.
