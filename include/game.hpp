#ifndef GAME_H
#define GAME_H

enum class AnimationType {
    None,
    Lockpicking,
    OneHandedSharp,
    TwoHanded,
    WeaponThrow,
    Crossbow,
    CrossbowEmpty,
    OneHandedBlunt,
    Knife,
    Fists,
    Chainsaw,
    ChainsawEmpty,
    AutomaticRifle,
    AutomaticRifleEmpty,
    SubmachineGun,
    SubmachineGunEmpty,
    Bow,
    BowEmpty,
    Shotgun,
    ShotgunEmpty,
    DoubleBarrelShotgun,
    DoubleBarrelShotgunEmpty,
    DoubleActionRevolver,
    DoubleActionRevolverEmpty,
    SingleActionRevolver,
    SingleActionRevolverEmpty,
    Pistol,
    PistolEmpty,
    ItemPickup,
    DropkickStart,
    DropkickEnd,
    OpenVan,
    Watch,
    Zipline,
    Wallrun,
    BruteforceDoor,
    FinishingKick,
    KneeFinisher,
    ElbowFinisher,
    NeckGrabFinisher,
    NeckFinisher,
    OpenDoor,
    OpenFridge,
    OpenCloset,
    OpenCar,
    ZombieGuts,
    Medkit,
    ClimbPipeRight,
    ClimbPipeLeft,
    SlidePipe,
    RotateLeftPipe,
    RotateRightPipe,
    ClimbLadderLeft,
    ClimbLadderRight,
    LandOnCar,
    WeaponRepair,
    LandOnTrash,
    SafetyRoll,
    RemoteControl,
    OpenChest,
    Sliding,
    GrapplingHookStart,
    GrapplingHookLoop,
    SoftLanding,
    Looting,
    ClimbRight,
    ClimbLeft,
    CommonClimbs,
    WallClimbing,
    JumpOverEnemies,
    NeckSnap,
    HeavyBodyCollision,
    BuggyIgnition,
    OpenToolbox,
    OpenSmallChest,
    OpenLittleBasket,
    OpenBigBasket,
    OpenWardrobe,
    InBuggy,
    GrabbedByBiterRight,
    GrabbedByBiterLeft,
    GrabbedByBiterFront,
    BrokeOutBiterLeft,
    EmptyHanded,
    END,
};

