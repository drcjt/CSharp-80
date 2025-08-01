using Xunit;

namespace System.Tests
{
    internal static class Int16Tests
    {
        [Fact]
        public static void Ctor_Empty()
        {
            var i = new short();
            Assert.Equal(0, i);
        }

        [Fact]
        public static void MaxValue()
        {
            Assert.Equal(0x7FFF, short.MaxValue);
        }

        [Fact]
        public static void MinValue()
        {
            Assert.Equal(unchecked((short)0x8000), short.MinValue);
        }

        [Theory]
        [InlineData((short)789, (short)789, true)]
        [InlineData((short)789, (short)-789, false)]
        [InlineData((short)789, (short)0, false)]
        [InlineData((short)0, (short)0, true)]
        [InlineData((short)-789, (short)-789, true)]
        [InlineData((short)-789, (short)789, false)]
        [InlineData((short)789, null, false)]
        [InlineData((short)789, "789", false)]
        [InlineData((short)789, 789, false)]
        public static void EqualsTest(short i, object? obj, bool expected)
        {
            if (obj is short j)
            {
                Assert.Equal(expected, i.Equals(j));
                Assert.Equal(expected, i.GetHashCode().Equals(j.GetHashCode()));
            }
            Assert.Equal(expected, i.Equals(obj));
        }
    }
}
