"""A trip to another body: a second craft sent around it, and back to the craft on the launchpad from the map view.

A second craft, the same as the first, is launched onto the runway, then put around another body with Set Orbit of
the Alt+F12 menu: the space centre is out of its reach, and KSP no longer loads the craft on the launchpad. The
player then switches back to that craft as Switch To of the map view does: KSP saves the game as persistent and
opens the flight again on it. After it, KSP no longer reverts the flight. The second craft is left around the
other body.
"""
import sys

import common
from common import call


def arguments(parser):
    parser.add_argument("--other-body", default="Mun",
                        help="the body the second craft is sent around (default Mun; Moon in Real Solar System)")
    parser.add_argument("--other-altitude", type=float, default=100000,
                        help="the altitude of its circular orbit, in metres above the body's radius: above its "
                             "highest peak, within its sphere of influence (default 100000)")


def play(session):
    options = session.options
    craft = session.craft_id
    call("go_to_scene", scene="SPACECENTER")
    session.shoot("space-centre-before-the-trip")
    call("launch_vessel", craft=options.craft, site="Runway")
    session.shoot("second-craft-on-the-runway")
    call("set_orbit", body=options.other_body, altitude=options.other_altitude)
    session.shoot("second-craft-around-" + options.other_body.lower())
    call("switch_vessel", vessel=craft)
    session.arrived("switched to from around " + options.other_body)


if __name__ == "__main__":
    common.run_alone(sys.modules[__name__])
