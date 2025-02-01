#pragma once

#pragma pack(push, 1) // Ensure no padding between members

#ifdef DUALSENSEAPI_EXPORTS
#define DUALSENSEAPI __declspec(dllexport)
#else
#define DUALSENSEAPI __declspec(dllimport)
#endif

struct Touchpad
{
	int RawTrackingNum;
	bool IsActive;
	int ID;
	int X;
	int Y;
};

struct Gyro
{
	int X;
	int Y;
	int Z;
};

struct Accelerometer
{
	int16_t X;
	int16_t Y;
	int16_t Z;
	uint32_t SensorTimestamp;
};

enum class BatteryState : uint8_t
{
	POWER_SUPPLY_STATUS_DISCHARGING = 0x0,
	POWER_SUPPLY_STATUS_CHARGING = 0x2,
	POWER_SUPPLY_STATUS_FULL = 0x1,
	POWER_SUPPLY_STATUS_NOT_CHARGING = 0xb,
	POWER_SUPPLY_STATUS_ERROR = 0xf,
	POWER_SUPPLY_TEMP_OR_VOLTAGE_OUT_OF_RANGE = 0xa,
	POWER_SUPPLY_STATUS_UNKNOWN = 0x0
};

struct Battery
{
	uint8_t State; // Changed to uint8_t for direct mapping to C# byte
	int Level;
};

struct ButtonState
{
	bool square, triangle, circle, cross;
	bool DpadUp, DpadDown, DpadLeft, DpadRight;
	bool L1, L3, R1, R3, R2Btn, L2Btn;
	bool share, options, ps, touchBtn, micBtn;
	uint8_t L2, R2;
	int RX, RY, LX, LY, TouchPacketNum;
	char PathIdentifier[256]; // Use fixed-size array to match C# string marshaling
	Touchpad trackPadTouch0;
	Touchpad trackPadTouch1;
	Gyro gyro;
	Accelerometer accelerometer;
	Battery battery;

	void SetDPadState(int dpad_state)
	{
		switch (dpad_state)
		{
		case 0:
			DpadUp = true;
			DpadDown = false;
			DpadLeft = false;
			DpadRight = false;
			break;

		case 1:
			DpadUp = true;
			DpadDown = false;
			DpadLeft = false;
			DpadRight = true;
			break;

		case 2:
			DpadUp = false;
			DpadDown = false;
			DpadLeft = false;
			DpadRight = true;
			break;

		case 3:
			DpadUp = false;
			DpadDown = true;
			DpadLeft = false;
			DpadRight = true;
			break;

		case 4:
			DpadUp = false;
			DpadDown = true;
			DpadLeft = false;
			DpadRight = false;
			break;

		case 5:
			DpadUp = false;
			DpadDown = true;
			DpadLeft = true;
			DpadRight = false;
			break;

		case 6:
			DpadUp = false;
			DpadDown = false;
			DpadLeft = true;
			DpadRight = false;
			break;

		case 7:
			DpadUp = true;
			DpadDown = false;
			DpadLeft = true;
			DpadRight = false;
			break;

		default:
			DpadUp = false;
			DpadDown = false;
			DpadLeft = false;
			DpadRight = false;
			break;
		}
	}
};
#pragma pack(pop)

namespace Feature
{
enum MuteLight : uint8_t
{
	Off = 0,
	On,
	Breathing,
	DoNothing, // literally nothing, this input is ignored,
			   // though it might be a faster blink in other versions
	NoAction4,
	NoAction5,
	NoAction6,
	NoAction7 = 7
};

enum class LightBrightness : uint8_t
{
	Bright = 0,
	Mid,
	Dim,
	BNoAction3,
	BNoAction4,
	BNoAction5,
	BNoAction6,
	BNoAction7 = 7
};

enum LightFadeAnimation : uint8_t
{
	Nothing = 0,
	FadeIn, // from black to blue
	FadeOut // from blue to black
};

enum RumbleMode : uint8_t
{
	StandardRumble = 0xFF,
	Haptic_Feedback = 0xFC
};

enum FeatureType : int8_t {
	Full = 0x57,
	LetGo = 0x08,
	BT_Init = 0x1 | 0x2 | 0x4 | 0x8 | 0x10 | 0x40,
};

enum AudioOutput : uint8_t
{
	Headset = 0x05,
	Speaker = 0x31
};

enum MicrophoneLED : uint8_t
{
	Pulse = 0x2,
	MLED_On = 0x1,
	MLED_Off = 0x0
};

enum MicrophoneStatus : uint8_t
{
	MSTATUS_On = 0,
	MSTATUS_Off = 0x10
};

enum Brightness : uint8_t
{
	High = 0x0,
	Medium = 0x1,
	Low = 0x2
};

enum PlayerLED : uint8_t
{
	PLAYER_OFF = 0x0,
	PLAYER_1 = 0x01,
	PLAYER_2 = 0x02,
	PLAYER_3 = 0x03,
	PLAYER_4 = 0x04,
	PLAYER_5 = 0x05,
};

enum DeviceType : uint8_t
{
	DualSense = 0,
	DualSense_Edge = 1,
	//DualShock4 = 2,
};

enum ConnectionType : uint8_t
{
	BT = 0x0,
	USB = 0x1
};
}; // namespace Feature

