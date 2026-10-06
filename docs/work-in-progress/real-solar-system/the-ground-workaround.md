# Real Solar System: the ground workaround

Part of [Terrain Precision Fix](../../../README.md), one point of [Work in progress](../../work-in-progress.md), on [Real Solar System](../../work-in-progress.md#real-solar-system).

**Status: in progress.** With this mod, Real Solar System's ground workaround has nothing left to
correct for this defect, over 75 loads on the Moon and on Earth
([The ground workaround](../../non-regression/real-solar-system/the-ground-workaround.md)). Still open:

- the same six loads on Earth, with the craft in *landed*
  (`reload-earth-rss-landed.sfs`), since in *prelaunch* the workaround does not run and stock's pass runs
  instead;
- the same series without this mod, to compare (on the Moon, with the workaround off, the craft
  tipped over at the very first load);
- on the Moon, the two other moments the workaround runs: coming back to a craft left parked, and
  switching to one.

If this mod and the workaround got in each other's way, a craft would be moved by the workaround
(`Moving Vessel` in `KSP.log`), or jump, at loads where it does not without it.
