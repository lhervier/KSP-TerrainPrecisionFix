"""Plays the scene changes protocol: a craft on the launchpad through every way of leaving the flight and coming back.

It drives KSP through KSP-MCPServer, a mod that answers HTTP requests on 127.0.0.1, and needs nothing but
Python 3: no AI, no package to install. Start KSP with KSP-MCPServer and KSP Diag - Colliders installed, wait
for the main menu, then run:

    python run-scene-changes.py --folder <your career game> --craft VAB/<craft>.craft --out out

The game should be a career: in a sandbox, the Research and Development, the Administration and Mission Control
only show the dialog that tells they are closed in this mode. With --new-game CAREER, the script starts that
game itself from the main menu, as New Game does at the Normal difficulty, the craft files already in its
Ships/VAB and Ships/SPH folders (the second craft, SPH/Diag3-Rover.craft by default, see --second-craft). It opens the game at the space centre and launches the craft onto the launchpad, then plays
each case of the folder scene-changes, in turn:

1. revert-to-launch: Revert to Launch, twice;
2. quicksave: a quicksave (F5), then a quickload of it (F9), twice;
3. space-centre: the space centre, the screen of each of its buildings opened and closed, then the tracking
   station, entered through its building, and the craft flown from there, as its Fly button does;
4. vab: the Vehicle Assembly Building, entered through its building, and a launch from it;
5. revert-to-vab: Revert to Vehicle Assembly Building, and a launch from it;
6. recovery: Recover, the recovery report closed, and a launch from the space centre;
7. main-menu: Quit to Main Menu, and the game loaded back into the flight of the craft;
8. another-body: the Space Plane Hangar, entered through its building, and a second craft launched from it onto
   the runway, then put around another body (the Mun by default) with Set Orbit, and back to the craft on the
   launchpad as Switch To of the map view does.

--cases plays some of them only, in that order. Each case can also be played alone, as a script of that folder
taking the same arguments. At every arrival in flight, once the craft has settled, it presses the button of KSP
Diag - Colliders that logs the colliders under each craft: the launchpad's, and its height above the terrain the
game computes there. It takes a screenshot of every scene and screen it goes through, writes every reading to
readings.json, prints the height of the launchpad at each arrival, and leaves KSP running (unless --quit is
given).

A new case is a script of the folder scene-changes (see its common.py), added to CASES below. The cases after
which KSP no longer reverts the flight come last.
"""
import os
import sys

sys.path.insert(0, os.path.join(os.path.dirname(os.path.abspath(__file__)), "scene-changes"))
import common  # noqa: E402

# The cases, in the order they are played.
CASES = ["revert-to-launch", "quicksave", "space-centre", "vab", "revert-to-vab", "recovery", "main-menu",
         "another-body"]

if __name__ == "__main__":
    common.main([common.load(name) for name in CASES], __doc__)
