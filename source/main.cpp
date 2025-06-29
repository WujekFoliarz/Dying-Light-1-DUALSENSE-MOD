#include <duaLib.h>
#include <libScePadHaptics.hpp>
#include <minhook.h>
#include <cstdint>
#include <Windows.h>
#include <string>
#include <vector>
#include "functions.hpp"
#pragma comment(lib, "winmm.lib")

// https://github.com/OneshotGH/CSGOSimple-master/blob/master/CSGOSimple/helpers/utils.cpp
std::uint8_t* PatternScan(void* module, const char* signature) {
	static auto pattern_to_byte = [](const char* pattern) {
		auto bytes = std::vector<int>{};
		auto start = const_cast<char*>(pattern);
		auto end = const_cast<char*>(pattern) + strlen(pattern);

		for (auto current = start; current < end; ++current) {
			if (*current == '?') {
				++current;
				if (*current == '?')
					++current;
				bytes.push_back(-1);
			}
			else {
				bytes.push_back(strtoul(current, &current, 16));
			}
		}
		return bytes;
		};

	auto dosHeader = (PIMAGE_DOS_HEADER)module;
	auto ntHeaders = (PIMAGE_NT_HEADERS)((std::uint8_t*)module + dosHeader->e_lfanew);

	auto sizeOfImage = ntHeaders->OptionalHeader.SizeOfImage;
	auto patternBytes = pattern_to_byte(signature);
	auto scanBytes = reinterpret_cast<std::uint8_t*>(module);

	auto s = patternBytes.size();
	auto d = patternBytes.data();

	for (auto i = 0ul; i < sizeOfImage - s; ++i) {
		bool found = true;
		for (auto j = 0ul; j < s; ++j) {
			if (scanBytes[i + j] != d[j] && d[j] != -1) {
				found = false;
				break;
			}
		}
		if (found) {
			return &scanBytes[i];
		}
	}
	return nullptr;
}

struct Hook {
	std::string name;
	LPVOID target;
	LPVOID detour;
	LPVOID* original;
};

const std::string libScePadDllName = "libScePad_x64.dll";
const std::string gameDllName = "gamedll_x64_rwdi.dll";
const std::string engineDllName = "engine_x64_rwdi.dll";
std::vector<Hook> g_hooks;
std::atomic<bool> g_threadRunning;

