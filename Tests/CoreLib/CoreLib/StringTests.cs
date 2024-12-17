﻿using System;

namespace CoreLib
{
    public static class StringTests
    {
        public static unsafe void NewStringTests()
        {
            var chars = new char[25];
            string newString = new String(chars);
            Assert.AreEqual(25, newString.Length);
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
            Assert.AreEqual(3, middle.Length);

            for (int i = 1; i < 4; i++)
            {
                Assert.AreEqual(true, source[i] == middle[i - 1]);
            }
        }

        private static void InvalidSubstringTests()
        {
            var source = "abcde";
            var invalid1 = source.Substring(10, 2);
            Assert.AreEqual(true, invalid1 == null);

            var invalid2 = source.Substring(1, 10);
            Assert.AreEqual(true, invalid2 == null);
        }

        public static void EqualsTests()
        {
            var str1 = "abc";
            var str2 = "def";
            var str3 = new string(new char[] { 'a', 'b', 'c' });
            var str4 = "abc";

            // Reference equality as both strings are actually referring to same frozen string object
            Assert.IsTrue(str1.Equals(str4));

            // Strings that are not the same
            Assert.IsFalse(str1.Equals(str2));

            // Two separate strings that have same characters/length
            Assert.IsTrue(str1.Equals(str3));
        }
    }
}
