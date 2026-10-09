"""Plays "The protocol: switching to a craft far away" with the instruments installed.

It drives KSP through KSP-MCPServer, a mod that answers HTTP requests on 127.0.0.1, and needs nothing but
Python 3: no AI, no package to install. Start KSP with KSP-MCPServer and KSP Diag - Landed Vessel, KSP Diag -
Terrain Height or both installed, copy switch-kerbin.sfs into a sandbox game, wait for the main menu, then run:

    python run-switching.py --folder <your sandbox game> --rounds 6 --out out

For each round it does what the protocol asks a player to do, in the same order: load the save, which opens on
the rover with the capsule as its target, record; switch to the capsule as the ] key does, wait four seconds and
for the digits to stop moving, record. Every instrument installed records at every step. It never saves the game.
At the end it takes a screenshot of each table, that window alone, prints what it read, writes it to lines.json, and quits KSP
(unless --keep-running is given). The windows of the instruments are hidden once the flight opens, so that
the scene shows, and each is shown only for its screenshot.
"""
import argparse
import json
import os
import time
import urllib.request

URL = None

# What each instrument is called by, and the live value whose settling the protocol waits for.
INSTRUMENTS = {
    "landedvessel": "SettledMm",
    "terrainheight": "CollisionSurfaceMm",
}


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


def installed():
    """The instruments whose tools the server offers."""
    body = json.dumps({"jsonrpc": "2.0", "id": 1, "method": "tools/list"}).encode()
    request = urllib.request.Request(URL, body, {"Content-Type": "application/json"})
    with urllib.request.urlopen(request, timeout=60) as response:
        names = {t["name"] for t in json.loads(response.read())["result"]["tools"]}
    return [i for i in INSTRUMENTS if i + "_record" in names]


def log(*parts):
    print(time.strftime("%H:%M:%S"), *parts, flush=True)


def wait_for_digits_to_settle(instrument):
    """What the protocol asks: wait for the live value to stop moving (within 0.0005 mm over 2 s)."""
    last = None
    still = 0
    start = time.time()
    while time.time() - start < 30 and still < 4:
        value = call(instrument + "_read")["live"][INSTRUMENTS[instrument]]
        still = still + 1 if last is not None and value is not None and abs(value - last) < 0.0005 else 0
        last = value
        time.sleep(0.5)


def record(rows, instruments, round_, step):
    """Presses Record in every instrument installed."""
    row = dict(round=round_, step=step)
    for instrument in instruments:
        row[instrument] = call(instrument + "_record")
    rows.append(row)
    parts = []
    if "landedvessel" in row:
        parts.append("Settled %16.3f mm  Moved %9.3f mm" % (row["landedvessel"]["SettledMm"],
                                                             row["landedvessel"]["MovedMm"]))
    if "terrainheight" in row:
        parts.append("Ground under craft %14.3f mm" % row["terrainheight"]["CollisionSurfaceMm"])
    log("  round %d, %-6s  %s" % (round_, step, "  ".join(parts)))


def screenshots(directory, name, instruments, width):
    """One screenshot per table, each window alone in the middle of the screen: the windows stay hidden but
    for their own screenshot."""
    for shown in instruments:
        call(shown + "_show_window", visible=True)
        size = call(shown + "_move_window", x=0, y=60)
        call(shown + "_move_window", x=(width - size["width"]) / 2, y=60)
        call("wait", seconds=1)
        call("screenshot", path=os.path.join(os.path.abspath(directory), "%s-%s.png" % (name, shown)),
             return_image=False)
        call(shown + "_show_window", visible=False)


def main():
    global URL
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--folder", required=True, help="the sandbox game under saves/ the save was copied into")
    parser.add_argument("--save", default="switch-kerbin", help="the save, without .sfs")
    parser.add_argument("--rounds", type=int, default=6, help="how many rounds")
    parser.add_argument("--out", default="out", help="where lines.json and the screenshots go")
    parser.add_argument("--screen-width", type=float, default=1280, help="the width of KSP's window, in pixels")
    parser.add_argument("--port", type=int, default=8770, help="the port of KSP-MCPServer")
    parser.add_argument("--keep-running", action="store_true", help="leave KSP running at the end")
    options = parser.parse_args()
    URL = "http://127.0.0.1:%d/mcp/" % options.port
    os.makedirs(options.out, exist_ok=True)
    instruments = installed()
    if not instruments:
        raise SystemExit("neither KSP Diag - Landed Vessel nor KSP Diag - Terrain Height answers")
    log("instruments:", ", ".join(instruments))

    rows = []
    for round_ in range(1, options.rounds + 1):
        call("load_save", folder=options.folder, save=options.save)
        if round_ == 1:
            for i, instrument in enumerate(instruments):
                call(instrument + "_clear")
                call(instrument + "_move_window", x=640 * i, y=40)
                call(instrument + "_show_window", visible=False)
        target = [v for v in call("list_vessels") if v["target"]]
        if len(target) != 1:
            raise RuntimeError("the save should open with the capsule as the target")

        # Step 1: flying the rover, the window reading its target.
        call("wait", seconds=3)
        record(rows, instruments, round_, "target")

        # Step 2: switch to the capsule, give it four seconds, wait for the digits, record.
        call("switch_vessel", vessel=target[0]["id"])
        call("wait", seconds=4)
        for instrument in instruments:
            wait_for_digits_to_settle(instrument)
        record(rows, instruments, round_, "flown")

    screenshots(options.out, "switching", instruments, options.screen_width)
    with open(os.path.join(options.out, "lines.json"), "w", newline="") as f:
        json.dump(rows, f, indent=1)
    log("done")
    if not options.keep_running:
        call("quit_game")


if __name__ == "__main__":
    main()
