# -*- coding: utf-8 -*-
"""Builds one save per circular equatorial Mun orbit, from the craft sitting on the launch pad.

Only that craft's VESSEL node is touched: its situation, and its orbital elements. KSP works the speed
out of SMA itself, so nothing here depends on knowing the Mun's gravitational parameter.
"""
import io
import os
import re
import sys

SRC = r'D:\KSP-Dev\saves\claude\persistent.sfs'
DST_DIR = r'D:\KSP-Dev\saves\claude'
MARKER = 'liquidEngine3.v2'
MUN_RADIUS = 200000.0
MUN_REF = 2
# The Mun's own subdivision levels, read from a save made there: when these do not match the body the
# craft is loaded at, KSP puts the craft back on the ground rather than trusting the save.
MUN_PQS_MIN = 2
MUN_PQS_MAX = 9
ALTITUDES_KM = [3, 4, 5, 6, 10, 16, 25, 40]

TAB3 = '\t\t\t'
TAB4 = '\t\t\t\t'
TAB1 = TAB3[:1]
TAB2 = TAB3[:2]


def find_block(lines, start, header):
    """The [first, last] line indices of the block named header, braces included."""
    i = start
    while i < len(lines):
        if lines[i].strip() == header:
            depth = 0
            j = i + 1
            while j < len(lines):
                s = lines[j].strip()
                if s == '{':
                    depth += 1
                elif s == '}':
                    depth -= 1
                    if depth == 0:
                        return i, j
                j += 1
        i += 1
    return None


def each_vessel(lines):
    """Every VESSEL block, as (index in the flight state, first line, last line)."""
    index = 0
    i = 0
    while i < len(lines):
        if lines[i].strip() == 'VESSEL':
            span = find_block(lines, i, 'VESSEL')
            if span is None:
                return
            yield index, span[0], span[1]
            index += 1
            i = span[1] + 1
        else:
            i += 1


def set_field(lines, lo, hi, indent, key, value):
    pattern = re.compile('^' + re.escape(indent) + re.escape(key) + r' = .*?(\r?\n)$')
    for k in range(lo, hi + 1):
        m = pattern.match(lines[k])
        if m:
            lines[k] = indent + key + ' = ' + value + m.group(1)
            return
    raise KeyError('field not found: ' + key)


def universal_time(lines):
    for line in lines:
        m = re.match(r'^\t\tUT = (\S+)', line)
        if m:
            return m.group(1)
    raise KeyError('UT not found')


def main():
    lines = io.open(SRC, encoding='utf-8', newline='').readlines()
    ut = universal_time(lines)

    craft = None
    for index, lo, hi in each_vessel(lines):
        if any(MARKER in lines[k] for k in range(lo, hi + 1)):
            craft = (index, lo, hi)
            break
    if craft is None:
        sys.exit('no craft carrying ' + MARKER + ' in ' + SRC)
    index, lo, hi = craft
    orbit = find_block(lines, lo, 'ORBIT')
    print('craft #%d at lines %d-%d, orbit at %d-%d, UT = %s'
          % (index, lo + 1, hi + 1, orbit[0] + 1, orbit[1] + 1, ut))

    for km in ALTITUDES_KM:
        out = list(lines)
        altitude = km * 1000.0

        # Straight into flight on that craft, rather than at the space centre.
        set_field(out, 0, len(out) - 1, TAB1, 'scene', '7')
        set_field(out, 0, len(out) - 1, TAB2, 'activeVessel', str(index))

        # Off the ground, and nothing left of where it was standing.
        set_field(out, lo, hi, TAB3, 'name', 'Bench Mun %d km' % km)
        set_field(out, lo, hi, TAB3, 'sit', 'ORBITING')
        set_field(out, lo, hi, TAB3, 'landed', 'False')
        set_field(out, lo, hi, TAB3, 'launchedFrom', '')
        set_field(out, lo, hi, TAB3, 'landedAt', '')
        set_field(out, lo, hi, TAB3, 'displaylandedAt', '')
        set_field(out, lo, hi, TAB3, 'splashed', 'False')
        set_field(out, lo, hi, TAB3, 'skipGroundPositioning', 'False')
        set_field(out, lo, hi, TAB3, 'hgt', '-1')
        set_field(out, lo, hi, TAB3, 'nrm', '0,1,0')
        set_field(out, lo, hi, TAB3, 'met', '0')
        set_field(out, lo, hi, TAB3, 'lastUT', ut)
        set_field(out, lo, hi, TAB3, 'distanceTraveled', '0')

        # Where it is, for the record: on rails, KSP works all three out of the orbit below.
        set_field(out, lo, hi, TAB3, 'lat', '0')
        set_field(out, lo, hi, TAB3, 'lon', '0')
        set_field(out, lo, hi, TAB3, 'alt', repr(altitude))
        set_field(out, lo, hi, TAB3, 'PQSMin', str(MUN_PQS_MIN))
        set_field(out, lo, hi, TAB3, 'PQSMax', str(MUN_PQS_MAX))

        # Circular and equatorial, starting at the same point of the orbit whatever the altitude, at the
        # moment the save is loaded.
        set_field(out, orbit[0], orbit[1], TAB4, 'SMA', repr(MUN_RADIUS + altitude))
        set_field(out, orbit[0], orbit[1], TAB4, 'ECC', '0')
        set_field(out, orbit[0], orbit[1], TAB4, 'INC', '0')
        set_field(out, orbit[0], orbit[1], TAB4, 'LPE', '0')
        set_field(out, orbit[0], orbit[1], TAB4, 'LAN', '0')
        set_field(out, orbit[0], orbit[1], TAB4, 'MNA', '0')
        set_field(out, orbit[0], orbit[1], TAB4, 'EPH', ut)
        set_field(out, orbit[0], orbit[1], TAB4, 'REF', str(MUN_REF))

        path = os.path.join(DST_DIR, 'bench-mun-%02dkm.sfs' % km)
        with io.open(path, 'w', encoding='utf-8', newline='') as handle:
            handle.write(''.join(out))
        print('wrote ' + path)


main()
