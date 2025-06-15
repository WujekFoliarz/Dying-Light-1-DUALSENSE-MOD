#include "main.hpp"

#if COMPILE_TO_DLL == 0
int main(int argc, char* argv[]) {
	return 0;
}
#elif COMPILE_TO_DLL == 1 && defined(_WIN32)
BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved) {
	switch (ul_reason_for_call) {
		case DLL_PROCESS_ATTACH: {
			break;
		}

		case DLL_THREAD_ATTACH: {
			break;
		}

		case DLL_THREAD_DETACH: {
			break;
		}

		case DLL_PROCESS_DETACH: {
			break;
		}
	}
	return TRUE;
}
#endif