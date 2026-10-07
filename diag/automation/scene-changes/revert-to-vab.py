"""Revert to Vehicle Assembly Building, and a launch from it.
"""
import sys

import common
from common import call


def play(session):
    call("revert_to_editor", facility="VAB")
    session.shoot("vab-after-revert")
    call("launch_vessel", craft=session.options.craft, site="LaunchPad")
    session.arrived("launched after a revert to the VAB")


if __name__ == "__main__":
    common.run_alone(sys.modules[__name__])
