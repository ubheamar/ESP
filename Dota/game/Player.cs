using AssaultCubeESP.util;
using Binarysharp.MemoryManagement;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCubeESP.game
{
    class Player
    {
        IntPtr _pointerPlayer = IntPtr.Zero;
        private Memory _memory;
        public Player(IntPtr pointerPlayer, Memory memory)
        {
            this._pointerPlayer = pointerPlayer;
            this._memory = memory;
        }
        public string Name { get { return "Player name"; } }

        public int Team { get { return _memory[_pointerPlayer + AppConstant.OffTeam, false].Read<int>(); } }

        public int Health
        {
            get { return _memory[_pointerPlayer + AppConstant.OffHealth,false].Read<int>(); }
        }
        public Vector3 PositionHead => _memory.ReadVector3(_pointerPlayer + AppConstant.OffHeadPos);

        public Vector3 PositionFoot
        {
            get { return _memory.ReadVector3(_pointerPlayer + AppConstant.OffFootPos); }

        }
    }
}
