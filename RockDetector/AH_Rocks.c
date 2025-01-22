/*******************************************************************************
 AH_Rocks.c 

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
#include <assert.h>

void clear_rock(ROCK* rock)
{
    rock->flag = 0;
    rock->label = 0;
    rock->xmin = 0;
    rock->xmax = 0;
    rock->ymin = 0;
    rock->ymax = 0;
    rock->size = 0;
    rock->centroid_x = 0;
    rock->centroid_y = 0;
    rock->x0 = 0.0f;
    rock->y0 = 0.0f;
    rock->mean_g = 0.0f;
    rock->perimeter = 0.0f;
    rock->start[0] = 0.0f;
    rock->start[1] = 0.0f;
    rock->end[0] = 0.0f;
    rock->end[1] = 0.0f;
    rock->g_start = 0.0f;
    rock->g_end = 0.0f;
    rock->shadowLength = 0.0f;
    rock->shadowWidth = 0.0f;
    rock->shadowLength_modeled = 0.0f;
    rock->rock_height_modeled = 0.0f;
    rock->confidence = 0.0f;
    rock->rock_major = 0.0f;
    rock->rock_minor = 0.0f;
    rock->rock_theta = 0.0f;
    rock->rock_width_modeled = 0.0f;
    rock->tile_row = 0;
    rock->tile_col = 0;
}

int getsubset(unsigned char *SceneImg, int fcols, int frows, unsigned char *subimg, 
        int cols, int rows, int start_col, int end_col, int start_row, int end_row) 
{
  if((start_col >= 0) ||
    (start_row >= 0) ||
    (end_col >= 0) ||
    (end_row >= 0) ||
    (start_row + rows - 1 < frows) ||
    (start_col + cols - 1 < fcols))
  {
      printf("error getting subset\n");
      return 0;
  }
  
  int i , j;
  for(i = 0; i < rows; ++i) {
    for(j = 0; j < cols; ++j) {
      subimg[i*cols +j] = SceneImg[(start_row + i) *fcols + start_col +j];
    }
  }
  return 1 ;
}

void get_gamma_image(unsigned char* SceneImg, unsigned char* GammaImg,
                     int rows, int cols, float MetGamma)
{
    int GammaLUT[HISTLEN];

    // calculate look up table for gamma adjusted pixel intensities
    double dblMax = 255.0;
    int intMax = 255;
    for (int i = 0; i < 256; i++)
    {
        GammaLUT[i] = (int)(i + dblMax * pow((i / dblMax), (1.0 / MetGamma)));
        if (GammaLUT[i] > intMax)
        {
            GammaLUT[i] = intMax;
        }
    }

    // generate gamma adjusted image
    for (int i = 0; i < rows * cols; i++)
    {
        GammaImg[i] = (unsigned char)GammaLUT[SceneImg[i]];
    }
}

int get_gMET_threshold(unsigned char* GammaImg,
                       int rows, int cols)
{
    // initialize histogram
    int Hist[HISTLEN];
    for (int i = 0; i < HISTLEN; i++)
    {
        Hist[i] = 0;
    }

    // calculate histogram for gamma adjusted image
    for (int i = 0; i < rows * cols; i++)
    {
        Hist[GammaImg[i]]++;
    }

    // Normalize Histogram to compute probabilities:
    double sum = 0;
    for (int i = 0; i < HISTLEN; ++i)
    {
        sum += Hist[i];
    }

    double nHist[HISTLEN];
    for (int i = 0; i < HISTLEN; i++)
    {
        nHist[i] = Hist[i] / sum;
    }

    // accumulate probabilities
    double pT[HISTLEN];
    pT[0] = nHist[0];

    for (int i = 1; i < HISTLEN; i++)
    {
        pT[i] = pT[i - 1] + nHist[i];
    }

    // Entropy for black and white parts of the histogram:
    double epsilon = Double_MIN_VALUE;
    double hhB = 0, hhW = 0;
    double hB[HISTLEN];
    double hW[HISTLEN];
    for (int t = 0; t < HISTLEN; t++)
    {
        // Black entropy:
        if (pT[t] > epsilon)
        {
            hhB = 0;
            for (int i = 0; i <= t; i++)
            {
                if (nHist[i] > epsilon)
                {
                    hhB -= nHist[i] / pT[t] * log(nHist[i] / pT[t]);
                }
            }
            hB[t] = hhB;
        }
        else
        {
            hB[t] = 0;
        }

        // White entropy:
        double pTW = 1 - pT[t];
        if (pTW > epsilon)
        {
            hhW = 0;
            for (int i = t + 1; i < HISTLEN; ++i)
            {
                if (nHist[i] > epsilon)
                {
                    hhW -= nHist[i] / pTW * log(nHist[i] / pTW);
                }
            }
            hW[t] = hhW;
        }
        else
        {
            hW[t] = 0;
        }
    }

    // Find histogram index with maximum entropy:
    double jMax = hB[0] + hW[0];
    int tMax = 0;
    for (int t = 1; t < HISTLEN; ++t)
    {
        double j = hB[t] + hW[t];
        if (j > jMax)
        {
            jMax = j;
            tMax = t;
        }
    }

    return tMax;
}

void get_binary_image(unsigned char* src_img, unsigned char* dst_img, int rows, int cols, int threshold)
{
    for(int i=0; i < rows*cols; i++)
    {
        if (src_img[i] < threshold)
        {
            dst_img[i] = MASKFR;
        }
        else
        {
            dst_img[i] = MASKBK;
        }
    }
}

/*******************************************************************************
* PROCEDURE: Gamma Maximum Entropy Thresholding (gMET) Shadows Segmentation
*            Note: New Method - Keep Restricted
*******************************************************************************/

int gMET_shadows(unsigned char *SceneImg, unsigned char *ShaImg,
         int rows, int cols, float MetGamma, int gamma_threshold_override)
{
    // get the image that has been brightened and stretched by the gamma function
    unsigned char* GammaImg = (unsigned char*)malloc(sizeof(unsigned char) * rows * cols);
    get_gamma_image(SceneImg, GammaImg, rows, cols, MetGamma);

   
    // find the threshold below which values are considedered shadows
    int threshold = 0;
    if (gamma_threshold_override > 0)
    {
        //use a user provided threshold
        threshold = gamma_threshold_override;
    }
    else
    { 
        // calculate the threshold below with all pixels are shadows using gMET
        threshold = get_gMET_threshold(GammaImg, rows, cols);
    }
    

    // create the binary image using the theshold
    get_binary_image(GammaImg, ShaImg, rows, cols, threshold);

    free(GammaImg);

    return threshold;
}


