namespace System.Tests
{
    internal static class IntPtrTests
    {
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
                Assert.AreEqual(expected, value == other);
                Assert.AreEqual(!expected, value != other);
                Assert.AreEqual(expected, value.GetHashCode().Equals(other.GetHashCode()));
            }
            Assert.AreEqual(expected, value.Equals(obj));
            Assert.AreEqual(value.GetHashCode(), value.GetHashCode());
        }
    }
}
