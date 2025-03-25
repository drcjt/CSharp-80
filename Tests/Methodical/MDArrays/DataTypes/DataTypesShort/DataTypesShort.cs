using System;

namespace MDArrays.@short
{
    public struct VT
    {
        public short[,] short2darr;
        public short[,,] short3darr;
        public short[,] short2darr_b;
        public short[,,] short3darr_b;
    }

    public class CL
    {
        public readonly short[,] short2darr = { { 0, 1 }, { 0, 0 } };
        public readonly short[,,] short3darr = { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
        public readonly short[,] short2darr_b = { { 0, 49 }, { 0, 0 } };
        public readonly short[,,] short3darr_b = { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };
    }

    internal static class DataTypesshort
    {
        static readonly short[,] short2darr = { { 0, 1 }, { 0, 0 } };
        static readonly short[,,] short3darr = { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
        static readonly short[,] short2darr_b = { { 0, 49 }, { 0, 0 } };
        static readonly short[,,] short3darr_b = { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };

        static readonly short[][,] ja1 = new short[2][,];
        static readonly short[][,,] ja2 = new short[2][,,];
        static readonly short[][,] ja1_b = new short[2][,];
        static readonly short[][,,] ja2_b = new short[2][,,];

        public static int Main()
        {
            VT vt1;
            vt1.short2darr = new short[,] { { 0, 1 }, { 0, 0 } };
            vt1.short3darr = new short[,,] { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
            vt1.short2darr_b = new short[,] { { 0, 49 }, { 0, 0 } };
            vt1.short3darr_b = new short[,,] { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };

            CL cl1 = new();

            ja1[0] = new short[,] { { 0, 1 }, { 0, 0 } };
            ja2[1] = new short[,,] { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
            ja1_b[0] = new short[,] { { 0, 49 }, { 0, 0 } };
            ja2_b[1] = new short[,,] { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };

            int result = Int16Tests(vt1, cl1);
            if (result != 0) return result;

            result = Int16ToByteTests(vt1, cl1);
            if (result != 0) return result;

            result = Int16ToInt32Tests(vt1, cl1);
            if (result != 0) return result;

            result = Int16ToSByteTests(vt1, cl1);
            if (result != 0) return result;

            result = Int16ToUInt32Tests(vt1, cl1);
            if (result != 0) return result;

            result = Int16ToUInt16Tests(vt1, cl1);
            if (result != 0) return result;

            result = Int16ToCharTests(vt1, cl1);
            if (result != 0) return result;

            result = Int16ToBoolTests(vt1, cl1);
            if (result != 0) return result;

            return 0;
        }

        private static int Int16Tests(VT vt1, CL cl1)
        {
            const int int16ErrorBase = 100;

            short expected = 1;
            // 2d
            if (expected != short2darr[0, 1]) return int16ErrorBase + 1;
            if (expected != vt1.short2darr[0, 1]) return int16ErrorBase + 2;
            if (expected != cl1.short2darr[0, 1]) return int16ErrorBase + 3;
            if (expected != ja1[0][0, 1]) return int16ErrorBase + 4;

            // 3d
            if (expected != short3darr[1, 0, 1]) return int16ErrorBase + 5;
            if (expected != vt1.short3darr[1, 0, 1]) return int16ErrorBase + 6;
            if (expected != cl1.short3darr[1, 0, 1]) return int16ErrorBase + 7;
            if (expected != ja2[1][1, 0, 1]) return int16ErrorBase + 8;

            return 0;
        }

        private static int Int16ToByteTests(VT vt1, CL cl1)
        {
            const int int16ToByteErrorBase = 200;

            byte expected = 1;

            // 2d
            if (expected != Convert.ToByte(short2darr[0, 1])) return int16ToByteErrorBase + 1;
            if (expected != Convert.ToByte(vt1.short2darr[0, 1])) return int16ToByteErrorBase + 2;
            if (expected != Convert.ToByte(cl1.short2darr[0, 1])) return int16ToByteErrorBase + 3;
            if (expected != Convert.ToByte(ja1[0][0, 1])) return int16ToByteErrorBase + 4;

            // 3d
            if (expected != Convert.ToByte(short3darr[1, 0, 1])) return int16ToByteErrorBase + 5;
            if (expected != Convert.ToByte(vt1.short3darr[1, 0, 1])) return int16ToByteErrorBase + 6;
            if (expected != Convert.ToByte(cl1.short3darr[1, 0, 1])) return int16ToByteErrorBase + 7;
            if (expected != Convert.ToByte(ja2[1][1, 0, 1])) return int16ToByteErrorBase + 8;

            return 0;
        }

        private static int Int16ToInt32Tests(VT vt1, CL cl1)
        {
            const int int16ToInt32ErrorBase = 300;

            int expected = 1;

            // 2d
            if (expected != Convert.ToInt32(short2darr[0, 1])) return int16ToInt32ErrorBase + 1;
            if (expected != Convert.ToInt32(vt1.short2darr[0, 1])) return int16ToInt32ErrorBase + 2;
            if (expected != Convert.ToInt32(cl1.short2darr[0, 1])) return int16ToInt32ErrorBase + 3;
            if (expected != Convert.ToInt32(ja1[0][0, 1])) return int16ToInt32ErrorBase + 4;

            // 3d
            if (expected != Convert.ToInt32(short3darr[1, 0, 1])) return int16ToInt32ErrorBase + 5;
            if (expected != Convert.ToInt32(vt1.short3darr[1, 0, 1])) return int16ToInt32ErrorBase + 6;
            if (expected != Convert.ToInt32(cl1.short3darr[1, 0, 1])) return int16ToInt32ErrorBase + 7;
            if (expected != Convert.ToInt32(ja2[1][1, 0, 1])) return int16ToInt32ErrorBase + 8;

            return 0;
        }

        private static int Int16ToSByteTests(VT vt1, CL cl1)
        {
            const int int16ToSByteErrorBase = 400;

            sbyte expected = 1;

            // 2d
            if (expected != Convert.ToSByte(short2darr[0, 1])) return int16ToSByteErrorBase + 1;
            if (expected != Convert.ToSByte(vt1.short2darr[0, 1])) return int16ToSByteErrorBase + 2;
            if (expected != Convert.ToSByte(cl1.short2darr[0, 1])) return int16ToSByteErrorBase + 3;
            if (expected != Convert.ToSByte(ja1[0][0, 1])) return int16ToSByteErrorBase + 4;

            // 3d
            if (expected != Convert.ToSByte(short3darr[1, 0, 1])) return int16ToSByteErrorBase + 5;
            if (expected != Convert.ToSByte(vt1.short3darr[1, 0, 1])) return int16ToSByteErrorBase + 6;
            if (expected != Convert.ToSByte(cl1.short3darr[1, 0, 1])) return int16ToSByteErrorBase + 7;
            if (expected != Convert.ToSByte(ja2[1][1, 0, 1])) return int16ToSByteErrorBase + 8;

            return 0;
        }

        private static int Int16ToUInt32Tests(VT vt1, CL cl1)
        {
            const int Int16ToUInt32ErrorBase = 500;

            uint expected = 1;

            // 2d
            if (expected != Convert.ToUInt32(short2darr[0, 1])) return Int16ToUInt32ErrorBase + 1;
            if (expected != Convert.ToUInt32(vt1.short2darr[0, 1])) return Int16ToUInt32ErrorBase + 2;
            if (expected != Convert.ToUInt32(cl1.short2darr[0, 1])) return Int16ToUInt32ErrorBase + 3;
            if (expected != Convert.ToUInt32(ja1[0][0, 1])) return Int16ToUInt32ErrorBase + 4;

            // 3d
            if (expected != Convert.ToUInt32(short3darr[1, 0, 1])) return Int16ToUInt32ErrorBase + 5;
            if (expected != Convert.ToUInt32(vt1.short3darr[1, 0, 1])) return Int16ToUInt32ErrorBase + 6;
            if (expected != Convert.ToUInt32(cl1.short3darr[1, 0, 1])) return Int16ToUInt32ErrorBase + 7;
            if (expected != Convert.ToUInt32(ja2[1][1, 0, 1])) return Int16ToUInt32ErrorBase + 8;

            return 0;
        }

        private static int Int16ToUInt16Tests(VT vt1, CL cl1)
        {
            const int Int16ToUInt16ErrorBase = 600;

            ushort expected = 1;

            // 2d
            if (expected != Convert.ToUInt16(short2darr[0, 1])) return Int16ToUInt16ErrorBase + 1;
            if (expected != Convert.ToUInt16(vt1.short2darr[0, 1])) return Int16ToUInt16ErrorBase + 2;
            if (expected != Convert.ToUInt16(cl1.short2darr[0, 1])) return Int16ToUInt16ErrorBase + 3;
            if (expected != Convert.ToUInt16(ja1[0][0, 1])) return Int16ToUInt16ErrorBase + 4;

            // 3d
            if (expected != Convert.ToUInt16(short3darr[1, 0, 1])) return Int16ToUInt16ErrorBase + 5;
            if (expected != Convert.ToUInt16(vt1.short3darr[1, 0, 1])) return Int16ToUInt16ErrorBase + 6;
            if (expected != Convert.ToUInt16(cl1.short3darr[1, 0, 1])) return Int16ToUInt16ErrorBase + 7;
            if (expected != Convert.ToUInt16(ja2[1][1, 0, 1])) return Int16ToUInt16ErrorBase + 8;

            return 0;
        }

        private static int Int16ToCharTests(VT vt1, CL cl1)
        {
            const int int16ToCharErrorBase = 700;

            char expected = '1';

            // 2d
            if (expected != Convert.ToChar(short2darr_b[0, 1])) return int16ToCharErrorBase + 1;
            if (expected != Convert.ToChar(vt1.short2darr_b[0, 1])) return int16ToCharErrorBase + 2;
            if (expected != Convert.ToChar(cl1.short2darr_b[0, 1])) return int16ToCharErrorBase + 3;
            if (expected != Convert.ToChar(ja1_b[0][0, 1])) return int16ToCharErrorBase + 4;

            // 3d
            if (expected != Convert.ToChar(short3darr_b[1, 0, 1])) return int16ToCharErrorBase + 5;
            if (expected != Convert.ToChar(vt1.short3darr_b[1, 0, 1])) return int16ToCharErrorBase + 6;
            if (expected != Convert.ToChar(cl1.short3darr_b[1, 0, 1])) return int16ToCharErrorBase + 7;
            if (expected != Convert.ToChar(ja2_b[1][1, 0, 1])) return int16ToCharErrorBase + 8;

            return 0;
        }

        private static int Int16ToBoolTests(VT vt1, CL cl1)
        {
            const int int16ToBoolErrorBase = 800;

            bool expected = true;

            // 2d
            if (expected != Convert.ToBoolean(short2darr[0, 1])) return int16ToBoolErrorBase + 1;
            if (expected != Convert.ToBoolean(vt1.short2darr[0, 1])) return int16ToBoolErrorBase + 2;
            if (expected != Convert.ToBoolean(cl1.short2darr[0, 1])) return int16ToBoolErrorBase + 3;
            if (expected != Convert.ToBoolean(ja1[0][0, 1])) return int16ToBoolErrorBase + 4;

            // 3d
            if (expected != Convert.ToBoolean(short3darr[1, 0, 1])) return int16ToBoolErrorBase + 5;
            if (expected != Convert.ToBoolean(vt1.short3darr[1, 0, 1])) return int16ToBoolErrorBase + 6;
            if (expected != Convert.ToBoolean(cl1.short3darr[1, 0, 1])) return int16ToBoolErrorBase + 7;
            if (expected != Convert.ToBoolean(ja2[1][1, 0, 1])) return int16ToBoolErrorBase + 8;

            return 0;
        }
    }
}
