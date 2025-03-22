using System;

namespace MDArrays.@bool
{
    public struct VT
    {
        public bool[,] bool2darr;
        public bool[,,] bool3darr;
    }

    public class CL
    {
        public readonly bool[,] bool2darr = { { false, true }, { false, false } };
        public readonly bool[,,] bool3darr = { { { false, false } }, { { false, true } }, { { false, false } } };
    }

    internal static class DataTypesBool
    {
        static readonly bool[,] bool2darr = { { false, true }, { false, false } };
        static readonly bool[,,] bool3darr = { { { false, false } }, { { false, true } }, { { false, false } } };

        static readonly bool[][,] ja1 = new bool[2][,];
        static readonly bool[][,,] ja2 = new bool[2][,,];

        public static int TestEntryPoint()
        {
            VT vt1;
            vt1.bool2darr = new bool[,] { { false, true }, { false, false } };
            vt1.bool3darr = new bool[,,] { { { false, false } }, { { false, true } }, { { false, false } } };

            CL cl1 = new();

            ja1[0] = new bool[,] { { false, true }, { false, false } };
            ja2[1] = new bool[,,] { { { false, false } }, { { false, true } }, { { false, false } } };


            int result = BoolTests(vt1, cl1);
            if (result != 0) return result;

            result = BoolToByteTests(vt1, cl1);
            if (result != 0) return result;

            result = BoolToInt32Tests(vt1, cl1);
            if (result != 0) return result;

            result = BoolToSByteTests(vt1, cl1);
            if (result != 0) return result;

            result = BoolToInt16Tests(vt1, cl1);
            if (result != 0) return result;

            result = BoolToUInt32Tests(vt1, cl1);
            if (result != 0) return result;

            result = BoolToUInt16Tests(vt1, cl1);
            if (result != 0) return result;

            return 0;
        }

        private static int BoolTests(VT vt1, CL cl1)
        {
            const int BoolErrorBase = 100;

            bool expected = true;
            // 2d
            if (expected != bool2darr[0, 1]) return BoolErrorBase + 1;
            if (expected != vt1.bool2darr[0, 1]) return BoolErrorBase + 2;
            if (expected != cl1.bool2darr[0, 1]) return BoolErrorBase + 3;
            if (expected != ja1[0][0, 1]) return BoolErrorBase + 4;

            // 3d
            if (expected != bool3darr[1, 0, 1]) return BoolErrorBase + 5;
            if (expected != vt1.bool3darr[1, 0, 1]) return BoolErrorBase + 6;
            if (expected != cl1.bool3darr[1, 0, 1]) return BoolErrorBase + 7;
            if (expected != ja2[1][1, 0, 1]) return BoolErrorBase + 8;

            return 0;
        }

        private static int BoolToByteTests(VT vt1, CL cl1)
        {
            const int BoolToByteErrorBase = 200;

            byte expected = 1;

            // 2d
            if (expected != Convert.ToByte(bool2darr[0, 1])) return BoolToByteErrorBase + 1;
            if (expected != Convert.ToByte(vt1.bool2darr[0, 1])) return BoolToByteErrorBase + 2;
            if (expected != Convert.ToByte(cl1.bool2darr[0, 1])) return BoolToByteErrorBase + 3;
            if (expected != Convert.ToByte(ja1[0][0, 1])) return BoolToByteErrorBase + 4;

            // 3d
            if (expected != Convert.ToByte(bool3darr[1, 0, 1])) return BoolToByteErrorBase + 5;
            if (expected != Convert.ToByte(vt1.bool3darr[1, 0, 1])) return BoolToByteErrorBase + 6;
            if (expected != Convert.ToByte(cl1.bool3darr[1, 0, 1])) return BoolToByteErrorBase + 7;
            if (expected != Convert.ToByte(ja2[1][1, 0, 1])) return BoolToByteErrorBase + 8;

            return 0;
        }

        private static int BoolToInt32Tests(VT vt1, CL cl1)
        {
            const int BoolToInt32ErrorBase = 300;

            int expected = 1;

            // 2d
            if (expected != Convert.ToInt32(bool2darr[0, 1])) return BoolToInt32ErrorBase + 1;
            if (expected != Convert.ToInt32(vt1.bool2darr[0, 1])) return BoolToInt32ErrorBase + 2;
            if (expected != Convert.ToInt32(cl1.bool2darr[0, 1])) return BoolToInt32ErrorBase + 3;
            if (expected != Convert.ToInt32(ja1[0][0, 1])) return BoolToInt32ErrorBase + 4;

            // 3d
            if (expected != Convert.ToInt32(bool3darr[1, 0, 1])) return BoolToInt32ErrorBase + 5;
            if (expected != Convert.ToInt32(vt1.bool3darr[1, 0, 1])) return BoolToInt32ErrorBase + 6;
            if (expected != Convert.ToInt32(cl1.bool3darr[1, 0, 1])) return BoolToInt32ErrorBase + 7;
            if (expected != Convert.ToInt32(ja2[1][1, 0, 1])) return BoolToInt32ErrorBase + 8;

            return 0;
        }

