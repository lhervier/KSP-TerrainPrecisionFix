# Checking the culprit: in flight

Part of [Terrain Precision Fix](../README.md): the measurements that check the first culprit,
[the ground](the-culprit-ground.md), in flight, where the quads of the highest level are built and dropped
all the time as the craft goes, on stock and with this mod, on Kerbin.

[KSP Diag - Quad Seams](https://github.com/lhervier/KSP-Diag-QuadSeams) reads the quads, and
[KSP Diag - Floating Origin](https://github.com/lhervier/KSP-Diag-FloatingOrigin), which only reads,
says when the world moves. Each has its own page, with its method.

This fix is built on top of [KSP Community Fixes](https://github.com/KSPModdingLibs/KSPCommunityFixes),
the base most players run, so every campaign on this page is run in an install that has it: KSP 1.12.5
with Harmony, ModuleManager, KSP Community Fixes 1.41.1, the two instruments and
[KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer), which flies the craft — and this mod, or not.
*On stock*, below, means that install without this mod.

Each quad is rounded once, when it is built, through the frame of the moment
([Why it is different at every load](the-culprit-ground.md#why-it-is-different-at-every-load)). On the
ground, the quads under a craft are built together. In flight, they are built one after the other as the
craft goes, each through the frame of its own moment, while the floating origin moves every 500 m the
craft travels, and every frame once it goes fast. A rocket, `Quad-Rocket`, is launched from the
launchpad of the Space Center and left to fly until it falls into the sea, about 70 seconds later, at up
to about 950 m and 650 m/s; about once a second, the *Log* button of Diag QuadSeams writes, for every
quad of level 10, the highest on Kerbin, the distance from the centre of Kerbin to each of its vertices.
Two quads of level 10 that share an edge should put its vertices at the same distance; the step between
them is how far apart they are. *Siblings* are two quads split from the same quad, at the same moment;
*cousins* any other two. It is
[the protocol of the quads of the highest level, in flight](https://github.com/lhervier/KSP-Diag-QuadSeams/blob/master/docs/the-protocol-flight.md)
of KSP Diag - Quad Seams, played by its script,
[`run-flight.py`](https://github.com/lhervier/KSP-Diag-QuadSeams/blob/master/docs/the-protocol-flight.md#played-by-a-script),
and read by its `analyse-flight.py`: one flight without this mod, one with it.

## The quads of level 10, over one flight

**On stock**, one flight, 72 *Logs*
([the readings](https://github.com/lhervier/KSP-Diag-QuadSeams/blob/master/docs/the-measurements-flight.md)).

**With this mod**, in that same install, with this mod as the only difference, at `logLevel = Debug`.
The session is logged in [`diag/runs/flight-kerbin-fix.log`](../diag/runs/flight-kerbin-fix.log); what
the script printed is in [`flight-kerbin-fix-script.txt`](../diag/runs/flight-kerbin-fix-script.txt), what
each *Log* answered in [`flight-kerbin-fix-readings.json`](../diag/runs/flight-kerbin-fix-readings.json),
the two files of the *Logs* in [`flight-kerbin-fix-logs.csv`](../diag/runs/flight-kerbin-fix-logs.csv) and
[`flight-kerbin-fix-quads.zip`](../diag/runs/flight-kerbin-fix-quads.zip), and what `analyse-flight.py`
printed in [`flight-kerbin-fix-analysis.txt`](../diag/runs/flight-kerbin-fix-analysis.txt). 73 *Logs*:
the same flight to within a few metres, up to 954 m and 654 m/s, the floating origin moved 2,361 times,
every frame from about 236 m/s on.

In millimetres, the median, the 90th percentile and the largest value:

| | on stock | with this mod |
|---|---|---|
| the same quad, from one *Log* to the next, over 100 m/s | 0.050 / 0.226 / 0.699 | 0.050 / 0.238 / 0.855 |
| the step between siblings | 0.007 / 0.033 / 0.418 | 0.061 / 0.220 / 0.783 |
| the step between cousins both there from the first *Log*, built as the scene opened | 0.006 / 0.027 / 0.403 | 0.062 / 0.231 / 0.774 |
| **the step between cousins, one at least built during the flight** | **4.520 / 16.930 / 22.454** | **0.060 / 0.202 / 0.674** |

Each line compares 4,023 to 11,451 pairs of *Logs* or shared edges, about as many on stock as with this mod; their
counts are in the two `analysis.txt` files. Under 100 m/s, the first eleven *Logs*, nothing moved
either way, to the thousandth of a millimetre: the floating origin did not move in that time.

The log shows this mod placing 1,000 quads in the session, 220 of them during the flight, corrected by
34 mm in the median and by 93.9 mm at most: 1.5 steps of a float at the radius of Kerbin, against the
sixteen past which this mod refuses a correction. It refused none, and the log shows no error this mod
causes.

## What the measurements say

**On stock, in flight, the ground is not one surface.** Two quads of level 10 built at different
moments of the flight step up or down where they meet: by 4.5 mm in the median, by 16.9 mm or less nine
times out of ten, by up to 22.5 mm on Kerbin. Two quads built at the same moment meet within 0.42 mm.
While the craft flies faster than 100 m/s, each quad keeps the height it was built at — from one *Log*
to the next, a vertex moved by 0.70 mm at most, through 2,365 moves of the floating origin — so the step
stays as long as the two quads do.

**With this mod, the steps are gone.** Cousins built during the flight meet within 0.674 mm, no worse
than those built together (0.774 mm) or than siblings (0.783 mm): when a quad was built no longer
matters. What is left is under a millimetre, about the same for every pair. Siblings meet a little less
closely than on stock, 0.061 mm in the median against 0.007 mm: on stock, two siblings carry the same
rounding, and with this mod each is placed on its own. Where those few hundredths of a millimetre come
from has not been checked on its own.

**This flight does not say what a move of the origin does at low speed.** Stock places the quads again
after a move only when the active craft is landed or slower than 100 m/s (`FloatingOrigin.cs:423`), and
the floating origin did not move while this craft was that slow; a rover driving on the ground covers
that case ([Checking the culprit: driving on while the world moves](checking-the-culprit-driving.md)).
Nor does this flight say anything of the statics, nor of another body: Kerbin only, where a float's step
at the radius of the body is 62.5 mm. The same flight on the Mun, from a launchpad placed by Kerbal
Konstructs, is still to be flown; on Earth and the Moon of Real Solar System, see the
[Still to test](limits-and-solutions/rescaled-systems-real-solar-system.md#still-to-test) of that case.
