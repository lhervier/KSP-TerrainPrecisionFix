"""Revert to Launch, twice.

KSP opens the flight again on the craft as it was launched, onto the launchpad.
"""
import sys

import common
from common import call


def play(session):
    for n in (1, 2):
        call("revert_to_launch")
        session.arrived("revert to launch %d" % n)


if __name__ == "__main__":
    common.run_alone(sys.modules[__name__])
