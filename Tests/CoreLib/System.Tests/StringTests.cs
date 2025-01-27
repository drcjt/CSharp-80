namespace System.Tests
{
    internal static class StringTests
    {
        public static void Ctor_CharArray_EmptyString()
        {
            var s = new string(new char[0]);
            Assert.AreEqual(0, s.Length);
        }

        public static void Ctor_CharArray_Tests()
        {
            Ctor_CharArray(['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'], "abcdefgh");
        }

        private static void Ctor_CharArray(char[] data, string expected)
        {
            var s = new string(data);
            Assert.AreEqual(expected, s);
        }

        public static void Contains_Char_Tests()
        {
            Contains_Char("Hello", 'H', true);
            Contains_Char("Hello", 'Z', false);
            Contains_Char("Hello", 'e', true);
            Contains_Char("Hello", 'E', false);
            Contains_Char("", 'H', false);
        }

        private static void Contains_Char(string s, char value, bool expected)
        {
            Assert.AreEqual(expected, s.Contains(value));
        }

        public static void EqualsTests()
        {
            Assert.IsTrue("Hello".Equals("Hello"));
            Assert.IsFalse("Hello".Equals("World"));
            Assert.IsTrue("Hello".Equals((object)"Hello"));
            Assert.IsFalse("Hello".Equals((object)"World"));
        }
    }
}
