namespace System.Tests
{
    internal static class UIntPtrTests
    {
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
                Assert.AreEqual(expected, value == other);
                Assert.AreEqual(!expected, value != other);
                Assert.AreEqual(expected, value.GetHashCode().Equals(other.GetHashCode()));

                IEquatable<nuint> iEquatable = value;
                Assert.AreEqual(expected, iEquatable.Equals(other));

            }
            Assert.AreEqual(expected, value.Equals(obj));
            Assert.AreEqual(value.GetHashCode(), value.GetHashCode());
        }
    }
}
