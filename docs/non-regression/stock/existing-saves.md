# Existing saves

Part of [Terrain Precision Fix](../../../README.md), one test of [Non-regression tests](../../non-regression.md), on [stock](../../non-regression.md#stock).

**Status: checked, no problem — a landed craft goes through one more draw, always the same one.** The fix takes away the draw, and the draw was also an escape hatch: in stock, a base
that comes back buried and tears itself apart can be reloaded until it survives. With the fix, it breaks
the same way every time. What a player with a long-running save should expect:

- **the window is one loading per landed craft.** The craft was saved on the ground of one particular
  draw, and comes back on the corrected ground; once it has been loaded and saved again with the fix
  installed, both sides agree and the question never comes back;
- **the corrected ground is among the stock draws, not a worse one** — the readings are in
  [KSP Diag - Terrain Height, with this mod](../../checking-the-culprit-loading.md#the-ground-over-six-loads);
- **the failure becomes repairable**: raising a craft by a few centimetres in the `.sfs` is a permanent
  repair once the ground is stable, where in stock the next loading draws the ground under it again.

Seen on the Moon under Real Solar System: a craft saved without this mod, loaded once and saved again
with it (`reload-moon-rss-resave.sfs`), then loaded 42 times with it. What tells is Real Solar System's
own ground workaround, which moves a landed craft back onto the ground at every load where it comes back
more than 10 cm off, and logs `Moving Vessel` when it does
([Real Solar System: the ground workaround](../real-solar-system/the-ground-workaround.md)). Without this
mod, on the save made without it, it had to move the craft at 6 loads out of 14. On the save made again with this
mod, it ran at 36 of the 42 loads — 24 in a row, and the six of each instrument in
[Checking the culprit: loading the same save](../../checking-the-culprit-loading.md) — and never once had
to move the craft: no `Moving Vessel` line in `KSP.log`. At the six other loads, the workaround turned
off, the craft came to rest within 0.364 mm.
