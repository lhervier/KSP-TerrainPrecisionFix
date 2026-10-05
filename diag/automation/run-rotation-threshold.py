"""Reads, body by body, the altitude above which KSP turns the body rather than the world around the craft.

It drives KSP through KSP-MCPServer, a mod that answers HTTP requests on 127.0.0.1, and needs nothing but
Python 3: no AI, no package to install. Start KSP with KSP-MCPServer and KSP Diag - Floating Origin
installed, put a craft in flight (any craft: it is only moved from orbit to orbit), then run:

    python run-rotation-threshold.py Mun Minmus Gilly ...

For each body, it puts the craft in a circular equatorial orbit with the cheat Set Orbit, waits two seconds,
and reads the Frame column of KSP Diag - Floating Origin: Rotating while KSP turns the world with the body,
Inertial once it turns the body. It starts at an altitude above the body's relief and, where the body has
an atmosphere, above its top; doubles the altitude until the frame is inertial, never going past the body's
sphere of influence; then halves the interval down to 500 m. It turns on the cheats Ignore Max Temperature
and No Crash Damage first, and stops if the craft is lost or leaves the body.
"""
import json
import sys
import urllib.request

URL = "http://127.0.0.1:8770/mcp/"

# Lowest altitude read on each body, in metres: above its relief, and above the top of its atmosphere if it
# has one. The bodies of Real Solar System, then those of Outer Planets Mod; any other starts at 40 km.
START = {"Vesta": 60000, "Phobos": 30000, "Deimos": 30000, "Earth": 142000, "Venus": 147000,
         "Mars": 127000, "Titan": 602000, "Triton": 112000, "Pluto": 112000,
         "Hale": 15000, "Ovok": 30000, "Polta": 50000, "Priax": 50000, "Tekto": 97000, "Thatmo": 37000}
DEFAULT_START = 40000
# Highest altitude read, kept under the body's sphere of influence where that is small.
CAP = {"Phobos": 38000, "Deimos": 38000, "Mimas": 190000, "Enceladus": 230000, "Miranda": 200000,
       "Hale": 33000, "Ovok": 65000, "Priax": 360000, "Tal": 110000, "Karen": 800000}
DEFAULT_CAP = 2000000
PRECISION = 500


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


def is_rotating(body, altitude, craft):
    """Puts the craft in a circular orbit of the body at that altitude, and says whether the world turns."""
    call("set_orbit", body=body, altitude=altitude)
    call("wait", seconds=2)
    vessel = call("get_state").get("vessel")
    if not vessel or vessel["name"] != craft:
        sys.exit("the craft was lost around %s at %d m" % (body, altitude))
    if vessel["body"] != body:
        sys.exit("the craft left %s for %s at %d m" % (body, vessel["body"], altitude))
    return call("floatingorigin_read")["live"]["RotatingFrame"]


def main():
    craft = call("get_state")["vessel"]["name"]
    call("set_member", type="CheatOptions", member="IgnoreMaxTemperature", value=True)
    call("set_member", type="CheatOptions", member="NoCrashDamage", value=True)
    for body in sys.argv[1:]:
        start = START.get(body, DEFAULT_START)
        cap = CAP.get(body, DEFAULT_CAP)
        if not is_rotating(body, start, craft):
            print(body, "INERTIAL already at %d m" % start, flush=True)
            continue

        # Up until the frame is inertial, then down to the precision.
        low, high, altitude = start, None, start
        while altitude < cap:
            altitude = min(altitude * 2, cap)
            if is_rotating(body, altitude, craft):
                low = altitude
            else:
                high = altitude
                break
        if high is None:
            print(body, "still rotating at %d m (sphere of influence)" % low, flush=True)
            continue
        while high - low > PRECISION:
            middle = (low + high) // 2
            if is_rotating(body, middle, craft):
                low = middle
            else:
                high = middle
        print(body, "threshold between %d and %d m" % (low, high), flush=True)


if __name__ == "__main__":
    main()
