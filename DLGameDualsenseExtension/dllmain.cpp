// dllmain.cpp : Definiuje punkt wejścia dla aplikacji DLL.
#include "pch.h"

HANDLE hMainThread = nullptr;
const std::string folder = "haptics\\"; // folder path to sound files
std::atomic<bool> RunControllerLogic = true; // global bool for main loop

class Timer {
public:
	Timer(void) : start(std::chrono::high_resolution_clock::now()) {}

	auto Time() {
		auto now = std::chrono::high_resolution_clock::now();
		auto d = std::chrono::duration_cast<std::chrono::seconds>(now - start);
		return d;
	}

	void Restart() {
		start = std::chrono::high_resolution_clock::now();
	}

private:
	std::chrono::time_point<std::chrono::high_resolution_clock> start;
};


uintptr_t FindDMAAddy(HANDLE hProc, uintptr_t ptr, std::vector<unsigned int> offsets) {
	uintptr_t addr = ptr;
	for (unsigned int i = 0; i < offsets.size(); ++i) {
		ReadProcessMemory(hProc, (BYTE*)addr, &addr, sizeof(addr), 0);
		addr += offsets[i];
	}
	return addr;
}

struct PlayerVariables {
public:
	float HP = 0;
	int Anim = 0;
	int PauseState = 0;
	uint8_t UVActive = 0;
};

struct LEDAnimationValues {
public:
	struct UV {
		bool transitionDirectionRed = true;
		int purple[3] = { 60, 0, 255 };
		int red[3] = { 255, 0, 0 };
	};
	UV UV;
};

