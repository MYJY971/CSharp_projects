#include "ArucoDll.h"
#include <opencv2/highgui/highgui.hpp>

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
			//mDetector.detect(theUndInputImage, theMarkers, theCameraParameters.CameraMatrix, Mat(), theMarkerSize, false);
			
			double res = 0;// (double)theMarkers.size();
			
			//Mat treshIm = mDetector.getThresholdedImage();
			return res;


		}
		catch (const std::exception&)
		{
			return 0;
		}
		
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
		//mDetector.detect(theUndInputImage, theMarkers, theCameraParameters.CameraMatrix, Mat(), theMarkerSize, false);
		(byte)mDetector.getThresholdedImage().data;
		return theMarkers.size();
	}

/////////



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

		theCameraParameters.glGetProjectionMatrix(theInputImage.size(), Size(glWidth,glHeight), proj_matrix, gnear, gfar);

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

	///////
	//void  __glGetModelViewMatrix(double modelview_matrix[16], const cv::Mat &Rvec, const cv::Mat &Tvec) throw(cv::Exception);

	void  __glGetModelViewMatrix(double modelview_matrix[16], const cv::Mat &Rvec, const cv::Mat &Tvec) throw(cv::Exception) {
		assert(Tvec.type() == CV_32F);
		// check if paremeters are valid
		Mat Rot(3, 3, CV_32FC1);
		Rodrigues(Rvec, Rot);

		double para[3][4];
		for (int i = 0; i < 3; i++)
			for (int j = 0; j < 3; j++)
				para[i][j] = Rot.at< float >(i, j);
		// now, add the translation
		para[0][3] = Tvec.ptr< float >(0)[0];
		para[1][3] = Tvec.ptr< float >(0)[1];
		para[2][3] = Tvec.ptr< float >(0)[2];
		double scale = 1;

		modelview_matrix[0 + 0 * 4] = para[0][0];
		// R1C2
		modelview_matrix[0 + 1 * 4] = para[0][1];
		modelview_matrix[0 + 2 * 4] = para[0][2];
		modelview_matrix[0 + 3 * 4] = para[0][3];
		// R2
		modelview_matrix[1 + 0 * 4] = para[1][0];
		modelview_matrix[1 + 1 * 4] = para[1][1];
		modelview_matrix[1 + 2 * 4] = para[1][2];
		modelview_matrix[1 + 3 * 4] = para[1][3];
		// R3
		modelview_matrix[2 + 0 * 4] = -para[2][0];
		modelview_matrix[2 + 1 * 4] = -para[2][1];
		modelview_matrix[2 + 2 * 4] = -para[2][2];
		modelview_matrix[2 + 3 * 4] = -para[2][3];
		modelview_matrix[3 + 0 * 4] = 0.0;
		modelview_matrix[3 + 1 * 4] = 0.0;
		modelview_matrix[3 + 2 * 4] = 0.0;
		modelview_matrix[3 + 3 * 4] = 1.0;
		if (scale != 0.0) {
			modelview_matrix[12] *= scale;
			modelview_matrix[13] *= scale;
			modelview_matrix[14] *= scale;
		}

	}



	DLL_EXPORT void PerformAR(char image[], char* path_mapPara, char * path_CamPara, int imageWidth, int imageHeight, int glWidth, int glHeight,
		double gnear, double gfar, double proj_matrix[16], double modelview_matrix[16],
		float markerSize, int &nbDetectedMarkers)
	{
		bool the3DInfoAvailable = false;
		float theMarkerSize = -1;
		
		vector<Marker> theMarkers;
		//board
		Mat theInputImage, theUndInputImage, theResizedImage;
		CameraParameters theCameraParams;
		Size theGlWindowSize;
		MarkerMap theMMConfig;
		MarkerDetector theMarkerDetector;
		MarkerMapPoseTracker mmPoseTracker;

		
		
		theCameraParams.readFromXMLFile(path_CamPara);

		//read board configuration
		theMMConfig.readFromFile(path_mapPara);
		theMarkerSize = markerSize;
		if (theMMConfig.isExpressedInPixels())
		{
			theMMConfig = theMMConfig.convertToMeters(theMarkerSize);
		}
		mmPoseTracker.setParams(theCameraParams, theMMConfig);

		//read image
		theInputImage = Mat(imageHeight, imageWidth, CV_8UC3, image);

		//read camera parameters
		theCameraParams.readFromXMLFile(path_CamPara);
		theCameraParams.resize(theInputImage.size());

		theMarkerDetector.setThresholdParams(25, 7);

		MarkerDetector::Params params;
		//play with this paramteres if the detection does not work correctly
		params._borderDistThres = .01;//acept markers near the borders
		params._thresParam1 = 5;
		params._thresParam1_range = 10;//search in wide range of values for param1
		params._cornerMethod = MarkerDetector::SUBPIX;//use subpixel corner refinement
		params._subpix_wsize = (15. / 2000.)*float(theInputImage.cols);//search corner subpix in a 5x5 widow area
		theMarkerDetector.setParams(params);//set the params above
		theMarkerDetector.setDictionary(theMMConfig.getDictionary());

		//

		theUndInputImage.create(theInputImage.size(), CV_8UC3);
		//by deafult, opencv works in BGR, so we must convert to RGB because OpenGL in windows preffer
		cv::cvtColor(theInputImage, theInputImage, CV_BGR2RGB);
		//remove distorion in image
		cv::undistort(theInputImage, theUndInputImage, theCameraParams.CameraMatrix, theCameraParams.Distorsion);
		//detect markers
		theMarkers = theMarkerDetector.detect(theUndInputImage);
		mmPoseTracker.estimatePose(theMarkers);

		//

		theCameraParams.glGetProjectionMatrix(theInputImage.size(), Size(glWidth, glHeight), proj_matrix, gnear, gfar);
		if (!mmPoseTracker.getRTMatrix().empty()) {
			//__glGetModelViewMatrix(modelview_matrix, mmPoseTracker.getRvec(), mmPoseTracker.getTvec());
			theMarkers[0].glGetModelViewMatrix(modelview_matrix);
		}

		nbDetectedMarkers = theMarkers.size();
	}


	//TEST/////////////////////////////
	DLL_EXPORT void TestARCpp2(Mat image, char* path_mapPara, char * path_CamPara, int glWidth, int glHeight,
		double gnear, double gfar, double proj_matrix[16], double modelview_matrix[16],
		float markerSize, int &nbDetectedMarkers)
	{
		bool the3DInfoAvailable = false;
		float theMarkerSize = -1;

		vector<Marker> theMarkers;
		//board
		Mat theInputImage, theUndInputImage, theResizedImage;
		CameraParameters theCameraParams;
		Size theGlWindowSize;
		MarkerMap theMMConfig;
		MarkerDetector theMarkerDetector;
		MarkerMapPoseTracker mmPoseTracker;



		theCameraParams.readFromXMLFile(path_CamPara);

		//read board configuration
		theMMConfig.readFromFile(path_mapPara);
		theMarkerSize = markerSize;
		if (theMMConfig.isExpressedInPixels())
		{
			theMMConfig = theMMConfig.convertToMeters(theMarkerSize);
		}
		mmPoseTracker.setParams(theCameraParams, theMMConfig);

		//read image
		theInputImage = image;

		//read camera parameters
		theCameraParams.readFromXMLFile(path_CamPara);
		theCameraParams.resize(theInputImage.size());

		theMarkerDetector.setThresholdParams(25, 7);

		MarkerDetector::Params params;
		//play with this paramteres if the detection does not work correctly
		params._borderDistThres = .01;//acept markers near the borders
		params._thresParam1 = 5;
		params._thresParam1_range = 10;//search in wide range of values for param1
		params._cornerMethod = MarkerDetector::SUBPIX;//use subpixel corner refinement
		params._subpix_wsize = (15. / 2000.)*float(theInputImage.cols);//search corner subpix in a 5x5 widow area
		theMarkerDetector.setParams(params);//set the params above
		theMarkerDetector.setDictionary(theMMConfig.getDictionary());

		//

		theUndInputImage.create(theInputImage.size(), CV_8UC3);
		//by deafult, opencv works in BGR, so we must convert to RGB because OpenGL in windows preffer
		cv::cvtColor(theInputImage, theInputImage, CV_BGR2RGB);
		//remove distorion in image
		cv::undistort(theInputImage, theUndInputImage, theCameraParams.CameraMatrix, theCameraParams.Distorsion);
		//detect markers
		theMarkers = theMarkerDetector.detect(theUndInputImage);
		mmPoseTracker.estimatePose(theMarkers);

		//

		theCameraParams.glGetProjectionMatrix(theInputImage.size(), Size(glWidth, glHeight), proj_matrix, gnear, gfar);
		if (!mmPoseTracker.getRTMatrix().empty()) {
			__glGetModelViewMatrix(modelview_matrix, mmPoseTracker.getRvec(), mmPoseTracker.getTvec());
		}


	}

}