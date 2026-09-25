# Sloped ground

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: TBD.** Every campaign so far is on flat ground, because Terrain Precision Fix Diag 1 asks for
it. On a slope, a separate stock bug, read in the code and not measured, puts a single-part craft down
into the ground at every load. This fix does not touch it, and a reading taken there would show it.

On unpacking, `Vessel.CheckGroundCollision` puts the craft back onto the ground. It runs on every load
for a craft made of a single part (`Vessel.GoOffRails`), and in a few other cases. It compares `D`, the
distance from the root down to the ground along the **vertical**, with `L`, the distance from the root
to the lowest point of the craft along the **ground normal**, and moves the craft by `L' − D`:

```csharp
float num5 = Mathf.Cos(Mathf.Abs((float)Vector3d.Angle(groundCollisionHit.normal, vector3d2)) * ((float)Math.PI / 180f)) * num4;
if (Mathf.Abs(num4 - num5) > 0.1f)
    num4 = num5;
```

`num4` is `L`, and `vector3d2` the vertical. For a craft resting on a flat slope of angle `α`, the root
is `L / cos α` above the ground measured along the vertical, which is what `D` finds, so the move should
be zero. The code multiplies by `cos α` where it should divide, and only above 10 cm of difference.
Either way `L' < D`, so the craft is always moved **down**:

| branch | taken when | move |
|---|---|---|
| `L` kept | `L (1 − cos α) ≤ 0.1 m` | `L (1/cos α − 1)` |
| `L cos α` | `L (1 − cos α) > 0.1 m` | `L sin²α / cos α` |

For `L = 1 m`: 15 mm at 10°, 64 mm at 20°, 103 mm at 25°, 289 mm at 30°. Moves under 10 cm are not
applied, except when the root carries a `ModuleGroundPart`.

*To test:* the campaign already run on flat ground, run again on a slope of 30° or more — the same
install, the same lone capsule, the same six loads, without and with this fix. It is the one case where
the two defects can be told apart: the fix makes the ground stop moving and leaves the slope move as it
is. Expected, if the reading is right: a `ground contact! - error. Moving Vessel down` line at every
load, whose value changes from load to load without the fix, and comes back the same with it. The same
craft with a second part, which skips the pass, is the counter-test.
