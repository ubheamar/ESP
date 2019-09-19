using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCubeESP.game
{
    public class AppConstant
    {
        public static string ProcessName = "LdBoxHeadless";
        public static int Fps = 60;
        public static IntPtr AddBaseGame = new IntPtr(0x0050F4E8);
        public static int AddViewMatrix = 0x00501AE8;
        public static int OffPlayerEntity = 0x0C;
        public static int OffPlayerArray = 0x10;
        public static int OffNumPlayers = 0x18; //size of ptrPlayerArray
        //player variables
        public static int OffName = 0x0224;
        public static int OffHealth = 0xF8;
        public static int OffTeam = 0x032C;
        public static int OffHeadPos = 0x04;
        public static int OffFootPos = 0x34;
    }
}