/*
    calculates the 2x2 gradient

    x kernel:               y kernel:          and result goes here:
    
        ---------------     ---------------    --------------
        | -0.5 |  0.5 |     | -0.5 | -0.5 |    |      |     |
        ---------------     --------------     --------------
        | -0.5 |  0.5 |     |  0.5 | 0.5  |    |      |  *  |
        ---------------     ---------------    --------------
*/
void differentiate(unsigned char *img, float *gx, float *gy, int rows, int cols)
{
  unsigned char *pt, *ptUp, *ptUpLeft, *ptLeft;
  float *pt_gx, *pt_gy;
 
  //zero first row
  for (int x = 0; x < cols; x++)
  {
     gx[x] = 0;
     gy[x] = 0;
  }

  //zero first col
  for (int y = 0; y < rows; y++)
  {
      gx[y * cols] = 0;
      gy[y * cols] = 0;
  }

  // calculate 2x2 gradient
  for (int y=1; y < rows; y++)
  {
    pt_gx = &gx[y*cols + 1];
    pt_gy = &gy[y*cols + 1];

    pt = &img[y*cols + 1];
    ptUp = &img[(y-1)*cols + 1];
    ptUpLeft = ptUp - 1;
    ptLeft = pt - 1;

    for (int x=1; x < cols; x++)
    {
      *pt_gx = (float)0.5*(*pt + *ptUp - *ptUpLeft - *ptLeft);
      *pt_gy = (float)0.5*(*pt + *ptLeft - *ptUp - *ptUpLeft);
      pt_gx++;
      pt_gy++;

      pt++;
      ptUp++;
      ptUpLeft++;
      ptLeft++;
    }
  }
}

int BorderOrInside(unsigned short *CC, int cols, int rows, int lab, int ix, int iy) {
  assert(iy >= 0 && iy < rows);
  assert(ix >= 0 && ix < cols);
  if (iy >= rows)
      return 0;

  if(CC[iy*cols + ix] == lab)
    return 1;
  else if(CC[iy*cols + ix+1] == lab)
    return 1;
  else if(CC[iy*cols + ix-1] == lab)
    return 1;
  else if(CC[(iy-1)*cols + ix] == lab)
    return 1;
  else if(CC[(iy+1)*cols + ix] == lab)
    return 1;
  else return 0;
}

int RocksModeling(unsigned short *CC, int cols, int rows, ROCK *rocks, int maxlab, int MinSize) {
  int i, m, n, k;
  double npoints, xsum, ysum, y2sum, x2sum, xysum;
  double dx, dy, r;
  double theta = 0;
  double major = 0;
  double minor = 0;
  double scale;
  double *a, v[4], w[2];

  for( i= 0, k = 0; i < maxlab; ++i) {
    if(k < rocks[i].size) k = rocks[i].size;
  }
  a = (double *)malloc(sizeof(double)*(k+200)*2);

  for(i = 0; i < maxlab; ++i) {
    if(rocks[i].size >=MinSize && rocks[i].flag == 1) {
      fflush(stdout);
      xsum  = 0.0; ysum  = 0.0;   
      x2sum = 0.0; y2sum = 0.0;   xysum = 0.0;
      npoints = 0;
      k = 0;
      assert(rocks[i].ymax < rows);
      assert(rocks[i].xmax < cols);
      if (rocks[i].ymax >= rows || rocks[i].xmax >= cols)
      {
          printf("failed rocks modeling");
          return 0;
      }
      for(m = rocks[i].ymin; m <= rocks[i].ymax; ++m) {
        for(n = rocks[i].xmin; n <= rocks[i].xmax; ++n) {
          if(CC[m*cols+n] == i) {
            dx = n - rocks[i].x0;
            dy = m - rocks[i].y0;
            a[k*2] = dx;
            a[k*2+1] = dy;
            xsum +=dx;
            ysum +=dy;
            x2sum +=dx*dx;
            y2sum +=dy*dy;
            xysum +=dy*dx;
            npoints++;
            k++;
          }
        }
      }
      w[0] = 0.0;
      w[1] = 0.0;
      if(SVDecompositionD(a, w, v, k, 2)) {
        if(fabs(w[0]) >= fabs(w[1])) {
          major = fabs(w[0])/2.0;
          minor = fabs(w[1] + 0.000001)/2.0;
          dx = v[0];
          dy = v[2];
          scale = sqrt (npoints / (3.1415926 * major * minor)); //equalize areas
          major = major*scale;
          minor = minor*scale;
          theta = atan2(dy, dx);
          if(theta < 0.0) theta = 2.0*3.1415926 + theta;
        
        } else if(fabs(w[0]) < fabs(w[1])) {

          major = fabs(w[1])/2.0;
          minor = fabs(w[0] + 0.00001)/2.0;
          dx = v[1];
          dy = v[3];
          scale = sqrt (npoints / (3.1415926 * major * minor)); //equalize areas
          major = major*scale;
          minor = minor*scale;
          theta = atan2(dy, dx);
          if(theta < 0.0) theta = 2.0*3.1415926 + theta;
        }

        if(major/minor > 10) {
          rocks[i].flag = 0;
        
        } else {
          rocks[i].rock_major = (float)major;
          rocks[i].rock_minor = (float)minor;
          //theta = atan(y2, x2);
          rocks[i].rock_theta = (float)theta;
        }
      } else {
        rocks[i].flag = 0;
        rocks[i].rock_major = 0;
        rocks[i].rock_minor = 0;
        //theta = atan(y2, x2);
        rocks[i].rock_theta = 0;
        rocks[i].confidence = 0.0;
      }
      //  printf("major minor theta %f %f %f\n", major, minor, theta);
      //xCenter = rocks[i].x0 + xoffset + 0.5;
      //yCenter = rocks[1].y0 + yoffset + 0.5;

      /* shaLength  = 2.0*((majorax/2.0)*cosBeta + (minorax/2.0)*sinBeta);
         shaWidth   = 2.0*((minorax/2.0)*cosBeta + (majorax/2.0)*sinBeta);
         rockHeight = shaLength*tan(deg2rad(90.0-SunInc));
         rockArea   = 0.25*(3.1415926*majorax*minorax);
         rockPer    = 2.0*3.1415926*sqrt(0.5*(sqr(0.5*majorax)+sqr(0.5*minorax)));
         rockDiam   = shaWidth;
         // if shadow width vector closer to illumination vector:
         xRock      = xCenter - (0.5*shaWidth)*cosAzim;
         yRock      = yCenter + (0.5*shaWidth)*sinAzim;
         // if shadow length vector closer to illumination vector:
         //xRock      = xCenter - (0.5*shaLength)*cosAzim;
         //yRock      = yCenter + (0.5*shaLength)*sinAzim;
         majorax    = rockDiam;
         minorax    = rockDiam;
         //ModelOK=get_Rock(fragNo,SunInc,SunAzm,GndRes,MinSize,HazSize,AsRatio,HazImg,rows,cols);
       */
     } else if(rocks[i].size > 0 && rocks[i].size < MinSize) {
       r = sqrt((float)rocks[i].size/3.1415926);
       rocks[i].shadowLength = (float)(r*2);
       rocks[i].shadowWidth = (float)(r*2);
       rocks[i].rock_major = rocks[i].shadowLength/2;
       rocks[i].rock_minor = rocks[i].shadowWidth/2.0f;
       rocks[i].rock_theta = 0.0;
     }
   }
   free(a);
   return 1;
}

