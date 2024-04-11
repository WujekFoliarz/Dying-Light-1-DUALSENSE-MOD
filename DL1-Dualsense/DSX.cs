using System.Net.Sockets;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Diagnostics;

namespace DL1_Dualsense
{
    public class DSX
    {
        static bool debug = false;
        static UdpClient client;
        static IPEndPoint endPoint;
        static int controllerIndex = 0;
        static DateTime TimeSent;

        private static void Send(Packet data)
        {
            var RequestData = Encoding.ASCII.GetBytes(Triggers.PacketToJson(data));
            client.Send(RequestData, RequestData.Length, endPoint);
            TimeSent = DateTime.Now;
        }

        public static void Connect()
        {
            client = new UdpClient();
            var portNumber = File.ReadAllText(@"C:\Temp\DualSenseX\DualSenseX_PortNumber.txt");
            endPoint = new IPEndPoint(Triggers.localhost, Convert.ToInt32(portNumber));
            Console.WriteLine($"DSX PORT: {portNumber}\n");
        }

        public static void Update(int R, int G, int B, int chase, bool micLed, Trigger whichTrigger, TriggerMode triggerMode, int triggerThreshold, int start, int end, int force)
        {
            Packet p = new Packet();
            p.instructions = new Instruction[7];
            
            p.instructions[0].type = InstructionType.TriggerUpdate;
            p.instructions[0].parameters = new object[] { controllerIndex, whichTrigger, triggerMode, start, end, force, 8, 8 };

            if(triggerMode == TriggerMode.Bow)
            {
                p.instructions[0].type = InstructionType.TriggerUpdate;
                p.instructions[0].parameters = new object[] { controllerIndex, Trigger.Right, TriggerMode.Bow, 0, 8, 2, 5 };
            }

            p.instructions[1].type = InstructionType.TriggerThreshold;
            p.instructions[1].parameters = new object[] { controllerIndex, whichTrigger, triggerThreshold };


            p.instructions[2].type = InstructionType.RGBUpdate;
            p.instructions[2].parameters = new object[] { controllerIndex, R, G, B };

            p.instructions[3].type = InstructionType.PlayerLED;

            if(chase == 1) {
                p.instructions[3].parameters = new object[] { controllerIndex, false, false, true, false, false };
            }
            else if (chase == 2)
            {
                p.instructions[3].parameters = new object[] { controllerIndex, false, true, false, true, false };
            }
            else if (chase == 3)
            {
                p.instructions[3].parameters = new object[] { controllerIndex, true, false, true, false, true };
            }
            else if (chase == 4)
            {
                p.instructions[3].parameters = new object[] { controllerIndex, true, true, false, true, true };
            }
            else
            {
                p.instructions[3].parameters = new object[] { controllerIndex, false, false, false, false, false };
            }

            p.instructions[4].type = InstructionType.PlayerLEDNewRevision;

            if(chase == 1){
                p.instructions[4].parameters = new object[] { controllerIndex, PlayerLEDNewRevision.One };
            }
            else if (chase == 2)
            {
                p.instructions[4].parameters = new object[] { controllerIndex, PlayerLEDNewRevision.Two };
            }
            else if (chase == 3)
            {
                p.instructions[4].parameters = new object[] { controllerIndex, PlayerLEDNewRevision.Three };
            }
            else if (chase == 4)
            {
                p.instructions[4].parameters = new object[] { controllerIndex, PlayerLEDNewRevision.Four };
            }
            else
            {
                p.instructions[4].parameters = new object[] { controllerIndex, PlayerLEDNewRevision.AllOff };
            }

            p.instructions[5].type = InstructionType.MicLED;
            if(micLed == true)
                p.instructions[5].parameters = new object[] { controllerIndex, MicLEDMode.On };
            else
                p.instructions[5].parameters = new object[] { controllerIndex, MicLEDMode.Off };


            Send(p);

            Process[] process = Process.GetProcessesByName("DSX");

            if (process.Length == 0)
            {
                Console.WriteLine("DSX is not running... \n");
                Thread.Sleep(1000);
                Console.Clear();
            }
            else
            {
                Console.SetCursorPosition(0, 0);
                Console.Write("The program is running.");
                
                if (debug == true)
                {
                    Console.WriteLine("Waiting for Server Response...\n");
                    byte[] bytesReceivedFromServer = client.Receive(ref endPoint);

                    if (bytesReceivedFromServer.Length > 0)
                    {
                        ServerResponse ServerResponseJson = JsonConvert.DeserializeObject<ServerResponse>($"{Encoding.ASCII.GetString(bytesReceivedFromServer, 0, bytesReceivedFromServer.Length)}");
                        Console.WriteLine("===================================================================");

                        Console.WriteLine($"Status: {ServerResponseJson.Status}");
                        DateTime CurrentTime = DateTime.Now;
                        TimeSpan Timespan = CurrentTime - TimeSent;
                        // First send shows high Milliseconds response time for some reason
                        Console.WriteLine($"Time Received: {ServerResponseJson.TimeReceived}, took: {Timespan.TotalMilliseconds} to receive response from DSX");
                        Console.WriteLine($"isControllerConnected: {ServerResponseJson.isControllerConnected}");
                        Console.WriteLine($"BatteryLevel: {ServerResponseJson.BatteryLevel}");

                        Console.WriteLine("===================================================================\n");
                    }
                }
            }
        }
    }

