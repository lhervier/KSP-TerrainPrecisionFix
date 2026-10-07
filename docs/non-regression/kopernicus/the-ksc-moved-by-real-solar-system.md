# Kopernicus: the KSC moved by Real Solar System

Part of [Terrain Precision Fix](../../../README.md), one test of [Non-regression tests](../../non-regression.md), on [Kopernicus](../../non-regression.md#kopernicus).

**Status: checked, no problem — the KSC is taken out of its sphere where Kopernicus puts it, and put
back.** Kopernicus can move the KSC to another place on the home body. Real Solar System has it
do so, to Cape Canaveral, with a `SpaceCenter` node in its configuration of Earth. Kopernicus does it
through the KSC's own `PQSCity`, before any flight, and this mod places a static from where its
`PQSCity` says it is.

## Checked on Earth

With Kopernicus 248, in the Earth session of [Scene changes](../stock/scene-changes.md): a craft on the
launchpad at Cape Canaveral, through every way of leaving the flight and coming back, a trip to the Moon
included, logged in
[`diag/runs/scene-changes-earth-rss-fix.log`](../../../diag/runs/scene-changes-earth-rss-fix.log).

The KSC is taken out of its sphere at Cape Canaveral at each of the twelve times the craft is in flight
near it, corrected each time by a different amount, from 103 to 1 191 mm, and put back under it before
every scene change, and when the second craft reaches the Moon. The launchpad comes back to the same
place every time, within 0.11 mm. The space centre opens each time, and every building in it (the
errors of the logs are in [Scene changes](../stock/scene-changes.md#the-result)), although Kopernicus
looks the KSC up under the home body's terrain sphere when it opens (see
[The flag fix](../../limits-and-solutions/kopernicus/the-flag-fix.md#where-kopernicus-looks-for-the-ksc)).