int MeasureShadowsLengthandWidth(unsigned short *CC, unsigned char *OrgImg,
         float *gx, float *gy, int cols, int rows, ROCK *rocks, 
         int maxlab, int MinSize, double angle)
{
  double dx, dy;
  double total;

  #define profileMaxCount 200
  double profile[profileMaxCount];
  int bv_profile[profileMaxCount];
  double profile1[profileMaxCount], profile2[profileMaxCount];

  int flag, ix, iy, ix1, iy1, ix2, iy2;
  int i,  m, n, k, ir;
  double r, minp, maxp;
  double dx1, dy1, dx2, dy2;
  int flag1, flag2;
  
  int m1 = 1;
  int m2 = 1;
  double fm1,fm2;
  dx = cos(angle);
  dy = -sin(angle); 
  
  for(i = 0; i < maxlab; ++i) {
      if (rocks[i].flag == 0)
          continue;

    //when the rock is too small, the direct measure the shaodw lenghth is not very accurate
    //we will use differene method to do so
    if(rocks[i].size >= MinSize) {
      total = 0;
      for(m = rocks[i].ymin, k = 0; m <= rocks[i].ymax; ++m) {
        for(n = rocks[i].xmin; n <= rocks[i].xmax; ++n) {
          if(CC[m*cols + n] == i) {
            total += OrgImg[m*cols + n];
            k++;
          }
        }
      }
      //dark = total/k;
      r = sqrt((float)rocks[i].size/3.1415926);
      ir = (int)(r+2.5);
      //use three pixel wide band to find the maximum grad
      for(k = -ir; k <= ir; ++k) {
        assert(k + ir >= 0);
        assert(k + ir < profileMaxCount);
        profile[k+ir] = 0;
        bv_profile[k+ir] = 0;
        ix = (int)(rocks[i].x0 + dx*k + 0.5);
        iy = (int)(rocks[i].y0 + dy*k + 0.5);
        //check if the point is within the shadow region
        flag = 0;
        if(ix > 0 && ix < cols-1 && iy > 0 && iy < rows-1) {
          flag = BorderOrInside(CC, cols, rows, i,  ix, iy);
        }
        if(flag == 1 ) {
          profile[k+ir] = gx[iy*cols + ix]*dx + gy[iy*cols + ix]*dy;
          bv_profile[k+ir]=OrgImg[iy*cols + ix];
          //x_profile[k+ir] = ix;
          //y_profile[k+ir] = iy;
        }
        ix = (int)(rocks[i].x0 + dx*k + 0.5 - dy);
        iy = (int)(rocks[i].y0 + dy*k + 0.5 + dx);
        flag = 0;
        if(ix > 0 && ix < cols && iy > 0 && iy < rows-1 ) {
          flag = BorderOrInside(CC, cols, rows, i,  ix, iy);
        }
        if(flag == 1 ) {
          profile[k+ir] += gx[iy*cols + ix]*dx + gy[iy*cols + ix]*dy;
          bv_profile[k+ir]+=OrgImg[iy*cols + ix];

        } else {
          profile[k+ir] +=profile[k+ir];
        }
        ix = (int)(rocks[i].x0 + dx*k + 0.5 + dy);
        iy = (int)(rocks[i].y0 + dy*k + 0.5 - dx);
        flag = 0;
        if(ix > 0 && ix < cols-1 && iy > 0 && iy < rows-1) {
          flag = BorderOrInside(CC, cols, rows, i,  ix, iy);
        }
        if(flag == 1 ) {
          profile[k+ir] += gx[iy*cols + ix]*dx + gy[iy*cols + ix]*dy;
          bv_profile[k+ir] +=OrgImg[iy*cols + ix];
        
        } else {
          profile[k+ir] +=profile[k+ir];
        }
      }
      //to find the maximum, which will be the end of the rock shadow
      maxp = -100000.0;
      assert(ir*2 < profileMaxCount);
      for(k = ir; k <= ir*2; ++k) {
        if(profile[k] > maxp) {
          m = k;
          maxp = profile[k];
        }
      }
      r = 1.0-bv_profile[m]/3.0/128;
      rocks[i].end[0] = (float)(rocks[i].x0 + dx*abs((m-ir)));
      rocks[i].end[1] = (float)(rocks[i].y0 + dy*abs((m-ir)));
      rocks[i].g_end = (float)(maxp/3.0);
      //rocks[i].end[0] = x_profile[m-1] + r*dx;
      //rocks[i].end[1] = y_profile[m-1] + r*dy;
      minp = 1000000.0;
      for(k = 0; k <= ir; ++k) {
        if(profile[k] < minp) {
          m = k;
          minp = profile[k];
        }
      }
    
      //rocks[i].start[0] = rocks[i].x0 - dx*fabs((m-ir));
      //rocks[i].start[1] = rocks[i].y0 - dy*fabs((m-ir));
      //r = 1.0 - bv_profile[m]/3.0/128;
      //rocks[i].start[0] = x_profile[m-1] - r*dx;
      //rocks[i].start[1] = y_profile[m-1] - r*dy;
      rocks[i].start[0] = (float)(rocks[i].x0 - dx*abs((m-ir)));
      rocks[i].start[1] = (float)(rocks[i].y0 - dy*abs((m-ir)));
      rocks[i].g_start = (float)(minp/3.0);
    
    } else {
      r = sqrt((float)rocks[i].size/3.1415926);
      rocks[i].start[0] = rocks[i].x0 - (float)(dx*r);
      rocks[i].start[1] = rocks[i].y0 - (float)(dy*r);
      rocks[i].end[0] = rocks[i].x0 + (float)(dx*r);
      rocks[i].end[1] = rocks[i].y0 + (float)(dy*r);
      rocks[i].g_start = 0;
      rocks[i].g_end = 0;
      //TODO: initialize g_start and g_end here with points on the gx gy, ISSUE #25
    }
  }
  
  dx1 = -dy;
  dy1 = dx;
  
  dx2 = dy;
  dy2 = -dx;
  
  for(i = 0; i < maxlab; ++i) {
    if(rocks[i].size >= MinSize && rocks[i].flag == 1) {
      r = sqrt((float)rocks[i].size/3.1415926);
      ir = (int)(r+2.5);
      for(k = 0; k < 200; ++k) {
        profile[k] = 0.0;
      }
      for(k = -ir+2; k <= ir-2; ++k) {
        profile[k+ir] = 0;
        bv_profile[k+ir] = 0;
        ix = (int)(rocks[i].x0 + dx*k + 0.5);
        iy = (int)(rocks[i].y0 + dy*k + 0.5);
        //check if the point is within the shadow region
        flag = 0;
        if(ix > 0 && ix < cols-1 && iy > 0 && iy < rows-1) {
          flag = BorderOrInside(CC, cols, rows, i,  ix, iy);
        }
        if(flag == 1 ) {
          for(m = 0; m <= ir+3; ++m) {
            profile1[m] = 0.0;
            profile2[m] = 0.0;
          }
          for(m = 0; m <= ir+3; ++m) {
            ix1 = (int)(ix + dx1*m + 0.5);
            iy1 = (int)(iy + dy1*m + 0.5);
            ix2 = (int)(ix + dx2*m + 0.5);
            iy2 = (int)(iy + dy2*m + 0.5);
            if(ix1 > 0 && ix1 < cols-1 && iy1 > 0 && iy1 < rows-1) {
              //profile1[m] = gx[iy1*cols + ix1]*dx1 + gy[iy1*cols + ix1]*dy1;
              flag1 = 0;
              flag1 = BorderOrInside(CC, cols, rows, i,  ix1, iy1);
              if(flag1 == 1 && CC[iy1*cols + ix1] == i) {
                profile1[m] = gx[iy1*cols + ix1]*dx1 + gy[iy1*cols + ix1]*dy1;
              }
            }
            if(ix2 > 0 && ix2 < cols-1 && iy2 > 0 && iy2 < rows-1) {
              //profile2[m] = gx[iy2*cols + ix2]*dx2 + gy[iy2*cols + ix2]*dy2;  
              flag2 = BorderOrInside(CC, cols, rows, i,  ix2, iy2);
              if(flag2 == 1 &&  CC[iy2*cols + ix2] == i) {
                profile2[m] = gx[iy2*cols + ix2]*dx2 + gy[iy2*cols + ix2]*dy2;
              }
            }
          }
          minp = 100000.0;
          maxp= -100000.0;
          for(m = 1; m <= ir+3; ++m) {
            if(profile1[m] > maxp) {
              m1 = m;
              maxp = profile1[m];
            }
          }
          if(m1 != 1 && m1 != ir+3 && maxp > 10.0) {
            fm1 =m1+interpolate((float)(profile1[m1-1]), (float)(profile1[m1]), (float)(profile1[m1+1]));
          
          } else {
            fm1 = m1;
          }
          //if(maxp < 1.0)
          //{
          //     fm1 = 0.0;
          //}
          maxp= -100000.0;
          for(m = 1; m < ir+3; ++m) {
            if(profile1[m] > maxp) {
              m2 = m;
              maxp = profile2[m];
            }
          }
          if(m2 != 1 && m2 != ir+3 && maxp > 10.0) {
            fm2 =m2+interpolate((float)(profile2[m2-1]), (float)(profile2[m2]), (float)(profile2[m2+1]));
          } else {
            fm2 = m2;
          }
          //if(maxp < 1.0)
          //{
          //   fm2 = 0.0;
          //}
          profile[k+ir] = fm1 + fm2;
        }
      }
      maxp = -10000.0;
      for(k = 2; k <= 2*ir-4; ++k) {
        if(profile[k] > maxp) {
          m2 = k;
          maxp = profile[k];
        } 
      }
      if(m2 != 2 && m2 != 2*ir-4) {
        fm2 =maxp+interpolate((float)(profile[m2-1]),(float)(profile[m2]), (float)(profile[m2+1]));

      } else {
        fm2 = maxp;
      }
      rocks[i].shadowWidth = (float)fm2;
      /* minp = 100000.0;
         maxp = -100000.0;
         for(m = rocks[i].ymin-1; m <= rocks[i].ymax+1; ++m)
         {
         if(m > 1 && m < rows-1)
         {
         for(n = rocks[i].xmin-1; n <= rocks[i].xmax+1; ++n)
         {
         if(CC[m*cols+n] == i)
         {
         if(CC[m*cols+n+1] != i ||   CC[m*cols+n-1] != i ||
         CC[(m-1)*cols+n] != i || CC[(m+1)*cols+n] != i)
         {
         r = dy*n - dx*m - dy*rocks[i].x0 + dx*rocks[i].y0;
         if(r < minp) minp = r;
         if(r > maxp) maxp = r;
         }
         }
         }
         }
         }*/
      //rocks[i].shadowWidth = (fabs(maxp) + fabs(minp));
    } else {
      rocks[i].shadowWidth = (float)sqrt((float)rocks[i].size/3.1415926)*2;
    }
  }
  return 1;
}

