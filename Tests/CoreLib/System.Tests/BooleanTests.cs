using Xunit;

namespace System.Tests
{
    internal static class BooleanTests
    {
        [Fact]
        public static void TrueString_Get_ReturnsTrue()
        {
            Assert.Equal("True", bool.TrueString);
        }

        [Fact]
        public static void FalseString_Get_ReturnsFalse()
        {
            Assert.Equal("False", bool.FalseString);
        }

        [Theory]
        [InlineData(true, "True")]
        [InlineData(false, "False")]
        public static void ToString_Invoke_ReturnsExpected(bool value, string expected)
        {
            Assert.Equal(expected, value.ToString());
        }

        [Theory]
        [InlineData(true, true, 0)]
        [InlineData(true, false, 1)]
        [InlineData(true, null, 1)]
        [InlineData(false, false, 0)]
        [InlineData(false, true, -1)]
        [InlineData(false, null, 1)]
        public static void CompareTo_Other_ReturnsExpected(bool b, object? obj, int expected)
        {
            if (obj is bool boolValue)
            {
                Assert.Equal(expected, Math.Sign(b.CompareTo(boolValue)));
            }

            Assert.Equal(expected, Math.Sign(b.CompareTo(obj)));
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(true, "true")]
        [InlineData(false, 0)]
        [InlineData(false, "false")]
        public static void CompareTo_ObjectNotBool_ThrowsArgumentException(bool b, object obj)
        {
            try
            {
                _ = b.CompareTo(obj);
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail("");
        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(true, false, false)]
        [InlineData(true, "1", false)]
        [InlineData(true, "True", false)]
        [InlineData(true, null, false)]
        [InlineData(false, false, true)]
        [InlineData(false, true, false)]
        [InlineData(false, "0", false)]
        [InlineData(false, "False", false)]
        [InlineData(false, null, false)]
        public static void Equals_Other_ReturnsExpected(bool b1, object? obj, bool expected)
        {
            if (obj is bool boolValue)
            {
                Assert.Equal(expected, b1.Equals(boolValue));
                Assert.Equal(expected, b1.GetHashCode().Equals(obj.GetHashCode()));
            }

            Assert.Equal(expected, b1.Equals(obj));
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public static void GetHashCode_Invoke_ReturnsExpected(bool value, int expected)
        {
            Assert.Equal(expected, value.GetHashCode());
        }
    }
}
