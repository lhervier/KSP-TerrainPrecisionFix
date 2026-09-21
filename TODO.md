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
1. **The ground anchor, in an install a reviewer could rebuild.** The comment posted on #214 quotes
   its readings, so every one of them has to be reproducible by someone else. They are not, yet: they come from the
   author's own years-old save, in a game that also has EvaCMGroundPlugin installed. Redo them plainly,
   on KSP + Harmony + ModuleManager + KSPCF + Diag 1, with the fix for the second half. What has to come
   out of it: the `Moving Vessel` values without the fix (both signs, a different one every load) and
   with it (the same value at every load); a `.sfs` showing `PQSMin`/`PQSMax` at `0/0` on a
   freshly placed anchor; and the 2.08 cm collider gap re-read from `groundAnchor.mu`. ⚠️ **The protocol
   differs from every other campaign here: save after each load.** It is the re-save that arms the
   ratchet — without it the anchor is put back to the same place every time and the climb never appears.
2. **Kopernicus, twice.** Most planet packs go through it, so this is the compatibility question a
   reviewer asks first. Two runs are missing, not one:
   - **a clean stock-body series with the terrain instruments.** The reading that exists came out of a
     scatter campaign, in an install built for scatter, so its protocol cannot be described without
     describing that campaign — which means the figure cannot be quoted anywhere that does not also
     tell that story. Re-run it plainly: Kopernicus on Kerbin, Diag 1 and Diag 2, six loads, with and
     without the fix. Cheap, and it turns a figure that has to be explained away into one that can
     simply be given;
   - **a body a planet pack creates or reconfigures**, rather than merely loads. Nothing has been
     measured there at all.
3. **Existing saves.** The transition a player feels on a save made before the fix, written in the
   README rather than discovered in play.
4. **Breaking Ground surface features.** They have colliders and are placed like the rocks. Enough to
   know whether the fix introduces a physical offset there, even if the answer is "yes, and it needs a
   fix of its own".
5. **The same campaign on a slope.** Every campaign so far is on flat ground, because Terrain Precision
   Fix Diag 1 asks for it. On a slope a single-part craft is put into the ground at every load by a
   stock bug this fix does not touch, and a reviewer who meets it there will read it as "the fix does
   not work".

Everything else below can be listed as open in the README without holding the issue back.

## Every case the fix has to be checked against

Each case, with what is known and what is planned to test it, is a chapter of
[Limits and solutions](docs/limits-and-solutions.md) — the public page the KSPCF issue links to. That
page is the test plan: add a case there, as a chapter with **Status: TBD**, not here, and add its line
to the table of the KSPCF comment. What stays below is only what does not belong on a public page.

## Existing saves — what is left to write

The one campaign run on a real save (the author's own years-old save, loaded base by base with KSP,
Harmony, KSPCF and this fix only) is not written up. Its public side is the chapter
[Existing saves](docs/limits-and-solutions.md#existing-saves); left to decide or do here:

- **the result of that run**: the bases on the Mun, Minmus and Gilly loaded one by one and none broke;
  the base on Eve settles onto feet built below the surface, on loading and on leaving time warp alike,
  which is a construction defect and not this fix. The save does not sample the worst case: its bases
  are anchored on girder rails, docked and strutted, where the configuration that suffers most is
  docked modules standing on landing legs. One screenshot, to show the scale of what was loaded, still
  has to be added; it proves nothing on its own;
- **where the docked-assemblies and landing-legs caveat goes.** The README's `Disclaimer` is copied word
  for word into the two Diag READMEs, so adding to it means editing all three; the alternative is the
  correcting mod's own closing paragraph, which is allowed to differ;
- **one sentence under `How this was made`** saying those bases were assembled in EVA construction with
  a personal tool (KSP-EvaCMGroundPlugin), that they load and work without it, and nothing more;
- **KSPCF patches are individually switchable**: confirm it against the KSPCF settings mechanism before
  writing, in that chapter, that a player can turn this one off, load, raise a craft, and turn it back on;
- **only if the bound shows a real cost**: a migration helper that walks a `.sfs` and raises landed
  vessels onto the corrected ground. A separate tool, not a prerequisite.

## The cost of the fix on Kerbin

The performance campaign was flown over the Mun. The same measurement on Kerbin, where quads are four
times larger and a craft can be made to fly low for much longer, is worth having. Not a prerequisite.
