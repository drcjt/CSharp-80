using System;

namespace CoreLib
{
    public class StringTests
    {
        public static unsafe void NewStringTests()
        {
            var chars = new char[25];
            string newString = new String(chars);
            Assert.AreEquals(25, newString.Length);
        }

        public static void SubstringTests()
        {
            ValidSubstringTests();
            InvalidSubstringTests();
        }

        private static void ValidSubstringTests()
        {
            var source = "abcde";
            var middle = source.Substring(1, 3);
            Assert.AreEquals(3, middle.Length);

            for (int i = 1; i < 4; i++)
            {
                Assert.AreEquals(true, source[i] == middle[i - 1]);
            }
        }

        private static void InvalidSubstringTests()
        {
            var source = "abcde";
            var invalid1 = source.Substring(10, 2);
            Assert.AreEquals(true, invalid1 == null);

            var invalid2 = source.Substring(1, 10);
            Assert.AreEquals(true, invalid2 == null);
        }
    }
}
