"""Plays the scene changes protocol: a craft on the launchpad through every way of leaving the flight and coming back.

It drives KSP through KSP-MCPServer, a mod that answers HTTP requests on 127.0.0.1, and needs nothing but
Python 3: no AI, no package to install. Start KSP with KSP-MCPServer and KSP Diag - Colliders installed, wait
for the main menu, then run:

    python run-scene-changes.py --folder <your career game> --craft VAB/<craft>.craft --out out

The game should be a career: in a sandbox, the Research and Development, the Administration and Mission Control
only show the dialog that tells they are closed in this mode. With --new-game CAREER, the script starts that
game itself from the main menu, as New Game does at the Normal difficulty, the craft file already in its
Ships/VAB folder. It opens the game at the space centre and launches the craft onto the launchpad, then, in
turn:

1. reverts to launch, twice;
2. saves (F5), and loads that save, twice (F9);
3. goes to the space centre, opens the screen of every building that has one, closes it, then enters the
   tracking station through its building and flies the craft from there, as its Fly button does;
4. goes to the space centre again, enters the Vehicle Assembly Building through its building and launches the
   craft from it;
5. reverts to the Vehicle Assembly Building, and launches the craft again;
6. recovers the craft, closes the recovery report, and launches the craft from the space centre;
7. quits to the main menu, and loads the game's persistent save, which holds the craft on the launchpad.

At every arrival in flight, once the craft has settled, it presses the button of KSP Diag - Colliders that logs
the colliders under each craft: the launchpad's, and its height above the terrain the game computes there. It
takes a screenshot of every scene and screen it goes through, writes every reading to readings.json, prints the
height of the launchpad at each arrival, and leaves KSP running (unless --quit is given).
"""
import argparse
import json
import os
import time
import urllib.request

URL = None

# The screens that open over the space centre, as KSP names the buildings.
SCREENS = ["AstronautComplex", "RnD", "MissionControl", "Administration"]


def call(tool, **args):
    """Calls one tool of KSP-MCPServer and returns its answer, decoded from JSON when it is JSON."""
    body = json.dumps({"jsonrpc": "2.0", "id": 1, "method": "tools/call",
                       "params": {"name": tool, "arguments": args}}).encode()
    request = urllib.request.Request(URL, body, {"Content-Type": "application/json"})
    with urllib.request.urlopen(request, timeout=900) as response:
        result = json.loads(response.read())["result"]
    text = result["content"][0].get("text", "") if result["content"] else ""
    if result.get("isError"):
        raise RuntimeError(tool + ": " + text)
    try:
        answer = json.loads(text)
    except ValueError:
        return text
    # The tools another mod adds answer inside "returned".
    if isinstance(answer, dict) and list(answer) == ["returned"]:
        return answer["returned"]
    return answer


def log(*parts):
    print(time.strftime("%H:%M:%S"), *parts, flush=True)


def close(dialogs_only):
    """Closes what is open over the scene, one thing at a time, until nothing is: the dialogs only (the guide of a
    new career shows one in the space centre, the editor and the tracking station), or with them the recovery
    report and the screen of a building."""
    for _ in range(10):
        answer = call("close_screen", dialogs_only=dialogs_only)
        if answer["closed"] == "nothing":
            return
        log("closed: " + answer["closed"] + (" '%s', pressing '%s'" % (answer.get("dialog"), answer["button"])
                                             if "button" in answer else ""))
    raise RuntimeError("still something open over the scene after ten closings")


class Session:
    """The steps of the protocol, numbered in the order they are played, and what each of them read."""

    def __init__(self, out, settle):
        self.out = out
        self.settle = settle
        self.readings = []
        self.step = 0

    def shoot(self, name):
        """Takes a screenshot of the scene as it stands, interface included, once its dialogs are closed."""
        self.step += 1
        path = os.path.join(self.out, "%02d-%s.png" % (self.step, name))
        call("wait", seconds=2)
        close(dialogs_only=True)
        call("screenshot", path=path, return_image=False)
        log("screenshot " + os.path.basename(path))

    def arrived(self, how):
        """Reads the colliders under the craft, once it has settled after an arrival in flight."""
        self.step += 1
        call("wait", seconds=self.settle)
        call("colliders_show_window", visible=False)
        state = call("get_state")
        crafts = call("colliders_log_under_crafts")
        active = state.get("vessel", {}).get("name")
        under = next((c for c in crafts if c["craft"] == active), None)
        reading = {"step": self.step, "arrival": how, "state": state, "underCrafts": crafts}
        self.readings.append(reading)
        with open(os.path.join(self.out, "readings.json"), "w", newline="") as f:
            json.dump(self.readings, f, indent=1)
        first = under["colliders"][0] if under and under["colliders"] else None
        if first is None:
            log("%02d %s: no collider under the craft" % (self.step, how))
        else:
            log("%02d %s: %s (%s) %.3f mm above the terrain, %s" % (
                self.step, how, first["name"], first["parent"], first["heightMm"], state["vessel"]["situation"]))


