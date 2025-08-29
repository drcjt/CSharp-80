using Xunit;

namespace System.Tests
{
    internal static class IntPtrTests
    {
        [Fact]
        public static void EqualsTests()
        {
            EqualsTest((nint)42, (nint)42, true);
            EqualsTest((nint)42, (nint)43, false);
            EqualsTest((nint)42, 42, false);
            EqualsTest((nint)42, null, false);
        }

        private static void EqualsTest(nint value, object? obj, bool expected)
        {
            if (obj is nint other)
            {
                Assert.Equal(expected, value == other);
                Assert.Equal(!expected, value != other);
                Assert.Equal(expected, value.GetHashCode().Equals(other.GetHashCode()));

                IEquatable<nint> iEquatable = value;
                Assert.Equal(expected, iEquatable.Equals(other));

            }
            Assert.Equal(expected, value.Equals(obj));
            Assert.Equal(value.GetHashCode(), value.GetHashCode());
        }

        [Theory]
        [InlineData(234, 234, 0)]
        [InlineData(234, 0x8000, 1)]
        [InlineData(-234, 0x8000, 1)]
        [InlineData(0x8000, 0x8000, 0)]
        [InlineData(234, -123, 1)]
        [InlineData(234, 0, 1)]
        [InlineData(234, 123, 1)]
        [InlineData(234, 456, -1)]
        [InlineData(234, 0x7FFF, -1)]
        [InlineData(-234, 0x7FFF, -1)]
        [InlineData(0x7FFF, 0x7FFF, 0)]
        [InlineData(-234, -234, 0)]
        [InlineData(-234, 234, -1)]
        [InlineData(-234, -432, 1)]
        [InlineData(234, null, 1)]
        public static void CompareTo_Other_ReturnsExpected(int l, object? value, int expected)
        {
            nint i = l;
            if (value is int intValue)
            {
                nint intPtrValue = intValue;
                Assert.Equal(expected, Math.Sign(i.CompareTo(intPtrValue)));
                Assert.Equal(-expected, Math.Sign(intPtrValue.CompareTo(i)));

                Assert.Equal(expected, Math.Sign(i.CompareTo((object)intPtrValue)));
            }
            else
            {
                Assert.Equal(expected, Math.Sign(i.CompareTo(value)));
            }
        }

        [Theory]
        [InlineData("a")]
        public static void CompareTo_ObjectNotIntPtr_ThrowsArgumentException(object value)
        {
            try
            {
                ((nint)123).CompareTo(value);
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail("");
        }
    }
}
