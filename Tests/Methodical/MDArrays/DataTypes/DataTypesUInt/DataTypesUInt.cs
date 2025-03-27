using System;

namespace MDArrays.@uint
{
    public struct VT
    {
        public uint[,] uint2darr;
        public uint[,,] uint3darr;
        public uint[,] uint2darr_b;
        public uint[,,] uint3darr_b;
    }

    public class CL
    {
        public readonly uint[,] uint2darr = { { 0, 1 }, { 0, 0 } };
        public readonly uint[,,] uint3darr = { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
        public readonly uint[,] uint2darr_b = { { 0, 49 }, { 0, 0 } };
        public readonly uint[,,] uint3darr_b = { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };
    }

    internal static class DataTypesuint
    {
        static readonly uint[,] uint2darr = { { 0, 1 }, { 0, 0 } };
        static readonly uint[,,] uint3darr = { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
        static readonly uint[,] uint2darr_b = { { 0, 49 }, { 0, 0 } };
        static readonly uint[,,] uint3darr_b = { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };

        static readonly uint[][,] ja1 = new uint[2][,];
        static readonly uint[][,,] ja2 = new uint[2][,,];
        static readonly uint[][,] ja1_b = new uint[2][,];
        static readonly uint[][,,] ja2_b = new uint[2][,,];

        public static int Main()
        {
            VT vt1;
            vt1.uint2darr = new uint[,] { { 0, 1 }, { 0, 0 } };
            vt1.uint3darr = new uint[,,] { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
            vt1.uint2darr_b = new uint[,] { { 0, 49 }, { 0, 0 } };
            vt1.uint3darr_b = new uint[,,] { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };

            CL cl1 = new();

            ja1[0] = new uint[,] { { 0, 1 }, { 0, 0 } };
            ja2[1] = new uint[,,] { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
            ja1_b[0] = new uint[,] { { 0, 49 }, { 0, 0 } };
            ja2_b[1] = new uint[,,] { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };

            int result = UInt32ToBoolTests(vt1, cl1);
            if (result != 0) return result;

            result = UInt32ToByteTests(vt1, cl1);
            if (result != 0) return result;

            result = UInt32ToCharTests(vt1, cl1);
            if (result != 0) return result;

            result = UInt32ToInt32Tests(vt1, cl1);
            if (result != 0) return result;

            result = UInt32ToSByteTests(vt1, cl1);
            if (result != 0) return result;

            result = UInt32ToInt16Tests(vt1, cl1);
            if (result != 0) return result;

            result = UInt32Tests(vt1, cl1);
            if (result != 0) return result;

            result = UInt32ToUInt16Tests(vt1, cl1);
            if (result != 0) return result;

            return 0;
        }

        private static int UInt32ToBoolTests(VT vt1, CL cl1)
        {
            const int uint32ToBoolErrorBase = 100;

            bool expected = true;

            // 2d
            if (expected != Convert.ToBoolean(uint2darr[0, 1])) return uint32ToBoolErrorBase + 1;
            if (expected != Convert.ToBoolean(vt1.uint2darr[0, 1])) return uint32ToBoolErrorBase + 2;
            if (expected != Convert.ToBoolean(cl1.uint2darr[0, 1])) return uint32ToBoolErrorBase + 3;
            if (expected != Convert.ToBoolean(ja1[0][0, 1])) return uint32ToBoolErrorBase + 4;

            // 3d
            if (expected != Convert.ToBoolean(uint3darr[1, 0, 1])) return uint32ToBoolErrorBase + 5;
            if (expected != Convert.ToBoolean(vt1.uint3darr[1, 0, 1])) return uint32ToBoolErrorBase + 6;
            if (expected != Convert.ToBoolean(cl1.uint3darr[1, 0, 1])) return uint32ToBoolErrorBase + 7;
            if (expected != Convert.ToBoolean(ja2[1][1, 0, 1])) return uint32ToBoolErrorBase + 8;

            return 0;
        }

        private static int UInt32ToByteTests(VT vt1, CL cl1)
        {
            const int int16ToByteErrorBase = 200;

            byte expected = 1;

            // 2d
            if (expected != Convert.ToByte(uint2darr[0, 1])) return int16ToByteErrorBase + 1;
            if (expected != Convert.ToByte(vt1.uint2darr[0, 1])) return int16ToByteErrorBase + 2;
            if (expected != Convert.ToByte(cl1.uint2darr[0, 1])) return int16ToByteErrorBase + 3;
            if (expected != Convert.ToByte(ja1[0][0, 1])) return int16ToByteErrorBase + 4;

            // 3d
            if (expected != Convert.ToByte(uint3darr[1, 0, 1])) return int16ToByteErrorBase + 5;
            if (expected != Convert.ToByte(vt1.uint3darr[1, 0, 1])) return int16ToByteErrorBase + 6;
            if (expected != Convert.ToByte(cl1.uint3darr[1, 0, 1])) return int16ToByteErrorBase + 7;
            if (expected != Convert.ToByte(ja2[1][1, 0, 1])) return int16ToByteErrorBase + 8;

            return 0;
        }

        private static int UInt32ToCharTests(VT vt1, CL cl1)
        {
            const int uint32ToCharErrorBase = 300;

            char expected = '1';

            // 2d
            if (expected != Convert.ToChar(uint2darr_b[0, 1])) return uint32ToCharErrorBase + 1;
            if (expected != Convert.ToChar(vt1.uint2darr_b[0, 1])) return uint32ToCharErrorBase + 2;
            if (expected != Convert.ToChar(cl1.uint2darr_b[0, 1])) return uint32ToCharErrorBase + 3;
            if (expected != Convert.ToChar(ja1_b[0][0, 1])) return uint32ToCharErrorBase + 4;

            // 3d
            if (expected != Convert.ToChar(uint3darr_b[1, 0, 1])) return uint32ToCharErrorBase + 5;
            if (expected != Convert.ToChar(vt1.uint3darr_b[1, 0, 1])) return uint32ToCharErrorBase + 6;
            if (expected != Convert.ToChar(cl1.uint3darr_b[1, 0, 1])) return uint32ToCharErrorBase + 7;
            if (expected != Convert.ToChar(ja2_b[1][1, 0, 1])) return uint32ToCharErrorBase + 8;

            return 0;
        }

        private static int UInt32ToInt32Tests(VT vt1, CL cl1)
        {
            const int uint32ToInt32ErrorBase = 400;

            int expected = 1;

            // 2d
            if (expected != Convert.ToInt32(uint2darr[0, 1])) return uint32ToInt32ErrorBase + 1;
            if (expected != Convert.ToInt32(vt1.uint2darr[0, 1])) return uint32ToInt32ErrorBase + 2;
            if (expected != Convert.ToInt32(cl1.uint2darr[0, 1])) return uint32ToInt32ErrorBase + 3;
            if (expected != Convert.ToInt32(ja1[0][0, 1])) return uint32ToInt32ErrorBase + 4;

            // 3d
            if (expected != Convert.ToInt32(uint3darr[1, 0, 1])) return uint32ToInt32ErrorBase + 5;
            if (expected != Convert.ToInt32(vt1.uint3darr[1, 0, 1])) return uint32ToInt32ErrorBase + 6;
            if (expected != Convert.ToInt32(cl1.uint3darr[1, 0, 1])) return uint32ToInt32ErrorBase + 7;
            if (expected != Convert.ToInt32(ja2[1][1, 0, 1])) return uint32ToInt32ErrorBase + 8;

            return 0;
        }

        private static int UInt32ToSByteTests(VT vt1, CL cl1)
        {
            const int uint32ToSByteErrorBase = 500;

            sbyte expected = 1;

            // 2d
            if (expected != Convert.ToSByte(uint2darr[0, 1])) return uint32ToSByteErrorBase + 1;
            if (expected != Convert.ToSByte(vt1.uint2darr[0, 1])) return uint32ToSByteErrorBase + 2;
            if (expected != Convert.ToSByte(cl1.uint2darr[0, 1])) return uint32ToSByteErrorBase + 3;
            if (expected != Convert.ToSByte(ja1[0][0, 1])) return uint32ToSByteErrorBase + 4;

            // 3d
            if (expected != Convert.ToSByte(uint3darr[1, 0, 1])) return uint32ToSByteErrorBase + 5;
            if (expected != Convert.ToSByte(vt1.uint3darr[1, 0, 1])) return uint32ToSByteErrorBase + 6;
            if (expected != Convert.ToSByte(cl1.uint3darr[1, 0, 1])) return uint32ToSByteErrorBase + 7;
            if (expected != Convert.ToSByte(ja2[1][1, 0, 1])) return uint32ToSByteErrorBase + 8;

            return 0;
        }

        private static int UInt32ToInt16Tests(VT vt1, CL cl1)
        {
            const int UInt32ToInt16ErrorBase = 600;

            short expected = 1;

            // 2d
            if (expected != Convert.ToInt16(uint2darr[0, 1])) return UInt32ToInt16ErrorBase + 1;
            if (expected != Convert.ToInt16(vt1.uint2darr[0, 1])) return UInt32ToInt16ErrorBase + 2;
            if (expected != Convert.ToInt16(cl1.uint2darr[0, 1])) return UInt32ToInt16ErrorBase + 3;
            if (expected != Convert.ToInt16(ja1[0][0, 1])) return UInt32ToInt16ErrorBase + 4;

            // 3d
            if (expected != Convert.ToInt16(uint3darr[1, 0, 1])) return UInt32ToInt16ErrorBase + 5;
            if (expected != Convert.ToInt16(vt1.uint3darr[1, 0, 1])) return UInt32ToInt16ErrorBase + 6;
            if (expected != Convert.ToInt16(cl1.uint3darr[1, 0, 1])) return UInt32ToInt16ErrorBase + 7;
            if (expected != Convert.ToInt16(ja2[1][1, 0, 1])) return UInt32ToInt16ErrorBase + 8;

            return 0;
        }

        private static int UInt32Tests(VT vt1, CL cl1)
        {
            const int uint32ErrorBase = 700;

            uint expected = 1;
            // 2d
            if (expected != uint2darr[0, 1]) return uint32ErrorBase + 1;
            if (expected != vt1.uint2darr[0, 1]) return uint32ErrorBase + 2;
            if (expected != cl1.uint2darr[0, 1]) return uint32ErrorBase + 3;
            if (expected != ja1[0][0, 1]) return uint32ErrorBase + 4;

            // 3d
            if (expected != uint3darr[1, 0, 1]) return uint32ErrorBase + 5;
            if (expected != vt1.uint3darr[1, 0, 1]) return uint32ErrorBase + 6;
            if (expected != cl1.uint3darr[1, 0, 1]) return uint32ErrorBase + 7;
            if (expected != ja2[1][1, 0, 1]) return uint32ErrorBase + 8;

            return 0;
        }

        private static int UInt32ToUInt16Tests(VT vt1, CL cl1)
        {
            const int UInt32ToUInt16ErrorBase = 800;

            ushort expected = 1;

            // 2d
            if (expected != Convert.ToUInt16(uint2darr[0, 1])) return UInt32ToUInt16ErrorBase + 1;
            if (expected != Convert.ToUInt16(vt1.uint2darr[0, 1])) return UInt32ToUInt16ErrorBase + 2;
            if (expected != Convert.ToUInt16(cl1.uint2darr[0, 1])) return UInt32ToUInt16ErrorBase + 3;
            if (expected != Convert.ToUInt16(ja1[0][0, 1])) return UInt32ToUInt16ErrorBase + 4;

            // 3d
            if (expected != Convert.ToUInt16(uint3darr[1, 0, 1])) return UInt32ToUInt16ErrorBase + 5;
            if (expected != Convert.ToUInt16(vt1.uint3darr[1, 0, 1])) return UInt32ToUInt16ErrorBase + 6;
            if (expected != Convert.ToUInt16(cl1.uint3darr[1, 0, 1])) return UInt32ToUInt16ErrorBase + 7;
            if (expected != Convert.ToUInt16(ja2[1][1, 0, 1])) return UInt32ToUInt16ErrorBase + 8;

            return 0;
        }
    }
}