def main():
    global URL
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--folder", required=True, help="the career game to play in")
    parser.add_argument("--new-game", choices=["CAREER", "SCIENCE_SANDBOX", "SANDBOX"],
                        help="start the game from the main menu in this mode, instead of opening it")
    parser.add_argument("--craft", required=True, help="the craft, as VAB/<name>.craft")
    parser.add_argument("--settle", type=float, default=10.0,
                        help="how long to let the craft settle on the launchpad at each arrival, in seconds")
    parser.add_argument("--ut", type=float, help="the universal time to play at, in seconds, for daylight at the KSC")
    parser.add_argument("--out", default="out", help="where readings.json and the screenshots go")
    parser.add_argument("--port", type=int, default=8770, help="the port of KSP-MCPServer")
    parser.add_argument("--quit", action="store_true", help="quit KSP at the end")
    parser.add_argument("--keep-running", action="store_true",
                        help="leave KSP running at the end, which it does unless --quit is given")
    options = parser.parse_args()
    URL = "http://127.0.0.1:%d/mcp/" % options.port
    out = os.path.abspath(options.out)
    os.makedirs(out, exist_ok=True)
    session = Session(out, options.settle)
    quick = "scene-changes-quicksave"

    if options.new_game:
        call("new_game", folder=options.folder, mode=options.new_game)
    else:
        call("open_game", folder=options.folder)
    if options.ut is not None:
        call("set_time", ut=options.ut)
    craft = call("launch_vessel", craft=options.craft, site="LaunchPad")["vessel"]["name"]
    session.arrived("launched from the space centre")

    # 1. Revert to Launch.
    for n in (1, 2):
        call("revert_to_launch")
        session.arrived("revert to launch %d" % n)

    # 2. Quicksave, quickload.
    call("save_game", save=quick)
    for n in (1, 2):
        call("load_save", folder=options.folder, save=quick)
        session.arrived("quickload %d" % n)

    # 3. The space centre, its buildings, the tracking station, and back to the craft from there.
    call("go_to_scene", scene="SPACECENTER")
    session.shoot("space-centre-from-flight")
    for facility in SCREENS:
        call("open_facility", facility=facility)
        session.shoot("open-" + facility)
        close(dialogs_only=False)
    call("open_facility", facility="TrackingStation")
    session.shoot("tracking-station")
    call("fly_vessel", vessel=craft)
    session.arrived("flown from the tracking station")

    # 4. The Vehicle Assembly Building, entered through its building.
    call("go_to_scene", scene="SPACECENTER")
    call("open_facility", facility="VAB")
    session.shoot("vab-from-space-centre")
    call("launch_vessel", craft=options.craft, site="LaunchPad")
    session.arrived("launched from the VAB")

    # 5. Revert to the Vehicle Assembly Building.
    call("revert_to_editor", facility="VAB")
    session.shoot("vab-after-revert")
    call("launch_vessel", craft=options.craft, site="LaunchPad")
    session.arrived("launched after a revert to the VAB")

    # 6. Recovery.
    recovered = call("recover_vessel")
    log("recovered: %s" % json.dumps(recovered.get("recovery")))
    session.shoot("recovery-report")
    close(dialogs_only=False)
    session.shoot("space-centre-after-recovery")
    call("launch_vessel", craft=options.craft, site="LaunchPad")
    session.arrived("launched after a recovery")

    # 7. The main menu, and back to the craft left on the launchpad.
    call("go_to_scene", scene="MAINMENU")
    session.shoot("main-menu")
    call("load_save", folder=options.folder, save="persistent")
    session.arrived("loaded from the main menu")

    log("done: %d arrivals in flight, in %s" % (len(session.readings), out))
    if options.quit:
        call("quit_game")


if __name__ == "__main__":
    main()
