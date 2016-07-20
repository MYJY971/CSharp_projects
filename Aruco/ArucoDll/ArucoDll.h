#pragma once
#include <stdexcept>
#include <Windows.h>
#include <opencv/cv.h>
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/calib3d/calib3d.hpp>
#include "aruco.h"

using namespace std;
using namespace cv;

#define DLL_EXPORT extern "C" __declspec(dllexport)

namespace ArucoDll
{
	//extern "C" { __declspec(dllexport) double Add(double a, double b); }
	DLL_EXPORT double Add(double a, double b);
	DLL_EXPORT double TestARCPP(Mat image, char *path_CamPara);
	DLL_EXPORT int TestAR(char image[], int imageWidth, int imageHeight, char * path_CamPara);
	DLL_EXPORT void PerformARMarkers(char image[], char * path_CamPara, int imageWidth, int imageHeight, int glWidth, int glHeight,
							  double gnear, double gfar, double proj_matrix[16], double modelview_matrix[16], 
							  float markerSize, int &nbDetectedMarkers);
	DLL_EXPORT void PerformAR(char image[], char* path_mapPara, char * path_CamPara, int imageWidth, int imageHeight, int glWidth, int glHeight,
		double gnear, double gfar, double proj_matrix[16], double modelview_matrix[16],
		float markerSize, int &nbDetectedMarkers);
	DLL_EXPORT void TestARCpp2(Mat image, char* path_mapPara, char * path_CamPara, int glWidth, int glHeight,
		double gnear, double gfar, double proj_matrix[16], double modelview_matrix[16],
		float markerSize, int &nbDetectedMarkers);
}