int RocksScores(ROCK *rocks, int maxlab, RD_PARMS *rd_parms) {
  int i;
  double g_end_sum, g_start_sum;
  int nr;
  double r;
  int maxsize;
  maxsize = 0;
  for(i = 0; i < maxlab; ++i) {
    if(rocks[i].size > maxsize && rocks[i].flag == 1) {
      maxsize = rocks[i].size;
    }
  }
  maxsize = maxsize/2;
  if(maxsize > 5) {
    nr = 0;
    g_end_sum = 0.0;
    g_start_sum = 0.0;
    for(i = 0; i < maxlab; ++i) {
      if(rocks[i].size >= maxsize && rocks[i].flag == 1) {
        g_end_sum +=rocks[i].g_end;
        g_start_sum +=rocks[i].g_start;
        nr++;
      }
    }
    if(nr > 0) {
      g_end_sum = g_end_sum/nr*0.8;
      g_start_sum = g_start_sum/nr*0.8;
      if(g_end_sum < 25) g_end_sum = 25;
      if(g_start_sum > -20) g_start_sum = -25.0;

      for(i = 0; i < maxlab; ++i) {
        if(rocks[i].size >= rd_parms->min_shadow_size) {
          r = (rocks[i].g_end/g_end_sum + rocks[i].g_start/g_start_sum)/2.0;
          if(r > 1.0) r = 1.0;
          rocks[i].confidence = (float)r;

        } else if(rocks[i].size >= 3) {
          rocks[i].confidence = 0.6f;

        } else if(rocks[i].size >= 2) {
          rocks[i].confidence = 0.4f;

        } else {
          rocks[i].confidence = 0.0f;
        }
      }
    } 
    else
    {
      // EBA: will this else ever be reached?
      printf("Unexpected behaviour. Maxsize=%d, nr=%d", maxsize, nr);
      exit(-1);
    }
  } else {
    for(i = 0; i < maxlab; ++i) {
      // Choose near 100% probability for these rocks that are without a 
      // large reference rock such that they pass the confidence test, and, are
      // classified according to other measures like gradient threshold, while 
      // they can still be identified by the unusual confidence score.
      if(rocks[i].size >= 5) {
        rocks[i].confidence = 0.9995f;

      } else if(rocks[i].size >= 2) {
        rocks[i].confidence = 0.9992f;

      } else {
        rocks[i].confidence = 0.9990f;
      }
    }
  }
  return 1;
}

