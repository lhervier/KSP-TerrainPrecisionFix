"""Plays the loads of "The protocol: an anchor on a static" once the anchors are placed, with both instruments.

It drives KSP through KSP-MCPServer, a mod that answers HTTP requests on 127.0.0.1, and needs nothing but
Python 3: no AI, no package to install. Start KSP with KSP-MCPServer, KSP Diag - Landed Vessel and KSP Diag
- Terrain Height installed, place the two anchors by hand as the protocol says, quicksave, then run:

    python run-anchor-on-a-static.py --folder <your sandbox game> --alone <id> --base <id> --loads 5 --out out

--alone and --base name the anchor placed alone and the anchor with a battery on it: their ids, as the
list_vessels tool of KSP-MCPServer gives them, or their names when no other vessel bears them. First it
reads both anchors as placed, then, for each load, does what the protocol asks a player to do, in the same
order: quicksave, quickload, wait for the digits to stop moving, set each anchor as target, record in both
instruments, take a screenshot of each window alone. It prints what it read, writes it to lines.json, saves
the screenshots next to it, and quits KSP (unless --keep-running is given).

With --from-save, the anchors are already placed in a save of the game: it loads that save first, and reads
the anchors as loaded from it instead of as placed. With --only, it reads one of the two anchors, alone or
base, and leaves the other out.
"""
import argparse
import json
import os
import time
import urllib.request

URL = None


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


def wait_for_digits_to_settle():
    """What the protocol asks: wait for Settled to stop moving (within 0.0005 mm over 2 s)."""
    last = None
    still = 0
    start = time.time()
    while time.time() - start < 30 and still < 4:
        value = call("landedvessel_read")["live"]["SettledMm"]
        still = still + 1 if last is not None and value is not None and abs(value - last) < 0.0005 else 0
        last = value
        time.sleep(0.5)


def record(stop, anchor, vessel, out, rows):
    """Sets the anchor as target, presses Record in KSP Diag - Landed Vessel, then in KSP Diag - Terrain
    Height, and takes a screenshot of each window, the other one hidden."""
    # KSP refuses a target while targeting is locked, as it is while its window is not the one in focus:
    # wait for a click in it.
    start = time.time()
    while True:
        try:
            call("set_target", vessel=vessel)
            break
        except RuntimeError as error:
            if "Targeting is locked" not in str(error) or time.time() - start > 900:
                raise
            if start + 2 > time.time():
                log("  targeting is locked: click in the window of KSP")
            time.sleep(2)
    call("wait", seconds=1)
    wait_for_digits_to_settle()
    landed = call("landedvessel_record")
    terrain = call("terrainheight_record")
    pictures = []
    for shown, hidden, name in (("landedvessel", "terrainheight", "landed-vessel"),
                                ("terrainheight", "landedvessel", "terrain-height")):
        call(hidden + "_show_window", visible=False)
        call(shown + "_show_window", visible=True)
        picture = os.path.abspath(os.path.join(out, "%s-%s-%s.png" % (stop, anchor, name)))
        call("screenshot", path=picture, return_image=False)
        pictures.append(os.path.basename(picture))
    call("landedvessel_show_window", visible=True)
    rows.append(dict(stop=stop, anchor=anchor, landed_vessel=landed, terrain_height=terrain,
                     screenshots=pictures))
    # As placed, nothing was loaded yet: On rails and Moved are empty.
    def mm(value):
        return "%16.3f" % value if value is not None else "%16s" % "--"
    log("  %-8s %-6s On rails %s mm  Settled %s mm  Moved %s mm  Ground under craft %s mm"
        % (stop, anchor, mm(landed["OnRailsMm"]), mm(landed["SettledMm"]), mm(landed["MovedMm"]),
           mm(terrain["CollisionSurfaceMm"])))


def main():
    global URL
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--folder", required=True, help="the sandbox game under saves/ the anchors are placed in")
    parser.add_argument("--alone", required=True, help="the anchor placed alone: its id, or its name")
    parser.add_argument("--base", required=True, help="the anchor with a battery on it: its id, or its name")
    parser.add_argument("--loads", type=int, default=5, help="how many quicksaves and quickloads")
    parser.add_argument("--out", default="out", help="where lines.json and the screenshots go")
    parser.add_argument("--port", type=int, default=8770, help="the port of KSP-MCPServer")
    parser.add_argument("--keep-running", action="store_true", help="leave KSP running at the end")
    parser.add_argument("--from-save", help="a save of the game the anchors are already placed in, to load first")
    parser.add_argument("--only", choices=["alone", "base"], help="read this anchor only")
    options = parser.parse_args()
    URL = "http://127.0.0.1:%d/mcp/" % options.port
    os.makedirs(options.out, exist_ok=True)

    # The anchors as placed, in the session they were placed in; or as loaded from the save they were
    # placed in. The windows of the instruments only exist in flight.
    first = "placed"
    if options.from_save:
        call("load_save", folder=options.folder, save=options.from_save)
        call("wait", seconds=3)
        first = "loaded"
    call("landedvessel_clear")
    call("terrainheight_clear")
    # One window at a time in the middle of the screen, for the screenshots.
    call("landedvessel_move_window", x=320, y=40)
    call("terrainheight_move_window", x=320, y=40)
    rows = []
    anchors = [(name, vessel) for name, vessel in (("alone", options.alone), ("base", options.base))
               if options.only in (None, name)]
    for anchor, vessel in anchors:
        record(first, anchor, vessel, options.out, rows)

    for load in range(1, options.loads + 1):
        call("save_game", folder=options.folder, save="quicksave")
        call("load_save", folder=options.folder, save="quicksave")
        call("wait", seconds=3)
        for anchor, vessel in anchors:
            record("load%d" % load, anchor, vessel, options.out, rows)
        with open(os.path.join(options.out, "lines.json"), "w", newline="") as f:
            json.dump(rows, f, indent=1)

    log("done")
    if not options.keep_running:
        call("quit_game")


if __name__ == "__main__":
    main()
