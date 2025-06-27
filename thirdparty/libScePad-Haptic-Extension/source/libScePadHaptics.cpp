#include "libScePadHaptics.hpp"
#include <cstdint>
#define MINIAUDIO_IMPLEMENTATION
#include "miniaudio.h"
#include "libScePad.h"

#include <iostream>
#include <thread>
#include <chrono>
#include <Windows.h>
#include <mmdeviceapi.h>
#include <propkey.h>
#include <functiondiscoverykeys_devpkey.h>
#include <atomic>
#include <mutex>
#include <unordered_map>

#if (defined _WIN32) || (defined _WIN64)
#include <iostream>
#include <string>
#include <guiddef.h>
#include <stdexcept>
#include <codecvt>

static GUID StringToGuid(const std::string& guidStr) {
	GUID guid = {};

	unsigned int d1, d2, d3;
	unsigned int d4[8];

	int matched = std::sscanf(
		guidStr.c_str(),
		"{%08x-%04x-%04x-%02x%02x-%02x%02x%02x%02x%02x%02x}",
		&d1, &d2, &d3,
		&d4[0], &d4[1], &d4[2], &d4[3], &d4[4], &d4[5], &d4[6], &d4[7]);

	guid.Data1 = d1;
	guid.Data2 = static_cast<WORD>(d2);
	guid.Data3 = static_cast<WORD>(d3);
	for (int i = 0; i < 8; ++i) {
		guid.Data4[i] = static_cast<BYTE>(d4[i]);
	}

	return guid;
}


static ma_result FindDeviceByContainerIdWindows(ma_context* pContext, const GUID& targetContainerId, ma_device_id* pDeviceId) {
	ma_device_info* pPlaybackInfos;
	ma_uint32 playbackCount;
	ma_result result = ma_context_get_devices(pContext, &pPlaybackInfos, &playbackCount, nullptr, nullptr);
	if (result != MA_SUCCESS) {
		printf("Failed to enumerate devices.\n");
		return result;
	}

	HRESULT hr;
	IMMDeviceEnumerator* pEnumerator = nullptr;
	hr = CoCreateInstance(__uuidof(MMDeviceEnumerator), nullptr, CLSCTX_ALL, __uuidof(IMMDeviceEnumerator), (void**)&pEnumerator);
	if (FAILED(hr)) {
		printf("Failed to create device enumerator.\n");
		return MA_ERROR;
	}

	for (ma_uint32 i = 0; i < playbackCount; i++) {
		std::wstring wasapiId(pPlaybackInfos[i].id.wasapi);
		IMMDevice* pDevice = nullptr;
		hr = pEnumerator->GetDevice(wasapiId.c_str(), &pDevice);
		if (SUCCEEDED(hr)) {
			IPropertyStore* pProps = nullptr;
			hr = pDevice->OpenPropertyStore(STGM_READ, &pProps);
			if (SUCCEEDED(hr)) {
				PROPVARIANT pv;
				PropVariantInit(&pv);
				hr = pProps->GetValue(PKEY_Device_ContainerId, &pv);
				if (SUCCEEDED(hr) && pv.vt == VT_CLSID) {
					GUID containerId = *pv.puuid;
					if (containerId == targetContainerId) {
						*pDeviceId = pPlaybackInfos[i].id;
						PropVariantClear(&pv);
						pProps->Release();
						pDevice->Release();
						pEnumerator->Release();
						return MA_SUCCESS;
					}
				}
				PropVariantClear(&pv);
				pProps->Release();
			}
			pDevice->Release();
		}
	}

	pEnumerator->Release();
	return ma_result::MA_NO_DEVICE;
}
#endif

bool isMaDeviceWorking(ma_device* device) {
	if (device == nullptr) return false;

	ma_device_state state = ma_device_get_state(device);
	if (state == ma_device_state_uninitialized) return false;
	if (state == ma_device_state_stopped) return false;
	if (state != ma_device_state_started) return false;
	if (!ma_device_is_started(device)) return false;

	return true;
}

struct ControllerAudio {
	ma_device_id deviceId;
	ma_device device;
	ma_engine engine;
	ma_channel_converter converter;
	std::mutex lock;
	uint32_t handle = 0;
	bool connected = false;
	bool wasAudioWorking = false;
	std::unordered_multimap<std::string, std::unique_ptr<ma_sound>> sounds;
};