int SpliteClusteredRock(unsigned short *CC, unsigned char *OrgImg, 
            int cols, int rows, ROCK *rocks, int *maxlab,
            RD_PARMS *rd_parms) {
  ROCK *rocks_split;
  int i,  m, n;
  unsigned char *tmpbuf, *pt;
  unsigned short *tmpCC, *ptShort;
  double total, dark;
  int subcols, subrows, newlab, k;
  int tmpmaxlab;
  int mink = 1;
  double d, mind;
  int maxi, mini;
  rocks_split = (ROCK *)malloc(sizeof(ROCK)*1000);
  tmpbuf = (unsigned char *)malloc(sizeof(char)*cols*rows);
  tmpCC = (unsigned short *)malloc(sizeof(short)*cols*rows);
  //use a buffer to store new rocks.
  newlab = *maxlab;
  for(i = 0; i < *maxlab; ++i) {
    if( rocks[i].size > rd_parms->min_shadow_size_split && rocks[i].flag == 1) {
      maxi = 0;
      mini = 255;
      pt = tmpbuf;
      ptShort = &tmpCC[0];
      total = 0;
      for(m = rocks[i].ymin, k = 0; m <= rocks[i].ymax; ++m) {
        for(n = rocks[i].xmin; n <= rocks[i].xmax; ++n) {
          if(CC[m*cols + n] == i) {
            total += OrgImg[m*cols + n];
            if(OrgImg[m*cols + n] > maxi) maxi = OrgImg[m*cols + n];
            if(OrgImg[m*cols + n] < mini) mini = OrgImg[m*cols + n];
            k++;
          }
        }
      }
      dark = mini + (maxi - mini)*rd_parms->spliting_ratio;
      //dark = total/k*rd_parms->spliting_ratio;
      for(m = rocks[i].ymin; m <= rocks[i].ymax; ++m) {
        for(n = rocks[i].xmin; n <= rocks[i].xmax; ++n) {
          if(OrgImg[m*cols+n] > dark || CC[m*cols+n] != i) {
            *pt = 0;

          } else {
            *pt =1;
          }
          *ptShort = 0;
          ptShort++;
          pt++;
        }
      }
      subcols = rocks[i].xmax - rocks[i].xmin +1;
      subrows = rocks[i].ymax - rocks[i].ymin +1;

      if (0 == LabelConnectedRegions(tmpbuf, subrows, subcols, tmpCC, &tmpmaxlab))
      {
          printf("Failed image labelling\n");
          return 0; //FAIL
      }

      //printf("position %d %d number of rocks %d\n", rocks[i].centroid_x, rocks[i].centroid_y, tmpmaxlab);
      pt = tmpbuf;
      if(tmpmaxlab > 999) {
        printf("warning: the number of splited region exceed 100 %d\n", tmpmaxlab);
        
        *maxlab = newlab+1;
        free(tmpbuf);
        free(tmpCC);
        free(rocks_split);
        return newlab+1;
      }
      if(tmpmaxlab > 1) {
        //figure out the centroid of these rocks
        for(m = 0; m <= tmpmaxlab; ++m) {
          rocks_split[m].size = 0;
          rocks_split[m].centroid_x = 0;
          rocks_split[m].centroid_y = 0;
          rocks_split[m].xmax= 0;
          rocks_split[m].ymax = 0;
          rocks_split[m].xmin = cols;
          rocks_split[m].ymin = rows;
          rocks_split[m].size = 0;
        }
        for(m = 0; m < subrows; ++m) {
          for(n = 0; n < subcols; ++n) {
            rocks_split[tmpCC[m*subcols + n]].size++;
                        rocks_split[tmpCC[m*subcols + n]].centroid_x += n;
            rocks_split[tmpCC[m*subcols + n]].centroid_y += m;
            if(rocks_split[tmpCC[m*subcols + n]].xmin > n) rocks_split[tmpCC[m*subcols + n]].xmin = n;
            if(rocks_split[tmpCC[m*subcols + n]].ymin > m) rocks_split[tmpCC[m*subcols + n]].ymin = m;
            if(rocks_split[tmpCC[m*subcols + n]].xmax < n) rocks_split[tmpCC[m*subcols + n]].xmax = n;
            if(rocks_split[tmpCC[m*subcols + n]].ymax < m) rocks_split[tmpCC[m*subcols + n]].ymax = m;
          }
        }
        for(m = 1; m <= tmpmaxlab; ++m) {
          rocks_split[m].centroid_x /=rocks_split[m].size;
          rocks_split[m].centroid_y /=rocks_split[m].size;
        }
        //pt = tmpbuf;
        for(m = 0; m < subrows; ++m) {
          for(n = 0; n < subcols; ++n) {
            if(CC[(m+rocks[i].ymin)*cols + n+rocks[i].xmin] == i) {
              mind = 10000;
              for(k = 1; k <= tmpmaxlab; ++k) {
                d = abs(m-rocks_split[k].centroid_y);
                d += abs(n-rocks_split[k].centroid_x);
                if(d < mind) {
                  mind = d;
                  mink = k;
                }
              }
              if(mink != 1)
              {
                 int newLabel = newlab + mink;
                 assert(newLabel < pow(2, 17));
                 CC[(m+rocks[i].ymin)*cols + n+rocks[i].xmin] = (unsigned short)newLabel;
              }
            }
          }          
        }

        newlab +=tmpmaxlab; 
      }    
    }
  }
  *maxlab = newlab+1;
  free(tmpbuf);
  free(tmpCC);
  free(rocks_split);
  return 1; //SUCCESS
}

