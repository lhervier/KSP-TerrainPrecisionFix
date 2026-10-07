"""A quicksave (F5), then a quickload of it (F9), twice.

The save is made once, on the launchpad, and loaded back into the flight each time.
"""
import sys

import common
from common import call

SAVE = "scene-changes-quicksave"


def play(session):
    call("save_game", save=SAVE)
    for n in (1, 2):
        call("load_save", folder=session.options.folder, save=SAVE)
        session.arrived("quickload %d" % n)


if __name__ == "__main__":
    common.run_alone(sys.modules[__name__])
