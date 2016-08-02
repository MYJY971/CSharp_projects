#include <iostream>
#include <fstream>
#include <sstream>
#include <opencv2/highgui/highgui.hpp>
#include "AruDll.h"

using namespace std;
using namespace cv;
using namespace AruDll;

int main(int argc, char **argv)
{

	
	VideoCapture vreader(0); // open the video file for reading

	cv::Mat InImage;

	if (vreader.isOpened()) vreader >> InImage;

	char* path = "C:\\Stage\\Yanis\\Libraries\\Aruco\\aruco-test-data-2.0\\1_single\\intrinsics.yml";
	char* pathMap = "C:\\Stage\\Yanis\\Libraries\\Aruco\\aruco-test-data-2.0\\2_markermap\\map4.yml";
	int nbM;
	int result = 0;
	double projMat[16];
	double lookat[16];

	result = TestARCPP(InImage, path); //Add(5, 5);

	cout << "SUCESS -->" << result << endl;

	/*result = TestARCPP(InImage, path, 0);

	cout << "SUCESS -->" << result << endl;*/
	system("PAUSE");

	return 0;

}