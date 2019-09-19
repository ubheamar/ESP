using AssaultCubeESP.structs;
using Binarysharp.MemoryManagement;
using Binarysharp.MemoryManagement.Assembly.CallingConvention;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCubeESP.util
{
    public class Memory : MemorySharp
    {
        public Memory(System.Diagnostics.Process process) : base(process)
        {
        }

        public Memory(int processId) : base(processId)
        {
        }
        /// <summary>
        /// Read 3 consecutive floats into x,y,z of a Vector
        /// </summary>
        public Vector3 ReadVector3(IntPtr baseAddress)
        {
            //3 floats contiguously in memory
            byte[] buffer = new byte[3 * 4];

            //read memory into buffer
            buffer = Read<byte>(baseAddress, buffer.Length, false);

            //convert bytes to floats
            Vector3 vec = new Vector3();
            vec.X = BitConverter.ToSingle(buffer, (0 * 4));
            vec.Y = BitConverter.ToSingle(buffer, (1 * 4));
            vec.Z = BitConverter.ToSingle(buffer, (2 * 4));
            return vec;
        }

        /// <summary>
        /// Reads 16 consecutive floats into a Matrix
        /// </summary>
        public structs.Matrix ReadMatrix( long baseAddress)
        {
            //float matrix[16]; 16-value array laid out contiguously in memory       
            byte[] buffer = new byte[16 * 4];

            //read memory into buffer

            buffer = Read<byte>((IntPtr)baseAddress, buffer.Length, false);


            //convert bytes to floats
            structs.Matrix mat = new structs.Matrix();
            mat.m11 = BitConverter.ToSingle(buffer, (0 * 4));
            mat.m12 = BitConverter.ToSingle(buffer, (1 * 4));
            mat.m13 = BitConverter.ToSingle(buffer, (2 * 4));
            mat.m14 = BitConverter.ToSingle(buffer, (3 * 4));

            mat.m21 = BitConverter.ToSingle(buffer, (4 * 4));
            mat.m22 = BitConverter.ToSingle(buffer, (5 * 4));
            mat.m23 = BitConverter.ToSingle(buffer, (6 * 4));
            mat.m24 = BitConverter.ToSingle(buffer, (7 * 4));

            mat.m31 = BitConverter.ToSingle(buffer, (8 * 4));
            mat.m32 = BitConverter.ToSingle(buffer, (9 * 4));
            mat.m33 = BitConverter.ToSingle(buffer, (10 * 4));
            mat.m34 = BitConverter.ToSingle(buffer, (11 * 4));

            mat.m41 = BitConverter.ToSingle(buffer, (12 * 4));
            mat.m42 = BitConverter.ToSingle(buffer, (13 * 4));
            mat.m43 = BitConverter.ToSingle(buffer, (14 * 4));
            mat.m44 = BitConverter.ToSingle(buffer, (15 * 4));
            return mat;
        }
    }
}
