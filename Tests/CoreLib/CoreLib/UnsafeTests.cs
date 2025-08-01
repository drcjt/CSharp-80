using Internal.Runtime.CompilerServices;
using Xunit;

namespace CoreLib
{
    public class UnsafeTests
    {
        [Fact]
        public static void SizeOfTests()
        {
            Assert.Equal(1, Unsafe.SizeOf<sbyte>());
            Assert.Equal(1, Unsafe.SizeOf<byte>());
            Assert.Equal(2, Unsafe.SizeOf<short>());
            Assert.Equal(2, Unsafe.SizeOf<ushort>());
            Assert.Equal(4, Unsafe.SizeOf<int>());
            Assert.Equal(4, Unsafe.SizeOf<uint>());
            Assert.Equal(4, Unsafe.SizeOf<Byte4>());
            Assert.Equal(8, Unsafe.SizeOf<Byte4Short2>());
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(10, 0)]
        [InlineData(10, 2)]
        [InlineData(10, 255)]
        [InlineData(1000, 255)]
        public static unsafe void InitBlockStack(int numBytes, byte value)
        {
            byte* stackPtr = stackalloc byte[numBytes];
            Unsafe.InitBlock(stackPtr, value, (uint)numBytes);
            for (int i = 0; i < numBytes; i++) 
            {
                Assert.Equal(value, stackPtr[i]);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public static unsafe void CopyBlockStack(int numBytes)
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
                Assert.Equal(value, destination[i]);
                Assert.Equal(source[i], destination[i]);
            }
        }

        [Fact]
        public static void RefAs()
        {
            TestBytes testBytes = new TestBytes();
            testBytes.B0 = 0x42;
            testBytes.B1 = 0x42;
            testBytes.B2 = 0x42;
            testBytes.B3 = 0x42;

            ref int r = ref Unsafe.As<byte, int>(ref testBytes.B0);
            Assert.Equal(0x42424242, r);

            byte[] b = new byte[4] { 0x42, 0x42, 0x42, 0x42 };
            ref int r2 = ref Unsafe.As<byte, int>(ref b[0]);

            Assert.Equal(0x42424242, r2);
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
