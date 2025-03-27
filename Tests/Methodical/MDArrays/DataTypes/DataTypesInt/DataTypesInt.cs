using System;

namespace MDArrays.@int
{
    public struct VT
    {
        public int[,] int2darr;
        public int[,,] int3darr;
        public int[,] int2darr_b;
        public int[,,] int3darr_b;
        public int[,] int2darr_c;
        public int[,,] int3darr_c;
    }

    public class CL
    {
        public readonly int[,] int2darr = { { 0, -1 }, { 0, 0 } };
        public readonly int[,,] int3darr = { { { 0, 0 } }, { { 0, -1 } }, { { 0, 0 } } };
        public readonly int[,] int2darr_b = { { 0, 1 }, { 0, 0 } };
        public readonly int[,,] int3darr_b = { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
        public readonly int[,] int2darr_c = { { 0, 49 }, { 0, 0 } };
        public readonly int[,,] int3darr_c = { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };
    }

    internal static class DataTypesInt
    {
        static readonly int[,] int2darr = { { 0, -1 }, { 0, 0 } };
        static readonly int[,,] int3darr = { { { 0, 0 } }, { { 0, -1 } }, { { 0, 0 } } };
        static readonly int[][,] ja1 = new int[2][,];
        static readonly int[][,,] ja2 = new int[2][,,];
        static readonly int[,] int2darr_b = { { 0, 1 }, { 0, 0 } };
        static readonly int[,,] int3darr_b = { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
        static readonly int[][,] ja1_b = new int[2][,];
        static readonly int[][,,] ja2_b = new int[2][,,];
        static readonly int[,] int2darr_c = { { 0, 49 }, { 0, 0 } };
        static readonly int[,,] int3darr_c = { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };
        static readonly int[][,] ja1_c = new int[2][,];
        static readonly int[][,,] ja2_c = new int[2][,,];

        public static int Main()
        {
            VT vt1;
            vt1.int2darr = new int[,] { { 0, -1 }, { 0, 0 } };
            vt1.int3darr = new int[,,] { { { 0, 0 } }, { { 0, -1 } }, { { 0, 0 } } };
            vt1.int2darr_b = new int[,] { { 0, 1 }, { 0, 0 } };
            vt1.int3darr_b = new int[,,] { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
            vt1.int2darr_c = new int[,] { { 0, 49 }, { 0, 0 } };
            vt1.int3darr_c = new int[,,] { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };

            CL cl1 = new();

            ja1[0] = new int[,] { { 0, -1 }, { 0, 0 } };
            ja2[1] = new int[,,] { { { 0, 0 } }, { { 0, -1 } }, { { 0, 0 } } };
            ja1_b[0] = new int[,] { { 0, 1 }, { 0, 0 } };
            ja2_b[1] = new int[,,] { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
            ja1_c[0] = new int[,] { { 0, 49 }, { 0, 0 } };
            ja2_c[1] = new int[,,] { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };

            int result = Int32ToBoolTests(vt1, cl1);
            if (result != 0) return result;

            result = Int32ToByteTests(vt1, cl1);
            if (result != 0) return result;

            result = Int32ToCharTests(vt1, cl1);
            if (result != 0) return result;

            result = Int32Tests(vt1, cl1);
            if (result != 0) return result;

            result = Int32ToSByteTests(vt1, cl1);
            if (result != 0) return result;

            result = Int32ToInt16Tests(vt1, cl1);
            if (result != 0) return result;

            result = Int32ToUInt32Tests(vt1, cl1);
            if (result != 0) return result;

            result = Int32ToUInt16Tests(vt1, cl1);
            if (result != 0) return result;

            return 0;
        }

        private static int Int32ToBoolTests(VT vt1, CL cl1)
        {
            const int Int32ToBoolErrorBase = 100;

            bool expected = true;

            // 2d
            if (expected != Convert.ToBoolean(int2darr[0, 1])) return Int32ToBoolErrorBase + 1;
            if (expected != Convert.ToBoolean(vt1.int2darr[0, 1])) return Int32ToBoolErrorBase + 2;
            if (expected != Convert.ToBoolean(cl1.int2darr[0, 1])) return Int32ToBoolErrorBase + 3;
            if (expected != Convert.ToBoolean(ja1[0][0, 1])) return Int32ToBoolErrorBase + 4;

            // 3d
            if (expected != Convert.ToBoolean(int3darr[1, 0, 1])) return Int32ToBoolErrorBase + 5;
            if (expected != Convert.ToBoolean(vt1.int3darr[1, 0, 1])) return Int32ToBoolErrorBase + 6;
            if (expected != Convert.ToBoolean(cl1.int3darr[1, 0, 1])) return Int32ToBoolErrorBase + 7;
            if (expected != Convert.ToBoolean(ja2[1][1, 0, 1])) return Int32ToBoolErrorBase + 8;

            return 0;
        }

        private static int Int32ToByteTests(VT vt1, CL cl1)
        {
            const int Int32ToByteErrorBase = 200;

            byte expected = 1;

            // 2d
            if (expected != Convert.ToByte(int2darr_b[0, 1])) return Int32ToByteErrorBase + 1;
            if (expected != Convert.ToByte(vt1.int2darr_b[0, 1])) return Int32ToByteErrorBase + 2;
            if (expected != Convert.ToByte(cl1.int2darr_b[0, 1])) return Int32ToByteErrorBase + 3;
            if (expected != Convert.ToByte(ja1_b[0][0, 1])) return Int32ToByteErrorBase + 4;

            // 3d
            if (expected != Convert.ToByte(int3darr_b[1, 0, 1])) return Int32ToByteErrorBase + 5;
            if (expected != Convert.ToByte(vt1.int3darr_b[1, 0, 1])) return Int32ToByteErrorBase + 6;
            if (expected != Convert.ToByte(cl1.int3darr_b[1, 0, 1])) return Int32ToByteErrorBase + 7;
            if (expected != Convert.ToByte(ja2_b[1][1, 0, 1])) return Int32ToByteErrorBase + 8;

            return 0;
        }

        private static int Int32ToCharTests(VT vt1, CL cl1)
        {
            const int Int32ToCharErrorBase = 300;

            char expected = '1';

            // 2d
            if (expected != Convert.ToChar(int2darr_c[0, 1])) return Int32ToCharErrorBase + 1;
            if (expected != Convert.ToChar(vt1.int2darr_c[0, 1])) return Int32ToCharErrorBase + 2;
            if (expected != Convert.ToChar(cl1.int2darr_c[0, 1])) return Int32ToCharErrorBase + 3;
            if (expected != Convert.ToChar(ja1_c[0][0, 1])) return Int32ToCharErrorBase + 4;

            // 3d
            if (expected != Convert.ToChar(int3darr_c[1, 0, 1])) return Int32ToCharErrorBase + 5;
            if (expected != Convert.ToChar(vt1.int3darr_c[1, 0, 1])) return Int32ToCharErrorBase + 6;
            if (expected != Convert.ToChar(cl1.int3darr_c[1, 0, 1])) return Int32ToCharErrorBase + 7;
            if (expected != Convert.ToChar(ja2_c[1][1, 0, 1])) return Int32ToCharErrorBase + 8;

            return 0;
        }

        private static int Int32Tests(VT vt1, CL cl1)
        {
            const int Int32ErrorBase = 400;

            int expected = -1;
            // 2d
            if (expected != int2darr[0, 1]) return Int32ErrorBase + 1;
            if (expected != vt1.int2darr[0, 1]) return Int32ErrorBase + 2;
            if (expected != cl1.int2darr[0, 1]) return Int32ErrorBase + 3;
            if (expected != ja1[0][0, 1]) return Int32ErrorBase + 4;

            // 3d
            if (expected != int3darr[1, 0, 1]) return Int32ErrorBase + 5;
            if (expected != vt1.int3darr[1, 0, 1]) return Int32ErrorBase + 6;
            if (expected != cl1.int3darr[1, 0, 1]) return Int32ErrorBase + 7;
            if (expected != ja2[1][1, 0, 1]) return Int32ErrorBase + 8;

            return 0;
        }
        private static int Int32ToSByteTests(VT vt1, CL cl1)
        {
            const int Int32ToSByteErrorBase = 500;

            sbyte expected = -1;

            // 2d
            if (expected != Convert.ToSByte(int2darr[0, 1])) return Int32ToSByteErrorBase + 1;
            if (expected != Convert.ToSByte(vt1.int2darr[0, 1])) return Int32ToSByteErrorBase + 2;
            if (expected != Convert.ToSByte(cl1.int2darr[0, 1])) return Int32ToSByteErrorBase + 3;
            if (expected != Convert.ToSByte(ja1[0][0, 1])) return Int32ToSByteErrorBase + 4;

            // 3d
            if (expected != Convert.ToSByte(int3darr[1, 0, 1])) return Int32ToSByteErrorBase + 5;
            if (expected != Convert.ToSByte(vt1.int3darr[1, 0, 1])) return Int32ToSByteErrorBase + 6;
            if (expected != Convert.ToSByte(cl1.int3darr[1, 0, 1])) return Int32ToSByteErrorBase + 7;
            if (expected != Convert.ToSByte(ja2[1][1, 0, 1])) return Int32ToSByteErrorBase + 8;

            return 0;
        }

        private static int Int32ToInt16Tests(VT vt1, CL cl1)
        {
            const int Int32ToInt16ErrorBase = 600;

            short expected = -1;

            // 2d
            if (expected != Convert.ToSByte(int2darr[0, 1])) return Int32ToInt16ErrorBase + 1;
            if (expected != Convert.ToSByte(vt1.int2darr[0, 1])) return Int32ToInt16ErrorBase + 2;
            if (expected != Convert.ToSByte(cl1.int2darr[0, 1])) return Int32ToInt16ErrorBase + 3;
            if (expected != Convert.ToSByte(ja1[0][0, 1])) return Int32ToInt16ErrorBase + 4;

            // 3d
            if (expected != Convert.ToSByte(int3darr[1, 0, 1])) return Int32ToInt16ErrorBase + 5;
            if (expected != Convert.ToSByte(vt1.int3darr[1, 0, 1])) return Int32ToInt16ErrorBase + 6;
            if (expected != Convert.ToSByte(cl1.int3darr[1, 0, 1])) return Int32ToInt16ErrorBase + 7;
            if (expected != Convert.ToSByte(ja2[1][1, 0, 1])) return Int32ToInt16ErrorBase + 8;

            return 0;
        }

        private static int Int32ToUInt32Tests(VT vt1, CL cl1)
        {
            const int Int32ToUInt32ErrorBase = 700;

            uint expected = 1;

            // 2d
            if (expected != Convert.ToUInt32(int2darr_b[0, 1])) return Int32ToUInt32ErrorBase + 1;
            if (expected != Convert.ToUInt32(vt1.int2darr_b[0, 1])) return Int32ToUInt32ErrorBase + 2;
            if (expected != Convert.ToUInt32(cl1.int2darr_b[0, 1])) return Int32ToUInt32ErrorBase + 3;
            if (expected != Convert.ToUInt32(ja1_b[0][0, 1])) return Int32ToUInt32ErrorBase + 4;

            // 3d
            if (expected != Convert.ToUInt32(int3darr_b[1, 0, 1])) return Int32ToUInt32ErrorBase + 5;
            if (expected != Convert.ToUInt32(vt1.int3darr_b[1, 0, 1])) return Int32ToUInt32ErrorBase + 6;
            if (expected != Convert.ToUInt32(cl1.int3darr_b[1, 0, 1])) return Int32ToUInt32ErrorBase + 7;
            if (expected != Convert.ToUInt32(ja2_b[1][1, 0, 1])) return Int32ToUInt32ErrorBase + 8;

            return 0;
        }

        private static int Int32ToUInt16Tests(VT vt1, CL cl1)
        {
            const int Int32ToUInt16ErrorBase = 800;

            ushort expected = 1;

            // 2d
            if (expected != Convert.ToUInt16(int2darr_b[0, 1])) return Int32ToUInt16ErrorBase + 1;
            if (expected != Convert.ToUInt16(vt1.int2darr_b[0, 1])) return Int32ToUInt16ErrorBase + 2;
            if (expected != Convert.ToUInt16(cl1.int2darr_b[0, 1])) return Int32ToUInt16ErrorBase + 3;
            if (expected != Convert.ToUInt16(ja1_b[0][0, 1])) return Int32ToUInt16ErrorBase + 4;

            // 3d
            if (expected != Convert.ToUInt16(int3darr_b[1, 0, 1])) return Int32ToUInt16ErrorBase + 5;
            if (expected != Convert.ToUInt16(vt1.int3darr_b[1, 0, 1])) return Int32ToUInt16ErrorBase + 6;
            if (expected != Convert.ToUInt16(cl1.int3darr_b[1, 0, 1])) return Int32ToUInt16ErrorBase + 7;
            if (expected != Convert.ToUInt16(ja2_b[1][1, 0, 1])) return Int32ToUInt16ErrorBase + 8;

            return 0;
        }
    }
}