using Xunit;

namespace System.Tests
{
    internal static class Int16Tests
    {
        public static void Ctor_Empty()
        {
            var i = new short();
            Assert.Equal(0, i);
        }

        public static void MaxValue()
        {
            Assert.Equal(0x7FFF, short.MaxValue);
        }

        public static void MinValue()
        {
            Assert.Equal(unchecked((short)0x8000), short.MinValue);
        }

        public static void EqualsTests()
        {
            EqualsTest((short)789, (short)789, true);
            EqualsTest((short)789, (short)-789, false);
            EqualsTest((short)789, (short)0, false);
            EqualsTest((short)0, (short)0, true);
            EqualsTest((short)-789, (short)-789, true);
            EqualsTest((short)-789, (short)789, false);
            EqualsTest((short)789, null, false);
            EqualsTest((short)789, "789", false);
            EqualsTest((short)789, 789, false);
        }

        private static void EqualsTest(short i, object? obj, bool expected)
        {
            if (obj is short j)
            {
                Assert.Equal(expected, i.Equals(j));
                Assert.Equal(expected, i.GetHashCode().Equals(j.GetHashCode()));
            }
            Assert.Equal(expected, i.Equals(obj));
        }
    }
}
