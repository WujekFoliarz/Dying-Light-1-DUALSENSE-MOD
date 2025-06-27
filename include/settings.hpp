#ifndef SETTINGS_H  
#define SETTINGS_H  

#include "game.hpp"  
#include <unordered_map>  
#include <duaLib.h>
#include <chrono>
#include <vector>
#include <string>
#include <random>

std::string getRandomString(const std::vector<std::string>& vec) {
	if (vec.empty()) return "";

	static std::random_device rd;
	static std::mt19937 gen(rd());
	std::uniform_int_distribution<> dis(0, vec.size() - 1);

	return vec[dis(gen)];
}

struct HapticEffect {
	std::chrono::milliseconds delayBeforeNextPlay = std::chrono::milliseconds(0);
	uint8_t triggerThreshold;
	std::vector<std::string> effectsOnAnimation;
	std::vector<std::string> effectsOnR2Pushed;
	std::vector<std::string> effectsOnR2Released;
	std::vector<std::string> effectsOnR2Held;
	bool loop = false;
};

std::unordered_map<AnimationType, ScePadTriggerEffectParam> g_triggerSettings = {
	{AnimationType::None, {
		   SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
		   {0},
		   {
			   {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
			   {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}}
		   }
	}},

	{AnimationType::EmptyHanded, {
		   SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_L2 | SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
		   {0},
		   {
			   {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
			   {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}}
		   }
	}},

	{AnimationType::OneHandedSharp, {
		   SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
		   {0}, {
			   {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
			   {SCE_PAD_TRIGGER_EFFECT_MODE_MULTIPLE_POSITION_FEEDBACK, {0}, {.multiplePositionFeedbackParam = {0,0,1,0,0,1,0,0,8,0}}}
		   }
	}},


	{AnimationType::TwoHanded, {
		SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
		{0}, {
			{SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
			{SCE_PAD_TRIGGER_EFFECT_MODE_FEEDBACK, {0}, {.feedbackParam = {0,8}}}
		}
	}},

	{AnimationType::WeaponThrow, {
		   SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
		   {0}, {
			   {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
			   {SCE_PAD_TRIGGER_EFFECT_MODE_MULTIPLE_POSITION_FEEDBACK, {0}, {.multiplePositionFeedbackParam = {8,8,8,8,8,8,8,8,0,0}}}
		   }
	}},

	{AnimationType::Crossbow, {
		   SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
		   {0}, {
			   {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
			   {SCE_PAD_TRIGGER_EFFECT_MODE_WEAPON, {0}, {.weaponParam = {2,5,8}}}
		   }
	}},

	{AnimationType::CrossbowEmpty, {
		   SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
		   {0}, {
			   {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
			   {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}}
		   }
	}},

	{AnimationType::OneHandedBlunt, {
		SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
		{0}, {
			{SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
			{SCE_PAD_TRIGGER_EFFECT_MODE_FEEDBACK, {0}, {.feedbackParam = {4,6}}}
		}
   }},

   {AnimationType::Knife, {
		SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
		{0}, {
			{SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
			{SCE_PAD_TRIGGER_EFFECT_MODE_SLOPE_FEEDBACK, {0}, {.slopeFeedbackParam = {7,9,7,7}}}
		}
   }},


	{AnimationType::Fists, {
		   SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
		   {0}, {
			   {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
			   {SCE_PAD_TRIGGER_EFFECT_MODE_MULTIPLE_POSITION_FEEDBACK, {0}, {.multiplePositionFeedbackParam = {6,8,0,0,0,4,0,0,0,0}}}
		   }
	}},

	{AnimationType::Chainsaw, {
		   SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
		   {0}, {
			   {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
			   {SCE_PAD_TRIGGER_EFFECT_MODE_VIBRATION, {0}, {.vibrationParam = {5,8,40}}}
		   }
	}},

	{AnimationType::ChainsawEmpty, {
		   SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
		   {0}, {
			   {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
			   {SCE_PAD_TRIGGER_EFFECT_MODE_WEAPON, {0}, {.weaponParam = {6,8,8}}}
		   }
	}},

	{AnimationType::AutomaticRifle, {
		   SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
		   {0}, {
			   {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
			   {SCE_PAD_TRIGGER_EFFECT_MODE_VIBRATION, {0}, {.vibrationParam = {6,8,10}}}
		   }
	}},

	{AnimationType::AutomaticRifleEmpty, {
		   SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
		   {0}, {
			   {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
			   {SCE_PAD_TRIGGER_EFFECT_MODE_WEAPON, {0}, {.weaponParam = {6,8,8}}}
		   }
	}},

	{AnimationType::SubmachineGun, {
		   SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
		   {0}, {
			   {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
			   {SCE_PAD_TRIGGER_EFFECT_MODE_VIBRATION, {0}, {.vibrationParam = {6,8,15}}}
		   }
	}},


	{AnimationType::SubmachineGunEmpty, {
		   SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
		   {0}, {
			   {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
			   {SCE_PAD_TRIGGER_EFFECT_MODE_WEAPON, {0}, {.weaponParam = {6,8,4}}}
		   }
	}},

	{AnimationType::Bow, {
		  SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
		  {0}, {
			  {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
			  {SCE_PAD_TRIGGER_EFFECT_MODE_SLOPE_FEEDBACK, {0}, {.slopeFeedbackParam = {1,9,1,8}}}
		  }
	}},
	
	{AnimationType::BowEmpty, {
		  SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
		  {0}, {
			  {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
			  {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}}
		  }
	} },

   {AnimationType::Shotgun, {
	   SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
	   {0}, {
		   {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
		   {SCE_PAD_TRIGGER_EFFECT_MODE_WEAPON, {0}, {.weaponParam = {3,7,8}}}
	   }
   }},

   {AnimationType::ShotgunEmpty, {
	  SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
	  {0}, {
		  {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
		  {SCE_PAD_TRIGGER_EFFECT_MODE_WEAPON, {0}, {.weaponParam = {3,7,8}}}
	  }
   }},

   {AnimationType::DoubleBarrelShotgun, {
	  SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
	  {0}, {
		  {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
		  {SCE_PAD_TRIGGER_EFFECT_MODE_WEAPON, {0}, {.weaponParam = {2,4,8}}}
	  }
   }},

   { AnimationType::DoubleBarrelShotgunEmpty, {
	  SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
	  {0}, {
		  {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
		  {SCE_PAD_TRIGGER_EFFECT_MODE_WEAPON, {0}, {.weaponParam = {2,4,8}}}
	  }
   } },

   {AnimationType::DoubleActionRevolver, {
	  SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
	  {0}, {
		  {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
		  {SCE_PAD_TRIGGER_EFFECT_MODE_WEAPON, {0}, {.weaponParam = {3,5,8}}}
	  }
   }},

   { AnimationType::DoubleActionRevolverEmpty, {
	  SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
	  {0}, {
		  {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
		  {SCE_PAD_TRIGGER_EFFECT_MODE_WEAPON, {0}, {.weaponParam = {3,5,8}}}
	  }
   } },

   { AnimationType::SingleActionRevolver, {
	  SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
	  {0}, {
		  {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
		  {SCE_PAD_TRIGGER_EFFECT_MODE_WEAPON, {0}, {.weaponParam = {3,5,4}}}
	  }
   }},

   { AnimationType::SingleActionRevolverEmpty, {
	  SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
	  {0}, {
		  {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
		  {SCE_PAD_TRIGGER_EFFECT_MODE_WEAPON, {0}, {.weaponParam = {3,5,4}}}
	  }
   } },

   {AnimationType::Pistol, {
	  SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
	  {0}, {
		  {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
		  {SCE_PAD_TRIGGER_EFFECT_MODE_WEAPON, {0}, {.weaponParam = {4,8,8}}}
	  }
   }},

   { AnimationType::PistolEmpty, {
	  SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
	  {0}, {
		  {SCE_PAD_TRIGGER_EFFECT_MODE_OFF, {0}, {}},
		  {SCE_PAD_TRIGGER_EFFECT_MODE_WEAPON, {0}, {.weaponParam = {4,8,8}}}
	  }
   } },

   {AnimationType::InBuggy, {
	  SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_L2 | SCE_PAD_TRIGGER_EFFECT_TRIGGER_MASK_R2,
	  {0}, {
		  {SCE_PAD_TRIGGER_EFFECT_MODE_FEEDBACK, {0}, {.feedbackParam = {3,4}}},
		  {SCE_PAD_TRIGGER_EFFECT_MODE_FEEDBACK, {0}, {.feedbackParam = {4,8}}}
	  }
   }},
};

std::unordered_map<AnimationType, HapticEffect> g_hapticSettings = {
	{AnimationType::OneHandedSharp, {std::chrono::milliseconds(10), 120, {}, { "empty_machete_left_00_0.wav", "empty_machete_left_01_0.wav", "empty_machete_right_00_0.wav", "empty_machete_right_01_0.wav" }, {}, {}}},
	{AnimationType::OneHandedBlunt, {std::chrono::milliseconds(25), 120, {}, { "empty_wrench_left_00_0.wav", "empty_wrench_left_01_0.wav", "empty_wrench_right_00_0.wav", "empty_wrench_right_01_0.wav"}, {}, {}}},
	{AnimationType::Fists, {std::chrono::milliseconds(50), 120, {}, { "empty_fists_left_00_0.wav", "empty_fists_left_01_0.wav", "empty_fists_right_00_0.wav","empty_fists_right_01_0.wav"}, {}, {}}},
	{AnimationType::Chainsaw, {std::chrono::milliseconds(0), 250, {}, {}, {}, {"chainsaw.wav"}, true}},
	{AnimationType::AutomaticRifle, {std::chrono::milliseconds(100), 120, {}, {}, {}, {"m4_shot_00_0.wav", "m4_shot_01_0.wav", "m4_shot_02_0.wav"}}},
	{AnimationType::Knife, {std::chrono::milliseconds(10), 120, {}, { "empty_knife_00_0.wav","empty_knife_01_0.wav","empty_knife_02_0.wav","empty_knife_03_0.wav" }, {}, {}}},
	{AnimationType::TwoHanded, {std::chrono::milliseconds(2300), 120, {}, { "empty_hammer_heavy_00_0.wav", "empty_hammer_heavy_00_0-2.wav", "empty_hammer_heavy_01_0.wav", "empty_hammer_heavy_01_0-2.wav", "empty_hammer_heavy_02_0.wav", "empty_hammer_heavy_02_0-2.wav", "empty_hammer_heavy_03_0.wav", "empty_hammer_heavy_03_0-2.wav",  }, {}, {}, false}},
	{AnimationType::Crossbow, {std::chrono::milliseconds(1000), 120, {}, { "crossbow_shot_00_0.wav",  }, {}, {}}},
	{AnimationType::CrossbowEmpty, {std::chrono::milliseconds(1000), 120, {}, {}, {}, {}}},
	{AnimationType::Pistol, {std::chrono::milliseconds(200), 120, {}, { "beretta_shot_00_0.wav",  }, {}, {}}},
	{AnimationType::SingleActionRevolver, {std::chrono::milliseconds(200), 120, {}, { "rmngtn_shot_00_0.wav",  }, {}, {}}},
	{AnimationType::DoubleActionRevolver, {std::chrono::milliseconds(200), 120, {}, { "rmngtn_shot_00_0.wav",  }, {}, {}}},
	{AnimationType::Bow, {std::chrono::milliseconds(200), 120, {}, {"bow_reload_0.wav"}, {"bow_shot_0.wav"}, {}}},
	{AnimationType::Shotgun, {std::chrono::milliseconds(100), 120, {}, {"shotgunc_shot_00_0.wav"}, {}, {}}},
	{AnimationType::DoubleBarrelShotgun, {std::chrono::milliseconds(100), 120, {}, {"shotgunc_shot_00_0.wav"}, {}, {}}},
	{AnimationType::SubmachineGun, {std::chrono::milliseconds(80), 120, {}, {}, {}, {"submachinea_shot_00_0.wav", "submachinea_shot_01_0.wav", "submachinea_shot_02_0.wav"}}},
	{AnimationType::SubmachineGunEmpty, {std::chrono::milliseconds(0), 120, {}, {"rmngtn_empty_0.wav"}, {}, {}}},
	{AnimationType::AutomaticRifleEmpty, {std::chrono::milliseconds(0), 120, {}, {"rmngtn_empty_0.wav"}, {}, {}}},
	{AnimationType::PistolEmpty, {std::chrono::milliseconds(0), 120, {}, {"rmngtn_empty_0.wav"}, {}, {}}},
	{AnimationType::ShotgunEmpty, {std::chrono::milliseconds(0), 120, {}, {"rmngtn_empty_0.wav"}, {}, {}}},
	{AnimationType::DoubleBarrelShotgunEmpty, {std::chrono::milliseconds(0), 120, {}, {"rmngtn_empty_0.wav"}, {}, {}}},
	{AnimationType::DoubleActionRevolverEmpty, {std::chrono::milliseconds(0), 120, {}, {"rmngtn_empty_0.wav"}, {}, {}}},
	{AnimationType::SingleActionRevolverEmpty, {std::chrono::milliseconds(0), 120, {}, {"rmngtn_empty_0.wav"}, {}, {}}},

	{AnimationType::ItemPickup, {std::chrono::milliseconds(0), 0,			{"pickup_default_0.wav"}, {}, {},{}, }},
	{AnimationType::DropkickStart, {std::chrono::milliseconds(0), 0,		{"wrestling_kick_empty_0.wav"}, {}, {},{}, }},
	{AnimationType::DropkickEnd, {std::chrono::milliseconds(0), 0,			{"wrestling_stand_up_0.wav"}, {}, {},{}, }},
	{AnimationType::OpenVan, {std::chrono::milliseconds(0), 0,				{"van_opening_0.wav"}, {}, {},{}, }},
	{AnimationType::Watch, {std::chrono::milliseconds(0), 0,				{"watch_beep_00_0.wav"}, {}, {}, {}, }},
	{AnimationType::Zipline, {std::chrono::milliseconds(0), 0,				{"zip_line_loop_0.wav"}, {}, {},{}, true}},
	{AnimationType::Wallrun, {std::chrono::milliseconds(0), 0,				{"climb_hang_shimmy_01_0.wav"}, {}, {},{}, }},
	{AnimationType::BruteforceDoor, {std::chrono::milliseconds(0), 0,		{"groundpound.wav"}, {}, {},{}, }},
	{AnimationType::FinishingKick, {std::chrono::milliseconds(0), 0,		{"player_grab_finish_knee_00_0.wav"}, {}, {},{}, }},
	{AnimationType::KneeFinisher, {std::chrono::milliseconds(0), 0,			{"player_grab_finish_knee_00_0.wav"}, {}, {},{}, }},
	{AnimationType::ElbowFinisher, {std::chrono::milliseconds(0), 0,		{"elbowfinisher.wav"}, {}, {},{}, }},
	{AnimationType::NeckGrabFinisher, {std::chrono::milliseconds(0), 0,		{"neckgrabfinisher.wav"}, {}, {},{}, }},
	{AnimationType::NeckFinisher, {std::chrono::milliseconds(0), 0,			{"player_grab_finish_neck_0.wav"}, {}, {},{}, }},
	{AnimationType::OpenDoor, {std::chrono::milliseconds(0), 0,				{"climb_hang_shimmy_00_0.wav"}, {}, {},{}, }},
	{AnimationType::OpenFridge, {std::chrono::milliseconds(0), 0,			{"fridge_a_opening_0.wav"}, {}, {},{}, }},
	{AnimationType::OpenCloset, {std::chrono::milliseconds(0), 0,			{"barn_platform_opening_0.wav"}, {}, {},{}, }},
	{AnimationType::OpenCar, {std::chrono::milliseconds(0), 0,				{"sedan_trunk_opening_0.wav"}, {}, {},{}, }},
	{AnimationType::ZombieGuts, {std::chrono::milliseconds(0), 0,			{"player_skill_camouflage_00_0.wav"}, {}, {},{}, }},
	{AnimationType::Medkit, {std::chrono::milliseconds(0), 0,				{"player_self_healing_bandage_0.wav"}, {}, {},{}, }},
	{AnimationType::ClimbPipeRight, {std::chrono::milliseconds(0), 0,		{"climb_metal_ladder_right_00_0.wav"}, {}, {},{}, }},
	{AnimationType::ClimbPipeLeft, {std::chrono::milliseconds(0), 0,		{"climb_metal_ladder_left_00_0.wav"}, {}, {},{}, }},
	{AnimationType::SlidePipe, {std::chrono::milliseconds(0), 0,			{"player_slide_loop_0.wav"}, {}, {},{}, }},
	{AnimationType::RotateLeftPipe, {std::chrono::milliseconds(0), 0,		{"climb_hang_shimmy_00_0.wav"}, {}, {},{}, }},
	{AnimationType::RotateRightPipe, {std::chrono::milliseconds(0), 0,		{"climb_hang_shimmy_01_0.wav"}, {}, {},{},}},
	{AnimationType::ClimbLadderLeft, {std::chrono::milliseconds(0), 0,		{"climb_metal_ladder_left_00_0.wav"}, {}, {},{}, }},
	{AnimationType::ClimbLadderRight, {std::chrono::milliseconds(0), 0,		{"climb_metal_ladder_right_00_0.wav"}, {}, {},{}, }},
	{AnimationType::LandOnCar, {std::chrono::milliseconds(0), 0,			{"landing_heavy_car_00_0.wav"}, {}, {},{}, }},
	{AnimationType::WeaponRepair, {std::chrono::milliseconds(0), 0,			{"player_repair_weapon_0.wav"}, {}, {},{}, }},
	{AnimationType::LandOnTrash, {std::chrono::milliseconds(0), 0,			{"landing_damper_trashbags_0.wav"}, {}, {},{}, }},
	{AnimationType::SafetyRoll, {std::chrono::milliseconds(0), 0,			{"landing_damper_roll_0.wav"}, {}, {},{}, }},
	{AnimationType::RemoteControl, {std::chrono::milliseconds(0), 0,		{"pursuit_remote_activate_0.wav"}, {}, {}, {},}},
	{AnimationType::OpenChest, {std::chrono::milliseconds(0), 0,			{"chest_opening_0.wav"}, {}, {},{}, }},
	{AnimationType::Sliding, {std::chrono::milliseconds(0), 0,				{"player_slide_loop_0.wav"}, {}, {},{}, }},
	{AnimationType::GrapplingHookStart, {std::chrono::milliseconds(0), 0,	{"weapon_rope_throw_0.wav"}, {}, {}, {},}},
	{AnimationType::GrapplingHookLoop, {std::chrono::milliseconds(0), 0,	{"weapon_rope_throw_back_0.wav"}, {}, {},{}, }},
	{AnimationType::SoftLanding, {std::chrono::milliseconds(0), 0,			{"landing_weak_dirt_00_0.wav"}, {}, {},{}, }},
	{AnimationType::Looting, {std::chrono::milliseconds(0), 0,				{"looting_0.wav"}, {}, {},{}, }},
	{AnimationType::ClimbRight, {std::chrono::milliseconds(0), 0,			{"climb_hang_shimmy_01_0.wav"}, {}, {},{}, }},
	{AnimationType::ClimbLeft, {std::chrono::milliseconds(0), 0,			{"climb_hang_shimmy_00_0.wav"}, {}, {}, {},}},
	{AnimationType::NeckSnap, {std::chrono::milliseconds(0), 0,				{"player_grab_finish_neck_0.wav"}, {}, {},{}, }},
	{AnimationType::HeavyBodyCollision, {std::chrono::milliseconds(0), 0,	{"body_zombie_collision_ground_heavy_03_0.wav"}, {}, {},{}, }},
	{AnimationType::BuggyIgnition, {std::chrono::milliseconds(0), 0,		{"buggy_ignition_start_0.wav"}, {}, {},{}, }},
	{AnimationType::OpenToolbox, {std::chrono::milliseconds(0), 0,			{"box_tools_opening_leg_0.wav"}, {}, {},{}, }},
	{AnimationType::OpenSmallChest, {std::chrono::milliseconds(0), 0,		{"oven_open_0.wav"}, {}, {},{}, }},
	{AnimationType::OpenLittleBasket, {std::chrono::milliseconds(0), 0,		{"open_wicker_basket_0.wav"}, {}, {},{}, }},
	{AnimationType::OpenBigBasket, {std::chrono::milliseconds(0), 0,		{"open_wicker_basket_0.wav"}, {}, {},{}, }},
	{AnimationType::OpenWardrobe, {std::chrono::milliseconds(0), 0,			{"wardrobe_c_opening_0.wav"}, {}, {},{}, }},
	{AnimationType::GrabbedByBiterRight, {std::chrono::milliseconds(0), 0,	{"zombie_walker_grab_bite_long_right_0.wav"}, {}, {},{}, true}},
	{AnimationType::GrabbedByBiterLeft, {std::chrono::milliseconds(0), 0,	{"zombie_walker_grab_bite_long_left_0.wav"}, {}, {}, {},true}},
	{AnimationType::BrokeOutBiterLeft, {std::chrono::milliseconds(0), 0,	{"zombie_walker_grab_bite_long_left_0.wav"}, {}, {}, {},true}},
	{AnimationType::GrabbedByBiterFront, {std::chrono::milliseconds(0), 0,	{"zombie_walker_grab_bite_long_right_0.wav"}, {}, {},{}, true}},
	{AnimationType::CommonClimbs, {std::chrono::milliseconds(0), 0,			{"climb_hang_shimmy_00_0.wav", "climb_hang_shimmy_01_0.wav", "climb_hang_shimmy_02_0.wav"}, {}, {},{}}},
};

#endif // SETTINGS_H