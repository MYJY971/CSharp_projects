#include <iostream>
#include <fstream>
#include <sstream>
#include <opencv2/highgui/highgui.hpp>
#include "ArucoDll.h"
#include "AruDll.h"

using namespace std;
using namespace ArucoDll;
using namespace cv;

int main(int argc, char **argv)
{
	
	//string video_path = "C:/Stage/Yanis/CSharp_projects/Librairies/ARUCO/aruco-2.0.7/MY-Build/aruco-test-data-2.0/1_single/video.avi";
	VideoCapture vreader(0); // open the video file for reading

	cv::Mat InImage;

	if (vreader.isOpened()) vreader >> InImage;

	char* path = "C:\\Stage\\Yanis\\Libraries\\Aruco\\aruco-test-data-2.0\\1_single\\intrinsics.yml";
	char* pathMap = "C:\\Stage\\Yanis\\Libraries\\Aruco\\aruco-test-data-2.0\\2_markermap\\map4.yml";
	int nbM;
	int result = 0;
	double projMat[16];
	double lookat[16];
	//TestARCpp2(InImage, pathMap, path, 640, 480, 0.1, 100, projMat, lookat, 0.5, nbM);
	//result = TestARCPP(InImage, path);
	result = AruDll::TestARCPP(InImage, path); //Add(5, 5);

	/*try {
		result = TestAR(InImage, path);
	}
	catch (Exception e)
	{
		cout << "ERROR" << endl;
	}*/

    cout << "SUCESS -->" << result << endl;
	//cout << TestAR(InImage, path) << endl;
	system("PAUSE");
	return 0;
	
}