int FilterRocks(ROCK* rocks, int maxlab, RD_PARMS* rd_parms)
{
    double aratio;

    for (int i = 0; i < maxlab; ++i)
    {
        if (rocks[i].flag == 0)
            continue;

        if (rocks[i].rock_minor > 0)
        {
            aratio = rocks[i].rock_major / rocks[i].rock_minor;

        }
        else
        {
            aratio = rd_parms->rock_elongate_ratio;
        }

        if (aratio >= rd_parms->rock_elongate_ratio)
        {
            rocks[i].confidence = 0.0; rocks[i].flag = 0;
        }

        //filter rocks by thresholds
        if (rocks[i].flag == 1 && (rocks[i].size < rd_parms->min_shadow_size ||
            rocks[i].mean_g < rd_parms->mean_gradient_threshold ||
            rocks[i].confidence < rd_parms->confidence_threshold))
        {
            rocks[i].flag = 0;
        }
    }

    return 1;
}

int RockHeightWidth(ROCK *rocks, int maxlab, RD_PARMS *rd_parms )
{
  int i, j;
  double ta;
  double dx, dy;
  double dxw, dyw;
  double cs, si;
  double a, b, angle, theta;
  double xa0, ya0, xa1, ya1, xb1, yb1;
  double step;
  double d, mind, w, bw;
  double l, maxd;
  double sw[2][2];
  double sl[2][2];
  double mint, maxt;
  int max;
  max = 36;
  step=2*3.1415926/(double)max;
  ta = tan(rd_parms->sun_incidence_angle*3.1415926/180.0);
  //angle = (rd_parms->sun_azimuth_angle + 180.0)*3.1415/180.0;                 //BUBGUB: subsolar aziumth or shadow direction? +180 makes it shadow dir
  angle = rd_parms->sun_azimuth_angle * 3.1415 / 180.0;                 //BUBGUB: subsolar aziumth or shadow direction? +180 makes it shadow dir
  //cs1 = cos(angle-3.1415/2.0);
  //si1 = -sin(angle-3.1415/2.0); 
  //cs2 = cos(angle+3.1415/2.0);
  //si2 = -sin(angle+3.1415/2.0);
  cs = cos(angle);
  si = -sin(angle);
  for(i = 0; i < maxlab; ++i) {
    if(rocks[i].size >= rd_parms->min_shadow_size && rocks[i].flag == 1) {
      dx = rocks[i].start[0] - rocks[i].end[0];
      dy = rocks[i].start[1] - rocks[i].end[1];
      rocks[i].shadowLength = (float)sqrt(dx*dx + dy*dy);
    
      a = rocks[i].rock_major;
      b = rocks[i].rock_minor;
      theta = rocks[i].rock_theta;
      mind = 10000.0;
      maxd = -10000.0;
      mint = 10000.0;
      maxt = -10000.0;
      for(j = 0; j < 36; ++j) {
        xa0=a*cos(j*step);
        ya0=b*sin(j*step);
        xb1=-a*sin(j*step);
        yb1= b*cos(j*step);
        xa1=(xa0*cos(theta)-ya0*sin(theta));
        ya1=(xa0*sin(theta)+ya0*cos(theta));
        
        dx=(xb1*cos(theta)-yb1*sin(theta));
        dy=(xb1*sin(theta)+yb1*cos(theta));
        
        d = sqrt(dx*dx + dy*dy);
        dx = dx/d;
        dy = dy/d;
        d = dx*cs + dy*si;
        if(d < mint) {
          mint = d;
          sw[0][0] = xa1;
          sw[0][1] = ya1;
        }
        if(d > maxt) {
          maxt = d;
          sw[1][0] = xa1;
          sw[1][1] = ya1;
        }
        
        w = sqrt(xa1*xa1 + ya1*ya1);
        d = (xa1*cs + ya1*si)/w;
        if(d < mind) {
          mind = d;
          sl[0][0] = xa1;
          sl[0][1] = ya1;
        }
        if(d > maxd) {
          maxd = d;
          sl[1][0] = xa1;
          sl[1][1] = ya1;
        }
      }
      dx = sw[0][0] - sw[1][0];
      dy = sw[0][1] - sw[1][1];
      dxw = (dx*cs + dy*si)*cs;
      dyw = (dx*cs + dy*si)*si;
      bw = sqrt(dx*dx + dy*dy - dxw*dxw - dyw*dyw);
      dx = sl[0][0] - sl[1][0];
      dy = sl[0][1] - sl[1][1];
      //printf("dx cs %f\n", dx*cs + dy*si);
      l = sqrt(dx*dx + dy*dy);
      rocks[i].rock_width_modeled = (float)bw;
      rocks[i].shadowLength_modeled = (float)l;
    } 
    else
    {
      rocks[i].rock_width_modeled = rocks[i].rock_major*2;
      rocks[i].shadowLength_modeled = (float)(rocks[i].rock_major*2.0);
    }

    rocks[i].rock_height_modeled = (float)(rocks[i].shadowLength_modeled / ta);
  }
  return 1;
}

