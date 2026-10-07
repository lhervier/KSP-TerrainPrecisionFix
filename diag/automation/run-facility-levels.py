"""Plays the facility levels protocol: launches onto the launchpad and the runway at each of their three levels.

It drives KSP through KSP-MCPServer, a mod that answers HTTP requests on 127.0.0.1, and needs nothing but
Python 3: no AI, no package to install. Start KSP with KSP-MCPServer and KSP Diag - Colliders installed, wait
for the main menu, put the two craft files in saves/<folder>/Ships/VAB and Ships/SPH, then run:

    python run-facility-levels.py --folder <a new game> --out out

It starts that career itself from the main menu, set up as its player would to build the two crafts (see
new_career in scene-changes/common.py). Then, at each level of the launchpad and the runway, from the first, the
one a new career starts with:

1. the menus of the launchpad and the runway, opened for a screenshot that shows their level;
2. the Vehicle Assembly Building, entered through its building, and the craft launched onto the launchpad; then
   Revert to Launch, twice, and a quicksave (F5) loaded back (F9); then Recover;
3. the Space Plane Hangar, entered through its building, and the second craft launched onto the runway, its
   brakes put on at once, as after each revert; the same reverts and quickload; then Recover;
4. the launchpad and the runway upgraded from their menus, as their Upgrade button does, for the next level.

At every arrival in flight, once the craft has settled, it presses the button of KSP Diag - Colliders that logs
the colliders under each craft: the launchpad's or the runway's, with its height above the terrain the game
computes there. It takes a screenshot of every menu and building it goes through, writes every reading to
readings.json, prints the height under the craft at each arrival, and leaves KSP running (unless --quit is
given).
"""
import argparse
import json
import os
import sys

sys.path.insert(0, os.path.join(os.path.dirname(os.path.abspath(__file__)), "scene-changes"))
import common  # noqa: E402
from common import call, close, log  # noqa: E402

QUICKSAVE = "facility-levels-quicksave"

# The facilities that are launched from, with the editor each craft is built in.
SITES = [("LaunchPad", "VAB", "craft"), ("Runway", "SPH", "second_craft")]


def flights(session, site, editor, craft, level):
    """Launches the craft from its editor onto its site, reverts it twice, loads a quicksave of it, and recovers
    it, reading the colliders under it at each arrival."""
    on_pad = site == "LaunchPad"
    where = "level %d %s" % (level, "launchpad" if on_pad else "runway")
    call("open_facility", facility=editor)
    session.shoot("level%d-%s" % (level, editor.lower()))
    call("launch_vessel", craft=craft, site=site)
    if not on_pad:
        common.brake()
    session.arrived(where + ": launched from the " + editor, launchpad=on_pad)
    for n in (1, 2):
        call("revert_to_launch")
        if not on_pad:
            common.brake()
        session.arrived(where + ": revert to launch %d" % n, launchpad=on_pad)
    call("save_game", save=QUICKSAVE)
    call("load_save", folder=session.options.folder, save=QUICKSAVE)
    session.arrived(where + ": quickload", launchpad=on_pad)
    recovered = call("recover_vessel")
    common.at_space_centre()
    log("recovered: %s" % json.dumps(recovered.get("recovery")))
    close(dialogs_only=False)


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--folder", required=True, help="the new game to start, under saves/")
    parser.add_argument("--craft", default="VAB/Diag3-Rocket.craft",
                        help="the craft launched onto the launchpad, as VAB/<name>.craft (default VAB/Diag3-Rocket.craft)")
    parser.add_argument("--second-craft", default="SPH/Diag3-Rover.craft",
                        help="the craft launched onto the runway, as SPH/<name>.craft (default SPH/Diag3-Rover.craft)")
    parser.add_argument("--settle", type=float, default=10.0,
                        help="how long to let the craft settle at each arrival, in seconds")
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
    for level in (1, 2, 3):
        if level > 1:
            for site, _, _ in SITES:
                state = call("facility_menu", facility=site, button="Upgrade")
                log("%s upgraded to level %d of %d, %.0f funds left" % (
                    site, state["level"], state["levels"], state["funds"]))
        for site, _, _ in SITES:
            call("facility_menu", facility=site)
            session.shoot("level%d-%s-menu" % (level, site.lower()))
            close(dialogs_only=False)
        for site, editor, craft in SITES:
            flights(session, site, editor, getattr(options, craft), level)

    log("done: %d arrivals in flight, in %s" % (len(session.readings), out))
    if options.quit:
        call("quit_game")


if __name__ == "__main__":
    main()
