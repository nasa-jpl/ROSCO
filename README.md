# ROck Shadow COunter (ROSCO)

ROSCO is a codebase originally developed at [NASA/JPL](https://www.jpl.nasa.gov) to detect rocks in aerial imagery by by looking for their shadows.

This repository brings several existing sources into a linear history and is the currently active branch of the shadow-based rock detection code from 2006 to present.  The sources included:

* code recovered from the original authors, Andres Huertas and Yang Cheng, used for the Mars Phoenix Lander mission
* code handed off by Andres at his retirement (used for the MSL and InSight missions)
* code used for the Mars 2020 mission
* the RockCollect user interface built by Bob Crocco.

## Contributors and Stakeholders

* [Andres Huertas](https://www-robotics.jpl.nasa.gov/who-we-are/people/andres_huertas/) (retired) - member of the computer vision group at NASA/JPL
* [Yang Cheng](https://www-robotics.jpl.nasa.gov/who-we-are/people/yang_cheng) - member of the [Aerial and Orbital Image Analysis](https://www-robotics.jpl.nasa.gov/who-we-are/groups/347P) group at NASA/JPL
* [Marshall Trautman](https://www.linkedin.com/in/marshall-trautman) - engineering applications software engineer at NASA/JPL
* [Matthew Golombek](https://science.jpl.nasa.gov/people/golombek) - senior research scientist at NASA/JPL
* [Shane Byrne](https://www.lpl.arizona.edu/faculty/shane-byrne) - faculty member at the Lunar and Planetary Laboratory, University of Arizona
* [Bob Crocco](https://www.linkedin.com/in/bob-crocco-47a140) - software developer at NASA/JPL, primary author of the RockCollect user interface
* [Marsette Vona](https://www2.ccs.neu.edu/research/gpc/vona.html) - software developer at NASA/JPL

## RockDetector Command Line Tool and Library

The core rock detection algorithms are implemented in a C++ codebase in the [RockDetector](./RockDetector) subdirectory.  This code builds on Linux, Mac OS X, and Windows.  It creates both a command-line executable and a library.

There are two dependencies:

* [OpenMP](https://www.openmp.org) enables parallelization for a big performance improvement
* [SWIG](https://www.swig.org) enables a Python interface to the library.

Both are optional, and will be used only if you install them on your system before building RockDetector.

Test instructions are [here](RockDetector/test/README.md).

### Building RockDetector on Linux and Mac OS X

Install [cmake](https://cmake.org).

```
  cd RockDetector
  mkdir build
  cd build
  cmake .. 
  make
``` 

The command line executable will be generated at `RockDetector/build/bin/RockDetector` and the shared library will be at `RockDetector/build/lib/libRockDetectorShared.*`.

### Building RockDetector on Windows

Install the latest version of cmake from [here](https://cmake.org/download).  These instructions were tested with cmake-3.28.3 on windows x86_64.

Install [Microsoft Visual Studio Community Edition](https://visualstudio.microsoft.com/vs/community).  These instructions were tested with

* [Visual Studio 16 2019](https://visualstudio.microsoft.com/vs/older-downloads), which is also the version currently used by the [RockCollect](#rockcollect-user-interface) UI.
* a [git bash](https://www.atlassian.com/git/tutorials/git-bash) command prompt, but the built-in cmd or [cygwin](https://cygwin.com) should also work.

Then run the following commands to generate the Visual Studio solution file:

```
  cd RockDetector
  mkdir build
  cd build
  cmake .. -G "Visual Studio 16 2019"
``` 

Then open the newly created `RockDetector/build/Rockdetector.sln` in Visual Studio.  In the "Solution Configurations" pulldown in the toolbar make sure "Release" is chosen.  In the adjacent "Solution Platforms" pulldown make sure "x64" is chosen.  Then in the Solution Explorer pane right click on "Solution 'RockDetector'" and chose "Build Solution".  The command line executable will be generated at `RockDetector/build/bin/Release/RockDetector.exe` and the shared library will be at `RockDetector/build/Release/RockDetectorShared.dll`.

## RockCollect User Interface

TODO

## References

* Otero, Richard; Huertas, A.; Almeida, E.; Golombek, M.; Trautman, M.; Rothrock, B., 2017, "Mars 2020 robust rock detection and analysis method", 14th International Planetary Probe Workshop, The Hague, Netherlands, June 12-16, 2017, JPL Open Repository; [CL17-2518.pdf](https://dataverse.jpl.nasa.gov/file.xhtml?fileId=59706&version=2.0).
* L. Matthies, A. Huertas, Y. Cheng and A. Johnson, "Stereo vision and shadow analysis for landing hazard detection," 2008 IEEE International Conference on Robotics and Automation, Pasadena, CA, USA, 2008, pp. 2735-2742, doi: [10.1109/ROBOT.2008.4543625](https://ieeexplore.ieee.org/document/4543625).
* A. Huertas, Yang Cheng and R. Madison, "Passive imaging based multi-cue hazard detection for spacecraft safe landing," 2006 IEEE Aerospace Conference, Big Sky, MT, USA, 2006, pp. 14 pp.-, doi: [10.1109/AERO.2006.1655794](https://ieeexplore.ieee.org/document/1655794).
* Golombek, M. P., et al. (2008), Size-frequency distributions of rocks on the northern plains of Mars with special reference to Phoenix landing surfaces, J. Geophys. Res., 113, E00A09, doi: [10.1029/2007JE003065](https://agupubs.onlinelibrary.wiley.com/doi/full/10.1029/2007JE003065).

## License

This codebase is released under the [Apache 2.0 license](https://github.com/nasa-jpl/ROSCO?tab=Apache-2.0-1-ov-file#readme).
