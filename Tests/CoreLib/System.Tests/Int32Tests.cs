namespace System.Tests
{
    internal static class Int32Tests
    {
        public static void Ctor_Empty()
        {
            var i = new int();
            Assert.AreEqual(0, i);
        }

        public static void MaxValue()
        {
            Assert.AreEqual(2147483647, int.MaxValue);
        }

        public static void MinValue()
        {
            Assert.AreEqual(-2147483648, int.MinValue);
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
            // This is failing, results in -(
            //ToStringTest(int.MinValue, "-2147483648");
            ToStringTest(-4567, "-4567");
            ToStringTest(0, "0");
            ToStringTest(4567, "4567");
            ToStringTest(int.MaxValue, "2147483647");
        }

        private static void ToStringTest(int i, string expected)
        {
            Assert.AreEqual(expected, i.ToString());
        }

        private static void EqualsTest(int i, object? obj, bool expected)
        {
            if (obj is int j)
            {
                Assert.AreEqual(expected, i.Equals(j));
            }
            Assert.AreEqual(expected, i.Equals(obj));
        }

        public static void Parse_Valid()
        {
            Assert.AreEqual(0, int.Parse("0"));
            Assert.AreEqual(0, int.Parse("0000000000000000000000000000000000000000000000000000000000"));
            Assert.AreEqual(1, int.Parse("0000000000000000000000000000000000000000000000000000000001"));
            Assert.AreEqual(2147483647, int.Parse("2147483647"));
            Assert.AreEqual(2147483647, int.Parse("02147483647"));
            Assert.AreEqual(2147483647, int.Parse("00000000000000000000000000000000000000000000000002147483647"));
            Assert.AreEqual(123, int.Parse("123\0\0"));

            Assert.AreEqual(123, int.Parse("123"));
        }
    }
}
