# TODO

What is left to do, and only that. Everything already measured is in [the README](README.md) and its
chapters under [docs/](docs/), with its logs in [perfs/](perfs/) and in the repositories of the two
probes; no result is recorded here.

## Before opening the KSPCF issue

Not everything below has to be done first. What does, in the order a reviewer will ask for it:

0. **A GitHub release on each repository the issue sends a reader to.** This one is not a measurement,
   it is a hard prerequisite: the issue opens by asking the reader to install Terrain Precision Fix
   Diag 1, and both Diag READMEs send them to
   `https://github.com/lhervier/KSP-TerrainPrecisionFix<...>/releases/latest` under `Get it`. Neither
   repository has a single release today, so that link is a 404 — on the exact page a maintainer lands
   on from the first instruction of the repro. Nor does this repository, which the issue also links.
   `build.bat` already produces the zip; what is missing is the release itself. At least Diag 1 and
   Diag 2, which the repro needs, and this repository, which the fix section offers. PQS Bench and
   Stock Quad Cache are linked only as supporting material for the performance figure, so they can wait
   — but their `Get it` sections should not promise a download that does not exist either. Check every
   `releases/latest` link across the family before the issue goes out.
1. **The ground anchor, in an install a reviewer could rebuild.** The issue opens on it, so every
   reading it quotes has to be reproducible by someone else. They are not, yet: they come from the
   author's own years-old save, in a game that also has EvaCMGroundPlugin installed. Redo them plainly,
   on KSP + Harmony + ModuleManager + KSPCF + Diag 1, with the fix for the second half. What has to come
   out of it: the `Moving Vessel` values without the fix (both signs, a different one every load) and
   with it (the same value at every load); a `.sfs` showing `PQSMin`/`PQSMax` at `0/0` on a
   freshly placed anchor; and the 2.08 cm collider gap re-read from `groundAnchor.mu`. ⚠️ **The protocol
   differs from every other campaign here: save after each load.** It is the re-save that arms the
   ratchet — without it the anchor is put back to the same place every time and the climb never appears.