void setupHooks() {
	HMODULE sceHModule = GetModuleHandleA(libScePadDllName.c_str());
	HMODULE gameHModule = GetModuleHandleA(gameDllName.c_str());
	HMODULE engineHModule = GetModuleHandleA(engineDllName.c_str());

#pragma region libScePad
	g_hooks.push_back({ "scePadInit",
					  reinterpret_cast<LPVOID>(GetProcAddress(sceHModule, "scePadInit")),
					  reinterpret_cast<LPVOID>(&libScePad_x64::scePadInit_Hook),
					  reinterpret_cast<LPVOID*>(&libScePad_x64::scePadInit_Org) }
	);

	g_hooks.push_back({ "scePadOpen",
					  reinterpret_cast<LPVOID>(GetProcAddress(sceHModule, "scePadOpen")),
					  reinterpret_cast<LPVOID>(&libScePad_x64::scePadOpen_Hook),
					  reinterpret_cast<LPVOID*>(&libScePad_x64::scePadOpen_Org) }
	);

	g_hooks.push_back({ "scePadReadState",
					  reinterpret_cast<LPVOID>(GetProcAddress(sceHModule, "scePadReadState")),
					  reinterpret_cast<LPVOID>(&libScePad_x64::scePadReadState_Hook),
					  reinterpret_cast<LPVOID*>(&libScePad_x64::scePadReadState_Org) }
	);

	g_hooks.push_back({ "scePadSetLightBar",
					  reinterpret_cast<LPVOID>(GetProcAddress(sceHModule, "scePadSetLightBar")),
					  reinterpret_cast<LPVOID>(&libScePad_x64::scePadSetLightBar_Hook),
					  reinterpret_cast<LPVOID*>(&libScePad_x64::scePadSetLightBar_Org) }
	);

	g_hooks.push_back({ "scePadSetVibration",
					  reinterpret_cast<LPVOID>(GetProcAddress(sceHModule, "scePadSetVibration")),
					  reinterpret_cast<LPVOID>(&libScePad_x64::scePadSetVibration_Hook),
					  reinterpret_cast<LPVOID*>(&libScePad_x64::scePadSetVibration_Org) }
	);
#pragma endregion

#pragma region gamedll_x64_rwdi.dll
	g_hooks.push_back({ "animationFunction",
				  reinterpret_cast<LPVOID>(PatternScan(reinterpret_cast<void*>(gameHModule), "48 89 5C 24 10 57 48 83 EC 50 48 8B 01 0F")),
				  reinterpret_cast<LPVOID>(&gamedll_x64_rwdi::animationFunction_Hook),
				  reinterpret_cast<LPVOID*>(&gamedll_x64_rwdi::animationFunction_Org) }
	);

	g_hooks.push_back({ "hudSoundFunction",
			  reinterpret_cast<LPVOID>(PatternScan(reinterpret_cast<void*>(gameHModule), "48 63 C1 48 8D 15 36 59")),
			  reinterpret_cast<LPVOID>(&gamedll_x64_rwdi::hudSound_Hook),
			  reinterpret_cast<LPVOID*>(&gamedll_x64_rwdi::hudSound_Org) }
	);

	g_hooks.push_back({ "pausedFunction",
			  reinterpret_cast<LPVOID>(PatternScan(reinterpret_cast<void*>(gameHModule), "48 89 5C 24 08 57 48 83 EC 20 81")),
			  reinterpret_cast<LPVOID>(&gamedll_x64_rwdi::paused_Hook),
			  reinterpret_cast<LPVOID*>(&gamedll_x64_rwdi::paused_Org) }
	);

	g_hooks.push_back({ "unpausedFunction",
			  reinterpret_cast<LPVOID>(PatternScan(reinterpret_cast<void*>(gameHModule), "48 89 5C 24 08 57 48 83 EC 20 33 DB 48 8B F9 89 59 30")),
			  reinterpret_cast<LPVOID>(&gamedll_x64_rwdi::unpaused_Hook),
			  reinterpret_cast<LPVOID*>(&gamedll_x64_rwdi::unpaused_Org) }
	);

	g_hooks.push_back({ "flashlightOnFunction",
			  reinterpret_cast<LPVOID>(PatternScan(reinterpret_cast<void*>(gameHModule), "40 53 48 83 EC 20 80 79 24 00 48 8B D9 74 1C")),
			  reinterpret_cast<LPVOID>(&gamedll_x64_rwdi::flashlightOn_Hook),
			  reinterpret_cast<LPVOID*>(&gamedll_x64_rwdi::flashlightOn_Org) }
	);

	g_hooks.push_back({ "flashlightOffFunction",
			  reinterpret_cast<LPVOID>(PatternScan(reinterpret_cast<void*>(gameHModule), "40 53 48 83 EC 20 80 79 24 00 48 8B D9 74 26")),
			  reinterpret_cast<LPVOID>(&gamedll_x64_rwdi::flashlightOff_Hook),
			  reinterpret_cast<LPVOID*>(&gamedll_x64_rwdi::flashlightOff_Org) }
	);

	g_hooks.push_back({ "uvLightHook",
			  reinterpret_cast<LPVOID>(PatternScan(reinterpret_cast<void*>(gameHModule), "40 53 48 83 EC 50 48 83 B9 50")),
			  reinterpret_cast<LPVOID>(&gamedll_x64_rwdi::uvLight_Hook),
			  reinterpret_cast<LPVOID*>(&gamedll_x64_rwdi::uvLight_Org) }
	);

#pragma endregion

}