void LoadAllSounds(void* controller) { // it just works
	Dualsense_LoadSound(controller, "machete1", std::string(folder + "empty_machete_left_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "machete2", std::string(folder + "empty_machete_left_01_0.wav").c_str());
	Dualsense_LoadSound(controller, "machete3", std::string(folder + "empty_machete_right_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "machete4", std::string(folder + "empty_machete_right_01_0.wav").c_str());
	Dualsense_LoadSound(controller, "2h1", std::string(folder + "empty_hammer_heavy_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "2h2", std::string(folder + "empty_hammer_heavy_00_0-2.wav").c_str());
	Dualsense_LoadSound(controller, "2h3", std::string(folder + "empty_hammer_heavy_01_0.wav").c_str());
	Dualsense_LoadSound(controller, "2h4", std::string(folder + "empty_hammer_heavy_01_0-2.wav").c_str());
	Dualsense_LoadSound(controller, "2h5", std::string(folder + "empty_hammer_heavy_02_0.wav").c_str());
	Dualsense_LoadSound(controller, "2h6", std::string(folder + "empty_hammer_heavy_02_0-2.wav").c_str());
	Dualsense_LoadSound(controller, "2h7", std::string(folder + "empty_hammer_heavy_03_0.wav").c_str());
	Dualsense_LoadSound(controller, "2h8", std::string(folder + "empty_hammer_heavy_03_0-2.wav").c_str());
	Dualsense_LoadSound(controller, "groundpound", std::string(folder + "groundpound.wav").c_str());
	Dualsense_LoadSound(controller, "crossbowshot", std::string(folder + "crossbow_shot_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "blunt1", std::string(folder + "empty_wrench_left_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "blunt2", std::string(folder + "empty_wrench_left_01_0.wav").c_str());
	Dualsense_LoadSound(controller, "blunt3", std::string(folder + "empty_wrench_right_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "blunt4", std::string(folder + "empty_wrench_right_01_0.wav").c_str());
	Dualsense_LoadSound(controller, "knife1", std::string(folder + "empty_knife_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "knife2", std::string(folder + "empty_knife_01_0.wav").c_str());
	Dualsense_LoadSound(controller, "knife3", std::string(folder + "empty_knife_02_0.wav").c_str());
	Dualsense_LoadSound(controller, "knife4", std::string(folder + "empty_knife_03_0.wav").c_str());
	Dualsense_LoadSound(controller, "fists1", std::string(folder + "empty_fists_left_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "fists2", std::string(folder + "empty_fists_left_01_0.wav").c_str());
	Dualsense_LoadSound(controller, "fists3", std::string(folder + "empty_fists_right_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "fists4", std::string(folder + "empty_fists_right_01_0.wav").c_str());
	Dualsense_LoadSound(controller, "chainsaw", std::string(folder + "chainsaw.wav").c_str());
	Dualsense_LoadSound(controller, "rifleshot1", std::string(folder + "m4_shot_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "rifleshot2", std::string(folder + "m4_shot_01_0.wav").c_str());
	Dualsense_LoadSound(controller, "rifleshot3", std::string(folder + "m4_shot_02_0.wav").c_str());
	Dualsense_LoadSound(controller, "gunempty", std::string(folder + "rmngtn_empty_0.wav").c_str());
	Dualsense_LoadSound(controller, "smgshot1", std::string(folder + "submachinea_shot_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "smgshot2", std::string(folder + "submachinea_shot_01_0.wav").c_str());
	Dualsense_LoadSound(controller, "smgshot3", std::string(folder + "submachinea_shot_02_0.wav").c_str());
	Dualsense_LoadSound(controller, "bowshot", std::string(folder + "bow_shot_0.wav").c_str());
	Dualsense_LoadSound(controller, "bowreload", std::string(folder + "bow_reload_0.wav").c_str());
	Dualsense_LoadSound(controller, "shotgunshot", std::string(folder + "shotgunc_shot_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "revolvershot", std::string(folder + "rmngtn_shot_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "pistolshot", std::string(folder + "beretta_shot_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "pickupitem", std::string(folder + "pickup_default_0.wav").c_str());
	Dualsense_LoadSound(controller, "dropkick", std::string(folder + "wrestling_kick_empty_0.wav").c_str());
	Dualsense_LoadSound(controller, "dropkickend", std::string(folder + "wrestling_stand_up_0.wav").c_str());
	Dualsense_LoadSound(controller, "vanopening", std::string(folder + "van_opening_0.wav").c_str());
	Dualsense_LoadSound(controller, "watchbeep", std::string(folder + "watch_beep_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "zipline", std::string(folder + "zip_line_loop_0.wav").c_str());
	Dualsense_LoadSound(controller, "hangonedge2", std::string(folder + "climb_hang_shimmy_01_0.wav").c_str());
	Dualsense_LoadSound(controller, "kneefinisher", std::string(folder + "player_grab_finish_knee_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "elbowfinisher", std::string(folder + "elbowfinisher.wav").c_str());
	Dualsense_LoadSound(controller, "neckgrabfinisher", std::string(folder + "neckgrabfinisher.wav").c_str());
	Dualsense_LoadSound(controller, "neckfinisher", std::string(folder + "player_grab_finish_neck_0.wav").c_str());
	Dualsense_LoadSound(controller, "openingfridge", std::string(folder + "fridge_a_opening_0.wav").c_str());
	Dualsense_LoadSound(controller, "openingwoodendoor", std::string(folder + "barn_platform_opening_0.wav").c_str());
	Dualsense_LoadSound(controller, "cartrunkopening", std::string(folder + "sedan_trunk_opening_0.wav").c_str());
	Dualsense_LoadSound(controller, "camouflage", std::string(folder + "player_skill_camouflage_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "medkit", std::string(folder + "player_self_healing_bandage_0.wav").c_str());
	Dualsense_LoadSound(controller, "climbladderright", std::string(folder + "climb_metal_ladder_right_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "climbladderleft", std::string(folder + "climb_metal_ladder_left_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "slide", std::string(folder + "player_slide_loop_0.wav").c_str());
	Dualsense_LoadSound(controller, "landingoncar", std::string(folder + "landing_heavy_car_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "weaponrepair", std::string(folder + "player_repair_weapon_0.wav").c_str());
	Dualsense_LoadSound(controller, "landingontrash", std::string(folder + "landing_damper_trashbags_0.wav").c_str());
	Dualsense_LoadSound(controller, "safetyroll", std::string(folder + "landing_damper_roll_0.wav").c_str());
	Dualsense_LoadSound(controller, "remote", std::string(folder + "pursuit_remote_activate_0.wav").c_str());
	Dualsense_LoadSound(controller, "openingchest", std::string(folder + "chest_opening_0.wav").c_str());
	Dualsense_LoadSound(controller, "grapplinghookstart", std::string(folder + "weapon_rope_throw_0.wav").c_str());
	Dualsense_LoadSound(controller, "grapplinghookend", std::string(folder + "weapon_rope_throw_back_0.wav").c_str());
	Dualsense_LoadSound(controller, "landondirt", std::string(folder + "landing_weak_dirt_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "looting", std::string(folder + "looting_0.wav").c_str());
	Dualsense_LoadSound(controller, "hangonedge1", std::string(folder + "climb_hang_shimmy_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "hangonedge3", std::string(folder + "climb_hang_shimmy_02_0.wav").c_str());
	Dualsense_LoadSound(controller, "heavybodycollision", std::string(folder + "body_zombie_collision_ground_heavy_03_0.wav").c_str());
	Dualsense_LoadSound(controller, "zombie_bite_loopright", std::string(folder + "zombie_walker_grab_bite_long_right_0.wav").c_str());
	Dualsense_LoadSound(controller, "zombie_bite_loopleft", std::string(folder + "zombie_walker_grab_bite_long_left_0.wav").c_str());
	Dualsense_LoadSound(controller, "flashlighton", std::string(folder + "torch_on_0.wav").c_str());
	Dualsense_LoadSound(controller, "flashlightoff", std::string(folder + "torch_off_0.wav").c_str());
	Dualsense_LoadSound(controller, "uv_failed", std::string(folder + "uv_flashlight_start_failed_0.wav").c_str());
	Dualsense_LoadSound(controller, "uv_loop", std::string(folder + "uv_flashlight_working_loop_0.wav").c_str());
	Dualsense_LoadSound(controller, "hit1", std::string(folder + "hit_fists_00_0.wav").c_str());
	Dualsense_LoadSound(controller, "hit2", std::string(folder + "hit_fists_01_0.wav").c_str());
	Dualsense_LoadSound(controller, "hit3", std::string(folder + "hit_fists_02_0.wav").c_str());
	Dualsense_LoadSound(controller, "uichangeselection", std::string(folder + "uichangeselection.wav").c_str());
	Dualsense_LoadSound(controller, "uiselect", std::string(folder + "uiselect.wav").c_str());
	Dualsense_LoadSound(controller, "uiselect2", std::string(folder + "uiselect2.wav").c_str());
}

void MainThread() {
#define AnimRange(equal, smaller, bigger) playerVars.Anim == equal || playerVars.Anim >= smaller && playerVars.Anim <= bigger
#define AnimEqual(equal) playerVars.Anim == equal
#define AnimThreeDifferent(one, two, three) playerVars.Anim == one || playerVars.Anim == two || playerVars.Anim == three
#define AnimTwoDifferent(one, two) playerVars.Anim == one || playerVars.Anim == two
#define AnimFourDifferent(a, b, c, d) playerVars.Anim == a || playerVars.Anim == b || playerVars.Anim == c || playerVars.Anim == d
#define AnimFiveDifferent(a, b, c, d, e) playerVars.Anim == a || playerVars.Anim == b || playerVars.Anim == c || playerVars.Anim == d || playerVars.Anim == e
#define GamePaused playerVars.PauseState == 255
#define UVLightOn playerVars.UVActive == 1
#define UVLightOff playerVars.UVActive == 0
#define UVLightDepleted playerVars.UVActive == 256
#define FlashlightOn (int)vigem.Red == 255 && vigem.Green == 255 && vigem.Blue == 255
#define R2Pushed (buttonState.R2 >= triggerThreshold) && (lastButtonState.R2 < triggerThreshold)
#define R2Released (buttonState.R2 < triggerThreshold) && (lastButtonState.R2 >= triggerThreshold)

	void* controller = Dualsense_Create();
	ViGEm vigem;
	LEDAnimationValues ledAnimation;
	vigem.InitializeVigembus();
	if (!vigem.isWorking()) {
		MessageBox(0, L"Connection to VigemBus driver couldn't be made, are you sure you've installed VigemBus driver?", L"Error", 0);
	}
	else {
		vigem.StartDS4();

		bool MicLed = false;
		bool WasFlashlightOn = false;
		bool WasFlashlightOffPlayed = false;
		PlayerVariables playerVars;
		ButtonState buttonState{};
		ButtonState lastButtonState{};
		int triggerThreshold = 0;
		std::chrono::steady_clock::time_point LastTime = std::chrono::high_resolution_clock::now();
		std::chrono::steady_clock::time_point TimeNow = LastTime;

		std::this_thread::sleep_for(std::chrono::seconds(8)); // wait for 8 seconds before doing anything
		uintptr_t engineModuleBase = (uintptr_t)GetModuleHandle(L"engine_x64_rwdi.dll");
		if (!engineModuleBase) {
			std::cerr << "Failed to get engine_x64_rwdi.dll base address!" << std::endl;
			return;
		}
		std::cout << "Base engine address: " << std::hex << engineModuleBase << std::endl;

		uintptr_t playerBase = engineModuleBase + 0x00A10198;
		std::vector<unsigned int> weaponAnimNodeOffset{ 0x20,0xDF0,0x0, 0x338,0x1A8,0x30,0x24 };
		std::vector<unsigned int> healthOffset{ 0x20,0xDF0,0x0, 0x125C };
		std::vector<unsigned int> uvOffset{ 0x20,0xDF0,0x0, 0xC50, 0x54 };
		std::vector<unsigned int> pauseOffset{ 0x20,0xA20,0x30 };

#pragma region TriggerForces
		uint8_t none[11] = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		uint8_t oneHandedSharp[11] = { 20, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		uint8_t twoHanded[11] = { 0, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		uint8_t weaponThrow[11] = { 0, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		uint8_t crossbow[11] = { 55, 0, 75, 40, 90, 255, 0, 0, 0, 0, 0 };
		uint8_t oneHandedBlunt[11] = { 70, 170, 120, 0, 0, 0, 0, 0, 0, 0, 0 };
		uint8_t knife[11] = { 20, 1, 20, 0, 0, 0, 0, 0, 0, 0, 0 };
		uint8_t chainsaw[11] = { 35, 137, 65, 0, 0, 0, 0, 0, 0, 0, 0 };
		uint8_t fists[11] = { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		uint8_t automaticRifle[11] = { 10, 255, 120, 0, 0, 0, 0, 0, 0, 0, 0 };
		uint8_t automaticRifleEmpty[11] = { 120, 0, 255, 0, 0, 0, 0, 0, 0, 0, 0 };
		uint8_t submachineGun[11] = { 15, 255, 120, 0, 0, 0, 0, 0, 0, 0, 0 };
		uint8_t submachineGunEmpty[11] = { 120, 0, 255, 0, 0, 0, 0, 0, 0, 0, 0 };
		uint8_t bow[11] = { 0, 255, 0, 255, 255, 255, 0, 0, 0, 0, 0 };
		uint8_t shotgun[11] = { 10, 75, 82, 0, 0, 0, 0, 0, 0, 0, 0 };
		uint8_t doubleBarrelShotgun[11] = { 255, 0, 255, 0, 0, 0, 0, 0, 0, 0, 0 };
		uint8_t doubleActionRevolver[11] = { 255, 184, 255, 143, 71, 0, 0, 0, 0, 0, 0 };
		uint8_t singleActionRevolver[11] = { 93, 184, 255, 143, 71, 0, 0, 0, 0, 0, 0 };
		uint8_t pistol[11] = { 93, 184, 255, 143, 71, 0, 0, 0, 0, 0, 0 };
#pragma endregion

		Dualsense_InitializeAudioEngine(controller);
		LoadAllSounds(controller);
		HapticFeedbackStatus hf_status = Dualsense_GetHapticFeedbackStatus(controller);
		std::cout << hf_status << std::endl;

		Timer timer;
		int R, G, B = 0;
		while (RunControllerLogic) {
#pragma region ValueRead
			uintptr_t healthAddress = FindDMAAddy(GetCurrentProcess(), playerBase, healthOffset);
			ReadProcessMemory(GetCurrentProcess(), (LPCVOID)healthAddress, &playerVars.HP, sizeof(playerVars.HP), NULL);

			uintptr_t animAddress = FindDMAAddy(GetCurrentProcess(), playerBase, weaponAnimNodeOffset);
			ReadProcessMemory(GetCurrentProcess(), (LPCVOID)animAddress, &playerVars.Anim, sizeof(playerVars.Anim), NULL);

			uintptr_t pauseAddress = FindDMAAddy(GetCurrentProcess(), playerBase, pauseOffset);
			ReadProcessMemory(GetCurrentProcess(), (LPCVOID)pauseAddress, &playerVars.PauseState, sizeof(playerVars.PauseState), NULL);

			uintptr_t uvAddress = FindDMAAddy(GetCurrentProcess(), playerBase, uvOffset);
			ReadProcessMemory(GetCurrentProcess(), (LPCVOID)uvAddress, &playerVars.UVActive, sizeof(playerVars.UVActive), NULL);
#pragma endregion

#pragma region ReadAndVerifyControllerConnection
			Dualsense_Read(controller);
			Dualsense_GetButtonState(controller, &buttonState);

			if (!Dualsense_IsConnected(controller)) {
				if (Dualsense_Connect(controller, true));
				{
					std::this_thread::sleep_for(std::chrono::milliseconds(1100));
					Dualsense_InitializeAudioEngine(controller);
					if (Dualsense_GetAudioDeviceName(controller) == "")
						Dualsense_UseRumbleNotHaptics(controller, true);
					else
						Dualsense_UseRumbleNotHaptics(controller, false);
					LoadAllSounds(controller);
					hf_status = Dualsense_GetHapticFeedbackStatus(controller);
					std::cout << hf_status << std::endl;
				}
				std::this_thread::sleep_for(std::chrono::milliseconds(1000));
			}

			Dualsense_SetPlayerLED(controller, 1);
#pragma endregion

#pragma region ControllerEffectLogic

			if (GamePaused) {
				Dualsense_SetRightTrigger(controller, Trigger::Rigid_B, none);
				R = 255; G = 102; B = 0;

				if (buttonState.DpadLeft || buttonState.DpadRight || buttonState.DpadDown || buttonState.DpadUp) {
						Dualsense_PlayHaptics(controller, "uichangeselection", false, false);
						timer.Restart();
					
				}

				if (buttonState.cross) {
						Dualsense_PlayHaptics(controller, "uiselect2", false, false);
						timer.Restart();
					
				}

				if (buttonState.circle) {
						Dualsense_PlayHaptics(controller, "uiselect", false, false);
					
				}
			}
			else {
				if (AnimRange(2058, 2024, 2029)) { // 1H sharp
					triggerThreshold = 120;
					Dualsense_SetRightTrigger(controller, Trigger::Rigid_A, oneHandedSharp);

					if (R2Pushed) {
						switch (rand() % 4) {
						case 0: Dualsense_PlayHaptics(controller, "machete1", true, false); break;
						case 1: Dualsense_PlayHaptics(controller, "machete2", true, false); break;
						case 2: Dualsense_PlayHaptics(controller, "machete3", true, false); break;
						case 3: Dualsense_PlayHaptics(controller, "machete4", true, false); break;
						}
					}
				}
				else if (AnimRange(1974, 1940, 1960)) { // 2H weapon
					triggerThreshold = 120;
					Dualsense_SetRightTrigger(controller, Trigger::Rigid, twoHanded);

					if (R2Pushed && (timer.Time() > std::chrono::seconds(4))) {
						switch (rand() % 8) {
						case 0: Dualsense_PlayHaptics(controller, "2h1", true, false); break;
						case 1: Dualsense_PlayHaptics(controller, "2h2", true, false); break;
						case 2: Dualsense_PlayHaptics(controller, "2h3", true, false); break;
						case 3: Dualsense_PlayHaptics(controller, "2h4", true, false); break;
						case 4: Dualsense_PlayHaptics(controller, "2h5", true, false); break;
						case 5: Dualsense_PlayHaptics(controller, "2h6", true, false); break;
						case 6: Dualsense_PlayHaptics(controller, "2h7", true, false); break;
						case 7: Dualsense_PlayHaptics(controller, "2h8", true, false); break;
						}

						timer.Restart();
					}

					if (AnimEqual(1957)) {
						Dualsense_PlayHaptics(controller, "groundpound", true, false);
					}
				}
				else if (AnimThreeDifferent(1891, 2053, 1181)) { // Weapon throwing
					triggerThreshold = 120;
					Dualsense_SetRightTrigger(controller, Trigger::Rigid, weaponThrow);
				}
				else if (AnimTwoDifferent(886, 850)) { // Crossbow
					triggerThreshold = 255;
					Dualsense_SetRightTrigger(controller, Trigger::Rigid_A, crossbow);

					if (R2Pushed) {
						Dualsense_PlayHaptics(controller, "crossbowshot", true, false);
					}
				}
				else if (AnimRange(1897, 1861, 1897)) { // 1H blunt
					triggerThreshold = 255;
					Dualsense_SetRightTrigger(controller, Trigger::Rigid_A, oneHandedBlunt);

					if (R2Pushed) {
						switch (rand() % 4) {
						case 0: Dualsense_PlayHaptics(controller, "blunt1", true, false); break;
						case 1: Dualsense_PlayHaptics(controller, "blunt2", true, false); break;
						case 2: Dualsense_PlayHaptics(controller, "blunt3", true, false); break;
						case 3: Dualsense_PlayHaptics(controller, "blunt4", true, false); break;
						}
					}
				}
				else if (AnimEqual(1184) || AnimRange(1152, 1152, 1165)) { // Knives
					triggerThreshold = 120;
					Dualsense_SetRightTrigger(controller, Trigger::Rigid_A, knife);

					if (R2Pushed) {
						switch (rand() % 4) {
						case 0: Dualsense_PlayHaptics(controller, "knife1", true, false); break;
						case 1: Dualsense_PlayHaptics(controller, "knife2", true, false); break;
						case 2: Dualsense_PlayHaptics(controller, "knife3", true, false); break;
						case 3: Dualsense_PlayHaptics(controller, "knife4", true, false); break;
						}
					}
				}
				else if (AnimRange(933, 933, 949)) { // Fists
					triggerThreshold = 120;
					Dualsense_SetRightTrigger(controller, Trigger::Rigid_A, fists);

					if (R2Pushed) {
						switch (rand() % 4) {
						case 0: Dualsense_PlayHaptics(controller, "fists1", true, false); break;
						case 1: Dualsense_PlayHaptics(controller, "fists2", true, false); break;
						case 2: Dualsense_PlayHaptics(controller, "fists3", true, false); break;
						case 3: Dualsense_PlayHaptics(controller, "fists4", true, false); break;
						}
					}
				}
				else if (AnimEqual(830)) { // Chainsaw
					triggerThreshold = 255;
					Dualsense_SetRightTrigger(controller, Trigger::Pulse_B, chainsaw);

					if (R2Pushed) {
						Dualsense_PlayHaptics(controller, "chainsaw", true, false);
					}
				}
				else if (AnimEqual(1282) || AnimEqual(1246)) { // Automatic rifles
					triggerThreshold = 255;
					Dualsense_SetRightTrigger(controller, Trigger::Pulse_B, automaticRifle);

					if (buttonState.R2 >= triggerThreshold) {
						switch (rand() % 3) {
						case 0: Dualsense_PlayHaptics(controller, "rifleshot1", true, false); break;
						case 1: Dualsense_PlayHaptics(controller, "rifleshot2", true, false); break;
						case 2: Dualsense_PlayHaptics(controller, "rifleshot3", true, false); break;
						}
					}
				}
				else if (AnimEqual(1283)) { // Automatic rifle (EMPTY)
					triggerThreshold = 255;
					Dualsense_SetRightTrigger(controller, Trigger::Pulse_AB, automaticRifleEmpty);

					if (R2Pushed) {
						Dualsense_PlayHaptics(controller, "gunempty", true, false);
					}
				}
				else if (AnimEqual(2169) || AnimEqual(2079)) { // Submachine gun
					triggerThreshold = 255;
					Dualsense_SetRightTrigger(controller, Trigger::Pulse_B, submachineGun);

					if (buttonState.R2 >= triggerThreshold) {
						switch (rand() % 3) {
						case 0: Dualsense_PlayHaptics(controller, "smgshot1", true, false); break;
						case 1: Dualsense_PlayHaptics(controller, "smgshot2", true, false); break;
						case 2: Dualsense_PlayHaptics(controller, "smgshot3", true, false); break;
						}
					}
				}
				else if (AnimEqual(2170) || AnimEqual(2080)) { // Submachine gun (EMPTY)
					triggerThreshold = 255;
					Dualsense_SetRightTrigger(controller, Trigger::Pulse_AB, submachineGunEmpty);

					if (R2Pushed) {
						Dualsense_PlayHaptics(controller, "gunempty", true, false);
					}
				}
				else if (AnimEqual(757) || AnimRange(720, 720, 759)) { // Bows
					if (R2Released) {
						Dualsense_PlayHaptics(controller, "bowshot", true, false);
					}

					if (R2Pushed) {
						Dualsense_SetRightTrigger(controller, Trigger::Rigid, bow);
						Dualsense_PlayHaptics(controller, "bowreload", true, false);
					}
				}
				else if (AnimEqual(1715) || AnimEqual(1690)) { // Shotguns
					triggerThreshold = 255;
					Dualsense_SetRightTrigger(controller, Trigger::Pulse_AB, shotgun);

					if (R2Pushed) {
						Dualsense_PlayHaptics(controller, "shotgunshot", true, false);
					}
				}
				else if (AnimEqual(1724)) { // Double-barrel shotgun
					triggerThreshold = 255;
					Dualsense_SetRightTrigger(controller, Trigger::Pulse_AB, doubleBarrelShotgun);

					if (R2Pushed) {
						Dualsense_PlayHaptics(controller, "shotgunshot", true, false);
					}
				}
				else if (AnimEqual(1589) || AnimEqual(1562)) { // Double-Action Revolver
					triggerThreshold = 255;
					Dualsense_SetRightTrigger(controller, Trigger::Pulse_AB, doubleActionRevolver);

					if (R2Pushed) {
						Dualsense_PlayHaptics(controller, "revolvershot", true, false);
					}
				}
				else if (AnimEqual(1811) || AnimEqual(1783) || AnimEqual(1812)) { // Single-Action Revolver
					triggerThreshold = 255;
					Dualsense_SetRightTrigger(controller, Trigger::Pulse_AB, singleActionRevolver);

					if (R2Pushed) {
						Dualsense_PlayHaptics(controller, "revolvershot", true, false);
					}
				}
				else if (AnimEqual(703) || AnimEqual(667) || AnimEqual(704)) { // Pistol
					triggerThreshold = 255;
					Dualsense_SetRightTrigger(controller, Trigger::Rigid_AB, pistol);

					if (R2Pushed) {
						Dualsense_PlayHaptics(controller, "pistolshot", true, false);
					}
				}
				else if (AnimEqual(2454)) { // Item pickup
					Dualsense_PlayHaptics(controller, "pickupitem", true, false);
				}
				else if (AnimEqual(2403)) { // Dropkick start
					Dualsense_PlayHaptics(controller, "dropkick", true, false);
				}
				else if (AnimEqual(2404)) { // Dropkick end
					Dualsense_PlayHaptics(controller, "dropkickend", true, false);
				}
				else if (AnimEqual(5546)) { // Open van
					Dualsense_PlayHaptics(controller, "vanopening", true, false);
				}
				else if (AnimEqual(2790)) { // Watch
					Dualsense_PlayHaptics(controller, "watchbeep", true, false);
				}
				else if (playerVars.Anim >= 2730 && playerVars.Anim <= 2738 && playerVars.Anim != 2731) { // Zipline
					Dualsense_PlayHaptics(controller, "zipline", true, true);
				}
				else if (AnimEqual(2731)) { // Wallrun
					Dualsense_PlayHaptics(controller, "hangonedge2", true, false);
				}
				else if (AnimEqual(2488)) { // Bruteforce door
					Dualsense_PlayHaptics(controller, "groundpound", true, false);
				}
				else if (AnimEqual(2625)) { // Finishing kick
					Dualsense_PlayHaptics(controller, "kneefinisher", true, false);
				}
				else if (AnimEqual(2341)) { // Knee finisher
					Dualsense_PlayHaptics(controller, "kneefinisher", true, false);
				}
				else if (AnimEqual(2340)) { // Elbow finisher
					Dualsense_PlayHaptics(controller, "elbowfinisher", true, false);
				}
				else if (AnimEqual(2342)) { // Neck grab finisher
					Dualsense_PlayHaptics(controller, "neckgrabfinisher", true, false);
				}
				else if (AnimEqual(2343)) { // Neck finisher
					Dualsense_PlayHaptics(controller, "neckfinisher", true, false);
				}
				else if (AnimEqual(1324)) { // Open door
					Dualsense_PlayHaptics(controller, "hangonedge1", true, false);
				}
				else if (AnimEqual(2827) || AnimEqual(2829)) { // Open fridge
					Dualsense_PlayHaptics(controller, "openingfridge", true, false);
				}
				else if (AnimEqual(5540) || AnimEqual(2835)) { // Open closet
					Dualsense_PlayHaptics(controller, "openingwoodendoor", true, false);
				}
				else if (AnimEqual(2969) || AnimEqual(2967)) { // Open car
					Dualsense_PlayHaptics(controller, "cartrunkopening", true, false);
				}
				else if (AnimEqual(2282)) { // Smearing zombie guts
					Dualsense_PlayHaptics(controller, "camouflage", true, false);
				}
				else if (AnimEqual(660)) { // Using medkit
					Dualsense_PlayHaptics(controller, "medkit", true, false);
				}
				else if (AnimEqual(2460) || AnimEqual(2457) || AnimEqual(2607)) { // Climb pipe right hand
					Dualsense_PlayHaptics(controller, "climbladderright", true, false);
				}
				else if (AnimEqual(2459) || AnimEqual(2458) || AnimEqual(2610)) { // Climb pipe left hand
					Dualsense_PlayHaptics(controller, "climbladderleft", true, false);
				}
				else if (AnimEqual(2475) || AnimEqual(2422)) { // Slide down a pipe
					Dualsense_PlayHaptics(controller, "slide", true, false);
				}
				else if (AnimEqual(2473)) { // Rotating left on a pipe
					Dualsense_PlayHaptics(controller, "slide", true, false);
				}
				else if (AnimEqual(2474)) { // Rotating right on a pipe
					Dualsense_PlayHaptics(controller, "slide", true, false);
				}
				else if (AnimEqual(2408) || AnimEqual(2406)) { // Climbing on ladder left
					Dualsense_PlayHaptics(controller, "climbladderleft", true, false);
				}
				else if (AnimEqual(2409) || AnimEqual(2407)) { // Climbing on ladder right
					Dualsense_PlayHaptics(controller, "climbladderright", true, false);
				}
				else if (AnimEqual(2288)) { // Landed on car
					Dualsense_PlayHaptics(controller, "landingoncar", true, false);
				}
				else if (AnimEqual(2738)) { // Repairing weapon
					Dualsense_PlayHaptics(controller, "weaponrepair", true, false);
				}
				else if (AnimEqual(2290)) { // Landed on trash
					Dualsense_PlayHaptics(controller, "landingontrash", true, false);
				}
				else if (AnimEqual(2284)) { // Rolling
					Dualsense_PlayHaptics(controller, "safetyroll", true, false);
				}
				else if (AnimEqual(1322)) { // Using remote control
					Dualsense_PlayHaptics(controller, "remote", true, false);
				}
				else if (AnimEqual(2907)) { // Opening chest
					Dualsense_PlayHaptics(controller, "openingchest", true, false);
				}
				else if (AnimEqual(2611)) { // Sliding
					Dualsense_PlayHaptics(controller, "slide", true, false);
				}
				else if (AnimEqual(2048)) { // Grappling hook start
					Dualsense_PlayHaptics(controller, "grapplinghookstart", true, false);
				}
				else if (AnimEqual(2047)) { // Grappling hook loop
					Dualsense_PlayHaptics(controller, "grapplinghookend", true, false);
				}
				else if (AnimEqual(2283) || AnimEqual(2329) || AnimEqual(2423)) { // Soft landing
					Dualsense_PlayHaptics(controller, "landondirt", true, false);
				}
				else if (AnimEqual(2256) || AnimEqual(2257)) { // Looting
					Dualsense_PlayHaptics(controller, "looting", true, false);
				}
				else if (AnimEqual(2603)) { // Climb right
					Dualsense_PlayHaptics(controller, "hangonedge2", true, false);
				}
				else if (AnimEqual(2599)) { // Climb left
					Dualsense_PlayHaptics(controller, "hangonedge1", true, false);
				}
				else if (AnimRange(2500, 2500, 2560)) { // Common climbs
					switch (rand() % 3) {
					case 0: Dualsense_PlayHaptics(controller, "hangonedge1", true, false); break;
					case 1: Dualsense_PlayHaptics(controller, "hangonedge2", true, false); break;
					case 2: Dualsense_PlayHaptics(controller, "hangonedge3", true, false); break;
					}
				}
				else if (AnimRange(2520, 2520, 2536)) { // Climbing on a wall
					switch (rand() % 3) {
					case 0: Dualsense_PlayHaptics(controller, "hangonedge1", true, false); break;
					case 1: Dualsense_PlayHaptics(controller, "hangonedge2", true, false); break;
					case 2: Dualsense_PlayHaptics(controller, "hangonedge3", true, false); break;
					}
				}
				else if (AnimEqual(2388)) { // Jump over enemies
					switch (rand() % 3) {
					case 0: Dualsense_PlayHaptics(controller, "blunt1", true, false); break;
					case 1: Dualsense_PlayHaptics(controller, "blunt3", true, false); break;
					case 2: Dualsense_PlayHaptics(controller, "blunt6", true, false); break;
					}
				}
				else if (AnimEqual(2319)) { // Neck snap from behind
					Dualsense_PlayHaptics(controller, "neckfinisher", true, false);
				}
				else if (AnimEqual(1210)) { // Heavy body collision
					Dualsense_PlayHaptics(controller, "heavybodycollision", true, false);
				}
				else {
					Dualsense_SetLeftTrigger(controller, Trigger::Off, none);
					Dualsense_SetRightTrigger(controller, Trigger::Off, none);
				}

				if (AnimEqual(2440) || AnimEqual(2438)) { // Grabbed by biter right
					Dualsense_PlayHaptics(controller, "zombie_bite_loopright", true, false);
				}
				else if (AnimEqual(2441)) { // Grabbed by biter right
					Dualsense_PlayHaptics(controller, "zombie_bite_loopright", true, false);
				}
				else if (AnimEqual(2328)) { // Grabbed by biter left
					Dualsense_PlayHaptics(controller, "zombie_bite_loopleft", true, false);
				}
				else if (AnimEqual(2327)) { // Broke out of biter grab left
					Dualsense_PlayHaptics(controller, "zombie_bite_loopleft", true, false);
				}
				else if (AnimEqual(2439)) { // Grabbed by biter front
					Dualsense_PlayHaptics(controller, "zombie_bite_loopright", true, false);
				}
				else {
					Dualsense_StopSoundsThatStartWith(controller, "zombie_bite_");
				}

				/*
				if (UVLightDepleted) {
					if (ledAnimation.UV.transitionDirectionRed) {
						if (R < ledAnimation.UV.red[0]) R += 1;
						if (B > ledAnimation.UV.red[2]) B -= 1;

						// Check if we've reached color2
						if (R >= ledAnimation.UV.red[0] && B <= ledAnimation.UV.red[2]) {
							ledAnimation.UV.transitionDirectionRed = false; // Reverse direction
						}
					}
					else {
						if (R > ledAnimation.UV.red[0]) R -= 1;
						if (B < ledAnimation.UV.red[2]) B += 1;

						// Check if we've reached color2
						if (R <= ledAnimation.UV.red[0] && B >= ledAnimation.UV.red[2]) {
							ledAnimation.UV.transitionDirectionRed = true; // Reverse direction
						}
					}

					if (R2Pushed) {
						Dualsense_PlayHaptics(controller, "uv_failed", true, false);
					}
				}
				*/
				if (UVLightOn) {
					R = 60; G = 0; B = 255;
					Dualsense_PlayHaptics(controller, "uv_loop", true, true);
				}
				else if (FlashlightOn) {
					if (!WasFlashlightOn) {
						Dualsense_PlayHaptics(controller, "flashlighton", true, false);
						WasFlashlightOn = true;
						WasFlashlightOffPlayed = false;
					}			
					
					Dualsense_StopSoundsThatStartWith(controller, "uv_");
					R = 255; G = 255; B = 255;
				}
				else {
					Dualsense_StopSoundsThatStartWith(controller, "uv_");

					if (playerVars.Anim <= 2730 || playerVars.Anim >= 2738) {
						Dualsense_StopSoundsThatStartWith(controller, "zipline");
					}

					if (!WasFlashlightOffPlayed && WasFlashlightOn) {
						Dualsense_PlayHaptics(controller, "flashlightoff", true, false);
						WasFlashlightOffPlayed = true;
						WasFlashlightOn = false;
					}

					if(vigem.Red > 0 || vigem.Green > 0 || vigem.Blue > 0){
						R = vigem.Red; G = vigem.Green; B = vigem.Blue;
					}
				}
			}

			if (vigem.rotors.lrotor == 153 && vigem.rotors.rrotor == 153 || vigem.rotors.lrotor == 168 && vigem.rotors.rrotor == 168) {
				switch (rand() % 3) {
				case 0: Dualsense_PlayHaptics(controller, "hit1", true, false); break;
				case 1: Dualsense_PlayHaptics(controller, "hit2", true, false); break;
				case 2: Dualsense_PlayHaptics(controller, "hit3", true, false); break;
				}
			}
#pragma endregion

			skiplogic:
#pragma region ControllerWrite

			if (!buttonState.micBtn && lastButtonState.micBtn && !MicLed) {
				Dualsense_SetMicrophoneLED(controller, true, false);
				MicLed = true;
			}
			else if (!buttonState.micBtn && lastButtonState.micBtn && MicLed) {
				Dualsense_SetMicrophoneLED(controller, false, false);
				MicLed = false;
			}

			Dualsense_SetRumble(controller, vigem.rotors.lrotor, vigem.rotors.rrotor);
			Dualsense_SetLightbarColor(controller, (uint8_t)R, (uint8_t)G, (uint8_t)B);
			Dualsense_Write(controller);
			if (buttonState.R2 < triggerThreshold)
				buttonState.R2 = 0;
			vigem.UpdateDS4(buttonState);
#pragma endregion

			lastButtonState = buttonState;
			std::this_thread::sleep_for(std::chrono::milliseconds(1));
		}
	}

	Dualsense_Destroy(controller);
}

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
) {
	switch (ul_reason_for_call) {
		case DLL_PROCESS_ATTACH:
		{	
			/*
			AllocConsole();
			FILE* fDummy;
			freopen_s(&fDummy, "CONIN$", "r", stdin);
			freopen_s(&fDummy, "CONOUT$", "w", stderr);
			freopen_s(&fDummy, "CONOUT$", "w", stdout);
			*/

			std::cout << "Starting dualsense mod" << std::endl;

			hMainThread = CreateThread(0, 0, (LPTHREAD_START_ROUTINE)MainThread, hModule, 0, 0);

			break;
		}
		case DLL_PROCESS_DETACH:
		{
			RunControllerLogic = false;

			if (hMainThread != nullptr) {
				WaitForSingleObject(hMainThread, INFINITE);
				CloseHandle(hMainThread);
			}
			break;
		}
	}
	return TRUE;
}

