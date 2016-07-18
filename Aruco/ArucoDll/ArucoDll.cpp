#include "ArucoDll.h"

using namespace std;
using namespace cv;
using namespace aruco;
/*
BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	switch (fdwReason)
	{
	case DLL_PROCESS_ATTACH:
		// attach to process
		// return FALSE to fail DLL load
		break;

	case DLL_PROCESS_DETACH:
		// detach from process
		break;

	case DLL_THREAD_ATTACH:
		// attach to thread
		break;

	case DLL_THREAD_DETACH:
		// detach from thread
		break;
	}
	return TRUE; // succesful
}

*/

namespace ArucoDll
{

	//CameraParameters camPara;
	DLL_EXPORT double Add(double a, double b)
	{
		return a + b;
	}

	DLL_EXPORT double TestARCPP(Mat image, char * path_CamPara)
	{
		MarkerDetector mDetector;
		vector<Marker> theMarkers;
		Mat theInputImage, theUndInputImage;
		theInputImage = image;//Mat(imageHeight, imageWidth, CV_8UC3, image);

		CameraParameters theCameraParameters;
		theCameraParameters.readFromXMLFile(path_CamPara);
		theCameraParameters.resize(theInputImage.size());

		float theMarkerSize = 0.05f;

		//image captured
		theUndInputImage.create(theInputImage.size(), CV_8UC3);
		//transform color that by default is BGR to RGB because windows systems do not allow reading BGR images with opengl properly
		cv::cvtColor(theInputImage, theInputImage, CV_BGR2RGB);
		//remove distortion in image
		cv::undistort(theInputImage, theUndInputImage, theCameraParameters.CameraMatrix, theCameraParameters.Distorsion);
		//detect markers
		mDetector.detect(theUndInputImage, theMarkers, theCameraParameters.CameraMatrix, Mat(), theMarkerSize, false);

		return double(theMarkers.size());
	}

	DLL_EXPORT int TestAR(char image[],int imageWidth,int imageHeight, char * path_CamPara)
	{
		MarkerDetector mDetector;
		vector<Marker> theMarkers;
		Mat theInputImage, theUndInputImage;
		theInputImage = Mat(imageHeight, imageWidth, CV_8UC3, image);

		CameraParameters theCameraParameters;
		theCameraParameters.readFromXMLFile(path_CamPara);
		theCameraParameters.resize(theInputImage.size());

		float theMarkerSize = 0.05f;

		//image captured
		theUndInputImage.create(theInputImage.size(), CV_8UC3);
		//transform color that by default is BGR to RGB because windows systems do not allow reading BGR images with opengl properly
		cv::cvtColor(theInputImage, theInputImage, CV_BGR2RGB);
		//remove distortion in image
		cv::undistort(theInputImage, theUndInputImage, theCameraParameters.CameraMatrix, theCameraParameters.Distorsion);
		//detect markers
		mDetector.detect(theUndInputImage, theMarkers, theCameraParameters.CameraMatrix, Mat(), theMarkerSize, false);

		return theMarkers.size();
	}

	


}