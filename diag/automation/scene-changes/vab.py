"""The Vehicle Assembly Building, entered through its building at the space centre, and a launch from it.
"""
import sys

import common
from common import call


def play(session):
    call("go_to_scene", scene="SPACECENTER")
    common.at_space_centre()
    call("open_facility", facility="VAB")
    session.shoot("vab-from-space-centre")
    call("launch_vessel", craft=session.options.craft, site="LaunchPad")
    session.arrived("launched from the VAB")


if __name__ == "__main__":
    common.run_alone(sys.modules[__name__])
