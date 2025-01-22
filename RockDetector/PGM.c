
#include <stdio.h>
#include <stdlib.h>
#include <assert.h>
#include "PGM.h"

int read_pgm_image_dimensions(char* infilename, int* rows, int* cols)
{
    FILE* fp;

    //// Open the input image file for reading if a filename was given. If no
    //// filename was provided, set fp to read from standard input.
    if (infilename == NULL)
    {
        fp = stdin;

    }
    else
    {

        if ((fp = fopen(infilename, "rb")) == NULL)
        {
            char tmp[1024];
            snprintf(tmp, 1024, "Error reading the file %s in read_pgm_image().\n", infilename);
            fputs(tmp, stderr);
            return(0);
        }
    }

    // general case
    char imgtype = (char)getc(fp);
    char bandcode[2];
    char type = '0';
    bandcode[0] = (char)getc(fp);
    bandcode[1] = '\0';
    int intType = atoi(bandcode);
    assert(intType < 256);
    type = (char)intType;

    char eol = (char)getc(fp);
    if (eol != '\n' && eol != ' ' && eol != '\r')
    {
        fprintf(stderr, "read_image: bad header format.\r\n");
        if (fp != stdin) fclose(fp);
        return 0;
    }
    if (imgtype != 'P' || type != 5)
    {
        fprintf(stderr, "this format is not supported.\r\n");
        if (fp != stdin) fclose(fp);
        return(0);
    }

    // read comment block
    while (getc(fp) == '#') while (getc(fp) != '\n');
    fseek(fp, -1, SEEK_CUR);

    // read columns and rows, and max value
    int max;
    int read = fscanf(fp, "%d %d %d", cols, rows, &max);
    if(read == 2)
    {
        read = fscanf(fp, "%d", &max); //sometimes the max is on the next line
    }
    else if(read == 3)
    {
        //already got max
    }
    else
    {
        fprintf(stderr, "failed reading image dimensions.\r\n");
        if (fp != stdin) fclose(fp);
        return(0);
    }

    if (fp != stdin) fclose(fp);
    return(1);
}


/******************************************************************************
* Function: read_pgm_image_rect
* Reads a portion of an image in PGM format. The image is read from standard input when
* infilename = NULL. Image dimensions read from the header. Comments are
* skipped. Memory to store the image is allocated by the caller.
* On failure returns 0 else returns 1.
******************************************************************************/
int read_pgm_image_rect(char* infilename, unsigned char* image, int startRow, int startCol, int rectNumRows, int rectNumCols)
{
    FILE* fp;

    //// Open the input image file for reading if a filename was given. If no
    //// filename was provided, set fp to read from standard input.
    if (infilename == NULL)
    {
        fp = stdin;

    }
    else
    {

        if ((fp = fopen(infilename, "rb")) == NULL)
        {
            char tmp[1024];
            snprintf(tmp, 1024, "Error reading the file %s in read_pgm_image().\n", infilename);
            fputs(tmp, stderr);
            return(0);
        }
    }

    // general case
    char imgtype = (char)getc(fp);
    char bandcode[2];
    char type = '0';
    bandcode[0] = (char)getc(fp);
    bandcode[1] = '\0';
    int intType = atoi(bandcode);
    assert(intType < 256);
    type = (char)intType;

    char eol = (char)getc(fp);
    if (eol != '\n' && eol != ' ' && eol != '\r')
    {
        fprintf(stderr, "read_image: bad header format.\r\n");
        if (fp != stdin) fclose(fp);
        return 0;
    }
    if (imgtype != 'P' || type != 5)
    {
        fprintf(stderr, "this format is not supported.\r\n");
        if (fp != stdin) fclose(fp);
        return(0);
    }

    // read comment block
    while (getc(fp) == '#') while (getc(fp) != '\n');
    fseek(fp, -1, SEEK_CUR);

    // read columns and rows, and max value
    int max;
    int wholeImageCols;
    int wholeImageRows;
    size_t read = fscanf(fp, "%d %d %d",  &wholeImageCols, &wholeImageRows, &max);
    if (3 != read && 2 != read) //sometimes the max is on the next line
    {
        fprintf(stderr, "failed reading image dimensions.\r\n");
        if (fp != stdin) fclose(fp);
        return(0);
    }
  
    // Consume the final newline
    fgetc(fp);

    //validate request
    if (startRow < 0 || startCol < 0 ||
        rectNumRows <= 0 || (startRow + rectNumRows) > wholeImageRows ||
        rectNumCols <= 0 || (startCol + rectNumCols) > wholeImageCols)
    {
        fprintf(stderr, "Invalid rectangle requested in read_pgm_image().\n");
        if (fp != stdin) fclose(fp);
        return(0);
    }

    // read comment block
    while (getc(fp) == '#') while (getc(fp) != '\n');
    fseek(fp, -1, SEEK_CUR);

    int bytesPerPixel = 1; //8bit
    if (max > (1 << 16) - 1)
    {
        char tmp[1024];
        snprintf(tmp, 1024, "Greater than 16 bit not supported yet for %s.\n", infilename);
        fputs(tmp, stderr);
        return(0);
    }
    else if (max > (1 << 8) - 1)
    {
        //16 bit
        bytesPerPixel = 2;
    }

    //read each row of data into the rect
    long startImageData = ftell(fp);
    for (int idxRow = 0; idxRow < rectNumRows; idxRow++)
    {
        int idxWholeImageRow = startRow + idxRow;
        int idxWholeImagePixel = idxWholeImageRow * wholeImageCols + startCol;
        int idxRectPixel = idxRow * rectNumCols;
        if (0 != fseek(fp, startImageData + sizeof(unsigned char) * idxWholeImagePixel, SEEK_SET))
        {
            fprintf(stderr, "Error seeking through file in read_pgm_image().\n");
            if (fp != stdin) fclose(fp);
            return(0);
        }

        read = fread(&(image[idxRectPixel]), bytesPerPixel, rectNumCols, fp);
        if (read != (size_t)rectNumCols* bytesPerPixel)
        {
            fprintf(stderr, "Error reading image data in read_pgm_image().\n");
            if (fp != stdin) fclose(fp);
            return(0);
        }
    }

    if (fp != stdin) fclose(fp);
    return(1);
}


