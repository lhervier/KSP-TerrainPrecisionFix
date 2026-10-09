# Checking the culprit: in flight

Part of [Terrain Precision Fix](../README.md): the measurements that check the first culprit,
[the ground](the-culprit-ground.md), in flight, where the quads of the highest level are built and dropped
all the time as the craft goes, on stock and with this mod, on Kerbin and on Earth in Real Solar System;
and the third, [the scatter](the-culprit-scatter.md), along a flight over the Mun.

[KSP Diag - Terrain Quads](https://github.com/lhervier/KSP-Diag-TerrainQuads) reads the quads, and
[KSP Diag - Floating Origin](https://github.com/lhervier/KSP-Diag-FloatingOrigin), which only reads,
says when the world moves. Each has its own page, with its method. The scatter has an instrument of its
own, described in [The rocks, along a flight](#the-rocks-along-a-flight).

This fix is built on top of [KSP Community Fixes](https://github.com/KSPModdingLibs/KSPCommunityFixes),
the base most players run, so every campaign on this page is run in an install that has it: KSP 1.12.5
with Harmony, ModuleManager, KSP Community Fixes 1.41.1, the two instruments and
[KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer), which flies the craft — and this mod, or not;
on Earth, [Real Solar System](https://github.com/KSP-RO/RealSolarSystem) 20.1.3.0 and what it requires
as well. *On stock*, below, means that install without this mod.

Each quad is rounded once, when it is built, through the frame of the moment
([Why it is different at every load](the-culprit-ground.md#why-it-is-different-at-every-load)). On the
ground, the quads under a craft are built together. In flight, they are built one after the other as the
craft goes, each through the frame of its own moment, while the floating origin moves every 500 m the
craft travels, and every frame once it goes fast. A rocket, `Quad-Rocket`, is launched from the
launchpad and left to fly until it falls into the sea, about a minute later; about once a second, the
*Log* button of Diag TerrainQuads writes, for every quad of the highest level, the distance from the centre
of the body to each of its vertices. Two quads of the highest level that share an edge should put its
vertices at the same distance; the step between them is how far apart they are. *Siblings* are two
quads split from the same quad, at the same moment; *cousins* any other two. It is
[the protocol of the quads of the highest level, in flight](https://github.com/lhervier/KSP-Diag-TerrainQuads/blob/main/docs/the-protocol-flight.md)
of KSP Diag - Terrain Quads, played by its script,
[`run-flight.py`](https://github.com/lhervier/KSP-Diag-TerrainQuads/blob/main/docs/the-protocol-flight.md#played-by-a-script),
and read by its `analyse-flight.py`: on each body, one flight without this mod, one with it, the same
craft and the same steps.

## On Kerbin

From the launchpad of the Space Center; the highest subdivision level of Kerbin is 10.

**On stock**, one flight, 72 *Logs*, up to 957 m and 653 m/s
([the readings](https://github.com/lhervier/KSP-Diag-TerrainQuads/blob/main/docs/the-measurements-flight.md#on-kerbin)).

**With this mod**, in that same install, with this mod as the only difference, at `logLevel = Debug`.
The session is logged in [`flight-kerbin-fix.log`](../diag/checking-the-culprit-flight/flight-kerbin-fix.log); what
the script printed is in [`flight-kerbin-fix-script.txt`](../diag/checking-the-culprit-flight/flight-kerbin-fix-script.txt), what
each *Log* answered in [`flight-kerbin-fix-readings.json`](../diag/checking-the-culprit-flight/flight-kerbin-fix-readings.json),
the two files of the *Logs* in [`flight-kerbin-fix-logs.csv`](../diag/checking-the-culprit-flight/flight-kerbin-fix-logs.csv) and
[`flight-kerbin-fix-quads.zip`](../diag/checking-the-culprit-flight/flight-kerbin-fix-quads.zip), and what `analyse-flight.py`
printed in [`flight-kerbin-fix-analysis.txt`](../diag/checking-the-culprit-flight/flight-kerbin-fix-analysis.txt). 73 *Logs*:
the same flight to within a few metres, up to 954 m and 654 m/s, the floating origin moved 2,361 times,
every frame from about 236 m/s on.

For the quads of level 10, in millimetres, the median, the 90th percentile and the largest value:

| | on stock | with this mod |
|---|---|---|
| the same quad, from one *Log* to the next, over 100 m/s | 0.050 / 0.226 / 0.699 | 0.050 / 0.238 / 0.855 |
| the step between siblings | 0.007 / 0.033 / 0.418 | 0.061 / 0.220 / 0.783 |
| the step between cousins both there from the first *Log*, built as the scene opened | 0.006 / 0.027 / 0.403 | 0.062 / 0.231 / 0.774 |
| **the step between cousins, one at least built during the flight** | **4.520 / 16.930 / 22.454** | **0.060 / 0.202 / 0.674** |

Each line compares 4,023 to 11,451 pairs of *Logs* or shared edges, about as many on stock as with this
mod; their counts are in the two `analysis.txt` files. Under 100 m/s, the first eleven *Logs*, nothing
moved either way, to the thousandth of a millimetre: the floating origin did not move in that time.

The log shows this mod placing 1,000 quads in the session, 220 of them during the flight, corrected by
34 mm in the median and by 93.9 mm at most: 1.5 steps of a float at the radius of Kerbin, against the
sixteen past which this mod refuses a correction. It refused none, and the log shows no error this mod
causes.

## On Earth

From the launchpad of Cape Canaveral; the highest subdivision level of Earth is 11. The same craft flies
a lower and shorter curve there than on Kerbin.

**On stock**, one flight, 55 *Logs*, up to 565 m and 488 m/s
([the readings](https://github.com/lhervier/KSP-Diag-TerrainQuads/blob/main/docs/the-measurements-flight.md#on-earth-in-real-solar-system)).

**With this mod**, in that same install, with this mod as the only difference, at `logLevel = Debug`.
The session is logged in [`flight-earth-rss-fix.log`](../diag/checking-the-culprit-flight/flight-earth-rss-fix.log);
what the script printed is in [`flight-earth-rss-fix-script.txt`](../diag/checking-the-culprit-flight/flight-earth-rss-fix-script.txt),
what each *Log* answered in [`flight-earth-rss-fix-readings.json`](../diag/checking-the-culprit-flight/flight-earth-rss-fix-readings.json),
the two files of the *Logs* in [`flight-earth-rss-fix-logs.csv`](../diag/checking-the-culprit-flight/flight-earth-rss-fix-logs.csv)
and [`flight-earth-rss-fix-quads.zip`](../diag/checking-the-culprit-flight/flight-earth-rss-fix-quads.zip), and what
`analyse-flight.py` printed in [`flight-earth-rss-fix-analysis.txt`](../diag/checking-the-culprit-flight/flight-earth-rss-fix-analysis.txt).
56 *Logs*, up to 613 m and 503 m/s, the floating origin moved 1,492 times, every frame from about
236 m/s on: on Earth, the two flights differ by some fifty metres at the top of their curve.

For the quads of level 11, in millimetres, the median, the 90th percentile and the largest value:

| | on stock | with this mod |
|---|---|---|
| the same quad, from one *Log* to the next, over 100 m/s | 0.160 / 0.740 / 2.172 | 0.145 / 0.755 / 2.218 |
| the step between siblings | 0.137 / 0.286 / 1.376 | 0.312 / 0.884 / 2.458 |
| the step between cousins both there from the first *Log*, built as the scene opened | 0.132 / 0.331 / 1.592 | 0.282 / 0.807 / 2.124 |
| **the step between cousins, one at least built during the flight** | **57.458 / 264.204 / 288.716** | **0.381 / 0.827 / 2.137** |

Each line compares 778 to 10,405 pairs of *Logs* or shared edges, about as many on stock as with this
mod. The quads of Earth are larger than those of Kerbin, and the flight shorter: far fewer quads are
built on the way, some 800 shared edges between cousins built in flight, against 4,000 on Kerbin.
Under 100 m/s, the first eleven *Logs*, nothing moved either way: the floating origin did not move in
that time.

The log shows this mod placing 272 quads in the session, 48 of them during the flight, corrected by
585 mm in the median and by 1,093.7 mm at most: 2.2 steps of a float at the radius of Earth, against the
sixteen past which this mod refuses a correction. It refused none, and the log shows no error this mod
causes.

## The rocks, along a flight

[KSP Diag - Scatter](https://github.com/lhervier/KSP-Diag-Scatter) measures the terrain scatter against
the ground: for every quad carrying scatter, the height of its holders and of the matrices they are drawn
with, and for the objects of the quad nearest to the craft, the height of their vertices above the ground.
After a load, it is read in
[Checking the culprit: loading the same save](checking-the-culprit-loading.md#the-scatter-over-twelve-loads).
In flight, the world origin follows the craft, and the terrain keeps building quads ahead of it and
destroying those behind it: [the scatter fix](the-fix-scatter.md) leaves each holder to follow its quad
through all of that, without doing anything more.

The scatter fix is off by default, so the flight is measured with the terrain fix alone — this mod as
installed by default — and with the scatter fix as well. **The flight with the scatter fix was taken when
it was a mod of its own**, Rock Precision Fix 0.1.0, installed next to Terrain Precision Fix 0.1.0: the
same two patches as this mod's scatter fix, whose lines in its log are tagged `[RockPrecisionFix]`.

The instrument reads the rocks along a flight, following
[its protocol](https://github.com/lhervier/KSP-Diag-Scatter/blob/main/docs/measuring-the-rocks.md#over-a-flight),
in KSP 1.12.5 with Harmony, ModuleManager and KSP Community Fixes 1.41.1:
the save
[`ref-mune-5km.sfs`](https://github.com/lhervier/KSP-Diag-Scatter/blob/main/diag/README.md#the-saves),
a Mk1 command pod in a circular equatorial orbit 5 km over the Mun, loaded once, one record 30 s into the
flight, then every two minutes, and one after the pod crashed into the relief. The records with both
fixes are in [`diag/runs`](../diag/README.md#the-scatter-fix-the-rocks-over-a-flight), and those with the
terrain fix alone with
[the instrument](https://github.com/lhervier/KSP-Diag-Scatter/blob/main/diag/README.md#the-rocks-over-a-flight).

Flown the same way, both flights passed over the same ground: every record names the same quads as its
counterpart in the other flight, 1,600 in all, and the same nearest quad, whose 20 rocks and 200 vertices
compare one by one. The pod is 5 km up, so every quad is at least that far from the world origin, where
single precision coordinates step by about half a millimetre: the same quads' centres, and the ground under
the same vertices, come out up to 0.7 mm and 1.6 mm apart from one flight to the other.

| record | nearest quad | *up* of its holder, the terrain fix alone | its vertices above the ground, the terrain fix alone minus both fixes | *up* of all the holders, both fixes |
|---|---|---|---|---|
| 1 | `Mun Zp200000011` | +0.8 mm | +0.5 to +1.0 mm | 0.000 mm, 344 holders |
| 2 | `Mun Xn231111111` | +17.5 mm | +17.3 to +18.1 mm | 0.000 mm, 168 holders |
| 3 | `Mun Xn211311311` | −3.5 mm | −3.5 to −2.0 mm | 0.000 mm, 144 holders |
| 4 | `Mun Xn122020000` | +1.5 mm | +1.2 to +1.7 mm | 0.000 mm, 224 holders |
| 5 | `Mun Xn013331113` | −1.5 mm | −1.9 to −0.5 mm | 0.000 mm, 152 holders |
| 6, after the crash | `Mun Zn200000011` | −15.8 mm | −16.9 to −16.3 mm | 0.000 mm, 568 holders |

**With the terrain fix alone**, no holder is drawn on its quad, at any record: over the 1,600, the
*up* of their matrices runs from −26.5 to +21.1 mm, never 0, 9.0 mm on average (root mean square), about
what the twelve loads of the Mun gave. The offset does not grow along the flight, but it does not go away
either.

**With both fixes**, at every record, every holder, 1,600 in all, stands at the height of its quad's
centre, to the micrometre, and its matrix too, neither shifted from it, up or across: 0.000 mm. Those
built minutes into the flight and those still there after the crash alike.

On the nearest quad, every vertex stands higher or lower against the ground with the terrain fix alone
than with both fixes, by nearly the same amount for the 200 of them, and that amount is the *up* of their
holder in the first flight: taking it off leaves 0.2 mm (median over the vertices), 1.5 mm at most,
within the precision of the reading at that distance.

## What the measurements say

**On stock, in flight, the ground is not one surface.** Two quads of the highest level built at different
moments of the flight step up or down where they meet: by 4.5 mm in the median and up to 22.5 mm on
Kerbin; by 57 mm in the median and up to 289 mm on Earth, where a float's step is eight times larger.
Two quads built at the same moment meet within 0.42 mm on Kerbin and 1.6 mm on Earth. While the craft
flies faster than 100 m/s, each quad keeps the height it was built at — from one *Log* to the next, a
vertex moved by 0.70 mm at most on Kerbin and 2.2 mm on Earth, through thousands of moves of the floating
origin — so the step stays as long as the two quads do.

**With this mod, the steps are gone.** Cousins built during the flight meet within 0.674 mm on Kerbin
and 2.137 mm on Earth, no worse than those built together or than siblings on the same body: when a
quad was built no longer matters. What is left is about the same for every pair, under a millimetre on
Kerbin and a few millimetres on Earth. Siblings meet a little less closely than on stock — in the median,
0.061 mm against 0.007 mm on Kerbin, 0.312 mm against 0.137 mm on Earth: on stock, two siblings carry
the same rounding, and with this mod each is placed on its own. Where those remainders come from has
not been checked on its own.

**These flights do not say what a move of the origin does at low speed.** Stock places the quads again
after a move only when the active craft is landed or slower than 100 m/s (`FloatingOrigin.FixedUpdate`), and
the floating origin did not move while these craft were that slow; a rover driving on the ground covers
that case ([Checking the culprit: driving on while the world moves](checking-the-culprit-driving.md)).
Nor do they say anything of the statics.

**Two bodies are enough.** Kerbin and Earth are the two ends of what a player lands on: a float's step of
62.5 mm and of 500 mm at the radius of the body, a stock body and one made by Kopernicus. The code that
places a quad is the same on every body, and the steps grow with the float's step, so no other body was
flown.

**The scatter follows its quads along a flight with the scatter fix, and only with it.** With the
terrain fix alone, no holder is drawn on its quad at any record, by up to 26.5 mm over the Mun, and the
objects move with their holder and only with it. With the scatter fix as well, every holder of the flight
is drawn exactly on its quad, those built minutes into the flight and those left after the crash alike:
along a flight, the scatter is drawn on its quads just as after a load.
