#ifndef AH_ROCKMODEL
#define AH_ROCKMODEL
/*******************************************************************************
 AH_RockModel.h

 Author: Andres Huertas - Jet Propulsion Laboratory - Vision Group

 History:
   01/23/06: A. Huertas. Created.
 
 Usage:
   header file to AH_RockModel.c
*****************************************************************************/
#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

#define ANDRES    0
#define SHOWMODS  1

#define TRUE      1
#define FALSE     0
#define INSIDE    0
#define OUTSIDE 255
#define NO_INT    0

#define UP 0
#define DOWN 1
#define UP_OR_DOWN 2
#define LEFT 3
#define RIGHT 4
#define LEFT_OR_RIGHT 5
#define NA 6
#define maxPoints 2500
#define maxRocks  5000

#define ELLIPTICAL 1
#define CIRCULAR   2

#define Double_MIN_VALUE 4.9E-324
#define Double_MAX_VALUE 1.7976931348623157E308

#ifndef MAX
#define MAX(a,b) (((a) < (b)) ? (b) : (a))
#endif

#ifndef MIN
#define MIN(a,b) (((a) < (b)) ? (a) : (b))
#endif


int LabelConnectedRegions(unsigned char  *B, int rows, int cols, unsigned short *CC, int* numRegions);


#ifdef __cplusplus
}
#endif /* __cplusplus */

#endif /*AH_ROCKMODEL*/
