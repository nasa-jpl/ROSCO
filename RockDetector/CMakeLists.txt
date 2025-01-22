# RockDetector/CMakeLists.txt

CMAKE_MINIMUM_REQUIRED(VERSION 3.15)

PROJECT(RockDetector VERSION 1.0 DESCRIPTION "Orbital Rock Detector" LANGUAGES C)

SET( CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib CACHE PATH
     "Output directory for libraries" )
	 
SET( EXECUTABLE_OUTPUT_PATH ${CMAKE_BINARY_DIR}/bin CACHE PATH
     "Output directory for executable files" )

SET( RockDetector_sources
     AH_RockUtils.c
     AH_RockModel.c
     AH_Rocks.c
     DoubleMatrix.c
     PGM.c
   )

find_package(OpenMP)
FIND_PACKAGE(SWIG)

add_library(RockDetectorStatic STATIC ${RockDetector_sources})
add_library(RockDetectorShared SHARED ${RockDetector_sources})

if (MSVC)
    # warning level 4 and all warnings as errors
    target_compile_options(RockDetectorStatic PRIVATE /W4 /WX)
    target_compile_options(RockDetectorShared PRIVATE /W4 /WX)
    if (OPENMP_FOUND)
        add_compile_options(/openmp)
    endif()
    add_definitions(-D_CRT_SECURE_NO_WARNINGS)
    set(CMAKE_WINDOWS_EXPORT_ALL_SYMBOLS ON)
 else()
    # lots of warnings and all warnings as errors
    add_compile_options(-Wall -Wextra -pedantic -Werror -g)

    if (OPENMP_FOUND)
        set (CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} ${OpenMP_CXX_FLAGS}")
    endif()
endif()

ADD_EXECUTABLE( RockDetector RockDetector.c )
ADD_EXECUTABLE( RunTests test/runtests.c )

set_property(TARGET RockDetectorStatic RockDetectorShared RockDetector RunTests PROPERTY C_STANDARD 99)
target_compile_features(RockDetector PUBLIC c_std_99)

if (MSVC)
	TARGET_LINK_LIBRARIES( RockDetector RockDetectorStatic)
    TARGET_LINK_LIBRARIES( RunTests RockDetectorStatic)
else()
	TARGET_LINK_LIBRARIES( RockDetector RockDetectorStatic m) # need to explicitly state this for Linux
    TARGET_LINK_LIBRARIES( RunTests RockDetectorStatic m) # need to explicitly state this for Linux
endif()

# performance improvements if openmp is available
if(OpenMP_CXX_FOUND)
    target_link_libraries(RockDetector PUBLIC OpenMP::OpenMP_C)
endif()

 # generate python support with swig
if(SWIG_FOUND)
    cmake_policy(SET CMP0078 NEW) # standard target names
    cmake_policy(SET CMP0086 NEW) # module option
    INCLUDE(${SWIG_USE_FILE})

    FIND_PACKAGE(PythonLibs)
    INCLUDE_DIRECTORIES(${PYTHON_INCLUDE_PATH})
    INCLUDE_DIRECTORIES(${CMAKE_CURRENT_SOURCE_DIR})
    SET(CMAKE_SWIG_FLAGS "")
    set(CMAKE_SWIG_OUTDIR ${CMAKE_CURRENT_BINARY_DIR}/swig)
    set_property(SOURCE RockDetectorPy.i PROPERTY CPLUSPLUS OFF)
    SWIG_ADD_LIBRARY(RockDetectorPy TYPE MODULE LANGUAGE python SOURCES RockDetectorPy.i)
    set_property(TARGET RockDetectorPy PROPERTY C_STANDARD 99)
    target_compile_features(RockDetectorPy PUBLIC c_std_99)
    SWIG_LINK_LIBRARIES(RockDetectorPy RockDetectorStatic ${PYTHON_LIBRARIES})

    install(TARGETS RockDetectorPy
         LIBRARY DESTINATION lib)
endif()
