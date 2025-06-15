#include "main.hpp"

#if COMPILE_TO_DLL == 0
int main(int argc, char* argv[]) {
	return 0;
}
#elif COMPILE_TO_DLL == 1 && defined(_WIN32)
BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved) {
	switch (fdwReason) {
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
			if (lpvReserved != nullptr) {
				break;
			}
			break;
		}
	}
	return TRUE;
}
#endif