#ifndef AH_ROCKSH
#define AH_ROCKSH

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

#ifdef _WIN32
#define EXPORT __declspec(dllexport)
#else
#define EXPORT
#endif

/*******************************************************************************
 AH_Rocks.h

 Author: Andres Huertas - Jet Propulsion Laboratory - Vision Group

 Project: MTP/AEDL Project (PI: Yang Chen, JPL Vision Group)

 History:
   01/22/06: A. Huertas. Created.
 
 Usage:
   header file to AH_Rocks.c
*****************************************************************************/

#define VERBOSE   1

#define TRUE      1
#define FALSE     0
#define BITD      8
#define HISTLEN 256
#define Double_MIN_VALUE 4.9E-324
#define Double_MAX_VALUE 1.7976931348623157E308

#define MASKFR    0
#define MASKBK  255
#define INSIDE    0
#define OUTSIDE 255

#define MAXLAB 20000

#define RD_GAMMA				2.5			/*1-10*/
#define RD_SUN_INCIDENCE_ANGLE	55			/*20-70 degree*/
#define RD_SUN_AZIMUTH_ANGLE	215			/*0-360 degree CCW from x axis*/
#define RD_MIN_SHADOW_AREA		5			/*1-20 pixels*/
#define RD_GROUND_RES			0.31		/*meter*/
#define RD_CONFIDENCE_THRESHOLD 0.6			/*0-1.0*/
#define RD_ROCK_SPLITING_RATIO  0.5          /*0.3-0.7*/
#define RD_MIN_ROCK_SIZE_SPLIT  20           /*10-??? pixels*/

typedef struct _RD_PARMS
{
	float gamma;
	float sun_incidence_angle;
	float sun_azimuth_angle;
	float min_shadow_size;
	float ground_resolution;
	float confidence_threshold;
	float min_shadow_size_split;
	float spliting_ratio;
	float rock_elongate_ratio;
	float mean_gradient_threshold;
	float max_shadow_size;
	int   gamma_threshold_override;
}RD_PARMS;

//tile rock
typedef struct _ROCK
{
	char flag;					// 0 means invalid
	int label;					// a unique identifier, changes run to run in parallel execution 
	int xmin;					// tile relative min column of shadow bounds
	int xmax;					// tile relative max column of shadow bounds
	int ymin;					// tile relative min column of shadow bounds
	int ymax;					// tile relative max column of shadow bounds
	int size;					// size of shadow area
	int centroid_x;				// tile relative column pixel location of shadow center (may not be within shadow if non convex)
	int centroid_y;				// tile relative row pixel location of shadow center (may not be within shadow if non convex)
	float x0, y0;				// tile relative column, row, subpixel location of shadow center  (may not be within shadow if non convex)
	float mean_g;				// the mean of the magnitude of the gradient on the perimeter pixels of the shadow region
	float perimeter;			// count of the pixels that are shadow border pixels for big rocks, estimated perimeter from radius of small rocks
	float start[2];				// tile relative (col, row) start is the location of the shadow on the rock, in the light direction, through the shadow centroid
	float end[2];				// tile relative (col, row) end is the location of the shadow farthest from the rock, in the light direction, through the shadow centroid
	float g_start;				// gradient magnitude at start location
	float g_end;				// gradient magnitude at end location
	float shadowLength;			// shadow length along the line between start and end
	float shadowWidth;  
	float shadowLength_modeled;
	float rock_height_modeled;	// height estimated from sun incidence angle and modeled shadow length 
	float confidence;			// a score assigned based on how well the rock's gradient matches a large rock (reference rock)
	float rock_major;			// rock ellipse major axis
	float rock_minor;			// rock ellipse minor axis
	float rock_theta;			// angle of rock ellipse
	float rock_width_modeled;
	int tile_row;				// tile address row (enables calculating absolute pixel addresses)
	int tile_col;				// tile address col (enables calculating absolute pixel addresses)
}ROCK;

void clear_rock(ROCK* rock);

int getsubset(unsigned char *SceneImg, int fcols, int frows, unsigned char *subimg, 
        int cols, int rows, int start_col, int end_col, int start_row, int end_row);

int gMET_shadows(unsigned char *SceneImg, unsigned char *ShaImg,
			  int rows, int cols, float MetGamma, int gamma_threshold_override);

void get_gamma_image(unsigned char* SceneImg, unsigned char* GammaImg,
					 int rows, int cols, float MetGamma);
void get_binary_image(unsigned char* src_img, unsigned char* dst_img, int rows, int cols, int threshold);

void differentiate(unsigned char *img, float *gx, float *gy, int rows, int cols);

int BorderOrInside(unsigned short *CC, int cols, int rows, int lab, int ix, int iy);
int RocksModeling(unsigned short *CC, int cols, int rows, ROCK *rocks, int maxlab, int MinSize);
int MeasureShadowsLengthandWidth(unsigned short *CC, unsigned char *OrgImg,
								 float *gx, float *gy, int cols, int rows, ROCK *rocks, 
								 int maxlab, int MinSize, double angle);
int SpliteClusteredRock(unsigned short *CC, unsigned char *OrgImg, 
						int cols, int rows, ROCK *rocks, int *maxlab,
						RD_PARMS *rd_parms);
int RocksScores(ROCK *rocks, int maxlab, RD_PARMS *rd_parms);

int RockHeightWidth(ROCK *rocks, int maxlab, RD_PARMS *rd_parms );
void sort_rocks(int n, float ra[], float x[], float y[]);

float interpolate(float cd, float c0, float cu);
int RockDetection(unsigned char *SceneImg, int cols, int rows, 
				  RD_PARMS rd_parms, ROCK *rocks, int *numRocks, unsigned short* CC);
int BorderPixel(unsigned short *CC, int cols, int rows, int n, int m, int i);
int brightnessCorrection(unsigned char *subset, int cols, int rows)	;
void invert_binary_image(unsigned char* image, int rows, int cols);
#ifdef __cplusplus
}
#endif /* __cplusplus */
#endif /* AH_ROCKSH*/
