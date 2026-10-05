# Rescaled systems: Real Solar System

Part of [Terrain Precision Fix](../../README.md), one case of [Limits and solutions](../limits-and-solutions.md).

**Status: checked, no problem — this mod breaks nothing with Real Solar System; where Real Solar System
works around the defect, with this mod its workarounds have nothing left to correct for it; the
safeguard grows with the body, checked on five of them.**

The defect grows with the body: a float's step is 62.5 mm at the radius of Kerbin, 125 mm on the Moon,
500 mm on Earth. [Real Solar System](https://github.com/KSP-RO/RealSolarSystem) ships two workarounds for
what it does to a craft: one moves a landed craft back onto the ground when it goes off rails, the other
looks after the runway of the KSC. Each is checked on its own page, with what this mod corrected on the
bodies of Real Solar System. The measurements on the Moon and on Earth, and their install, are in
[Checking the culprit: loading the same save](../checking-the-culprit-loading.md) and
[Checking the culprit: driving on while the world moves](../checking-the-culprit-driving.md); the logs
of every session with this mod in [`diag/runs`](../../diag/README.md#on-real-solar-system). The KSC that
Kopernicus moves to Cape Canaveral for Real Solar System is a point of the case
[Kopernicus](kopernicus.md).

### The ground workaround

**Checked on the Moon and on Earth — with this mod, it has nothing left to correct for this defect.**
Real Solar System moves a landed craft back onto the ground whenever it goes off rails, when it is more
than 10 cm off. It moves the craft, never the ground, so the craft still comes to rest somewhere else at
each load, and under 10 cm it does nothing: without this mod, the craft still jumped or tipped over now
and then, on both bodies. It never runs while a craft is already rolling, where a move of the floating
origin moves the ground under it too. With this mod, the ground comes back within a millimetre: over 75
loads in a row the craft never moved, and the workaround, which still runs, never had anything to
correct.

**→ Full chapter: [The ground workaround](rss/the-ground-workaround.md)**

### The runway fix

**Checked on Earth — with this mod, the runway is where it is drawn, has no step, and does not move
when the floating origin does.** Real Solar System turns off the colliders of the runway's sections,
leaving the one collider of the whole runway, and keeps the floating origin from moving while a craft
rolls on it. Without it, each piece of the runway is rounded on its own, and differently where it is
drawn and where the physics touches it: depending on the load, a craft rests in the air above the deck
or sunk into it, climbs a step between two sections, or rolls past a step it only sees. The runway fix
removes the step a wheel climbs, not the rest. With this mod, and Real Solar System built without its
runway fix, 37 entries in flight, 30 of them reloads just before a step, showed none of these. And a
move of the floating origin, which moves the deck and the grass beside it together by up to 232 mm
without this mod, moves them by less than 0.12 mm with it: the hold has nothing left to correct
either.

**→ Full chapter: [The runway fix](rss/the-runway-fix.md)**

### What this mod corrected

**Checked on the Moon, Earth, Venus, Mars and Mercury — none of the terrain was left uncorrected.** This
mod refuses any correction too large to be a rounding, and that limit grows with the body. On Earth,
where the float step is 500 mm, it corrected the terrain by up to four steps, 2 m, and refused nothing,
on Venus, Mars and Mercury as well.

**→ Full chapter: [What this mod corrected](rss/what-this-mod-corrected.md)**

## Still to test

- **A flight, not only loads.** Everything measured under Real Solar System is a landed craft loaded
  again, never the quads built one after the other while the floating origin moves in flight, which
  every player of Real Solar System goes through at each launch. A launch from Cape Canaveral to orbit,
  and a descent onto the sites of Venus, Mars and Mercury (those of the saves of
  [`diag/`](../../diag/README.md)), flown by MechJeb2 so that they can be played again, with infinite fuel
  (`Alt+F12 → Cheats`) to keep to stock parts; MechJeb2 then joins the install. The log should show no
  exception from this mod and no refusal, the terrain no hole and no offset between quads, and the
  largest correction stays to read; with this mod, then without it, to compare by eye. And, on the
  Moon, the approach protocol of the stock campaigns, which covers the quads built again and the moves
  of the origin on the ground.
- **Without the ground workaround, the rest of the series**: see
  [The ground workaround](rss/the-ground-workaround.md#with-this-mod).
