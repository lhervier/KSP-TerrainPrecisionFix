# Ground anchors

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: TBD.** The ground anchor, the `ModuleGroundPart` part an engineer places in EVA construction, is
the part most exposed to the moment the ground is drawn, for two stock reasons read in the code: it is
the one thing that makes `Vessel.GoOffRails` skip the physics hold, so an anchored craft starts its
physics on the very first frame; and `groundAnchor.cfg` sets `kinematicDelay = 0`, so the anchor is
frozen (`PermanentGroundContact`, `FreezeAll`) after a single frame, at whatever height it is at that
instant.

It also has causes of its own, which this fix does not touch and does not claim:

- a part dropped in EVA construction is saved with `PQSMin`/`PQSMax` at `0/0` (written literally in
  `EVAConstructionModeEditor`), and `Vessel.GoOffRails` only skips the repositioning pass when those
  levels are non-zero and match the current `pqsController`, so the pass runs on every load;
- `Vessel.CheckGroundCollision` puts the lowest point of the craft's **colliders** on the terrain, and
  the anchor's collider stops above the part origin while its model reaches it, so the pass lifts it;
- its 10 cm dead zone is disabled when the root part carries a `ModuleGroundPart`, so even a millimetric
  correction is applied.

A stable ground should make the anchor's behaviour repeatable, not fix it. That is the subject of
KSP Community Fixes' issue [#214](https://github.com/KSPModdingLibs/KSPCommunityFixes/issues/214).

*To test,* on KSP + Harmony + ModuleManager + KSP Community Fixes + Terrain Precision Fix Diag 1, with and
without this fix: an anchor placed in EVA construction, **saved after each load** — the re-save is what
lets a lift accumulate. To read: the `Moving Vessel` lines of each load, the `PQSMin`/`PQSMax` of the
anchor in the `.sfs`, and the gap between the anchor's collider and its origin in `groundAnchor.mu`.
