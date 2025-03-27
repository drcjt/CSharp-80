using System;

namespace MDArrays.@sbyte
{
    public struct VT
    {
        public sbyte[,] sbyte2darr;
        public sbyte[,,] sbyte3darr;
        public sbyte[,] sbyte2darr_b;
        public sbyte[,,] sbyte3darr_b;
    }

    public class CL
    {
        public readonly sbyte[,] sbyte2darr = { { 0, 1 }, { 0, 0 } };
        public readonly sbyte[,,] sbyte3darr = { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
        public readonly sbyte[,] sbyte2darr_b = { { 0, 49 }, { 0, 0 } };
        public readonly sbyte[,,] sbyte3darr_b = { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };
    }

    internal static class DataTypesSByte
    {
        static readonly sbyte[,] sbyte2darr = { { 0, 1 }, { 0, 0 } };
        static readonly sbyte[,,] sbyte3darr = { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
        static readonly sbyte[,] sbyte2darr_b = { { 0, 49 }, { 0, 0 } };
        static readonly sbyte[,,] sbyte3darr_b = { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };

        static readonly sbyte[][,] ja1 = new sbyte[2][,];
        static readonly sbyte[][,,] ja2 = new sbyte[2][,,];
        static readonly sbyte[][,] ja1_b = new sbyte[2][,];
        static readonly sbyte[][,,] ja2_b = new sbyte[2][,,];

        public static int Main()
        {
            VT vt1;
            vt1.sbyte2darr = new sbyte[,] { { 0, 1 }, { 0, 0 } };
            vt1.sbyte3darr = new sbyte[,,] { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
            vt1.sbyte2darr_b = new sbyte[,] { { 0, 49 }, { 0, 0 } };
            vt1.sbyte3darr_b = new sbyte[,,] { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };

            CL cl1 = new();

            ja1[0] = new sbyte[,] { { 0, 1 }, { 0, 0 } };
            ja2[1] = new sbyte[,,] { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
            ja1_b[0] = new sbyte[,] { { 0, 49 }, { 0, 0 } };
            ja2_b[1] = new sbyte[,,] { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };

            int result = SByteToBoolTests(vt1, cl1);
            if (result != 0) return result;

            result = SByteToByteTests(vt1, cl1);
            if (result != 0) return result;

            result = SByteToCharTests(vt1, cl1);
            if (result != 0) return result;

            result = SByteToInt32Tests(vt1, cl1);
            if (result != 0) return result;

            result = SByteTests(vt1, cl1);
            if (result != 0) return result;

            result = SByteToInt16Tests(vt1, cl1);
            if (result != 0) return result;

            result = SByteToUInt32Tests(vt1, cl1);
            if (result != 0) return result;

            result = SByteToUInt16Tests(vt1, cl1);
            if (result != 0) return result;

            return 0;
        }

        private static int SByteToBoolTests(VT vt1, CL cl1)
        {
            const int SByteToBoolErrorBase = 100;

            bool expected = true;

            // 2d
            if (expected != Convert.ToBoolean(sbyte2darr[0, 1])) return SByteToBoolErrorBase + 1;
            if (expected != Convert.ToBoolean(vt1.sbyte2darr[0, 1])) return SByteToBoolErrorBase + 2;
            if (expected != Convert.ToBoolean(cl1.sbyte2darr[0, 1])) return SByteToBoolErrorBase + 3;
            if (expected != Convert.ToBoolean(ja1[0][0, 1])) return SByteToBoolErrorBase + 4;

            // 3d
            if (expected != Convert.ToBoolean(sbyte3darr[1, 0, 1])) return SByteToBoolErrorBase + 5;
            if (expected != Convert.ToBoolean(vt1.sbyte3darr[1, 0, 1])) return SByteToBoolErrorBase + 6;
            if (expected != Convert.ToBoolean(cl1.sbyte3darr[1, 0, 1])) return SByteToBoolErrorBase + 7;
            if (expected != Convert.ToBoolean(ja2[1][1, 0, 1])) return SByteToBoolErrorBase + 8;

            return 0;
        }

        private static int SByteToByteTests(VT vt1, CL cl1)
        {
            const int SByteToByteErrorBase = 200;

            byte expected = 1;

            // 2d
            if (expected != Convert.ToByte(sbyte2darr[0, 1])) return SByteToByteErrorBase + 1;
            if (expected != Convert.ToByte(vt1.sbyte2darr[0, 1])) return SByteToByteErrorBase + 2;
            if (expected != Convert.ToByte(cl1.sbyte2darr[0, 1])) return SByteToByteErrorBase + 3;
            if (expected != Convert.ToByte(ja1[0][0, 1])) return SByteToByteErrorBase + 4;

            // 3d
            if (expected != Convert.ToByte(sbyte3darr[1, 0, 1])) return SByteToByteErrorBase + 5;
            if (expected != Convert.ToByte(vt1.sbyte3darr[1, 0, 1])) return SByteToByteErrorBase + 6;
            if (expected != Convert.ToByte(cl1.sbyte3darr[1, 0, 1])) return SByteToByteErrorBase + 7;
            if (expected != Convert.ToByte(ja2[1][1, 0, 1])) return SByteToByteErrorBase + 8;

            return 0;
        }

        private static int SByteToCharTests(VT vt1, CL cl1)
        {
            const int SByteToCharErrorBase = 300;

            char expected = '1';

            // 2d
            if (expected != Convert.ToChar(sbyte2darr_b[0, 1])) return SByteToCharErrorBase + 1;
            if (expected != Convert.ToChar(vt1.sbyte2darr_b[0, 1])) return SByteToCharErrorBase + 2;
            if (expected != Convert.ToChar(cl1.sbyte2darr_b[0, 1])) return SByteToCharErrorBase + 3;
            if (expected != Convert.ToChar(ja1_b[0][0, 1])) return SByteToCharErrorBase + 4;

            // 3d
            if (expected != Convert.ToChar(sbyte3darr_b[1, 0, 1])) return SByteToCharErrorBase + 5;
            if (expected != Convert.ToChar(vt1.sbyte3darr_b[1, 0, 1])) return SByteToCharErrorBase + 6;
            if (expected != Convert.ToChar(cl1.sbyte3darr_b[1, 0, 1])) return SByteToCharErrorBase + 7;
            if (expected != Convert.ToChar(ja2_b[1][1, 0, 1])) return SByteToCharErrorBase + 8;

            return 0;
        }

        private static int SByteToInt32Tests(VT vt1, CL cl1)
        {
            const int SByteToInt32ErrorBase = 400;

            int expected = 1;

            // 2d
            if (expected != Convert.ToInt32(sbyte2darr[0, 1])) return SByteToInt32ErrorBase + 1;
            if (expected != Convert.ToInt32(vt1.sbyte2darr[0, 1])) return SByteToInt32ErrorBase + 2;
            if (expected != Convert.ToInt32(cl1.sbyte2darr[0, 1])) return SByteToInt32ErrorBase + 3;
            if (expected != Convert.ToInt32(ja1[0][0, 1])) return SByteToInt32ErrorBase + 4;

            // 3d
            if (expected != Convert.ToInt32(sbyte3darr[1, 0, 1])) return SByteToInt32ErrorBase + 5;
            if (expected != Convert.ToInt32(vt1.sbyte3darr[1, 0, 1])) return SByteToInt32ErrorBase + 6;
            if (expected != Convert.ToInt32(cl1.sbyte3darr[1, 0, 1])) return SByteToInt32ErrorBase + 7;
            if (expected != Convert.ToInt32(ja2[1][1, 0, 1])) return SByteToInt32ErrorBase + 8;

            return 0;
        }

        private static int SByteTests(VT vt1, CL cl1)
        {
            const int SByteErrorBase = 500;

            sbyte expected = 1;
            // 2d
            if (expected != sbyte2darr[0, 1]) return SByteErrorBase + 1;
            if (expected != vt1.sbyte2darr[0, 1]) return SByteErrorBase + 2;
            if (expected != cl1.sbyte2darr[0, 1]) return SByteErrorBase + 3;
            if (expected != ja1[0][0, 1]) return SByteErrorBase + 4;

            // 3d
            if (expected != sbyte3darr[1, 0, 1]) return SByteErrorBase + 5;
            if (expected != vt1.sbyte3darr[1, 0, 1]) return SByteErrorBase + 6;
            if (expected != cl1.sbyte3darr[1, 0, 1]) return SByteErrorBase + 7;
            if (expected != ja2[1][1, 0, 1]) return SByteErrorBase + 8;

            return 0;
        }

        private static int SByteToInt16Tests(VT vt1, CL cl1)
        {
            const int SByteToInt16ErrorBase = 600;

            short expected = 1;

            // 2d
            if (expected != Convert.ToInt16(sbyte2darr[0, 1])) return SByteToInt16ErrorBase + 1;
            if (expected != Convert.ToInt16(vt1.sbyte2darr[0, 1])) return SByteToInt16ErrorBase + 2;
            if (expected != Convert.ToInt16(cl1.sbyte2darr[0, 1])) return SByteToInt16ErrorBase + 3;
            if (expected != Convert.ToInt16(ja1[0][0, 1])) return SByteToInt16ErrorBase + 4;

            // 3d
            if (expected != Convert.ToInt16(sbyte3darr[1, 0, 1])) return SByteToInt16ErrorBase + 5;
            if (expected != Convert.ToInt16(vt1.sbyte3darr[1, 0, 1])) return SByteToInt16ErrorBase + 6;
            if (expected != Convert.ToInt16(cl1.sbyte3darr[1, 0, 1])) return SByteToInt16ErrorBase + 7;
            if (expected != Convert.ToInt16(ja2[1][1, 0, 1])) return SByteToInt16ErrorBase + 8;

            return 0;
        }

        private static int SByteToUInt32Tests(VT vt1, CL cl1)
        {
            const int ByteToUInt32ErrorBase = 700;

            uint expected = 1;

            // 2d
            if (expected != Convert.ToUInt32(sbyte2darr[0, 1])) return ByteToUInt32ErrorBase + 1;
            if (expected != Convert.ToUInt32(vt1.sbyte2darr[0, 1])) return ByteToUInt32ErrorBase + 2;
            if (expected != Convert.ToUInt32(cl1.sbyte2darr[0, 1])) return ByteToUInt32ErrorBase + 3;
            if (expected != Convert.ToUInt32(ja1[0][0, 1])) return ByteToUInt32ErrorBase + 4;

            // 3d
            if (expected != Convert.ToUInt32(sbyte3darr[1, 0, 1])) return ByteToUInt32ErrorBase + 5;
            if (expected != Convert.ToUInt32(vt1.sbyte3darr[1, 0, 1])) return ByteToUInt32ErrorBase + 6;
            if (expected != Convert.ToUInt32(cl1.sbyte3darr[1, 0, 1])) return ByteToUInt32ErrorBase + 7;
            if (expected != Convert.ToUInt32(ja2[1][1, 0, 1])) return ByteToUInt32ErrorBase + 8;

            return 0;
        }

        private static int SByteToUInt16Tests(VT vt1, CL cl1)
        {
            const int ByteToUInt16ErrorBase = 800;

            ushort expected = 1;

            // 2d
            if (expected != Convert.ToUInt16(sbyte2darr[0, 1])) return ByteToUInt16ErrorBase + 1;
            if (expected != Convert.ToUInt16(vt1.sbyte2darr[0, 1])) return ByteToUInt16ErrorBase + 2;
            if (expected != Convert.ToUInt16(cl1.sbyte2darr[0, 1])) return ByteToUInt16ErrorBase + 3;
            if (expected != Convert.ToUInt16(ja1[0][0, 1])) return ByteToUInt16ErrorBase + 4;

            // 3d
            if (expected != Convert.ToUInt16(sbyte3darr[1, 0, 1])) return ByteToUInt16ErrorBase + 5;
            if (expected != Convert.ToUInt16(vt1.sbyte3darr[1, 0, 1])) return ByteToUInt16ErrorBase + 6;
            if (expected != Convert.ToUInt16(cl1.sbyte3darr[1, 0, 1])) return ByteToUInt16ErrorBase + 7;
            if (expected != Convert.ToUInt16(ja2[1][1, 0, 1])) return ByteToUInt16ErrorBase + 8;

            return 0;
        }
    }
}