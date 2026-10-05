# Existing saves

Part of [Terrain Precision Fix](../../README.md), one test of [Non-regression tests: the ground](../non-regression-ground.md).

**Status: checked, no problem — a landed craft goes through one more draw, always the same one.** The fix takes away the draw, and the draw was also an escape hatch: in stock, a base
that comes back buried and tears itself apart can be reloaded until it survives. With the fix, it breaks
the same way every time. What a player with a long-running save should expect:

- **the window is one loading per landed craft.** The craft was saved on the ground of one particular
  draw, and comes back on the corrected ground; once it has been loaded and saved again with the fix
  installed, both sides agree and the question never comes back;
- **the corrected ground is among the stock draws, not a worse one** — the readings are in
  [KSP Diag - Terrain Height, with this mod](../checking-the-culprit-loading.md#the-ground-over-six-loads);
- **the failure becomes repairable**: raising a craft by a few centimetres in the `.sfs` is a permanent
  repair once the ground is stable, where in stock the next loading draws the ground under it again.

Seen on the Moon under Real Solar System
([Rescaled systems: Real Solar System](../limits-and-solutions/rescaled-systems-real-solar-system.md)): a craft saved without this
mod, loaded once and saved again with it, then loaded 42 times with it without moving once.

*Still worth measuring:* on a copy of a long-running save, how far each landed base actually moves at its first
loading with the fix — a range in millimetres over real bases, to say how much transition there is to
absorb. The configuration expected to suffer most is modules docked to each other and standing on
landing legs.

*To test:* that a save made again with the fix no longer jumps, on Kerbin. On
[`switch-kerbin.sfs`](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/diag/switch-kerbin.sfs),
made without the fix, the capsule comes to rest 31.8 mm off, at the same place every time. With the fix,
load it, switch to the capsule with `]` and back to the rover, save under another name, then six times
"load → *Record* → `]` → *Record*" with KSP Diag - Landed Vessel: *Moved* should stay within a few
hundredths of a millimetre of zero.
