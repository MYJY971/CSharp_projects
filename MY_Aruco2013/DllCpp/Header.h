#pragma once
#include <stdexcept>
#include <Windows.h>
#include "opencv\cv.h"

using namespace std;


#define DLL_EXPORT extern "C" __declspec(dllexport)

namespace testDll
{
	//extern "C" { __declspec(dllexport) double Add(double a, double b); }
	DLL_EXPORT double Add(double a, double b);

}