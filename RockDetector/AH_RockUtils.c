/*******************************************************************************
* AH_utils.c - Andres Huertas - Various utilities, including I/O
*******************************************************************************/

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>
#include <math.h>
#include <assert.h>

#include "AH_RockUtils.h"
#include "AH_RockModel.h"
#include "PGM.h"

#ifdef _OPENMP
#include <omp.h>
#endif

//settings (#define to prevent openmp shared variables
//subImageCols: original: tcols
//subImageRows: original: trows
#define forcedOverlap 50
#define tcols 500
#define trows 500

//needed to make vc compiler (windows) results emulate the rounding behavior seen in the linux gcc compiler
float RoundToNearestTieToEven(float in, int precisionDigits)
{
    double moveDecimals = pow(10, precisionDigits);
    double result = rint(in * moveDecimals);
    result /= moveDecimals;

    return (float)result;
}

const double ROCKLIST_FILE_VERSION = 2.0;

int write_rocklist_header(FILE* fp_out, RD_PARMS* rd_parms)
{

    fprintf(fp_out, "version %f\n", ROCKLIST_FILE_VERSION);
    fprintf(fp_out, "%%GSD_resolution %f\n", rd_parms->ground_resolution);
    fprintf(fp_out, "%%gamma %f\n", rd_parms->gamma);
    fprintf(fp_out, "%%sun_azimuth_angle %f\n", rd_parms->sun_azimuth_angle);
    fprintf(fp_out, "%%sun_incidence_angle %f\n", rd_parms->sun_incidence_angle);
    fprintf(fp_out, "%%min_shadow_size %f\n", rd_parms->min_shadow_size);
    fprintf(fp_out, "%%max_shadow_size %f\n", rd_parms->max_shadow_size);
    fprintf(fp_out, "%%confidence_threshold %f\n", rd_parms->confidence_threshold);
    fprintf(fp_out, "%%min_split_shadow_size  %f\n", rd_parms->min_shadow_size_split);
    fprintf(fp_out, "%%mean_gradient  %f\n", rd_parms->mean_gradient_threshold);
    fprintf(fp_out, "%%shadow excentricity %f\n", rd_parms->rock_elongate_ratio);
    fprintf(fp_out, "%%gamma_threshold_override %d\n", rd_parms->gamma_threshold_override);
    fprintf(fp_out, "id, tileR, tileC, shaX, shaY, rockX, rockY, tileShaX, tileShaY, shaArea, shaLen, rockWidth, rockHeight, score, gradMean, Compact, Exent, Class, gamma\n");
    fflush(stdout);

    return 1;
}