//int WriteSortedRocks(ROCK *rocks, int maxlab, float minsize, char *filename)
//{
//  int i, nr;
//  float *x, *y, *d;
//  FILE *fp;
//  nr = 0;
//  for(i = 0; i < maxlab; ++i) {
//    if(rocks[i].size >= minsize) {
//      nr++;
//    }
//  }
//  x = (float *)malloc(sizeof(float)*(nr+100));
//  y = (float *)malloc(sizeof(float)*(nr+100));
//  d = (float *)malloc(sizeof(float)*(nr+100));
//  nr = 1;
//  for(i = 0; i < maxlab; ++i) {
//    if(rocks[i].size >= minsize) {
//      //x[nr] = rocks[i].start[0];
//      //y[nr] = rocks[i].start[1];
//      x[nr] = rocks[i].x0;
//      y[nr] = rocks[i].y0;
//      d[nr] = rocks[i].shadowWidth_meter;
//      //d[nr] = rocks[i].rock_width_modeled;
//      //d[nr] = rocks[i].shadowWidth;
//      nr++;
//    }
//  }
//  nr--;
//  sort_rocks(nr, d, x, y);
//  fp = fopen(filename, "w");
//  if(fp == NULL ) return 0;
//  for(i = nr; i >= 1; --i) {
//    fprintf(fp, "%d %f %f %f\n", nr-i, d[i], x[i], y[i]);
//  }
//  fclose(fp);
//
///*
////#define pairhandcount
//#ifdef pairhandcount
//  xh = (float *)malloc(sizeof(float)*(nr+100));
//  yh = (float *)malloc(sizeof(float)*(nr+100));
//  dh = (float *)malloc(sizeof(float)*(nr+100));
//  fp = fopen("test.txt", "r");
//  k = 0;
//  while(fgets(buf, 128, fp) !=NULL)
//    {
//      sscanf(buf, "%f %f %f", &dh[k], &xh[k], &yh[k]);
//      dh[k] = dh[k]/0.31;
//      k++;
//    }
//  fclose(fp);
//  fp = fopen("matched_rock.txt ", "w");
//  for(i = nr; i >0 ; --i)
//    {
//      flag = 0;
//      for(j = 0; j < k; ++j)
//  {
//    dx = x[i] - xh[j];
//    dy = y[i] - yh[j];
//    dis = sqrt(dx*dx + dy*dy);
//    
//    if(dis <= d[i]|| dis <= dh[j])
//      {
//        if(d[i] > dh[j])
//    {
//      ratio = d[i]/dh[j];
//    }
//        else
//    {
//      ratio = dh[j]/d[i];
//    }
//        //found the match
//        if(ratio < 2.0)
//    {
//      flag = 1;
//      break;
//    }
//      }
//  }
//      if(flag == 1)
//  {
//    fprintf(fp, " %f %f %f %f %f %f\n", x[i], y[i], d[i], xh[j], yh[j], dh[j]);
//  }
//    }
//  fclose(fp);
//#endif
//*/
//  free(x);
//  free(y);
//  free(d);
//  return 1;
//}

void sort_rocks(int n, float ra[], float x[], float y[]) {

  long l,j,ir,i;
  float rra;
  float rrx, rry;
  
  l=(n >> 1)+1;
  ir=n;
  for (;;) {
    if (l > 1) {
      rra=ra[--l];
      rrx=x[l];
      rry=y[l];
    } else {
      rra=ra[ir];
      rrx=x[ir];
      rry=y[ir];
      ra[ir]=ra[1];
      x[ir]=x[1];
      y[ir]=y[1];
      if (--ir == 1) {
        ra[1]=rra;
        x[1]=rrx;
        y[1]=rry;
        return;
      }
    }
    i=l;
    j=l << 1;
    while (j <= ir) {
      if (j < ir && ra[j] < ra[j+1]) ++j;
      if (rra < ra[j]) {
        ra[i]=ra[j];
        x[i]=x[j];
        y[i]=y[j];
        j += (i=j);
      }
      else j=ir+1;
    }
    ra[i]=rra;
    x[i]=rrx;
    y[i]=rry;
  }
}


float interpolate(float cd, float c0, float cu) { 
  float h, a, b;

  a = (cu + cd - 2.0f*c0)/2.0f;
  b = (cu - cd)/2.0f;
  if(fabs(a)< 1.0e-6) {
    //printf("ill conditioned peak\n");
    return 0.0;
  }
  h = -b/2/a;
     
  if(fabs(h) > 1.0) {
    return 0.0;
  }
  return h; 
}

//inplace invert of binary (0 or 255) image
void invert_binary_image(unsigned char* image, int rows, int cols)
{
    int imagesize = rows * cols;
    for (int i = 0; i < imagesize; i++)
    {
        assert(image[i] == 0 || image[i] == 255); //expects binary

        if (image[i] == 0)
        {
            image[i] = 1;
        }
        else
        {
            image[i] = 0;
        }
    }
}

