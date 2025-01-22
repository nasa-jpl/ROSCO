import os
import ctypes

rockDetector=0
if platform.system() == 'Windows':
	rockDetector = ctypes.cdll.LoadLibrary("RockDetectorShared.dll")
else:
	rockDetector = ctypes.cdll.LoadLibrary("RockDetectorShared.so")

#detection
rockDetector.detect_from_files.argtypes [ctypes.c_char_p, ctypes.c_char_p, ctypes.c_char_p]
