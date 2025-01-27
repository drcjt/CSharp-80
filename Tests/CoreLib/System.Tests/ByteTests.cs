namespace System.Tests
{
    internal static class ByteTests
    {
        public static void Ctor_Empty()
        {
            var i = new byte();
            Assert.AreEqual(0, i);
        }

        public static void MaxValue()
        {
            Assert.AreEqual(0xFF, byte.MaxValue);
        }

        public static void MinValue()
        {
            Assert.AreEqual(0, byte.MinValue);
        }

        public static void EqualsTests()
        {
            EqualsTest((byte)78, (byte)78, true);
            EqualsTest((byte)78, (byte)0, false);
            EqualsTest((byte)0, (byte)0, true);
            EqualsTest((byte)78, null, false);
            EqualsTest((byte)78, "78", false);
            EqualsTest((byte)78, 78, false);
        }

        private static void EqualsTest(byte i, object? obj, bool expected)
        {
            if (obj is byte j)
            {
                Assert.AreEqual(expected, i.Equals(j));
                Assert.AreEqual(expected, i.GetHashCode().Equals(j.GetHashCode()));
                Assert.AreEqual(i, i.GetHashCode());
            }
            Assert.AreEqual(expected, i.Equals(obj));
        }
    }
}
