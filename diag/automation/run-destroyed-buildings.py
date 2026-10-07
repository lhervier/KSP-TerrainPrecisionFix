"""Plays the destroyed buildings protocol: the VAB brought down by a heavy craft dropped onto it in flight, then repaired.

It drives KSP through KSP-MCPServer, a mod that answers HTTP requests on 127.0.0.1, and needs nothing but
Python 3: no AI, no package to install. Start KSP with KSP-MCPServer and KSP Diag - Colliders installed, wait
for the main menu, put the craft files in saves/<folder>/Ships/VAB and Ships/SPH, then run:

    python run-destroyed-buildings.py --folder <a new game> --out out

It starts that career itself from the main menu, set up as its player would to build the crafts and bring a
building down (see new_career in scene-changes/common.py), then:

1. the menu of the Vehicle Assembly Building, opened for a screenshot: no repairs needed; the launchpad upgraded
   to its second level, the first that takes the 54 t of VAB-Dropper;
2. the VAB entered through its building, and VAB-Dropper launched onto the launchpad: a probe on three full
   Rockomax X200-32 tanks. It is moved 350 m above the roof of the VAB with Set Position of the Alt+F12 menu, and
   falls onto it. As soon as it has hit, Revert to Launch, the VAB collapsing;
3. the same drop, the VAB left to collapse; then the space centre, and the menu of the VAB opened for a
   screenshot: its damage and the cost of its repairs;
4. the VAB being out of use, the Space Plane Hangar entered through its building, and a rover launched from it
   onto the runway, its brakes put on at once; then a quicksave (F5) loaded back (F9); then Recover;
5. the VAB repaired from its menu, as its Repair button does, its menu opened again for a screenshot, and a
   rocket launched from the VAB, entered through its building, onto the launchpad.

At every arrival in flight, once the craft has settled, it presses the button of KSP Diag - Colliders that logs
the colliders under each craft, and it takes a screenshot towards the VAB from the launchpad or the runway. It
writes every reading to readings.json and leaves KSP running (unless --quit is given).
"""
import argparse
import json
import os
import sys
import time

sys.path.insert(0, os.path.join(os.path.dirname(os.path.abspath(__file__)), "scene-changes"))
import common  # noqa: E402
from common import call, close, log  # noqa: E402

QUICKSAVE = "destroyed-buildings-quicksave"


def towards_vab(session, name, runway=False):
    """Takes a screenshot towards the VAB, the craft beside it: from the launchpad, the VAB 650 m due west, or
    from the runway, the VAB 1 km to the south-east."""
    options = session.options
    heading = options.runway_camera_heading if runway else options.camera_heading
    call("set_camera", distance=options.camera_distance, heading=heading, pitch=options.camera_pitch,
         aim_heading=options.camera_aim)
    session.shoot(name)


def drop(session):
    """Moves the active craft above the roof of the VAB, as Set Position does, and waits until it has hit."""
    options = session.options
    craft = call("get_state")["vessel"]["id"]
    try:
        call("set_position", latitude=options.drop_latitude, longitude=options.drop_longitude,
             altitude=options.drop_height, timeout=1)
    except RuntimeError as e:
        # Set Position lets the craft go at once; the tool waits for it to settle, which it does not do up there.
        if "had not settled" not in str(e):
            raise
    start = time.time()
    previous = None
    while time.time() - start < options.fall_timeout:
        vessel = call("get_state").get("vessel")
        if vessel is None or vessel.get("id") != craft or vessel["situation"] == "LANDED":
            break
        # Once every part of it has exploded, the craft stays the active vessel, frozen where it hit.
        if previous is not None and vessel["altitude"] == previous:
            break
        previous = vessel["altitude"]
        call("wait", seconds=0.2)
    else:
        raise RuntimeError("the craft was still falling after %.0f s" % options.fall_timeout)
    log("the craft has hit after %.1f s of fall" % (time.time() - start))