void dataCallback(ma_device* pDevice, void* pOutput, const void* pInput, ma_uint32 frameCount) {
	ma_channel_converter* converter = (ma_channel_converter*)pDevice->pUserData;
	if (!converter) return;

	ma_channel_converter_process_pcm_frames(converter, pOutput, pInput, frameCount);
}

ControllerAudio g_controllerAudio[4] = {};
ma_context g_context;
std::mutex g_contextLock;
std::atomic<bool> g_initialized = false;
std::atomic<bool> g_threadRunning = false;
std::thread g_watchThread;

void watchFunc() {
	while (g_threadRunning) {
		for (auto& controller : g_controllerAudio) {
			std::lock_guard<std::mutex> guard(controller.lock);

			if (controller.handle == 0) continue;

			int busType = -1;
			s_SceControllerType controllerType;
			uint32_t result = scePadGetControllerBusType(controller.handle, &busType);
			result = scePadGetControllerType(controller.handle, &controllerType);

			if (result == SCE_PAD_ERROR_DEVICE_NOT_CONNECTED) {
				if (controller.connected) { 
					controller.connected = false;

					for (auto& soundPair : controller.sounds) {
						ma_sound_stop(soundPair.second.get());
						ma_sound_uninit(soundPair.second.get());
					}
					controller.sounds.clear();

					if (controller.wasAudioWorking) {
						ma_engine_uninit(&controller.engine);
						ma_device_uninit(&controller.device);
						controller.wasAudioWorking = false;
					}
				}
			}
			
			if (result != SCE_OK || busType == 2) continue;

			controller.connected = true;

			if (!isMaDeviceWorking(&controller.device)) {				
				s_ScePadContainerIdInfo info = {};
				result = scePadGetContainerIdInformation(controller.handle, &info);
				if (result == SCE_OK) {

					GUID targetContainerId = StringToGuid(info.id);

					if (strlen(info.id) == 0) continue;

					{
						std::lock_guard<std::mutex> guard(g_contextLock);
						result = FindDeviceByContainerIdWindows(&g_context, targetContainerId, &controller.deviceId);
						if (result != MA_SUCCESS) continue;
					}
					
					ma_channel inputChannels[2] = {
						MA_CHANNEL_FRONT_LEFT,
						MA_CHANNEL_FRONT_RIGHT
					};

					ma_channel outputChannelsDualsense[4] = {
						MA_CHANNEL_FRONT_LEFT,
						MA_CHANNEL_FRONT_RIGHT,
						MA_CHANNEL_FRONT_CENTER,
						MA_CHANNEL_BACK_CENTER,
					};

					ma_channel outputChannelsDualshock[2] = {
						MA_CHANNEL_FRONT_LEFT,
						MA_CHANNEL_FRONT_RIGHT
					};
					
					ma_channel_converter_config channelConverterConfig = ma_channel_converter_config_init(
					ma_format::ma_format_f32,          
						2,                         
						inputChannels,                           
						controllerType == DUALSENSE ? 4 : 2,                             
						controllerType == DUALSENSE ? outputChannelsDualsense : outputChannelsDualshock,   
						ma_channel_mix_mode_simple); 

					result = ma_channel_converter_init(&channelConverterConfig, NULL, &controller.converter);
					
					ma_device_config deviceConfig = ma_device_config_init(ma_device_type_playback);
					deviceConfig.playback.pDeviceID = &controller.deviceId;
					deviceConfig.playback.format = ma_format_f32;
					deviceConfig.playback.channels = 0;
					deviceConfig.sampleRate = 48000;
					deviceConfig.playback.channelMixMode = ma_channel_mix_mode_default;
					deviceConfig.dataCallback = dataCallback;
					deviceConfig.pUserData = &controller.converter;
					deviceConfig.playback.shareMode = ma_share_mode_shared;

					{
						std::lock_guard<std::mutex> guard(g_contextLock);
						if (ma_device_init(&g_context, &deviceConfig, &controller.device) != MA_SUCCESS) continue;
					}

					ma_device_start(&controller.device);

					ma_engine_config config = ma_engine_config_init();
					config.pPlaybackDeviceID = &controller.deviceId;

					if (ma_engine_init(&config, &controller.engine) != MA_SUCCESS) continue;

					controller.wasAudioWorking = true;
				}
			}
		}
	
		std::this_thread::sleep_for(std::chrono::seconds(1));
	}
}

