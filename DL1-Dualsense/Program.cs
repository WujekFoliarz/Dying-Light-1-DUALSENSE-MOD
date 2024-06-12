using DL1_Dualsense;
using System.Diagnostics;

internal class Program
{
    private static Game game = new Game();
    public static bool airDrop = false;
    public static bool meleeHit = false;

    private static void Main(string[] args)
    {
        DualsenseControllerAPI controllerAPI = new DualsenseControllerAPI();
        bool apiRunning = false;

        float hp = 0; // Player's health
        int weapon = 0;
        int lastWeapon = 0;
        int uvAnimationCycleR = 60;
        int uvAnimationCycleB = 255;
        bool uvAnimationSide = false; // false means increasing, true means decreasing
        int maxCycleValue = 255;
        int minCycleValue = 50; // The minimum value of R we want to reach
        int increment = 30;
        bool flashlight = false;
        bool uvWorking = false;
        bool lastDpadUP = false; // for flashlight
        bool lastR2btn = false; // for weapons
        bool wasZipline = false;
        float walkSpeed = 0;
        int Lfoot = 1;
        int Rfoot = 0;
        bool firstTimeAirdrop = true;

        if (!apiRunning)
        {
            controllerAPI.Start();
            apiRunning = true;

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                while (apiRunning)
                {
                    controllerAPI.emulatedControllerRefresh();
                }
            }).Start();
        }

        bool firstTime = true;
        Stopwatch hapticCooldown = new Stopwatch();
        Stopwatch footstepsCooldown = new Stopwatch();
        Random rand = new Random();
        hapticCooldown.Start();
        footstepsCooldown.Start();
        while (true)
        {
            Process[] process = Process.GetProcessesByName("DyingLightGame");
            if (process.Length == 0)
            {
                controllerAPI.hapticFeedback.initialized = false;
                Thread.Sleep(1000);
                controllerAPI.hapticFeedback.Dispose();
                apiRunning = false;
                controllerAPI.microphoneLED = true;
                controllerAPI.R = 0;
                controllerAPI.G = 0;
                controllerAPI.B = 0;
                controllerAPI.r_rotor = 0;
                controllerAPI.l_rotor = 0;
                controllerAPI.leftTriggerMode = TriggerModes.Rigid_B;
                controllerAPI.rightTriggerMode = TriggerModes.Rigid_B;
                controllerAPI.leftTriggerForces = [0, 0, 0, 0, 0, 0, 0];
                controllerAPI.rightTriggerForces = [0, 0, 0, 0, 0, 0, 0];
                controllerAPI.playerLED = 0;
                controllerAPI.emulatedControllerRefresh();
                controllerAPI.Close();
                Thread.Sleep(2000);
                Environment.Exit(0);
            }
            else
            {
                try
                {

                    if (game.isMicrophoneOn())
                    {
                        controllerAPI.microphoneLED = true;
                    }
                    else
                    {
                        controllerAPI.microphoneLED = false;
                    }

                    if (!game.isPlayerInMenu())
                    {
                        controllerAPI.useTouchpad = false;

                        hp = game.getHP();
                        lastWeapon = weapon;
                        weapon = game.getWeaponType();
                        walkSpeed = game.getPlayerWalkSpeed();

                        //Console.WriteLine(weapon);
                        if (weapon == 2058 || weapon >= 2024 && weapon <= 2029) // 1H sharp weapon (machete etc.)
                        {
                            controllerAPI.triggerThreshold = 120;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid_A;
                            controllerAPI.rightTriggerForces = [20, 255, 0, 0, 0, 0, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && !lastR2btn)
                            {
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                switch (rand.Next(0, 4))
                                {
                                    case 0:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Machete1);
                                        break;
                                    case 1:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Machete2);
                                        break;
                                    case 2:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Machete3);
                                        break;
                                    case 3:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Machete4);
                                        break;
                                }
                            }
                        }
                        else if (weapon == 1974 || weapon >= 1940 && weapon <= 1960) // 2H weapon
                        {
                            controllerAPI.triggerThreshold = 120;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid;
                            controllerAPI.rightTriggerForces = [0, 255, 0, 0, 0, 0, 0];

                            if (weapon == 1957 && hapticCooldown.ElapsedMilliseconds >= 500)
                            {
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.PlayHaptics(1, 1, HapticEffect.GroundPound);
                                hapticCooldown.Restart();
                            }

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && !lastR2btn)
                            {
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                switch (rand.Next(0, 8))
                                {
                                    case 0:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Heavy1);
                                        break;
                                    case 1:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Heavy2);
                                        break;
                                    case 2:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Heavy3);
                                        break;
                                    case 3:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Heavy4);
                                        break;
                                    case 4:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Heavy5);
                                        break;
                                    case 5:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Heavy6);
                                        break;
                                    case 6:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Heavy7);
                                        break;
                                }
                            }
                        }
                        else if (weapon == 886 || weapon == 850) // Crossbow
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid_A;
                            controllerAPI.rightTriggerForces = [55, 0, 75, 40, 90, 255, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && !lastR2btn)
                            {
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.PlayHaptics(1, 1, HapticEffect.CrossbowShot);
                            }
                        }
                        else if (weapon == 1897 || weapon >= 1861 && weapon <= 1897) // 1H blunt weapon (baseball bat etc.)
                        {
                            controllerAPI.triggerThreshold = 120;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid_A;
                            controllerAPI.rightTriggerForces = [70, 170, 120, 0, 0, 0, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && !lastR2btn)
                            {
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                switch (rand.Next(0, 6))
                                {
                                    case 0:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Blunt1);
                                        break;
                                    case 1:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Blunt2);
                                        break;
                                    case 2:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Blunt3);
                                        break;
                                    case 3:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Blunt4);
                                        break;
                                    case 4:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Blunt5);
                                        break;
                                    case 5:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Blunt6);
                                        break;
                                }
                            }
                        }
                        else if (weapon == 1184 || weapon >= 1152 && weapon <= 1165) // Knifes
                        {
                            controllerAPI.triggerThreshold = 120;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid_A;
                            controllerAPI.rightTriggerForces = [20, 1, 20, 0, 0, 0, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && !lastR2btn)
                            {
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                switch (rand.Next(0, 4))
                                {
                                    case 0:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Knife1);
                                        break;
                                    case 1:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Knife2);
                                        break;
                                    case 2:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Knife3);
                                        break;
                                    case 3:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Knife4);
                                        break;
                                }
                            }
                        }
                        else if (weapon <= 949 && weapon >= 933) // Fists
                        {
                            controllerAPI.triggerThreshold = 120;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid_A;
                            controllerAPI.rightTriggerForces = [1, 1, 0, 0, 0, 0, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && !lastR2btn)
                            {
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                switch (rand.Next(0, 4))
                                {
                                    case 0:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Fists1);
                                        break;
                                    case 1:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Fists2);
                                        break;
                                    case 2:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Fists3);
                                        break;
                                    case 3:
                                        controllerAPI.PlayHaptics(rand.NextSingle(), rand.NextSingle(), HapticEffect.Fists4);
                                        break;
                                }
                            }
                        }
                        else if (weapon == 830) // Chainsaw
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Pulse_B;
                            controllerAPI.rightTriggerForces = [35, 137, 65, 0, 0, 0, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 1500)
                            {
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.PlayHaptics(1, 1, HapticEffect.Chainsaw);
                                hapticCooldown.Restart();
                            }
                        }
                        else if (weapon == 1282 && lastWeapon != 1283 || weapon == 1246) // Automatic rifle
                        {
                            controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Pulse_B;
                            controllerAPI.rightTriggerForces = [10, 255, 120, 0, 0, 0, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30)
                            {
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                switch (rand.Next(0, 3))
                                {
                                    case 0:
                                        controllerAPI.PlayHaptics(1, 1, HapticEffect.RifleShot1);
                                        break;
                                    case 1:
                                        controllerAPI.PlayHaptics(1, 1, HapticEffect.RifleShot2);
                                        break;
                                    case 2:
                                        controllerAPI.PlayHaptics(1, 1, HapticEffect.RifleShot3);
                                        break;
                                }

                                hapticCooldown.Restart();
                            }
                        }
                        else if (weapon == 1283) // Automatic rifle (EMPTY)
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Pulse_AB;
                            controllerAPI.rightTriggerForces = [120, 0, 255, 0, 0, 0, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30 && !lastR2btn)
                            {
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.PlayHaptics(1, 1, HapticEffect.GunEmpty);
                                hapticCooldown.Restart();
                            }
                        }
                        else if (weapon == 2169 || weapon == 2079) // Submachine gun
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Pulse_B;
                            controllerAPI.rightTriggerForces = [15, 255, 120, 0, 0, 0, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold)
                            {
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                                switch (rand.Next(0, 3))
                                {
                                    case 0:
                                        controllerAPI.PlayHaptics(1, 1, HapticEffect.SmgShot1);
                                        break;
                                    case 1:
                                        controllerAPI.PlayHaptics(1, 1, HapticEffect.SmgShot2);
                                        break;
                                    case 2:
                                        controllerAPI.PlayHaptics(1, 1, HapticEffect.SmgShot3);
                                        break;
                                }

                                hapticCooldown.Restart();
                            }
                        }
                        else if (weapon == 2170 || weapon == 2080) // Submachine gun (EMPTY)
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Pulse_AB;
                            controllerAPI.rightTriggerForces = [120, 0, 255, 0, 0, 0, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30)
                            {
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.PlayHaptics(1, 1, HapticEffect.GunEmpty);
                                hapticCooldown.Restart();
                            }
                        }
                        else if (weapon == 757 || weapon >= 720 && weapon <= 759) // Bows
                        {
                            if(weapon != 741 && weapon != 743 && weapon != 742 && weapon != 744 && weapon != 745)
                            {
                                if (weapon == 757 && hapticCooldown.ElapsedMilliseconds >= 600 || weapon == 726 && hapticCooldown.ElapsedMilliseconds >= 600)
                                {
                                    controllerAPI.speakerVolume = SpeakerVolume.Off;
                                    controllerAPI.PlayHaptics(1, 1, HapticEffect.BowShot);
                                    hapticCooldown.Restart();
                                }
                                else
                                {
                                    controllerAPI.triggerThreshold = 80;
                                    controllerAPI.rightTriggerMode = TriggerModes.Rigid;
                                    controllerAPI.rightTriggerForces = [0, 255, 0, 255, 255, 255, 0];

                                    if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30 && !lastR2btn)
                                    {
                                        controllerAPI.speakerVolume = SpeakerVolume.Off;
                                        controllerAPI.PlayHaptics(1, 1, HapticEffect.BowLoad);
                                        hapticCooldown.Restart();
                                    }
                                    else if (controllerAPI.state.R2 < controllerAPI.triggerThreshold && lastR2btn)
                                    {
                                        controllerAPI.speakerVolume = SpeakerVolume.Off;
                                        controllerAPI.PlayHaptics(1, 1, HapticEffect.BowReload);
                                        hapticCooldown.Restart();
                                    }
                                }
                            }
                            else
                            {
                                controllerAPI.rightTriggerMode = TriggerModes.Rigid_B;
                                controllerAPI.rightTriggerForces = [0, 0, 0, 0, 0, 0, 0];
                            }
                        }
                        else if (weapon == 1715 || weapon == 1690) // Shotguns
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Pulse_AB;
                            controllerAPI.rightTriggerForces = [10, 75, 82, 0, 0, 0, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30 && !lastR2btn)
                            {
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.PlayHaptics(1, 1, HapticEffect.ShotgunShot);
                                hapticCooldown.Restart();
                            }
                        }
                        else if (weapon == 1715 || weapon == 1690 || weapon == 1724) // Double-barrel shotgun
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Pulse_AB;
                            controllerAPI.rightTriggerForces = [255, 0, 255, 0, 0, 0, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30 && !lastR2btn)
                            {
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.PlayHaptics(1, 1, HapticEffect.ShotgunShot);
                                hapticCooldown.Restart();
                            }
                        }
                        else if (weapon == 1589 || weapon == 1562) // Double-Action Revolver
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid_AB;
                            controllerAPI.rightTriggerForces = [255, 184, 255, 143, 71, 0, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30 && !lastR2btn)
                            {
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.PlayHaptics(1, 1, HapticEffect.RevolverShot);
                                hapticCooldown.Restart();
                            }
                        }
                        else if (weapon == 1811 || weapon == 1783 || weapon == 1812) // Single-Action Revolver
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid_AB;
                            controllerAPI.rightTriggerForces = [93, 184, 255, 143, 71, 0, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30 && !lastR2btn)
                            {
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.PlayHaptics(1, 1, HapticEffect.RevolverShot);
                                hapticCooldown.Restart();
                            }
                        }
                        else if (weapon == 703 || weapon == 667 || weapon == 704) // Pistol
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid_AB;
                            controllerAPI.rightTriggerForces = [93, 184, 255, 143, 71, 0, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30 && !lastR2btn)
                            {
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.PlayHaptics(1, 1, HapticEffect.PistolShot);
                                hapticCooldown.Restart();
                            }
                        }
                        else if (weapon == 2454 && hapticCooldown.ElapsedMilliseconds >= 1000) // Item pickup
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Very_Loud;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.PickupItem);
                            controllerAPI.PlaySpeaker(HapticEffect.PickupItem);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2403 && hapticCooldown.ElapsedMilliseconds >= 800) // Dropkick start
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.Dropkick);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2404 && hapticCooldown.ElapsedMilliseconds >= 1000) // Dropkick end
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.DropkickEnd);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 5546 && hapticCooldown.ElapsedMilliseconds >= 1500) // Open van
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.VanOpening);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2790 && hapticCooldown.ElapsedMilliseconds >= 150) // Watch
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.WatchBeep);
                            hapticCooldown.Restart();
                        }
                        else if (weapon >= 2730 && weapon <= 2738 && weapon != 2731) // Zipline
                        {
                            if (hapticCooldown.ElapsedMilliseconds >= 2500)
                            {
                                controllerAPI.speakerVolume = SpeakerVolume.Quiet;
                                controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                                controllerAPI.PlayHaptics(1, 1, HapticEffect.Zipline);
                                hapticCooldown.Restart();
                                wasZipline = true;
                            }
                        }
                        else if (weapon == 2731 && hapticCooldown.ElapsedMilliseconds >= 150) // Wallrun
                        {
                                controllerAPI.speakerVolume = SpeakerVolume.Quiet;
                                controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                                controllerAPI.PlayHaptics(1, 1, HapticEffect.HangOnEdge2);
                                hapticCooldown.Restart();
                        }
                        else if (weapon == 2488 && hapticCooldown.ElapsedMilliseconds >= 1000) // Bruteforce door
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 0, HapticEffect.GroundPound);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2625 && hapticCooldown.ElapsedMilliseconds >= 1000) // Finishing kick
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.KneeFinisher);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2341 && hapticCooldown.ElapsedMilliseconds >= 2000) // Knee finisher
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.KneeFinisher);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2340 && hapticCooldown.ElapsedMilliseconds >= 2000) // Elbow finisher
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.ElbowFinisher);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2342 && hapticCooldown.ElapsedMilliseconds >= 2000) // Neck grab finisher
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.NeckGrabFinisher);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2343 && hapticCooldown.ElapsedMilliseconds >= 2000) // Neck finisher
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.NeckFinisher);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 1324 && hapticCooldown.ElapsedMilliseconds >= 1000) // Open door
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 0, HapticEffect.HangOnEdge1);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2827 && hapticCooldown.ElapsedMilliseconds >= 1000 || weapon == 2829 && hapticCooldown.ElapsedMilliseconds >= 1000) // Open fridge
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.OpeningFridge);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 5540 && hapticCooldown.ElapsedMilliseconds >= 1000 || weapon == 2835 && hapticCooldown.ElapsedMilliseconds >= 1000) // Open closet
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.OpeningWoodenDoor);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2969 && hapticCooldown.ElapsedMilliseconds >= 1500 || weapon == 2967 && hapticCooldown.ElapsedMilliseconds >= 1500) // Open car
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.CarTrunkOpening);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2282 && hapticCooldown.ElapsedMilliseconds >= 2500) // Smearing zombie guts
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.Camouflage);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 660 && hapticCooldown.ElapsedMilliseconds >= 1000) // Using medkit
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.Medkit);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2460 && hapticCooldown.ElapsedMilliseconds >= 200 || weapon == 2457 && hapticCooldown.ElapsedMilliseconds >= 200 || weapon == 2607 && hapticCooldown.ElapsedMilliseconds >= 200) // Climb pipe right hand
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                            controllerAPI.PlayHaptics(0, 1, HapticEffect.ClimbPipeRight);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2459 && hapticCooldown.ElapsedMilliseconds >= 200 || weapon == 2458 && hapticCooldown.ElapsedMilliseconds >= 200 || weapon == 2610 && hapticCooldown.ElapsedMilliseconds >= 200) // Climb pipe left hand
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                            controllerAPI.PlayHaptics(1, 0, HapticEffect.ClimbPipeLeft);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2475 && hapticCooldown.ElapsedMilliseconds >= 500 || weapon == 2422 && hapticCooldown.ElapsedMilliseconds >= 1500) // Slide down a pipe
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.Slide);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2473 && hapticCooldown.ElapsedMilliseconds >= 600) // Rotating left on a pipe
                        {
                            //controllerAPI.speakerVolume = SpeakerVolume.Off;
                            //controllerAPI.PlayHaptics(1, 0, HapticEffect.Slide);
                            //hapticCooldown.Restart();
                        }
                        else if (weapon == 2474 && hapticCooldown.ElapsedMilliseconds >= 600) // Rotating right on a pipe
                        {
                            //controllerAPI.speakerVolume = SpeakerVolume.Off;
                            //controllerAPI.PlayHaptics(0, 1, HapticEffect.Slide);
                            //hapticCooldown.Restart();
                        }
                        else if (weapon == 2408 && hapticCooldown.ElapsedMilliseconds >= 200 || weapon == 2406 && hapticCooldown.ElapsedMilliseconds >= 200) // Climbing on ladder left
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                            controllerAPI.PlayHaptics(1, 0, HapticEffect.ClimbLadderLeft);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2409 && hapticCooldown.ElapsedMilliseconds >= 200 || weapon == 2407 && hapticCooldown.ElapsedMilliseconds >= 200) // Climbing on ladder right
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                            controllerAPI.PlayHaptics(0, 1, HapticEffect.ClimbLadderRight);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2288 && hapticCooldown.ElapsedMilliseconds >= 1500) // Landed on car
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.LandingOnCar);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2738 && hapticCooldown.ElapsedMilliseconds >= 1500) // Reparing weapon
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.WeaponRepair);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2290 && hapticCooldown.ElapsedMilliseconds >= 1500) // Landed on trash
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.LandingOnTrash);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2284 && hapticCooldown.ElapsedMilliseconds >= 1500) // Rolling
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.SafetyRoll);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 1322 && hapticCooldown.ElapsedMilliseconds >= 2000) // Using remote control
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Very_Loud;
                            controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.Remote);
                            controllerAPI.PlaySpeaker(HapticEffect.Remote);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2907 && hapticCooldown.ElapsedMilliseconds >= 4000) // Opening chest
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.OpeningChest);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2611 && hapticCooldown.ElapsedMilliseconds >= 300) // Sliding
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.Slide);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2048 && hapticCooldown.ElapsedMilliseconds >= 800) // Grappling hook start
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Moderate;
                            controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.GrapplingHookStart);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2047 && hapticCooldown.ElapsedMilliseconds >= 1000) // Grappling hook loop
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Moderate;
                            controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.GrapplingHookEnd);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2283 && hapticCooldown.ElapsedMilliseconds >= 1000 || weapon == 2329 && hapticCooldown.ElapsedMilliseconds >= 1000 || weapon == 2423 && hapticCooldown.ElapsedMilliseconds >= 1000) // Soft landing
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.LandOnDirt);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2256 && hapticCooldown.ElapsedMilliseconds >= 300 || weapon == 2257 && hapticCooldown.ElapsedMilliseconds >= 300) // Looting
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.Looting);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2603 && hapticCooldown.ElapsedMilliseconds >= 300) // Climb right
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            switch (rand.Next(0, 3))
                            {
                                case 0:
                                    controllerAPI.PlayHaptics(0, 1, HapticEffect.HangOnEdge1);
                                    break;
                                case 1:
                                    controllerAPI.PlayHaptics(0, 1, HapticEffect.HangOnEdge2);
                                    break;
                                case 2:
                                    controllerAPI.PlayHaptics(0, 1, HapticEffect.HangOnEdge3);
                                    break;
                            }
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2599 && hapticCooldown.ElapsedMilliseconds >= 300) // Climb left
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            switch (rand.Next(0, 3))
                            {
                                case 0:
                                    controllerAPI.PlayHaptics(1, 0, HapticEffect.HangOnEdge1);
                                    break;
                                case 1:
                                    controllerAPI.PlayHaptics(1, 0, HapticEffect.HangOnEdge2);
                                    break;
                                case 2:
                                    controllerAPI.PlayHaptics(1, 0, HapticEffect.HangOnEdge3);
                                    break;
                            }
                            hapticCooldown.Restart();
                        }
                        else if (weapon >= 2500 && weapon <= 2560 && hapticCooldown.ElapsedMilliseconds >= 1000) // Common climbs
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            switch (rand.Next(0, 3))
                            {
                                case 0:
                                    controllerAPI.PlayHaptics(1, 1, HapticEffect.HangOnEdge1);
                                    break;
                                case 1:
                                    controllerAPI.PlayHaptics(1, 1, HapticEffect.HangOnEdge2);
                                    break;
                                case 2:
                                    controllerAPI.PlayHaptics(1, 1, HapticEffect.HangOnEdge3);
                                    break;
                            }

                            hapticCooldown.Restart();
                        }
                        else if (weapon >= 2520 && weapon <= 2536 && hapticCooldown.ElapsedMilliseconds >= 150) // Climbing on a wall
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            switch (rand.Next(0, 3))
                            {
                                case 0:
                                    controllerAPI.PlayHaptics(1, 1, HapticEffect.HangOnEdge1);
                                    break;
                                case 1:
                                    controllerAPI.PlayHaptics(1, 1, HapticEffect.HangOnEdge2);
                                    break;
                                case 2:
                                    controllerAPI.PlayHaptics(1, 1, HapticEffect.HangOnEdge3);
                                    break;
                            }

                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2388 && hapticCooldown.ElapsedMilliseconds >= 1000) // Jump over enemies
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            switch (rand.Next(0, 3))
                            {
                                case 0:
                                    controllerAPI.PlayHaptics(1, 1, HapticEffect.Blunt1);
                                    break;
                                case 1:
                                    controllerAPI.PlayHaptics(1, 1, HapticEffect.Blunt3);
                                    break;
                                case 2:
                                    controllerAPI.PlayHaptics(1, 1, HapticEffect.Blunt6);
                                    break;
                            }
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2319 && hapticCooldown.ElapsedMilliseconds >= 2000) // neck snap from behind
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.NeckFinisher);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 1210 && hapticCooldown.ElapsedMilliseconds >= 2000) // neck snap from behind
                        {
                            controllerAPI.speakerVolume = SpeakerVolume.Off;
                            controllerAPI.PlayHaptics(1, 1, HapticEffect.HeavyBodyCollision);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2440 || weapon == 2438) // grabbed by biter right
                        {
                            if(hapticCooldown.ElapsedMilliseconds >= 2000)
                            {
                                controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.PlayHaptics(0, 1, HapticEffect.ZombieBiteLoopRight);
                                hapticCooldown.Restart();
                            }

                        }
                        else if (weapon == 2441) // grabbed by biter right
                        {
                            if (hapticCooldown.ElapsedMilliseconds >= 2000)
                            {
                                controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.PlayHaptics(1, 0, HapticEffect.ZombieBiteLoopleft);
                                hapticCooldown.Restart();
                            }

                        }
                        else if (weapon == 2328) // grabbed by biter left
                        {
                            if (hapticCooldown.ElapsedMilliseconds >= 2000)
                            {
                                controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.PlayHaptics(1, 0, HapticEffect.ZombieBiteLoopleft);
                                hapticCooldown.Restart();
                            }

                        }
                        else if (weapon == 2327) // broke out of biter grab left
                        {
                            if (hapticCooldown.ElapsedMilliseconds >= 2000)
                            {
                                controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.PlayHaptics(1, 0, HapticEffect.ZombieBiteBreak);
                                hapticCooldown.Restart();
                            }

                        }
                        else if (weapon == 2439) // grabbed by biter front
                        {
                            if (hapticCooldown.ElapsedMilliseconds >= 2000)
                            {
                                controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.PlayHaptics(1, 1, HapticEffect.ZombieBiteLoopRight);
                                hapticCooldown.Restart();
                            }

                        }
                        else
                        {
                            controllerAPI.triggerThreshold = 0;
                            controllerAPI.rightTriggerMode = TriggerModes.Off;
                            controllerAPI.rightTriggerForces = [0, 0, 0, 0, 0, 0, 0];
                        }

                        if (weapon <= 2730 || weapon >= 2738)
                        {
                            if (wasZipline)
                            {
                                wasZipline = false;
                                controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                            }
                        }

                        if (meleeHit && hapticCooldown.ElapsedMilliseconds >= 350)
                        {
                            controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                            controllerAPI.speakerVolume = SpeakerVolume.Moderate;

                            switch (rand.Next(0, 3))
                            {
                                case 0:
                                    controllerAPI.PlayHaptics(1, 1, HapticEffect.Hit1);
                                    break;
                                case 1:
                                    controllerAPI.PlayHaptics(1, 1, HapticEffect.Hit2);
                                    break;
                                case 2:
                                    controllerAPI.PlayHaptics(1, 1, HapticEffect.Hit3);
                                    break;
                            }

                            hapticCooldown.Restart();
                        }

                        if (airDrop)
                        {
                            if (firstTimeAirdrop)
                            {
                                controllerAPI.hapticFeedback.bufferedWaveProvider.ClearBuffer();
                                controllerAPI.speakerVolume = SpeakerVolume.Loud;
                                hapticCooldown.Restart();
                                controllerAPI.PlayHaptics(1, 1, HapticEffect.ElectronicBeep);
                                controllerAPI.PlaySpeaker(HapticEffect.ElectronicBeep);
                                firstTimeAirdrop = false;
                            }

                            if (!flashlight && controllerAPI.state.DpadUp && hapticCooldown.ElapsedMilliseconds > 250 && !lastDpadUP)
                            {
                                controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                                controllerAPI.speakerVolume = SpeakerVolume.Moderate;
                                controllerAPI.PlayHaptics(0, 1, HapticEffect.FlashlightOn);
                                controllerAPI.PlaySpeaker(HapticEffect.FlashlightOn);
                                flashlight = true;
                                hapticCooldown.Restart();
                            }
                            else if (flashlight && controllerAPI.state.DpadUp && hapticCooldown.ElapsedMilliseconds > 250 && !lastDpadUP)
                            {
                                controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                                controllerAPI.speakerVolume = SpeakerVolume.Moderate;
                                controllerAPI.PlayHaptics(0, 1, HapticEffect.FlashlightOff);
                                controllerAPI.PlaySpeaker(HapticEffect.FlashlightOff);
                                flashlight = false;
                                hapticCooldown.Restart();
                            }

                            if (uvAnimationSide == false && uvAnimationCycleB >= 255)
                            {
                                uvAnimationSide = true;
                            }
                            else if (uvAnimationSide == true && uvAnimationCycleB <= 0)
                            {
                                uvAnimationSide = false;
                            }

                            if (uvAnimationSide == false)
                            {
                                uvAnimationCycleB = Math.Min(uvAnimationCycleB + 10, 255);
                            }
                            else
                            {
                                uvAnimationCycleB = Math.Max(uvAnimationCycleB - 8, 0);
                            }

                            controllerAPI.R = 0;
                            controllerAPI.G = 0;
                            controllerAPI.B = uvAnimationCycleB;
                        }
                        else if (game.isUVRecharging())
                        {
                            if (!flashlight && controllerAPI.state.DpadUp && hapticCooldown.ElapsedMilliseconds > 250 && !lastDpadUP)
                            {
                                controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                                controllerAPI.speakerVolume = SpeakerVolume.Moderate;
                                controllerAPI.PlayHaptics(0, 1, HapticEffect.FlashlightOn);
                                controllerAPI.PlaySpeaker(HapticEffect.FlashlightOn);
                                flashlight = true;
                                hapticCooldown.Restart();
                            }
                            else if (flashlight && controllerAPI.state.DpadUp && hapticCooldown.ElapsedMilliseconds > 250 && !lastDpadUP)
                            {
                                controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                                controllerAPI.speakerVolume = SpeakerVolume.Moderate;
                                controllerAPI.PlayHaptics(0, 1, HapticEffect.FlashlightOff);
                                controllerAPI.PlaySpeaker(HapticEffect.FlashlightOff);
                                flashlight = false;
                                hapticCooldown.Restart();
                            }

                            if (controllerAPI.state.L2Btn && hapticCooldown.ElapsedMilliseconds > 600)
                            {
                                controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                                controllerAPI.speakerVolume = SpeakerVolume.Moderate;
                                controllerAPI.PlayHaptics(0, 1, HapticEffect.UVFlashlightFailed);
                                flashlight = true;
                                hapticCooldown.Restart();
                            }

                            if (uvAnimationSide == false && uvAnimationCycleB >= maxCycleValue)
                            {
                                uvAnimationSide = true;
                            }
                            else if (uvAnimationSide == true && uvAnimationCycleB <= minCycleValue)
                            {
                                uvAnimationSide = false;
                            }

                            if (uvAnimationSide == false)
                            {
                                uvAnimationCycleR = Math.Min(uvAnimationCycleR + increment, minCycleValue);
                                uvAnimationCycleB = Math.Min(uvAnimationCycleB + increment, maxCycleValue);
                            }
                            else
                            {
                                uvAnimationCycleR = Math.Max(uvAnimationCycleR - increment, minCycleValue);
                                uvAnimationCycleB = Math.Max(uvAnimationCycleB - increment, minCycleValue);
                            }

                            controllerAPI.R = uvAnimationCycleR;
                            controllerAPI.G = 0;
                            controllerAPI.B = uvAnimationCycleB;
                        }
                        // Change RGB colors to purple
                        else if (game.isUVOn())
                        {
                            controllerAPI.R = 60;
                            controllerAPI.G = 0;
                            controllerAPI.B = 255;
                        }
                        // Change RGB colors to white
                        else if (game.isFlashlightOn())
                        {
                            if (firstTime == true && hapticCooldown.ElapsedMilliseconds >= 200 || controllerAPI.state.DpadUp && hapticCooldown.ElapsedMilliseconds >= 200 && !lastDpadUP)
                            {
                                if (!flashlight)
                                {
                                    controllerAPI.speakerVolume = SpeakerVolume.Moderate;
                                    controllerAPI.PlayHaptics(0, 1, HapticEffect.FlashlightOn);
                                    controllerAPI.PlaySpeaker(HapticEffect.FlashlightOn);
                                    flashlight = true;
                                }
                                else
                                {
                                    controllerAPI.speakerVolume = SpeakerVolume.Moderate;
                                    controllerAPI.PlayHaptics(0, 1, HapticEffect.FlashlightOff);
                                    controllerAPI.PlaySpeaker(HapticEffect.FlashlightOn);
                                    flashlight = false;
                                }
                                hapticCooldown.Restart();

                                firstTime = false;
                            }

                            controllerAPI.R = 255;
                            controllerAPI.G = 255;
                            controllerAPI.B = 255;
                        }
                        else
                        {
                            firstTime = true;
                            firstTimeAirdrop = true;
                            // Change RGB colors according to HP
                            if (hp >= 175) { controllerAPI.R = 0; controllerAPI.G = 255; controllerAPI.B = 0; }
                            else if (hp < 175 && hp > 75) { controllerAPI.R = 255; controllerAPI.G = 255; controllerAPI.B = 0; }
                            else if (hp < 75 && hp != 0) { controllerAPI.R = 255; controllerAPI.G = 255; controllerAPI.B = 0; }
                            else { controllerAPI.R = 0; controllerAPI.G = 0; controllerAPI.B = 0; }
                        }

                        if(walkSpeed > 0 && walkSpeed <= 2.4)
                        {
                            if (Lfoot == 1)
                            {
                                Lfoot = 0;
                                Rfoot = 1;
                            }
                            else
                            {
                                Lfoot = 1;
                                Rfoot = 0;
                            }

                            if (footstepsCooldown.ElapsedMilliseconds >= 800 && hapticCooldown.ElapsedMilliseconds >= 500)
                            {
                                new Task(() => {
                                    controllerAPI.speakerVolume = SpeakerVolume.Off;
                                    controllerAPI.PlayHaptics(Lfoot, Rfoot, HapticEffect.Footstep);
                                }).Start();
                                footstepsCooldown.Restart();
                            }
                        }
                        else if (walkSpeed > 2.5 && walkSpeed < 5.7)
                        {

                            if (Lfoot == 1)
                            {
                                Lfoot = 0;
                                Rfoot = 1;
                            }
                            else
                            {
                                Lfoot = 1;
                                Rfoot = 0;
                            }

                            if (footstepsCooldown.ElapsedMilliseconds >= 350 && hapticCooldown.ElapsedMilliseconds >= 500)
                            {
                                new Task(() => {
                                    controllerAPI.speakerVolume = SpeakerVolume.Off;
                                    controllerAPI.PlayHaptics(Lfoot, Rfoot, HapticEffect.Footstep);
                                }).Start();
                                footstepsCooldown.Restart();
                            }


                        }
                        else if (walkSpeed >= 5.71)
                        {

                            if (Lfoot == 1)
                            {
                                Lfoot = 0;
                                Rfoot = 1;
                            }
                            else
                            {
                                Lfoot = 1;
                                Rfoot = 0;
                            }

                            if (footstepsCooldown.ElapsedMilliseconds >= 260 && hapticCooldown.ElapsedMilliseconds >= 1200)
                            {
                                new Task(() => {
                                    controllerAPI.speakerVolume = SpeakerVolume.Off;
                                    controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                                    controllerAPI.PlayHaptics(Lfoot, Rfoot, HapticEffect.StepOnWood);
                                }).Start();
                                footstepsCooldown.Restart();
                            }

                        }
                        else if (walkSpeed <= -1)
                        {

                            if (Lfoot == 1)
                            {
                                Lfoot = 0;
                                Rfoot = 1;
                            }
                            else
                            {
                                Lfoot = 1;
                                Rfoot = 0;
                            }

                            if (footstepsCooldown.ElapsedMilliseconds >= 300 && hapticCooldown.ElapsedMilliseconds >= 500)
                            {
                                new Task(() => {
                                    controllerAPI.speakerVolume = SpeakerVolume.Off;
                                    controllerAPI.PlayHaptics(Lfoot, Rfoot, HapticEffect.Footstep);
                                }).Start();
                                footstepsCooldown.Restart();
                            }

                        }

                        if (controllerAPI.state.R1 || controllerAPI.state.L2Btn && footstepsCooldown.ElapsedMilliseconds >= 500)
                        {
                            footstepsCooldown.Restart();
                        }

                        switch (game.getChaseLevel())
                        {
                            case 1:
                                controllerAPI.playerLED = PlayerID.PLAYER_1;
                                break;
                            case 2:
                                controllerAPI.playerLED = PlayerID.PLAYER_2;
                                break;
                            case 3:
                                controllerAPI.playerLED = PlayerID.PLAYER_3;
                                break;
                            case 4:
                                controllerAPI.playerLED = PlayerID.PLAYER_4;
                                break;
                            default:
                                controllerAPI.playerLED = 0;
                                break;
                        }
                    }
                    else
                    {
                        if (wasZipline)
                        {
                            controllerAPI.hapticFeedback.forceStop = true;
                            wasZipline = false;
                        }

                        if (controllerAPI.state.LX >= 240 ||
                            controllerAPI.state.LX <= 10 || controllerAPI.state.LY >= 240 || controllerAPI.state.LY <= 10
                            || controllerAPI.state.DpadDown || controllerAPI.state.DpadLeft || controllerAPI.state.DpadRight || controllerAPI.state.DpadUp || controllerAPI.state.touchBtn)
                        {
                            if (hapticCooldown.ElapsedMilliseconds > 300)
                            {
                                controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.PlayHaptics(1, 1, HapticEffect.FlashlightOn);
                                hapticCooldown.Restart();
                            }
                        }
                        else if (controllerAPI.state.cross)
                        {
                            if (hapticCooldown.ElapsedMilliseconds > 300)
                            {
                                controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.PlayHaptics(1, 1, HapticEffect.FlashlightOn);
                                hapticCooldown.Restart();
                            }
                        }
                        else if (controllerAPI.state.circle)
                        {
                            if (hapticCooldown.ElapsedMilliseconds > 300)
                            {
                                controllerAPI.hapticFeedback.bufferedWaveProviderHaptics.ClearBuffer();
                                controllerAPI.speakerVolume = SpeakerVolume.Off;
                                controllerAPI.PlayHaptics(1, 1, HapticEffect.FlashlightOff);
                                hapticCooldown.Restart();
                            }
                        }

                        controllerAPI.useTouchpad = true;
                        controllerAPI.R = 255;
                        controllerAPI.G = 102;
                        controllerAPI.B = 0;
                        controllerAPI.leftTriggerMode = TriggerModes.Rigid_B;
                        controllerAPI.rightTriggerMode = TriggerModes.Rigid_B;
                        controllerAPI.leftTriggerForces = [0, 0, 0, 0, 0, 0, 0];
                        controllerAPI.rightTriggerForces = [0, 0, 0, 0, 0, 0, 0];
                        controllerAPI.playerLED = 0;
                    }

                    lastDpadUP = controllerAPI.state.DpadUp;
                    if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold)
                        lastR2btn = true;
                    else
                        lastR2btn = false;

                    Thread.Sleep(25);
                }
                catch (Exception e)
                {
                    Console.WriteLine("\n" + e.Message + " " + e.Source);
                    break;
                }
            }
        }
    }

    public static void turnMicrophone(bool onORoff)
    {
        game.setMicrophone(onORoff);
    }
}
