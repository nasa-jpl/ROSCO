#ifndef AH_ROCKUTILS
#define AH_ROCKUTILS

 /*******************************************************************************
 AH_RockUtils.h

 Author: Bob Crocco - Jet Propulsion Laboratory - 397Q

 History:
   07/06/20: B.Crocco. Created.

 Usage:
   header file to AH_RockUtils.c
*****************************************************************************/

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

#include "AH_Rocks.h"

typedef struct _OUTROCK
{
    int id;
    int tileR;
    int tileC;
    float shaX;
    float shaY;
    float rockX;
    float rockY;
    float tileShaX;
    float tileShaY;
    int shaArea;
    float shaLen;
    float rockWidth;
    float rockHeight;
    float score;
    float gradMean;
    float Compact;
    float Exent;
    int Class;
    float gamma;
    float shaEllipseMajor;
    float shaEllipseMinor;
    float shaEllipseTheta;
}OUTROCK;

int read_param_file(char* filename, RD_PARMS* src);
EXPORT int write_param_file(char* filename, RD_PARMS* src);
int read_rocklist(char* rockListPath, RD_PARMS* rd_parms, OUTROCK** rocks, int* numRocks);

int compare_rocklist_files(char* rocklist0, char* rocklist1, int ignoreId);

int compare_rocklists(RD_PARMS* parms0, OUTROCK* rocks0, int numRocks0,
                      RD_PARMS* parms1, OUTROCK* rocks1, int numRocks1, int ignoreId);

//TODO: rework this api
int detect(char* inputImagePath, RD_PARMS* params, OUTROCK** out_rocks, int* num_out_rocks);

EXPORT int detect_per_tile_settings(char* inputImagePath, char* outputRockListPath, int numSettings, RD_PARMS* settingsArray);
EXPORT int detect_from_files(char* inputImagePath, char* paramsPath, char* outputRockListPath);
EXPORT int detect_tile_rocks(char* inputImagePath, unsigned short* labelImg, unsigned char* subimg,
                int tRow, int tCol, int frows, int fcols, int nbr, int nbc,
                RD_PARMS* params, OUTROCK* rocks, int* maxlab);

//intermediate products
EXPORT int gamma_from_files(char* inputImagePath, float gamma, char* outputGammaPath);
EXPORT int threshold_from_files(char* inputImagePath, float gamma, int thresholdOverride, char* outputShadowPath, int* selectedThreshold);
EXPORT int shadows_from_files(char* inputThresholdImagePath, char* outputShadowsImagePath, int* outNumShadows);
#ifdef __cplusplus
}
#endif /* __cplusplus */

#endif /*AH_ROCKUTILS*/
