<!-- comment to post on KSPCommunityFixes issue #214, once the terrain issue is opened -->
<!-- replace #NNN with the number of the terrain issue -->

I think this one is two problems stacked, and that is why it has never been reproducible.

The first is the ground. KSP rebuilds the terrain collision surface at a different height on every
load — over six loads of one save it spreads 10.7 cm on Kerbin — so the repositioning pass that runs
on the anchor is computing against a surface that moves under it. In stock it logs `Moving Vessel
down -0.040m` on one load and `up 0.001m` on the next: both signs, never the same value. That part is
#NNN, with the measurements and a patch.

The second is the anchor itself, and it does not go away when the ground is stable — it just becomes
readable. With the ground held still the same file gives `up 0.042m`, four times out of four, and it
is the sum of three stock behaviours:

- `Vessel.CheckGroundCollision` puts the lowest point of the craft's **colliders** on the terrain, and
  `groundAnchor`'s collider stops 2.08 cm above the part origin while its visible mesh reaches it (the
  four spikes have no collider at all) — so the pass leaves the base of the anchor in the air;
- the 10 cm dead zone that would normally swallow such a correction is **explicitly disabled** when
  the root part carries a `ModuleGroundPart`, so it is applied anyway;
- about 20 ms later `ModuleGroundPart.MakePartKinematic` rivets the part at the position it was just
  lifted to (`PermanentGroundContact`, `FreezeAll`, a `FixedJoint`), and that becomes the altitude
  written to the next save. It is a ratchet, not a wobble, which is why it only ever goes up.

And it repeats forever because a part dropped in EVA construction is saved with `PQSMin`/`PQSMax` at
`0/0` (written literally in `EVAConstructionModeEditor`), while `Vessel.GoOffRails` only skips the
repositioning when those levels are non-zero and match the current `pqsController`.

All of it measured on stock 1.12.5. Happy to turn the second half into a patch if you want it.
