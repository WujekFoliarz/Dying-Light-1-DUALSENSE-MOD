#ifndef FUNCTIONS_H
#define FUNCTIONS_H

#include <cstdint>
#include <iostream>
#include "game.hpp"
#include "settings.hpp"
#include <chrono>
#include <libScePadHaptics.hpp>
#include <mutex>

#define HAPTIC_PATH "haptics\\"

class Timer {
public:
	Timer() : start(std::chrono::high_resolution_clock::now()) {}
	auto Time() const {
		auto now = std::chrono::high_resolution_clock::now();
		return std::chrono::duration_cast<std::chrono::milliseconds>(now - start);
	}
	void Restart() { start = std::chrono::high_resolution_clock::now(); }

private:
	std::chrono::time_point<std::chrono::high_resolution_clock> start;
};

std::mutex g_lock;
std::atomic<bool> g_useDefaultLightbar = true;
std::atomic<bool> g_useDefaultRumble = false;
std::atomic<bool> g_uvOn = false;
uint32_t g_handle = 0;
uint32_t g_lastScePadAudioPlayResult = 0;
AnimationType g_currentAnimation = AnimationType::None;
// Defaults to none if the animation is outside of is specified in GetAnimationType()
AnimationType g_currentForcedAnimation = AnimationType::None;
Timer g_timer;
s_ScePadData g_lastData;
uint8_t g_triggerThreshold = 250;
std::atomic<bool> g_paused = true;

namespace libScePad_x64 {
	typedef __int64 (*scePadInit_t)();
	scePadInit_t scePadInit_Org = nullptr;
	__int64 scePadInit_Hook() {
		s_ScePadInitParam param = {};
		param.allowBT = true;
		uint32_t result = scePadInit3(&param);
		scePadAudioInit();
		return result;
	}

	typedef __int64(__fastcall* scePadOpen_t)(int userID, int a2, int a3, void* a4);
	scePadOpen_t scePadOpen_Org = nullptr;
	__int64 __fastcall scePadOpen_Hook(int userID, int a2, int a3, void* a4) {
		int res = scePadOpen(userID, 0, 0);
		scePadAudioOpen(res);
		if (userID == 1) {
			std::lock_guard<std::mutex> guard(g_lock);
			g_handle = res;
		}		
		return res;
	}

	typedef __int64(__fastcall* scePadReadState_t)(int handle, s_ScePadData* state);
	scePadReadState_t scePadReadState_Org = nullptr;
	__int64 __fastcall scePadReadState_Hook(int handle, s_ScePadData* state) {
		scePadSetAudioOutPath(g_handle, SCE_PAD_AUDIO_PATH_ONLY_SPEAKER);
		s_ScePadVolumeGain volume = { 100,0,0,100 };
		scePadSetVolumeGain(g_handle, &volume);

		uint32_t result = scePadReadState(handle, state);

		if (result == SCE_OK) {
			state->L2_Analog = state->L2_Analog >= g_triggerThreshold ? state->L2_Analog : 0;
			state->R2_Analog = state->R2_Analog >= g_triggerThreshold ? state->R2_Analog : 0;
		}

		return result;
	}

	typedef __int64(__fastcall* scePadSetLightBar_t)(int handle, s_SceLightBar* led);
	scePadSetLightBar_t scePadSetLightBar_Org = nullptr;
	__int64 __fastcall scePadSetLightBar_Hook(int handle, s_SceLightBar* led) {
		if (g_useDefaultLightbar && !g_paused && !g_uvOn)
			return scePadSetLightBar(handle, led);

		return 0;
	}

	typedef __int64(__fastcall* scePadSetVibration_t)(int handle, s_ScePadVibrationParam* vibration);
	scePadSetVibration_t scePadSetVibration_Org = nullptr;
	__int64 __fastcall scePadSetVibration_Hook(int handle, s_ScePadVibrationParam* vibration) {
		return scePadSetVibration(handle, vibration);
	}
}

