/*******************************************************************************
 AH_RockModel.c 

 Author:
    Andres Huertas - Jet Propulsion Laboratory

 Sponsor:
    Mars Technology Program (AEDL) - Yang Cheng, PI

 History: 
    01/23/06: A. Huertas. Created from original version in Java.

 Usage:
    Module to AH_Rocks.c

*******************************************************************************/

#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <assert.h>
#include "AH_RockModel.h"

typedef struct {
  long id;
  float area;
  long  xloc; 
  long  yloc; 
  float length;  
  float width;  
  float angle;  
  float perimeter;
  float height;
} Rock;

double PI      = 3.1415926535897;
double HALFPI  = 1.5707963267949;

int npoints = 0,  nfrags  = 0, shape = CIRCULAR;
int xpoints[maxPoints];
int ypoints[maxPoints];

double xCenter, yCenter, xI, yI;
double major, minor, angle, theta;    
int bitCount=0, pixelCount = 0;
double xsum, ysum, x2sum, y2sum, xysum;
double txsum, tysum, tx2sum, ty2sum, txysum;
int left, top, width, height, xleft, xrite; //BB of shadow
double nn, xm, ym, pw, ph;     
double u20, u02, u11;   // central moments
int BBt, BBb, BBl, BBr, rockPer; // bounding box of rock model
int xStart, yStart, intSamples=0;

double aveInt = 0.0;    // for intensity stats later

Rock rockFrag[maxRocks];

// A few utils:

double deg2rad(double xDegrees) { return xDegrees*(PI/180.0); }

double sqr(double x) { return x*x; }

double getMag(double x1, double y1, double x2, double y2) {
  return sqrt(sqr(x2-x1)+sqr(y2-y1));
}

int LabelConnectedRegions(unsigned char  *B, int rows, int cols, unsigned short *CC, int* numRegions)
{
  *numRegions = 0;
  int r,c,l,kmax,kmin;
  
  int nc = cols;
  int nr = rows;
  int maxNumBlobs = nc*nr;
  int maxValidLabel = (int)pow(2, 17)-1;

  int *h = (int *)malloc(sizeof(int)*maxNumBlobs); // label equivalency
  for (int k = 1; k < maxNumBlobs; k++)
  {
      h[k] = k;
  }
  int k = 0;
  
  //intialize labels
  int imagesize = cols * rows;
  for (int i = 0; i < imagesize; ++i)
  {
      CC[i] = 0;
  }

  //first label borders 
  // top left corner
  if (B[0*cols+0]>0) 
  {
      k++; 
      if (k > maxValidLabel)
      {
          return 0; //FAIL, to many labels for short
      }
      CC[cols*0+0] = (unsigned short)k;
  } 
  
  // top
  for(c=1;c<nc;c++) if (B[cols*0+c]>0)
  {
    if (B[0*cols +c-1]==0)
    {
        // new region
        k++; 
        if (k > maxValidLabel)
        {
            return 0; //FAIL, to many labels for short
        }
        CC[0*cols+c] = (unsigned short)k;
    } 
    else
    {
        CC[0 * cols + c] = CC[cols * 0 + c - 1];
    }
  } 
  // left
  for(r=1;r<nr;r++) if (B[cols*r+0]>0)
  {
    if (B[(r-1)*cols+0]==0)
    {   
        // new region
        k++; 
        if (k > maxValidLabel)
        {
            return 0; //FAIL, to many labels for short
        }
        CC[r*cols+0] = (unsigned short)k;
    }
    else
    {
        CC[r * cols + 0] = CC[cols * (r - 1) + 0];
    }
  }
  
  // initial assignment;
  for(r=1;r<nr;r++) 
    for(c=1;c<nc;c++) {
      if (B[r*cols+c]==0) CC[r*cols+c]=0;
      else { 
    if ((B[(r-1)*cols+c]==0)&&(B[r*cols + c-1]==0)) {
      k++;
      if (k > maxValidLabel)
      {
          return 0; //FAIL, to many labels for short
      }
      CC[r*cols +c] = (unsigned short)k;
      if (k>=maxNumBlobs) printf("More blobs than expected");
    }
    else {
      if (B[r*cols + c-1]==0) CC[r*cols +c] = CC[(r-1)*cols+c];
      else {
        if (B[(r-1)*cols +c]==0) CC[r*cols + c] = CC[r*cols + c-1];
        else {
          if (CC[(r-1)*cols +c]==CC[r*cols +c-1]) CC[r*cols +c] = CC[(r-1)*cols +c];
          else {
            CC[r*cols +c] = CC[(r-1)*cols + c];
            kmin = MIN(h[CC[(r-1)*cols + c]],h[CC[r*cols +c-1]]); // reassign works 
            kmax = MAX(h[CC[(r-1)*cols + c]],h[CC[r*cols + c-1]]); // swapping did not
            for(int i=kmin;i<=k;i++) if (h[i] == kmax) h[i] = kmin;
          }
        }
      }
    }
      }
    }
  
  // pack labels
  l = 1; 
  for(int p=1;p<=k;p++) {
    if (h[p]==p) {
      h[p] = l;
      l++;
      
      if (l > maxValidLabel)
      {
          return 0; //FAIL, to many labels for short
      }
    }
    else
    {
        h[p] = h[h[p]];
    }
  }

  // label image
  
  for (r = 0; r < nr; r++)
  {
      for (c = 0; c < nc; c++)
      {
          if (B[r * cols + c] > 0)
          {
              int newLabel = h[CC[r * cols + c]];
              assert(newLabel < maxValidLabel);
              CC[r * cols + c] = (unsigned short)newLabel;
          }
      }
  }
  free(h);  
  *numRegions = l-1;
  return 1; //SUCCESS

}
