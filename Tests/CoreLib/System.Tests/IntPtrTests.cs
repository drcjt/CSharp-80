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
    }
}
