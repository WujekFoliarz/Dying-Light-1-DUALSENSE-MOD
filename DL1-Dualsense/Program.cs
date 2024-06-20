using DL1_Dualsense;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using System.Diagnostics;
using Wujek_Dualsense_API;

internal class Program
{
    private static Game game = new Game();
    private static Dualsense dualsense = new Dualsense(0);
    public static bool airDrop = false;
    public static bool meleeHit = false;
    private static ViGEmClient client = new ViGEmClient();
    private static IDualShock4Controller dualshock4 = client.CreateDualShock4Controller();
    private static int triggerThreshold = 0;

    private static void Main(string[] args)
    {
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
        float Lfoot = 1;
        float Rfoot = 0;
        bool firstTimeAirdrop = true;

        dualsense.Start();

        if(dualsense.ConnectionType == ConnectionType.USB)
            dualsense.SetVibrationType(Vibrations.VibrationType.Haptic_Feedback);
        else if (dualsense.ConnectionType == ConnectionType.BT)
            dualsense.SetVibrationType(Vibrations.VibrationType.Standard_Rumble);

        dualshock4.Connect();
        dualshock4.FeedbackReceived += Dualshock4_FeedbackReceived;

        new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = true;
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            while (true)
            {
                dualshock4.SetButtonState(DualShock4Button.Cross, dualsense.ButtonState.cross);
                dualshock4.SetButtonState(DualShock4Button.Circle, dualsense.ButtonState.circle);
                dualshock4.SetButtonState(DualShock4Button.Triangle, dualsense.ButtonState.triangle);
                dualshock4.SetButtonState(DualShock4Button.Square, dualsense.ButtonState.square);

                DualShock4DPadDirection direction = DualShock4DPadDirection.None;
                if (dualsense.ButtonState.DpadDown) { direction = DualShock4DPadDirection.South; }
                else if (dualsense.ButtonState.DpadUp) { direction = DualShock4DPadDirection.North; }
                else if (dualsense.ButtonState.DpadLeft) { direction = DualShock4DPadDirection.West; }
                else if (dualsense.ButtonState.DpadRight) { direction = DualShock4DPadDirection.East; }
                dualshock4.SetDPadDirection(direction);

                dualshock4.SetAxisValue(DualShock4Axis.LeftThumbX, (byte)dualsense.ButtonState.LX);
                dualshock4.SetAxisValue(DualShock4Axis.LeftThumbY, (byte)dualsense.ButtonState.LY);
                dualshock4.SetAxisValue(DualShock4Axis.RightThumbX, (byte)dualsense.ButtonState.RX);
                dualshock4.SetAxisValue(DualShock4Axis.RightThumbY, (byte)dualsense.ButtonState.RY);
                dualshock4.SetButtonState(DualShock4Button.ThumbLeft, dualsense.ButtonState.L3);
                dualshock4.SetButtonState(DualShock4Button.ThumbRight, dualsense.ButtonState.R3);

                if ((byte)dualsense.ButtonState.L2 >= triggerThreshold)
                    dualshock4.LeftTrigger = (byte)dualsense.ButtonState.L2;
                else
                    dualshock4.LeftTrigger = (byte)0;

                if ((byte)dualsense.ButtonState.R2 >= triggerThreshold)
                    dualshock4.RightTrigger = (byte)dualsense.ButtonState.R2;
                else
                    dualshock4.RightTrigger = (byte)0;

                dualshock4.SetButtonState(DualShock4SpecialButton.Touchpad, dualsense.ButtonState.touchBtn);
                dualshock4.SetButtonState(DualShock4Button.Share, dualsense.ButtonState.share);
                dualshock4.SetButtonState(DualShock4Button.Options, dualsense.ButtonState.options);
                dualshock4.SetButtonState(DualShock4Button.ShoulderLeft, dualsense.ButtonState.L1);
                dualshock4.SetButtonState(DualShock4Button.ShoulderRight, dualsense.ButtonState.R1);
                dualshock4.SetButtonState(DualShock4SpecialButton.Ps, dualsense.ButtonState.ps);

                Thread.Sleep(1);
            }

        }).Start();

        bool firstTime = true;
        Stopwatch hapticCooldown = new Stopwatch();
        Stopwatch footstepsCooldown = new Stopwatch();
        Stopwatch lightbarAnimationCooldown = new Stopwatch();
        Random rand = new Random();
        hapticCooldown.Start();
        footstepsCooldown.Start();
        lightbarAnimationCooldown.Start();

        new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = true;
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            while (true)
            {
                Process[] process = Process.GetProcessesByName("DyingLightGame");
                if (process.Length == 0)
                {
                    dualsense.Dispose();
                    Thread.Sleep(2000);
                    Environment.Exit(0);
                }
                else
                {
                    try
                    {

                        if (game.isMicrophoneOn())
                        {
                            dualsense.SetMicrophoneLED(LED.MicrophoneLED.ON);
                        }
                        else
                        {
                            dualsense.SetMicrophoneLED(LED.MicrophoneLED.OFF);
                        }

                        if (!game.isPlayerInMenu())
                        {
                            hp = game.getHP();
                            lastWeapon = weapon;
                            weapon = game.getWeaponType();
                            walkSpeed = game.getPlayerWalkSpeed();

                            //Console.WriteLine(weapon);
                            if (weapon == 2058 || weapon >= 2024 && weapon <= 2029) // 1H sharp weapon (machete etc.)
                            {
                                triggerThreshold = 120;
                                dualsense.SetRightTrigger(TriggerType.TriggerModes.Rigid_A, 20, 255, 0, 0, 0, 0, 0);

                                if (dualsense.ButtonState.R2 >= triggerThreshold && !lastR2btn)
                                {
                                    switch (rand.Next(0, 4))
                                    {
                                        case 0:
                                            dualsense.PlayHaptics(HapticEffect.Machete1, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 1:
                                            dualsense.PlayHaptics(HapticEffect.Machete2, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 2:
                                            dualsense.PlayHaptics(HapticEffect.Machete3, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 3:
                                            dualsense.PlayHaptics(HapticEffect.Machete4, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                    }
                                }
                            }
                            else if (weapon == 1974 || weapon >= 1940 && weapon <= 1960) // 2H weapon
                            {
                                triggerThreshold = 120;
                                dualsense.SetRightTrigger(TriggerType.TriggerModes.Rigid, 0, 255, 0, 0, 0, 0, 0);

                                if (weapon == 1957 && hapticCooldown.ElapsedMilliseconds >= 500)
                                {
                                    dualsense.PlayHaptics(HapticEffect.GroundPound, 0.0f, 1.0f, 1.0f, true);
                                    hapticCooldown.Restart();
                                }

                                if (dualsense.ButtonState.R2 >= triggerThreshold && !lastR2btn)
                                {

                                    switch (rand.Next(0, 8))
                                    {
                                        case 0:
                                            dualsense.PlayHaptics(HapticEffect.Heavy1, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 1:
                                            dualsense.PlayHaptics(HapticEffect.Heavy2, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 2:
                                            dualsense.PlayHaptics(HapticEffect.Heavy3, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 3:
                                            dualsense.PlayHaptics(HapticEffect.Heavy4, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 4:
                                            dualsense.PlayHaptics(HapticEffect.Heavy5, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 5:
                                            dualsense.PlayHaptics(HapticEffect.Heavy6, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 6:
                                            dualsense.PlayHaptics(HapticEffect.Heavy7, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                    }
                                }
                            }
                            else if (weapon == 1891 || weapon == 2053 || weapon == 1181) // Weapon throwing
                            {
                                triggerThreshold = 120;
                                dualsense.SetRightTrigger(TriggerType.TriggerModes.Rigid, 0, 255, 0, 0, 0, 0, 0);
                            }
                            else if (weapon == 886 || weapon == 850) // Crossbow
                            {
                                triggerThreshold = 255;
                                dualsense.SetRightTrigger(TriggerType.TriggerModes.Rigid_A, 55, 0, 75, 40, 90, 255, 0);

                                if (dualsense.ButtonState.R2 >= triggerThreshold && !lastR2btn)
                                {

                                    dualsense.PlayHaptics(HapticEffect.CrossbowShot, 0.0f, 1.0f, 1.0f, true);
                                }
                            }
                            else if (weapon == 1897 || weapon >= 1861 && weapon <= 1897) // 1H blunt weapon (baseball bat etc.)
                            {
                                triggerThreshold = 120;
                                dualsense.SetRightTrigger(TriggerType.TriggerModes.Rigid_A, 70, 170, 120, 0, 0, 0, 0);

                                if (dualsense.ButtonState.R2 >= triggerThreshold && !lastR2btn)
                                {

                                    switch (rand.Next(0, 6))
                                    {
                                        case 0:
                                            dualsense.PlayHaptics(HapticEffect.Blunt1, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 1:
                                            dualsense.PlayHaptics(HapticEffect.Blunt2, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 2:
                                            dualsense.PlayHaptics(HapticEffect.Blunt3, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 3:
                                            dualsense.PlayHaptics(HapticEffect.Blunt4, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 4:
                                            dualsense.PlayHaptics(HapticEffect.Blunt5, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 5:
                                            dualsense.PlayHaptics(HapticEffect.Blunt6, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                    }
                                }
                            }
                            else if (weapon == 1184 || weapon >= 1152 && weapon <= 1165) // Knifes
                            {
                                triggerThreshold = 120;
                                dualsense.SetRightTrigger(TriggerType.TriggerModes.Rigid_A, 20, 1, 20, 0, 0, 0, 0);

                                if (dualsense.ButtonState.R2 >= triggerThreshold && !lastR2btn)
                                {

                                    switch (rand.Next(0, 4))
                                    {
                                        case 0:
                                            dualsense.PlayHaptics(HapticEffect.Knife1, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 1:
                                            dualsense.PlayHaptics(HapticEffect.Knife2, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 2:
                                            dualsense.PlayHaptics(HapticEffect.Knife3, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 3:
                                            dualsense.PlayHaptics(HapticEffect.Knife4, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                    }
                                }
                            }
                            else if (weapon <= 949 && weapon >= 933) // Fists
                            {
                                triggerThreshold = 120;
                                dualsense.SetRightTrigger(TriggerType.TriggerModes.Rigid_A, 1, 1, 0, 0, 0, 0, 0);

                                if (dualsense.ButtonState.R2 >= triggerThreshold && !lastR2btn)
                                {

                                    switch (rand.Next(0, 4))
                                    {
                                        case 0:
                                            dualsense.PlayHaptics(HapticEffect.Fists1, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 1:
                                            dualsense.PlayHaptics(HapticEffect.Fists2, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 2:
                                            dualsense.PlayHaptics(HapticEffect.Fists3, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                        case 3:
                                            dualsense.PlayHaptics(HapticEffect.Fists4, 0.0f, rand.NextSingle(), rand.NextSingle(), true);
                                            break;
                                    }
                                }
                            }
                            else if (weapon == 830) // Chainsaw
                            {
                                triggerThreshold = 255;
                                dualsense.SetRightTrigger(TriggerType.TriggerModes.Pulse_B, 35, 137, 65, 0, 0, 0, 0);

                                if (dualsense.ButtonState.R2 >= triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 1500)
                                {

                                    dualsense.PlayHaptics(HapticEffect.Chainsaw, 0.0f, 1.0f, 1.0f, true);
                                    hapticCooldown.Restart();
                                }
                            }
                            else if (weapon == 1282 && lastWeapon != 1283 || weapon == 1246) // Automatic rifle
                            {

                                triggerThreshold = 255;
                                dualsense.SetRightTrigger(TriggerType.TriggerModes.Pulse_B, 10, 255, 120, 0, 0, 0, 0);

                                if (dualsense.ButtonState.R2 >= triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30)
                                {

                                    switch (rand.Next(0, 3))
                                    {
                                        case 0:
                                            dualsense.PlayHaptics(HapticEffect.RifleShot1, 0.0f, 1.0f, 1.0f, true);
                                            break;
                                        case 1:
                                            dualsense.PlayHaptics(HapticEffect.RifleShot2, 0.0f, 1.0f, 1.0f, true);
                                            break;
                                        case 2:
                                            dualsense.PlayHaptics(HapticEffect.RifleShot3, 0.0f, 1.0f, 1.0f, true);
                                            break;
                                    }

                                    hapticCooldown.Restart();
                                }
                            }
                            else if (weapon == 1283) // Automatic rifle (EMPTY)
                            {
                                triggerThreshold = 255;
                                dualsense.SetRightTrigger(TriggerType.TriggerModes.Pulse_AB, 120, 0, 255, 0, 0, 0, 0);

                                if (dualsense.ButtonState.R2 >= triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30 && !lastR2btn)
                                {

                                    dualsense.PlayHaptics(HapticEffect.GunEmpty, 0.0f, 1.0f, 1.0f, true);
                                    hapticCooldown.Restart();
                                }
                            }
                            else if (weapon == 2169 || weapon == 2079) // Submachine gun
                            {
                                triggerThreshold = 255;
                                dualsense.SetRightTrigger(TriggerType.TriggerModes.Pulse_B, 15, 255, 120, 0, 0, 0, 0);

                                if (dualsense.ButtonState.R2 >= triggerThreshold)
                                {


                                    switch (rand.Next(0, 3))
                                    {
                                        case 0:
                                            dualsense.PlayHaptics(HapticEffect.SmgShot1, 0.0f, 1.0f, 1.0f, true);
                                            break;
                                        case 1:
                                            dualsense.PlayHaptics(HapticEffect.SmgShot2, 0.0f, 1.0f, 1.0f, true);
                                            break;
                                        case 2:
                                            dualsense.PlayHaptics(HapticEffect.SmgShot3, 0.0f, 1.0f, 1.0f, true);
                                            break;
                                    }

                                    hapticCooldown.Restart();
                                }
                            }
                            else if (weapon == 2170 || weapon == 2080) // Submachine gun (EMPTY)
                            {
                                triggerThreshold = 255;
                                dualsense.SetRightTrigger(TriggerType.TriggerModes.Pulse_AB, 120, 0, 255, 0, 0, 0, 0);

                                if (dualsense.ButtonState.R2 >= triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30)
                                {

                                    dualsense.PlayHaptics(HapticEffect.GunEmpty, 0.0f, 1.0f, 1.0f, true);
                                    hapticCooldown.Restart();
                                }
                            }
                            else if (weapon == 757 || weapon >= 720 && weapon <= 759) // Bows
                            {
                                if (weapon != 741 && weapon != 743 && weapon != 742 && weapon != 744 && weapon != 745)
                                {
                                    if (weapon == 757 && hapticCooldown.ElapsedMilliseconds >= 600 || weapon == 726 && hapticCooldown.ElapsedMilliseconds >= 600)
                                    {

                                        dualsense.PlayHaptics(HapticEffect.BowShot, 0.0f, 1.0f, 1.0f, true);
                                        hapticCooldown.Restart();
                                    }
                                    else
                                    {
                                        triggerThreshold = 80;
                                        dualsense.SetRightTrigger(TriggerType.TriggerModes.Rigid, 0, 255, 0, 255, 255, 255, 0);

                                        if (dualsense.ButtonState.R2 >= triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30 && !lastR2btn)
                                        {

                                            dualsense.PlayHaptics(HapticEffect.BowLoad, 0.0f, 1.0f, 1.0f, true);
                                            hapticCooldown.Restart();
                                        }
                                        else if (dualsense.ButtonState.R2 < triggerThreshold && lastR2btn)
                                        {

                                            dualsense.PlayHaptics(HapticEffect.BowReload, 0.0f, 1.0f, 1.0f, true);
                                            hapticCooldown.Restart();
                                        }
                                    }
                                }
                                else
                                {
                                    dualsense.SetRightTrigger(TriggerType.TriggerModes.Rigid_B, 0, 0, 0, 0, 0, 0, 0);
                                }
                            }
                            else if (weapon == 1715 || weapon == 1690) // Shotguns
                            {
                                triggerThreshold = 255;
                                dualsense.SetRightTrigger(TriggerType.TriggerModes.Pulse_AB, 10, 75, 82, 0, 0, 0, 0);

                                if (dualsense.ButtonState.R2 >= triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30 && !lastR2btn)
                                {

                                    dualsense.PlayHaptics(HapticEffect.ShotgunShot, 0.0f, 1.0f, 1.0f, true);
                                    hapticCooldown.Restart();
                                }
                            }
                            else if (weapon == 1715 || weapon == 1690 || weapon == 1724) // Double-barrel shotgun
                            {
                                triggerThreshold = 255;
                                dualsense.SetRightTrigger(TriggerType.TriggerModes.Pulse_AB, 255, 0, 255, 0, 0, 0, 0);

                                if (dualsense.ButtonState.R2 >= triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30 && !lastR2btn)
                                {

                                    dualsense.PlayHaptics(HapticEffect.ShotgunShot, 0.0f, 1.0f, 1.0f, true);
                                    hapticCooldown.Restart();
                                }
                            }
                            else if (weapon == 1589 || weapon == 1562) // Double-Action Revolver
                            {
                                triggerThreshold = 255;
                                dualsense.SetRightTrigger(TriggerType.TriggerModes.Pulse_AB, 255, 184, 255, 143, 71, 0, 0);

                                if (dualsense.ButtonState.R2 >= triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30 && !lastR2btn)
                                {

                                    dualsense.PlayHaptics(HapticEffect.RevolverShot, 0.0f, 1.0f, 1.0f, true);
                                    hapticCooldown.Restart();
                                }
                            }
                            else if (weapon == 1811 || weapon == 1783 || weapon == 1812) // Single-Action Revolver
                            {
                                triggerThreshold = 255;
                                dualsense.SetRightTrigger(TriggerType.TriggerModes.Pulse_AB, 93, 184, 255, 143, 71, 0, 0);

                                if (dualsense.ButtonState.R2 >= triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30 && !lastR2btn)
                                {

                                    dualsense.PlayHaptics(HapticEffect.RevolverShot, 0.0f, 1.0f, 1.0f, true);
                                    hapticCooldown.Restart();
                                }
                            }
                            else if (weapon == 703 || weapon == 667 || weapon == 704) // Pistol
                            {
                                triggerThreshold = 255;
                                dualsense.SetRightTrigger(TriggerType.TriggerModes.Rigid_AB, 93, 184, 255, 143, 71, 0, 0);

                                if (dualsense.ButtonState.R2 >= triggerThreshold && hapticCooldown.ElapsedMilliseconds >= 30 && !lastR2btn)
                                {

                                    dualsense.PlayHaptics(HapticEffect.PistolShot, 0.0f, 1.0f, 1.0f, true);
                                    hapticCooldown.Restart();
                                }
                            }
                            else if (weapon == 2454 && hapticCooldown.ElapsedMilliseconds >= 1000) // Item pickup
                            {

                                dualsense.PlayHaptics(HapticEffect.PickupItem, 1.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2403 && hapticCooldown.ElapsedMilliseconds >= 800) // Dropkick start
                            {

                                dualsense.PlayHaptics(HapticEffect.Dropkick, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2404 && hapticCooldown.ElapsedMilliseconds >= 1000) // Dropkick end
                            {

                                dualsense.PlayHaptics(HapticEffect.DropkickEnd, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 5546 && hapticCooldown.ElapsedMilliseconds >= 1500) // Open van
                            {

                                dualsense.PlayHaptics(HapticEffect.VanOpening, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2790 && hapticCooldown.ElapsedMilliseconds >= 1000) // Watch
                            {
                                dualsense.SetLightbar(25, 25, 25);
                                dualsense.PlayHaptics(HapticEffect.WatchBeep, 1.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon >= 2730 && weapon <= 2738 && weapon != 2731) // Zipline
                            {
                                if (hapticCooldown.ElapsedMilliseconds >= 2500)
                                {
                                    dualsense.PlayHaptics(HapticEffect.Zipline, 0.0f, 1.0f, 1.0f, true);
                                    hapticCooldown.Restart();
                                    wasZipline = true;
                                }
                            }
                            else if (weapon == 2731 && hapticCooldown.ElapsedMilliseconds >= 150) // Wallrun
                            {


                                dualsense.PlayHaptics(HapticEffect.HangOnEdge2, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2488 && hapticCooldown.ElapsedMilliseconds >= 1000) // Bruteforce door
                            {
                                dualsense.PlayHaptics(HapticEffect.GroundPound, 0.0f, 0.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2625 && hapticCooldown.ElapsedMilliseconds >= 1000) // Finishing kick
                            {

                                dualsense.PlayHaptics(HapticEffect.KneeFinisher, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2341 && hapticCooldown.ElapsedMilliseconds >= 2000) // Knee finisher
                            {

                                dualsense.PlayHaptics(HapticEffect.KneeFinisher, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2340 && hapticCooldown.ElapsedMilliseconds >= 2000) // Elbow finisher
                            {

                                dualsense.PlayHaptics(HapticEffect.ElbowFinisher, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2342 && hapticCooldown.ElapsedMilliseconds >= 2000) // Neck grab finisher
                            {

                                dualsense.PlayHaptics(HapticEffect.NeckGrabFinisher, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2343 && hapticCooldown.ElapsedMilliseconds >= 2000) // Neck finisher
                            {

                                dualsense.PlayHaptics(HapticEffect.NeckFinisher, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 1324 && hapticCooldown.ElapsedMilliseconds >= 1000) // Open door
                            {
                                dualsense.PlayHaptics(HapticEffect.HangOnEdge1, 0.0f, 1.0f, 0.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2827 && hapticCooldown.ElapsedMilliseconds >= 1000 || weapon == 2829 && hapticCooldown.ElapsedMilliseconds >= 1000) // Open fridge
                            {

                                dualsense.PlayHaptics(HapticEffect.OpeningFridge, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 5540 && hapticCooldown.ElapsedMilliseconds >= 1000 || weapon == 2835 && hapticCooldown.ElapsedMilliseconds >= 1000) // Open closet
                            {

                                dualsense.PlayHaptics(HapticEffect.OpeningWoodenDoor, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2969 && hapticCooldown.ElapsedMilliseconds >= 1500 || weapon == 2967 && hapticCooldown.ElapsedMilliseconds >= 1500) // Open car
                            {

                                dualsense.PlayHaptics(HapticEffect.CarTrunkOpening, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2282 && hapticCooldown.ElapsedMilliseconds >= 2500) // Smearing zombie guts
                            {

                                dualsense.PlayHaptics(HapticEffect.Camouflage, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 660 && hapticCooldown.ElapsedMilliseconds >= 1000) // Using medkit
                            {

                                dualsense.PlayHaptics(HapticEffect.Medkit, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2460 && hapticCooldown.ElapsedMilliseconds >= 200 || weapon == 2457 && hapticCooldown.ElapsedMilliseconds >= 200 || weapon == 2607 && hapticCooldown.ElapsedMilliseconds >= 200) // Climb pipe right hand
                            {

                                dualsense.PlayHaptics(HapticEffect.ClimbLadderRight, 0.0f, 0.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2459 && hapticCooldown.ElapsedMilliseconds >= 200 || weapon == 2458 && hapticCooldown.ElapsedMilliseconds >= 200 || weapon == 2610 && hapticCooldown.ElapsedMilliseconds >= 200) // Climb pipe left hand
                            {
                                dualsense.PlayHaptics(HapticEffect.ClimbLadderLeft, 0.0f, 1.0f, 0.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2475 && hapticCooldown.ElapsedMilliseconds >= 500 || weapon == 2422 && hapticCooldown.ElapsedMilliseconds >= 1500) // Slide down a pipe
                            {

                                dualsense.PlayHaptics(HapticEffect.Slide, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2473 && hapticCooldown.ElapsedMilliseconds >= 600) // Rotating left on a pipe
                            {
                                //
                                //controllerAPI.PlayHaptics(1, 0, HapticEffect.Slide);
                                //hapticCooldown.Restart();
                            }
                            else if (weapon == 2474 && hapticCooldown.ElapsedMilliseconds >= 600) // Rotating right on a pipe
                            {
                                //
                                //controllerAPI.PlayHaptics(0, 1, HapticEffect.Slide);
                                //hapticCooldown.Restart();
                            }
                            else if (weapon == 2408 && hapticCooldown.ElapsedMilliseconds >= 200 || weapon == 2406 && hapticCooldown.ElapsedMilliseconds >= 200) // Climbing on ladder left
                            {
                                dualsense.PlayHaptics(HapticEffect.ClimbLadderLeft, 0.0f, 1.0f, 0.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2409 && hapticCooldown.ElapsedMilliseconds >= 200 || weapon == 2407 && hapticCooldown.ElapsedMilliseconds >= 200) // Climbing on ladder right
                            {
                                dualsense.PlayHaptics(HapticEffect.ClimbLadderLeft, 0.0f, 0.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2288 && hapticCooldown.ElapsedMilliseconds >= 1500) // Landed on car
                            {

                                dualsense.PlayHaptics(HapticEffect.LandingOnCar, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2738 && hapticCooldown.ElapsedMilliseconds >= 1500) // Reparing weapon
                            {

                                dualsense.PlayHaptics(HapticEffect.WeaponRepair, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2290 && hapticCooldown.ElapsedMilliseconds >= 1500) // Landed on trash
                            {

                                dualsense.PlayHaptics(HapticEffect.LandingOnTrash, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2284 && hapticCooldown.ElapsedMilliseconds >= 1500) // Rolling
                            {


                                dualsense.PlayHaptics(HapticEffect.SafetyRoll, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 1322 && hapticCooldown.ElapsedMilliseconds >= 2000) // Using remote control
                            {


                                dualsense.PlayHaptics(HapticEffect.Remote, 1.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2907 && hapticCooldown.ElapsedMilliseconds >= 4000) // Opening chest
                            {

                                dualsense.PlayHaptics(HapticEffect.OpeningChest, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2611 && hapticCooldown.ElapsedMilliseconds >= 300) // Sliding
                            {


                                dualsense.PlayHaptics(HapticEffect.Slide, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2048 && hapticCooldown.ElapsedMilliseconds >= 800) // Grappling hook start
                            {


                                dualsense.PlayHaptics(HapticEffect.GrapplingHookStart, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2047 && hapticCooldown.ElapsedMilliseconds >= 1000) // Grappling hook loop
                            {


                                dualsense.PlayHaptics(HapticEffect.GrapplingHookEnd, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2283 && hapticCooldown.ElapsedMilliseconds >= 1000 || weapon == 2329 && hapticCooldown.ElapsedMilliseconds >= 1000 || weapon == 2423 && hapticCooldown.ElapsedMilliseconds >= 1000) // Soft landing
                            {

                                dualsense.PlayHaptics(HapticEffect.LandOnDirt, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2256 && hapticCooldown.ElapsedMilliseconds >= 300 || weapon == 2257 && hapticCooldown.ElapsedMilliseconds >= 300) // Looting
                            {


                                dualsense.PlayHaptics(HapticEffect.Looting, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2603 && hapticCooldown.ElapsedMilliseconds >= 300) // Climb right
                            {

                                switch (rand.Next(0, 3))
                                {
                                    case 0:
                                        dualsense.PlayHaptics(HapticEffect.HangOnEdge1, 0.0f, 0.0f, 1.0f, true);
                                        break;
                                    case 1:
                                        dualsense.PlayHaptics(HapticEffect.HangOnEdge2, 0.0f, 0.0f, 1.0f, true);
                                        break;
                                    case 2:
                                        dualsense.PlayHaptics(HapticEffect.HangOnEdge3, 0.0f, 0.0f, 1.0f, true);
                                        break;
                                }
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2599 && hapticCooldown.ElapsedMilliseconds >= 300) // Climb left
                            {

                                switch (rand.Next(0, 3))
                                {
                                    case 0:
                                        dualsense.PlayHaptics(HapticEffect.HangOnEdge1, 0.0f, 1.0f, 0.0f, true);
                                        break;
                                    case 1:
                                        dualsense.PlayHaptics(HapticEffect.HangOnEdge2, 0.0f, 1.0f, 0.0f, true);
                                        break;
                                    case 2:
                                        dualsense.PlayHaptics(HapticEffect.HangOnEdge3, 0.0f, 1.0f, 0.0f, true);
                                        break;
                                }
                                hapticCooldown.Restart();
                            }
                            else if (weapon >= 2500 && weapon <= 2560 && hapticCooldown.ElapsedMilliseconds >= 1000) // Common climbs
                            {

                                switch (rand.Next(0, 3))
                                {
                                    case 0:
                                        dualsense.PlayHaptics(HapticEffect.HangOnEdge1, 0.0f, 1.0f, 1.0f, true);
                                        break;
                                    case 1:
                                        dualsense.PlayHaptics(HapticEffect.HangOnEdge2, 0.0f, 1.0f, 1.0f, true);
                                        break;
                                    case 2:
                                        dualsense.PlayHaptics(HapticEffect.HangOnEdge3, 0.0f, 1.0f, 1.0f, true);
                                        break;
                                }

                                hapticCooldown.Restart();
                            }
                            else if (weapon >= 2520 && weapon <= 2536 && hapticCooldown.ElapsedMilliseconds >= 150) // Climbing on a wall
                            {

                                switch (rand.Next(0, 3))
                                {
                                    case 0:
                                        dualsense.PlayHaptics(HapticEffect.HangOnEdge1, 0.0f, 1.0f, 1.0f, true);
                                        break;
                                    case 1:
                                        dualsense.PlayHaptics(HapticEffect.HangOnEdge2, 0.0f, 1.0f, 1.0f, true);
                                        break;
                                    case 2:
                                        dualsense.PlayHaptics(HapticEffect.HangOnEdge3, 0.0f, 1.0f, 1.0f, true);
                                        break;
                                }

                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2388 && hapticCooldown.ElapsedMilliseconds >= 1000) // Jump over enemies
                            {

                                switch (rand.Next(0, 3))
                                {
                                    case 0:
                                        dualsense.PlayHaptics(HapticEffect.Blunt1, 0.0f, 1.0f, 1.0f, true);
                                        break;
                                    case 1:
                                        dualsense.PlayHaptics(HapticEffect.Blunt3, 0.0f, 1.0f, 1.0f, true);
                                        break;
                                    case 2:
                                        dualsense.PlayHaptics(HapticEffect.Blunt6, 0.0f, 1.0f, 1.0f, true);
                                        break;
                                }
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2319 && hapticCooldown.ElapsedMilliseconds >= 2000) // neck snap from behind
                            {

                                dualsense.PlayHaptics(HapticEffect.NeckFinisher, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 1210 && hapticCooldown.ElapsedMilliseconds >= 2000) // neck snap from behind
                            {

                                dualsense.PlayHaptics(HapticEffect.HeavyBodyCollision, 0.0f, 1.0f, 1.0f, true);
                                hapticCooldown.Restart();
                            }
                            else if (weapon == 2440 || weapon == 2438) // grabbed by biter right
                            {
                                if (hapticCooldown.ElapsedMilliseconds >= 2000)
                                {
                                    dualsense.PlayHaptics(HapticEffect.ZombieBiteLoopleft, 0.0f, 0.0f, 1.0f, true);
                                    hapticCooldown.Restart();
                                }

                            }
                            else if (weapon == 2441) // grabbed by biter right
                            {
                                if (hapticCooldown.ElapsedMilliseconds >= 2000)
                                {
                                    dualsense.PlayHaptics(HapticEffect.ZombieBiteLoopleft, 0.0f, 0.0f, 1.0f, true);
                                    hapticCooldown.Restart();
                                }

                            }
                            else if (weapon == 2328) // grabbed by biter left
                            {
                                if (hapticCooldown.ElapsedMilliseconds >= 2000)
                                {
                                    dualsense.PlayHaptics(HapticEffect.ZombieBiteLoopleft, 0.0f, 1.0f, 0.0f, true);
                                    hapticCooldown.Restart();
                                }

                            }
                            else if (weapon == 2327) // broke out of biter grab left
                            {
                                if (hapticCooldown.ElapsedMilliseconds >= 2000)
                                {
                                    dualsense.PlayHaptics(HapticEffect.ZombieBiteLoopleft, 0.0f, 1.0f, 0.0f, true);
                                    hapticCooldown.Restart();
                                }

                            }
                            else if (weapon == 2439) // grabbed by biter front
                            {
                                if (hapticCooldown.ElapsedMilliseconds >= 2000)
                                {
                                    dualsense.PlayHaptics(HapticEffect.ZombieBiteLoopRight, 0.0f, 1.0f, 1.0f, true);
                                    hapticCooldown.Restart();
                                }

                            }
                            else
                            {
                                triggerThreshold = 0;
                                dualsense.SetLeftTrigger(TriggerType.TriggerModes.Off, 0, 0, 0, 0, 0, 0, 0);
                                dualsense.SetRightTrigger(TriggerType.TriggerModes.Off, 0, 0, 0, 0, 0, 0, 0);
                            }

                            if (weapon <= 2730 || weapon >= 2738)
                            {
                                if (wasZipline)
                                {
                                    wasZipline = false;

                                }
                            }

                            if (meleeHit && hapticCooldown.ElapsedMilliseconds >= 350)
                            {


                                switch (rand.Next(0, 3))
                                {
                                    case 0:
                                        dualsense.PlayHaptics(HapticEffect.Hit1, 0.0f, 1.0f, 1.0f, true);
                                        break;
                                    case 1:
                                        dualsense.PlayHaptics(HapticEffect.Hit2, 0.0f, 1.0f, 1.0f, true);
                                        break;
                                    case 2:
                                        dualsense.PlayHaptics(HapticEffect.Hit3, 0.0f, 1.0f, 1.0f, true);
                                        break;
                                }

                                hapticCooldown.Restart();
                            }

                            if (airDrop)
                            {
                                if (firstTimeAirdrop)
                                {
                                    hapticCooldown.Restart();
                                    dualsense.PlayHaptics(HapticEffect.ElectronicBeep, 1.0f, 1.0f, 1.0f, true);
                                    firstTimeAirdrop = false;
                                }

                                if (!flashlight && dualsense.ButtonState.DpadUp && hapticCooldown.ElapsedMilliseconds > 250 && !lastDpadUP)
                                {

                                    dualsense.PlayHaptics(HapticEffect.FlashlightOn, 1.0f, 0.0f, 1.0f, true);
                                    flashlight = true;
                                    hapticCooldown.Restart();
                                }
                                else if (flashlight && dualsense.ButtonState.DpadUp && hapticCooldown.ElapsedMilliseconds > 250 && !lastDpadUP)
                                {


                                    dualsense.PlayHaptics(HapticEffect.FlashlightOff, 1.0f, 0.0f, 1.0f, true);
                                    flashlight = false;
                                    hapticCooldown.Restart();
                                }

                                if (uvAnimationSide == false && lightbarAnimationCooldown.ElapsedMilliseconds >= 350)
                                {
                                    uvAnimationSide = true;
                                    dualsense.SetLightbarTransition(0, 0, 255, 10, 25);
                                    lightbarAnimationCooldown.Restart();
                                }
                                else if (uvAnimationSide == true && lightbarAnimationCooldown.ElapsedMilliseconds >= 350)
                                {
                                    uvAnimationSide = false;
                                    dualsense.SetLightbarTransition(0, 0, 0, 10, 25);
                                    lightbarAnimationCooldown.Restart();
                                }
                            }
                            else if (game.isUVRecharging())
                            {
                                if (!flashlight && dualsense.ButtonState.DpadUp && hapticCooldown.ElapsedMilliseconds > 250 && !lastDpadUP)
                                {
                                    dualsense.PlayHaptics(HapticEffect.FlashlightOn, 1.0f, 0.0f, 1.0f, true);
                                    flashlight = true;
                                    hapticCooldown.Restart();
                                }
                                else if (flashlight && dualsense.ButtonState.DpadUp && hapticCooldown.ElapsedMilliseconds > 250 && !lastDpadUP)
                                {
                                    dualsense.PlayHaptics(HapticEffect.FlashlightOff, 1.0f, 0.0f, 1.0f, true);
                                    flashlight = false;
                                    hapticCooldown.Restart();
                                }

                                if (dualsense.ButtonState.L2Btn && hapticCooldown.ElapsedMilliseconds > 600)
                                {
                                    dualsense.PlayHaptics(HapticEffect.UVFlashlightFailed, 0.5f, 0.0f, 1.0f, true);
                                    flashlight = true;
                                    hapticCooldown.Restart();
                                }

                                if (uvAnimationSide == false && lightbarAnimationCooldown.ElapsedMilliseconds >= 350)
                                {
                                    dualsense.SetLightbarTransition(200, 0, 10, 5, 20);
                                    uvAnimationSide = true;
                                    lightbarAnimationCooldown.Restart();
                                }
                                else if (uvAnimationSide == true && lightbarAnimationCooldown.ElapsedMilliseconds >= 350)
                                {
                                    dualsense.SetLightbarTransition(60, 0, 255, 5, 20);
                                    uvAnimationSide = false;
                                    lightbarAnimationCooldown.Restart();
                                }
                            }
                            // Change RGB colors to purple
                            else if (game.isUVOn())
                            {
                                if (!dualsense.ButtonState.L2Btn)
                                {
                                    uvWorking = false;
                                }

                                if (dualsense.ButtonState.L2Btn)
                                {
                                    if (uvWorking == false && hapticCooldown.ElapsedMilliseconds >= 1200)
                                    {
                                        dualsense.PlayHaptics(HapticEffect.UVFlashlightStart, 1.0f, 0.0f, 1.0f, true);
                                        uvWorking = true;
                                    }
                                    else if (uvWorking == true && hapticCooldown.ElapsedMilliseconds >= 1300)
                                    {
                                        dualsense.PlayHaptics(HapticEffect.UVFlashlightWorking, 0.5f, 0.0f, 1.0f, true);
                                        hapticCooldown.Restart();
                                    }
                                }

                                dualsense.SetLightbar(60, 0, 255);
                            }
                            // Change RGB colors to white
                            else if (game.isFlashlightOn())
                            {
                                if (firstTime == true && hapticCooldown.ElapsedMilliseconds >= 200 || dualsense.ButtonState.DpadUp && hapticCooldown.ElapsedMilliseconds >= 200 && !lastDpadUP)
                                {
                                    if (!flashlight)
                                    {
                                        dualsense.PlayHaptics(HapticEffect.FlashlightOn, 1.0f, 0.0f, 1.0f, true);
                                        flashlight = true;
                                    }
                                    else
                                    {
                                        dualsense.PlayHaptics(HapticEffect.FlashlightOff, 1.0f, 0.0f, 1.0f, true);
                                        flashlight = false;
                                    }
                                    hapticCooldown.Restart();

                                    firstTime = false;
                                }

                                dualsense.SetLightbar(255, 255, 255);
                            }
                            else
                            {
                                firstTime = true;
                                firstTimeAirdrop = true;
                                // Change RGB colors according to HP
                                if (hp >= 175) { dualsense.SetLightbarTransition(0, 255, 0, 5, 10); }
                                else if (hp < 175 && hp > 75) { dualsense.SetLightbarTransition(255, 255, 0, 5, 10); }
                                else if (hp < 75 && hp != 0) { dualsense.SetLightbarTransition(255, 0, 0, 5, 10); }
                                else { dualsense.SetLightbarTransition(0, 0, 0, 5, 10); }
                            }

                            if (!game.isUVOn())
                            {
                                if (walkSpeed > 0 && walkSpeed <= 2.4)
                                {
                                    if (footstepsCooldown.ElapsedMilliseconds >= 800 && hapticCooldown.ElapsedMilliseconds >= 500)
                                    {
                                        if (Lfoot == 1.0f)
                                        {
                                            Lfoot = 0.0f;
                                            Rfoot = 1.0f;
                                        }
                                        else
                                        {
                                            Lfoot = 1.0f;
                                            Rfoot = 0.0f;
                                        }
                                        dualsense.PlayHaptics(HapticEffect.Footstep, 0.0f, Lfoot, Rfoot, true);
                                        footstepsCooldown.Restart();
                                    }
                                }
                                else if (walkSpeed > 2.5 && walkSpeed < 5.7)
                                {
                                    if (footstepsCooldown.ElapsedMilliseconds >= 350 && hapticCooldown.ElapsedMilliseconds >= 500)
                                    {
                                        if (Lfoot == 1.0f)
                                        {
                                            Lfoot = 0.0f;
                                            Rfoot = 1.0f;
                                        }
                                        else
                                        {
                                            Lfoot = 1.0f;
                                            Rfoot = 0.0f;
                                        }
                                        dualsense.PlayHaptics(HapticEffect.Footstep, 0.0f, Lfoot, Rfoot, true);
                                        footstepsCooldown.Restart();
                                    }


                                }
                                else if (walkSpeed >= 5.71)
                                {
                                    if (footstepsCooldown.ElapsedMilliseconds >= 260 && hapticCooldown.ElapsedMilliseconds >= 1200)
                                    {
                                        if (Lfoot == 1.0f)
                                        {
                                            Lfoot = 0.0f;
                                            Rfoot = 1.0f;
                                        }
                                        else
                                        {
                                            Lfoot = 1.0f;
                                            Rfoot = 0.0f;
                                        }

                                        dualsense.PlayHaptics(HapticEffect.StepOnWood, 0.0f, Lfoot, Rfoot, true);
                                        footstepsCooldown.Restart();
                                    }

                                }
                                else if (walkSpeed <= -1)
                                {


                                    if (footstepsCooldown.ElapsedMilliseconds >= 300 && hapticCooldown.ElapsedMilliseconds >= 500)
                                    {
                                        if (Lfoot == 1.0f)
                                        {
                                            Lfoot = 0.0f;
                                            Rfoot = 1.0f;
                                        }
                                        else
                                        {
                                            Lfoot = 1.0f;
                                            Rfoot = 0.0f;
                                        }

                                        dualsense.PlayHaptics(HapticEffect.Footstep, 0.0f, Lfoot, Rfoot, true);
                                        footstepsCooldown.Restart();
                                    }

                                }
                            }
                            

                            if (dualsense.ButtonState.R1 || dualsense.ButtonState.L2Btn && footstepsCooldown.ElapsedMilliseconds >= 500)
                            {
                                footstepsCooldown.Restart();
                            }

                            switch (game.getChaseLevel())
                            {
                                case 1:
                                    dualsense.SetPlayerLED(LED.PlayerLED.PLAYER_1);
                                    break;
                                case 2:
                                    dualsense.SetPlayerLED(LED.PlayerLED.PLAYER_2);
                                    break;
                                case 3:
                                    dualsense.SetPlayerLED(LED.PlayerLED.PLAYER_3);
                                    break;
                                case 4:
                                    dualsense.SetPlayerLED(LED.PlayerLED.PLAYER_4);
                                    break;
                                default:
                                    dualsense.SetPlayerLED(LED.PlayerLED.OFF);
                                    break;
                            }
                        }
                        else
                        {
                            if (wasZipline)
                            {
                                dualsense.PlayHaptics(HapticEffect.UIselect1, 0.0f, 0.0f, 0.0f, true);
                                wasZipline = false;
                            }

                            if (dualsense.ButtonState.LX >= 240 ||
                                dualsense.ButtonState.LX <= 10 || dualsense.ButtonState.LY >= 240 || dualsense.ButtonState.LY <= 10
                                || dualsense.ButtonState.DpadDown || dualsense.ButtonState.DpadLeft || dualsense.ButtonState.DpadRight || dualsense.ButtonState.DpadUp || dualsense.ButtonState.touchBtn)
                            {
                                if (hapticCooldown.ElapsedMilliseconds > 300)
                                {
                                    dualsense.PlayHaptics(HapticEffect.UIchangeSelection, 0.0f, 1.0f, 1.0f, true);
                                    hapticCooldown.Restart();
                                }
                            }
                            else if (dualsense.ButtonState.cross)
                            {
                                if (hapticCooldown.ElapsedMilliseconds > 300)
                                {
                                    dualsense.PlayHaptics(HapticEffect.UIselect1, 0.0f, 1.0f, 1.0f, true);
                                    hapticCooldown.Restart();
                                }
                            }
                            else if (dualsense.ButtonState.circle)
                            {
                                if (hapticCooldown.ElapsedMilliseconds > 300)
                                {
                                    dualsense.PlayHaptics(HapticEffect.UIselect2, 0.0f, 1.0f, 1.0f, true);
                                    hapticCooldown.Restart();
                                }
                            }

                            dualsense.SetLightbar(255, 102, 0);
                            dualsense.SetLeftTrigger(TriggerType.TriggerModes.Rigid_B, 0, 0, 0, 0, 0, 0, 0);
                            dualsense.SetRightTrigger(TriggerType.TriggerModes.Rigid_B, 0, 0, 0, 0, 0, 0, 0);
                            dualsense.SetPlayerLED(LED.PlayerLED.OFF);
                        }

                        lastDpadUP = dualsense.ButtonState.DpadUp;
                        if (dualsense.ButtonState.R2 >= triggerThreshold)
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
        }).Start();


    }

    private static Stopwatch watch = new Stopwatch();
    private static void Dualshock4_FeedbackReceived(object sender, DualShock4FeedbackReceivedEventArgs e)
    {
        int l_rotor = e.SmallMotor;
        int r_rotor = e.LargeMotor;

        dualsense.SetStandardRumble((byte)l_rotor, (byte)r_rotor);

        //Console.WriteLine(l_rotor + " " + r_rotor);

        if (l_rotor == 0 && r_rotor == 2 || l_rotor == 38 && r_rotor == 25)
        {
            if (watch.ElapsedMilliseconds <= 21000)
            {
                airDrop = true;
                watch.Start();
            }
        }

        if (l_rotor == 153 && r_rotor == 153 || l_rotor == 168 && r_rotor == 168)
            meleeHit = true;
        else
            meleeHit = false;

        if (watch.ElapsedMilliseconds >= 21000)
        {
            airDrop = false;
            watch.Reset();
        }
    }

    public static void turnMicrophone(bool onORoff)
    {
        game.setMicrophone(onORoff);
    }
}