/******************************************************************************
* Function: read_pgm_image
* Reads in an image in PGM format. The image is read from standard input when
* infilename = NULL. Image dimensions read from the header. Comments are
* skipped. Memory to store the image is allocated in this function.
* On failure returns 0 else returns 1.
******************************************************************************/
int read_pgm_image(char* infilename, unsigned char** image, int* rows, int* cols)
{
    FILE* fp;

    //// Open the input image file for reading if a filename was given. If no
    //// filename was provided, set fp to read from standard input.

    if (infilename == NULL)
    {
        fp = stdin;

    }
    else
    {

        if ((fp = fopen(infilename, "rb")) == NULL)
        {
            char tmp[1024];
            snprintf(tmp, 1024, "Error reading the file %s in read_pgm_image().\n", infilename);
            fputs(tmp, stderr);
            return(0);
        }
    }

    //// Verify that the image is in PGM format, read in the number of columns
    //// and rows in the image and scan past all of the header information.

#if 0
  // original
    fgets(buf, 70, fp);
    char tmp[1024];
    snprintf(tmp, 1024, "firstLine=%s\n", buf);
    fputs(tmp, stdout);
    if (strncmp(buf, "P5", 2) != 0)
    {
        snprintf(tmp, 1024, "The file %s is not in PGM format in ", infilename);
        fputs(tmp, stderr);
        fprintf(stderr, "read_pgm_image().\n");
        if (fp != stdin) fclose(fp);
        return(0);
    }

    do { fgets(buf, 70, fp); } while (buf[0] == '#');  /* skip all comment lines */
    sscanf(buf, "%d %d", cols, rows);
    do { fgets(buf, 70, fp); } while (buf[0] == '#');  /* skip all comment lines */
#else
  // general case
    char imgtype = (char)getc(fp);
    char bandcode[2];
    char type = '0';
    bandcode[0] = (char)getc(fp);
    bandcode[1] = '\0';
    int intType = atoi(bandcode);
    assert(intType < 256);
    type = (char)intType;

    char eol = (char)getc(fp);
    if (eol != '\n' && eol != ' ' && eol != '\r')
    {
        fprintf(stderr, "read_image: bad header format.\r\n");
        if (fp != stdin) fclose(fp);
        return 0;
    }
    if (imgtype != 'P' || type != 5)
    {
        fprintf(stderr, "this format is not supported.\r\n");
        if (fp != stdin) fclose(fp);
        return(0);
    }

    // read comment block
    while (getc(fp) == '#') while (getc(fp) != '\n');
    fseek(fp, -1, SEEK_CUR);

    // read columns and rows, and max value
    int max;
    int read = fscanf(fp, "%d %d %d", cols, rows, &max);
    if (3 != read && 2 != read) //sometimes the max is on the next line
    {
        fprintf(stderr, "failed reading image dimensions.\r\n");
        if (fp != stdin) fclose(fp);
        return(0);
    }

    // Consume the final newline
    fgetc(fp);

    // read comment block
    while (getc(fp) == '#') while (getc(fp) != '\n');
    fseek(fp, -1, SEEK_CUR);
#endif

    //// Allocate memory to store the image then read the image from the file.

    if (((*image) = (unsigned char*)malloc((*rows) * (*cols))) == NULL)
    {
        fprintf(stderr, "Memory allocation failure in read_pgm_image().\n");
        if (fp != stdin) fclose(fp);
        return(0);
    }
    if ((size_t)(*rows) != fread((*image), (*cols), (*rows), fp))
    {
        fprintf(stderr, "Error reading the image data in read_pgm_image().\n");
        if (fp != stdin) fclose(fp);
        free((*image));
        return(0);
    }

    if (fp != stdin) fclose(fp);
    return(1);
}

