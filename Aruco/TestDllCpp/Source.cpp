#include <iostream>
#include <fstream>
#include <sstream>
#include <opencv2/highgui/highgui.hpp>
#include "ArucoDll.h"

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
	
	double result = 0;
	result = TestARCPP(InImage, path);

	/*try {
		result = TestAR(InImage, path);
	}
	catch (Exception e)
	{
		cout << "ERROR" << endl;
	}*/

    cout << "SUCESS -->" << result << endl;
	//cout << TestAR(InImage, path) << endl;
	//system("PAUSE");
	return 0;
}