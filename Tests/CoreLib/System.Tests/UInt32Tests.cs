using Xunit;

namespace System.Tests
{
    internal static class UInt32Tests
    {
        [Fact]
        public static void Ctor_Empty()
        {
            var i = new uint();
            Assert.Equal((uint)0, i);
        }

        [Fact]
        public static void MaxValue()
        {
            Assert.Equal(4294967295, uint.MaxValue);
        }

        [Fact]
        public static void MinValue()
        {
            Assert.Equal((uint)0, uint.MinValue);
        }

        [Theory]
        [InlineData((uint)789, (uint)789, true)]
        [InlineData((uint)789, (uint)0, false)]
        [InlineData((uint)0, (uint)0, true)]
        [InlineData((uint)789, null, false)]
        [InlineData((uint)789, "789", false)]
        [InlineData((uint)789, 789, false)]
        public static void EqualsTest(uint i, object? obj, bool expected)
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
