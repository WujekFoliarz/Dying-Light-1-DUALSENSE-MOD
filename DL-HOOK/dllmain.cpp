// dllmain.cpp : Defines the entry point for the DLL application.
#include "Hooks.h"
#include <Windows.h>
#include <tlhelp32.h>

#pragma region Proxy
struct dsound_dll {
	HMODULE dll;
	FARPROC oDirectSoundCaptureCreate;
	FARPROC oDirectSoundCaptureCreate8;
	FARPROC oDirectSoundCaptureEnumerateA;
	FARPROC oDirectSoundCaptureEnumerateW;
	FARPROC oDirectSoundCreate;
	FARPROC oDirectSoundCreate8;
	FARPROC oDirectSoundEnumerateA;
	FARPROC oDirectSoundEnumerateW;
	FARPROC oDirectSoundFullDuplexCreate;
	FARPROC oDllCanUnloadNow;
	FARPROC oDllGetClassObject;
	FARPROC oGetDeviceID;
} dsound;

extern "C" {
	FARPROC PA = 0;
	int runASM();

	void fDirectSoundCaptureCreate() { PA = dsound.oDirectSoundCaptureCreate; runASM(); }
	void fDirectSoundCaptureCreate8() { PA = dsound.oDirectSoundCaptureCreate8; runASM(); }
	void fDirectSoundCaptureEnumerateA() { PA = dsound.oDirectSoundCaptureEnumerateA; runASM(); }
	void fDirectSoundCaptureEnumerateW() { PA = dsound.oDirectSoundCaptureEnumerateW; runASM(); }
	void fDirectSoundCreate() { PA = dsound.oDirectSoundCreate; runASM(); }
	void fDirectSoundCreate8() { PA = dsound.oDirectSoundCreate8; runASM(); }
	void fDirectSoundEnumerateA() { PA = dsound.oDirectSoundEnumerateA; runASM(); }
	void fDirectSoundEnumerateW() { PA = dsound.oDirectSoundEnumerateW; runASM(); }
	void fDirectSoundFullDuplexCreate() { PA = dsound.oDirectSoundFullDuplexCreate; runASM(); }
	void fDllCanUnloadNow() { PA = dsound.oDllCanUnloadNow; runASM(); }
	void fDllGetClassObject() { PA = dsound.oDllGetClassObject; runASM(); }
	void fGetDeviceID() { PA = dsound.oGetDeviceID; runASM(); }
}

void setup_dsound() {
	dsound.oDirectSoundCaptureCreate = GetProcAddress(dsound.dll, "DirectSoundCaptureCreate");
	dsound.oDirectSoundCaptureCreate8 = GetProcAddress(dsound.dll, "DirectSoundCaptureCreate8");
	dsound.oDirectSoundCaptureEnumerateA = GetProcAddress(dsound.dll, "DirectSoundCaptureEnumerateA");
	dsound.oDirectSoundCaptureEnumerateW = GetProcAddress(dsound.dll, "DirectSoundCaptureEnumerateW");
	dsound.oDirectSoundCreate = GetProcAddress(dsound.dll, "DirectSoundCreate");
	dsound.oDirectSoundCreate8 = GetProcAddress(dsound.dll, "DirectSoundCreate8");
	dsound.oDirectSoundEnumerateA = GetProcAddress(dsound.dll, "DirectSoundEnumerateA");
	dsound.oDirectSoundEnumerateW = GetProcAddress(dsound.dll, "DirectSoundEnumerateW");
	dsound.oDirectSoundFullDuplexCreate = GetProcAddress(dsound.dll, "DirectSoundFullDuplexCreate");
	dsound.oDllCanUnloadNow = GetProcAddress(dsound.dll, "DllCanUnloadNow");
	dsound.oDllGetClassObject = GetProcAddress(dsound.dll, "DllGetClassObject");
	dsound.oGetDeviceID = GetProcAddress(dsound.dll, "GetDeviceID");
}
#pragma endregion


BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved) {

	char path[MAX_PATH];
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	{
		DisableThreadLibraryCalls(hModule);
		MH_Initialize();

		CopyMemory(path + GetSystemDirectory(path, MAX_PATH - 12), "\\dsound.dll", 12);
		dsound.dll = LoadLibrary(path);

		if (dsound.dll == NULL)
		{
			MessageBox(0, "Original dsound.dll missing", "Exiting", MB_ICONERROR);
			ExitProcess(0);
		}
		setup_dsound();

		HANDLE h = CreateThread(0, 0, (LPTHREAD_START_ROUTINE)CreateHooks, hModule, 0, 0);
		if (h != 0)
		{
			SetThreadPriority(h, 2);
			CloseHandle(h);
		}

		break;
	}
	case DLL_PROCESS_DETACH:
	{
		FreeLibrary(dsound.dll);
		MH_Uninitialize();
	}
	break;
	}
	return TRUE;
}
