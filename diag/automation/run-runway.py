"""Plays "The protocol: the runway and the grass beside it" with both instruments at once.

It drives KSP through KSP-MCPServer, a mod that answers HTTP requests on 127.0.0.1, and needs nothing but
Python 3: no AI, no package to install. Start KSP with KSP-MCPServer, KSP Diag - Landed Vessel and KSP Diag
- Terrain Height installed, copy runway-kerbin.sfs into a sandbox game, wait for the main menu, then run:

    python run-runway.py --folder <your sandbox game> --loads 6 --out out

For each loading it does what the protocol asks a player to do, in the same order: load the save, wait for
the digits to stop moving, record on the craft being flown, switch to the other craft as the [ key does,
wait, record again. Both instruments record at every stop. It prints what it read, writes it to lines.json,
and quits KSP (unless --keep-running is given).
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


def record(load, craft, rows):
    """Presses Record in KSP Diag - Landed Vessel, then in KSP Diag - Terrain Height."""
    landed = call("landedvessel_record")
    terrain = call("terrainheight_record")
    rows.append(dict(load=load, craft=craft, landed_vessel=landed, terrain_height=terrain))
    log("  load %d, %-6s Settled %16.3f mm  Moved %8.3f mm  Ground under craft %12.3f mm"
        % (load, craft, landed["SettledMm"], landed["MovedMm"], terrain["CollisionSurfaceMm"]))


def main():
    global URL
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--folder", required=True, help="the sandbox game under saves/ the save was copied into")
    parser.add_argument("--save", default="runway-kerbin", help="the save, without .sfs")
    parser.add_argument("--loads", type=int, default=6, help="how many loadings")
    parser.add_argument("--out", default="out", help="where lines.json goes")
    parser.add_argument("--port", type=int, default=8770, help="the port of KSP-MCPServer")
    parser.add_argument("--keep-running", action="store_true", help="leave KSP running at the end")
    options = parser.parse_args()
    URL = "http://127.0.0.1:%d/mcp/" % options.port
    os.makedirs(options.out, exist_ok=True)

    rows = []
    for load in range(1, options.loads + 1):
        call("load_save", folder=options.folder, save=options.save)
        if load == 1:
            call("landedvessel_clear")
            call("terrainheight_clear")
            call("landedvessel_move_window", x=0, y=40)
            call("terrainheight_move_window", x=640, y=40)
        vessels = call("list_vessels")
        other = [v for v in vessels if v["loaded"] and not v["active"]]
        if len(other) != 1:
            raise RuntimeError("expected one other craft within range, found %d" % len(other))

        # Step 1: the craft on the grass, the one the save opens on.
        call("wait", seconds=3)
        wait_for_digits_to_settle()
        record(load, "grass", rows)

        # Step 2: switch to the craft on the runway, give it a few seconds, record.
        call("switch_vessel", vessel=other[0]["id"])
        call("wait", seconds=4)
        wait_for_digits_to_settle()
        record(load, "runway", rows)

    with open(os.path.join(options.out, "lines.json"), "w", newline="") as f:
        json.dump(rows, f, indent=1)
    log("done")
    if not options.keep_running:
        call("quit_game")


if __name__ == "__main__":
    main()
