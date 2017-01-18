import sys
from cx_Freeze import setup, Executable
from os.path import dirname
from cx_Freeze import hooks

def load_scipy_fixed(finder, module):
    """includes scipy._lib instead of scipy.lib as that causes runtime trouble of frozen executable."""
    finder.IncludePackage("scipy._lib")
    finder.IncludePackage("scipy.misc")

hooks.load_scipy = load_scipy_fixed

build_exe_options = {"packages": ["numpy.core._methods", "numpy.lib.format", "scipy.sparse.csgraph._validation"], "excludes": ["tkinter"]}

setup(  name = "Touchcode",
        version = "0.1",
        description = "Converts a list of touchpoints into a 12 bit touchcode.",
        options = {"build_exe": build_exe_options},
        executables = [Executable("touchcode.py", base=None)])
