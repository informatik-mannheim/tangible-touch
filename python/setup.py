import sys
from cx_Freeze import setup, Executable
from os.path import dirname

### change load_scipy to load scipy._lib instead of scipy.lib
from cx_Freeze import hooks

def load_scipy(finder, module):
    """the scipy module loads items within itself in a way that causes
       problems without the entire package and a number of other subpackages
       being present."""
    finder.IncludePackage("scipy._lib")
    finder.IncludePackage("scipy.misc")

hooks.load_scipy = load_scipy
###


# Dependencies are automatically detected, but it might need fine tuning.
build_exe_options = {"packages": ["numpy.core._methods", "numpy.lib.format", "scipy.sparse.csgraph._validation"], "excludes": ["tkinter"]}

# GUI applications require a different base on Windows (the default is for a
# console application).
base = None
#if sys.platform == "win32":
#    base = "Win32Console"

setup(  name = "Touchcode",
        version = "0.1",
        description = "Converts a list of touchpoints into a 12 bit touchcode.",
        options = {"build_exe": build_exe_options},
        executables = [Executable("touchcode.py", base=base)])
