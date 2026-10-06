# Kerbal Konstructs: a section of the runway at the first loading

Part of [Terrain Precision Fix](../../../README.md), one point of [Work in progress](../../work-in-progress.md), on [Kerbal Konstructs](../../work-in-progress.md#kerbal-konstructs).

**Status: planned — seen, not explained.** The statics of [Kerbal Konstructs](https://github.com/KSP-RO/Kerbal-Konstructs)
hang from a stock `PQSCity` and carry the same defect as the KSC's
([The culprit: the statics](../../the-culprit-statics.md)). On a runway it placed on the Mun, this mod
brings the deck back within 0.015 mm from the second loading to the sixth, instead of 17.7 mm on stock,
and the ground beside it within 0.034 mm. At the first loading of a session, a section of the runway
21.3 mm above the deck is still active under the craft. It is not a rounding, and this mod does not touch
it. The readings: [Checking the culprit: loading the same save](../../checking-the-culprit-loading.md).

*To test:* why that section of the runway is only there at the first loading of a session — whether
Kerbal Konstructs turns it off afterwards, and whether stock shows it too.
