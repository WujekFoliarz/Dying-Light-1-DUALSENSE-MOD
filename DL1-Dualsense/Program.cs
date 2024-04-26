using DL1_Dualsense;
using System.Diagnostics;

internal class Program
{
    private static Game game = new Game();
    private static DualsenseControllerAPI controllerAPI = new DualsenseControllerAPI();
    private static void Main(string[] args)
    {
        bool gameRunning = true;
        bool apiRunning = false;

        // Microphone status
        bool micLed;

        // Player's health
        float hp = 0;

        int weapon = 0;
        int lastWeapon = 0;

        Console.WriteLine("The program is running.");
        new Thread(() => {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            Thread.CurrentThread.IsBackground = true;
            while (true)
            {               
                Process[] process = Process.GetProcessesByName("DyingLightGame");
                if (process.Length == 0)
                {
                    gameRunning = false;
                    Console.WriteLine("Dying Light is not running... \n");
                    Thread.Sleep(1000);
                    Console.Clear();
                }
                else
                {
                    if (!gameRunning) { game = new Game(); }
                    if (!apiRunning) { controllerAPI.Start(); apiRunning = true; }

                    try
                    {
                        if (game.isMicrophoneOn())
                            controllerAPI.microphoneLED = false;
                        else
                            controllerAPI.microphoneLED = true;

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
                        if (hp >= 175) { controllerAPI.R = 0; controllerAPI.G = 255; controllerAPI.B = 0; }
                        else if (hp < 175 && hp > 75) { controllerAPI.R = 255; controllerAPI.G = 255; controllerAPI.B = 0; }
                        else if (hp < 75 && hp != 0) { controllerAPI.R = 255; controllerAPI.G = 255; controllerAPI.B = 0; }
                        else { controllerAPI.R = 0; controllerAPI.G = 0; controllerAPI.B = 0; }

                        lastWeapon = weapon;
                        weapon = game.getWeaponType();

                        //Console.WriteLine(weapon);
                        if (weapon == 2058 || weapon >= 2024 && weapon <= 2029) // 1H sharp weapon (machete etc.)
                        {
                            controllerAPI.triggerThreshold = 120;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid_A;
                            controllerAPI.rightTriggerForces = [20, 255, 0, 0, 0, 0, 0];
                        }
                        else if (weapon == 1974 || weapon >= 1940 && weapon <= 1960) // 2H weapon
                        {
                            controllerAPI.triggerThreshold = 120;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid;
                            controllerAPI.rightTriggerForces = [0, 255, 0, 0, 0, 0, 0];
                        }
                        else if (weapon == 886 || weapon == 850) // Crossbow
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid_A;
                            controllerAPI.rightTriggerForces = [55, 0, 75, 40, 90, 255, 0];
                        }
                        else if (weapon == 1897 || weapon >= 1861 && weapon <= 1897) // 1H blunt weapon (baseball bat etc.)
                        {
                            controllerAPI.triggerThreshold = 120;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid_A;
                            controllerAPI.rightTriggerForces = [70, 170, 120, 0, 0, 0, 0];
                        }
                        else if (weapon == 1184 || weapon >= 1152 && weapon <= 1165) // Knifes
                        {
                            controllerAPI.triggerThreshold = 120;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid_A;
                            controllerAPI.rightTriggerForces = [20, 1, 20, 0, 0, 0, 0];
                        }
                        else if (weapon <= 949 && weapon >= 933) // Fists
                        {
                            controllerAPI.triggerThreshold = 120;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid_A;
                            controllerAPI.rightTriggerForces = [1, 1, 0, 0, 0, 0, 0];
                        }
                        else if (weapon == 830) // Chainsaw
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Pulse_B;
                            controllerAPI.rightTriggerForces = [35, 137, 65, 0, 0, 0, 0];
                        }
                        else if (weapon == 1282 && lastWeapon != 1283 || weapon == 1246) // Automatic rifle
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Pulse_B;
                            controllerAPI.rightTriggerForces = [10, 255, 120, 0, 0, 0, 0];
                        }
                        else if (weapon == 1283) // Automatic rifle (EMPTY)
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Pulse_AB;
                            controllerAPI.rightTriggerForces = [120, 0, 255, 0, 0, 0, 0];
                        }
                        else if (weapon == 2169 || weapon == 2079) // Submachine gun
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Pulse_B;
                            controllerAPI.rightTriggerForces = [15, 255, 120, 0, 0, 0, 0];
                        }
                        else if (weapon == 2170 || weapon == 2080) // Submachine gun (EMPTY)
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Pulse_AB;
                            controllerAPI.rightTriggerForces = [120, 0, 255, 0, 0, 0, 0];
                        }
                        else if (weapon != 757 && weapon >= 734 && weapon <= 759) // Bows
                        {
                            controllerAPI.triggerThreshold = 80;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid;
                            controllerAPI.rightTriggerForces = [0, 255, 0, 255, 255, 255, 0];
                        }
                        else if (weapon == 1715 || weapon == 1690) // Shotguns
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Pulse_AB;
                            controllerAPI.rightTriggerForces = [10, 75, 82, 0, 0, 0, 0];
                        }
                        else if (weapon == 1715 || weapon == 1690) // Double-barrel shotgun
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Pulse_AB;
                            controllerAPI.rightTriggerForces = [255, 0, 255, 0, 0, 0, 0];
                        }
                        else if (weapon == 1589 || weapon == 1562) // Double-Action Revolver
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid_AB;
                            controllerAPI.rightTriggerForces = [255, 184, 255, 143, 71, 0, 0];
                        }
                        else if (weapon == 1811 || weapon == 1783 || weapon == 1812) // Single-Action Revolver
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid_AB;
                            controllerAPI.rightTriggerForces = [93, 184, 255, 143, 71, 0, 0];
                        }
                        else if (weapon == 703 || weapon == 667 || weapon == 704) // Pistol
                        {
                            controllerAPI.triggerThreshold = 255;
                            controllerAPI.rightTriggerMode = TriggerModes.Rigid_AB;
                            controllerAPI.rightTriggerForces = [93, 184, 255, 143, 71, 0, 0];
                        }
                        else
                        {
                            controllerAPI.triggerThreshold = 0;
                            controllerAPI.rightTriggerMode = TriggerModes.Off;
                            controllerAPI.rightTriggerForces = [0,0,0,0,0,0,0];
                        }

                        // Change RGB colors to white
                        if (game.isFlashlightOn())
                        {
                            controllerAPI.R = 255;
                            controllerAPI.G = 255;
                            controllerAPI.B = 255;
                        }

                        // Change RGB colors to purple
                        if (game.isUVOn())
                        {
                            controllerAPI.R = 60;
                            controllerAPI.G = 0;
                            controllerAPI.B = 255;
                        }

                        if (game.isUVRecharging())
                        {
                            controllerAPI.R = 60;
                            controllerAPI.G = 0;
                            controllerAPI.B = 255;
                            Thread.Sleep(250);
                            controllerAPI.R = 255;
                            controllerAPI.G = 0;
                            controllerAPI.B = 0;
                        }

                        Thread.Sleep(250);
                    }
                    catch (InvalidOperationException)
                    {
                        gameRunning = false;
                    }
                }
            }
        }).Start();
        Console.ReadKey();
    }
}