int RockDetection(unsigned char *SceneImg, int cols, int rows, 
      RD_PARMS rd_parms, ROCK *rocks, int *numRocks, unsigned short* CC) {

  int imagesize;
  int i, j, m, n;
  unsigned char *OrgImg, *ShaImg;
  float *gx, *gy;
  float *mag;
  int maxlab, lab;
  double angle;
  int p;
  
  // angle appears to be shadow direction, not subsolar azimuth.
  // add a 180 degree offset to azimuth to this angle
  angle = (rd_parms.sun_azimuth_angle + 180.0)*3.1415926/180.0;
  //angle = (rd_parms.sun_azimuth_angle) * 3.1415926 / 180.0;
  imagesize = cols*rows;
    
  OrgImg = (unsigned char *)malloc(sizeof(char)*cols*rows);
  ShaImg = (unsigned char *)malloc(sizeof(char)*cols*rows);

  gx = (float *)malloc(sizeof(float)*cols*rows);
  gy = (float *)malloc(sizeof(float)*cols*rows);
  mag = (float *)malloc(sizeof(float)*cols*rows);

  //save original image, gMET_shadows will alter it
  for (i = 0; i < imagesize; ++i)
  {
      OrgImg[i] = SceneImg[i];
  }

  // Detect shadows:
  gMET_shadows(SceneImg,ShaImg,rows,cols,rd_parms.gamma, rd_parms.gamma_threshold_override);
  
  for(i = 0; i < imagesize; ++i) {
    gx[i] = 0.0;
    gy[i] = 0.0;
  }

  // Generate gradient images
  differentiate(OrgImg, gx, gy, rows, cols);
  for(i = 0; i < imagesize; ++i) {
    mag[i] = (float)sqrt(gx[i]*gx[i] + gy[i]*gy[i]);
  }
  
  //make shadow areas 1, non-shadow areas 0
  invert_binary_image(ShaImg, rows, cols);

  //initialize label image
  if (0 == LabelConnectedRegions(ShaImg, rows, cols, CC, &maxlab))
  {
      printf("Labelling failed");
      return 0;
  }
  
  for(i = 0; i < imagesize; ++i) {
    if(ShaImg[i] == 0) {
      ShaImg[i] = 1;
      CC[i] = 0;

    } else ShaImg[i] = 0;
      //ShaImg[i] -=255;
  }

  maxlab = 0;
   
  for(i = 0; i < imagesize; ++i) {
    if(CC[i] > maxlab) maxlab = CC[i];
  }
  if(maxlab > MAXLAB) {
    printf("too many rocks to store in the buffer\n");
    free(gx);
    free(gy);
    free(mag);
    free(OrgImg);
    free(ShaImg);
    return 0;
  }

  for(i = 0; i < maxlab; ++i) {  // Andres changed to remove = july/2008
    rocks[i].centroid_x = 0;
    rocks[i].centroid_y = 0;
    rocks[i].xmax= 0;
    rocks[i].ymax = 0;
    rocks[i].xmin = cols;
    rocks[i].ymin = rows;
    rocks[i].size = 0;
  }

  for(i = 0; i < rows; ++i) {
    for(j = 0; j < cols; ++j) {
      if(CC[i*cols + j] != 0) {
        lab = CC[i*cols + j];
        rocks[lab].size++;
        rocks[lab].centroid_x +=j;
        rocks[lab].centroid_y +=i;
        if(rocks[lab].xmin > j) rocks[lab].xmin = j;
        if(rocks[lab].ymin > i) rocks[lab].ymin = i;
        if(rocks[lab].xmax < j) rocks[lab].xmax = j;
        if(rocks[lab].ymax < i) rocks[lab].ymax = i;
      }
    }
  }
  
  for(i = 0; i < maxlab; ++i) {
    if(rocks[i].size != 0) {
      rocks[i].centroid_x = rocks[i].centroid_x/rocks[i].size;
      rocks[i].centroid_y = rocks[i].centroid_y/rocks[i].size;
    }

    if(rocks[i].size > rd_parms.max_shadow_size)
    {
      //too big to be considered a rock
      rocks[i].flag= 0;
    }
    else if (rocks[i].size == 0)
    {
        rocks[i].flag = 0;
    }
    else
    {
      rocks[i].flag = 1;
    }
  }

  for(i = 0; i < maxlab; ++i) {
    if(rocks[i].size != 0) {
      p = 0;
      rocks[i].mean_g = 0.0;
      for(m = rocks[i].ymin; m <= rocks[i].ymax; ++m) {
        for(n = rocks[i].xmin; n <= rocks[i].xmax; ++n) {
          if(CC[m*cols + n] == i) {
            if(BorderPixel(CC, cols, rows, m, n, i)) {
              p++;
              rocks[i].mean_g += mag[m*cols + n];
            }
          }
        }
      }
      rocks[i].mean_g = rocks[i].mean_g/p;
      rocks[i].perimeter = (float)p;
    }
  }

  for(i = 0; i < maxlab; ++i) {
    if(rocks[i].size != 0) {
      if(rocks[i].mean_g < rd_parms.mean_gradient_threshold) {
        rocks[i].flag = 0;
        for(m = rocks[i].ymin; m <= rocks[i].ymax; ++m) {
          for(n = rocks[i].xmin; n <= rocks[i].xmax; ++n) {
            if(CC[m*cols + n] == i) {
              CC[m*cols + n] = 0;
            }
          }
        }
      }
    }
  }
  //check the border gx and gy
  //check the shape of the 
  if (0 == SpliteClusteredRock(CC, OrgImg, cols, rows, rocks, &maxlab, &rd_parms))
  {
      printf("splitting failed");
      return 0;
  }

  //redo the statistics  
  for(i = 0; i <= maxlab; ++i) {
    rocks[i].centroid_x = 0;
    rocks[i].centroid_y = 0;
    rocks[i].xmax= 0;
    rocks[i].ymax = 0;
    rocks[i].xmin = cols;
    rocks[i].ymin = rows;
    rocks[i].size = 0;
  }

  for(i = 0; i < rows; ++i) {
    for(j = 0; j < cols; ++j) {
      if(CC[i*cols + j] != 0) {
        lab = CC[i*cols + j];
        rocks[lab].size++;
        rocks[lab].centroid_x +=j;
        rocks[lab].centroid_y +=i;
        if(rocks[lab].xmin > j) rocks[lab].xmin = j;
        if(rocks[lab].ymin > i) rocks[lab].ymin = i;
        if(rocks[lab].xmax < j) rocks[lab].xmax = j;
        if(rocks[lab].ymax < i) rocks[lab].ymax = i;
      }
    }
  }

  for(i = 0; i < maxlab; ++i) {
    if(rocks[i].size != 0) {
      rocks[i].x0 = (float)rocks[i].centroid_x/(float)rocks[i].size;
      rocks[i].y0 = (float)rocks[i].centroid_y/(float)rocks[i].size;
      rocks[i].centroid_x = rocks[i].centroid_x/rocks[i].size;
      rocks[i].centroid_y = rocks[i].centroid_y/rocks[i].size;
      rocks[i].flag = 1;
    }
    if(rocks[i].size > rd_parms.max_shadow_size) {
      rocks[i].flag  = 0;
    }
  }

  for(i = 0; i < maxlab; ++i) {
    if(rocks[i].size != 0 && rocks[i].flag == 1) {
      p = 0;
      rocks[i].mean_g = 0.0;
      for(m = rocks[i].ymin; m <= rocks[i].ymax; ++m) {
        for(n = rocks[i].xmin; n <= rocks[i].xmax; ++n) {
          if(CC[m*cols + n] == i) {
            if(BorderPixel(CC, cols, rows, m, n, i)) {
              p++;
              rocks[i].mean_g += mag[m*cols + n];
            }
          }
        }
      }
      rocks[i].mean_g = rocks[i].mean_g/p;
      rocks[i].perimeter = (float)p;
    }
  }

  MeasureShadowsLengthandWidth(CC, OrgImg, gx, gy, cols, rows, rocks, maxlab,
                               (int)rd_parms.min_shadow_size, angle);
  RocksModeling(CC, cols, rows, rocks, maxlab, (int)rd_parms.min_shadow_size);
  RocksScores(rocks, maxlab, &rd_parms);
  RockHeightWidth(rocks, maxlab, &rd_parms);
  FilterRocks(rocks, maxlab, &rd_parms);

  *numRocks = maxlab;

  free(gx);
  free(gy);
  free(mag);
  free(OrgImg);
  free(ShaImg);
  return 1; //SUCCESS
}

int brightnessCorrection(unsigned char *subset, int cols, int rows) {

  int i, k, j; 
  double mean;
  mean = 0;
  for(i = 0; i < cols*rows; ++i) {
    mean +=subset[i];
  }
  mean = mean/(float)(cols*rows);
  if(mean < 120) {
    k = 120-(int)mean;
    for(i = 0; i < cols*rows; ++i) {
      j =subset[i] + k;
      if(j > 255) j = 255;
      subset[i] = (unsigned char)j;
    }
  }
  return 1;
}

int BorderPixel(unsigned short *CC, int cols, int rows, int n, int m, int i) {

  if(m <= 0 || n <= 0 || n >= rows-1 || m >= cols -1) return 1;
  if(CC[n*cols + m -1] != i) return 1;
  else if(CC[n*cols + m+1] != i) return 1;
  else if(CC[(n-1)*cols + m] != i) return 1;
  else if(CC[(n+1)*cols + m] != i) return 1;
  return 0;
}
