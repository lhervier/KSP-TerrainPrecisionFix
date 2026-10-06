"""Drives the rover of station-kourou-rss.sfs east, from 27.8 km of the CommNet ground station of Kourou to
27.3 km of it, across the 27.5 km at which this mod takes the station out of its sphere.

It drives KSP through KSP-MCPServer, a mod that answers HTTP requests on 127.0.0.1, and needs nothing but
Python 3: no AI, no package to install. Start KSP with Real Solar System, this mod and KSP-MCPServer
installed, load station-kourou-rss.sfs, then run:

    python run-station-approach.py

It releases the brakes, drives the rover east at 2 m/s to the point 27.3 km west of the station, and stops
there with the brakes on. Before and after, it prints the length of the rover's first CommNet hop, the
one the part action window of its cabin shows as the distance of the first hop.
"""
import json
import urllib.request

URL = "http://127.0.0.1:8770/mcp/"

# 27.3 km due west of the station of Kourou (latitude 5.23938, longitude -52.768487), on the latitude of the
# rover in the save.
TARGET = (5.23899, -53.01502)


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
        return json.loads(text)
    except ValueError:
        return text


def first_hop():
    """The length of the active vessel's first CommNet hop, in metres."""
    return call("get_member", type="CommNet.CommNetVessel", member="ControlPath.0.cost")["value"]


def main():
    print("first hop before: %.2f m" % first_hop())
    call("set_controls", brakes=False)
    call("drive_to", latitude=TARGET[0], longitude=TARGET[1], speed=2, tolerance=2)
    print("first hop after:  %.2f m" % first_hop())


if __name__ == "__main__":
    main()
