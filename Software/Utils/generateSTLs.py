#!/usr/bin/python
# -*- coding: utf-8 -*-
import os, sys
FREECADPATH = '/usr/lib/freecad/lib/'
sys.path.append(FREECADPATH)

import FreeCAD, Mesh

assembly_file = os.path.abspath(os.path.join('..', '..', 'src', 'SunriseAssembly.FCStd'))
out_path = os.path.abspath('./stl')

if __name__ == '__main__':

    try:
        os.stat(out_path)
    except OSError:
        os.mkdir(out_path)

    FreeCAD.open(assembly_file)
    doc = App.getDocument("SunriseAssembly")

    printable = doc.getObjectsByLabel('Printable') + doc.getObjectsByLabel('Printable001')

    for group in printable:
        for obj in group.Group:
            Mesh.export([obj] , os.path.join(out_path, obj.Label + '.stl'))

    FreeCAD.closeDocument("SunriseAssembly")