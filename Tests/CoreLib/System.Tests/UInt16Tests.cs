using Xunit;

namespace System.Tests
{
    internal static class UInt16Tests
    {
        public static void Ctor_Empty()
        {
            var i = new ushort();
            Assert.Equal(0, i);
        }

        public static void MaxValue()
        {
            Assert.Equal(0xFFFF, ushort.MaxValue);
        }

        public static void MinValue()
        {
            Assert.Equal(0, ushort.MinValue);
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
                Assert.Equal(expected, i.Equals(j));
                Assert.Equal(expected, i.GetHashCode().Equals(j.GetHashCode()));
                Assert.Equal(i, i.GetHashCode());
            }
            Assert.Equal(expected, i.Equals(obj));
        }
    }
}
