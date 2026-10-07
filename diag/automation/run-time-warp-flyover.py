"""Plays the time warp flyover protocol: a craft flies low over a static of the Mun in time warp on rails, where KSP
moves the floating origin at every frame, then the craft landed on that static is read again.

It drives KSP through KSP-MCPServer, a mod that answers HTTP requests on 127.0.0.1, and needs nothing but
Python 3: no AI, no package to install. Start KSP with KSP-MCPServer, KSP Diag - Colliders and Kerbal Konstructs
with the launch pad of warp-static-mun-kk/ installed, put warp-static-mun-kk.sfs and the rover's craft file in
the game, wait for the main menu, then run:

    python run-time-warp-flyover.py --folder <the game> --save warp-static-mun-kk --out out

It loads the save, a capsule landed on a launch pad Kerbal Konstructs placed near the highest point of the
Mun's equator, reads the colliders under it with KSP Diag - Colliders, leaves for the space centre and launches
the rover onto the runway. Then, for each pass:

1. it puts the rover with Set Orbit on a circular orbit of the Mun, --altitude metres up, whose southernmost
   point is over the capsule, --lead metres before it (from the second pass, after switching to the rover);
2. time warps on rails at --rate, as the . key does, until the rover has passed over the capsule and is
   farther than --away metres from it, then back to normal time;
3. switches to the capsule, as Switch To of the map view does, and reads the colliders under it again.

It prints the closest the rover came to the capsule, writes every reading to readings.json, ends at the space
centre, and leaves KSP running (unless --quit is given).
"""
import argparse
import json
import math
import os
import sys
import time

sys.path.insert(0, os.path.join(os.path.dirname(os.path.abspath(__file__)), "scene-changes"))
import common  # noqa: E402
from common import call, log  # noqa: E402

# Where this mod takes a static out of its sphere, around the active craft, in metres (27.5 km by default).
REACH = 27500.0


def vessel(vessel_id):
    """The vessel of that id, as list_vessels gives it."""
    return next(v for v in call("list_vessels") if v["id"] == vessel_id)


def put_on_orbit(options, capsule):
    """Puts the active craft on a circular orbit of the Mun whose southernmost point is over the capsule,
    options.lead metres before it."""
    sma = 200000.0 + options.altitude
    inclination = abs(capsule["latitude"])
    # Where the northernmost point falls with the ascending node at 0; the southernmost one is opposite.
    north = call("set_orbit", body="Mun", sma=sma, inclination=inclination, lan=0, argument_of_periapsis=90,
                 mean_anomaly=0)["vessel"]
    lan = (capsule["longitude"] - (north["longitude"] + 180.0) + 540.0) % 360.0 - 180.0
    state = call("set_orbit", body="Mun", sma=sma, inclination=inclination, lan=lan, argument_of_periapsis=270,
                 mean_anomaly=-options.lead / sma)
    log("on orbit: %.0f m up, inclination %.4f, lan %.4f" % (options.altitude, inclination, lan))
    return state


def fly_over(options, capsule_id):
    """Time warps on rails until the active craft has passed over the capsule and gone farther than options.away,
    then stops. Returns the closest it came."""
    warp = call("set_warp", rate_index=options.rate)
    log("warping at %.0fx" % warp["rate"])
    closest = math.inf
    start = time.time()
    while time.time() - start < options.timeout:
        distance = vessel(capsule_id)["distance"]
        closest = min(closest, distance)
        if closest < REACH and distance > options.away:
            break
        time.sleep(0.1)
    call("set_warp", rate_index=0)
    log("back to normal time, closest to the capsule %.0f m" % closest)
    if closest >= REACH:
        raise RuntimeError("the rover never came within %.0f m of the capsule" % REACH)
    return closest


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--folder", required=True, help="the game, under saves/")
    parser.add_argument("--save", default="warp-static-mun-kk", help="the save of the capsule on the launch pad")
    parser.add_argument("--craft", default="SPH/Diag3-Rover.craft",
                        help="the craft flown over the capsule, launched onto the runway, as SPH/<name>.craft "
                             "(default SPH/Diag3-Rover.craft)")
    parser.add_argument("--passes", type=int, default=1, help="how many passes (default 1)")
    parser.add_argument("--altitude", type=float, default=9000.0,
                        help="the altitude of the orbit in metres, above the 8 300 m of the Mun's highest relief "
                             "plus the 250 m KSP asks for before it allows every rate (default 9000)")
    parser.add_argument("--rate", type=int, default=2, help="the rate index on rails (default 2: 10x)")
    parser.add_argument("--lead", type=float, default=60000.0,
                        help="how far before the capsule the orbit starts, in metres (default 60000)")
    parser.add_argument("--away", type=float, default=40000.0,
                        help="how far past the capsule the time warp stops, in metres (default 40000)")
    parser.add_argument("--timeout", type=float, default=300.0, help="the longest a pass may last, in seconds")
    parser.add_argument("--settle", type=float, default=10.0,
                        help="how long to let the capsule settle at each reading, in seconds")
    parser.add_argument("--out", default="out", help="where readings.json goes")
    parser.add_argument("--port", type=int, default=8770, help="the port of KSP-MCPServer")
    parser.add_argument("--quit", action="store_true", help="quit KSP at the end")
    parser.add_argument("--keep-running", action="store_true",
                        help="leave KSP running at the end, which it does unless --quit is given")
    options = parser.parse_args()
    common.URL = "http://127.0.0.1:%d/mcp/" % options.port
    out = os.path.abspath(options.out)
    os.makedirs(out, exist_ok=True)
    session = common.Session(options, out)

    call("load_save", folder=options.folder, save=options.save)
    session.arrived("loaded", launchpad=False)
    capsule = call("get_state")["vessel"]
    passes = []
    call("go_to_scene", scene="SPACECENTER")
    common.at_space_centre()
    # Not from the launchpad: KSP clears it before a launch, recovering whatever stands on a launch pad, the
    # capsule on the launch pad of Kerbal Konstructs included.
    call("launch_vessel", craft=options.craft, site="Runway")
    rover = call("get_state")["vessel"]["id"]
    for n in range(1, options.passes + 1):
        if n > 1:
            call("switch_vessel", vessel=rover)
        put_on_orbit(options, capsule)
        closest = fly_over(options, capsule["id"])
        passes.append({"pass": n, "closest": closest})
        with open(os.path.join(out, "passes.json"), "w", newline="") as f:
            json.dump(passes, f, indent=1)
        call("switch_vessel", vessel=capsule["id"])
        session.arrived("after pass %d" % n, launchpad=False)

    # Quitting KSP from flight logs an exception of its own user interface as it is torn down.
    call("go_to_scene", scene="SPACECENTER")
    common.at_space_centre()
    log("done: %d passes, %d readings, in %s" % (len(passes), len(session.readings), out))
    if options.quit:
        call("quit_game")


if __name__ == "__main__":
    main()