        private static int BoolToSByteTests(VT vt1, CL cl1)
        {
            const int BoolToSByteErrorBase = 400;

            sbyte expected = 1;

            // 2d
            if (expected != Convert.ToSByte(bool2darr[0, 1])) return BoolToSByteErrorBase + 1;
            if (expected != Convert.ToSByte(vt1.bool2darr[0, 1])) return BoolToSByteErrorBase + 2;
            if (expected != Convert.ToSByte(cl1.bool2darr[0, 1])) return BoolToSByteErrorBase + 3;
            if (expected != Convert.ToSByte(ja1[0][0, 1])) return BoolToSByteErrorBase + 4;

            // 3d
            if (expected != Convert.ToSByte(bool3darr[1, 0, 1])) return BoolToSByteErrorBase + 5;
            if (expected != Convert.ToSByte(vt1.bool3darr[1, 0, 1])) return BoolToSByteErrorBase + 6;
            if (expected != Convert.ToSByte(cl1.bool3darr[1, 0, 1])) return BoolToSByteErrorBase + 7;
            if (expected != Convert.ToSByte(ja2[1][1, 0, 1])) return BoolToSByteErrorBase + 8;

            return 0;
        }

        private static int BoolToInt16Tests(VT vt1, CL cl1)
        {
            const int BoolToInt16ErrorBase = 500;

            short expected = 1;

            // 2d
            if (expected != Convert.ToInt16(bool2darr[0, 1])) return BoolToInt16ErrorBase + 1;
            if (expected != Convert.ToInt16(vt1.bool2darr[0, 1])) return BoolToInt16ErrorBase + 2;
            if (expected != Convert.ToInt16(cl1.bool2darr[0, 1])) return BoolToInt16ErrorBase + 3;
            if (expected != Convert.ToInt16(ja1[0][0, 1])) return BoolToInt16ErrorBase + 4;

            // 3d
            if (expected != Convert.ToInt16(bool3darr[1, 0, 1])) return BoolToInt16ErrorBase + 5;
            if (expected != Convert.ToInt16(vt1.bool3darr[1, 0, 1])) return BoolToInt16ErrorBase + 6;
            if (expected != Convert.ToInt16(cl1.bool3darr[1, 0, 1])) return BoolToInt16ErrorBase + 7;
            if (expected != Convert.ToInt16(ja2[1][1, 0, 1])) return BoolToInt16ErrorBase + 8;

            return 0;
        }

        private static int BoolToUInt32Tests(VT vt1, CL cl1)
        {
            const int BoolToUInt32ErrorBase = 600;

            uint expected = 1;

            // 2d
            if (expected != Convert.ToUInt32(bool2darr[0, 1])) return BoolToUInt32ErrorBase + 1;
            if (expected != Convert.ToUInt32(vt1.bool2darr[0, 1])) return BoolToUInt32ErrorBase + 2;
            if (expected != Convert.ToUInt32(cl1.bool2darr[0, 1])) return BoolToUInt32ErrorBase + 3;
            if (expected != Convert.ToUInt32(ja1[0][0, 1])) return BoolToUInt32ErrorBase + 4;

            // 3d
            if (expected != Convert.ToUInt32(bool3darr[1, 0, 1])) return BoolToUInt32ErrorBase + 5;
            if (expected != Convert.ToUInt32(vt1.bool3darr[1, 0, 1])) return BoolToUInt32ErrorBase + 6;
            if (expected != Convert.ToUInt32(cl1.bool3darr[1, 0, 1])) return BoolToUInt32ErrorBase + 7;
            if (expected != Convert.ToUInt32(ja2[1][1, 0, 1])) return BoolToUInt32ErrorBase + 8;

            return 0;
        }

        private static int BoolToUInt16Tests(VT vt1, CL cl1)
        {
            const int BoolToUInt16ErrorBase = 700;

            ushort expected = 1;

            // 2d
            if (expected != Convert.ToUInt16(bool2darr[0, 1])) return BoolToUInt16ErrorBase + 1;
            if (expected != Convert.ToUInt16(vt1.bool2darr[0, 1])) return BoolToUInt16ErrorBase + 2;
            if (expected != Convert.ToUInt16(cl1.bool2darr[0, 1])) return BoolToUInt16ErrorBase + 3;
            if (expected != Convert.ToUInt16(ja1[0][0, 1])) return BoolToUInt16ErrorBase + 4;

            // 3d
            if (expected != Convert.ToUInt16(bool3darr[1, 0, 1])) return BoolToUInt16ErrorBase + 5;
            if (expected != Convert.ToUInt16(vt1.bool3darr[1, 0, 1])) return BoolToUInt16ErrorBase + 6;
            if (expected != Convert.ToUInt16(cl1.bool3darr[1, 0, 1])) return BoolToUInt16ErrorBase + 7;
            if (expected != Convert.ToUInt16(ja2[1][1, 0, 1])) return BoolToUInt16ErrorBase + 8;

            return 0;
        }
    }
}