    public static class Triggers
    {
        public static IPAddress localhost = new IPAddress(new byte[] { 127, 0, 0, 1 });

        public static string PacketToJson(Packet packet)
        {
            return JsonConvert.SerializeObject(packet);
        }

        public static Packet JsonToPacket(string json)
        {
            return JsonConvert.DeserializeObject<Packet>(json);
        }
    }

    public enum TriggerMode
    {
        Normal = 0,
        GameCube = 1,
        VerySoft = 2,
        Soft = 3,
        Hard = 4,
        VeryHard = 5,
        Hardest = 6,
        Rigid = 7,
        VibrateTrigger = 8,
        Choppy = 9,
        Medium = 10,
        VibrateTriggerPulse = 11,
        CustomTriggerValue = 12,
        Resistance = 13,
        Bow = 14,
        Galloping = 15,
        SemiAutomaticGun = 16,
        AutomaticGun = 17,
        Machine = 18
    }

    public enum CustomTriggerValueMode
    {
        OFF = 0,
        Rigid = 1,
        RigidA = 2,
        RigidB = 3,
        RigidAB = 4,
        Pulse = 5,
        PulseA = 6,
        PulseB = 7,
        PulseAB = 8,
        VibrateResistance = 9,
        VibrateResistanceA = 10,
        VibrateResistanceB = 11,
        VibrateResistanceAB = 12,
        VibratePulse = 13,
        VibratePulseA = 14,
        VibratePulsB = 15,
        VibratePulseAB = 16
    }

    public enum PlayerLEDNewRevision
    {
        One = 0,
        Two = 1,
        Three = 2,
        Four = 3,
        Five = 4, // Five is Also All On
        AllOff = 5
    }
    public enum MicLEDMode
    {
        On = 0,
        Pulse = 1,
        Off = 2
    }

    public enum Trigger
    {
        Invalid,
        Left,
        Right
    }

    public enum InstructionType
    {
        Invalid,
        TriggerUpdate,
        RGBUpdate,
        PlayerLED,
        TriggerThreshold,
        MicLED,
        PlayerLEDNewRevision,
        ResetToUserSettings
    }

    public struct Instruction
    {
        public InstructionType type;
        public object[] parameters;
    }

    public class Packet
    {
        public Instruction[] instructions;
    }

    public class ServerResponse
    {
        public string Status;
        public string TimeReceived;
        public bool isControllerConnected;
        public int BatteryLevel;
    }
}
