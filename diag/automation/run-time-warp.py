"""Plays the time warp protocol: a craft on the runway, days of time warp at the highest rate, then a night and a
day at the KSC, to see its lights come on and go off.

It drives KSP through KSP-MCPServer, a mod that answers HTTP requests on 127.0.0.1, and needs nothing but
Python 3: no AI, no package to install. Start KSP with KSP-MCPServer and KSP Diag - Colliders installed, wait
for the main menu, put the craft file in saves/<folder>/Ships/SPH, then run:

    python run-time-warp.py --folder <a new game> --ut 3600 --out out

It starts that game itself from the main menu, in sandbox, then:

1. launches the craft onto the runway, its brakes put on at once;
2. warps on rails at the highest rate, as the . key does, for --days days of game time, then back to normal
   time, as the , key does;
3. warps to the night at the KSC, as Warp To of the map view does, and waits for its lights to come on;
4. warps to the next noon at the KSC, the same way, and waits for them to go off.

At the launch and at the end of each warp, once the craft has settled, it presses the button of KSP Diag -
Colliders that logs the colliders under each craft: the runway's, with its height above the terrain the game
computes there. It takes a screenshot at each of these moments, the camera turned towards the VAB, 1 km to the
south-east, with KSP Diag - Colliders drawing nothing. It writes every reading to readings.json, prints the height
under the craft each time, ends at the space centre, and leaves KSP running (unless --quit is given).
"""
import argparse
import os
import sys
import time

sys.path.insert(0, os.path.join(os.path.dirname(os.path.abspath(__file__)), "scene-changes"))
import common  # noqa: E402
from common import call, close, log  # noqa: E402

# The highest rate on rails with KSP's rates (100000x).
HIGHEST_RATE = 7

# Local times at the KSC, as fractions of the solar day: the day and night switches of the KSC turn its lights on
# before 0.25 and after 0.7, and look at the time every ten seconds.
NIGHT = 0.9
NOON = 0.5
SWITCH_PERIOD = 10.0

OPTIONS = None


def shoot(session, name):
    """Takes a screenshot towards the VAB, the craft beside it, without the colliders KSP Diag - Colliders draws."""
    call("set_camera", distance=OPTIONS.camera_distance, heading=OPTIONS.camera_heading,
         pitch=OPTIONS.camera_pitch, aim_heading=OPTIONS.camera_aim)
    call("colliders_set_display", mode=2)
    session.shoot(name)
    call("colliders_set_display", mode=0)


def warp_highest(days):
    """Warps on rails at the highest rate KSP allows here for <days> days of game time, then stops."""
    state = call("get_state")
    end = state["ut"] + days * state["vessel"]["solarDay"]
    try:
        warp = call("set_warp", rate_index=HIGHEST_RATE)
    except RuntimeError as e:
        warp = None
        log("highest rate refused, warping at the rate allowed: " + str(e))
    if warp is not None:
        log("warping at %.0fx" % warp["rate"])
    while call("get_state")["ut"] < end:
        time.sleep(0.1)
    call("set_warp", rate_index=0)
    log("back to normal time at UT %.0f" % call("get_state")["ut"])


def warp_to_local_time(target):
    """Warps to the next time the local time at the craft is <target>, as Warp To does."""
    vessel = call("get_state")["vessel"]
    seconds = ((target - vessel["localTime"]) % 1.0) * vessel["solarDay"]
    state = call("warp_to", seconds=seconds)
    log("warped %.0f s, to local time %.3f" % (seconds, state["vessel"]["localTime"]))


def main():
    global OPTIONS
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--folder", required=True, help="the new game to start, under saves/")
    parser.add_argument("--craft", default="SPH/Diag3-Rover.craft",
                        help="the craft launched onto the runway, as SPH/<name>.craft (default SPH/Diag3-Rover.craft)")
    parser.add_argument("--days", type=float, default=3.0,
                        help="how long to warp at the highest rate, in solar days of the body (default 3)")
    parser.add_argument("--settle", type=float, default=10.0,
                        help="how long to let the craft settle at each arrival, in seconds")
    parser.add_argument("--ut", type=float, help="the universal time to start at, in seconds, for daylight at the KSC")
    parser.add_argument("--camera-distance", type=float, default=30.0, help="metres behind the craft")
    parser.add_argument("--camera-pitch", type=float, default=5.0, help="degrees")
    parser.add_argument("--camera-heading", type=float, default=95.0,
                        help="degrees the camera looks towards from the runway, the VAB lying at 119")
    parser.add_argument("--camera-aim", type=float, default=12.0,
                        help="degrees the camera aims to the right of the craft, between it and the VAB")
    parser.add_argument("--out", default="out", help="where readings.json and the screenshots go")
    parser.add_argument("--port", type=int, default=8770, help="the port of KSP-MCPServer")
    parser.add_argument("--quit", action="store_true", help="quit KSP at the end")
    parser.add_argument("--keep-running", action="store_true",
                        help="leave KSP running at the end, which it does unless --quit is given")
    options = parser.parse_args()
    common.URL = "http://127.0.0.1:%d/mcp/" % options.port
    OPTIONS = options
    out = os.path.abspath(options.out)
    os.makedirs(out, exist_ok=True)
    session = common.Session(options, out)

    call("new_game", folder=options.folder, mode="SANDBOX")
    close(dialogs_only=True)
    common.at_space_centre()
    if options.ut is not None:
        call("set_time", ut=options.ut)

    call("launch_vessel", craft=options.craft, site="Runway")
    common.brake()
    session.arrived("launched onto the runway", launchpad=False)
    shoot(session, "launched")

    warp_highest(options.days)
    session.arrived("after %g days at the highest rate" % options.days, launchpad=False)
    shoot(session, "after-warp")

    warp_to_local_time(NIGHT)
    call("wait", seconds=SWITCH_PERIOD + 2)
    session.arrived("at night", launchpad=False)
    shoot(session, "night")

    warp_to_local_time(NOON)
    call("wait", seconds=SWITCH_PERIOD + 2)
    session.arrived("at noon", launchpad=False)
    shoot(session, "noon")

    # Quitting KSP from flight logs an exception of its own user interface as it is torn down.
    call("go_to_scene", scene="SPACECENTER")
    common.at_space_centre()
    log("done: %d readings, in %s" % (len(session.readings), out))
    if options.quit:
        call("quit_game")


if __name__ == "__main__":
    main()
