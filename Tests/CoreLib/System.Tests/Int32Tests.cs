using Xunit;

namespace System.Tests
{
    internal static class Int32Tests
    {
        [Fact]
        public static void Ctor_Empty()
        {
            var i = new int();
            Assert.Equal(0, i);
        }

        [Fact]
        public static void MaxValue()
        {
            Assert.Equal(2147483647, int.MaxValue);
        }

        [Fact]
        public static void MinValue()
        {
            Assert.Equal(-2147483648, int.MinValue);
        }

        [Theory]
        [InlineData(int.MinValue, "-2147483648")]
        [InlineData(-4567, "-4567")]
        [InlineData(0, "0")]
        [InlineData(4567, "4567")]
        [InlineData(int.MaxValue, "2147483647")]
        public static void ToStringTest(int i, string expected)
        {
            Assert.Equal(expected, i.ToString());
        }

        [Theory]
        [InlineData(789, 789, true)]
        [InlineData(789, -789, false)]
        [InlineData(789, 0, false)]
        [InlineData(0, 0, true)]
        [InlineData(-789, -789, true)]
        [InlineData(-789, 789, false)]
        [InlineData(789, null, false)]
        [InlineData(789, "789", false)]
        public static void EqualsTest(int i, object? obj, bool expected)
        {
            if (obj is int j)
            {
                Assert.Equal(expected, i.Equals(j));
                Assert.Equal(expected, i.GetHashCode().Equals(j.GetHashCode()));
            }
            Assert.Equal(expected, i.Equals(obj));
        }

        [Fact]
        public static void Parse_Valid()
        {
            Assert.Equal(0, int.Parse("0"));
            Assert.Equal(0, int.Parse("0000000000000000000000000000000000000000000000000000000000"));
            Assert.Equal(1, int.Parse("0000000000000000000000000000000000000000000000000000000001"));
            Assert.Equal(2147483647, int.Parse("2147483647"));
            Assert.Equal(2147483647, int.Parse("02147483647"));
            Assert.Equal(2147483647, int.Parse("00000000000000000000000000000000000000000000000002147483647"));
            Assert.Equal(123, int.Parse("123\0\0"));

            Assert.Equal(123, int.Parse("123"));
        }
    }
}
