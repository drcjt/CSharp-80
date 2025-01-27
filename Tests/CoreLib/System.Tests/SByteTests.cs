namespace System.Tests
{
    internal static class SByteTests
    {
        public static void Ctor_Empty()
        {
            var i = new sbyte();
            Assert.AreEqual(0, i);
        }

        public static void MaxValue()
        {
            Assert.AreEqual(0x7F, sbyte.MaxValue);
        }

        public static void MinValue()
        {
            Assert.AreEqual(-0x80, sbyte.MinValue);
        }

        public static void EqualsTests()
        {
            EqualsTest((sbyte)78, (sbyte)78, true);
            EqualsTest((sbyte)78, (sbyte)-78, false);
            EqualsTest((sbyte)78, (sbyte)0, false);
            EqualsTest((sbyte)0, (sbyte)0, true);
            EqualsTest((sbyte)-78, (sbyte)-78, true);
            EqualsTest((sbyte)-78, (sbyte)78, false);
            EqualsTest((sbyte)78, null, false);
            EqualsTest((sbyte)78, "78", false);
            EqualsTest((sbyte)78, 78, false);
        }

        private static void EqualsTest(sbyte i, object? obj, bool expected)
        {
            if (obj is sbyte j)
            {
                Assert.AreEqual(expected, i.Equals(j));
                Assert.AreEqual(expected, i.GetHashCode().Equals(j.GetHashCode()));
            }
            Assert.AreEqual(expected, i.Equals(obj));
        }
    }
}
