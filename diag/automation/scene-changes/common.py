"""What the cases of the scene changes protocol share: the calls to KSP-MCPServer, the readings, and the start.

Every case is a script of this folder. It starts with the craft in flight on the launchpad, the active vessel,
leaves the flight one way, and ends with that craft in flight on the launchpad again, read by KSP Diag -
Colliders at every arrival. A case is a module with:

- a docstring, whose first line says what it plays;
- play(session): plays it;
- optionally arguments(parser): adds the arguments it needs to the command line.

Played alone, a case calls main() with itself; run-scene-changes.py, one folder up, plays them all in turn.
"""
import argparse
import collections
import importlib.util
import json
import os
import sys
import time
import urllib.request

FOLDER = os.path.dirname(os.path.abspath(__file__))
URL = None

# A case of the protocol: its name (its file's, without .py), its docstring and its module.
Case = collections.namedtuple("Case", "name doc module")


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
    """The steps of the protocol, numbered in the order they are played, what each of them read, and the craft
    on the launchpad (craft_id: the id of that vessel at its last arrival)."""

    def __init__(self, options, out):
        self.options = options
        self.out = out
        self.readings = []
        self.step = 0
        self.craft_id = None

    def shoot(self, name):
        """Takes a screenshot of the scene as it stands, interface included, once its dialogs are closed."""
        self.step += 1
        path = os.path.join(self.out, "%02d-%s.png" % (self.step, name))
        call("wait", seconds=2)
        close(dialogs_only=True)
        call("screenshot", path=path, return_image=False)
        log("screenshot " + os.path.basename(path))

    def arrived(self, how):
        """Reads the colliders under each craft, once the craft on the launchpad, just arrived in flight and the
        active vessel, has settled."""
        self.step += 1
        call("wait", seconds=self.options.settle)
        call("colliders_show_window", visible=False)
        state = call("get_state")
        crafts = call("colliders_log_under_crafts")
        active = state.get("vessel", {})
        self.craft_id = active.get("id")
        under = next((c for c in crafts if c["craft"] == active.get("name")), None)
        reading = {"step": self.step, "arrival": how, "state": state, "underCrafts": crafts}
        self.readings.append(reading)
        with open(os.path.join(self.out, "readings.json"), "w", newline="") as f:
            json.dump(self.readings, f, indent=1)
        first = under["colliders"][0] if under and under["colliders"] else None
        if first is None:
            log("%02d %s: no collider under the craft" % (self.step, how))
        else:
            log("%02d %s: %s (%s) %.3f mm above the terrain, %s" % (
                self.step, how, first["name"], first["parent"], first["heightMm"], active["situation"]))


def load(name):
    """The case of this folder whose file is <name>.py."""
    path = os.path.join(FOLDER, name + ".py")
    spec = importlib.util.spec_from_file_location("case_" + name.replace("-", "_"), path)
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return case(module)


def case(module):
    """The case a module plays, the module of a case run alone included."""
    name = os.path.splitext(os.path.basename(module.__file__))[0]
    return Case(name, module.__doc__, module)


def main(cases, description):
    """Reads the command line, opens the game, launches the craft onto the launchpad, then plays the cases asked
    for, in the order of <cases>."""
    global URL
    parser = argparse.ArgumentParser(description=description, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--folder", required=True, help="the career game to play in")
    parser.add_argument("--new-game", choices=["CAREER", "SCIENCE_SANDBOX", "SANDBOX"],
                        help="start the game from the main menu in this mode, instead of opening it")
    parser.add_argument("--craft", required=True, help="the craft, as VAB/<name>.craft")
    parser.add_argument("--cases", nargs="+", choices=[c.name for c in cases], metavar="CASE",
                        help="the cases to play, among: " + ", ".join(c.name for c in cases) +
                             " (default: all of them); they are played in that order whatever the order given")
    parser.add_argument("--settle", type=float, default=10.0,
                        help="how long to let the craft settle on the launchpad at each arrival, in seconds")
    parser.add_argument("--ut", type=float, help="the universal time to play at, in seconds, for daylight at the KSC")
    parser.add_argument("--out", default="out", help="where readings.json and the screenshots go")
    parser.add_argument("--port", type=int, default=8770, help="the port of KSP-MCPServer")
    parser.add_argument("--quit", action="store_true", help="quit KSP at the end")
    parser.add_argument("--keep-running", action="store_true",
                        help="leave KSP running at the end, which it does unless --quit is given")
    for c in cases:
        if hasattr(c.module, "arguments"):
            c.module.arguments(parser)
    options = parser.parse_args()
    URL = "http://127.0.0.1:%d/mcp/" % options.port
    out = os.path.abspath(options.out)
    os.makedirs(out, exist_ok=True)
    session = Session(options, out)
    chosen = [c for c in cases if options.cases is None or c.name in options.cases]

    if options.new_game:
        call("new_game", folder=options.folder, mode=options.new_game)
    else:
        call("open_game", folder=options.folder)
    if options.ut is not None:
        call("set_time", ut=options.ut)
    call("launch_vessel", craft=options.craft, site="LaunchPad")
    session.arrived("launched from the space centre")

    for c in chosen:
        log("case %s: %s" % (c.name, c.doc.strip().splitlines()[0]))
        c.module.play(session)

    log("done: %d arrivals in flight, in %s" % (len(session.readings), out))
    if options.quit:
        call("quit_game")


def run_alone(module):
    """Plays the case of <module> alone, after the start every case needs."""
    main([case(module)], module.__doc__)


if __name__ == "__main__":
    sys.exit("common.py is not a case: run run-scene-changes.py, or one of the cases of this folder")
