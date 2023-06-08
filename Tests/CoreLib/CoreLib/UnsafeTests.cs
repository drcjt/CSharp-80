using Internal.Runtime.CompilerServices;

namespace CoreLib
{
    public class UnsafeTests
    {
        public static void SizeOfTests()
        {
            Assert.Equals(1, Unsafe.SizeOf<sbyte>());
            Assert.Equals(1, Unsafe.SizeOf<byte>());
            Assert.Equals(2, Unsafe.SizeOf<short>());
            Assert.Equals(2, Unsafe.SizeOf<ushort>());
            Assert.Equals(4, Unsafe.SizeOf<int>());
            Assert.Equals(4, Unsafe.SizeOf<uint>());
            Assert.Equals(4, Unsafe.SizeOf<Byte4>());
            Assert.Equals(8, Unsafe.SizeOf<Byte4Short2>());
        }

        private static unsafe void InitBlockStack(int numBytes, byte value)
        {
            byte* stackPtr = stackalloc byte[numBytes];
            Unsafe.InitBlock(stackPtr, value, (uint)numBytes);
            for (int i = 0; i < numBytes; i++) 
            {
                Assert.Equals(value, stackPtr[i]);
            }
        }

        public unsafe static void InitBlockTests()
        {
            // TODO: Fix this when bug with stackalloc of size 0 fixed
            InitBlockStack(0, 1);
            InitBlockStack(1, 1);
            InitBlockStack(10, 0);           
            InitBlockStack(10, 2);
            InitBlockStack(10, 255);
            InitBlockStack(1000, 255);
        }

        private static unsafe void CopyBlockStack(int numBytes)
        {
            byte* source = stackalloc byte[numBytes];
            byte* destination = stackalloc byte[numBytes];

            for (int i = 0; i < numBytes; i++)
            {
                byte value = (byte)(i & 255);
                source[i] = value;
            }

            Unsafe.CopyBlock(destination, source, (uint)numBytes);

            for (int i = 0; i < numBytes; i++)
            {
                byte value = (byte)(i & 255);
                Assert.Equals(value, destination[i]);
                Assert.Equals(source[i], destination[i]);
            }
        }

        public unsafe static void CopyBlockTests()
        {
            CopyBlockStack(0);
            CopyBlockStack(1);
            CopyBlockStack(10);
            CopyBlockStack(100);
            CopyBlockStack(1000);
        }

        public static void RefAs()
        {
            TestBytes testBytes = new TestBytes();
            testBytes.B0 = 0x42;
            testBytes.B1 = 0x42;
            testBytes.B2 = 0x42;
            testBytes.B3 = 0x42;

            ref int r = ref Unsafe.As<byte, int>(ref testBytes.B0);
            Assert.Equals(0x42424242, r);

            byte[] b = new byte[4] { 0x42, 0x42, 0x42, 0x42 };
            ref int r2 = ref Unsafe.As<byte, int>(ref b[0]);

            Assert.Equals(0x42424242, r2);
        }
    }

    public class TestBytes
    {
        public byte B0;
        public byte B1;
        public byte B2;
        public byte B3;
    }

    public struct Byte4
    {
        public byte B0;
        public byte B1;
        public byte B2;
        public byte B3;
    }

    public struct Byte4Short2
    {
        public byte B0;
        public byte B1;
        public byte B2;
        public byte B3;
        public short S4;
        public short S6;
    }
}
