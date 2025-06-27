#ifndef SCEPADHAPTICS_H
#define SCEPADHAPTICS_H

#if defined(_WIN32) || defined(_WIN64)
#ifdef SCEPADHAPTICS_EXPORTS
#define SCEPADHAPTICS_API __declspec(dllexport)
#else
#define SCEPADHAPTICS_API __declspec(dllimport)
#endif
#else
#ifdef SCEPADHAPTICS_EXPORTS
#define SCEPADHAPTICS_API __attribute__((visibility("default")))
#else
#define SCEPADHAPTICS_API
#endif
#endif

#ifdef __cplusplus
extern "C" {
#endif

	SCEPADHAPTICS_API	int scePadAudioInit();
	SCEPADHAPTICS_API	int scePadAudioOpen(int handle);
	SCEPADHAPTICS_API	int scePadAudioPlayWave(int handle, const char* path, bool dontPlayIfAlreadyPlaying, bool loop);
	SCEPADHAPTICS_API	int scePadAudioStopWave(int handle, const char* path);

#ifdef __cplusplus
}
#endif

#endif // SCEPADHAPTICS_H