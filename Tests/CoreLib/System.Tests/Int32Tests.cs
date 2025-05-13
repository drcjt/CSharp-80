using Xunit;

namespace System.Tests
{
    internal static class Int32Tests
    {
        public static void Ctor_Empty()
        {
            var i = new int();
            Assert.Equal(0, i);
        }

        public static void MaxValue()
        {
            Assert.Equal(2147483647, int.MaxValue);
        }

        public static void MinValue()
        {
            Assert.Equal(-2147483648, int.MinValue);
        }

        public static void EqualsTests()
        {
            EqualsTest(789, 789, true);
            EqualsTest(789, -789, false);
            EqualsTest(789, 0, false);
            EqualsTest(0, 0, true);
            EqualsTest(-789, -789, true);
            EqualsTest(-789, 789, false);
            EqualsTest(789, null, false);
            EqualsTest(789, "789", false);
        }

        public static void ToStringTests()
        {
            ToStringTest(int.MinValue, "-2147483648");
            ToStringTest(-4567, "-4567");
            ToStringTest(0, "0");
            ToStringTest(4567, "4567");
            ToStringTest(int.MaxValue, "2147483647");
        }

        private static void ToStringTest(int i, string expected)
        {
            Assert.Equal(expected, i.ToString());
        }

        private static void EqualsTest(int i, object? obj, bool expected)
        {
            if (obj is int j)
            {
                Assert.Equal(expected, i.Equals(j));
                Assert.Equal(expected, i.GetHashCode().Equals(j.GetHashCode()));
            }
            Assert.Equal(expected, i.Equals(obj));
        }

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