int read_rocklist_header_v1(FILE* fp_in, RD_PARMS* rd_parms)
{
    int read = 0;
    memset(rd_parms, 0, sizeof(RD_PARMS));
    rd_parms->spliting_ratio = RD_ROCK_SPLITING_RATIO; //initialize to default

    read = fscanf(fp_in, "%%GSD_resolution %f\n", &rd_parms->ground_resolution);
    if (read != 1) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%gamma %f\n", &rd_parms->gamma);
    if (read != 1) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%sun_azimuth_angle %f\n", &rd_parms->sun_azimuth_angle);
    if (read != 1) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%sun_incidence_angle %f\n", &rd_parms->sun_incidence_angle);
    if (read != 1) { printf("error reading file. quitting. \n");  fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%min_shadow_size %f\n", &rd_parms->min_shadow_size);
    if (read != 1) { printf("error reading file. quitting. \n");  fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%max_shadow_size %f\n", &rd_parms->max_shadow_size);
    if (read != 1) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%confidence_threshold %f\n", &rd_parms->confidence_threshold);
    if (read != 1) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%min_split_shadow_size  %f\n", &rd_parms->min_shadow_size_split);
    if (read != 1) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%mean_gradient  %f\n", &rd_parms->mean_gradient_threshold);
    if (read != 1) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%shadow excentricity %f\n", &rd_parms->rock_elongate_ratio);
    if (read != 1) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%gamma_threshold_override %d\n", &rd_parms->gamma_threshold_override);
    if (read != 1) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "id, tileR, tileC, shaX, shaY, rockX, rockY, tileShaX, tileShaY, shaArea, shaLen, rockWidth, rockHeight, score, gradMean, Compact, Exent, Class, gamma\n");
    if (read != 0) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    //enforce file precision 
    rd_parms->ground_resolution = RoundToNearestTieToEven(rd_parms->ground_resolution, 6);
    rd_parms->gamma = RoundToNearestTieToEven(rd_parms->gamma, 6);
    rd_parms->sun_azimuth_angle = RoundToNearestTieToEven(rd_parms->sun_azimuth_angle, 6);
    rd_parms->sun_incidence_angle = RoundToNearestTieToEven(rd_parms->sun_incidence_angle, 6);
    rd_parms->min_shadow_size = RoundToNearestTieToEven(rd_parms->min_shadow_size, 6);
    rd_parms->max_shadow_size = RoundToNearestTieToEven(rd_parms->max_shadow_size, 6);
    rd_parms->confidence_threshold = RoundToNearestTieToEven(rd_parms->confidence_threshold, 6);
    rd_parms->min_shadow_size_split = RoundToNearestTieToEven(rd_parms->min_shadow_size_split, 6);
    rd_parms->mean_gradient_threshold = RoundToNearestTieToEven(rd_parms->mean_gradient_threshold, 6);
    rd_parms->rock_elongate_ratio = RoundToNearestTieToEven(rd_parms->rock_elongate_ratio, 6);
    
    return TRUE;
}

int read_rocklist_header(FILE* fp_in, RD_PARMS* rd_parms)
{
    int read = 0;

    float version = 0;
    read = fscanf(fp_in, "version %f\n", &version);
    if (read != 1)
    {
        fseek(fp_in, 0, SEEK_SET);
        return read_rocklist_header_v1(fp_in, rd_parms);
    }

    memset(rd_parms, 0, sizeof(RD_PARMS));
    rd_parms->spliting_ratio = RD_ROCK_SPLITING_RATIO; //initialize to default

    read = fscanf(fp_in, "%%GSD_resolution %f\n", &rd_parms->ground_resolution);
    if (read != 1) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%gamma %f\n", &rd_parms->gamma);
    if (read != 1) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%sun_azimuth_angle %f\n", &rd_parms->sun_azimuth_angle);
    if (read != 1) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%sun_incidence_angle %f\n", &rd_parms->sun_incidence_angle);
    if (read != 1) { printf("error reading file. quitting. \n");  fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%min_shadow_size %f\n", &rd_parms->min_shadow_size);
    if (read != 1) { printf("error reading file. quitting. \n");  fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%max_shadow_size %f\n", &rd_parms->max_shadow_size);
    if (read != 1) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%confidence_threshold %f\n", &rd_parms->confidence_threshold);
    if (read != 1) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%min_split_shadow_size  %f\n", &rd_parms->min_shadow_size_split);
    if (read != 1) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%mean_gradient  %f\n", &rd_parms->mean_gradient_threshold);
    if (read != 1) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%shadow excentricity %f\n", &rd_parms->rock_elongate_ratio);
    if (read != 1) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "%%gamma_threshold_override %d\n", &rd_parms->gamma_threshold_override);
    if (read != 1) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    read = fscanf(fp_in, "id, tileR, tileC, shaX, shaY, rockX, rockY, tileShaX, tileShaY, shaArea, shaLen, rockWidth, rockHeight, score, gradMean, Compact, Exent, Class, gamma\n");
    if (read != 0) { printf("error reading file. quitting. \n"); fclose(fp_in); return FALSE; }

    //enforce file precision 
    rd_parms->ground_resolution = RoundToNearestTieToEven(rd_parms->ground_resolution, 6);
    rd_parms->gamma = RoundToNearestTieToEven(rd_parms->gamma, 6);
    rd_parms->sun_azimuth_angle = RoundToNearestTieToEven(rd_parms->sun_azimuth_angle, 6);
    rd_parms->sun_incidence_angle = RoundToNearestTieToEven(rd_parms->sun_incidence_angle, 6);
    rd_parms->min_shadow_size = RoundToNearestTieToEven(rd_parms->min_shadow_size, 6);
    rd_parms->max_shadow_size = RoundToNearestTieToEven(rd_parms->max_shadow_size, 6);
    rd_parms->confidence_threshold = RoundToNearestTieToEven(rd_parms->confidence_threshold, 6);
    rd_parms->min_shadow_size_split = RoundToNearestTieToEven(rd_parms->min_shadow_size_split, 6);
    rd_parms->mean_gradient_threshold = RoundToNearestTieToEven(rd_parms->mean_gradient_threshold, 6);
    rd_parms->rock_elongate_ratio = RoundToNearestTieToEven(rd_parms->rock_elongate_ratio, 6);

    return TRUE;
}


void rock_to_out_rock(int id, float elongate_ratio, float gamma, ROCK* rock, OUTROCK* out_rock)
{
    assert(rock->flag != 0);

    int hazClass = 1; //unused
    
    int start_col = tcols * rock->tile_col;
    int start_row = trows * rock->tile_row;

    double r = sqrt(rock->size / 3.1415926);
    double compact = r * 3.1415926 * 2 / rock->perimeter;

    double aratio = elongate_ratio;
    assert(rock->rock_minor >= 0);
    if (rock->rock_minor > 0)
    {
        aratio = rock->rock_major / rock->rock_minor;
    }

    out_rock->id = id;
    out_rock->tileR = rock->tile_row;
    out_rock->tileC = rock->tile_col;
    out_rock->shaX = RoundToNearestTieToEven(start_col + rock->x0, 1);
    out_rock->shaY = RoundToNearestTieToEven(start_row + rock->y0, 1);
    out_rock->rockX = RoundToNearestTieToEven(start_col + rock->start[0], 1);
    out_rock->rockY = RoundToNearestTieToEven(start_row + rock->start[1], 1);
    out_rock->tileShaX = RoundToNearestTieToEven(rock->x0, 1);
    out_rock->tileShaY = RoundToNearestTieToEven(rock->y0, 1);
    out_rock->shaArea = rock->size;
    out_rock->shaLen = RoundToNearestTieToEven(rock->shadowLength_modeled, 2);
    out_rock->rockWidth = RoundToNearestTieToEven(rock->rock_width_modeled, 2);
    out_rock->rockHeight = RoundToNearestTieToEven(rock->rock_height_modeled, 2);
    out_rock->score = RoundToNearestTieToEven(rock->confidence, 4);
    out_rock->gradMean = RoundToNearestTieToEven(rock->mean_g, 4);
    out_rock->Compact = RoundToNearestTieToEven((float)compact, 5);
    out_rock->Exent = RoundToNearestTieToEven((float)aratio, 5);
    out_rock->Class = hazClass;
    out_rock->gamma = RoundToNearestTieToEven(gamma, 4);
}


int write_tile_rocks(ROCK* rocks, int maxlab, RD_PARMS* rd_parms,
                   unsigned short* CC, int* gid,
                   FILE* fp_out, FILE* fp_shadowPixels)
{
    for (int m = 0; m < maxlab; ++m)
    {
        if (rocks[m].flag == 0)
            continue;

        int start_row = rocks[m].tile_row * trows;
        int start_col = rocks[m].tile_col * tcols;

        OUTROCK out_rock;
        rock_to_out_rock(*gid, rd_parms->rock_elongate_ratio, rd_parms->gamma, &rocks[m], &out_rock);

        fprintf(fp_out, "%8d, %3d, %3d, %7.1f, %7.1f, %7.1f, %7.1f, %6.1f, %6.1f, %5d, %6.2f, %6.2f, %6.2f, %6.4f, %6.4f, %6.5f, %6.5f, %3d, %6.4f\n",
                out_rock.id, out_rock.tileR, out_rock.tileC, out_rock.shaX, out_rock.shaY, out_rock.rockX, out_rock.rockY,
                out_rock.tileShaX, out_rock.tileShaY, out_rock.shaArea, out_rock.shaLen, out_rock.rockWidth,
                out_rock.rockHeight, out_rock.score, out_rock.gradMean, out_rock.Compact, out_rock.Exent, out_rock.Class,
                out_rock.gamma);

        //write shadow pixel file
        if (fp_shadowPixels != NULL)
        {
            fprintf(fp_shadowPixels, "%d: ", *gid);
            int pixelsMarked = 0;
            for (int idxRow = rocks[m].ymin; idxRow <= rocks[m].ymax; idxRow++)
            {
                for (int idxCol = rocks[m].xmin; idxCol <= rocks[m].xmax; idxCol++)
                {
                    if (CC[idxRow * tcols + idxCol] == m)
                    {
                        if (pixelsMarked != 0)
                            fprintf(fp_shadowPixels, ", ");

                        fprintf(fp_shadowPixels, "%d %d", idxCol + start_col, idxRow + start_row);
                        pixelsMarked++;
                    }
                }
            }
            fprintf(fp_shadowPixels, "\n");
        }

        *gid = *gid + 1;
    }

    return 1;
}

// rebuilds a partially complete parameters structure and a partially complete rocklist using the data contained in the rocklist file
//   it is not possible to rebuild a majority of the ROCK or RD_PARMS structure values from the data 
//   persisted into the rocklist file
int read_rocklist(char* rockListPath, RD_PARMS* rd_parms, OUTROCK** out_rocks, int* numRocks)
{
    char tmp[1024];

    //read params
    FILE* fp_in;
    fp_in = fopen(rockListPath, "r");
    if (fp_in == NULL)
    {
        snprintf(tmp, 1024, "cannot open the rock list %s to read\n", rockListPath);
        fputs(tmp, stderr);
        return FALSE;
    }

    if(FALSE == read_rocklist_header(fp_in,rd_parms))
    {
        snprintf(tmp, 1024, "cannot read the rock list header for %s\n", rockListPath);
        fputs(tmp, stderr);
        return FALSE;
    }

    //read rocklist
    int maxRocklistRocks = MAXLAB;
    *out_rocks = (OUTROCK*)malloc(sizeof(OUTROCK) * maxRocklistRocks);

    OUTROCK out_rock;

    *numRocks = 0;
    int read = 0;
    while (EOF != (read = fscanf(fp_in, "%d, %d, %d, %f, %f, %f, %f, %f, %f, %d, %f, %f, %f, %f, %f, %f, %f, %d, %f\n",
           &out_rock.id, &out_rock.tileR, &out_rock.tileC, &out_rock.shaX, &out_rock.shaY, &out_rock.rockX, &out_rock.rockY,
           &out_rock.tileShaX, &out_rock.tileShaY, &out_rock.shaArea, &out_rock.shaLen, &out_rock.rockWidth,
           &out_rock.rockHeight, &out_rock.score, &out_rock.gradMean, &out_rock.Compact, &out_rock.Exent, &out_rock.Class,
           &out_rock.gamma)))
    {
        if (read != 19)
        {
            printf("error reading file at rock %d. Quitting \n", *numRocks);
            free(*out_rocks);
            *out_rocks = NULL;
            fclose(fp_in);
            return FALSE;
        }

        if(*numRocks >= maxRocklistRocks)
        {
            maxRocklistRocks *= 2;

            OUTROCK* biggerRocks = (OUTROCK*)realloc(*out_rocks, sizeof(OUTROCK) * maxRocklistRocks);
            if (biggerRocks == NULL)
            {
                printf("error growing the size of the rocklist. Quitting \n");
                free(*out_rocks);
                *out_rocks = NULL;
                fclose(fp_in);
                return FALSE;
            }
            *out_rocks = biggerRocks;
        }

        //enforce file precision
        out_rock.shaX = RoundToNearestTieToEven(out_rock.shaX, 1);
        out_rock.shaY = RoundToNearestTieToEven(out_rock.shaY, 1);
        out_rock.rockX = RoundToNearestTieToEven(out_rock.rockX, 1);
        out_rock.rockY = RoundToNearestTieToEven(out_rock.rockY, 1);
        out_rock.tileShaX = RoundToNearestTieToEven(out_rock.tileShaX, 1);
        out_rock.tileShaY = RoundToNearestTieToEven(out_rock.tileShaY, 1);
        out_rock.shaLen = RoundToNearestTieToEven(out_rock.shaLen, 2);
        out_rock.rockWidth = RoundToNearestTieToEven(out_rock.rockWidth, 2);
        out_rock.rockHeight = RoundToNearestTieToEven(out_rock.rockHeight, 2);
        out_rock.score = RoundToNearestTieToEven(out_rock.score, 4);
        out_rock.gradMean = RoundToNearestTieToEven(out_rock.gradMean, 4);
        out_rock.Compact = RoundToNearestTieToEven(out_rock.Compact, 5);
        out_rock.Exent = RoundToNearestTieToEven(out_rock.Exent, 5);
        out_rock.gamma = RoundToNearestTieToEven(out_rock.gamma, 4);

        memcpy(&(*out_rocks)[*numRocks], &out_rock, sizeof(OUTROCK));
        *numRocks = *numRocks + 1;
    }

    fclose(fp_in);
    return TRUE;
}

int compare_rocklists(RD_PARMS* parms0, OUTROCK* rocks0, int numRocks0,
                      RD_PARMS* parms1, OUTROCK* rocks1, int numRocks1, int ignoreId)
{
    if (numRocks0 != numRocks1)
    {
        return FALSE;
    }

    if (0 != memcmp(parms0, parms1, sizeof(RD_PARMS)))
    {
        return FALSE;
    }

    int matched = 0;
    if (ignoreId)
    {
        for (int idxRock0 = 0; idxRock0 < numRocks0; idxRock0++)
        {
            int foundMatch = 0;
            for (int idxRock1 = 0; idxRock1 < numRocks1; idxRock1++)
            {
                if ((rocks0[idxRock0].tileC != rocks1[idxRock1].tileC) ||
                    (rocks0[idxRock0].tileR != rocks1[idxRock1].tileR))
                {
                    continue;
                }

                if ((rocks0[idxRock0].shaX == rocks1[idxRock1].shaX) &&
                    (rocks0[idxRock0].shaY == rocks1[idxRock1].shaY) &&
                    (rocks0[idxRock0].rockX == rocks1[idxRock1].rockX) &&
                    (rocks0[idxRock0].rockY == rocks1[idxRock1].rockY) &&
                    (rocks0[idxRock0].tileShaX == rocks1[idxRock1].tileShaX) &&
                    (rocks0[idxRock0].tileShaY == rocks1[idxRock1].tileShaY) &&
                    (rocks0[idxRock0].shaArea == rocks1[idxRock1].shaArea) &&
                    (rocks0[idxRock0].shaLen == rocks1[idxRock1].shaLen) &&
                    (rocks0[idxRock0].rockWidth == rocks1[idxRock1].rockWidth) &&
                    (rocks0[idxRock0].rockHeight == rocks1[idxRock1].rockHeight) &&
                    (rocks0[idxRock0].score == rocks1[idxRock1].score) &&
                    (rocks0[idxRock0].gradMean == rocks1[idxRock1].gradMean) &&
                    (rocks0[idxRock0].Compact == rocks1[idxRock1].Compact) &&
                    (rocks0[idxRock0].Exent == rocks1[idxRock1].Exent) &&
                    (rocks0[idxRock0].Class == rocks1[idxRock1].Class) &&
                    (rocks0[idxRock0].gamma == rocks1[idxRock1].gamma))
                {
                    foundMatch = 1;
                    break;
                }
            }

            if (foundMatch == 1)
                matched++;
            }
        }
    else
    {
    for (int idxRock0 = 0; idxRock0 < numRocks0; idxRock0++)
    {
        for (int idxRock1 = 0; idxRock1 < numRocks1; idxRock1++)
        {
            if (0 == memcmp(&rocks0[idxRock0], &rocks1[idxRock1], sizeof(OUTROCK)))
            {
                matched++;
                break;
            }
        }
    }
    }

    if (matched != numRocks0)
    {
        return FALSE;
    }

    return TRUE;
}

//returns whether two rocklists match
int compare_rocklist_files(char* rocklist0, char* rocklist1, int ignoreId)
{
    char tmp[1024];

    int numRocks0 = 0;
    OUTROCK* rocks0 = NULL;
    RD_PARMS parms0;
    memset(&parms0, 0, sizeof(RD_PARMS));
    if (FALSE == read_rocklist(rocklist0, &parms0, &rocks0, &numRocks0))
    {
        snprintf(tmp, 1024, "Failed to read rocklist %s\n", rocklist0);
        fputs(tmp, stderr);
        free(rocks0);
        return FALSE;
    }

    int numRocks1 = 0;
    OUTROCK* rocks1 = NULL;
    RD_PARMS parms1;
    memset(&parms1, 0, sizeof(RD_PARMS));
    if (FALSE == read_rocklist(rocklist1, &parms1, &rocks1, &numRocks1))
    {
        snprintf(tmp, 1024, "Failed to read rocklist %s\n", rocklist1);
        fputs(tmp, stderr);
        free(rocks0);
        free(rocks1);
        return FALSE;
    }

    int result = compare_rocklists(&parms0, rocks0, numRocks0,
                             &parms1, rocks1, numRocks1, ignoreId);

    free(rocks0);
    free(rocks1);
    return result;
}

int detect_tile(char* inputImagePath, unsigned short* labelImg, unsigned char* subimg,
                int tRow, int tCol, int frows, int fcols, int nbr, int nbc,
                RD_PARMS* params, ROCK* rocks, int* maxlab)
{
    int start_col = tCol * tcols;
    int end_col = start_col + tcols + forcedOverlap - 1;
    int start_row = tRow * trows;
    int end_row = start_row + trows + forcedOverlap - 1;

    if (start_col < 0) start_col = 0;
    if (start_row < 0) start_row = 0;
    if (end_row >= frows) end_row = frows - 1;
    if (end_col >= fcols) end_col = fcols - 1;
    int cols = end_col - start_col + 1;
    int rows = end_row - start_row + 1;

    if (0 == read_pgm_image_rect(inputImagePath, subimg, start_row, start_col, rows, cols))
    {
        printf("failed to read sub-image (row, col) (%d,%d)\n", tRow, tCol);
        return 0;
    }

    for (int m = 0; m < MAXLAB; m++)
    {
        clear_rock(&rocks[m]);
    }

    *maxlab = 0;
    if (0 == RockDetection(subimg, cols, rows, *params, rocks, maxlab, labelImg))
    {
        printf("rock detection failed, sub-image (row, col) (%d,%d)\n", tRow, tCol);
        return 0;
    }

    for (int m = 0; m < *maxlab; ++m)
    {
        rocks[m].tile_col = tCol;
        rocks[m].tile_row = tRow;
    }

    for (int m = 0; m < *maxlab; ++m)
    {
        if (rocks[m].flag == 0)
            continue;

        //remove anything in 0-50 row except these cross row 50
        if (tRow > 0 && rocks[m].ymax < forcedOverlap - 1)
        {
            rocks[m].flag = 0;
        }
        //remove anything in 0-50 col except these cross col 50
        else if (tCol > 0 && rocks[m].xmax < forcedOverlap - 1)
        {
            rocks[m].flag = 0;
        }
        //remove any rock connected to the bottom row
        else if (tRow < nbr - 1 && rocks[m].ymax >= rows - 1)
        {
            rocks[m].flag = 0;
        }
        //remove any rock connected to the right col
        else if (tCol < nbc - 1 && rocks[m].xmax >= cols - 1)
        {
            rocks[m].flag = 0;
        }
    }

    return 1;
}


int detect_per_tile_settings(char* inputImagePath, char* outputRockListPath, int numSettings, RD_PARMS* settingsArray)
{
    if (settingsArray == NULL)
    {
        return 0;
    }

    if (outputRockListPath == NULL)
        return 0;

    char tmp[1024];
    FILE* fp_out = fopen(outputRockListPath, "w");
    if (fp_out == NULL)
    {
        snprintf(tmp, 1024, "cannot open the rkfile %s to write\n", outputRockListPath);
        fputs(tmp, stderr);
        return 0;
    }

    RD_PARMS params;
    params.gamma = 0.0f;
    params.sun_incidence_angle = 0.0f;
    params.sun_azimuth_angle = 0.0f;
    params.min_shadow_size = 0.0f;
    params.ground_resolution = 0.0f;
    params.confidence_threshold = 0.0f;
    params.min_shadow_size_split = 0.0f;
    params.spliting_ratio = 0.0f;
    params.rock_elongate_ratio = 0.0f;
    params.mean_gradient_threshold = 0.0f;
    params.max_shadow_size = 0.0f;
    params.gamma_threshold_override = 0;

    if (numSettings > 0) {
        RD_PARMS* first = NULL;
        int all_same = 1;
        for (int i = 0; i < numSettings; i++) {
            RD_PARMS* p = &settingsArray[i];
            if (p->max_shadow_size <= 0) continue;
            if (first == NULL) first = p;
            else if (p->gamma != first->gamma ||
                p->sun_incidence_angle != first->sun_incidence_angle ||
                p->sun_azimuth_angle != first->sun_azimuth_angle ||
                p->min_shadow_size != first->min_shadow_size ||
                p->ground_resolution != first->ground_resolution ||
                p->confidence_threshold != first->confidence_threshold ||
                p->min_shadow_size_split != first->min_shadow_size_split ||
                p->spliting_ratio != first->spliting_ratio ||
                p->rock_elongate_ratio != first->rock_elongate_ratio ||
                p->mean_gradient_threshold != first->mean_gradient_threshold ||
                p->max_shadow_size != first->max_shadow_size ||
                p->gamma_threshold_override != first->gamma_threshold_override) {
                // this tile has different settings per tile, can't write a meaningful header
                all_same = 0;
                break;
            }
        }
        if (all_same && first != NULL) {
            printf("All tiles have same settings.\n");
            params = *first;
        }
    }

    write_rocklist_header(fp_out, &params);

    //intialize rock pixel output file
    char outputPixelPath[1024];
    snprintf(outputPixelPath, 1024, "%s.ShadowPixels.txt", outputRockListPath);
    FILE* outputPixelFile = fopen(outputPixelPath, "w");
    if (outputPixelFile == NULL)
    {
        snprintf(tmp, 1024, "Error opening the output pixel file %s to write\n", outputPixelPath);
        fputs(tmp, stderr);
        return 0;
    }

    if (VERBOSE)
    {
        snprintf(tmp, 1024, "Reading the Scene image... %s.\n", inputImagePath);
        fputs(tmp, stdout);
    }

    int fcols, frows;
    if (read_pgm_image_dimensions(inputImagePath, &frows, &fcols) == 0)
    {
        snprintf(tmp, 1024, "Error reading input image, %s.\n", inputImagePath);
        fputs(tmp, stderr);
        return 0;
    }

    int nbc = fcols / tcols;
    int nbr = frows / trows;

    if (nbc * tcols < fcols) nbc += 1;
    if (nbr * trows < frows) nbr += 1;

    printf("Image size (rows, cols) (%d, %d) pixels, (%d, %d) tiles, tile size (%d, %d)\n",
           frows, fcols, nbr, nbc, trows, tcols);

    if (numSettings != nbc * nbr)
    {
        snprintf(tmp, 1024, "Error: incorrect number of tile settings for image %s, expected %d, got %d.\n",
                 inputImagePath, nbc * nbr, numSettings);
        fputs(tmp, stderr);
        return 0;
    }
    int gid = 1;
    int completed_trows = 0;

    static ROCK* rocks = NULL;
#pragma omp threadprivate(rocks)
    static unsigned char* subimg = NULL;
#pragma omp threadprivate(subimg)
    static unsigned short* labelImg = NULL;
#pragma omp threadprivate(labelImg)

    int tRow = 0;
    int success = 1;
#pragma omp parallel for  //num_threads(1) //single threaded
    for (tRow = 0; tRow < nbr; tRow++)
    {
        if (subimg == NULL)
        {
            subimg = (unsigned char*)malloc(sizeof(char) * (tcols + forcedOverlap) * (trows + forcedOverlap));
        }

        if (rocks == NULL)
        {
            rocks = (ROCK*)malloc(sizeof(ROCK) * (MAXLAB));
        }

        if (labelImg == NULL)
        {
            labelImg = (unsigned short*)malloc(sizeof(unsigned short) * (tcols + forcedOverlap) * (trows + forcedOverlap));
        }

        for (int tCol = 0; tCol < nbc; tCol++)
        {
            int tileIndex = tRow * nbc + tCol;
            RD_PARMS* curParms = &settingsArray[tileIndex];
            if (curParms->max_shadow_size <= 0) continue;
           
            int maxlab = 0;
            int tileSuccess = 0;
            if (0 == (tileSuccess = detect_tile(inputImagePath, labelImg, subimg, tRow, tCol, frows, fcols,
                nbr, nbc, curParms, rocks, &maxlab)))
            {
                printf("failed to detect tile %d %d (row, col)\n", tRow, tCol);
                continue;
            }

#pragma omp critical
            {
                success |= tileSuccess;
                //save rocks to rocklist file
                write_tile_rocks(rocks, maxlab, curParms, labelImg, &gid,
                                 fp_out, outputPixelFile);
            }

        }

        free(labelImg);
        labelImg = NULL;

        free(rocks);
        rocks = NULL;

        free(subimg);
        subimg = NULL;

#pragma omp atomic
        completed_trows++;

        int cur_progress = (int)(completed_trows / (float)nbr * 100) % 10;
        if (cur_progress == 0)
        {
            printf("Completed: %d%%\n", (int)(completed_trows / (float)nbr * 100));
        }
    }

    fclose(fp_out);
    fclose(outputPixelFile);

    printf("Completed: %d%%\n", 100);
    snprintf(tmp, 1024, "Saved rock list to %s\n", outputRockListPath);
    fputs(tmp, stdout);
    snprintf(tmp, 1024, "Saved rock pixels to %s\n", outputPixelPath);
    fputs(tmp, stdout);
    fflush(stdout);

    return success;
}


int detect_tile_rocks(char* inputImagePath, unsigned short* labelImg, unsigned char* subimg,
                int tRow, int tCol, int frows, int fcols, int nbr, int nbc,
                RD_PARMS* params, OUTROCK* outRocks, int* maxlab)
{
    ROCK* rocks = (ROCK*)malloc(sizeof(ROCK) * MAXLAB);
    int detectedRocks = 0;
    int success = detect_tile(inputImagePath, labelImg, subimg,
                              tRow, tCol, frows, fcols, nbr, nbc,
                              params, rocks, &detectedRocks);

    *maxlab = 0;
    if (success)
    {
        for (int idx = 0; idx < MAXLAB; idx++)
        {
            if (rocks[idx].flag == 0)
                continue;

            //idx is original label
            rock_to_out_rock(idx, params->rock_elongate_ratio, params->gamma, &(rocks[idx]), &(outRocks[*maxlab]));

            *maxlab = *maxlab + 1;
        }
    }
    
    free(rocks);
    return success;
}

//BUGBUG: caller must free memory, get rid of all ptrs and have caller alloc
int detect(char* inputImagePath, RD_PARMS* params, OUTROCK** all_rocks, int* num_out_rocks )
{
    char tmp[1024];

    if (VERBOSE)
    {
        snprintf(tmp, 1024, "Reading the Scene image... %s.\n", inputImagePath);
        fputs(tmp, stdout);
    }

    int fcols, frows;
    if (read_pgm_image_dimensions(inputImagePath, &frows, &fcols) == 0)
    {
        snprintf(tmp, 1024, "Error reading input image, %s.\n", inputImagePath);
        fputs(tmp, stderr);
        return 0;
    }

    int nbc = fcols / tcols;
    int nbr = frows / trows;

    if (nbc * tcols < fcols) nbc += 1;
    if (nbr * trows < frows) nbr += 1;
    
    printf("Image size (rows, cols) (%d, %d) pixels, (%d, %d) tiles, tile size (%d, %d)\n",
           frows, fcols, nbr, nbc, trows, tcols);

    int completed_trows = 0;

    *num_out_rocks = 0;
    int maxOutRocks = (MAXLAB)*nbc * nbr;
    *all_rocks = (OUTROCK*)malloc(sizeof(OUTROCK) * maxOutRocks);

    static ROCK* rocks = NULL;
#pragma omp threadprivate(rocks)
    static unsigned char* subimg = NULL;
#pragma omp threadprivate(subimg)
    static unsigned short* labelImg = NULL;
#pragma omp threadprivate(labelImg)

    int tRow = 0;
    int success = 1;
#pragma omp parallel for  //num_threads(1) //single threaded
    for (tRow = 0; tRow < nbr; tRow++)
    {
        if (subimg == NULL)
        {
            subimg = (unsigned char*)malloc(sizeof(char) * (tcols + forcedOverlap) * (trows + forcedOverlap));
        }

        if (rocks == NULL)
        {
            rocks = (ROCK*)malloc(sizeof(ROCK) * (MAXLAB));
        }

        if (labelImg == NULL)
        {
            labelImg = (unsigned short*)malloc(sizeof(unsigned short) * (tcols + forcedOverlap) * (trows + forcedOverlap));
        }

        for (int tCol = 0; tCol < nbc; tCol++)
        {
            int maxlab = 0;
            int tileSuccess = 0;
            if (0 == (tileSuccess = detect_tile(inputImagePath, labelImg, subimg, tRow, tCol, frows, fcols,
                nbr, nbc, params, rocks, &maxlab)))
            {
                printf("failed to detect tile %d %d (row, col)\n", tRow, tCol);
            }

#pragma omp critical
            {
                success |= tileSuccess;
                for (int m = 0; m < maxlab; ++m)
                {
                    if (rocks[m].flag != 0)
                    {
                        //simulate writing to file and back to match file detections
                        rock_to_out_rock(*num_out_rocks, params->rock_elongate_ratio, params->gamma, &rocks[m], &((*all_rocks)[*num_out_rocks]));
                        (*num_out_rocks)++;
                        assert(*num_out_rocks < maxOutRocks); //would need to realloc
                    }
                }
            }
        }

        free(labelImg);
        labelImg = NULL;

        free(rocks);
        rocks = NULL;

        free(subimg);
        subimg = NULL;

#pragma omp atomic
        completed_trows++;

        int cur_progress = (int)(completed_trows / (float)nbr * 100) % 10;
        if (cur_progress == 0)
        {
            printf("Completed: %d%%\n", (int)(completed_trows / (float)nbr * 100));
        }
    }
    
    printf("Completed: %d%%\n", 100);
    fflush(stdout);

    return success;
}

int gamma_from_files(char* inputImagePath, float gamma, char* outputGammaPath)
{
    if (inputImagePath == NULL)
        return 0;

    if (outputGammaPath == NULL)
        return 0;

    int fcols, frows;
    if (read_pgm_image_dimensions(inputImagePath, &frows, &fcols) == 0)
    {
        char tmp[1024];
        snprintf(tmp, 1024, "Error reading input image, %s.\n", inputImagePath);
        fputs(tmp, stderr);
        return 0;
    }

    unsigned char* SceneImg = (unsigned char*)malloc(sizeof(char) * (fcols) * (frows));
   
    if (0 == read_pgm_image_rect(inputImagePath, SceneImg, 0, 0, frows, fcols))
    {
        printf("failed to read source image");
        free(SceneImg);
        return 0;
    }

    unsigned char* GammaImg = (unsigned char*)malloc(sizeof(char) * (fcols) * (frows));

    get_gamma_image(SceneImg, GammaImg, frows, fcols, gamma);

    const int bitsPerPixel = 8;
    const int maxValue = (1 << bitsPerPixel) - 1;
    if (0 == write_pgm_image(outputGammaPath, GammaImg, frows, fcols, "", maxValue))
    {
        printf("failed to write output image");
        free(SceneImg);
        free(GammaImg);
        return 0;
    }

    free(SceneImg);
    free(GammaImg);

    return 1;

}

int threshold_from_files(char* inputImagePath, float gamma, int thresholdOverride, char* outputShadowPath, int* selectedThreshold)
{
    if (inputImagePath == NULL)
        return 0;

    int fcols, frows;
    if (read_pgm_image_dimensions(inputImagePath, &frows, &fcols) == 0)
    {
        char tmp[1024];
        snprintf(tmp, 1024, "Error reading input image, %s.\n", inputImagePath);
        fputs(tmp, stderr);
        return 0;
    }

    unsigned char* SceneImg = (unsigned char*)malloc(sizeof(char) * (fcols) * (frows));

    if (0 == read_pgm_image_rect(inputImagePath, SceneImg, 0, 0, frows, fcols))
    {
        printf("failed to read source image");
        free(SceneImg);
        return 0;
    }

    unsigned char* ShaImg = (unsigned char*)malloc(sizeof(char) * (fcols) * (frows));

    *selectedThreshold = gMET_shadows(SceneImg, ShaImg, frows, fcols, gamma, thresholdOverride);
      
    const int bitsPerPixel = 8;
    const int maxValue = (1 << bitsPerPixel) - 1;
    if (0 == write_pgm_image(outputShadowPath, ShaImg, frows, fcols, "", maxValue))
    {
        printf("failed to write output image");
        free(SceneImg);
        free(ShaImg);
        return 0;
    }

    free(SceneImg);
    free(ShaImg);

    return 1;
}

int shadows_from_files(char* inputThresholdImagePath, char* outputShadowsImagePath, int* outNumShadows)
{
    if (inputThresholdImagePath == NULL)
        return 0;

    int fcols, frows;
    if (read_pgm_image_dimensions(inputThresholdImagePath, &frows, &fcols) == 0)
    {
        char tmp[1024];
        snprintf(tmp, 1024, "Error reading input image, %s.\n", inputThresholdImagePath);
        fputs(tmp, stderr);
        return 0;
    }

    unsigned char* threshImg = (unsigned char*)malloc(sizeof(char) * (fcols) * (frows));

    if (0 == read_pgm_image_rect(inputThresholdImagePath, threshImg, 0, 0, frows, fcols))
    {
        printf("failed to read source image");
        free(threshImg);
        return 0;
    }

    unsigned short* labelImg = (unsigned short*)malloc(sizeof(unsigned short) * (fcols) * (frows));

    //make shadow areas 1, non-shadow areas 0
    invert_binary_image(threshImg, frows, fcols);

    //initialize label image
    if (0 == LabelConnectedRegions(threshImg, frows, fcols, labelImg, outNumShadows))
    {
        printf("Labelling failed");
        return 0;
    }

    //DEBUG CODE
    int imagesize = frows * fcols;
    for (int i = 0; i < imagesize; ++i)
    {
        if (threshImg[i] == 0 && labelImg[i] != 0)
        {
            labelImg[i] = 0;
            return 0;
        }
    }

    int testMaxlab = 0;
    for (int i = 0; i < imagesize; ++i)
    {
        if (labelImg[i] > testMaxlab) testMaxlab = labelImg[i];
    }

    if (testMaxlab != *outNumShadows)
    {
        printf("what!");
        return 0;
    }
    //END DEBUG CODE

    if (*outNumShadows > MAXLAB)
    {
        printf("too many rocks to store in the buffer\n");   
        free(threshImg);
        free(labelImg);
        return 0;
    }

    const int bitsPerPixel = 16;
    const int maxValue = (1 << bitsPerPixel) - 1;
    if (0 == write_pgm_image(outputShadowsImagePath, (unsigned char*)labelImg, frows, fcols, "", maxValue))
    {
        printf("failed to write output image");
        free(threshImg);
        free(labelImg);
        return 0;
    }

    free(threshImg);
    free(labelImg);

    return 1;
}

int detect_from_files(char* inputImagePath, char* paramsPath, char* outputRockListPath)
{
    RD_PARMS rd_parms;
    if (read_param_file(paramsPath, &rd_parms) == 0)
    {
        return 0;
    }

    if (outputRockListPath == NULL)
        return 0;

    char tmp[1024];
    FILE* fp_out = fopen(outputRockListPath, "w");
    if (fp_out == NULL)
    {
        snprintf(tmp, 1024, "cannot open the rkfile %s to write\n", outputRockListPath);
        fputs(tmp, stderr);
        return 0;
    }

    write_rocklist_header(fp_out, &rd_parms);

    //intialize rock pixel output file
    char outputPixelPath[1024];
    snprintf(outputPixelPath, 1024,"%s.ShadowPixels.txt", outputRockListPath);
    FILE* outputPixelFile = fopen(outputPixelPath, "w");
    if (outputPixelFile == NULL)
    {
        snprintf(tmp, 1024, "Error opening the output pixel file %s to write\n", outputPixelPath);
        fputs(tmp, stderr);
        return 0;
    }

    if (VERBOSE)
    {
        snprintf(tmp, 1024, "Reading the Scene image... %s.\n", inputImagePath);
        fputs(tmp, stdout);
    }
    
    int fcols, frows;
    if (read_pgm_image_dimensions(inputImagePath, &frows, &fcols) == 0)
    {
        snprintf(tmp, 1024, "Error reading input image, %s.\n", inputImagePath);
        fputs(tmp, stderr);
        return 0;
    }

    int nbc = fcols / tcols;
    int nbr = frows / trows;

    if (nbc * tcols < fcols) nbc += 1;
    if (nbr * trows < frows) nbr += 1;

    printf("Image size (rows, cols) (%d, %d) pixels, (%d, %d) tiles, tile size (%d, %d)\n",
           frows, fcols, nbr, nbc, trows, tcols);


    int gid = 1;
    int completed_trows = 0;

    static ROCK* rocks = NULL;
#pragma omp threadprivate(rocks)
    static unsigned char* subimg = NULL;
#pragma omp threadprivate(subimg)
    static unsigned short* labelImg = NULL;
#pragma omp threadprivate(labelImg)

    int tRow = 0;
    int success = 1;
#pragma omp parallel for  //num_threads(1) //single threaded
    for (tRow = 0; tRow < nbr; tRow++)
    {
        if (subimg == NULL)
        {
            subimg = (unsigned char*)malloc(sizeof(char) * (tcols + forcedOverlap) * (trows + forcedOverlap));
        }

        if (rocks == NULL)
        {
            rocks = (ROCK*)malloc(sizeof(ROCK) * (MAXLAB));
        }

        if (labelImg == NULL)
        {
            labelImg = (unsigned short*)malloc(sizeof(unsigned short) * (tcols + forcedOverlap) * (trows + forcedOverlap));
        }

        for (int tCol = 0; tCol < nbc; tCol++)
        {
            int maxlab = 0;
            int tileSuccess = 0;
            if (0 == (tileSuccess = detect_tile(inputImagePath, labelImg, subimg, tRow, tCol, frows, fcols,
                nbr, nbc, &rd_parms, rocks, &maxlab)))
            {
                printf("failed to detect tile %d %d (row, col)\n", tRow, tCol);
                continue;
            }

#pragma omp critical
            {
                success |= tileSuccess;
                //save rocks to rocklist file
                write_tile_rocks(rocks, maxlab, &rd_parms, labelImg, &gid,
                               fp_out, outputPixelFile);
            }

        }

        free(labelImg);
        labelImg = NULL;

        free(rocks);
        rocks = NULL;

        free(subimg);
        subimg = NULL;

#pragma omp atomic
        completed_trows++;

        int cur_progress = (int)(completed_trows / (float)nbr * 100) % 10;
        if (cur_progress == 0)
        {
            printf("Completed: %d%%\n", (int)(completed_trows / (float)nbr * 100));
        }
    }

    fclose(fp_out);
    fclose(outputPixelFile);

    printf("Completed: %d%%\n", 100);
    snprintf(tmp, 1024, "Saved rock list to %s\n", outputRockListPath);
    fputs(tmp, stdout);
    snprintf(tmp, 1024, "Saved rock pixels to %s\n", outputPixelPath);
    fputs(tmp, stdout);
    fflush(stdout);

    return success;
}

int write_param_file(char* filename, RD_PARMS* src)
{
    if (filename == NULL)
    {
        printf("missing param filename");
        return 0;
    }

    char tmp[1024];
    FILE* fp;
    fp = fopen(filename, "w");
    if (fp == NULL)
    {
        snprintf(tmp, 1024, "cannot open the parameterfile: %s\n", filename);
        fputs(tmp, stderr);
        fclose(fp);
        return 0;
    }

    snprintf(tmp, 1024, "%s %f\n", "rd_gamma", src->gamma);
    int err = fputs(tmp, fp);
    if (err < 0)
    {
        snprintf(tmp, 1024, "cannot write the parameterfile: %s\n", filename);
        fputs(tmp, stderr);
        fclose(fp);
        return 0;
    }

    snprintf(tmp, 1024, "%s %f\n", "rd_sun_incidence_angle", src->sun_incidence_angle);
    err = fputs(tmp, fp);
    if (err < 0)
    {
        snprintf(tmp, 1024, "cannot write the parameterfile: %s\n", filename);
        fputs(tmp, stderr);
        fclose(fp);
        return 0;
    }

    snprintf(tmp, 1024, "%s %f\n", "rd_sun_azimuth_angle", src->sun_azimuth_angle);
    err = fputs(tmp, fp);
    if (err < 0)
    {
        snprintf(tmp, 1024, "cannot write the parameterfile: %s\n", filename);
        fputs(tmp, stderr);
        fclose(fp);
        return 0;
    }

    snprintf(tmp, 1024, "%s %f\n", "rd_min_shadow_size", src->min_shadow_size);
    err = fputs(tmp, fp);
    if (err < 0)
    {
        snprintf(tmp, 1024, "cannot write the parameterfile: %s\n", filename);
        fputs(tmp, stderr);
        fclose(fp);
        return 0;
    }
    
    snprintf(tmp, 1024, "%s %f\n", "rd_max_shadow_size", src->max_shadow_size);
    err = fputs(tmp, fp);
    if (err < 0)
    {
        snprintf(tmp, 1024, "cannot write the parameterfile: %s\n", filename);
        fputs(tmp, stderr);
        fclose(fp);
        return 0;
    }
      
    snprintf(tmp, 1024, "%s %f\n", "rd_ground_resolution", src->ground_resolution);
    err = fputs(tmp, fp);
    if (err < 0)
    {
        snprintf(tmp, 1024, "cannot write the parameterfile: %s\n", filename);
        fputs(tmp, stderr);
        fclose(fp);
        return 0;
    }
       
    snprintf(tmp, 1024, "%s %f\n", "rd_confidence_threshold", src->confidence_threshold);
    err = fputs(tmp, fp);
    if (err < 0)
    {
        snprintf(tmp, 1024, "cannot write the parameterfile: %s\n", filename);
        fputs(tmp, stderr);
        fclose(fp);
        return 0;
    }

    snprintf(tmp, 1024, "%s %f\n", "rd_min_split_shadow_size", src->min_shadow_size_split);
    err = fputs(tmp, fp);
    if (err < 0)
    {
        snprintf(tmp, 1024, "cannot write the parameterfile: %s\n", filename);
        fputs(tmp, stderr);
        fclose(fp);
        return 0;
    }

    snprintf(tmp, 1024, "%s %f\n", "rd_spliting_ratio", src->spliting_ratio);
    err = fputs(tmp, fp);
    if (err < 0)
    {
        snprintf(tmp, 1024, "cannot write the parameterfile: %s\n", filename);
        fputs(tmp, stderr);
        fclose(fp);
        return 0;
    }
                
    snprintf(tmp, 1024, "%s %f\n", "rd_rock_elongate_ratio", src->rock_elongate_ratio);
    err = fputs(tmp, fp);
    if (err < 0)
    {
        snprintf(tmp, 1024, "cannot write the parameterfile: %s\n", filename);
        fputs(tmp, stderr);
        fclose(fp);
        return 0;
    }
      
    snprintf(tmp, 1024, "%s %f\n", "rd_mean_gradient_threshold", src->mean_gradient_threshold);
    err = fputs(tmp, fp);
    if (err < 0)
    {
        snprintf(tmp, 1024, "cannot write the parameterfile: %s\n", filename);
        fputs(tmp, stderr);
        fclose(fp);
        return 0;
    }
    
    snprintf(tmp, 1024, "%s %d\n", "rd_gamma_threshold_override", src->gamma_threshold_override);
    err = fputs(tmp, fp);
    if (err < 0)
    {
        snprintf(tmp, 1024, "cannot write the parameterfile: %s\n", filename);
        fputs(tmp, stderr);
        fclose(fp);
        return 0;
    }

    fclose(fp);
    return 1;

}

static int parse_string_value(const char* filename,
                              int line_no,
                              const char* key_str,
                              char* in_value_str,
                              char** return_value_str_ptr) {
    char* p = in_value_str;
    while (isspace(*p)) {
        ++p;
    }
    *return_value_str_ptr = p;
    char* end_ptr = p;
    while (!isspace(*end_ptr)) {
        ++end_ptr;
    }
    *end_ptr = '\0';
    if (end_ptr == p) {
        char tmp[1024];
        snprintf(tmp, 1024, "error %s+%d: key (%s) requires a string argument\n", filename, line_no, key_str);
        fputs(tmp, stderr);
        return 0;
    }
    return 1;
}

static int parse_float_value(const char* filename,
                             int line_no,
                             const char* key_str,
                             char* value_str,
                             int value_arr_count,
                             float value_arr[]) {
    char tmp[1024];
    for (int ii = 0; ii < value_arr_count; ++ii) {
        char* end_ptr;
        value_arr[ii] = strtof(value_str, &end_ptr);
        if (end_ptr == value_str) {
            // No conversion performed.
            if (value_arr_count == 1) {
                snprintf(tmp, 1024, "error %s+%d: key (%s) requires a float argument; saw (%s)\n",
                         filename, line_no, key_str, value_str);
                fputs(tmp, stderr);
            } else {
                snprintf(tmp, 1024, "error %s+%d: key (%s) requires %d float arguments; saw (%s)\n",
                         filename, line_no, key_str, value_arr_count, value_str);
                fputs(tmp, stderr);
            }
            return 0;
        }
        value_str = end_ptr;
    }
    return 1;
}

static int parse_int_value(const char* filename,
                           int line_no,
                           const char* key_str,
                           char* value_str,
                           int value_arr_count,
                           int value_arr[]) {
    char tmp[1024];
    for (int ii = 0; ii < value_arr_count; ++ii) {
        char* end_ptr;
        value_arr[ii] = strtol(value_str, &end_ptr, 0);
        if (end_ptr == value_str) {
            // No conversion performed.
            if (value_arr_count == 1) {
                snprintf(tmp, 1024, "error %s+%d: key (%s) requires an int argument; saw (%s)\n",
                         filename, line_no, key_str, value_str);
                fputs(tmp, stderr);
            } else {
                snprintf(tmp, 1024, "error %s+%d: key (%s) requires %d int arguments; saw (%s)\n",
                         filename, line_no, key_str, value_arr_count, value_str);
                fputs(tmp, stderr);
            }
            return 0;
        }
        value_str = end_ptr;
    }
    return 1;
}

//strtok_r() is POSIX and might not be defined on Windows
#ifndef strtok_r
char *strtok_r(char *str, const char *delim, char **saveptr)
{
    char *end;

    if (str == NULL) //not first call
    {
        str = *saveptr;
    }

    if (*str == '\0') //empty input
    {
        *saveptr = str;
        return NULL;
    }
    
    str += strspn(str, delim); //skip over any leading delimiters
    if (*str == '\0') //no non-delimeters found
    {
        *saveptr = str;
        return NULL;
    }

    end = str + strcspn(str, delim); //end = next delimiter or terminating null, whichever comes first
    if (*end == '\0') //no delimiters found
    {
      *saveptr = end;
      return str;
    }
    
    *end = '\0';
    *saveptr = end + 1;

    return str;
}
#endif

int read_param_file(char* filename, RD_PARMS* src)
{
    int return_value = 1;
    if (filename == NULL)
    {
        printf("missing param filename");
        return 0;
    }
    FILE* fp;
    char buf[256];
    char tmp[1024];
    fp = fopen(filename, "r");
    if (fp == NULL)
    {
        snprintf(tmp, 1024, "cannot open the parameterfile: %s\n", filename);
        fputs(tmp, stderr);
        return 0;
    }

    //clear struct
    src->gamma = 0.0f;
    src->sun_incidence_angle = 0.0f;
    src->sun_azimuth_angle = 0.0f;
    src->min_shadow_size = 0.0f;
    src->ground_resolution = 0.0f;
    src->confidence_threshold = 0.0f;
    src->min_shadow_size_split = 0.0f;
    src->spliting_ratio = RD_ROCK_SPLITING_RATIO; //initialize to default
    src->rock_elongate_ratio = 0.0f;
    src->mean_gradient_threshold = 0.0f;
    src->max_shadow_size = 0.0f;
    src->gamma_threshold_override = 0;

    int line_no = 0;
    while (fgets(buf, 256, fp))
    {
        ++line_no;
        /* DSC 20241031 Changed this because the commented out version
           matches "rd_gamma_threshold_override" (as well as "rd_gamm"a) and
           that breaks things.
        if (strstr(buf, "rd_gamma"))
        {
            if (2 != sscanf(buf, "%s %f", tmp1, &src->gamma))
            {
                snprintf(tmp, 1024, "cannot read the parameterfile: %s\n", filename);
                fputs(tmp, stderr);
                fclose(fp);
                return 0;
            }
        }
        */

        char* p = buf;
        // skip leading whilespace.
        while (isspace(*p)) {
            ++p;
        }
        if (*p == '#') {
            // Comment line, ignore
            continue;
        }
        char* value = NULL;
        char* key = strtok_r(p, " \t", &value);
        if (key == NULL) {
            // Blank line, ignore.
            continue;
        }
        if (strcmp(key, "rd_gamma") == 0) {
            if (parse_float_value(filename, line_no, key, value,
                                  1, &src->gamma) == 0) {
                return_value = 0;
                break;
            }
        }
        else if (strcmp(key, "rd_sun_incidence_angle") == 0) {
            if (parse_float_value(filename, line_no, key, value,
                                  1, &src->sun_incidence_angle) == 0) {
                return_value = 0;
                break;
            }
        }
        else if (strcmp(key, "rd_sun_azimuth_angle") == 0) {
            if (parse_float_value(filename, line_no, key, value,
                                  1, &src->sun_azimuth_angle) == 0) {
                return_value = 0;
                break;
            }
        }
        else if (strcmp(key, "rd_min_shadow_size") == 0) {
            if (parse_float_value(filename, line_no, key, value,
                                  1, &src->min_shadow_size) == 0) {
                return_value = 0;
                break;
            }
        }
        else if (strcmp(key, "rd_max_shadow_size") == 0) {
            if (parse_float_value(filename, line_no, key, value,
                                  1, &src->max_shadow_size) == 0) {
                return_value = 0;
                break;
            }
        }
        else if (strcmp(key, "rd_ground_resolution") == 0) {
            if (parse_float_value(filename, line_no, key, value,
                                  1, &src->ground_resolution) == 0) {
                return_value = 0;
                break;
            }
        }
        else if (strcmp(key, "rd_confidence_threshold") == 0) {
            if (parse_float_value(filename, line_no, key, value,
                                  1, &src->confidence_threshold) == 0) {
                return_value = 0;
                break;
            }
        }
        else if (strcmp(key, "rd_min_split_shadow_size") == 0) {
            if (parse_float_value(filename, line_no, key, value,
                                  1, &src->min_shadow_size_split) == 0) {
                return_value = 0;
                break;
            }
        }
        else if (strcmp(key, "rd_spliting_ratio") == 0) {
            if (parse_float_value(filename, line_no, key, value,
                                  1, &src->spliting_ratio) == 0) {
                return_value = 0;
                break;
            }
        }
        else if (strcmp(key, "rd_hazard_threshold") == 0) {
            float unused;
            if (parse_float_value(filename, line_no, key, value,
                                  1, &unused) == 0) {
                return_value = 0;
                break;
            }
        }
        else if (strcmp(key, "rd_rock_elongate_ratio") == 0) {
            if (parse_float_value(filename, line_no, key, value,
                                  1, &src->rock_elongate_ratio) == 0) {
                return_value = 0;
                break;
            }
        }
        else if (strcmp(key, "rd_save_rock_map") == 0) {
            int unused;
            if (parse_int_value(filename, line_no, key, value,
                                1, &unused) == 0) {
                return_value = 0;
                break;
            }
        }
        else if (strcmp(key, "rd_save_subset_images") == 0) {
            int unused;
            if (parse_int_value(filename, line_no, key, value,
                                1, &unused) == 0) {
                return_value = 0;
                break;
            }
        }
        else if (strcmp(key, "rd_save_image_rock_pairs") == 0) {
            int unused;
            if (parse_int_value(filename, line_no, key, value,
                                1, &unused) == 0) {
                return_value = 0;
                break;
            }
        }
        else if (strcmp(key, "rd_rockmapfile") == 0) {
            char* unused;
            if (parse_string_value(filename, line_no, key, value,
                                   &unused) == 0) {
                return_value = 0;
                break;
            }
        }
        else if (strcmp(key, "rd_mean_gradient_threshold") == 0) {
            if (parse_float_value(filename, line_no, key, value,
                                  1, &src->mean_gradient_threshold) == 0) {
                return_value = 0;
                break;
            }
        }
        else if (strcmp(key, "rd_test") == 0) {
            int unused;
            if (parse_int_value(filename, line_no, key, value,
                                1, &unused) == 0) {
                return_value = 0;
                break;
            }
        }
        else if (strcmp(key, "rd_gamma_threshold_override") == 0) {
            if (parse_int_value(filename, line_no, key, value,
                                1, &src->gamma_threshold_override) == 0) {
                return_value = 0;
                break;
            }
        }
        else if (strcmp(key, "rd_window_test_flag") == 0) {
            int unused;
            if (parse_int_value(filename, line_no, key, value,
                                1, &unused) == 0) {
                return_value = 0;
                break;
            }
        }
        else if (strcmp(key, "rd_window_test_range") == 0) {
            int unused[4];
            if (parse_int_value(filename, line_no, key, value,
                                4, unused) == 0) {
                return_value = 0;
                break;
            }
        }
        else {
            snprintf(tmp, 1024, "error %s+%d: unrecognized key (%s)\n", filename, line_no, key);
            fputs(tmp, stderr);
            fclose(fp);
            return 0;
        }
    }

    fclose(fp);
    return return_value;
}