DWORD WINAPI handleTriggerBasedEffects(LPVOID lPtr) {
	timeBeginPeriod(1);

	while (g_threadRunning) {
		AnimationType currentForcedAnimation = AnimationType::None;
		AnimationType currentAnimation = AnimationType::None;
		s_ScePadData data = {};

		{
			std::lock_guard<std::mutex> guard(g_lock);
			currentAnimation = g_currentAnimation;
			currentForcedAnimation = g_currentForcedAnimation;
			scePadReadState(g_handle, &data);
		}

		auto hapticIt = g_hapticSettings.find(currentAnimation);

		if (hapticIt != g_hapticSettings.end() && !g_paused) {

			HapticEffect currentEffect = hapticIt->second;

			if (currentEffect.delayBeforeNextPlay > std::chrono::milliseconds(0) && g_timer.Time() < currentEffect.delayBeforeNextPlay) {
				continue;
			}

			bool R2Pushed = (data.R2_Analog >= currentEffect.triggerThreshold) && (g_lastData.R2_Analog < currentEffect.triggerThreshold);
			bool R2Released = (data.R2_Analog < currentEffect.triggerThreshold) && (g_lastData.R2_Analog >= currentEffect.triggerThreshold);
			bool R2Held = (data.R2_Analog >= currentEffect.triggerThreshold) && (g_lastData.R2_Analog >= currentEffect.triggerThreshold);
			std::vector<std::string> filesToPlay;

			if (R2Pushed)
				filesToPlay.push_back(getRandomString(currentEffect.effectsOnR2Pushed));

			if (R2Released)
				filesToPlay.push_back(getRandomString(currentEffect.effectsOnR2Released));

			if (R2Held)
				filesToPlay.push_back(getRandomString(currentEffect.effectsOnR2Held));

			for (auto& file : filesToPlay) {
				if (!file.empty()) {
					std::string fullPath = HAPTIC_PATH + file;
				
					{
						std::lock_guard<std::mutex> guard(g_lock);
						g_lastScePadAudioPlayResult = scePadAudioPlayWave(g_handle, fullPath.c_str(), currentEffect.loop, currentEffect.loop);
						g_timer.Restart();
					}
				}
			}
		}
		
		{
			std::lock_guard<std::mutex> guard(g_lock);
			g_lastData = data;
		}

		// Stop effects that shouldn't be playing          
		if (g_currentAnimation != AnimationType::Chainsaw || (data.R2_Analog < 250)) {
			scePadAudioStopWave(g_handle, HAPTIC_PATH "chainsaw.wav");
		}

		std::this_thread::sleep_for(std::chrono::milliseconds(1));
	}

	return 0;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved) {
	switch (ul_reason_for_call) {
		case DLL_PROCESS_ATTACH:
		{
			//AllocConsole();
			//FILE* fp;
			//freopen_s(&fp, "CONOUT$", "w", stdout);
			//freopen_s(&fp, "CONOUT$", "w", stderr);
			//freopen_s(&fp, "CONIN$", "r", stdin);

			DisableThreadLibraryCalls(hModule);
			setupHooks();

			if (MH_Initialize() != MH_STATUS::MH_OK) {
				MessageBox(NULL, "Failed to initialize MinHook", "Error", MB_OK | MB_ICONERROR);
				return 1;
			}

			for (auto& hook : g_hooks) {
				// ignore errors from hudSoundFunction cuz it only seems to be present on steam
				if (MH_CreateHook(hook.target, hook.detour, hook.original) != MH_STATUS::MH_OK && hook.name != "hudSoundFunction") {
					std::string failMessage = "Failed to create hook for " + hook.name;
					MessageBox(NULL, failMessage.c_str(), "Error", MB_OK | MB_ICONERROR);
					return 1;
				}

				if (MH_EnableHook(hook.target) != MH_STATUS::MH_OK && hook.name != "hudSoundFunction") {
					std::string failMessage = "Failed to enable hook for " + hook.name;
					MessageBox(NULL, failMessage.c_str(), "Error", MB_OK | MB_ICONERROR);
					return 1;
				}
			}

			g_threadRunning = true;
			HANDLE hThread = CreateThread(NULL, 0, handleTriggerBasedEffects, NULL, 0, NULL);

			if (hThread == NULL) {
				MessageBox(NULL, "Failed to start handleTriggerBasedEffects thread", "Error", MB_OK | MB_ICONERROR);
				return 1;
			}

			break;
		}

		case DLL_PROCESS_DETACH:
		{
			g_threadRunning = false;
			MH_Uninitialize();
			scePadTerminate();
			break;
		}
	}
	return TRUE;
}