using Xunit;

namespace System.Tests
{
    internal static class UInt32Tests
    {
        public static void Ctor_Empty()
        {
            var i = new uint();
            Assert.Equal((uint)0, i);
        }

        public static void MaxValue()
        {
            Assert.Equal(4294967295, uint.MaxValue);
        }

        public static void MinValue()
        {
            Assert.Equal((uint)0, uint.MinValue);
        }

        public static void EqualsTests()
        {
            EqualsTest((uint)789, (uint)789, true);
            EqualsTest((uint)789, (uint)0, false);
            EqualsTest((uint)0, (uint)0, true);
            EqualsTest((uint)789, null, false);
            EqualsTest((uint)789, "789", false);
            EqualsTest((uint)789, 789, false);
        }

        private static void EqualsTest(uint i, object? obj, bool expected)
        {
            if (obj is uint j)
            {
                Assert.Equal(expected, i.Equals(j));
                Assert.Equal(expected, i.GetHashCode().Equals(j.GetHashCode()));
                Assert.Equal((int)i, i.GetHashCode());
            }
            Assert.Equal(expected, i.Equals(obj));
        }
    }
}
