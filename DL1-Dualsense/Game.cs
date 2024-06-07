using Swed64;

namespace DL1_Dualsense
{
    public class Game
    {
        private Swed swed = new Swed("DyingLightGame");

        public float getHP()
        {
            IntPtr moduleBase = swed.GetModuleBase("gamedll_x64_rwdi.dll");
            float hp = swed.ReadFloat(swed.ReadPointer(moduleBase, 0x01BAFA68, 0x9D8, 0x40, 0x480, 0x8E0, 0x18) + 0x92C);
            return hp;
        }

        public short getChaseLevel()
        {
            IntPtr moduleBase = swed.GetModuleBase("gamedll_x64_rwdi.dll");
            short chase = swed.ReadShort(swed.ReadPointer(moduleBase, 0x01C10B60, 0x20, 0x28, 0x28, 0x20) + 0x44);
            return chase;
        }

        public bool isPlayerInMenu() // not MAIN menu
        {
            IntPtr moduleBase = swed.GetModuleBase("gamedll_x64_rwdi.dll");
            short pause = swed.ReadShort(swed.ReadPointer(moduleBase, 0x01BAFA68, 0xA30) + 0x30);
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
            IntPtr moduleBase = swed.GetModuleBase("engine_x64_rwdi.dll");
            short uv = swed.ReadShort(swed.ReadPointer(moduleBase, offsets: new[] { 0x00A0D1E0, 0x4E8, 0xB8, 0x60, 0x10, 0x10, 0x10, 0x20 }) + 0x3C);
            if (uv == 1)
                return true;
            else
                return false;
        }

        public bool isUVRecharging()
        {
            IntPtr moduleBase = swed.GetModuleBase("engine_x64_rwdi.dll");
            short uv = swed.ReadShort(swed.ReadPointer(moduleBase, offsets: new[] { 0x00A0D1E0, 0x4E8, 0xB8, 0x60, 0x10, 0x10, 0x10, 0x20 }) + 0x3C);
            if (uv == 256)
                return true;
            else
                return false;
        }

        public bool isFlashlightOn()
        {
            IntPtr moduleBase = swed.GetModuleBase("gamedll_x64_rwdi.dll");
            short flashlight = swed.ReadShort(swed.ReadPointer(moduleBase, offsets: new[] { 0x01BAFA68, 0x9D8, 0x40, 0x480, 0x8E0, 0x18, 0x250, 0x188 }) + 0x24);
            if (flashlight == 0)
                return false;
            else
                return true;
        }

        public bool isMicrophoneOn()
        {
            IntPtr moduleBase = swed.GetModuleBase("gamedll_x64_rwdi.dll");
            short microphone = swed.ReadShort(swed.ReadPointer(moduleBase, 0x01C152F8, 0x50) + 0x40);
            if (microphone == 1)
                return true;
            else
                return false;
        }

        public void setMicrophone(bool onORoff)
        {
            IntPtr moduleBase = swed.GetModuleBase("gamedll_x64_rwdi.dll");
            switch (onORoff)
            {
                case true:
                    swed.WriteShort(swed.ReadPointer(moduleBase, 0x01C152F8, 0x50) + 0x40, 1);
                    break;
                case false:
                    swed.WriteShort(swed.ReadPointer(moduleBase, 0x01C152F8, 0x50) + 0x40, 0);
                    break;
            }
        }

        public int getWeaponType()
        {
            IntPtr moduleBase = swed.GetModuleBase("engine_x64_rwdi.dll");
            short animation = swed.ReadShort(swed.ReadPointer(moduleBase, offsets: new[] { 0x00A0D1E0, 0x4E8, 0xB8, 0x50, 0x2E0, 0x1A8, 0x30 }) + 0x30);
            Console.WriteLine(animation);
            return animation;
        }
    }
}
