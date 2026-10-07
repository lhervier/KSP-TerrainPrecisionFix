"""Recover, the recovery report closed, and a launch from the space centre.
"""
import json
import sys

import common
from common import call, close, log


def play(session):
    recovered = call("recover_vessel")
    common.at_space_centre()
    log("recovered: %s" % json.dumps(recovered.get("recovery")))
    session.shoot("recovery-report")
    close(dialogs_only=False)
    session.shoot("space-centre-after-recovery")
    call("launch_vessel", craft=session.options.craft, site="LaunchPad")
    session.arrived("launched after a recovery")


if __name__ == "__main__":
    common.run_alone(sys.modules[__name__])
