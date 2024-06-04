using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Security.Cryptography.X509Certificates;

namespace DL1_Dualsense
{
    public class HapticFeedback : IDisposable
    {
        private const string deviceId = "DualSense Wireless Controller";
        private static MMDevice device;
        public bool initialized = false;
        public bool forceStop = false;
        public bool Playing = false;
        public HapticEffect hapticEffect = new HapticEffect();


        public HapticFeedback()
        {
            device = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.All).FirstOrDefault(d => d.DeviceFriendlyName == deviceId);
            if (device == null || device.State == DeviceState.NotPresent || device.State == DeviceState.Unplugged)
            {
                Console.WriteLine("Device not found!");
                return;
            }

            initialized = true;
        }

        public void PlayHaptics(float leftSpeakerVolume, float rightSpeakerVolume, float leftHapticVolume, float rightHapticVolume)
        {
            try
            {
                if (hapticEffect.currentEffect != string.Empty)
                {
                    forceStop = false;
                    WasapiOut playbackStream = new WasapiOut(device, AudioClientShareMode.Shared, true, 100);
                    WaveFileReader waveProvider = new WaveFileReader(folder + hapticEffect.currentEffect);
                    MultiplexingWaveProvider multiplexingWaveProvider = new MultiplexingWaveProvider(new IWaveProvider[] { waveProvider }, 4);
                    multiplexingWaveProvider.ConnectInputToOutput(0, 0);
                    multiplexingWaveProvider.ConnectInputToOutput(0, 1);
                    multiplexingWaveProvider.ConnectInputToOutput(0, 2);
                    multiplexingWaveProvider.ConnectInputToOutput(0, 3);
                    playbackStream.Init(multiplexingWaveProvider);
                    playbackStream.AudioStreamVolume.SetChannelVolume(0, leftSpeakerVolume);
                    playbackStream.AudioStreamVolume.SetChannelVolume(1, rightSpeakerVolume);
                    playbackStream.AudioStreamVolume.SetChannelVolume(2, leftHapticVolume);
                    playbackStream.AudioStreamVolume.SetChannelVolume(3, rightHapticVolume);
                    playbackStream.Play();
                    Playing = true;
                    while (forceStop == false && playbackStream.PlaybackState == PlaybackState.Playing) { Thread.Sleep(5); }
                    Playing = false;
                    playbackStream.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


        }

        private static string folder = @"haptics\";
        public void Dispose()
        {
            device.AudioClient.Dispose();
            device.Dispose();
        }
    }

    public class HapticEffect
    {
        public void SetEffect(string newEffect)
        {
            currentEffect = newEffect;
        }

        public void RemoveEffect()
        {
            currentEffect = string.Empty;
        }

        public string currentEffect = string.Empty;
        public static readonly string UIchangeSelection = "uichangeselection.wav";
        public static readonly string UIselect1 = "uiselect.wav";
        public static readonly string UIselect2 = "uiselect2.wav";
        public static readonly string FlashlightOn = "torch_on_0.wav";
        public static readonly string FlashlightOff = "torch_off_0.wav";
        public static readonly string UVFlashlightStart = "uv_flashlight_start_00_0.wav";
        public static readonly string UVFlashlightDrained = "uv_flashlight_drained_0.wav";
        public static readonly string UVFlashlightFailed = "uv_flashlight_start_failed_0.wav";
        public static readonly string UVFlashlightStop = "uv_flashlight_stop_00_0.wav";
        public static readonly string UVFlashlightWorking = "uv_flashlight_working_loop_0.wav";
        public static readonly string Heavy1 = "empty_axe_heavy_right_00_0.wav";
        public static readonly string Heavy2 = "empty_axe_heavy_right_01_0.wav";
        public static readonly string Heavy3 = "empty_axe_heavy_right_02_0.wav";
        public static readonly string Heavy4 = "empty_hammer_heavy_00_0.wav";
        public static readonly string Heavy5 = "empty_hammer_heavy_01_0.wav";
        public static readonly string Heavy6 = "empty_hammer_heavy_02_0.wav";
        public static readonly string Heavy7 = "empty_hammer_heavy_03_0.wav";
        public static readonly string CrossbowShot = "crossbow_shot_00_0.wav";
        public static readonly string Machete1 = "empty_machete_left_00_0.wav";
        public static readonly string Machete2 = "empty_machete_left_01_0.wav";
        public static readonly string Machete3 = "empty_machete_right_00_0.wav";
        public static readonly string Machete4 = "empty_machete_right_01_0.wav";
        public static readonly string Blunt1 = "empty_wrench_left_00_0.wav";
        public static readonly string Blunt2 = "empty_wrench_left_01_0.wav";
        public static readonly string Blunt3 = "empty_wrench_right_00_0.wav";
        public static readonly string Blunt4 = "empty_wrench_right_01_0.wav";
        public static readonly string Blunt5 = "empty_hammer_left_00_0.wav";
        public static readonly string Blunt6 = "empty_hammer_right_00_0.wav";
        public static readonly string Knife1 = "empty_knife_00_0.wav";
        public static readonly string Knife2 = "empty_knife_01_0.wav";
        public static readonly string Knife3 = "empty_knife_02_0.wav";
        public static readonly string Knife4 = "empty_knife_03_0.wav";
        public static readonly string Fists1 = "empty_fists_left_00_0.wav";
        public static readonly string Fists2 = "empty_fists_left_01_0.wav";
        public static readonly string Fists3 = "empty_fists_right_00_0.wav";
        public static readonly string Fists4 = "empty_fists_right_01_0.wav";
        public static readonly string Chainsaw = "chainsaw.wav";
        public static readonly string RifleShot1 = "m4_shot_00_0.wav";
        public static readonly string RifleShot2 = "m4_shot_01_0.wav";
        public static readonly string RifleShot3 = "m4_shot_02_0.wav";
        public static readonly string GunEmpty = "rmngtn_empty_0.wav";
        public static readonly string SmgShot1 = "submachinea_shot_00_0.wav";
        public static readonly string SmgShot2 = "submachinea_shot_01_0.wav";
        public static readonly string SmgShot3 = "submachinea_shot_02_0.wav";
        public static readonly string BowShot = "bow_shot_0.wav";
        public static readonly string BowReload = "bow_reload_0.wav";
        public static readonly string BowLoad = "bow_load_0.wav";
        public static readonly string PickupItem = "pickup_default_0.wav";
        public static readonly string Camouflage = "player_skill_camouflage_00_0.wav";
        public static readonly string VanOpening = "van_opening_0.wav";
        public static readonly string CarTrunkOpening = "sedan_trunk_opening_0.wav";
        public static readonly string Remote = "pursuit_remote_activate_0.wav";
        public static readonly string Dropkick = "wrestling_kick_empty_0.wav";
        public static readonly string DropkickEnd = "wrestling_stand_up_0.wav";
        public static readonly string LandingOnCar = "landing_heavy_car_00_0.wav";
        public static readonly string LandingOnTrash = "landing_damper_trashbags_0.wav";
        public static readonly string SafetyRoll = "landing_damper_roll_0.wav";
        public static readonly string Medkit = "player_self_healing_bandage_0.wav";
        public static readonly string ClimbPipeLeft = "climb_pipe_left_00_0.wav";
        public static readonly string ClimbPipeRight = "climb_pipe_right_00_0.wav";
        public static readonly string Slide = "player_slide_loop_0.wav";
        public static readonly string PipeRotating = "climb_pipe_rotate_02_0.wav";
        public static readonly string GroundPound = "groundpound.wav";
        public static readonly string HangOnEdge1 = "climb_hang_shimmy_00_0.wav";
        public static readonly string HangOnEdge2 = "climb_hang_shimmy_01_0.wav";
        public static readonly string HangOnEdge3 = "climb_hang_shimmy_02_0.wav";
        public static readonly string LandOnDirt = "landing_weak_dirt_00_0.wav";
        public static readonly string Looting = "looting_0.wav";
        public static readonly string OpeningFridge = "fridge_a_opening_0.wav";
        public static readonly string OpeningWoodenDoor = "barn_platform_opening_0.wav";
        public static readonly string WeaponRepair = "player_repair_weapon_0.wav";
        public static readonly string KneeFinisher = "player_grab_finish_knee_00_0.wav";
        public static readonly string ElbowFinisher = "elbowfinisher.wav";
        public static readonly string NeckGrabFinisher = "neckgrabfinisher.wav";
        public static readonly string NeckFinisher = "player_grab_finish_neck_0.wav";
        public static readonly string WatchBeep = "watch_beep_00_0.wav";
        public static readonly string Zipline = "zip_line_loop_0.wav";
        public static readonly string ZiplineStart = "zip_line_start_0.wav";
        public static readonly string OpeningChest = "chest_opening_0.wav";
        public static readonly string ClimbLadderLeft = "climb_metal_ladder_left_00_0.wav";
        public static readonly string ClimbLadderRight = "climb_metal_ladder_left_00_0.wav";
        public static readonly string GrapplingHookStart = "weapon_rope_throw_0.wav";
        public static readonly string GrapplingHookEnd = "weapon_rope_throw_back_0.wav";
        public static readonly string PistolShot = "beretta_shot_00_0.wav";
        public static readonly string RevolverShot = "rmngtn_shot_00_0.wav";
        public static readonly string ShotgunShot = "shotgunc_shot_00_0.wav";
    }
}
