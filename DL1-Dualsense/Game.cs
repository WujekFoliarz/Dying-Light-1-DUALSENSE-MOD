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
            // I was unable to find a consistant UV Flashlight pointer that works 100% of the time.
            // I'll just have to do it this way, for now at least

            IntPtr moduleBase = swed.GetModuleBase("gamedll_x64_rwdi.dll");
            short uv1 = swed.ReadShort(swed.ReadPointer(moduleBase, 0x01D0E8E0, 0x410, 0x48, 0x1A8) + 0x54);
            short uv2 = swed.ReadShort(swed.ReadPointer(moduleBase, 0x01C399E0, 0x18, 0x1A8) + 0x54);
            short uv3 = swed.ReadShort(swed.ReadPointer(moduleBase, 0x01C15308, 0x390, 0x118, 0x38, 0xDE0, 0x2E0) + 0x54);

            if (uv1 == 0 && uv2 == 0)
            {
                if (uv3 == 1)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }

        public bool isUVRecharging()
        {
            IntPtr moduleBase = swed.GetModuleBase("gamedll_x64_rwdi.dll");
            short uv1 = swed.ReadShort(swed.ReadPointer(moduleBase, 0x01D0E8E0, 0x410, 0x48, 0x1A8) + 0x54);
            short uv2 = swed.ReadShort(swed.ReadPointer(moduleBase, 0x01C399E0, 0x18, 0x1A8) + 0x54);
            short uv3 = swed.ReadShort(swed.ReadPointer(moduleBase, 0x01C15308, 0x390, 0x118, 0x38, 0xDE0, 0x2E0) + 0x54);
            if (uv1 == 256 || uv2 == 256 || uv3 == 256)
                return true;
            else
                return false;
        }

        public bool isFlashlightOn()
        {
            IntPtr moduleBase = swed.GetModuleBase("gamedll_x64_rwdi.dll");
            short flashlight1 = swed.ReadShort(swed.ReadPointer(moduleBase, 0x01C15308, 0xC8, 0x188, 0x38, 0xDA0, 0x2E0) + 0x24);
            short flashlight2 = swed.ReadShort(swed.ReadPointer(moduleBase, 0x01C15308, 0x390, 0x118, 0x38, 0xDE0, 0x2E0) + 0x24);

            if (flashlight1 == 0 && flashlight2 == 0)
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
            short animation = swed.ReadShort(swed.ReadPointer(moduleBase, 0x00A3F608, 0x208, 0x90, 0x80, 0x2E0, 0x1A8, 0x8) + 0x24);
            return animation;
        }
    }
}
