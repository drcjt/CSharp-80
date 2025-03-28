using System;

namespace MDArrays.@byte
{
    public struct VT
    {
        public byte[,] byte2darr;
        public byte[,,] byte3darr;
        public byte[,] byte2darr_b;
        public byte[,,] byte3darr_b;
    }

    public class CL
    {
        public readonly byte[,] byte2darr = { { 0, 1 }, { 0, 0 } };
        public readonly byte[,,] byte3darr = { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
        public readonly byte[,] byte2darr_b = { { 0, 49 }, { 0, 0 } };
        public readonly byte[,,] byte3darr_b = { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };
    }

    internal static class DataTypesByte
    {
        static readonly byte[,] byte2darr = { { 0, 1 }, { 0, 0 } };
        static readonly byte[,,] byte3darr = { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
        static readonly byte[,] byte2darr_b = { { 0, 49 }, { 0, 0 } };
        static readonly byte[,,] byte3darr_b = { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };

        static readonly byte[][,] ja1 = new byte[2][,];
        static readonly byte[][,,] ja2 = new byte[2][,,];
        static readonly byte[][,] ja1_b = new byte[2][,];
        static readonly byte[][,,] ja2_b = new byte[2][,,];

        public static int Main()
        {
            VT vt1;
            vt1.byte2darr = new byte[,] { { 0, 1 }, { 0, 0 } };
            vt1.byte3darr = new byte[,,] { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
            vt1.byte2darr_b = new byte[,] { { 0, 49 }, { 0, 0 } };
            vt1.byte3darr_b = new byte[,,] { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };

            CL cl1 = new();

            ja1[0] = new byte[,] { { 0, 1 }, { 0, 0 } };
            ja2[1] = new byte[,,] { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
            ja1_b[0] = new byte[,] { { 0, 49 }, { 0, 0 } };
            ja2_b[1] = new byte[,,] { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };

            int result = ByteToBoolTests(vt1, cl1);
            if (result != 0) return result;

            result = ByteTests(vt1, cl1);
            if (result != 0) return result;

            result = ByteToCharTests(vt1, cl1);
            if (result != 0) return result;

            result = ByteToInt32Tests(vt1, cl1);
            if (result != 0) return result;

            result = ByteToSByteTests(vt1, cl1);
            if (result != 0) return result;

            result = ByteToInt16Tests(vt1, cl1);
            if (result != 0) return result;

            result = ByteToUInt32Tests(vt1, cl1);
            if (result != 0) return result;

            result = ByteToUInt16Tests(vt1, cl1);
            if (result != 0) return result;

            return 0;
        }

        private static int ByteToBoolTests(VT vt1, CL cl1)
        {
            const int ByteToBoolErrorBase = 100;

            bool expected = true;

            // 2d
            if (expected != Convert.ToBoolean(byte2darr[0, 1])) return ByteToBoolErrorBase + 1;
            if (expected != Convert.ToBoolean(vt1.byte2darr[0, 1])) return ByteToBoolErrorBase + 2;
            if (expected != Convert.ToBoolean(cl1.byte2darr[0, 1])) return ByteToBoolErrorBase + 3;
            if (expected != Convert.ToBoolean(ja1[0][0, 1])) return ByteToBoolErrorBase + 4;

            // 3d
            if (expected != Convert.ToBoolean(byte3darr[1, 0, 1])) return ByteToBoolErrorBase + 5;
            if (expected != Convert.ToBoolean(vt1.byte3darr[1, 0, 1])) return ByteToBoolErrorBase + 6;
            if (expected != Convert.ToBoolean(cl1.byte3darr[1, 0, 1])) return ByteToBoolErrorBase + 7;
            if (expected != Convert.ToBoolean(ja2[1][1, 0, 1])) return ByteToBoolErrorBase + 8;

            return 0;
        }

        private static int ByteTests(VT vt1, CL cl1)
        {
            const int ByteErrorBase = 200;

            byte expected = 1;
            // 2d
            if (expected != byte2darr[0, 1]) return ByteErrorBase + 1;
            if (expected != vt1.byte2darr[0, 1]) return ByteErrorBase + 2;
            if (expected != cl1.byte2darr[0, 1]) return ByteErrorBase + 3;
            if (expected != ja1[0][0, 1]) return ByteErrorBase + 4;

            // 3d
            if (expected != byte3darr[1, 0, 1]) return ByteErrorBase + 5;
            if (expected != vt1.byte3darr[1, 0, 1]) return ByteErrorBase + 6;
            if (expected != cl1.byte3darr[1, 0, 1]) return ByteErrorBase + 7;
            if (expected != ja2[1][1, 0, 1]) return ByteErrorBase + 8;

            return 0;
        }

        private static int ByteToCharTests(VT vt1, CL cl1)
        {
            const int ByteToCharErrorBase = 300;

            char expected = '1';

            // 2d
            if (expected != Convert.ToChar(byte2darr_b[0, 1])) return ByteToCharErrorBase + 1;
            if (expected != Convert.ToChar(vt1.byte2darr_b[0, 1])) return ByteToCharErrorBase + 2;
            if (expected != Convert.ToChar(cl1.byte2darr_b[0, 1])) return ByteToCharErrorBase + 3;
            if (expected != Convert.ToChar(ja1_b[0][0, 1])) return ByteToCharErrorBase + 4;

            // 3d
            if (expected != Convert.ToChar(byte3darr_b[1, 0, 1])) return ByteToCharErrorBase + 5;
            if (expected != Convert.ToChar(vt1.byte3darr_b[1, 0, 1])) return ByteToCharErrorBase + 6;
            if (expected != Convert.ToChar(cl1.byte3darr_b[1, 0, 1])) return ByteToCharErrorBase + 7;
            if (expected != Convert.ToChar(ja2_b[1][1, 0, 1])) return ByteToCharErrorBase + 8;

            return 0;
        }

        private static int ByteToInt32Tests(VT vt1, CL cl1)
        {
            const int ByteToInt32ErrorBase = 400;

            int expected = 1;

            // 2d
            if (expected != Convert.ToInt32(byte2darr[0, 1])) return ByteToInt32ErrorBase + 1;
            if (expected != Convert.ToInt32(vt1.byte2darr[0, 1])) return ByteToInt32ErrorBase + 2;
            if (expected != Convert.ToInt32(cl1.byte2darr[0, 1])) return ByteToInt32ErrorBase + 3;
            if (expected != Convert.ToInt32(ja1[0][0, 1])) return ByteToInt32ErrorBase + 4;

            // 3d
            if (expected != Convert.ToInt32(byte3darr[1, 0, 1])) return ByteToInt32ErrorBase + 5;
            if (expected != Convert.ToInt32(vt1.byte3darr[1, 0, 1])) return ByteToInt32ErrorBase + 6;
            if (expected != Convert.ToInt32(cl1.byte3darr[1, 0, 1])) return ByteToInt32ErrorBase + 7;
            if (expected != Convert.ToInt32(ja2[1][1, 0, 1])) return ByteToInt32ErrorBase + 8;

            return 0;
        }

        private static int ByteToSByteTests(VT vt1, CL cl1)
        {
            const int ByteToSByteErrorBase = 500;

            sbyte expected = 1;

            // 2d
            if (expected != Convert.ToSByte(byte2darr[0, 1])) return ByteToSByteErrorBase + 1;
            if (expected != Convert.ToSByte(vt1.byte2darr[0, 1])) return ByteToSByteErrorBase + 2;
            if (expected != Convert.ToSByte(cl1.byte2darr[0, 1])) return ByteToSByteErrorBase + 3;
            if (expected != Convert.ToSByte(ja1[0][0, 1])) return ByteToSByteErrorBase + 4;

            // 3d
            if (expected != Convert.ToSByte(byte3darr[1, 0, 1])) return ByteToSByteErrorBase + 5;
            if (expected != Convert.ToSByte(vt1.byte3darr[1, 0, 1])) return ByteToSByteErrorBase + 6;
            if (expected != Convert.ToSByte(cl1.byte3darr[1, 0, 1])) return ByteToSByteErrorBase + 7;
            if (expected != Convert.ToSByte(ja2[1][1, 0, 1])) return ByteToSByteErrorBase + 8;

            return 0;
        }

        private static int ByteToInt16Tests(VT vt1, CL cl1)
        {
            const int ByteToInt16ErrorBase = 600;

            short expected = 1;

            // 2d
            if (expected != Convert.ToInt16(byte2darr[0, 1])) return ByteToInt16ErrorBase + 1;
            if (expected != Convert.ToInt16(vt1.byte2darr[0, 1])) return ByteToInt16ErrorBase + 2;
            if (expected != Convert.ToInt16(cl1.byte2darr[0, 1])) return ByteToInt16ErrorBase + 3;
            if (expected != Convert.ToInt16(ja1[0][0, 1])) return ByteToInt16ErrorBase + 4;

            // 3d
            if (expected != Convert.ToInt16(byte3darr[1, 0, 1])) return ByteToInt16ErrorBase + 5;
            if (expected != Convert.ToInt16(vt1.byte3darr[1, 0, 1])) return ByteToInt16ErrorBase + 6;
            if (expected != Convert.ToInt16(cl1.byte3darr[1, 0, 1])) return ByteToInt16ErrorBase + 7;
            if (expected != Convert.ToInt16(ja2[1][1, 0, 1])) return ByteToInt16ErrorBase + 8;

            return 0;
        }

        private static int ByteToUInt32Tests(VT vt1, CL cl1)
        {
            const int ByteToUInt32ErrorBase = 700;

            uint expected = 1;

            // 2d
            if (expected != Convert.ToUInt32(byte2darr[0, 1])) return ByteToUInt32ErrorBase + 1;
            if (expected != Convert.ToUInt32(vt1.byte2darr[0, 1])) return ByteToUInt32ErrorBase + 2;
            if (expected != Convert.ToUInt32(cl1.byte2darr[0, 1])) return ByteToUInt32ErrorBase + 3;
            if (expected != Convert.ToUInt32(ja1[0][0, 1])) return ByteToUInt32ErrorBase + 4;

            // 3d
            if (expected != Convert.ToUInt32(byte3darr[1, 0, 1])) return ByteToUInt32ErrorBase + 5;
            if (expected != Convert.ToUInt32(vt1.byte3darr[1, 0, 1])) return ByteToUInt32ErrorBase + 6;
            if (expected != Convert.ToUInt32(cl1.byte3darr[1, 0, 1])) return ByteToUInt32ErrorBase + 7;
            if (expected != Convert.ToUInt32(ja2[1][1, 0, 1])) return ByteToUInt32ErrorBase + 8;

            return 0;
        }

        private static int ByteToUInt16Tests(VT vt1, CL cl1)
        {
            const int ByteToUInt16ErrorBase = 800;

            ushort expected = 1;

            // 2d
            if (expected != Convert.ToUInt16(byte2darr[0, 1])) return ByteToUInt16ErrorBase + 1;
            if (expected != Convert.ToUInt16(vt1.byte2darr[0, 1])) return ByteToUInt16ErrorBase + 2;
            if (expected != Convert.ToUInt16(cl1.byte2darr[0, 1])) return ByteToUInt16ErrorBase + 3;
            if (expected != Convert.ToUInt16(ja1[0][0, 1])) return ByteToUInt16ErrorBase + 4;

            // 3d
            if (expected != Convert.ToUInt16(byte3darr[1, 0, 1])) return ByteToUInt16ErrorBase + 5;
            if (expected != Convert.ToUInt16(vt1.byte3darr[1, 0, 1])) return ByteToUInt16ErrorBase + 6;
            if (expected != Convert.ToUInt16(cl1.byte3darr[1, 0, 1])) return ByteToUInt16ErrorBase + 7;
            if (expected != Convert.ToUInt16(ja2[1][1, 0, 1])) return ByteToUInt16ErrorBase + 8;

            return 0;
        }
    }
}