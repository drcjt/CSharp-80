using System.Collections.Generic;
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

        [Theory]
        [InlineData(234, 234, 0)]
        [InlineData(234, int.MinValue, 1)]
        [InlineData(-234, int.MinValue, 1)]
        [InlineData(int.MinValue, int.MinValue, 0)]
        [InlineData(234, -123, 1)]
        [InlineData(234, 0, 1)]
        [InlineData(234, 123, 1)]
        [InlineData(234, 456, -1)]
        [InlineData(234, int.MaxValue, -1)]
        [InlineData(-234, int.MaxValue, -1)]
        [InlineData(int.MaxValue, int.MaxValue, 0)]
        [InlineData(-234, -234, 0)]
        [InlineData(-234, 234, -1)]
        [InlineData(-234, -432, 1)]
        [InlineData(234, null, 1)]
        public static void CompareTo_Other_ReturnsExpected(int i, object? value, int expected)
        {
            if (value is int intValue)
            {
                Assert.Equal(expected, Math.Sign(i.CompareTo(intValue)));
                Assert.Equal(-expected, Math.Sign(intValue.CompareTo(i)));
            }

            Assert.Equal(expected, Math.Sign(i.CompareTo(value)));
        }

        [Theory]
        [InlineData("a")]
        public static void CompareTo_ObjectNotInt_ThrowsArgumentException(object value)
        {
            try
            {
                _ = 123.CompareTo(value);
            }
            catch (ArgumentException)
            {
                return;
            }
            Assert.Fail("");
        }
    }
}
