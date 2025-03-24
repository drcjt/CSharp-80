using System;

namespace MDArrays.@char
{
    public struct VT
    {
        public char[,] char2darr;
        public char[,,] char3darr;
    }

    public class CL
    {
        public readonly char[,] char2darr = { { '0', '1' }, { '0', '0' } };
        public readonly char[,,] char3darr = { { { '0', '0' } }, { { '0', '1' } }, { { '0', '0' } } };
    }

    internal static class DataTypesChar
    {
        static readonly char[,] char2darr = { { '0', '1' }, { '0', '0' } };
        static readonly char[,,] char3darr = { { { '0', '0' } }, { { '0', '1' } }, { { '0', '0' } } };

        static readonly char[][,] ja1 = new char[2][,];
        static readonly char[][,,] ja2 = new char[2][,,];

        public static int Main()
        {
            VT vt1;
            vt1.char2darr = new char[,] { { '0', '1' }, { '0', '0' } };
            vt1.char3darr = new char[,,] { { { '0', '0' } }, { { '0', '1' } }, { { '0', '0' } } };

            CL cl1 = new();

            ja1[0] = new char[,] { { '0', '1' }, { '0', '0' } };
            ja2[1] = new char[,,] { { { '0', '0' } }, { { '0', '1' } }, { { '0', '0' } } };


            int result = CharTests(vt1, cl1);
            if (result != 0) return result;

            result = CharToByteTests(vt1, cl1);
            if (result != 0) return result;

            result = CharToInt32Tests(vt1, cl1);
            if (result != 0) return result;

            result = CharToSByteTests(vt1, cl1);
            if (result != 0) return result;

            result = CharToInt16Tests(vt1, cl1);
            if (result != 0) return result;

            result = CharToUInt32Tests(vt1, cl1);
            if (result != 0) return result;

            result = CharToUInt16Tests(vt1, cl1);
            if (result != 0) return result;

            return 0;
        }

        private static int CharTests(VT vt1, CL cl1)
        {
            const int CharErrorBase = 100;

            char expected = '1';

            // 2d
            if (expected != char2darr[0, 1]) return CharErrorBase + 1;
            if (expected != vt1.char2darr[0, 1]) return CharErrorBase + 2;
            if (expected != cl1.char2darr[0, 1]) return CharErrorBase + 3;
            if (expected != ja1[0][0, 1]) return CharErrorBase + 4;

            // 3d
            if (expected != char3darr[1, 0, 1]) return CharErrorBase + 5;
            if (expected != vt1.char3darr[1, 0, 1]) return CharErrorBase + 6;
            if (expected != cl1.char3darr[1, 0, 1]) return CharErrorBase + 7;
            if (expected != ja2[1][1, 0, 1]) return CharErrorBase + 8;

            return 0;
        }

        private static int CharToByteTests(VT vt1, CL cl1)
        {
            const int CharToByteErrorBase = 200;

            byte expected = 49;

            // 2d
            if (expected != Convert.ToByte(char2darr[0, 1])) return CharToByteErrorBase + 1;
            if (expected != Convert.ToByte(vt1.char2darr[0, 1])) return CharToByteErrorBase + 2;
            if (expected != Convert.ToByte(cl1.char2darr[0, 1])) return CharToByteErrorBase + 3;
            if (expected != Convert.ToByte(ja1[0][0, 1])) return CharToByteErrorBase + 4;

            // 3d
            if (expected != Convert.ToByte(char3darr[1, 0, 1])) return CharToByteErrorBase + 5;
            if (expected != Convert.ToByte(vt1.char3darr[1, 0, 1])) return CharToByteErrorBase + 6;
            if (expected != Convert.ToByte(cl1.char3darr[1, 0, 1])) return CharToByteErrorBase + 7;
            if (expected != Convert.ToByte(ja2[1][1, 0, 1])) return CharToByteErrorBase + 8;

            return 0;
        }

        private static int CharToInt32Tests(VT vt1, CL cl1)
        {
            const int CharToInt32ErrorBase = 300;

            int expected = 49;

            // 2d
            if (expected != Convert.ToInt32(char2darr[0, 1])) return CharToInt32ErrorBase + 1;
            if (expected != Convert.ToInt32(vt1.char2darr[0, 1])) return CharToInt32ErrorBase + 2;
            if (expected != Convert.ToInt32(cl1.char2darr[0, 1])) return CharToInt32ErrorBase + 3;
            if (expected != Convert.ToInt32(ja1[0][0, 1])) return CharToInt32ErrorBase + 4;

            // 3d
            if (expected != Convert.ToInt32(char3darr[1, 0, 1])) return CharToInt32ErrorBase + 5;
            if (expected != Convert.ToInt32(vt1.char3darr[1, 0, 1])) return CharToInt32ErrorBase + 6;
            if (expected != Convert.ToInt32(cl1.char3darr[1, 0, 1])) return CharToInt32ErrorBase + 7;
            if (expected != Convert.ToInt32(ja2[1][1, 0, 1])) return CharToInt32ErrorBase + 8;

            return 0;
        }