AnimationType GetAnimationType(uint32_t anim) {
    if (anim == 2427) return AnimationType::Lockpicking;
    if (anim == 2058 || (anim >= 2024 && anim <= 2029)) return AnimationType::OneHandedSharp;
    if (anim == 1974 || (anim >= 1940 && anim <= 1960)) return AnimationType::TwoHanded;
    if (anim == 1891 || anim == 1968 || anim == 2053) return AnimationType::WeaponThrow;
    if (anim == 886 || anim == 850) return AnimationType::Crossbow;
    if (anim == 887 || anim == 851) return AnimationType::CrossbowEmpty;
    if (anim == 1897 || (anim >= 1861 && anim <= 1897)) return AnimationType::OneHandedBlunt;
    if (anim == 1184 || anim == 1158 || anim == 1161 || anim == 1159 || anim == 1660 || anim == 1661 || anim == 1662 || anim == 1665) return AnimationType::Knife;
    if (anim >= 933 && anim <= 949) return AnimationType::Fists;
    if (anim == 830) return AnimationType::Chainsaw;
    if (anim == 831) return AnimationType::ChainsawEmpty;
    if (anim == 1282 || anim == 1246) return AnimationType::AutomaticRifle;
    if (anim == 1283) return AnimationType::AutomaticRifleEmpty;
    if (anim == 2169 || anim == 2079) return AnimationType::SubmachineGun;
    if (anim == 2170 || anim == 2080) return AnimationType::SubmachineGunEmpty;
    if (anim == 741) return AnimationType::BowEmpty;
    if (anim == 759 || (anim >= 720 && anim <= 759)) return AnimationType::Bow;   
    if (anim == 1715 || anim == 1690) return AnimationType::Shotgun;
    if (anim == 1716 || anim == 1691) return AnimationType::ShotgunEmpty;
    if (anim == 1724 || anim == 1632) return AnimationType::DoubleBarrelShotgun;
    if (anim == 1725 || anim == 1633) return AnimationType::DoubleBarrelShotgunEmpty;
    if (anim == 1589 || anim == 1562) return AnimationType::DoubleActionRevolver;
    if (anim == 1590) return AnimationType::DoubleActionRevolverEmpty;
    if (anim == 1811 || anim == 1783) return AnimationType::SingleActionRevolver;
    if (anim == 1812 || anim == 1784) return AnimationType::SingleActionRevolverEmpty;
    if (anim == 667 || anim == 703) return AnimationType::Pistol;
    if (anim == 704 || anim == 668) return AnimationType::PistolEmpty;
    if (anim == 2454) return AnimationType::ItemPickup;
    if (anim == 2403) return AnimationType::DropkickStart;
    if (anim == 2404) return AnimationType::DropkickEnd;
    if (anim == 5546) return AnimationType::OpenVan;
    if (anim == 2790) return AnimationType::Watch;
    if (anim == 2737 || anim == 2736 || anim == 2735) return AnimationType::Zipline;
    if (anim == 2731) return AnimationType::Wallrun;
    if (anim == 2488) return AnimationType::BruteforceDoor;
    if (anim == 2625) return AnimationType::FinishingKick;
    if (anim == 2341) return AnimationType::KneeFinisher;
    if (anim == 2340) return AnimationType::ElbowFinisher;
    if (anim == 2342) return AnimationType::NeckGrabFinisher;
    if (anim == 2343) return AnimationType::NeckFinisher;
    if (anim == 1324) return AnimationType::OpenDoor;
    if (anim == 2827 || anim == 2829) return AnimationType::OpenFridge;
    if (anim == 5540 || anim == 2835) return AnimationType::OpenCloset;
    if (anim == 2969 || anim == 2967) return AnimationType::OpenCar;
    if (anim == 2282) return AnimationType::ZombieGuts;
    if (anim == 660) return AnimationType::Medkit;
    if (anim == 2460 || anim == 2457 || anim == 2607) return AnimationType::ClimbPipeRight;
    if (anim == 2459 || anim == 2458 || anim == 2610) return AnimationType::ClimbPipeLeft;
    if (anim == 2475 || anim == 2422) return AnimationType::SlidePipe;
    if (anim == 2473) return AnimationType::RotateLeftPipe;
    if (anim == 2474) return AnimationType::RotateRightPipe;
    if (anim == 2408 || anim == 2406) return AnimationType::ClimbLadderLeft;
    if (anim == 2409 || anim == 2407) return AnimationType::ClimbLadderRight;
    if (anim == 2288) return AnimationType::LandOnCar;
    if (anim == 2738) return AnimationType::WeaponRepair;
    if (anim == 2290) return AnimationType::LandOnTrash;
    if (anim == 2284) return AnimationType::SafetyRoll;
    if (anim == 1322) return AnimationType::RemoteControl;
    if (anim == 2907) return AnimationType::OpenChest;
    if (anim == 2611) return AnimationType::Sliding;
    if (anim == 2048) return AnimationType::GrapplingHookStart;
    if (anim == 2047) return AnimationType::GrapplingHookLoop;
    if (anim == 2283 || anim == 2329 || anim == 2423) return AnimationType::SoftLanding;
    if (anim == 2256 || anim == 2257) return AnimationType::Looting;
    if (anim == 2603) return AnimationType::ClimbRight;
    if (anim == 2599) return AnimationType::ClimbLeft;
    if (anim >= 2507 && anim <= 2560) return AnimationType::CommonClimbs;
    if (anim >= 2520 && anim <= 2536) return AnimationType::WallClimbing;
    if (anim == 2388) return AnimationType::JumpOverEnemies;
    if (anim == 2319) return AnimationType::NeckSnap;
    if (anim == 1210) return AnimationType::HeavyBodyCollision;
    if (anim == 2716) return AnimationType::BuggyIgnition;
    if (anim == 2979) return AnimationType::OpenToolbox;
    if (anim == 2833) return AnimationType::OpenSmallChest;
    if (anim == 17) return AnimationType::OpenLittleBasket;
    if (anim == 19) return AnimationType::OpenBigBasket;
    if (anim == 5537) return AnimationType::OpenWardrobe;
    if (anim == 2693 || anim == 2694) return AnimationType::InBuggy;
    if (anim == 2440 || anim == 2438 || anim == 2439) return AnimationType::GrabbedByBiterRight;
    if (anim == 2328) return AnimationType::GrabbedByBiterLeft;
    if (anim == 2327) return AnimationType::BrokeOutBiterLeft;
    if (anim == 2623 || anim == 2373 || anim == 2379 || anim == 2378) return AnimationType::EmptyHanded;
    return AnimationType::None;
}

#endif // GAME_H