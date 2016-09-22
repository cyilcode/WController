#define WCS_API extern "C" __declspec(dllexport)

WCS_API int test(int param)
{
	return param;
}