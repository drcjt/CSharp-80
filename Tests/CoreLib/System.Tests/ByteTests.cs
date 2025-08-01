using Xunit;

namespace System.Tests
{
    internal static class ByteTests
    {
        [Fact]
        public static void Ctor_Empty()
        {
            var i = new byte();
            Assert.Equal((byte)0, i);
        }

        [Fact]
        public static void MaxValue()
        {
            Assert.Equal((byte)0xFF, byte.MaxValue);
        }

        [Fact]
        public static void MinValue()
        {
            Assert.Equal((byte)0, byte.MinValue);
        }

        [Theory]
        [InlineData(78, (byte)78, true)]
        [InlineData(78, (byte)0, false)]
        [InlineData(0, (byte)0, true)]
        [InlineData(78, null, false)]
        [InlineData(78, "78", false)]
        [InlineData(78, 78, false)]
        public static void EqualsTest(byte i, object? obj, bool expected)
        {
            if (obj is byte j)
            {
                Assert.Equal(expected, i.Equals(j));
                Assert.Equal(expected, i.GetHashCode().Equals(j.GetHashCode()));
                Assert.Equal(i, i.GetHashCode());
            }
            Assert.Equal(expected, i.Equals(obj));
        }
    }
}