int scePadAudioInit() {
	if (!g_initialized) {
		if (ma_context_init(NULL, 0, NULL, &g_context) != MA_SUCCESS)
			return SCE_PAD_ERROR_FATAL;

		g_initialized = true;
		g_threadRunning = true;
		g_watchThread = std::thread(watchFunc);
		g_watchThread.detach();
	}

	return SCE_OK;
}

int scePadAudioOpen(int handle) {
	if (!g_initialized) return SCE_PAD_ERROR_NOT_INITIALIZED;

	uint32_t lastUnused = -1;
	uint32_t index = 0;

	for (auto& controller : g_controllerAudio) {
		std::lock_guard<std::mutex> guard(controller.lock);

		if (controller.handle == handle) {
			return SCE_PAD_ERROR_ALREADY_OPENED;
		}
		else if (controller.handle == 0) {
			lastUnused = index;
		}

		index++;
	}

	if (lastUnused != -1) {
		std::lock_guard<std::mutex> guard(g_controllerAudio[lastUnused].lock);
		g_controllerAudio[lastUnused].handle = handle;

		return SCE_OK;
	}

	return SCE_PAD_ERROR_NO_HANDLE;
}

int scePadAudioPlayWave(int handle, const char* path, bool dontPlayIfAlreadyPlaying, bool loop) {
	if (!g_initialized) return SCE_PAD_ERROR_NOT_INITIALIZED;

	for (auto& controller : g_controllerAudio) {
		std::lock_guard<std::mutex> guard(controller.lock);

		if (controller.handle != handle) continue;
		if (!controller.connected) return SCE_PAD_ERROR_DEVICE_NOT_CONNECTED;
		if (!isMaDeviceWorking(&controller.device)) return SCE_PAD_ERROR_DEVICE_NOT_CONNECTED;

		// Remove finished sounds
		for (auto it = controller.sounds.begin(); it != controller.sounds.end();) {
			if (loop && ma_sound_is_looping(it->second.get()) && std::string(path) == it->first) {
				return SCE_PAD_ERROR_NOT_PERMITTED;
			}
			else if (!ma_sound_is_playing(it->second.get())) {
				ma_sound_uninit(it->second.get());
				it = controller.sounds.erase(it);
			}
			else if (ma_sound_is_playing(it->second.get()) && std::string(path) == it->first && dontPlayIfAlreadyPlaying) {
				return SCE_OK;
			}
			else {
				++it;
			}
		}

		std::unique_ptr<ma_sound> sound = std::make_unique<ma_sound>();
		ma_result result = ma_sound_init_from_file(&controller.engine, path, 0, NULL, NULL, sound.get());

		if (loop)
			ma_sound_set_looping(sound.get(), true);

		result = ma_sound_start(sound.get());

		controller.sounds.insert({ path, std::move(sound) });

		return SCE_OK;
	}

	return SCE_PAD_ERROR_INVALID_HANDLE;
}

int scePadAudioStopWave(int handle, const char* path) {
	if (!g_initialized) return SCE_PAD_ERROR_NOT_INITIALIZED;
	for (auto& controller : g_controllerAudio) {
		std::lock_guard<std::mutex> guard(controller.lock);

		if (controller.handle != handle) continue;
		if (!controller.connected) return SCE_PAD_ERROR_DEVICE_NOT_CONNECTED;
		if (!isMaDeviceWorking(&controller.device)) return SCE_PAD_ERROR_DEVICE_NOT_CONNECTED;

		for (auto it = controller.sounds.begin(); it != controller.sounds.end();) {
			if ((ma_sound_is_playing(it->second.get()) || ma_sound_is_looping(it->second.get())) && std::string(path) == it->first) {
				ma_sound_stop(it->second.get());
				ma_sound_uninit(it->second.get());
				it = controller.sounds.erase(it);
			}
			else {
				++it;
			}
		}

		return SCE_OK;
	}

	return SCE_PAD_ERROR_INVALID_HANDLE;
}

#ifdef COMPILE_TO_EXE
int main() {
	return 0;
}
#endif