2. **The whole-frame cost, with KSPProfiler.** The performance figure is a micro-benchmark, and a
   reviewer can dismiss it in one line: 3.56× on one method is not a frame. PQS Bench cannot answer
   that — 164 ns per vertex is below anything a frame breakdown resolves — but
   [KSPProfiler](https://github.com/KSPModdingLibs/KSPProfiler), written by one of KSPCF's own
   maintainers, can: it inserts itself into Unity's player loop and reports mean / median / worst 25 % /
   worst 1 % per frame phase. Fly the same low pass with it installed, with and without the fix, and
   report what the frame does. Install the two side by side and do **not** couple them: its
   `GameLoopProfilerCaptureBase.captures` list is public and PQS Bench could register into its UI, but
   referencing its assembly would cost the instruments their "runs on a stock install with no
   dependencies", which is most of what makes them worth handing to a stranger.
3. **Kopernicus, twice.** Most planet packs go through it, so this is the compatibility question a
   reviewer asks first. Two runs are missing, not one:
   - **a clean stock-body series with the terrain instruments.** The reading that exists came out of a
     scatter campaign, in an install built for scatter, so its protocol cannot be described without
     describing that campaign — which means the figure cannot be quoted anywhere that does not also
     tell that story. Re-run it plainly: Kopernicus on Kerbin, Diag 1 and Diag 2, six loads, with and
     without the fix. Cheap, and it turns a figure that has to be explained away into one that can
     simply be given;
   - **a body a planet pack creates or reconfigures**, rather than merely loads. Nothing has been
     measured there at all.
4. **Existing saves.** The transition a player feels on a save made before the fix, written in the
   README rather than discovered in play.
5. **Breaking Ground surface features.** They have colliders and are placed like the rocks. Enough to
   know whether the fix introduces a physical offset there, even if the answer is "yes, and it needs a
   fix of its own".
6. **The same campaign on a slope.** Every campaign so far is on flat ground, because Terrain Precision
   Fix Diag 1 asks for it. On a slope a single-part craft is put into the ground at every load by a
   stock bug this fix does not touch, and a reviewer who meets it there will read it as "the fix does
   not work".

Everything else below can be listed as open in the README without holding the issue back.

## What the fix still has to be checked against

### Kopernicus — required, half done

Most planet packs go through Kopernicus, so the fix is not worth proposing until it works with it.

**Measured, on a stock body** (Kerbin, Kopernicus 1.12.1.247 + SSCEP, the scatter-collider series of
Rock Precision Fix Diag, six loads per install): the safeguard never fired, and the height of a quad
spreads 116 mm (median, 201 mm at worst) over six loads without the fix against 0.079 mm (0.265 mm at
worst) with it — so Kopernicus leaves stock bodies' quads hanging in the frame the fix computes. Written
up in [Kopernicus, on a stock body](docs/limits-and-solutions.md#kopernicus-on-a-stock-body).

Still to check:

- **A body from a planet pack**, which Kopernicus creates or reconfigures rather than merely loads: the
  frame the fix computes positions in (`body.rotation`, `body.position`) has to still be the one the
  quads hang from there too. The 1 m safeguard would catch a mismatch, but the result would be no fix,
  silently, apart from a warning in the log. Which pack and which body to run it on, and what that
  campaign would and would not settle, are worked out in
  [A body from a planet pack](docs/limits-and-solutions.md#a-body-from-a-planet-pack); what is left to
  do is the landed save on that body, and the run itself.
- **The collision surface under Kopernicus**, which only Terrain Precision Fix Diag 2 reads. The
  measurement above is the position the quads are placed at, not the surface a craft rests on. It does
  not need a planet pack: a Kopernicus install and the Diag 2 campaign on a stock body are enough.
- `maxLevelOffset`: the one Kopernicus lets a config set belongs to the scatter
  (`Configuration/ModLoader/LandControl.cs`), not to the colliders. On the terrains it creates,
  Kopernicus sets the colliders' `maxLevelOffset` to 0 (`Configuration/PQSLoader.cs`). On the bodies it
  only modifies, it keeps the stock value, which is still unknown (see below).
- `DisableFarAwayColliders` (`RuntimeUtility/SinkingBugFix.cs`) disables every collider of a body whose
  centre is more than 10,000 km from the world origin, to work around a PhysX raycast bug. That never
  includes the terrain under the craft, so it should not interact with the fix.
- Kopernicus replaces the stock scatter holder with its own subclass,
  `PQSMod_KopernicusLandClassScatterQuad`, and can give scatter objects colliders (`scatterColliders`).
  Measured, that one: the offset is physical, this fix halves it without closing it, and Rock Precision
  Fix closes it (see [Scatter with colliders](docs/limits-and-solutions.md#scatter-with-colliders)). The
  same question stays open for the ROCs below.
- Measure it, with Terrain Precision Fix Diag 1, on a body from a planet pack.

### Deferred rendering — because it broke the last PQS patch

Not because anything suggests it interacts with this one. `Deferred` replaces KSP's rendering path and
has no reason to care where a quad is placed. It is on this list because it is the mod that broke
KSPCF's own `PQSOnlyStartOnce` patch: that patch shipped, terrain stopped loading for some players, it
was disabled by default the same day, the cause was narrowed to `Deferred` two weeks later, and it is
still disabled. Anyone reviewing a PQS patch will have that in mind, so the install is worth having:
`Deferred` plus this fix, the Diag 2 campaign, and a look at whether the terrain still renders. Cheap,
and it answers a question that will be asked.

### Existing saves — the one risk a player can feel

The fix takes away the draw, and the draw was also an escape hatch: in stock, a base that comes back
buried and tears itself apart can be reloaded until it survives. With the fix, it breaks the same way
every time. The one campaign run on a real save is done and not written up yet, so both the writing and
what is still missing from it are below.

**To write in the README**, on the transition itself:

- **the window is one loading per landed craft.** The craft was saved on the ground of one particular
  draw, and comes back on the corrected ground; once it has been loaded and saved again with the fix
  installed, both sides agree and the question never comes back;
- **the corrected ground is among the stock draws, not a worse one** — the readings are in
  [Terrain Precision Fix Diag 2, with this mod](docs/checking-the-culprit.md#terrain-precision-fix-diag-2-with-this-mod-the-ground);
- **the failure becomes repairable**: raising a craft by a few centimetres in the `.sfs` is a permanent
  repair once the ground is stable, where in stock the next loading redraws the ground under it anyway;
- **KSPCF patches are individually switchable**, so a player whose years-old base does not survive can
  turn this one off, load, raise the craft, and turn it back on. To confirm against the KSPCF settings
  mechanism before putting it in the README.

**To write about the run** (KSP 1.12.5, Harmony, KSPCF, this fix, nothing else, on the author's own
years-old save, 2026-09-13): the bases on the Mun, Minmus and Gilly loaded one by one, and none broke;
the base on Eve settles onto feet that were built below the surface, on loading and on leaving time warp
alike, which is a construction defect and not this fix. Against that, what the save does not sample: its
bases are anchored and built on girder rails that follow the curvature of the terrain, then docked and
strutted, so they barely flex — where the configuration that suffers most is modules docked to each
other and standing on **landing legs**. One screenshot, to show the scale of what was loaded, still has
to be added to the repository; it proves nothing on its own.

Still to do on that campaign:

- **turn "nothing broke" into a bound**: on a copy of that save — never the original — read how far each
  landed base actually moved at the first loading with the fix. A range in millimetres over real bases
  says how much transition there is to absorb;
- **decide where the docked-assemblies and landing-legs caveat goes.** The README's `Disclaimer` is
  copied word for word into the two Diag READMEs, so adding to it means editing all three; the
  alternative is the correcting mod's own closing paragraph, which is allowed to differ;
- **one sentence under `How this was made`** saying those bases were assembled in EVA construction with
  a personal tool (KSP-EvaCMGroundPlugin), that they load and work without it, and nothing more: one
  issue, one bug, and the second bug that tool addresses stays closed unless it is asked about;
- **open, and only worth doing if that bound shows a real cost**: a migration helper that walks a `.sfs`
  and raises landed vessels onto the corrected ground. A separate tool, not a prerequisite.

### Breaking Ground

- **Surface features (ROC)**, the ones studied in EVA or with the robotic arms: placed like the rocks,
  `PQSMod_ROCScatterQuad.Setup` does the same `localPosition = quad.positionPlanet`. Unlike stock rocks,
  they have colliders. Where the physics puts those colliders, whether with the transform position or
  with the matrix, is not measured, and it decides whether this fix introduces a physical offset there.
  The neighbouring case is measured, on scatter given colliders by Kopernicus, and says the offset is
  real and that this fix halves it without closing it (see
  [Scatter with colliders](docs/limits-and-solutions.md#scatter-with-colliders)); whether a ROC hangs
  under its holder the same way is the thing to read, then measure. A subject of its own, with a fix of
  its own, not part of Rock Precision Fix. What the code shows so far: the holder hangs from a
  `rocParent` that `LandClassROC` creates as a child of the terrain sphere, at the identity, and is
  released through `roc.DestroyQuad(this)`, called from the quad's `onDestroy`. The identifier of a
  surface feature depends on its position within the quad (`rocPOS`, taken from `quad.verts`), not on
  the pose of its holder, so moving the holder would not change it.
- **Deployed experiments** (`ModuleGroundPart` and related modules): they are vessels, positioned in
  double like any craft, so a priori they already benefit from the corrected ground. To check with
  Terrain Precision Fix Diag 1 rather than assume. They are also the parts `Vessel.GoOffRails` skips the
  physics hold for.

### KSC buildings (`PQSCity`, `PQSCity2`)

Both do `base.transform.localPosition = planetRelativePosition;` with a `Vector3d` measured from the
centre of the body (`PQSCity` twice, `PQSCity2` three times), so they carry the same defect and the fix
does not cover them. A capsule on the runway spreads as much as on the grass next to it, but the runway
sits on terrain that `PQSCity` flattens, so that reading cannot tell the two apart. Differential test:
with this fix installed the terrain is stable, so if the capsule on the runway still moves, it is the
static.

### Colliders below the highest level, on the other bodies

`PQSMod_QuadMeshColliders` gives a collider to every quad at or above
`sphere.maxLevel - |maxLevelOffset|`, and the fix only acts on the highest level: with an offset other
than 0, the levels below it would have colliders and stay uncorrected. The value is **0** on Kerbin and
on the Mun, read in flight, and the mod logs it. Still worth reading on the other bodies as they are
visited, since nothing says a body cannot ship a different offset.

### The map view

Quads are built and destroyed there all the time, and nothing has been measured. To measure.

### The cost of the fix on Kerbin

The performance campaign was flown over the Mun. The same measurement on Kerbin, where quads are four
times larger and a craft can be made to fly low for much longer, is worth having. Not a prerequisite.

### KSPCF patches on the PQS

Verified separately, and worth one sentence next to the claim in the README: no KSPCF patch touches the
placement of PQS quads. Record the version or commit checked.

### Other things that stand on the ground — not looked at yet

Four cases nothing above covers. None of them has been read in the code, let alone measured: what
follows is the question and how to answer it, not a finding.

- **Kerbal Konstructs.** It plants statics — pads, runways, whole bases — anywhere on a body, the way
  `PQSCity` does at the KSC, and a great many players park their craft on them. Two questions, in
  order: does it position a static from the centre of the body through a float `Transform`, in which
  case it carries the same defect and this fix does not cover it (same case as the KSC buildings
  above); and does a static it places move **relative to** the corrected ground, which a player would
  see as a craft on a Konstructs pad sitting a few centimetres off. Its source is not in `kspmod-ext`:
  clone it and read `StaticObject`'s placement before guessing anything. Then measure, with Terrain
  Precision Fix Diag 2 reading the ground under a static against the ground beside it.

  **And a third question, which looked like the worst of the three until the source was read: it does
  not only stand on the terrain, it reshapes it.** Kerbal Konstructs flattens ground to seat what it
  plants, so a player who installs it expects the terrain under a Konstructs site to *not* be what the
  stock heightmap says. A fix that quietly undid those edits would be exactly the kind of "sneaky, very
  situational" side effect KSPCF's maintainers say they worry about, and it would be found by a player
  rather than by us.

  Read in the source (now cloned to `kspmod-ext\Kerbal-Konstructs`, GER-Space's fork), and it is
  reassuring: **Konstructs writes no terrain code of its own.** `Core/MapDecals/MapDecalInstance.cs`
  does `gameObject.AddComponent<PQSMod_MapDecal>()`, hangs it off `CelestialBody.pqsController`, and
  calls its stock `OnSetup()`. Its whole terrain-editing feature is the stock `PQSMod_MapDecal` /
  `PQSMod_MapDecalTangent` — the same components stock uses to flatten the ground around the KSC.
  Nothing else in `src/Core` touches the PQS beyond reading it (`GetSurfaceHeight`, `GetRelativePosition`,
  `radius`) and asking for a `RebuildSphere` after a decal changes.

  That matters because of *where* those components act. A decal edits `vbData.vertHeight` in
  `OnVertexBuildHeight`, which runs **before** `PQS.BuildVertexSurfaceRelative` consumes it — and this
  fix consumes exactly the same `vbData.directionFromCenter * vbData.vertHeight`. So the height it
  places is the already-decalled one, and the reshaped terrain should be carried through untouched.
  `PQSMod_MapDecal.OnQuadBuilt` only resets two flags, so there is no quad-level ordering conflict
  either. Parallax is in the same position: its only decal-related `PQSMod`,
  `PQSMod_MapDecalVertexRemoveScatter`, removes scatter inside a decal and does not touch height.

  **All of that is read, not measured**, and it is the KSC flattening path, which the campaigns already
  cross without anything odd showing up. What is left to do is cheap and should still be done: a
  Konstructs site, with and without this fix, Diag 2 reading the ground inside the flattened area and
  just outside it. Note that the 1 m safeguard would not catch a decal going wrong, since the quads
  would still be within a metre of where they belong.

- **Asteroids grabbed with a claw.** An asteroid is a vessel, positioned in double like any other, so
  on its own it should follow the corrected ground. Neither `ModuleAsteroid` nor `ModuleGrappleNode`
  reads the PQS (checked in the decompiled source), so there is no second placement path here — the
  case to test is the stock-fragile one: an asteroid resting on the ground, grappled by a claw, where
  the joint was made against a body the terrain was holding up. On the first load with the fix that
  ground moves once, by up to the stock spread, with a mass on the other end of a joint that is already
  the game's least stable. To test: a clawed asteroid on the Mun, saved and reloaded several times,
  with and without the fix.

- **Ground anchors** (the `ModuleGroundPart` part an engineer places in EVA construction — the bases in
  the save above are built on one). It is the part most exposed to *when* the ground is drawn, for two
  stock reasons: it is the one thing that makes `Vessel.GoOffRails` skip the physics hold, so an
  anchored craft starts its physics on the very first frame whatever state the terrain is in; and
  `groundAnchor.cfg` sets `kinematicDelay = 0`, so the anchor is frozen (`PermanentGroundContact`,
  `FreezeAll`) after a single render frame, at whatever height it happens to be at that instant. Welded
  rather than settled, in other words. A stable ground should make that better, not worse, but the part
  deserves to be read rather than assumed — and it is also the one case where `CheckGroundCollision`
  applies a move below 10 cm. **Already measured, and the answer is that this fix is not enough**: with
  it installed the repositioning pass becomes perfectly repeatable (`Moving Vessel up 0.042m`, the same
  value at four loads of one file on the Mun) but it does not go away, because it has two causes of its
  own that have nothing to do with the terrain — a part dropped in EVA construction is saved with
  `PQSMin`/`PQSMax` at `0/0`, which arms the pass at every load, and the anchor's collider stops 2.08 cm
  above the part origin while its model reaches it, so the pass lifts it by about 4.2 cm. Since the rivet
  (`PermanentGroundContact`, `FreezeAll`) lands 20 ms later, the lifted position is what gets saved: a
  ratchet, not a wobble. That is a **separate bug with a separate fix**, and this repository should claim
  none of it. It belongs to whatever proposal covers the anchor.

- **KAS.** Kerbal Attachment System attaches parts to each other and to the ground with joints of its
  own, and its static attachment is exactly the kind of thing that either rides on a vessel (and
  follows the corrected ground for free) or is pinned to the terrain sphere (and does not). Its source
  is not in `kspmod-ext` either: clone it, find how a statically attached part is anchored, and only
  then decide whether there is anything to measure.

## Stock bugs still to test

### Does the stock ground move during a single flight?

Loading is the moment the README measures, because it is the one that can be repeated at will. The same
draw should happen without any reload, since the world matrix of the terrain sphere moves at every
floating origin shift and stock places landed quads again at each of them
(`CelestialBody.PreciseUpdateQuadPositions`). If it does, the defect is wider than loading.

To measure: a capsule and a rover side by side on Kerbin, Terrain Precision Fix Diag 2 reading the
ground under the capsule, the rover driven beyond 500 m and back. Expected: *Difference* jumps at every
origin shift in stock, and does not with the fix, which patches that path too.

### Craft pushed into slopes on load — the `cos α` bug

Read in the code, not measured. Not a float rounding issue, and not something this fix touches. It is
the reason Terrain Precision Fix Diag 1 asks for flat ground — and the reason for the slope campaign in
the shortlist above.

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
applied, except when the root carries a `ModuleGroundPart`. Zero on flat ground, growing with the height
of the craft and quickly with the slope.

**To measure: the campaign already run on flat ground, run again on a slope.** The same install (KSP
1.12.5, Harmony, ModuleManager, KSP Community Fixes 1.41.1, Terrain Precision Fix Diag 1), the same lone
capsule, the same six loads — on a slope of 30° or more, once without this fix and once with it. This is
the one case where the two defects can be told apart, since the fix makes the ground stop moving and
leaves the slope move as it is.

Expected, if the reading of the code is right:

- **without the fix**, *Settled* wanders as it does on the flat, and a
  `ground contact! - error. Moving Vessel down` line appears at every load, with a value that changes
  from one load to the next because the ground does;
- **with the fix**, that line comes back with the *same* value at every load, matching the second
  formula with `α` taken from the normal under the craft. *Settled* stops varying, as everywhere else,
  but it settles **below** where the save put the craft, by that move.

What the reading does not say, and the campaign has to: **what the craft then does**. Pushed a few
centimetres into the ground it is thrown back out, which is the jump the README is about; pushed deeper,
the collision surface is a skin with nothing behind it, and a craft below it may simply stay buried.

Counter-test: the same craft with a second part, which skips the pass, and the line should disappear.
And on uneven ground a `down` line can also come from `Vessel.Load` raising the craft to the analytic
terrain height first; check that before reading it as this bug.

What it changes here, whatever the numbers: one paragraph in the README saying that on a slope a
single-part craft is still put down into the ground by stock, that this fix does not touch it, and that
with the fix it at least happens the same way at every load.
