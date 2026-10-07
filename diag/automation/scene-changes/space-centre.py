"""The space centre, the screen of each of its buildings, then the tracking station, and the craft flown from there.

Every building that opens a screen over the space centre is opened, then closed; the tracking station is entered
through its building, and the craft flown from it, as its Fly button does. In a sandbox game, the Research and
Development, the Administration and Mission Control only show the dialog that says they are closed in this mode:
play this case in a career.
"""
import sys

import common
from common import call, close

# The screens that open over the space centre, as KSP names the buildings.
SCREENS = ["AstronautComplex", "RnD", "MissionControl", "Administration"]


def play(session):
    call("go_to_scene", scene="SPACECENTER")
    common.at_space_centre()
    session.shoot("space-centre-from-flight")
    for facility in SCREENS:
        call("open_facility", facility=facility)
        session.shoot("open-" + facility)
        close(dialogs_only=False)
    call("open_facility", facility="TrackingStation")
    session.shoot("tracking-station")
    call("fly_vessel", vessel=session.craft_id)
    session.arrived("flown from the tracking station")


if __name__ == "__main__":
    common.run_alone(sys.modules[__name__])
