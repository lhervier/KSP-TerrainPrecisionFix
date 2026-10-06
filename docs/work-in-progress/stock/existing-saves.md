# Existing saves

Part of [Terrain Precision Fix](../../../README.md), one point of [Work in progress](../../work-in-progress.md), on [stock](../../work-in-progress.md#stock).

**Status: planned.** A craft saved without this mod comes back once on the corrected ground; loaded
and saved again with it, on the Moon under Real Solar System, the workaround of Real Solar System that
moves a landed craft back onto the ground never had to move it, in
[Existing saves](../../non-regression/stock/existing-saves.md). Still open:

- **A save made again with the fix, on Kerbin.** On
  [`switch-kerbin.sfs`](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/diag/switch-kerbin.sfs),
  made without the fix, the capsule comes to rest 31.8 mm off, at the same place every time. With the
  fix, load it, switch to the capsule with `]` and back to the rover, save under another name, then six
  times "load → *Record* → `]` → *Record*" with KSP Diag - Landed Vessel: *Moved* should stay within a
  few hundredths of a millimetre of zero. If it did not, a craft saved again with this mod would still
  move at every load.
- **A long-running save.** On a copy of one, how far each landed base actually moves at its first
  loading with the fix — a range in millimetres over real bases, to say how much transition there is to
  absorb. The configuration expected to suffer most is modules docked to each other and standing on
  landing legs.
