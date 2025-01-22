 /*******************************************************************************
 RockDetector.c Main Module extracted from AH_Rocks.c 

 Author: Andres Huertas - Jet Propulsion Laboratory - Vision Group

 Sponsor: MTP/AEDL _ Yang Cheng, PI

 History:
   01/23/06: A. Huertas. Created from original version in Java.
 
 Usage:
   AH_Rocks SceneImg Sun_Inc Sun_Azim Gnd_Res Min_Size Haz_Size As_Ratio MET_Gamma

 Input:
   1-SceneImg   : 8-bit PGM image
   2-Sun_Inc    : With respect to vertical,(off-nadir)(degrees)
   3-Sun_Azim   : With respect to image Y axis (degrees)
   4-Gnd_Res    : Ground resolution (cm/pixel)
   5-Min_Size   : Min. Area of shadow to process (pixels)
   6-Haz_Size   : Hazard diameter (pixel).(Eventually change to height when GSD/Altitude incorporated.)
   7-As_Ratio   : Max. Aspect Ratio of shadow region
   8-MET_Gamma  : Gamma correction (2-9). Use 9 for very high contrast shadows.

 Output:
   1-HazImg     : Rocks-inputname.pgm  (Gray rocks overlayed on black shadows in white background)
   2-ShaImg     : Shads-inputname.pgm  (For debug: White background with shadow rocks in black)
   3-OutImg     : Outs-inputname.pgm   (For debug: White with shadow rocks in black)

 Compile:
    gcc -c AH_Rocks.c AH_RockModel.c AH_RockUtils.c
 Link & Load:
    gcc -o AH_rocks AH_Rocks.o AH_RockModel.o AH_RockUtils.o ac_timing.o -lm

*******************************************************************************/

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include "AH_RockModel.h"
#include "AH_Rocks.h"
#include "DoubleMatrix.h"
#include "AH_RockUtils.h"

#define         CFO_STRING      0
#define         CFO_INT         1
#define         CFO_DOUBLE      2
#define         CFO_FLOAT       3

void show_usage_and_exit(void) {
  printf("Usage for rock_detection:\n");
  printf("------------------\n");
  printf("  Required arguments:\n");
  printf("    -i <Image filename> - image file in PGM format\n");
  printf("    -p <Rock Detection Parameters>  - Parameter file\n");
  printf("    -r <Rock List Record>  - Rock List file\n");
  exit(0);
}

void toggle(int *ptr) {
  if (*ptr!=0) *ptr=0;
  else *ptr=1;
}

int getflag(char **argv, char *argname, int *argvarptr) {
  if (strcmp(argv[0],argname)==0) {
    toggle(argvarptr);
    return 1;
  }
  return 0;
}

/* char **argv;                         List of arguments */
/* char *argname;                       Name of argument to try to match */
/* void *argvarptr;                     Ptr to the location of the arg */
/* int argtype;                         Type of the arg variable */
int getarg(char **argv, char *argname, void *argvarptr, int argtype) {

  float dummy;

  char tmp[1024];
  if (strcmp(argv[0],argname)==0) {

    if (argtype==CFO_STRING) {

      *((char **) argvarptr)=argv[1];

    } else if (argtype==CFO_DOUBLE) {

      if (sscanf(argv[1],"%f",&dummy)!=1) {
        snprintf(tmp,1024,"Error reading %s value: %s\n",argname,argv[1]);
        fputs(tmp, stderr);
        exit(1);
      } else {
        *((double *) argvarptr)=(double) dummy;
      }

    } else if (argtype==CFO_FLOAT) {

      if (sscanf(argv[1],"%f",&dummy)!=1) {
          snprintf(tmp,1024,"Error reading %s value: %s\n",argname,argv[1]);
        fputs(tmp, stderr);
        exit(1);
      } else {
        *((float *) argvarptr)=dummy;
      }

    } else if (argtype==CFO_INT) {

      if (sscanf(argv[1],"%d",((int *) argvarptr))!=1) {
        snprintf(tmp,1024,"Error reading %s value: %s\n",argname,argv[1]);
        fputs(tmp, stderr);
        exit(1);
      }
    }

    return 1;
  }
  return 0;
}

int main(int argc, char *argv[])
{

  char *SceneName = NULL;        /* Name of the input overhead image */
  char *PMfile = NULL;
  char *RKfile = NULL;

  argc--;
  argv++;

  if (argc == 0) show_usage_and_exit();//argc);

  while (argc>0) {
    if (argc == 0) show_usage_and_exit();//argc);
    if ((getarg(argv, "-i",     &SceneName,    CFO_STRING)!=1) &&
        (getarg(argv, "-p",     &PMfile,       CFO_STRING)!=1) &&
        (getarg(argv, "-r",     &RKfile,       CFO_STRING)!=1)) {
      show_usage_and_exit();//argc);
    }
    argc-=2;
    argv+=2;
  }

  return detect_from_files(SceneName, PMfile, RKfile);
}



