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
namespace AruDll
{
	DLL_EXPORT double Add(double a, double b);
	DLL_EXPORT int TestARCPP(Mat image, char *path_CamPara);

}