def vab_menu(session, name):
    """Opens the menu of the VAB for a screenshot, logs what it reads, and closes it."""
    state = call("facility_menu", facility="VAB")
    log("VAB menu: %s" % json.dumps(state))
    session.shoot(name)
    close(dialogs_only=False)


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--folder", required=True, help="the new game to start, under saves/")
    parser.add_argument("--dropper", default="VAB/VAB-Dropper.craft",
                        help="the heavy craft dropped onto the VAB, as VAB/<name>.craft (default VAB/VAB-Dropper.craft)")
    parser.add_argument("--craft", default="VAB/Diag3-Rocket.craft",
                        help="the rocket launched from the repaired VAB (default VAB/Diag3-Rocket.craft)")
    parser.add_argument("--rover", default="SPH/Diag3-Rover.craft",
                        help="the rover launched onto the runway around the destroyed VAB (default "
                             "SPH/Diag3-Rover.craft)")
    parser.add_argument("--drop-latitude", type=float, default=-0.0964,
                        help="above the middle of the roof of the VAB, degrees")
    parser.add_argument("--drop-longitude", type=float, default=-74.6237,
                        help="above the middle of the roof of the VAB, degrees")
    parser.add_argument("--drop-height", type=float, default=350.0,
                        help="metres of the craft above the terrain when it is let go (default 350)")
    parser.add_argument("--fall-timeout", type=float, default=30.0,
                        help="how long the craft may fall before the protocol gives up, in seconds")
    parser.add_argument("--collapse", type=float, default=20.0,
                        help="how long to let the VAB collapse after the second drop, in seconds")
    parser.add_argument("--settle", type=float, default=10.0,
                        help="how long to let a craft settle on the launchpad at each arrival, in seconds")
    parser.add_argument("--camera-distance", type=float, default=30.0, help="metres behind the craft")
    parser.add_argument("--camera-pitch", type=float, default=5.0, help="degrees")
    parser.add_argument("--camera-heading", type=float, default=245.0,
                        help="degrees the camera looks towards from the launchpad, the VAB lying at 268")
    parser.add_argument("--runway-camera-heading", type=float, default=95.0,
                        help="degrees the camera looks towards from the runway, the VAB lying at 119")
    parser.add_argument("--camera-aim", type=float, default=12.0,
                        help="degrees the camera aims to the right of the craft, between it and the VAB")
    parser.add_argument("--ut", type=float, help="the universal time to play at, in seconds, for daylight at the KSC")
    parser.add_argument("--out", default="out", help="where readings.json and the screenshots go")
    parser.add_argument("--port", type=int, default=8770, help="the port of KSP-MCPServer")
    parser.add_argument("--quit", action="store_true", help="quit KSP at the end")
    parser.add_argument("--keep-running", action="store_true",
                        help="leave KSP running at the end, which it does unless --quit is given")
    options = parser.parse_args()
    common.URL = "http://127.0.0.1:%d/mcp/" % options.port
    out = os.path.abspath(options.out)
    os.makedirs(out, exist_ok=True)
    session = common.Session(options, out)

    common.new_career(options.folder)
    if options.ut is not None:
        call("set_time", ut=options.ut)

    # 1. The VAB standing, and a launchpad that takes the dropper.
    vab_menu(session, "vab-menu-intact")
    state = call("facility_menu", facility="LaunchPad", button="Upgrade")
    log("LaunchPad upgraded to level %d of %d" % (state["level"], state["levels"]))

    # 2. Dropped onto the VAB, then reverted while it collapses.
    call("open_facility", facility="VAB")
    close(dialogs_only=True)
    call("launch_vessel", craft=options.dropper, site="LaunchPad")
    session.arrived("dropper launched from the VAB")
    towards_vab(session, "dropper-on-the-launchpad")
    drop(session)
    call("wait", seconds=1)
    session.shoot("vab-hit")
    call("revert_to_launch")
    session.arrived("revert to launch while the VAB collapses")
    towards_vab(session, "vab-after-the-revert")

    # 3. Dropped onto the VAB again, the VAB left to collapse, then the space centre.
    drop(session)
    call("wait", seconds=options.collapse)
    session.shoot("vab-collapsed")
    call("go_to_scene", scene="SPACECENTER")
    close(dialogs_only=True)
    vab_menu(session, "vab-menu-destroyed")

    # 4. Flights around the destroyed VAB, from the SPH.
    call("open_facility", facility="SPH")
    close(dialogs_only=True)
    call("launch_vessel", craft=options.rover, site="Runway")
    common.brake()
    session.arrived("rover launched from the SPH with the VAB destroyed", launchpad=False)
    towards_vab(session, "vab-destroyed-from-the-runway", runway=True)
    call("save_game", save=QUICKSAVE)
    call("load_save", folder=options.folder, save=QUICKSAVE)
    session.arrived("quickload with the VAB destroyed", launchpad=False)
    towards_vab(session, "vab-destroyed-after-the-quickload", runway=True)
    recovered = call("recover_vessel")
    log("recovered: %s" % json.dumps(recovered.get("recovery")))
    close(dialogs_only=False)

    # 5. Repaired.
    state = call("facility_menu", facility="VAB", button="Repair")
    log("VAB repaired: %s" % json.dumps(state))
    vab_menu(session, "vab-menu-repaired")
    call("open_facility", facility="VAB")
    close(dialogs_only=True)
    call("launch_vessel", craft=options.craft, site="LaunchPad")
    session.arrived("launched from the repaired VAB")
    towards_vab(session, "vab-repaired-from-the-launchpad")

    log("done: %d arrivals in flight, in %s" % (len(session.readings), out))
    if options.quit:
        call("quit_game")


if __name__ == "__main__":
    main()
