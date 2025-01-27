namespace System.Tests
{
    internal static class UInt16Tests
    {
        public static void Ctor_Empty()
        {
            var i = new ushort();
            Assert.AreEqual(0, i);
        }

        public static void MaxValue()
        {
            Assert.AreEqual(0xFFFF, ushort.MaxValue);
        }

        public static void MinValue()
        {
            Assert.AreEqual(0, ushort.MinValue);
        }

        public static void EqualsTests()
        {
            EqualsTest((ushort)789, (ushort)789, true);
            EqualsTest((ushort)789, (ushort)0, false);
            EqualsTest((ushort)0, (ushort)0, true);
            EqualsTest((ushort)789, null, false);
            EqualsTest((ushort)789, "789", false);
            EqualsTest((ushort)789, 789, false);
        }

        private static void EqualsTest(ushort i, object? obj, bool expected)
        {
            if (obj is ushort j)
            {
                Assert.AreEqual(expected, i.Equals(j));
                Assert.AreEqual(expected, i.GetHashCode().Equals(j.GetHashCode()));
                Assert.AreEqual(i, i.GetHashCode());
            }
            Assert.AreEqual(expected, i.Equals(obj));
        }
    }
}