        private static int CharToSByteTests(VT vt1, CL cl1)
        {
            const int CharToSByteErrorBase = 400;

            sbyte expected = 49;

            // 2d
            if (expected != Convert.ToSByte(char2darr[0, 1])) return CharToSByteErrorBase + 1;
            if (expected != Convert.ToSByte(vt1.char2darr[0, 1])) return CharToSByteErrorBase + 2;
            if (expected != Convert.ToSByte(cl1.char2darr[0, 1])) return CharToSByteErrorBase + 3;
            if (expected != Convert.ToSByte(ja1[0][0, 1])) return CharToSByteErrorBase + 4;

            // 3d
            if (expected != Convert.ToSByte(char3darr[1, 0, 1])) return CharToSByteErrorBase + 5;
            if (expected != Convert.ToSByte(vt1.char3darr[1, 0, 1])) return CharToSByteErrorBase + 6;
            if (expected != Convert.ToSByte(cl1.char3darr[1, 0, 1])) return CharToSByteErrorBase + 7;
            if (expected != Convert.ToSByte(ja2[1][1, 0, 1])) return CharToSByteErrorBase + 8;

            return 0;
        }

        private static int CharToInt16Tests(VT vt1, CL cl1)
        {
            const int CharToInt16ErrorBase = 500;

            short expected = 49;

            // 2d
            if (expected != Convert.ToInt16(char2darr[0, 1])) return CharToInt16ErrorBase + 1;
            if (expected != Convert.ToInt16(vt1.char2darr[0, 1])) return CharToInt16ErrorBase + 2;
            if (expected != Convert.ToInt16(cl1.char2darr[0, 1])) return CharToInt16ErrorBase + 3;
            if (expected != Convert.ToInt16(ja1[0][0, 1])) return CharToInt16ErrorBase + 4;

            // 3d
            if (expected != Convert.ToInt16(char3darr[1, 0, 1])) return CharToInt16ErrorBase + 5;
            if (expected != Convert.ToInt16(vt1.char3darr[1, 0, 1])) return CharToInt16ErrorBase + 6;
            if (expected != Convert.ToInt16(cl1.char3darr[1, 0, 1])) return CharToInt16ErrorBase + 7;
            if (expected != Convert.ToInt16(ja2[1][1, 0, 1])) return CharToInt16ErrorBase + 8;

            return 0;
        }

        private static int CharToUInt32Tests(VT vt1, CL cl1)
        {
            const int CharToUInt32ErrorBase = 600;

            uint expected = 49;

            // 2d
            if (expected != Convert.ToUInt32(char2darr[0, 1])) return CharToUInt32ErrorBase + 1;
            if (expected != Convert.ToUInt32(vt1.char2darr[0, 1])) return CharToUInt32ErrorBase + 2;
            if (expected != Convert.ToUInt32(cl1.char2darr[0, 1])) return CharToUInt32ErrorBase + 3;
            if (expected != Convert.ToUInt32(ja1[0][0, 1])) return CharToUInt32ErrorBase + 4;

            // 3d
            if (expected != Convert.ToUInt32(char3darr[1, 0, 1])) return CharToUInt32ErrorBase + 5;
            if (expected != Convert.ToUInt32(vt1.char3darr[1, 0, 1])) return CharToUInt32ErrorBase + 6;
            if (expected != Convert.ToUInt32(cl1.char3darr[1, 0, 1])) return CharToUInt32ErrorBase + 7;
            if (expected != Convert.ToUInt32(ja2[1][1, 0, 1])) return CharToUInt32ErrorBase + 8;

            return 0;
        }

        private static int CharToUInt16Tests(VT vt1, CL cl1)
        {
            const int CharToUInt16ErrorBase = 700;

            ushort expected = 49;

            // 2d
            if (expected != Convert.ToUInt16(char2darr[0, 1])) return CharToUInt16ErrorBase + 1;
            if (expected != Convert.ToUInt16(vt1.char2darr[0, 1])) return CharToUInt16ErrorBase + 2;
            if (expected != Convert.ToUInt16(cl1.char2darr[0, 1])) return CharToUInt16ErrorBase + 3;
            if (expected != Convert.ToUInt16(ja1[0][0, 1])) return CharToUInt16ErrorBase + 4;

            // 3d
            if (expected != Convert.ToUInt16(char3darr[1, 0, 1])) return CharToUInt16ErrorBase + 5;
            if (expected != Convert.ToUInt16(vt1.char3darr[1, 0, 1])) return CharToUInt16ErrorBase + 6;
            if (expected != Convert.ToUInt16(cl1.char3darr[1, 0, 1])) return CharToUInt16ErrorBase + 7;
            if (expected != Convert.ToUInt16(ja2[1][1, 0, 1])) return CharToUInt16ErrorBase + 8;

            return 0;
        }
    }
}
