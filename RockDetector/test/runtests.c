#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include "../AH_RockModel.h"
#include "../AH_Rocks.h"
//#include "DoubleMatrix.h"
#include "../AH_RockUtils.h"
#include "../PGM.h"

void count_results(int success, int* successes, int* failures)
{
    if (success != 0)
    {
        *successes = *successes + 1;
    }
    else
    {
        *failures = *failures + 1;
    }
}

void print_test_label(char* testFile, char* testFunc, char* testName)
{
    char tmp[1024];
    snprintf(tmp, 1024, "\nRunning test %s: %s: %s:...", testFile, testFunc, testName);
    fputs(tmp, stdout);
}

void print_test_result(int result)
{
    if (result == 0)
        printf("FAILED");
    else
        printf("PASSED");
}

int main(int argc, char *argv[])
{
    printf("Running tests...\n");

    if(argc != 2)
    {
        printf("Error.\n Should be: RunTests <path to test directory>\n");
        return 1;
    }
    char* testDir = argv[1];

    char testSmallImage[1024];
    snprintf(testSmallImage, 1024, "%s/%s", testDir, "small.pgm");

    char testRocklist[1024];
    snprintf(testRocklist, 1024, "%s/%s", testDir, "ESP_046282_Rocks_north_bot.txt");

    char testImage[1024];
    snprintf(testImage, 1024, "%s/%s", testDir, "ESP_046282_north_botRst.pgm");

    char* testFile = NULL;
    char* testFunc = NULL;
    char* testName = NULL;
    int numPassed = 0;
    int numFailed = 0;

    testFile = "AH_Rocks.c";
    {
        testFunc = "gMET_shadows";
        {
            testName = "test binary input";
            print_test_label(testFile, testFunc, testName);

            int rows = 3;
            int cols = 3;
            float gamma = 1;
            unsigned char SceneImg[9] = { 0xFF, 0xFF, 0xFF,
                                          0xFF, 0x00, 0xFF,
                                          0xFF, 0xFF, 0xFF };

            unsigned char ShaImg[9];
            gMET_shadows(SceneImg, ShaImg, rows, cols, gamma, 0);

            int success = 1;
            for (int idx = 0; idx < 9; idx++)
            {
                if (ShaImg[idx] != SceneImg[idx])
                {
                    success = 0;
                    break;
                }
            }

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);
        }

        {
            testName = "test all white input";
            print_test_label(testFile, testFunc, testName);

            int rows = 3;
            int cols = 3;
            float gamma = 1;
            unsigned char SceneImg[9] = { 0xFF, 0xFF, 0xFF,
                                          0xFF, 0xFF, 0xFF,
                                          0xFF, 0xFF, 0xFF };

            unsigned char ShaImg[9];
            gMET_shadows(SceneImg, ShaImg, rows, cols, gamma, 0);

            int success = 1;
            for (int idx = 0; idx < 9; idx++)
            {
                if (ShaImg[idx] != SceneImg[idx])
                {
                    success = 0;
                    break;
                }
            }

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);
        }

        {
            testName = "test all gray input";
            print_test_label(testFile, testFunc, testName);

            int rows = 3;
            int cols = 3;
            float gamma = 1;
            unsigned char SceneImg[9] = { 0x80, 0x80, 0x80,
                                          0x80, 0x80, 0x80,
                                          0x80, 0x80, 0x80 };

            unsigned char ShaImg[9];
            gMET_shadows(SceneImg, ShaImg, rows, cols, gamma, 0);

            int success = 1;
            for (int idx = 0; idx < 9; idx++)
            {
                if (ShaImg[idx] != 0xFF)
                {
                    success = 0;
                    break;
                }
            }

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);
        }

        {
            testName = "test all black input";
            print_test_label(testFile, testFunc, testName);

            int rows = 3;
            int cols = 3;
            float gamma = 1;
            unsigned char SceneImg[9] = { 0, 0, 0,
                                          0, 0, 0,
                                          0, 0, 0 };

            unsigned char ShaImg[9];
            gMET_shadows(SceneImg, ShaImg, rows, cols, gamma, 0);

            int success = 1;
            for (int idx = 0; idx < 9; idx++)
            {
                if (ShaImg[idx] != 0xFF) //all black image results in no shadows due to gMet selected threshold
                {
                    success = 0;
                    break;
                }
            }

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);
        }

        {
            testName = "test mostly black input";
            print_test_label(testFile, testFunc, testName);

            int rows = 3;
            int cols = 3;
            float gamma = 1;
            unsigned char SceneImg[9] = { 0, 0, 0,
                                          0, 0, 0,
                                          0, 0, 1 };

            unsigned char ShaImg[9];
            gMET_shadows(SceneImg, ShaImg, rows, cols, gamma, 0);

            int success = 1;
            for (int idx = 0; idx < 9; idx++)
            {
                if (idx < 8)
                {
                    if (ShaImg[idx] != 0)
                    {
                        success = 0;
                        break;
                    }
                }
                else
                {
                    if (ShaImg[idx] != 0xFF)
                    {
                        success = 0;
                        break;
                    }
                }
            }

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);
        }

        testFunc = "get_binary_image";
        {
            testName = "test binary image";
            print_test_label(testFile, testFunc, testName);

            int rows = 3;
            int cols = 3;
            unsigned char SceneImg[9] = { 0, 0, 0,
                                          128, 128, 128,
                                          255, 255, 255 };

            unsigned char ThreshImg[9];
            get_binary_image(SceneImg, ThreshImg, rows, cols, 127);

            int success = 1;
            for (int idx = 0; idx < 9; idx++)
            {
                if (idx < 3 && ThreshImg[idx] != 0x0)
                {
                    success = 0;
                    break;
                }
                else if (idx >= 3 && ThreshImg[idx] != 0xFF)
                {
                    success = 0;
                    break;
                }
            }

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);
        }

        testFunc = "differentiate";
        {
            testName = "test differentiate x";
            print_test_label(testFile, testFunc, testName);

            int rows = 3;
            int cols = 3;
            unsigned char SceneImg[9] = { 0, 255, 0,
                                          0, 255, 0,
                                          0, 255, 0 };

            float gx[9];
            float gy[9];
            differentiate(SceneImg, gx, gy, rows, cols);

            int success = 1;
            if (gx[0] != 0) success = 0;
            if (gx[1] != 0) success = 0;
            if (gx[2] != 0) success = 0;
            if (gx[3] != 0) success = 0;
            if (gx[4] != 255) success = 0;
            if (gx[5] != -255) success = 0;
            if (gx[6] != 0) success = 0;
            if (gx[7] != 255) success = 0;
            if (gx[8] != -255) success = 0;

            if (gy[0] != 0) success = 0;
            if (gy[1] != 0) success = 0;
            if (gy[2] != 0) success = 0;
            if (gy[3] != 0) success = 0;
            if (gy[4] != 0) success = 0;
            if (gy[5] != 0) success = 0;
            if (gy[6] != 0) success = 0;
            if (gy[7] != 0) success = 0;
            if (gy[8] != 0) success = 0;

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);
        }

        {
            testName = "test differentiate y";
            print_test_label(testFile, testFunc, testName);

            int rows = 3;
            int cols = 3;
            unsigned char SceneImg[9] = { 0, 0, 0,
                                          255, 255, 255,
                                          0, 0, 0 };

            float gx[9];
            float gy[9];
            differentiate(SceneImg, gx, gy, rows, cols);

            int success = 1;
            if (gy[0] != 0) success = 0;
            if (gy[1] != 0) success = 0;
            if (gy[2] != 0) success = 0;
            if (gy[3] != 0) success = 0;
            if (gy[4] != 255) success = 0;
            if (gy[5] != 255) success = 0;
            if (gy[6] != 0) success = 0;
            if (gy[7] != -255) success = 0;
            if (gy[8] != -255) success = 0;

            if (gx[0] != 0) success = 0;
            if (gx[1] != 0) success = 0;
            if (gx[2] != 0) success = 0;
            if (gx[3] != 0) success = 0;
            if (gx[4] != 0) success = 0;
            if (gx[5] != 0) success = 0;
            if (gx[6] != 0) success = 0;
            if (gx[7] != 0) success = 0;
            if (gx[8] != 0) success = 0;

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);
        }
    }

    testFile = "AH_RockModel.c";
    {
        testFunc = "LabelConnectedRegions";
        {
            testName = "test simple label";
            print_test_label(testFile, testFunc, testName);

            int rows = 3;
            int cols = 3;
            unsigned char BinaryImg[9] = { 0, 0, 0,
                                          255, 255, 0,
                                          0, 0, 0 };
            unsigned short Labels[9];
            int numLabels = 0;

            int success = LabelConnectedRegions(BinaryImg, rows, cols, Labels, &numLabels);

            if (numLabels != 1)
            {
                success = 0;
            }

            for (int i = 0; i < 9; i++)
            {
                if (i == 3 || i == 4)
                {
                    if (Labels[i] != 1)
                    {
                        success = 0;
                    }
                }
                else
                {
                    if (Labels[i] != 0)
                    {
                        success = 0;
                    }
                }
            }

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);
        }

        {
            testName = "test two labels";
            print_test_label(testFile, testFunc, testName);

            int rows = 3;
            int cols = 3;
            unsigned char BinaryImg[9] = { 255, 255, 255,
                                          0, 255, 0,
                                          255, 0, 0 };
            unsigned short Labels[9];
            int numLabels = 0;
            int success = LabelConnectedRegions(BinaryImg, rows, cols, Labels, &numLabels);

            if (numLabels != 2)
            {
                success = 0;
            }

            for (int i = 0; i < 9; i++)
            {
                if (i == 0 || i == 1 || i == 2 || i == 4)
                {
                    if (Labels[i] != 1)
                    {
                        success = 0;
                    }
                }
                else if (i == 6)
                {
                    if (Labels[i] != 2)
                    {
                        success = 0;
                    }
                }
                else
                {
                    if (Labels[i] != 0)
                    {
                        success = 0;
                    }
                }
            }

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);
        }

        {
            testName = "label no region";
            print_test_label(testFile, testFunc, testName);
            int rows = 20;
            int cols = 20;
            unsigned char* inputBinaryImage = (unsigned char*)malloc(sizeof(unsigned char) * rows * cols);
            unsigned short* labelledImage = (unsigned short*)malloc(sizeof(unsigned short) * rows * cols);

            memset(inputBinaryImage, 0, rows * cols * sizeof(char));
            int numRegions = 0;
            int success = LabelConnectedRegions(inputBinaryImage, rows, cols, labelledImage, &numRegions);
            if (numRegions != 0)
            {
                success = 0;
            }

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);

            free(inputBinaryImage);
            free(labelledImage);
        }

        {
            testName = "label horiz strips";
            print_test_label(testFile, testFunc, testName);
            int rows = 20;
            int cols = 10;
            unsigned char* inputBinaryImage = (unsigned char*)malloc(sizeof(unsigned char) * rows * cols);
            unsigned short* labelledImage = (unsigned short*)malloc(sizeof(unsigned short) * rows * cols);

            for (int idxRow = 0; idxRow < rows; idxRow++)
            {
                int val = idxRow % 2;
                for (int idxCol = 0; idxCol < cols; idxCol++)
                {
                    inputBinaryImage[idxRow * cols + idxCol] = (unsigned char)val;
                }
            }

            int numRegions = 0;
            int success = LabelConnectedRegions(inputBinaryImage, rows, cols, labelledImage, &numRegions);

            if (numRegions != rows / 2)
            {
                success = 0;
            }

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);

            free(inputBinaryImage);
            free(labelledImage);
        }
        {
            testName = "label vert strips";
            print_test_label(testFile, testFunc, testName);
            int rows = 20;
            int cols = 20;
            unsigned char* inputBinaryImage = (unsigned char*)malloc(sizeof(unsigned char) * rows * cols);
            unsigned short* labelledImage = (unsigned short*)malloc(sizeof(unsigned short) * rows * cols);

            for (int idxRow = 0; idxRow < rows; idxRow++)
            {
                for (int idxCol = 0; idxCol < cols; idxCol++)
                {
                    inputBinaryImage[idxRow * cols + idxCol] = idxCol % 2;
                }
            }

            int numRegions = 0;
            int success = LabelConnectedRegions(inputBinaryImage, rows, cols, labelledImage, &numRegions);
            if (numRegions != cols / 2)
            {
                success = 0;
            }

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);

            free(inputBinaryImage);
            free(labelledImage);
        }
        {
            testName = "label grid";
            print_test_label(testFile, testFunc, testName);
            int rows = 20;
            int cols = 20;
            unsigned char* inputBinaryImage = (unsigned char*)malloc(sizeof(unsigned char) * rows * cols);
            unsigned short* labelledImage = (unsigned short*)malloc(sizeof(unsigned short) * rows * cols);

            for (int idxRow = 0; idxRow < rows; idxRow++)
            {
                int val = idxRow % 2;
                for (int idxCol = 0; idxCol < cols; idxCol++)
                {
                    inputBinaryImage[idxRow * cols + idxCol] = (unsigned char)( val | idxCol % 2);
                }
            }

            int numRegions = 0;
            int success = LabelConnectedRegions(inputBinaryImage, rows, cols, labelledImage, &numRegions);
            if (numRegions != 1)
            {
                success = 0;
            }

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);

            free(inputBinaryImage);
            free(labelledImage);
        }

        {
            testName = "label regular dots";
            print_test_label(testFile, testFunc, testName);
            int rows = 20;
            int cols = 20;

            unsigned char* inputBinaryImage = (unsigned char*)malloc(sizeof(unsigned char) * rows * cols);
            unsigned short* labelledImage = (unsigned short*)malloc(sizeof(unsigned short) * rows * cols);

            int numRegionsExpected = 0;
            for (int idxRow = 0; idxRow < rows; idxRow++)
            {
                int val = idxRow % 2;
                for (int idxCol = 0; idxCol < cols; idxCol++)
                {
                    inputBinaryImage[idxRow * cols + idxCol] = val & idxCol % 2;

                    if (val & idxCol % 2)
                        numRegionsExpected++;
                }
            }

            int numRegions = 0;
            int success = LabelConnectedRegions(inputBinaryImage, rows, cols, labelledImage,&numRegions);
            if (numRegionsExpected != (cols / 2) * (rows / 2))
            {
                success = 0;
            }

            if (numRegions != numRegionsExpected)
            {
                success = 0;
            }

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);

            free(inputBinaryImage);
            free(labelledImage);
        }

        {
            testName = "test label checkerboard";
            print_test_label(testFile, testFunc, testName);

            int rows = 30;
            int cols = 11;

            unsigned char* inputBinaryImage = (unsigned char*)malloc(sizeof(unsigned char) * rows * cols);
            unsigned short* labelledImage = (unsigned short*)malloc(sizeof(unsigned short) * rows * cols);

            unsigned char shadow = 0;
            int expected_regions = 0;
            for (int r = 0; r < rows; r++)
            {
                shadow = (r % 2) == 0 ? 0 : 255;
                for (int c = 0; c < cols; c++)
                {
                    if (shadow == 255)
                    {
                        shadow = 0;
                    }
                    else
                    {
                        shadow = 255;
                        expected_regions++;
                    }

                    inputBinaryImage[r * cols + c] = shadow;
                }
            }

            int numLabels = 0;
            int success = LabelConnectedRegions(inputBinaryImage, rows, cols, labelledImage, &numLabels);

            if (numLabels != expected_regions)
            {
                success = 0;
            }

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);

            free(inputBinaryImage);
            free(labelledImage);
        }

        {
            testName = "test max labels";
            print_test_label(testFile, testFunc, testName);

            int rows = 550;
            int cols = 550;
            unsigned char* BinaryImg = (unsigned char*)malloc(sizeof(unsigned char) * 302500);
            unsigned char shadow = 0;
            for (int r = 0; r < rows; r++)
            {
                shadow = (r % 2) == 0 ? 0 : 255;
                for (int c = 0; c < cols; c++)
                {
                    if (shadow == 255)
                    {
                        shadow = 0;
                    }
                    else
                    {
                        shadow = 255;
                    }

                    BinaryImg[r * cols + c] = shadow;
                }
            }

            unsigned short* Labels = (unsigned short*)malloc(sizeof(unsigned short) * 302500);
            int numLabels = 0;
            int success = LabelConnectedRegions(BinaryImg, rows, cols, Labels, &numLabels);
            if (success == 0)
            {
                success = 1; //expected fail, too many labels
            }

            free(BinaryImg);
            free(Labels);

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);
        }
    }

    testFile = "AH_RockUtils.c";
    {
        testFunc = "read_pgm_image_dimensions";
        {
            testName = "test get image dimensions";
            print_test_label(testFile, testFunc, testName);

            int rows = 0;
            int cols = 0;

            int success = read_pgm_image_dimensions(testSmallImage, &rows, &cols);

            if (rows != 5)
            {
                success = 0;
            }

            if (cols != 10)
            {
                success = 0;
            }

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);
        }

        testFunc = "read_pgm_image_rect";
        {
            testName = "test reading subrect";
            print_test_label(testFile, testFunc, testName);

            int rows = 0;
            int cols = 0;

            int success = read_pgm_image_dimensions(testSmallImage, &rows, &cols);
            if (success == 1)
            {
                unsigned char* image = (unsigned char*)malloc(sizeof(unsigned char) * rows * cols);
                success = read_pgm_image_rect(testSmallImage, image, 1, 0, 1, cols);
                for (int i = 0; i < cols; i++)
                {
                    if (image[i] != 255)
                    {
                        success = 0;
                    }
                }
                free(image);
            }

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);
        }

        testFunc = "read_rocklist";
        {
            testName = "read reference rocklist";
            print_test_label(testFile, testFunc, testName);

            RD_PARMS parms;
            OUTROCK* rocks = NULL;
            int numRocks = 0;
            int success = read_rocklist(testRocklist, &parms, &rocks, &numRocks);

            if ((parms.ground_resolution != 0.273000f) ||
                (parms.gamma != 1.200000f) ||
                (parms.sun_azimuth_angle != 350.897003f) ||
                (parms.sun_incidence_angle != 49.995998f) ||
                (parms.min_shadow_size != 3.000000f) ||
                (parms.max_shadow_size != 800.000000f) ||
                (parms.confidence_threshold != 0.700000f) ||
                (parms.min_shadow_size_split != 100.000000f) ||
                (parms.mean_gradient_threshold != 20.000000f) ||
                (parms.rock_elongate_ratio != 4.000000f))
            {
                success = 0;
            }

            if (numRocks != 12627)
            {
                success = 0;
            }

            //"id,   tileR, tileC,  shaX,       shaY,       rockX,      rockY,      tileShaX,   tileShaY,   shaArea,    shaLen,     rockWidth,  rockHeight, score,      gradMean,   Compact,    Exent,      Class,  gamma
            //12620, 49,    23,     11837.7,    24920.7,    11836.7,    24920.5,    337.7,      420.7,      3,          1.65,       2.26,       1.38,       0.8991,     30.7565,    2.04665,    1.73205,    1,      1.2000
            OUTROCK rock = rocks[12620 - 1];
            if (rock.id != 12620) success = 0;
            if (rock.tileR != 49) success = 0;
            if (rock.tileC != 23) success = 0;
            if (rock.shaX != 11837.7f) success = 0;
            if (rock.shaY != 24920.7f) success = 0;
            if (rock.rockX != 11836.7f) success = 0;
            if (rock.rockY != 24920.5f) success = 0;
            if (rock.tileShaX != 337.7f) success = 0;
            if (rock.tileShaY != 420.7f) success = 0;
            if (rock.shaArea != 3) success = 0;
            if (rock.shaLen != 1.65f) success = 0;
            if (rock.rockWidth != 2.26f) success = 0;
            if (rock.rockHeight != 1.38f) success = 0;
            if (rock.score != 0.8991f) success = 0;
            if (rock.gradMean != 30.7565f) success = 0;
            if (rock.Compact != 2.04665f) success = 0;
            if (rock.Exent != 1.73205f) success = 0;
            if (rock.Class != 1) success = 0;
            if (rock.gamma != 1.2000f) success = 0;

            free(rocks);
            print_test_result(success);
            count_results(success, &numPassed, &numFailed);
        }

        testFunc = "compare_rocklist_files";
        {
            testName = "compare same rocklist";
            print_test_label(testFile, testFunc, testName);

            int success = compare_rocklist_files(testRocklist, testRocklist, 0);

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);
        }

        testFunc = "compare_rocklists";
        {
            testName = "test different rocklist";
            print_test_label(testFile, testFunc, testName);

            RD_PARMS parms0;
            OUTROCK* rocks0 = NULL;
            int numRocks0 = 0;
            int success = read_rocklist(testRocklist, &parms0, &rocks0, &numRocks0);

            if (success == 1)
            {
                RD_PARMS parms1;
                OUTROCK* rocks1 = NULL;
                int numRocks1 = 0;
                success = read_rocklist(testRocklist, &parms1, &rocks1, &numRocks1);

                if (success == 1)
                {
                    rocks1[1].shaArea += 1;

                    success = compare_rocklists(&parms0, rocks0, numRocks0,
                                                &parms1, rocks1, numRocks1, 0);

                    if (success == 0)
                    {
                        success = 1; //expect fail
                    }
                }

                free(rocks1);
            }

            free(rocks0);
            print_test_result(success);
            count_results(success, &numPassed, &numFailed);
        }

        {
            testName = "test different id rocklist";
            print_test_label(testFile, testFunc, testName);

            RD_PARMS parms0;
            OUTROCK* rocks0 = NULL;
            int numRocks0 = 0;
            int success = read_rocklist(testRocklist, &parms0, &rocks0, &numRocks0);

            if (success == 1)
            {
                RD_PARMS parms1;
                OUTROCK* rocks1 = NULL;
                int numRocks1 = 0;
                success = read_rocklist(testRocklist, &parms1, &rocks1, &numRocks1);

                if (success == 1)
                {
                    rocks1[1].id += 1;

                    success = compare_rocklists(&parms0, rocks0, numRocks0,
                                                &parms1, rocks1, numRocks1, 0);

                    //expect fail
                    if (success == 0)
                    {
                        //expect succeed
                        success = compare_rocklists(&parms0, rocks0, numRocks0,
                                                    &parms1, rocks1, numRocks1, 1);
                    }
                    else
                    {
                        success = 0;
                    }
                }

                free(rocks1);
            }
            
            free(rocks0);

            print_test_result(success);
            count_results(success, &numPassed, &numFailed);
        }

        {
            testName = "test reference against current";
            print_test_label(testFile, testFunc, testName);

            RD_PARMS parms0;
            OUTROCK* rocks0 = NULL;
            int numRocks0 = 0;
            int success = read_rocklist(testRocklist, &parms0, &rocks0, &numRocks0);

            if (success == 1)
            {
                RD_PARMS parms1 = parms0;
                OUTROCK* rocks1 = NULL;
                int numRocks1 = 0;
                success = detect(testImage, &parms1, &rocks1, &numRocks1);
                if (success == 1)
                {
                    success = compare_rocklists(&parms0, rocks0, numRocks0,
                                                &parms1, rocks1, numRocks1, 1);
                }
                free(rocks1);
            }
            free(rocks0);
            print_test_result(success);
            count_results(success, &numPassed, &numFailed);
        }
    }

    //TODO: test partial sized file
    if (numFailed == 0)
    {
        printf("\nAll tests passed\n");
    }
    else
    {
        printf("\n %d tests failed\n", numFailed);
    }

    return 1;
}
