using Xunit;

namespace System.Tests
{
    internal static class UInt16Tests
    {
        [Fact]
        public static void Ctor_Empty()
        {
            var i = new ushort();
            Assert.Equal(0, i);
        }

        [Fact]
        public static void MaxValue()
        {
            Assert.Equal(0xFFFF, ushort.MaxValue);
        }

        [Fact]
        public static void MinValue()
        {
            Assert.Equal(0, ushort.MinValue);
        }

        [Theory]
        [InlineData((ushort)789, (ushort)789, true)]
        [InlineData((ushort)789, (ushort)0, false)]
        [InlineData((ushort)0, (ushort)0, true)]
        [InlineData((ushort)789, null, false)]
        [InlineData((ushort)789, "789", false)]
        [InlineData((ushort)789, 789, false)]
        public static void EqualsTest(ushort i, object? obj, bool expected)
        {
            if (obj is ushort j)
            {
                Assert.Equal(expected, i.Equals(j));
                Assert.Equal(expected, i.GetHashCode().Equals(j.GetHashCode()));
                Assert.Equal(i, i.GetHashCode());
            }
            Assert.Equal(expected, i.Equals(obj));
        }

        [Theory]
        [InlineData((ushort)234, (ushort)234, 0)]
        [InlineData((ushort)234, (ushort)0, 1)]
        [InlineData((ushort)234, (ushort)123, 1)]
        [InlineData((ushort)234, (ushort)456, -1)]
        [InlineData((ushort)234, (ushort)0xFFFF, -1)]
        [InlineData((ushort)234, null, 1)]
        public static void CompareTo_Other_ReturnsExpected(ushort i, object? value, int expected)
        {
            if (value is ushort ushortValue)
            {
                Assert.Equal(expected, Math.Sign(i.CompareTo(ushortValue)));
            }

            Assert.Equal(expected, Math.Sign(i.CompareTo(value)));
        }

        [Theory]
        [InlineData("a")]
        [InlineData(234)]
        public static void CompareTo_ObjectNotUshort_ThrowsArgumentException(object value)
        {
            try
            {
                ((ushort)123).CompareTo(value);
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail("");
        }
    }
}
