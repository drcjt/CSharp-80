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

        [Theory]
        [InlineData((short)234, (short)234, 0)]
        [InlineData((short)234, short.MinValue, 1)]
        [InlineData((short)-234, short.MinValue, 1)]
        [InlineData(short.MinValue, short.MinValue, 0)]
        [InlineData((short)234, (short)-123, 1)]
        [InlineData((short)234, (short)0, 1)]
        [InlineData((short)234, (short)123, 1)]
        [InlineData((short)234, (short)456, -1)]
        [InlineData((short)234, short.MaxValue, -1)]
        [InlineData((short)-234, short.MaxValue, -1)]
        [InlineData(short.MaxValue, short.MaxValue, 0)]
        [InlineData((short)-234, (short)-234, 0)]
        [InlineData((short)-234, (short)234, -1)]
        [InlineData((short)-234, (short)-432, 1)]
        [InlineData((short)234, null, 1)]
        public static void CompareTo_Other_ReturnsExpected(short i, object? value, int expected)
        {
            if (value is short shortValue)
            {
                Assert.Equal(expected, Math.Sign(i.CompareTo(shortValue)));
                Assert.Equal(-expected, Math.Sign(shortValue.CompareTo(i)));
            }

            Assert.Equal(expected, Math.Sign(i.CompareTo(value)));
        }

        [Theory]
        [InlineData("a")]
        [InlineData(234)]
        public static void CompareTo_ObjectNotShort_ThrowsArgumentException(object value)
        {
            try
            {
                _ = ((short)123).CompareTo(value);
            }
            catch (ArgumentException)
            {
                return;
            }
            Assert.Fail("");
        }
    }
}
