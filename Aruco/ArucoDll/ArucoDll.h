#pragma once
#include <stdexcept>
#include <Windows.h>
#include <opencv/cv.h>
#include <opencv2/imgproc/imgproc.hpp>
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

}