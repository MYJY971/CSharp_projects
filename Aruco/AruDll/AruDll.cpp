#include "AruDll.h"

using namespace std;
using namespace cv;
using namespace aruco;
namespace AruDll 
{
	DLL_EXPORT double Add(double a, double b)
	{
		return a + b + 4;
	}

	DLL_EXPORT int TestARCPP(Mat image, char * path_CamPara)
	{
		_set_error_mode(_OUT_TO_STDERR);
		try
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

			int res = theMarkers.size();/*

			//Mat treshIm = mDetector.getThresholdedImage();

			
			//theUndInputImage.deallocate();
			delete(&mDetector);
			delete(&theMarkers);
			delete(&theUndInputImage);
			delete(&theInputImage);
			delete(&image);*/
			return 1;//res;


		}
		catch (const std::exception&)
		{
			printf("OH NO !!! EXCEPTION ! >_<");
			return 1;
		}

	}

	DLL_EXPORT void PerformARMarker(char image[], char * path_CamPara, int imageWidth, int imageHeight, int glWidth, int glHeight,
		double gnear, double gfar, double proj_matrix[16], double modelview_matrix[16],
		float markerSize, int &nbDetectedMarkers, int tresh1, int tresh2)
	{
		MarkerDetector mDetector;
		vector<Marker> theMarkers;
		Mat theInputImage, theUndInputImage;
		theInputImage = Mat(imageHeight, imageWidth, CV_8UC3, image);

		CameraParameters theCameraParameters;
		theCameraParameters.readFromXMLFile(path_CamPara);
		theCameraParameters.resize(theInputImage.size());

		theCameraParameters.glGetProjectionMatrix(theInputImage.size(), Size(glWidth, glHeight), proj_matrix, gnear, gfar);

		float theMarkerSize = markerSize;

		//tresh1 : dilatation, tresh2 : erosion (couleur de réference : blanc)
		mDetector.setThresholdParams(tresh1, tresh2);

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