namespace Trigger
{
enum TriggerMode : uint8_t
{
	Off = 0x0,
	Rigid = 0x1,
	Pulse = 0x2,
	Rigid_A = 0x1 | 0x20,
	Rigid_B = 0x1 | 0x04,
	Rigid_AB = 0x1 | 0x20 | 0x04,
	Pulse_A = 0x2 | 0x20,
	Pulse_A2 = 35,
	Pulse_B = 0x2 | 0x04,
	Pulse_B2 = 38,
	Pulse_AB = 39,
	Calibration = 0xFC
};
}

enum HapticFeedbackStatus {
		Working = 0,
		BluetoothNotUSB = 1,
		AudioDeviceNotFound = 2,
		AudioEngineNotInitialized = 3,
		Unknown = 4,
};

extern "C" {

	// Constructor/Destructor
	DUALSENSEAPI void* Dualsense_Create(const char* path = "");
	DUALSENSEAPI void Dualsense_Destroy(void* handle);

	// General Properties
	DUALSENSEAPI bool Dualsense_IsConnected(void* handle);
	DUALSENSEAPI const char* Dualsense_GetPath(void* handle);
	DUALSENSEAPI const char* Dualsense_GetInstanceID(void* handle);
	DUALSENSEAPI const char* Dualsense_EnumerateControllerIDs();

	// Haptics and Audio
	DUALSENSEAPI void Dualsense_StopAllHaptics(void* handle);
	DUALSENSEAPI bool Dualsense_PlayHaptics(void* handle, const char* soundName, bool dontPlayIfAlreadyPlaying, bool loopUntilStopped);
	DUALSENSEAPI bool Dualsense_SetSoundVolume(void* handle, const char* soundName, float volume);
	DUALSENSEAPI bool Dualsense_SetSoundPitch(void* handle, const char* soundName, float pitch);
	DUALSENSEAPI void Dualsense_StopSoundsThatStartWith(void* handle, const char* prefix);
	DUALSENSEAPI void Dualsense_SetSpeakerVolume(void* handle, int Volume);

	// Configuration and Settings
	DUALSENSEAPI void Dualsense_SetLightbarColor(void* handle, uint8_t r, uint8_t g, uint8_t b);
	DUALSENSEAPI void Dualsense_SetPlayerLED(void* handle, uint8_t player);
	DUALSENSEAPI void Dualsense_SetMicrophoneLED(void* handle, bool led, bool pulse);
	DUALSENSEAPI void Dualsense_SetMicrophoneVolume(void* handle, int volume);
	DUALSENSEAPI void Dualsense_UseRumbleNotHaptics(void* handle, bool flag);

	// Trigger Settings
	DUALSENSEAPI void Dualsense_SetRightTrigger(void* handle, uint8_t mode, const uint8_t* forces);
	DUALSENSEAPI void Dualsense_SetLeftTrigger(void* handle, uint8_t mode, const uint8_t* forces);

	// Audio Control
	DUALSENSEAPI void Dualsense_InitializeAudioEngine(void* handle);
	DUALSENSEAPI const char* Dualsense_GetAudioDeviceName(void* handle);

	// Reconnection
	DUALSENSEAPI bool Dualsense_Connect(void* handle, bool samePort);

	DUALSENSEAPI bool Dualsense_Write(void* handle);
	DUALSENSEAPI void Dualsense_Read(void* handle);
	DUALSENSEAPI void Dualsense_GetButtonState(void* handle, ButtonState* state);
	DUALSENSEAPI bool Dualsense_LoadSound(void* handle, const char* SoundName, const char* Path);
	DUALSENSEAPI void Dualsense_SetRumble(void* handle, uint8_t LeftMotor, uint8_t RightMotor);
	DUALSENSEAPI HapticFeedbackStatus Dualsense_GetHapticFeedbackStatus(void* handle);
}