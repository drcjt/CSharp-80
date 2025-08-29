using Xunit;

namespace System.Tests
{
    internal static class UIntPtrTests
    {
        [Fact]
        public static void EqualsTests()
        {
            EqualsTest((nuint)42, (nuint)42, true);
            EqualsTest((nuint)42, (nuint)43, false);
            EqualsTest((nuint)42, 42, false);
            EqualsTest((nuint)42, null, false);
        }

        private static void EqualsTest(nuint value, object? obj, bool expected)
        {
            if (obj is nuint other)
            {
                Assert.Equal(expected, value == other);
                Assert.Equal(!expected, value != other);
                Assert.Equal(expected, value.GetHashCode().Equals(other.GetHashCode()));

                IEquatable<nuint> iEquatable = value;
                Assert.Equal(expected, iEquatable.Equals(other));

            }
            Assert.Equal(expected, value.Equals(obj));
            Assert.Equal(value.GetHashCode(), value.GetHashCode());
        }

        [Theory]
        [InlineData(234u, 234u, 0)]
        [InlineData(234u, uint.MinValue, 1)]
        [InlineData(234u, 123u, 1)]
        [InlineData(234u, 456u, -1)]
        [InlineData(234u, uint.MaxValue, -1)]
        [InlineData(234u, null, 1)]
        public static void CompareTo_Other_ReturnsExpected(uint i0, object? value, int expected)
        {
            nuint i = i0;
            if (value is uint uintValue)
            {
                nuint uintPtrValue = uintValue;
                Assert.Equal(expected, Math.Sign(i.CompareTo(uintPtrValue)));

                Assert.Equal(expected, Math.Sign(i.CompareTo((object)uintPtrValue)));
            }
            else
            {
                Assert.Equal(expected, Math.Sign(i.CompareTo(value)));
            }
        }

        [Theory]
        [InlineData("a")]
        [InlineData(234)]
        public static void CompareTo_ObjectNotnuint_ThrowsArgumentException(object value)
        {
            try
            {
                ((nuint)123).CompareTo(value);
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail("");
        }

    }
}
