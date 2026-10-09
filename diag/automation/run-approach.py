"""Plays "The protocol: coming back to a craft you left" with both instruments at once.

It drives KSP through KSP-MCPServer, a mod that answers HTTP requests on 127.0.0.1, and needs nothing but
Python 3: no AI, no package to install. Start KSP with KSP-MCPServer, KSP Diag - Landed Vessel and KSP Diag
- Terrain Height installed, copy approach-kerbin.sfs into a sandbox game, wait for the main menu, then run:

    python run-approach.py --folder <your sandbox game> --trips 6 --out out

The save opens on the rover, 26 m south of the parked craft, which is already its target. For each round
trip it does what the protocol asks a player to do, in the same order, and records in both instruments at
every stop: beside the craft; about 700 m away; 2600 m away, out of range; then, on the way back, 2000 m
away, back in range; and back on the rover's starting spot, a few seconds after the craft is handed to
physics. It turns on the cheat Infinite Electricity first, for the rover. After each round trip it takes one
screenshot of each table, that window alone in the middle of the screen, and empties both tables: thirty lines
do not fit in a window. It prints what it read, writes it to lines.json after every round trip, and quits KSP
(unless --keep-running is given). The windows of the instruments are hidden once the flight opens, so that
the scene shows, and each is shown only for its screenshot.
"""
import argparse
import json
import os
import time
import urllib.request

URL = None

# Top speed of the rover between the stops, in metres per second.
SPEED = 15


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


def distance_to_target():
    return [v for v in call("list_vessels") if v["target"]][0]["distance"]


def wait_for_digits_to_settle():
    """Waits for the ground under the parked craft to stop moving (within 0.0005 mm over 2 s)."""
    last = None
    still = 0
    start = time.time()
    while time.time() - start < 20 and still < 4:
        value = call("terrainheight_read")["live"]["CollisionSurfaceMm"]
        still = still + 1 if last is not None and value is not None and abs(value - last) < 0.0005 else 0
        last = value
        time.sleep(0.5)


def record(trip, line, rows):
    """Presses Record in KSP Diag - Landed Vessel, then in KSP Diag - Terrain Height."""
    landed = call("landedvessel_record")
    terrain = call("terrainheight_record")
    distance = distance_to_target()
    rows.append(dict(trip=trip, line=line, distance=distance, landed_vessel=landed, terrain_height=terrain))
    if landed["SettledMm"] is None:
        log("  trip %d, line %d, %6.0f m: too far away to read" % (trip, line, distance))
    else:
        log("  trip %d, line %d, %6.0f m: Settled %16.3f mm  Moved %8.3f mm  Ground under craft %12.3f mm"
            % (trip, line, distance, landed["SettledMm"], landed["MovedMm"], terrain["CollisionSurfaceMm"]))


def screenshots(directory, trip, width):
    """One screenshot per table, each window alone in the middle of the screen, then both tables emptied: the
    windows stay hidden but for their own screenshot."""
    for shown in ("landedvessel", "terrainheight"):
        call(shown + "_show_window", visible=True)
        size = call(shown + "_move_window", x=0, y=60)
        call(shown + "_move_window", x=(width - size["width"]) / 2, y=60)
        call("wait", seconds=1)
        call("screenshot", path=os.path.join(os.path.abspath(directory), "trip%d-%s.png" % (trip, shown)),
             return_image=False)
        call(shown + "_show_window", visible=False)
    call("landedvessel_move_window", x=0, y=40)
    call("terrainheight_move_window", x=640, y=40)
    call("landedvessel_clear")
    call("terrainheight_clear")


def main():
    global URL
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--folder", required=True, help="the sandbox game under saves/ the save was copied into")
    parser.add_argument("--save", default="approach-kerbin", help="the save, without .sfs")
    parser.add_argument("--trips", type=int, default=6, help="how many round trips")
    parser.add_argument("--heading", type=float, default=180.0,
                        help="where the rover drives off to, in degrees from north (default south)")
    parser.add_argument("--out", default="out", help="where lines.json and the screenshots go")
    parser.add_argument("--screen-width", type=float, default=1280, help="the width of KSP's window, in pixels")
    parser.add_argument("--port", type=int, default=8770, help="the port of KSP-MCPServer")
    parser.add_argument("--keep-running", action="store_true", help="leave KSP running at the end")
    options = parser.parse_args()
    URL = "http://127.0.0.1:%d/mcp/" % options.port
    os.makedirs(options.out, exist_ok=True)
    away = options.heading % 360
    back = (away + 180) % 360

    call("load_save", folder=options.folder, save=options.save)
    # Six round trips of more than 6 km empty the rover's batteries faster than its panels fill them: the
    # cheat of Alt+F12 that gives it electricity without end. It changes nothing on the parked craft.
    call("set_cheats", infinite_electricity=True)
    call("landedvessel_clear")
    call("terrainheight_clear")
    call("landedvessel_move_window", x=0, y=40)
    call("terrainheight_move_window", x=640, y=40)
    call("landedvessel_show_window", visible=False)
    call("terrainheight_show_window", visible=False)
    rover = call("get_state")["vessel"]
    home = (rover["latitude"], rover["longitude"])
    call("wait", seconds=5)

    rows = []
    for trip in range(1, options.trips + 1):
        # Line 1: beside the craft.
        wait_for_digits_to_settle()
        record(trip, 1, rows)
        # Line 2: past 350 m, the craft packed again.
        call("drive", heading=away, speed=SPEED, distance=700)
        call("wait", seconds=2)
        record(trip, 2, rows)
        # Line 3: past 2500 m, the craft unloaded.
        call("drive", heading=away, speed=SPEED, distance=1900)
        call("wait", seconds=2)
        record(trip, 3, rows)
        # Turn round beyond 3 km.
        call("drive", heading=away, speed=SPEED, distance=500)
        # Line 4: back under 2250 m, the craft loaded again, still packed.
        call("drive", heading=back, speed=SPEED, distance=distance_to_target() - 2000)
        call("wait", seconds=3)
        record(trip, 4, rows)
        # Line 5: back on the starting spot, the craft handed over to physics a few seconds ago.
        call("drive", heading=back, speed=SPEED, distance=distance_to_target() - 80)
        call("drive_to", latitude=home[0], longitude=home[1], speed=5, tolerance=1)
        call("wait", seconds=4)
        wait_for_digits_to_settle()
        record(trip, 5, rows)

        screenshots(options.out, trip, options.screen_width)
        with open(os.path.join(options.out, "lines.json"), "w", newline="") as f:
            json.dump(rows, f, indent=1)
    log("done")
    if not options.keep_running:
        call("quit_game")


if __name__ == "__main__":
    main()