/******************************************************************************
* Function: write_pgm_image
* Writes an image in PGM format. The file is either
* written to the file specified by outfilename or to standard output if
* outfilename = NULL.
******************************************************************************/
int write_pgm_image(char* outfilename, unsigned char* image, int rows,
                    int cols, int maxval)
{
    FILE* fp;
    //// Open the output image file for writing if a filename was given. If no
    //// filename was provided, set fp to write to standard output.

    if (outfilename == NULL) fp = stdout;
    else
    {
        if ((fp = fopen(outfilename, "wb")) == NULL)
        {
            char tmp[1024];
            snprintf(tmp, 1024, "Error writing the file %s in write_pgm_image().\n", outfilename);
            fputs(tmp, stderr);
            return(0);
        }
    }

    //// Write the header information to the PGM file.

    fprintf(fp, "P5\n%d %d\n", cols, rows);
    //if (comment != NULL)
    //    if (strlen(comment) <= 70) fprintf(fp, "# %s\n", comment);
    fprintf(fp, "%d\n", maxval);

    //// Write the image data to the file.
    int bytesPerPixel = 1; //8bit
    if (maxval > (1 << 16) - 1)
    {
        char tmp[1024];
        snprintf(tmp, 1024, "Greater than 16 bit not supported yet for %s.\n",  outfilename);
        fputs(tmp, stderr);
        return(0);
    }
    else if (maxval > (1 << 8) - 1)
    {
        //16 bit
        bytesPerPixel = 2;
    }

    if (rows != fwrite(image, cols * bytesPerPixel, rows, fp))
    {
        fprintf(stderr, "Error writing the image data in write_pgm_image().\n");
        if (fp != stdout) fclose(fp);
        return(0);
    }

    if (fp != stdout) fclose(fp);
    return(1);
}

/******************************************************************************
* Function: write_ppm_image
* Writes an image in 3 channel color PPM format. The file is either
* written to the file specified by outfilename or to standard output if
* outfilename = NULL.
******************************************************************************/
int write_ppm_image(char* outfilename, unsigned char* image, int rows,
                    int cols)
{
    FILE* fp;
    //// Open the output image file for writing if a filename was given. If no
    //// filename was provided, set fp to write to standard output.

    if (outfilename == NULL) fp = stdout;
    else
    {
        if ((fp = fopen(outfilename, "wb")) == NULL)
        {
            char tmp[1024];
            snprintf(tmp, 1024, "Error writing the file %s in write_ppm_image().\n", outfilename);
            fputs(tmp, stderr);
            return(0);
        }
    }

    //// Write the header information to the PPM file.

    fprintf(fp, "P6\n%d %d\n", cols, rows);
    //if (comment != NULL)
    //    if (strlen(comment) <= 70) fprintf(fp, "# %s\n", comment);
    fprintf(fp, "%d\n", 255);

    //// Write the image data to the file.

    if (rows != fwrite(image, cols * 3, rows, fp))
    {
        fprintf(stderr, "Error writing the image data in write_ppm_image().\n");
        if (fp != stdout) fclose(fp);
        return(0);
    }

    if (fp != stdout) fclose(fp);
    return(1);
}
