using DL1_Dualsense;
using System.Diagnostics;

internal class Program
{
    private static Game game = new Game();
    private static DSX DSX = new DSX();
    private static void Main(string[] args)
    {
        bool gameRunning = true;
        // Lightbar color (0-255)
        int R = 0;
        int G = 0;
        int B = 0;

        // Chase level (0-4)
        int chase = 0;

        // Microphone status
        bool micLed;

        // Player's health
        float hp = 0;

        Trigger whichTrigger;
        TriggerMode triggerMode = TriggerMode.Normal;
        int triggerThreshold = 100;
        int weapon;
        int start = 0;
        int end = 0;
        int force = 0;

        DSX.Connect();
        Console.WriteLine("The program is running.");
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

                try
                {
                    if (game.isMicrophoneOn())
                        micLed = false;
                    else
                        micLed = true;

                    chase = game.getChaseLevel();
                    hp = game.getHP();

                    if (hp >= 175) { R = 0; G = 255; B = 0; }
                    else if (hp < 175 && hp > 75) { R = 255; G = 255; B = 0; }
                    else if (hp < 75 && hp != 0) { R = 255; G = 0; B = 0; }
                    else { R = 0; G = 0; B = 0; }

                    weapon = game.getWeaponType();
                    whichTrigger = Trigger.Right;
                    if (weapon == 2058 || weapon >= 2024 && weapon <= 2029) // 1H sharp weapon (machete etc.)
                    {
                        start = 0;
                        end = 0;
                        force = 8;
                        triggerMode = TriggerMode.Choppy;
                    }
                    else if (weapon == 1974 || weapon >= 1940 && weapon <= 1960) // 2H weapon
                    {
                        start = 0;
                        end = 0;
                        force = 8;
                        triggerMode = TriggerMode.Rigid;
                    }
                    else if (weapon == 886) // Crossbow
                    {
                        start = 2;
                        end = 6;
                        force = 8;
                        triggerMode = TriggerMode.SemiAutomaticGun;
                    }
                    else if (weapon == 1897 || weapon >= 1861 && weapon <= 1897) // 1H blunt weapon (baseball bat etc.)
                    {
                        start = 4;
                        end = 7;
                        force = 2;
                        triggerMode = TriggerMode.Medium;
                    }
                    else if (weapon == 1184 || weapon >= 1152 && weapon <= 1165) // Knifes
                    {
                        start = 4;
                        end = 7;
                        force = 2;
                        triggerMode = TriggerMode.Choppy;
                    }
                    else if (weapon <= 949 && weapon >= 933) // Fists
                    {
                        start = 3;
                        end = 8;
                        force = 8;
                        triggerMode = TriggerMode.Medium;
                    }
                    else if (weapon == 830) // chainsaw
                    {
                        start = 6;
                        end = 8;
                        force = 20;
                        triggerMode = TriggerMode.AutomaticGun;
                    }
                    else if (weapon == 1282 || weapon == 1246) // Automatic rifle
                    {
                        start = 4;
                        end = 8;
                        force = 10;
                        triggerMode = TriggerMode.AutomaticGun;
                    }
                    else if (weapon == 2169 || weapon == 2079) // Submachine gun
                    {
                        start = 8;
                        end = 8;
                        force = 12;
                        triggerMode = TriggerMode.AutomaticGun;
                    }
                    else if (weapon != 757 && weapon >= 734 && weapon <= 759) // Bows
                    {
                        start = 8;
                        end = 8;
                        force = 12;
                        triggerMode = TriggerMode.Bow;
                    }
                    else if (weapon == 1715 || weapon == 1690) // Shotguns
                    {
                        start = 3;
                        end = 8;
                        force = 8;
                        triggerMode = TriggerMode.SemiAutomaticGun;
                    }
                    else if (weapon == 1283) // Automatic rifle (EMPTY)
                    {
                        start = 3;
                        end = 8;
                        force = 8;
                        triggerMode = TriggerMode.SemiAutomaticGun;
                    }
                    else if (weapon == 1589 || weapon == 1562) // Double-Action Revolver
                    {
                        start = 3;
                        end = 8;
                        force = 8;
                        triggerMode = TriggerMode.SemiAutomaticGun;
                    }
                    else if (weapon == 1811 || weapon == 1783) // Single-Action Revolver
                    {
                        start = 3;
                        end = 8;
                        force = 8;
                        triggerMode = TriggerMode.SemiAutomaticGun;
                    }
                    else if (weapon == 703 || weapon == 667) // Pistol
                    {
                        start = 3;
                        end = 6;
                        force = 8;
                        triggerMode = TriggerMode.SemiAutomaticGun;
                    }
                    else
                    {
                        triggerMode = TriggerMode.Normal;
                    }

                    // Change RGB colors to white
                    if (game.isFlashlightOn())
                    {
                        R = 255; G = 255; B = 255;
                    }

                    // Change RGB colors to purple
                    if (game.isUVOn())
                    {
                        R = 60; G = 0; B = 255;
                        DSX.Update(R, G, B, chase, micLed, whichTrigger, triggerMode, triggerThreshold, start, end, force);
                    }

                    if (game.isUVRecharging())
                    {
                        Thread.Sleep(100);
                        R = 60; G = 0; B = 255;
                        DSX.Update(R, G, B, chase, micLed, whichTrigger, triggerMode, triggerThreshold, start, end, force);
                        Thread.Sleep(100);
                        R = 255; G = 0; B = 0;
                        DSX.Update(R, G, B, chase, micLed, whichTrigger, triggerMode, triggerThreshold, start, end, force);
                    }

                    // Update left trigger here

                    DSX.Update(R, G, B, chase, micLed, whichTrigger, triggerMode, triggerThreshold, start, end, force);

                    Thread.Sleep(250);
                }
                catch(InvalidOperationException)
                {
                    gameRunning = false;
                }

               
            }
        }
    }
}