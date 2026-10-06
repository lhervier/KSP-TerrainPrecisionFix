# Kopernicus: the KSC moved by Real Solar System

Part of [Terrain Precision Fix](../../../README.md), one test of [Non-regression tests](../../non-regression.md), on [Kopernicus](../../non-regression.md#kopernicus).

**Status: checked, no problem — the KSC is taken out of its sphere where Kopernicus puts it, and put back
without error.** Kopernicus can move the KSC to another place on the home body. Real Solar System has it
do so, to Cape Canaveral, with a `SpaceCenter` node in its configuration of Earth. Kopernicus does it
through the KSC's own `PQSCity`, before any flight, and this mod places a static from where its
`PQSCity` says it is.

## Checked on Earth

With Kopernicus 248, in the session described in
[Loading, an orbit and the space centre](../stock/loading-an-orbit-and-the-space-centre.md): two loadings
of `reload-earth-rss-landed.sfs`, a craft 1.4 km from the KSC, then the craft sent to orbit, then the
space centre, logged in [`diag/runs/statics-earth-rss-fix.log`](../../../diag/runs/statics-earth-rss-fix.log).

The KSC is taken out of its sphere at Cape Canaveral, corrected by 361 mm, then 266 mm — within a float
step at that distance from the centre of Earth, 500 mm — and put back under it when the save is loaded
again and when the craft reaches orbit. The space centre then opens without error, although Kopernicus
looks the KSC up under the home body's terrain sphere when it opens (see
[The flag fix](../../limits-and-solutions/kopernicus/the-flag-fix.md#where-kopernicus-looks-for-the-ksc)).
