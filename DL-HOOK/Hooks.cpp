#include "Hooks.h"

FARPROC InitializeGameScript_Address;
FARPROC Add_Source_Address;
FARPROC CLogV_Address;
FARPROC GetCategoryLevel_Address;
FARPROC LogSettingsInstance_Address;

BOOL CreateHooks(HMODULE hmodule) {

	const char* exePath = "DL1-Dualsense.exe";
	STARTUPINFO si = { sizeof(STARTUPINFO) };
	PROCESS_INFORMATION pi;

	si.dwFlags = STARTF_USESHOWWINDOW;
	si.wShowWindow = SW_HIDE;

	if (CreateProcess(NULL, (LPSTR)exePath, NULL, NULL, FALSE, 0, NULL, NULL, &si, &pi)) {
		// shieeet
	}
	else {
		// Failed to create process
		MessageBox(NULL, "Failed to launch DL1-Dualsense.", "Error", MB_OK | MB_ICONERROR);
	}

	return true;
}