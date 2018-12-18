#!/usr/bin/env python

from setuptools import setup
from setuptools.extension import Extension
from numpy import get_include as get_numpy_include

if __name__ == "__main__":
    setup(name="tifffile",
          author="Christoph Gohlke",
          version="2016.6.21",
          license="BSD",
          description="Read and write image data from TIFF files",
          packages=["tifffile"],
          ext_modules=[Extension('tifffile._tifffile', ['tifffile/tifffile.c'],
                                 include_dirs=[get_numpy_include()])],
          requires=['numpy (>=1.10)'])
