#pragma once
#include <stdexcept>

using namespace std;

#define DLL_EXPORT extern "C" __declspec(dllexport)
namespace OpenDll
{
	//extern "C" { __declspec(dllexport) double Add(double a, double b); }
	DLL_EXPORT double Add(double a, double b);

}