# Loading, an orbit and the space centre

Part of [Terrain Precision Fix](../../../README.md), one test of [Non-regression tests](../../non-regression.md), on [stock](../../non-regression.md#stock).

**Status: checked.**

*Why.* This mod takes the KSC out of its sphere when a craft comes near it, and has to put it back
before the code that expects it there runs. KSP finds the buildings of the KSC by their place in the
hierarchy below it.

*The test.* Load a save with a craft 1.4 km from the KSC, several times, then send the craft to a
200 km orbit (`Alt+F12 → Cheats → Set Orbit`), then go back to the space centre. On Kerbin,
[`runway-kerbin.sfs`](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/diag/runway-kerbin.sfs),
six loadings; on Earth, in [Real Solar System](../../non-regression.md#real-solar-system),
[`reload-earth-rss-landed.sfs`](https://github.com/lhervier/KSP-Diag-LandedVessel/blob/main/diag/reload-earth-rss-landed.sfs),
two.

*The result.* At every loading, the KSC is taken out of its sphere, and nothing else is: on Kerbin, the
Island Airfield, 33 km away, stays under its sphere. The KSC is put back when the save is loaded again
and when the craft reaches orbit, and the space centre opens normally. Its 39 destructible buildings and
9 upgradeable facilities stay registered under their usual names. The logs show no error this mod
causes. Logs: [Kerbin](../../../diag/runs/statics-kerbin-fix.log), [Earth](../../../diag/runs/statics-earth-rss-fix.log).
