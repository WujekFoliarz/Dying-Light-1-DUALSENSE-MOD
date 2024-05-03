#pragma once
#include <cstdio>
#include <iostream>
#include <windows.h>
#include "MinHook/MinHook.h"

bool __cdecl Add_Source_Hook(char const* Path, int FFSAddSourceFlags);

void __cdecl InitializeGameScript_Hook(LPCSTR param_1);

void __cdecl CLogV_Hook(int logtype, char* thread, char* sourcefile, int linenumber, int CLFilterAction, int CLLineMode, char const* __ptr64 message, char const* __ptr64 printarg);
//annoying ahh

int __cdecl GetCategoryLevel_Hook(int This, char* Catagory);
int* __cdecl LogSettingsInstance_Hook();

extern FARPROC GetCategoryLevel_Address;
extern FARPROC InitializeGameScript_Address;
extern FARPROC Add_Source_Address;
extern FARPROC CLogV_Address;
extern FARPROC LogSettingsInstance_Address;

BOOL WINAPI CreateHooks(HMODULE hmodule);