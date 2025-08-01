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
    }
}
