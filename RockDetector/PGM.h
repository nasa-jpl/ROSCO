#ifndef PGM
#define PGM

#ifdef _WIN32
#define EXPORT __declspec(dllexport)
#else
#define EXPORT
#endif

/*******************************************************************************
PGM.h

Author: Bob Crocco - Jet Propulsion Laboratory - 397Q

History:
    07/10/20: B.Crocco. Created. Migrated code from AH_RockUtils

Usage:
    header file to PGM.c
*****************************************************************************/

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

int read_pgm_image(char* infilename, unsigned char** image, int* rows, int* cols);
int read_pgm_image_rect(char* infilename, unsigned char* image, int startRow, int startCol, int rectNumRows, int rectNumCols);
int read_pgm_image_dimensions(char* infilename, int* rows, int* cols);

int write_pgm_image(char* outfilename, unsigned char* image, int rows, int cols, int maxval);
int write_ppm_image(char* outfilename, unsigned char* image, int rows, int cols);

#ifdef __cplusplus
}
#endif /* __cplusplus */

#endif /* PGM */
