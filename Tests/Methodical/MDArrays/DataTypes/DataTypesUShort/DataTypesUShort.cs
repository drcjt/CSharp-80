using System;

namespace MDArrays.@uushort
{
    public struct VT
    {
        public ushort[,] ushort2darr;
        public ushort[,,] ushort3darr;
        public ushort[,] ushort2darr_b;
        public ushort[,,] ushort3darr_b;
    }

    public class CL
    {
        public readonly ushort[,] ushort2darr = { { 0, 1 }, { 0, 0 } };
        public readonly ushort[,,] ushort3darr = { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
        public readonly ushort[,] ushort2darr_b = { { 0, 49 }, { 0, 0 } };
        public readonly ushort[,,] ushort3darr_b = { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };
    }

    internal static class DataTypesushort
    {
        static readonly ushort[,] ushort2darr = { { 0, 1 }, { 0, 0 } };
        static readonly ushort[,,] ushort3darr = { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
        static readonly ushort[,] ushort2darr_b = { { 0, 49 }, { 0, 0 } };
        static readonly ushort[,,] ushort3darr_b = { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };

        static readonly ushort[][,] ja1 = new ushort[2][,];
        static readonly ushort[][,,] ja2 = new ushort[2][,,];
        static readonly ushort[][,] ja1_b = new ushort[2][,];
        static readonly ushort[][,,] ja2_b = new ushort[2][,,];

        public static int Main()
        {
            VT vt1;
            vt1.ushort2darr = new ushort[,] { { 0, 1 }, { 0, 0 } };
            vt1.ushort3darr = new ushort[,,] { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
            vt1.ushort2darr_b = new ushort[,] { { 0, 49 }, { 0, 0 } };
            vt1.ushort3darr_b = new ushort[,,] { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };

            CL cl1 = new();

            ja1[0] = new ushort[,] { { 0, 1 }, { 0, 0 } };
            ja2[1] = new ushort[,,] { { { 0, 0 } }, { { 0, 1 } }, { { 0, 0 } } };
            ja1_b[0] = new ushort[,] { { 0, 49 }, { 0, 0 } };
            ja2_b[1] = new ushort[,,] { { { 0, 0 } }, { { 0, 49 } }, { { 0, 0 } } };

            int result = UInt16Tests(vt1, cl1);
            if (result != 0) return result;

            result = UInt16ToByteTests(vt1, cl1);
            if (result != 0) return result;

            result = UInt16ToInt32Tests(vt1, cl1);
            if (result != 0) return result;

            result = UInt16ToSByteTests(vt1, cl1);
            if (result != 0) return result;

            result = UInt16ToUInt32Tests(vt1, cl1);
            if (result != 0) return result;

            result = UInt16ToInt16Tests(vt1, cl1);
            if (result != 0) return result;

            result = UInt16ToCharTests(vt1, cl1);
            if (result != 0) return result;

            result = UInt16ToBoolTests(vt1, cl1);
            if (result != 0) return result;

            return 0;
        }

        private static int UInt16Tests(VT vt1, CL cl1)
        {
            const int uint16ErrorBase = 100;

            ushort expected = 1;
            // 2d
            if (expected != ushort2darr[0, 1]) return uint16ErrorBase + 1;
            if (expected != vt1.ushort2darr[0, 1]) return uint16ErrorBase + 2;
            if (expected != cl1.ushort2darr[0, 1]) return uint16ErrorBase + 3;
            if (expected != ja1[0][0, 1]) return uint16ErrorBase + 4;

            // 3d
            if (expected != ushort3darr[1, 0, 1]) return uint16ErrorBase + 5;
            if (expected != vt1.ushort3darr[1, 0, 1]) return uint16ErrorBase + 6;
            if (expected != cl1.ushort3darr[1, 0, 1]) return uint16ErrorBase + 7;
            if (expected != ja2[1][1, 0, 1]) return uint16ErrorBase + 8;

            return 0;
        }

        private static int UInt16ToByteTests(VT vt1, CL cl1)
        {
            const int uint16ToByteErrorBase = 200;

            byte expected = 1;

            // 2d
            if (expected != Convert.ToByte(ushort2darr[0, 1])) return uint16ToByteErrorBase + 1;
            if (expected != Convert.ToByte(vt1.ushort2darr[0, 1])) return uint16ToByteErrorBase + 2;
            if (expected != Convert.ToByte(cl1.ushort2darr[0, 1])) return uint16ToByteErrorBase + 3;
            if (expected != Convert.ToByte(ja1[0][0, 1])) return uint16ToByteErrorBase + 4;

            // 3d
            if (expected != Convert.ToByte(ushort3darr[1, 0, 1])) return uint16ToByteErrorBase + 5;
            if (expected != Convert.ToByte(vt1.ushort3darr[1, 0, 1])) return uint16ToByteErrorBase + 6;
            if (expected != Convert.ToByte(cl1.ushort3darr[1, 0, 1])) return uint16ToByteErrorBase + 7;
            if (expected != Convert.ToByte(ja2[1][1, 0, 1])) return uint16ToByteErrorBase + 8;

            return 0;
        }

        private static int UInt16ToInt32Tests(VT vt1, CL cl1)
        {
            const int uint16ToInt32ErrorBase = 300;

            int expected = 1;

            // 2d
            if (expected != Convert.ToInt32(ushort2darr[0, 1])) return uint16ToInt32ErrorBase + 1;
            if (expected != Convert.ToInt32(vt1.ushort2darr[0, 1])) return uint16ToInt32ErrorBase + 2;
            if (expected != Convert.ToInt32(cl1.ushort2darr[0, 1])) return uint16ToInt32ErrorBase + 3;
            if (expected != Convert.ToInt32(ja1[0][0, 1])) return uint16ToInt32ErrorBase + 4;

            // 3d
            if (expected != Convert.ToInt32(ushort3darr[1, 0, 1])) return uint16ToInt32ErrorBase + 5;
            if (expected != Convert.ToInt32(vt1.ushort3darr[1, 0, 1])) return uint16ToInt32ErrorBase + 6;
            if (expected != Convert.ToInt32(cl1.ushort3darr[1, 0, 1])) return uint16ToInt32ErrorBase + 7;
            if (expected != Convert.ToInt32(ja2[1][1, 0, 1])) return uint16ToInt32ErrorBase + 8;

            return 0;
        }

        private static int UInt16ToSByteTests(VT vt1, CL cl1)
        {
            const int uint16ToSByteErrorBase = 400;

            sbyte expected = 1;

            // 2d
            if (expected != Convert.ToSByte(ushort2darr[0, 1])) return uint16ToSByteErrorBase + 1;
            if (expected != Convert.ToSByte(vt1.ushort2darr[0, 1])) return uint16ToSByteErrorBase + 2;
            if (expected != Convert.ToSByte(cl1.ushort2darr[0, 1])) return uint16ToSByteErrorBase + 3;
            if (expected != Convert.ToSByte(ja1[0][0, 1])) return uint16ToSByteErrorBase + 4;

            // 3d
            if (expected != Convert.ToSByte(ushort3darr[1, 0, 1])) return uint16ToSByteErrorBase + 5;
            if (expected != Convert.ToSByte(vt1.ushort3darr[1, 0, 1])) return uint16ToSByteErrorBase + 6;
            if (expected != Convert.ToSByte(cl1.ushort3darr[1, 0, 1])) return uint16ToSByteErrorBase + 7;
            if (expected != Convert.ToSByte(ja2[1][1, 0, 1])) return uint16ToSByteErrorBase + 8;

            return 0;
        }

        private static int UInt16ToUInt32Tests(VT vt1, CL cl1)
        {
            const int uInt16ToUInt32ErrorBase = 500;

            uint expected = 1;

            // 2d
            if (expected != Convert.ToUInt32(ushort2darr[0, 1])) return uInt16ToUInt32ErrorBase + 1;
            if (expected != Convert.ToUInt32(vt1.ushort2darr[0, 1])) return uInt16ToUInt32ErrorBase + 2;
            if (expected != Convert.ToUInt32(cl1.ushort2darr[0, 1])) return uInt16ToUInt32ErrorBase + 3;
            if (expected != Convert.ToUInt32(ja1[0][0, 1])) return uInt16ToUInt32ErrorBase + 4;

            // 3d
            if (expected != Convert.ToUInt32(ushort3darr[1, 0, 1])) return uInt16ToUInt32ErrorBase + 5;
            if (expected != Convert.ToUInt32(vt1.ushort3darr[1, 0, 1])) return uInt16ToUInt32ErrorBase + 6;
            if (expected != Convert.ToUInt32(cl1.ushort3darr[1, 0, 1])) return uInt16ToUInt32ErrorBase + 7;
            if (expected != Convert.ToUInt32(ja2[1][1, 0, 1])) return uInt16ToUInt32ErrorBase + 8;

            return 0;
        }

        private static int UInt16ToInt16Tests(VT vt1, CL cl1)
        {
            const int UInt16ToInt16ErrorBase = 600;

            short expected = 1;

            // 2d
            if (expected != Convert.ToInt16(ushort2darr[0, 1])) return UInt16ToInt16ErrorBase + 1;
            if (expected != Convert.ToInt16(vt1.ushort2darr[0, 1])) return UInt16ToInt16ErrorBase + 2;
            if (expected != Convert.ToInt16(cl1.ushort2darr[0, 1])) return UInt16ToInt16ErrorBase + 3;
            if (expected != Convert.ToInt16(ja1[0][0, 1])) return UInt16ToInt16ErrorBase + 4;

            // 3d
            if (expected != Convert.ToInt16(ushort3darr[1, 0, 1])) return UInt16ToInt16ErrorBase + 5;
            if (expected != Convert.ToInt16(vt1.ushort3darr[1, 0, 1])) return UInt16ToInt16ErrorBase + 6;
            if (expected != Convert.ToInt16(cl1.ushort3darr[1, 0, 1])) return UInt16ToInt16ErrorBase + 7;
            if (expected != Convert.ToInt16(ja2[1][1, 0, 1])) return UInt16ToInt16ErrorBase + 8;

            return 0;
        }

        private static int UInt16ToCharTests(VT vt1, CL cl1)
        {
            const int uint16ToCharErrorBase = 700;

            char expected = '1';

            // 2d
            if (expected != Convert.ToChar(ushort2darr_b[0, 1])) return uint16ToCharErrorBase + 1;
            if (expected != Convert.ToChar(vt1.ushort2darr_b[0, 1])) return uint16ToCharErrorBase + 2;
            if (expected != Convert.ToChar(cl1.ushort2darr_b[0, 1])) return uint16ToCharErrorBase + 3;
            if (expected != Convert.ToChar(ja1_b[0][0, 1])) return uint16ToCharErrorBase + 4;

            // 3d
            if (expected != Convert.ToChar(ushort3darr_b[1, 0, 1])) return uint16ToCharErrorBase + 5;
            if (expected != Convert.ToChar(vt1.ushort3darr_b[1, 0, 1])) return uint16ToCharErrorBase + 6;
            if (expected != Convert.ToChar(cl1.ushort3darr_b[1, 0, 1])) return uint16ToCharErrorBase + 7;
            if (expected != Convert.ToChar(ja2_b[1][1, 0, 1])) return uint16ToCharErrorBase + 8;

            return 0;
        }

        private static int UInt16ToBoolTests(VT vt1, CL cl1)
        {
            const int uint16ToBoolErrorBase = 800;

            bool expected = true;

            // 2d
            if (expected != Convert.ToBoolean(ushort2darr[0, 1])) return uint16ToBoolErrorBase + 1;
            if (expected != Convert.ToBoolean(vt1.ushort2darr[0, 1])) return uint16ToBoolErrorBase + 2;
            if (expected != Convert.ToBoolean(cl1.ushort2darr[0, 1])) return uint16ToBoolErrorBase + 3;
            if (expected != Convert.ToBoolean(ja1[0][0, 1])) return uint16ToBoolErrorBase + 4;

            // 3d
            if (expected != Convert.ToBoolean(ushort3darr[1, 0, 1])) return uint16ToBoolErrorBase + 5;
            if (expected != Convert.ToBoolean(vt1.ushort3darr[1, 0, 1])) return uint16ToBoolErrorBase + 6;
            if (expected != Convert.ToBoolean(cl1.ushort3darr[1, 0, 1])) return uint16ToBoolErrorBase + 7;
            if (expected != Convert.ToBoolean(ja2[1][1, 0, 1])) return uint16ToBoolErrorBase + 8;

            return 0;
        }
    }
}
