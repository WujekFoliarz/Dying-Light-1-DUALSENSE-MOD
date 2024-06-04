using DL1_Dualsense;
using System.Diagnostics;

internal class Program
{
    private static Game game = new Game();


    private static void Main(string[] args)
    {
        DualsenseControllerAPI controllerAPI = new DualsenseControllerAPI();
        bool gameRunning = true;
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
        bool uvSoundPlaying = false;
        bool lastDpadUP = false; // for flashlight
        bool lastR2btn = false; // for weapons
        bool wasZipline = false;

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
        Random rand = new Random();
        hapticCooldown.Start();
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

                        hp = game.getHP();
                        lastWeapon = weapon;
                        weapon = game.getWeaponType();

                        //Console.WriteLine(weapon);
                        if (weapon == 2058 || weapon >= 2024 && weapon <= 2029) // 1H sharp weapon (machete etc.)
                        {
                            controllerAPI.triggerThreshold = 120;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid_A;
                            controllerAPI.rightTriggerForces = [20, 255, 0, 0, 0, 0, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && !lastR2btn)
                            {
                                switch (rand.Next(0, 4))
                                {
                                    case 0:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Machete1);
                                        break;
                                    case 1:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Machete2);
                                        break;
                                    case 2:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Machete3);
                                        break;
                                    case 3:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Machete4);
                                        break;
                                }
                            }
                        }
                        else if (weapon == 1974 || weapon >= 1940 && weapon <= 1960) // 2H weapon
                        {
                            controllerAPI.triggerThreshold = 120;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid;
                            controllerAPI.rightTriggerForces = [0, 255, 0, 0, 0, 0, 0];

                            if(weapon == 1957 && hapticCooldown.ElapsedMilliseconds >= 500)
                            {
                                controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.GroundPound);
                                hapticCooldown.Restart();
                            }

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && !lastR2btn)
                            {
                                switch (rand.Next(0, 8))
                                {
                                    case 0:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Heavy1);
                                        break;
                                    case 1:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Heavy2);
                                        break;
                                    case 2:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Heavy3);
                                        break;
                                    case 3:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Heavy4);
                                        break;
                                    case 4:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Heavy5);
                                        break;
                                    case 5:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Heavy6);
                                        break;
                                    case 6:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Heavy7);
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
                                controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.CrossbowShot);
                            }
                        }
                        else if (weapon == 1897 || weapon >= 1861 && weapon <= 1897) // 1H blunt weapon (baseball bat etc.)
                        {
                            controllerAPI.triggerThreshold = 120;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid_A;
                            controllerAPI.rightTriggerForces = [70, 170, 120, 0, 0, 0, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && !lastR2btn)
                            {
                                switch (rand.Next(0, 6))
                                {
                                    case 0:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Blunt1);
                                        break;
                                    case 1:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Blunt2);
                                        break;
                                    case 2:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Blunt3);
                                        break;
                                    case 3:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Blunt4);
                                        break;
                                    case 4:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Blunt5);
                                        break;
                                    case 5:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Blunt6);
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
                                switch (rand.Next(0, 4))
                                {
                                    case 0:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Knife1);
                                        break;
                                    case 1:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Knife2);
                                        break;
                                    case 2:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Knife3);
                                        break;
                                    case 3:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Knife4);
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
                                switch (rand.Next(0, 4))
                                {
                                    case 0:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Fists1);
                                        break;
                                    case 1:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Fists2);
                                        break;
                                    case 2:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Fists3);
                                        break;
                                    case 3:
                                        controllerAPI.PlayHaptics(0, 0, rand.NextSingle(), rand.NextSingle(), HapticEffect.Fists4);
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
                                controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.Chainsaw);
                                hapticCooldown.Restart();
                            }
                        }
                        else if (weapon == 1282 && lastWeapon != 1283 || weapon == 1246) // Automatic rifle
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Pulse_B;
                            controllerAPI.rightTriggerForces = [10, 255, 120, 0, 0, 0, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30)
                            {
                                switch (rand.Next(0, 3))
                                {
                                    case 0:
                                        controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.RifleShot1);
                                        break;
                                    case 1:
                                        controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.RifleShot2);
                                        break;
                                    case 2:
                                        controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.RifleShot3);
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
                                controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.GunEmpty);
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
                                switch (rand.Next(0, 3))
                                {
                                    case 0:
                                        controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.SmgShot1);
                                        break;
                                    case 1:
                                        controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.SmgShot2);
                                        break;
                                    case 2:
                                        controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.SmgShot3);
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
                                controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.GunEmpty);
                                hapticCooldown.Restart();
                            }
                        }
                        else if (weapon == 757 || weapon >= 734 && weapon <= 759) // Bows
                        {
                            if (weapon == 757 && hapticCooldown.ElapsedMilliseconds >= 50)
                            {
                                controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.BowShot);
                            }
                            else
                            {
                                controllerAPI.triggerThreshold = 80;
                                controllerAPI.rightTriggerMode = TriggerModes.Rigid;
                                controllerAPI.rightTriggerForces = [0, 255, 0, 255, 255, 255, 0];

                                if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30 && !lastR2btn)
                                {
                                    controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.BowLoad);
                                    hapticCooldown.Restart();
                                }
                                else if (controllerAPI.state.R2 <= controllerAPI.triggerThreshold && lastR2btn)
                                {
                                    controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.BowReload);
                                }
                            }
                        }
                        else if (weapon == 1715 || weapon == 1690) // Shotguns
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Pulse_AB;
                            controllerAPI.rightTriggerForces = [10, 75, 82, 0, 0, 0, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30 && !lastR2btn)
                            {
                                controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.ShotgunShot);
                                hapticCooldown.Restart();
                            }
                        }
                        else if (weapon == 1715 || weapon == 1690) // Double-barrel shotgun
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Pulse_AB;
                            controllerAPI.rightTriggerForces = [255, 0, 255, 0, 0, 0, 0];

                            if (controllerAPI.state.R2 >= controllerAPI.triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30 && !lastR2btn)
                            {
                                controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.ShotgunShot);
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
                                controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.RevolverShot);
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
                                controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.RevolverShot);
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
                                controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.PistolShot);
                                hapticCooldown.Restart();
                            }
                        }
                        else if (weapon == 2454 && hapticCooldown.ElapsedMilliseconds >= 1000) // Item pickup
                        {
                            controllerAPI.PlayHaptics(0.1f, 0.1f, 1, 1, HapticEffect.PickupItem);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2403 && hapticCooldown.ElapsedMilliseconds >= 1000) // Dropkick start
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.Dropkick);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2404 && hapticCooldown.ElapsedMilliseconds >= 500) // Dropkick end
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.DropkickEnd);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 5546 && hapticCooldown.ElapsedMilliseconds >= 1500) // Open van
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.VanOpening);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2790 && hapticCooldown.ElapsedMilliseconds >= 150) // Watch
                        {
                            controllerAPI.PlayHaptics(0.15f, 0.15f, 1, 1, HapticEffect.WatchBeep);
                            hapticCooldown.Restart();
                        }
                        else if (weapon >= 2730 && weapon <= 2738) // Zipline
                        {
                            if (hapticCooldown.ElapsedMilliseconds >= 2500)
                            {
                                 controllerAPI.PlayHaptics(0.1f, 0.1f, 1, 1, HapticEffect.Zipline);
                                 hapticCooldown.Restart();
                                 wasZipline = true;
                            }
                        }
                        else if (weapon == 2488 && hapticCooldown.ElapsedMilliseconds >= 1000) // Bruteforce door
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 0, HapticEffect.GroundPound);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2625 && hapticCooldown.ElapsedMilliseconds >= 1000) // Finishing kick
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.KneeFinisher);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2341 && hapticCooldown.ElapsedMilliseconds >= 1000) // Knee finisher
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.KneeFinisher);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2340 && hapticCooldown.ElapsedMilliseconds >= 1000) // Elbow finisher
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.ElbowFinisher);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2342 && hapticCooldown.ElapsedMilliseconds >= 1000) // Neck grab finisher
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.NeckGrabFinisher);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2343 && hapticCooldown.ElapsedMilliseconds >= 1000) // Neck finisher
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.NeckFinisher);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 1324 && hapticCooldown.ElapsedMilliseconds >= 1000) // Open door
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 0, HapticEffect.HangOnEdge1);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2827 && hapticCooldown.ElapsedMilliseconds >= 1000 || weapon == 2829 && hapticCooldown.ElapsedMilliseconds >= 1000) // Open fridge
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.OpeningFridge);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 5540 && hapticCooldown.ElapsedMilliseconds >= 1000 || weapon == 2835 && hapticCooldown.ElapsedMilliseconds >= 1000) // Open closet
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.OpeningWoodenDoor);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2969 && hapticCooldown.ElapsedMilliseconds >= 1500 || weapon == 2967 && hapticCooldown.ElapsedMilliseconds >= 1500) // Open car
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.CarTrunkOpening);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2282 && hapticCooldown.ElapsedMilliseconds >= 2500) // Smearing zombie guts
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.Camouflage);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 660 && hapticCooldown.ElapsedMilliseconds >= 1000) // Using medkit
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.Medkit);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2460 && hapticCooldown.ElapsedMilliseconds >= 300 || weapon == 2457 && hapticCooldown.ElapsedMilliseconds >= 300 || weapon == 2607 && hapticCooldown.ElapsedMilliseconds >= 300) // Climb pipe right hand
                        {
                            controllerAPI.PlayHaptics(0, 0, 0, 1, HapticEffect.ClimbPipeRight);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2459 && hapticCooldown.ElapsedMilliseconds >= 300 || weapon == 2458 && hapticCooldown.ElapsedMilliseconds >= 300 || weapon == 2610 && hapticCooldown.ElapsedMilliseconds >= 300) // Climb pipe left hand
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 0, HapticEffect.ClimbPipeLeft);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2475 && hapticCooldown.ElapsedMilliseconds >= 500 || weapon == 2422 && hapticCooldown.ElapsedMilliseconds >= 500) // Slide down a pipe
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.Slide);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2473 && hapticCooldown.ElapsedMilliseconds >= 600) // Rotating left on a pipe
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 0, HapticEffect.Slide);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2474 && hapticCooldown.ElapsedMilliseconds >= 600) // Rotating right on a pipe
                        {
                            controllerAPI.PlayHaptics(0, 0, 0, 1, HapticEffect.Slide);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2408 && hapticCooldown.ElapsedMilliseconds >= 200 || weapon == 2406 && hapticCooldown.ElapsedMilliseconds >= 200) // Climbing on ladder left
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 0, HapticEffect.ClimbLadderLeft);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2409 && hapticCooldown.ElapsedMilliseconds >= 200 || weapon == 2407 && hapticCooldown.ElapsedMilliseconds >= 200) // Climbing on ladder right
                        {
                            controllerAPI.PlayHaptics(0, 0, 0, 1, HapticEffect.ClimbLadderRight);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2288 && hapticCooldown.ElapsedMilliseconds >= 1500) // Landed on car
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.LandingOnCar);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2738 && hapticCooldown.ElapsedMilliseconds >= 1500) // Reparing weapon
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.WeaponRepair);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2290 && hapticCooldown.ElapsedMilliseconds >= 1500) // Landed on trash
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.LandingOnTrash);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2284 && hapticCooldown.ElapsedMilliseconds >= 1500) // Rolling
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.SafetyRoll);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 1322 && hapticCooldown.ElapsedMilliseconds >= 2000) // Using remote control
                        {
                            controllerAPI.PlayHaptics(0.1f, 0.1f, 1, 1, HapticEffect.Remote);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2907 && hapticCooldown.ElapsedMilliseconds >= 4000) // Using remote control
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.OpeningChest);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2611 && hapticCooldown.ElapsedMilliseconds >= 300) // Sliding
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.Slide);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2048 && hapticCooldown.ElapsedMilliseconds >= 200) // Sliding
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.GrapplingHookStart);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2047 && hapticCooldown.ElapsedMilliseconds >= 800) // Sliding
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.GrapplingHookEnd);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2283 && hapticCooldown.ElapsedMilliseconds >= 1000 || weapon == 2329 && hapticCooldown.ElapsedMilliseconds >= 1000 || weapon == 2423 && hapticCooldown.ElapsedMilliseconds >= 1000) // Soft landing
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.LandOnDirt);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2256 && hapticCooldown.ElapsedMilliseconds >= 300 || weapon == 2257 && hapticCooldown.ElapsedMilliseconds >= 300) // Looting
                        {
                            controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.Looting);
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2603 && hapticCooldown.ElapsedMilliseconds >= 300) // Climb right
                        {
                            switch (rand.Next(0, 3))
                            {
                                case 0:
                                    controllerAPI.PlayHaptics(0, 0, 0, 1, HapticEffect.HangOnEdge1);
                                    break;
                                case 1:
                                    controllerAPI.PlayHaptics(0, 0, 0, 1, HapticEffect.HangOnEdge2);
                                    break;
                                case 2:
                                    controllerAPI.PlayHaptics(0, 0, 0, 1, HapticEffect.HangOnEdge3);
                                    break;
                            }
                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2599 && hapticCooldown.ElapsedMilliseconds >= 300) // Climb left
                        {
                            switch (rand.Next(0, 3))
                            {
                                case 0:
                                    controllerAPI.PlayHaptics(0, 0, 1, 0, HapticEffect.HangOnEdge1);
                                    break;
                                case 1:
                                    controllerAPI.PlayHaptics(0, 0, 1, 0, HapticEffect.HangOnEdge2);
                                    break;
                                case 2:
                                    controllerAPI.PlayHaptics(0, 0, 1, 0, HapticEffect.HangOnEdge3);
                                    break;
                            }
                            hapticCooldown.Restart();
                        }
                        else if (weapon >= 2500 && weapon <= 2560 && hapticCooldown.ElapsedMilliseconds >= 300) // Common climbs
                        {
                                switch (rand.Next(0, 3))
                                {
                                    case 0:
                                        controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.HangOnEdge1);
                                        break;
                                    case 1:
                                        controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.HangOnEdge2);
                                        break;
                                    case 2:
                                        controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.HangOnEdge3);
                                        break;
                                }

                            hapticCooldown.Restart();
                        }
                        else if (weapon == 2388 && hapticCooldown.ElapsedMilliseconds >= 1000) // Jump over enemies
                        {
                            switch (rand.Next(0, 3))
                            {
                                case 0:
                                    controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.Blunt1);
                                    break;
                                case 1:
                                    controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.Blunt3);
                                    break;
                                case 2:
                                    controllerAPI.PlayHaptics(0, 0, 1, 1, HapticEffect.Blunt6);
                                    break;
                            }
                            hapticCooldown.Restart();
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
                                controllerAPI.hapticFeedback.forceStop = true;
                            }
                        }

                        //Console.WriteLine(weapon);

                        if (game.isUVRecharging())
                        {
                            if (!flashlight && controllerAPI.state.DpadUp && hapticCooldown.ElapsedMilliseconds > 250 && !lastDpadUP)
                            {
                                controllerAPI.PlayHaptics(0, 0.05f, 0, 1, HapticEffect.FlashlightOn);
                                flashlight = true;
                                hapticCooldown.Restart();
                            }
                            else if (flashlight && controllerAPI.state.DpadUp && hapticCooldown.ElapsedMilliseconds > 250 && !lastDpadUP)
                            {
                                controllerAPI.PlayHaptics(0, 0.05f, 0, 1, HapticEffect.FlashlightOff);
                                flashlight = false;
                                hapticCooldown.Restart();
                            }

                            if (controllerAPI.state.L2Btn && hapticCooldown.ElapsedMilliseconds > 600)
                            {
                                controllerAPI.PlayHaptics(0, 0.05f, 0, 1, HapticEffect.UVFlashlightFailed);
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
                            if(weapon < 2000 || weapon > 2500)
                            {
                                if (!controllerAPI.state.L2Btn)
                                {
                                    uvWorking = false;
                                }

                                if (controllerAPI.state.L2Btn)
                                {
                                    if (uvWorking == false && hapticCooldown.ElapsedMilliseconds >= 150)
                                    {
                                        controllerAPI.PlayHaptics(0, 0.06f, 0, 1, HapticEffect.UVFlashlightStart);
                                        uvWorking = true;
                                        hapticCooldown.Restart();
                                    }
                                    else if (uvWorking == true && !controllerAPI.hapticFeedback.Playing && hapticCooldown.ElapsedMilliseconds >= 1300)
                                    {
                                        controllerAPI.PlayHaptics(0, 0.65f, 0, 1, HapticEffect.UVFlashlightWorking);
                                        hapticCooldown.Restart();
                                    }
                                }

                                controllerAPI.R = 60;
                                controllerAPI.G = 0;
                                controllerAPI.B = 255;
                            }
                        }
                        // Change RGB colors to white
                        else if (game.isFlashlightOn())
                        {
                            if (firstTime == true && hapticCooldown.ElapsedMilliseconds >= 200|| controllerAPI.state.DpadUp && hapticCooldown.ElapsedMilliseconds >= 200 && !lastDpadUP)
                            {
                                if(weapon < 2000)
                                {
                                    if (!flashlight)
                                    {
                                        controllerAPI.PlayHaptics(0, 0.05f, 0, 1, HapticEffect.FlashlightOn);
                                        flashlight = true;
                                    }
                                    else
                                    {
                                        controllerAPI.PlayHaptics(0, 0.05f, 0, 1, HapticEffect.FlashlightOff);
                                        flashlight = false;
                                    }
                                    hapticCooldown.Restart();

                                    firstTime = false;
                                    controllerAPI.R = 255;
                                    controllerAPI.G = 255;
                                    controllerAPI.B = 255;
                                }

                            }
                        }
                        else
                        {
                            firstTime = true;
                            // Change RGB colors according to HP
                            if (hp >= 175) { controllerAPI.R = 0; controllerAPI.G = 255; controllerAPI.B = 0; }
                            else if (hp < 175 && hp > 75) { controllerAPI.R = 255; controllerAPI.G = 255; controllerAPI.B = 0; }
                            else if (hp < 75 && hp != 0) { controllerAPI.R = 255; controllerAPI.G = 255; controllerAPI.B = 0; }
                            else { controllerAPI.R = 0; controllerAPI.G = 0; controllerAPI.B = 0; }
                        }

                    }
                    else
                    {
                        if (controllerAPI.state.LX >= 240 ||
                            controllerAPI.state.LX <= 10 || controllerAPI.state.LY >= 240 || controllerAPI.state.LY <= 10
                            || controllerAPI.state.DpadDown || controllerAPI.state.DpadLeft || controllerAPI.state.DpadRight || controllerAPI.state.DpadUp || controllerAPI.state.touchBtn)
                        {

                            if (hapticCooldown.ElapsedMilliseconds > 300)
                            {
                                controllerAPI.PlayHaptics(0.02f, 0.02f, 1, 1, HapticEffect.UIchangeSelection);
                                hapticCooldown.Restart();
                            }
                        }
                        else if (controllerAPI.state.cross)
                        {
                            if (hapticCooldown.ElapsedMilliseconds > 300)
                            {
                                controllerAPI.PlayHaptics(0.02f, 0.02f, 1, 1, HapticEffect.UIselect2);
                                hapticCooldown.Restart();
                            }
                        }
                        else if (controllerAPI.state.circle)
                        {
                            if (hapticCooldown.ElapsedMilliseconds > 300)
                            {
                                controllerAPI.PlayHaptics(0.02f, 0.02f, 1, 1, HapticEffect.UIselect1);
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
                catch
                {
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
