"""Quit to Main Menu, and the game loaded back into the flight of the craft.

Leaving for the main menu saves the game as persistent, with the craft on the launchpad, and loading that save
opens the flight on it. After it, KSP no longer reverts the flight.
"""
import sys

import common
from common import call


def play(session):
    call("go_to_scene", scene="MAINMENU")
    session.shoot("main-menu")
    call("load_save", folder=session.options.folder, save="persistent")
    session.arrived("loaded from the main menu")


if __name__ == "__main__":
    common.run_alone(sys.modules[__name__])
