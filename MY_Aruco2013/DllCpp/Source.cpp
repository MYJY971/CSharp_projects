#include "Header.h"


using namespace std;



//BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
//{
//	switch (fdwReason)
//	{
//	case DLL_PROCESS_ATTACH:
//		// attach to process
//		// return FALSE to fail DLL load
//		break;
//
//	case DLL_PROCESS_DETACH:
//		// detach from process
//		break;
//
//	case DLL_THREAD_ATTACH:
//		// attach to thread
//		break;
//
//	case DLL_THREAD_DETACH:
//		// detach from thread
//		break;
//	}
//	return TRUE; // succesful
//}



namespace testDll
{

	//CameraParameters camPara;
	DLL_EXPORT double Add(double a, double b)
	{
		cv::Mat M;
		return a + b;
	}

	

}