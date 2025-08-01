using Xunit;

namespace System.Tests
{
    internal static class SByteTests
    {
        [Fact]
        public static void Ctor_Empty()
        {
            var i = new sbyte();
            Assert.Equal(0, i);
        }

        [Fact]
        public static void MaxValue()
        {
            Assert.Equal(0x7F, sbyte.MaxValue);
        }

        [Fact]
        public static void MinValue()
        {
            Assert.Equal(-0x80, sbyte.MinValue);
        }

        [Theory]
        [InlineData((sbyte)78, (sbyte)78, true)]
        [InlineData((sbyte)78, (sbyte)-78, false)]
        [InlineData((sbyte)78, (sbyte)0, false)]
        [InlineData((sbyte)0, (sbyte)0, true)]
        [InlineData((sbyte)-78, (sbyte)-78, true)]
        [InlineData((sbyte)-78, (sbyte)78, false)]
        [InlineData((sbyte)78, null, false)]
        [InlineData((sbyte)78, "78", false)]
        [InlineData((sbyte)78, 78, false)]
        public static void EqualsTest(sbyte i, object? obj, bool expected)
        {
            if (obj is sbyte j)
            {
                Assert.Equal(expected, i.Equals(j));
                Assert.Equal(expected, i.GetHashCode().Equals(j.GetHashCode()));
            }
            Assert.Equal(expected, i.Equals(obj));
        }
    }
}
