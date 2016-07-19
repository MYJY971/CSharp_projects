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

	DLL_EXPORT void DetectMarkers(char image[], int imageWidth, int imageHeight, char * path_CamPara, float markerSize, int &nbDetectedMarkers, double modelview_matrix[16])
	{
		MarkerDetector mDetector;
		vector<Marker> theMarkers;
		Mat theInputImage, theUndInputImage;
		theInputImage = Mat(imageHeight, imageWidth, CV_8UC3, image);

		CameraParameters theCameraParameters;
		theCameraParameters.readFromXMLFile(path_CamPara);
		theCameraParameters.resize(theInputImage.size());

		float theMarkerSize = markerSize;

		//image captured
		theUndInputImage.create(theInputImage.size(), CV_8UC3);
		//transform color that by default is BGR to RGB because windows systems do not allow reading BGR images with opengl properly
		cv::cvtColor(theInputImage, theInputImage, CV_BGR2RGB);
		//remove distortion in image
		cv::undistort(theInputImage, theUndInputImage, theCameraParameters.CameraMatrix, theCameraParameters.Distorsion);
		//detect markers
		mDetector.detect(theUndInputImage, theMarkers, theCameraParameters.CameraMatrix, Mat(), theMarkerSize, false);

		for (unsigned int i = 0; i < theMarkers.size(); i++)
		{
			theMarkers[i].glGetModelViewMatrix(modelview_matrix);
			break;
		}
		/*if(theMarkers.size()>0)
		theMarkers[0].glGetModelViewMatrix(modelview_matrix);*/

		nbDetectedMarkers = theMarkers.size();
		//return theMarkers.size();
	}

	DLL_EXPORT void GetGLProjection(char * path_CamPara, int imageWidth, int imageHeight, int glWidth, int glHeight,
		double gnear, double gfar, double proj_matrix[16])
	{
		Size inputImageSize = Size(imageWidth, imageHeight);
		Size glSize = Size(glWidth, glHeight);


		CameraParameters theCameraParameters;
		theCameraParameters.readFromXMLFile(path_CamPara);
		theCameraParameters.resize(inputImageSize);

		theCameraParameters.glGetProjectionMatrix(inputImageSize, glSize, proj_matrix, gnear, gfar);
	}

	DLL_EXPORT void PerformARMarkers(char image[], char * path_CamPara, int imageWidth, int imageHeight, int glWidth, int glHeight,
		double gnear, double gfar, double proj_matrix[16], double modelview_matrix[16],
		float markerSize, int &nbDetectedMarkers)
	{
		MarkerDetector mDetector;
		vector<Marker> theMarkers;
		Mat theInputImage, theUndInputImage;
		theInputImage = Mat(imageHeight, imageWidth, CV_8UC3, image);

		CameraParameters theCameraParameters;
		theCameraParameters.readFromXMLFile(path_CamPara);
		theCameraParameters.resize(theInputImage.size());

		theCameraParameters.glGetProjectionMatrix(theInputImage.size(), Size(glWidth,glHeight), proj_matrix, gnear, gfar);

		float theMarkerSize = markerSize;

		//image captured
		theUndInputImage.create(theInputImage.size(), CV_8UC3);
		//transform color that by default is BGR to RGB because windows systems do not allow reading BGR images with opengl properly
		cv::cvtColor(theInputImage, theInputImage, CV_BGR2RGB);
		//remove distortion in image
		cv::undistort(theInputImage, theUndInputImage, theCameraParameters.CameraMatrix, theCameraParameters.Distorsion);
		//detect markers
		mDetector.detect(theUndInputImage, theMarkers, theCameraParameters.CameraMatrix, Mat(), theMarkerSize, false);

		for (unsigned int i = 0; i < theMarkers.size(); i++)
		{
			theMarkers[i].glGetModelViewMatrix(modelview_matrix);
			break;
		}
		/*if(theMarkers.size()>0)
		theMarkers[0].glGetModelViewMatrix(modelview_matrix);*/

		nbDetectedMarkers = theMarkers.size();
	}


}