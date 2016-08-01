#include "AruDll.h"

using namespace std;
using namespace cv;
using namespace aruco;
namespace AruDll 
{
	DLL_EXPORT double Add(double a, double b)
	{
		return a + b;
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

			int res = theMarkers.size();

			//Mat treshIm = mDetector.getThresholdedImage();
			return res;


		}
		catch (const std::exception&)
		{
			printf("OH NO !!! EXCEPTION ! >_<");
			return 0;
		}

	}
}