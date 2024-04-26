using HidApi;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace DL1_Dualsense
{
    public class DualsenseControllerAPI
    {
        private bool bt_initialized = false;
        private bool xbox360ControllerEmulation = false;
        public int l_rotor;
        public int r_rotor;
        private int reportLength;
        private ConnectionType connectionType;
        private IXbox360Controller xbox360Controller;
        private IDualShock4Controller dualshock4;
        private DeviceInfo dualsenseInfo;
        private Device dualsense;
        private DSState state;
        private DSBattery battery;

        public int R = 0;
        public int G = 0;
        public int B = 0;
        public TriggerModes leftTriggerMode = TriggerModes.Off;
        public TriggerModes rightTriggerMode = TriggerModes.Off;
        public int[] leftTriggerForces = new int[7];
        public int[] rightTriggerForces = new int[7];
        public PlayerID playerLED = 0;
        public Brightness brightness = Brightness.high;
        public bool microphoneLED = false;
        public int triggerThreshold;

        public void Start()
        {
            dualsenseInfo = getDevice();
            dualsense = dualsenseInfo.ConnectToDevice();
            connectionType = getConnectionType(dualsense);

            byte[] report = new byte[reportLength];

            state = new DSState();
            battery = new DSBattery();
            ViGEmClient client = new ViGEmClient();


            xbox360Controller = client.CreateXbox360Controller();
            dualshock4 = client.CreateDualShock4Controller();

            if (!xbox360ControllerEmulation)
                dualshock4.Connect();
            else
                xbox360Controller.Connect();

            dualshock4.FeedbackReceived += Dualshock4_FeedbackReceived;
            

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = false;
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                while (true)
                {
                    try
                    {
                        int micLed = microphoneLED ? 1 : 0;
                        dualsense.Write(PrepareReport(R, G, B, leftTriggerMode, rightTriggerMode, leftTriggerForces, rightTriggerForces, playerLED, brightness, micLed));
                    }
                    catch (HidException)
                    {
                        Console.WriteLine("Connection to DUALSENSE was interrupted.");
                        break;
                    }

                    ReadInput();

                    // Emulate DUALSHOCK 4
                    if (!xbox360ControllerEmulation)
                    {
                        dualshock4.SetButtonState(DualShock4Button.Cross, state.cross);
                        dualshock4.SetButtonState(DualShock4Button.Circle, state.circle);
                        dualshock4.SetButtonState(DualShock4Button.Triangle, state.triangle);
                        dualshock4.SetButtonState(DualShock4Button.Square, state.square);

                        DualShock4DPadDirection direction = DualShock4DPadDirection.None;
                        if (state.DpadDown) { direction = DualShock4DPadDirection.South; }
                        else if (state.DpadUp) { direction = DualShock4DPadDirection.North; }
                        else if (state.DpadLeft) { direction = DualShock4DPadDirection.West; }
                        else if (state.DpadRight) { direction = DualShock4DPadDirection.East; }
                        dualshock4.SetDPadDirection(direction);

                        dualshock4.SetAxisValue(DualShock4Axis.LeftThumbX, (byte)state.LX);
                        dualshock4.SetAxisValue(DualShock4Axis.LeftThumbY, (byte)state.LY);
                        dualshock4.SetAxisValue(DualShock4Axis.RightThumbX, (byte)state.RX);
                        dualshock4.SetAxisValue(DualShock4Axis.RightThumbY, (byte)state.RY);
                        dualshock4.SetButtonState(DualShock4Button.ThumbLeft, state.L3);
                        dualshock4.SetButtonState(DualShock4Button.ThumbRight, state.R3);

                        if ((byte)state.L2 >= triggerThreshold)
                            dualshock4.LeftTrigger = (byte)state.L2;
                        else
                            dualshock4.LeftTrigger = (byte)0;

                        if ((byte)state.R2 >= triggerThreshold)
                            dualshock4.RightTrigger = (byte)state.R2;
                        else
                            dualshock4.RightTrigger = (byte)0;

                        dualshock4.SetButtonState(DualShock4SpecialButton.Touchpad, state.touchBtn);
                        dualshock4.SetButtonState(DualShock4Button.Share, state.share);
                        dualshock4.SetButtonState(DualShock4Button.Options, state.options);
                        dualshock4.SetButtonState(DualShock4Button.ShoulderLeft, state.L1);
                        dualshock4.SetButtonState(DualShock4Button.ShoulderRight, state.R1);
                        dualshock4.SetButtonState(DualShock4SpecialButton.Ps, state.ps);
                    }
                    else // Emulate XBOX 360 Controller
                    {
                        xbox360Controller.SetButtonState(Xbox360Button.A, state.cross);
                        xbox360Controller.SetButtonState(Xbox360Button.B, state.circle);
                        xbox360Controller.SetButtonState(Xbox360Button.Y, state.triangle);
                        xbox360Controller.SetButtonState(Xbox360Button.X, state.square);

                        xbox360Controller.SetButtonState(Xbox360Button.Up, state.DpadUp);
                        xbox360Controller.SetButtonState(Xbox360Button.Left, state.DpadLeft);
                        xbox360Controller.SetButtonState(Xbox360Button.Right, state.DpadRight);
                        xbox360Controller.SetButtonState(Xbox360Button.Down, state.DpadDown);

                        //-32767 minimum
                        //32766 max
                        xbox360Controller.SetAxisValue(Xbox360Axis.LeftThumbX, (short)ConvertRange(state.LX, 0, 255, -32767, 32766));
                        xbox360Controller.SetAxisValue(Xbox360Axis.LeftThumbY, (short)ConvertRange(state.LY, 255, 0, -32767, 32766));
                        xbox360Controller.SetAxisValue(Xbox360Axis.RightThumbX, (short)ConvertRange(state.RX, 0, 255, -32767, 32766));
                        xbox360Controller.SetAxisValue(Xbox360Axis.RightThumbY, (short)ConvertRange(state.RY, 255, 0, -32767, 32766));
                        xbox360Controller.SetButtonState(Xbox360Button.LeftThumb, state.L3);
                        xbox360Controller.SetButtonState(Xbox360Button.RightThumb, state.R3);

                        xbox360Controller.LeftTrigger = (byte)state.L2;
                        xbox360Controller.RightTrigger = (byte)state.R2;

                        xbox360Controller.SetButtonState(Xbox360Button.Start, state.options);
                        xbox360Controller.SetButtonState(Xbox360Button.Back, state.share);
                        xbox360Controller.SetButtonState(Xbox360Button.LeftShoulder, state.L1);
                        xbox360Controller.SetButtonState(Xbox360Button.RightShoulder, state.R1);
                        xbox360Controller.SetButtonState(Xbox360Button.Guide, state.ps);
                    }
                }
            }).Start();
        }

        private void Xbox360Controller_FeedbackReceived(object sender, Xbox360FeedbackReceivedEventArgs e)
        {
            l_rotor = e.SmallMotor;
            r_rotor = e.LargeMotor;
        }

        private void Dualshock4_FeedbackReceived(object sender, DualShock4FeedbackReceivedEventArgs e)
        {
            l_rotor = e.SmallMotor;
            r_rotor = e.LargeMotor;
        }

        byte[] PrepareReport(int r, int g, int b, TriggerModes LeftTriggerMode, TriggerModes RightTriggerMode, int[] LeftTriggerForces, int[] RightTriggerForces, PlayerID playerLED, Brightness ledBrightness, int MicrophoneLED)
        {
            byte[] outReport = new byte[reportLength]; // create empty byte array with length of output report
            if (connectionType == ConnectionType.USB)
            {
                outReport[0] = 0x02;
                outReport[1] = 0xff;
                outReport[2] = 0x1 | 0x2 | 0x4 | 0x10 | 0x40;
                outReport[3] = (byte)l_rotor; // right low freq motor 0-255
                outReport[4] = (byte)r_rotor; // left low freq motor 0-255
                outReport[9] = (byte)MicrophoneLED; //microphone led
                outReport[10] = false ? (byte)0x10 : (byte)0x00;
                outReport[11] = (byte)RightTriggerMode;  // Swapped with LeftTriggerMode
                outReport[12] = (byte)RightTriggerForces[0];  // Swapped with LeftTriggerForces
                outReport[13] = (byte)RightTriggerForces[1];
                outReport[14] = (byte)RightTriggerForces[2];
                outReport[15] = (byte)RightTriggerForces[3];
                outReport[16] = (byte)RightTriggerForces[4];
                outReport[17] = (byte)RightTriggerForces[5];
                outReport[20] = (byte)RightTriggerForces[6];  
                outReport[22] = (byte)LeftTriggerMode;  
                outReport[23] = (byte)LeftTriggerForces[0];  
                outReport[24] = (byte)LeftTriggerForces[1];
                outReport[25] = (byte)LeftTriggerForces[2];
                outReport[26] = (byte)LeftTriggerForces[3];
                outReport[27] = (byte)LeftTriggerForces[4];
                outReport[28] = (byte)LeftTriggerForces[5];
                outReport[31] = (byte)LeftTriggerForces[6]; 
                outReport[39] = (byte)LedOptions.PlayerLedBrightness;
                outReport[42] = (byte)PulseOptions.Off;
                outReport[43] = (byte)ledBrightness;
                outReport[44] = (byte)playerLED;
                outReport[45] = (byte)r;
                outReport[46] = (byte)g;
                outReport[47] = (byte)b;
            }
            else if (connectionType == ConnectionType.BT)
            {
                outReport[0] = 0x31;
                outReport[1] = 0x02;
                outReport[2] = 0xFF;
                if (bt_initialized == false)
                {
                    outReport[3] = 0x1 | 0x2 | 0x4 | 0x8 | 0x10 | 0x40;
                    Console.WriteLine("Bluetooth initialized.");
                    bt_initialized = true;
                }
                else if (bt_initialized == true)
                {
                    outReport[3] = 0x1 | 0x2 | 0x4 | 0x10 | 0x40;
                }
                outReport[4] = (byte)r_rotor; // right low freq motor 0-255
                outReport[5] = (byte)l_rotor; // left low freq motor 0-255
                outReport[10] = (byte)MicrophoneLED; // microphone led
                outReport[11] = 0x10;
                outReport[12] = (byte)RightTriggerMode;
                outReport[13] = (byte)RightTriggerForces[0];  
                outReport[14] = (byte)RightTriggerForces[1];
                outReport[15] = (byte)RightTriggerForces[2];
                outReport[16] = (byte)RightTriggerForces[3];
                outReport[17] = (byte)RightTriggerForces[4];
                outReport[18] = (byte)RightTriggerForces[5];
                outReport[21] = (byte)RightTriggerForces[6]; 
                outReport[23] = (byte)LeftTriggerMode;
                outReport[24] = (byte)LeftTriggerForces[0];
                outReport[25] = (byte)LeftTriggerForces[1];
                outReport[26] = (byte)LeftTriggerForces[2];
                outReport[27] = (byte)LeftTriggerForces[3];
                outReport[28] = (byte)LeftTriggerForces[4];
                outReport[29] = (byte)LeftTriggerForces[5];
                outReport[32] = (byte)LeftTriggerForces[6];

                outReport[40] = (byte)LedOptions.PlayerLedBrightness;
                outReport[43] = (byte)PulseOptions.Off;
                outReport[44] = (byte)ledBrightness;
                outReport[45] = (byte)playerLED;
                outReport[46] = (byte)r;
                outReport[47] = (byte)g;
                outReport[48] = (byte)b;
                uint crcChecksum = CRC32Utils.ComputeCRC32(outReport, 74);
                byte[] checksumBytes = BitConverter.GetBytes(crcChecksum);
                Array.Copy(checksumBytes, 0, outReport, 74, 4);
            }
            return outReport;
        }


        void ReadInput()
        {
            ReadOnlySpan<byte> output = dualsense.Read(reportLength);
            byte[] states = output.ToArray();
            int offset = 0;
            if (connectionType == ConnectionType.BT) { offset = 1; }

            // states 0 is always 1
            state.LX = states[1 + offset];
            state.LY = states[2 + offset];
            state.RX = states[3 + offset];
            state.RY = states[4 + offset];
            state.L2 = states[5 + offset];
            state.R2 = states[6 + offset];

            // state 7 always increments -> not used anywhere

            byte buttonState = states[8 + offset];
            state.triangle = (buttonState & (1 << 7)) != 0;
            state.circle = (buttonState & (1 << 6)) != 0;
            state.cross = (buttonState & (1 << 5)) != 0;
            state.square = (buttonState & (1 << 4)) != 0;

            // dpad
            byte dpad_state = (byte)(buttonState & 0x0F);
            state.SetDPadState(dpad_state);

            byte misc = states[9 + offset];
            state.R3 = (misc & (1 << 7)) != 0;
            state.L3 = (misc & (1 << 6)) != 0;
            state.options = (misc & (1 << 5)) != 0;
            state.share = (misc & (1 << 4)) != 0;
            state.R2Btn = (misc & (1 << 3)) != 0;
            state.L2Btn = (misc & (1 << 2)) != 0;
            state.R1 = (misc & (1 << 1)) != 0;
            state.L1 = (misc & (1 << 0)) != 0;

            byte misc2 = states[10 + offset];
            state.ps = (misc2 & (1 << 0)) != 0;
            state.touchBtn = (misc2 & 0x02) != 0;
            state.micBtn = (misc2 & 0x04) != 0;

            // trackpad touch
            state.trackPadTouch0.ID = (byte)(states[33 + offset] & 0x7F);
            state.trackPadTouch0.IsActive = (states[33 + offset] & 0x80) == 0;
            state.trackPadTouch0.X = ((states[35 + offset] & 0x0F) << 8) | states[34];
            state.trackPadTouch0.Y = ((states[36 + offset]) << 4) | ((states[35] & 0xF0) >> 4);

            // trackpad touch
            state.trackPadTouch1.ID = (byte)(states[37 + offset] & 0x7F);
            state.trackPadTouch1.IsActive = (states[37 + offset] & 0x80) == 0;
            state.trackPadTouch1.X = ((states[39 + offset] & 0x0F) << 8) | states[38];
            state.trackPadTouch1.Y = ((states[40 + offset]) << 4) | ((states[39] & 0xF0) >> 4);

            // accelerometer
            state.accelerometer.X = BitConverter.ToInt16(new byte[] { states[16 + offset], states[17 + offset] }, 0);
            state.accelerometer.Y = BitConverter.ToInt16(new byte[] { states[18 + offset], states[19 + offset] }, 0);
            state.accelerometer.Z = BitConverter.ToInt16(new byte[] { states[20 + offset], states[21 + offset] }, 0);

            // gyrometer
            state.gyro.Pitch = BitConverter.ToInt16(new byte[] { states[22 + offset], states[23 + offset] }, 0);
            state.gyro.Yaw = BitConverter.ToInt16(new byte[] { states[24 + offset], states[25 + offset] }, 0);
            state.gyro.Roll = BitConverter.ToInt16(new byte[] { states[26 + offset], states[27 + offset] }, 0);
        }

        ConnectionType getConnectionType(Device dualsense)
        {
            var report = dualsense.Read(100);
            if (report.Length == 64)
            {
                Console.WriteLine("Connection type = USB");
                reportLength = 64;
                return ConnectionType.USB;
            }
            else if (report.Length == 78)
            {
                Console.WriteLine("Connection type = Bluetooth");
                reportLength = 78;
                return ConnectionType.BT;
            }
            else { throw new ArgumentException("Could not specify connection type."); }
        }

        DeviceInfo getDevice()
        {
            foreach (var deviceInfo in Hid.Enumerate())
            {
                using var device = deviceInfo.ConnectToDevice();
                if (deviceInfo.VendorId == 1356 && deviceInfo.ProductId == 3302)
                {
                    Console.WriteLine("Found controller - " + deviceInfo.Path);
                    return deviceInfo;
                }
                else
                {
                    //Console.WriteLine(deviceInfo.ManufacturerString + " " + deviceInfo.ProductId);
                }
            }
            throw new ArgumentException("Could not find Dualsense device.");
        }

        int ConvertRange(int value, int oldMin, int oldMax, int newMin, int newMax)
        {
            if (oldMin == oldMax)
            {
                throw new ArgumentException("Old minimum and maximum cannot be equal.");
            }
            float ratio = (float)(newMax - newMin) / (float)(oldMax - oldMin);
            float scaledValue = (value - oldMin) * ratio + newMin;
            return Math.Clamp((int)scaledValue, newMin, newMax);
        }
    }

    [Flags]
    public enum ConnectionType
    {
        BT = 0x0,
        USB = 0x1
    }

    [Flags]
    public enum LedOptions
    {
        Off = 0x0,
        PlayerLedBrightness = 0x1,
        UninterrumpableLed = 0x2,
        Both = 0x01 | 0x02
    }

    [Flags]
    public enum PulseOptions
    {
        Off = 0x0,
        FadeBlue = 0x1,
        FadeOut = 0x2
    }

    [Flags]
    public enum Brightness
    {
        high = 0x0,
        medium = 0x1,
        low = 0x2
    }

    [Flags]
    public enum PlayerID
    {
        PLAYER_1 = 4,
        PLAYER_2 = 10,
        PLAYER_3 = 21,
        PLAYER_4 = 27,
        ALL = 31
    }

    [Flags]
    public enum TriggerModes
    {
        Off = 0x0,
        Rigid = 0x1,
        Pulse = 0x2,
        Rigid_A = 0x1 | 0x20,
        Rigid_B = 0x1 | 0x04,
        Rigid_AB = 0x1 | 0x20 | 0x04,
        Pulse_A = 0x2 | 0x20,
        Pulse_B = 0x2 | 0x04,
        Pulse_AB = 0x2 | 0x20 | 0x04,
        Calibration = 0xFC
    }

    [Flags]
    public enum BatteryState
    {
        POWER_SUPPLY_STATUS_DISCHARGING = 0x0,
        POWER_SUPPLY_STATUS_CHARGING = 0x1,
        POWER_SUPPLY_STATUS_FULL = 0x2,
        POWER_SUPPLY_STATUS_NOT_CHARGING = 0xb,
        POWER_SUPPLY_STATUS_ERROR = 0xf,
        POWER_SUPPLY_TEMP_OR_VOLTAGE_OUT_OF_RANGE = 0xa,
        POWER_SUPPLY_STATUS_UNKNOWN = 0x0
    }

    class DSGyro
    {
        public int Pitch { get; set; }
        public int Yaw { get; set; }
        public int Roll { get; set; }

        public DSGyro()
        {
            Pitch = 0;
            Yaw = 0;
            Roll = 0;
        }
    }

    class DSAccelerometer
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public DSAccelerometer()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }
    }

    class DSBattery
    {
        public BatteryState State { get; set; }
        public int Level { get; set; }

        public DSBattery()
        {
            State = BatteryState.POWER_SUPPLY_STATUS_UNKNOWN;
            Level = 0;
        }
    }

    class DSTouchpad
    {
        public bool IsActive { get; set; }
        public int ID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public DSTouchpad()
        {
            IsActive = false;
            ID = 0;
            X = 0;
            Y = 0;
        }
    }


    class DSState
    {
        public bool square, triangle, circle, cross;
        public bool DpadUp, DpadDown, DpadLeft, DpadRight;
        public bool L1, L3, R1, R3, R2Btn, L2Btn;
        public byte L2, R2;
        public bool share, options, ps, touch1, touch2, touchBtn, touchRight, touchLeft;
        public bool touchFinger1, touchFinger2;
        public bool micBtn;
        public int RX, RY, LX, LY;
        public DSTouchpad trackPadTouch0 = new DSTouchpad();
        public DSTouchpad trackPadTouch1 = new DSTouchpad();
        public DSGyro gyro = new DSGyro();
        public DSAccelerometer accelerometer = new DSAccelerometer();

        public void SetDPadState(int dpad_state)
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
                    DpadLeft = false;
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
    }

    class DSLight
    {
        public Brightness brightness = Brightness.low;
        public PlayerID playerNumber = PlayerID.PLAYER_1;
        public LedOptions ledOption = LedOptions.Both;
        public PulseOptions pulseOptions = PulseOptions.Off;
        public byte[] TouchpadColor = new byte[3];

        public void SetLEDOption(LedOptions option)
        {
            if (!Enum.IsDefined(typeof(LedOptions), option))
                throw new ArgumentException("LEDOption type is invalid");
            ledOption = option;
        }

        public void SetPulseOption(PulseOptions option)
        {
            if (!Enum.IsDefined(typeof(PulseOptions), option))
                throw new ArgumentException("PulseOption type is invalid");
            pulseOptions = option;
        }

        public void SetBrightness(Brightness brightness)
        {
            if (!Enum.IsDefined(typeof(Brightness), brightness))
                throw new ArgumentException("Brightness type is invalid");
            this.brightness = brightness;
        }

        public void SetPlayerID(PlayerID player)
        {
            if (!Enum.IsDefined(typeof(PlayerID), player))
                throw new ArgumentException("PlayerID type is invalid");
            playerNumber = player;
        }

        public void SetColor(int r, int g, int b)
        {
            if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
                throw new ArgumentException("Color channels should be between 0 and 255");

            TouchpadColor[0] = (byte)r;
            TouchpadColor[1] = (byte)g;
            TouchpadColor[2] = (byte)b;
        }

        public void SetColor((int, int, int) color)
        {
            SetColor(color.Item1, color.Item2, color.Item3);
        }
    }

    class DSAudio
    {
        public int microphone_mute;
        public int microphone_led;

        public void SetMicrophoneLED(bool value)
        {
            microphone_led = value ? 1 : 0;
        }

        public void SetMicrophoneState(bool state)
        {
            SetMicrophoneLED(state);
            microphone_mute = state ? 1 : 0;
        }
    }

    class DSTrigger
    {
        public TriggerModes mode = TriggerModes.Off;
        public int[] forces = new int[7];

        public void SetForce(int forceID, int force)
        {
            if (forceID < 0 || forceID > 6)
                throw new ArgumentException("Force ID should be between 0 and 6");
            forces[forceID] = force;
        }

        public void SetMode(TriggerModes mode)
        {
            if (!Enum.IsDefined(typeof(TriggerModes), mode))
                throw new ArgumentException("Trigger mode type is invalid");
            this.mode = mode;
        }
    }

    internal static class CRC32Utils
    {
        private static readonly uint[] ChecksumTableCRC32 =
        {
            0xd202ef8d, 0xa505df1b, 0x3c0c8ea1, 0x4b0bbe37, 0xd56f2b94, 0xa2681b02, 0x3b614ab8, 0x4c667a2e,
            0xdcd967bf, 0xabde5729, 0x32d70693, 0x45d03605, 0xdbb4a3a6, 0xacb39330, 0x35bac28a, 0x42bdf21c,
            0xcfb5ffe9, 0xb8b2cf7f, 0x21bb9ec5, 0x56bcae53, 0xc8d83bf0, 0xbfdf0b66, 0x26d65adc, 0x51d16a4a,
            0xc16e77db, 0xb669474d, 0x2f6016f7, 0x58672661, 0xc603b3c2, 0xb1048354, 0x280dd2ee, 0x5f0ae278,
            0xe96ccf45, 0x9e6bffd3, 0x762ae69, 0x70659eff, 0xee010b5c, 0x99063bca, 0xf6a70, 0x77085ae6,
            0xe7b74777, 0x90b077e1, 0x9b9265b, 0x7ebe16cd, 0xe0da836e, 0x97ddb3f8, 0xed4e242, 0x79d3d2d4,
            0xf4dbdf21, 0x83dcefb7, 0x1ad5be0d, 0x6dd28e9b, 0xf3b61b38, 0x84b12bae, 0x1db87a14, 0x6abf4a82,
            0xfa005713, 0x8d076785, 0x140e363f, 0x630906a9, 0xfd6d930a, 0x8a6aa39c, 0x1363f226, 0x6464c2b0,
            0xa4deae1d, 0xd3d99e8b, 0x4ad0cf31, 0x3dd7ffa7, 0xa3b36a04, 0xd4b45a92, 0x4dbd0b28, 0x3aba3bbe,
            0xaa05262f, 0xdd0216b9, 0x440b4703, 0x330c7795, 0xad68e236, 0xda6fd2a0, 0x4366831a, 0x3461b38c,
            0xb969be79, 0xce6e8eef, 0x5767df55, 0x2060efc3, 0xbe047a60, 0xc9034af6, 0x500a1b4c, 0x270d2bda,
            0xb7b2364b, 0xc0b506dd, 0x59bc5767, 0x2ebb67f1, 0xb0dff252, 0xc7d8c2c4, 0x5ed1937e, 0x29d6a3e8,
            0x9fb08ed5, 0xe8b7be43, 0x71beeff9, 0x6b9df6f, 0x98dd4acc, 0xefda7a5a, 0x76d32be0, 0x1d41b76,
            0x916b06e7, 0xe66c3671, 0x7f6567cb, 0x862575d, 0x9606c2fe, 0xe101f268, 0x7808a3d2, 0xf0f9344,
            0x82079eb1, 0xf500ae27, 0x6c09ff9d, 0x1b0ecf0b, 0x856a5aa8, 0xf26d6a3e, 0x6b643b84, 0x1c630b12,
            0x8cdc1683, 0xfbdb2615, 0x62d277af, 0x15d54739, 0x8bb1d29a, 0xfcb6e20c, 0x65bfb3b6, 0x12b88320,
            0x3fba6cad, 0x48bd5c3b, 0xd1b40d81, 0xa6b33d17, 0x38d7a8b4, 0x4fd09822, 0xd6d9c998, 0xa1def90e,
            0x3161e49f, 0x4666d409, 0xdf6f85b3, 0xa868b525, 0x360c2086, 0x410b1010, 0xd80241aa, 0xaf05713c,
            0x220d7cc9, 0x550a4c5f, 0xcc031de5, 0xbb042d73, 0x2560b8d0, 0x52678846, 0xcb6ed9fc, 0xbc69e96a,
            0x2cd6f4fb, 0x5bd1c46d, 0xc2d895d7, 0xb5dfa541, 0x2bbb30e2, 0x5cbc0074, 0xc5b551ce, 0xb2b26158,
            0x4d44c65, 0x73d37cf3, 0xeada2d49, 0x9ddd1ddf, 0x3b9887c, 0x74beb8ea, 0xedb7e950, 0x9ab0d9c6,
            0xa0fc457, 0x7d08f4c1, 0xe401a57b, 0x930695ed, 0xd62004e, 0x7a6530d8, 0xe36c6162, 0x946b51f4,
            0x19635c01, 0x6e646c97, 0xf76d3d2d, 0x806a0dbb, 0x1e0e9818, 0x6909a88e, 0xf000f934, 0x8707c9a2,
            0x17b8d433, 0x60bfe4a5, 0xf9b6b51f, 0x8eb18589, 0x10d5102a, 0x67d220bc, 0xfedb7106, 0x89dc4190,
            0x49662d3d, 0x3e611dab, 0xa7684c11, 0xd06f7c87, 0x4e0be924, 0x390cd9b2, 0xa0058808, 0xd702b89e,
            0x47bda50f, 0x30ba9599, 0xa9b3c423, 0xdeb4f4b5, 0x40d06116, 0x37d75180, 0xaede003a, 0xd9d930ac,
            0x54d13d59, 0x23d60dcf, 0xbadf5c75, 0xcdd86ce3, 0x53bcf940, 0x24bbc9d6, 0xbdb2986c, 0xcab5a8fa,
            0x5a0ab56b, 0x2d0d85fd, 0xb404d447, 0xc303e4d1, 0x5d677172, 0x2a6041e4, 0xb369105e, 0xc46e20c8,
            0x72080df5, 0x50f3d63, 0x9c066cd9, 0xeb015c4f, 0x7565c9ec, 0x262f97a, 0x9b6ba8c0, 0xec6c9856,
            0x7cd385c7, 0xbd4b551, 0x92dde4eb, 0xe5dad47d, 0x7bbe41de, 0xcb97148, 0x95b020f2, 0xe2b71064,
            0x6fbf1d91, 0x18b82d07, 0x81b17cbd, 0xf6b64c2b, 0x68d2d988, 0x1fd5e91e, 0x86dcb8a4, 0xf1db8832,
            0x616495a3, 0x1663a535, 0x8f6af48f, 0xf86dc419, 0x660951ba, 0x110e612c, 0x88073096, 0xff000000
        };

        private const uint HASH_SEED = 0xeada2d49;

        public static uint ComputeCRC32(byte[] byteData, int size)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException("In ComputeCRC32: the Size is negative.");
            uint hashResult = HASH_SEED;
            for (int i = 0; i < size; ++i)
                hashResult = ChecksumTableCRC32[(hashResult & 0xFF) ^ byteData[i]] ^ (hashResult >> 8);
            return hashResult;
        }
    }
}
