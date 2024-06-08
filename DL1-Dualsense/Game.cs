using Swed64;

namespace DL1_Dualsense
{
    public class Game
    {
        private Swed swed = new Swed("DyingLightGame");
        private IntPtr engineModuleBase;
        private IntPtr gameModuleBase;
        private int lastUV = 0;

        public Game()
        {
            engineModuleBase = swed.GetModuleBase("engine_x64_rwdi.dll"); 
            gameModuleBase = swed.GetModuleBase("gamedll_x64_rwdi.dll");
        }

        public float getHP()
        {
            float hp = swed.ReadFloat(swed.ReadPointer(gameModuleBase, 0x01BAFA68, 0x9D8, 0x40, 0x480, 0x8E0, 0x18) + 0x92C);
            return hp;
        }

        public short getChaseLevel()
        {
            short chase = swed.ReadShort(swed.ReadPointer(gameModuleBase, 0x01C10B60, 0x20, 0x28, 0x28, 0x20) + 0x44);
            return chase;
        }

        public bool isPlayerInMenu() // not MAIN menu
        {
            short pause = swed.ReadShort(swed.ReadPointer(gameModuleBase, 0x01BAFA68, 0xA30) + 0x30);
            switch (pause)
            {
                case 255:
                    return true;
                default:
                    return false;
            }
        }

        public bool isUVOn()
        {
            short uv = swed.ReadShort(swed.ReadPointer(gameModuleBase, offsets: new[] { 0x01C15268, 0x780, 0x338, 0x1068, 0x90, 0x16A0, 0x10, 0x10, 0x1A8 }) + 0x54);  // <--- for epic games store
            short uv2 = swed.ReadShort(swed.ReadPointer(engineModuleBase, offsets: new[] { 0x00A0D1E0, 0x4E8, 0xB8, 0x60, 0x10, 0x10, 0x10, 0x20 }) + 0x3C);  // <--- for steam
            if (uv == 1 || uv2 == 1)
                return true;
            else
                return false;
        }

        public bool isUVRecharging()
        {
            short uv = swed.ReadShort(swed.ReadPointer(gameModuleBase, offsets: new[] { 0x01C15268, 0x780, 0x338, 0x1068, 0x90, 0x16A0, 0x10, 0x10, 0x1A8 }) + 0x54);  // <--- for epic games store
            short uv2 = swed.ReadShort(swed.ReadPointer(engineModuleBase, offsets: new[] { 0x00A0D1E0, 0x4E8, 0xB8, 0x60, 0x10, 0x10, 0x10, 0x20 }) + 0x3C);  // <--- for steam
            if (uv == 256 || uv2 == 256)
                return true;         
            else
                return false;
        }

        public bool isFlashlightOn()
        {
            short flashlight = swed.ReadShort(swed.ReadPointer(gameModuleBase, offsets: new[] { 0x01BAFA68, 0x9D8, 0x40, 0x480, 0x8E0, 0x18, 0x250, 0x188 }) + 0x24);
            if (flashlight == 0)
                return false;
            else
                return true;
        }

        public bool isMicrophoneOn()
        {
            short microphone = swed.ReadShort(swed.ReadPointer(gameModuleBase, 0x01C152F8, 0x50) + 0x40);
            if (microphone == 1)
                return true;
            else
                return false;
        }

        public void setMicrophone(bool onORoff)
        {
            switch (onORoff)
            {
                case true:
                    swed.WriteShort(swed.ReadPointer(gameModuleBase, 0x01C152F8, 0x50) + 0x40, 1);
                    break;
                case false:
                    swed.WriteShort(swed.ReadPointer(gameModuleBase, 0x01C152F8, 0x50) + 0x40, 0);
                    break;
            }
        }

        public int getWeaponType()
        {
            short animation = swed.ReadShort(swed.ReadPointer(gameModuleBase, offsets: new[] { 0x01BAFA68, 0x10C0, 0x48, 0x8, 0x320, 0x1A8, 0x30 }) + 0x30);
            return animation;
        }
    }
}