namespace gamedll_x64_rwdi {
	typedef __int64(__fastcall* animationFunction_t)(void* a1);
	animationFunction_t animationFunction_Org = nullptr;
	__int64 __fastcall animationFunction_Hook(void* a1) {
		if (g_paused) {
			ScePadTriggerEffectParam triggerEffect = {};
			triggerEffect.command[0].mode = SCE_PAD_TRIGGER_EFFECT_MODE_OFF;
			triggerEffect.command[1].mode = SCE_PAD_TRIGGER_EFFECT_MODE_OFF;
			triggerEffect.command[0].commandData.offParam = { 0 };
			triggerEffect.command[1].commandData.offParam = { 0 };
			triggerEffect.triggerMask = SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_L2 | SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2;
			scePadSetTriggerEffect(g_handle, &triggerEffect);

			return animationFunction_Org(a1);
		}

		if (a1) {

			{
				std::lock_guard<std::mutex> guard(g_lock);
				s_ScePadData data = {};
				scePadReadState(g_handle, &data);
			}

			// Get animation ID  
			uint32_t anim = *reinterpret_cast<uint32_t*>(reinterpret_cast<char*>(a1) + 36);
			AnimationType animType = GetAnimationType(anim);

			// Set current trigger effect  
			{
				std::lock_guard<std::mutex> guard(g_lock);
				g_currentAnimation = animType == AnimationType::None ? g_currentAnimation : animType;
				g_currentForcedAnimation = animType;
			}
			auto triggerIt = g_triggerSettings.find(g_currentAnimation);
			if (triggerIt != g_triggerSettings.end())
				scePadSetTriggerEffect(g_handle, &triggerIt->second);

			// Play haptics  
			auto hapticIt = g_hapticSettings.find(g_currentAnimation);
			if (hapticIt != g_hapticSettings.end()) {
				HapticEffect currentEffect = hapticIt->second;

				std::string animationHapticEffect = getRandomString(currentEffect.effectsOnAnimation);
				if (!animationHapticEffect.empty()) {
					std::lock_guard<std::mutex> guard(g_lock);
					std::string fullPath = HAPTIC_PATH + animationHapticEffect;
					g_lastScePadAudioPlayResult = scePadAudioPlayWave(g_handle, fullPath.c_str(), true, currentEffect.loop);
				}											
			}

			// Stop effects that shouldn't be playing          
			if (g_currentAnimation != AnimationType::Zipline) {
				scePadAudioStopWave(g_handle, HAPTIC_PATH "zip_line_loop_0.wav");
			}
			if (g_currentAnimation != AnimationType::GrabbedByBiterRight && g_currentAnimation != AnimationType::GrabbedByBiterLeft && g_currentAnimation != AnimationType::GrabbedByBiterFront && g_currentAnimation != AnimationType::BrokeOutBiterLeft) {
				scePadAudioStopWave(g_handle, HAPTIC_PATH "zombie_walker_grab_bite_long_right_0.wav");
				scePadAudioStopWave(g_handle, HAPTIC_PATH "zombie_walker_grab_bite_long_left_0.wav");
			}
		}

		scePadSetVibrationMode(g_handle, g_lastScePadAudioPlayResult == SCE_PAD_ERROR_DEVICE_NOT_CONNECTED ? SCE_PAD_RUMBLE_MODE : SCE_PAD_HAPTICS_MODE);
		return animationFunction_Org(a1);
	}

	typedef const char* (__fastcall* hudSound_t)(int a1);
	hudSound_t hudSound_Org = nullptr;
	const char* __fastcall hudSound_Hook(int a1) {
		const char* sound = hudSound_Org(a1);

		if (strcmp(sound, "hud_entering_safe_zone.wav") == 0)
			scePadAudioPlayWave(g_handle, HAPTIC_PATH "hud_entering_safe_zone_0.wav", true, false);
		else if (strcmp(sound, "hud_leaving_safe_zone.wav") == 0)
			scePadAudioPlayWave(g_handle, HAPTIC_PATH "hud_leaving_safe_zone_0.wav", true, false);

		return sound;
	}

	typedef __int64(__fastcall* paused_t)(void* a1);
	paused_t paused_Org = nullptr;
	__int64 __fastcall paused_Hook(void* a1) {
		if (a1) {
			g_paused = true;
			s_SceLightBar led = {255,102,0};
			scePadSetLightBar(g_handle, &led);
			ScePadTriggerEffectParam triggerEffect = {};
			triggerEffect.command[0].mode = SCE_PAD_TRIGGER_EFFECT_MODE_OFF;
			triggerEffect.command[1].mode = SCE_PAD_TRIGGER_EFFECT_MODE_OFF;
			triggerEffect.command[0].commandData.offParam = { 0 };
			triggerEffect.command[1].commandData.offParam = { 0 };
			triggerEffect.triggerMask = SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_L2 | SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2;
			scePadSetTriggerEffect(g_handle, &triggerEffect);
		}
		return paused_Org(a1);
	}

	typedef __int64(__fastcall* unpaused_t)(void* a1);
	unpaused_t unpaused_Org = nullptr;
	__int64 __fastcall unpaused_Hook(void* a1) {
		if (a1) {
			g_paused = false;
		}
		return unpaused_Org(a1);
	}

	typedef void(__fastcall* flashlightOn_t)(void* a1);
	flashlightOn_t flashlightOn_Org = nullptr;
	void __fastcall flashlightOn_Hook(void* a1) {
		if (a1) {
			scePadAudioPlayWave(g_handle, HAPTIC_PATH "torch_on_0.wav", false, false);
		}
		flashlightOn_Org(a1);
	}

	typedef void(__fastcall* flashlightOff_t)(void* a1);
	flashlightOff_t flashlightOff_Org = nullptr;
	void __fastcall flashlightOff_Hook(void* a1) {
		if (a1) {
			scePadAudioPlayWave(g_handle, HAPTIC_PATH "torch_off_0.wav", false, false);
		}
		flashlightOff_Org(a1);
	}

	typedef void(__fastcall* uvLight_t)(void* a1, float a2);
	uvLight_t uvLight_Org = nullptr;
	void __fastcall uvLight_Hook(void* a1, float a2) {
		if(a1)
		{
			uint8_t unk = *reinterpret_cast<uint8_t*>(reinterpret_cast<char*>(a1) + 84);
			uvLight_Org(a1, a2);
			uint8_t unkNew = *reinterpret_cast<uint8_t*>(reinterpret_cast<char*>(a1) + 84);

			if (unk == 0 && unkNew == 1) {
				g_uvOn = true;
				scePadAudioPlayWave(g_handle, HAPTIC_PATH "uv_flashlight_working_loop_0.wav", true, true);			
				s_SceLightBar led = { 60, 0, 255 };
				scePadSetLightBar(g_handle, &led);
			}
			else if ((unk == 1 && unkNew == 0) || (unk == 0 && unkNew == 0)) {
				g_uvOn = false;
				scePadAudioStopWave(g_handle, HAPTIC_PATH "uv_flashlight_working_loop_0.wav");
			}
		}
	}
}
#endif // FUNCTIONS_H