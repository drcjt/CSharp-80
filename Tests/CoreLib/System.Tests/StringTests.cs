using System.Runtime.InteropServices;

namespace System.Tests
{
    internal static class StringTests
    {
        public static void Ctor_CharSpan_EmptyString_Tests()
        {
            Ctor_CharSpan_EmptyString(0, 0);
            Ctor_CharSpan_EmptyString(3, 1);
        }

        public static void Ctor_CharSpan_EmptyString(int length, int offset)
        {
            var span = new ReadOnlySpan<char>(new char[length], offset, 0);
            Assert.AreSame(string.Empty, new string(span));
        }

        public static void Ctor_CharSpan_Empty()
        {
            Assert.AreSame(string.Empty, new string((ReadOnlySpan<char>)null));
            Assert.AreSame(string.Empty, new string(ReadOnlySpan<char>.Empty));
        }

        public static void Ctor_CharSpan_Tests()
        {
            Ctor_CharSpan(['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'], 0, 8, "abcdefgh");
            Ctor_CharSpan(['a', 'b', 'c'], 0, 1, "a");
            Ctor_CharSpan(['a', 'b', 'c'], 2, 1, "c");
        }

        private static void Ctor_CharSpan(char[] data, int startIndex, int length, string expected)
        {
            var span = new ReadOnlySpan<char>(data, startIndex, length);
            Assert.AreEqual(expected, new string(span));
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

            ReadOnlySpan<char> span = s;
            Assert.AreEqual(expected, span.Contains(value));
        }

        public static void EqualsTests()
        {
            Assert.IsTrue("Hello".Equals("Hello"));
            Assert.IsFalse("Hello".Equals("World"));
            Assert.IsTrue("Hello".Equals((object)"Hello"));
            Assert.IsFalse("Hello".Equals((object)"World"));
        }

        public static void Contains_Match_Char()
        {
            Assert.IsFalse("".Contains('a'));
            Assert.IsFalse(((ReadOnlySpan<char>)"").Contains('a'));

            Contains_Match_Char_For_Length(1);
            Contains_Match_Char_For_Length(25);
            Contains_Match_Char_For_Length(50);
        }

        private static void Contains_Match_Char_For_Length(int length)
        {
            char[] ca = new char[length];
            for (int i = 0; i < length; i++)
            {
                ca[i] = (char)(i + 1);
            }

            var span = new Span<char>(ca);
            var ros = new ReadOnlySpan<char>(ca);
            var str = new string(ca);

            for (var targetIndex = 0; targetIndex < length; targetIndex++)
            {
                char target = ca[targetIndex];

                bool found = span.Contains(target);
                Assert.IsTrue(found);

                found = ros.Contains(target);
                Assert.IsTrue(found);

                found = str.Contains(target);
                Assert.IsTrue(found);
            }
        }

        public static void Contains_ZeroLength_Char()
        {
            var span = new Span<char>(Array.Empty<char>());
            bool found = span.Contains((char)0);
            Assert.IsFalse(found);

            span = Span<char>.Empty;
            found = span.Contains((char)0);
            Assert.IsFalse(found);

            var ros = new ReadOnlySpan<char>(Array.Empty<char>());
            found = ros.Contains((char)0);
            Assert.IsFalse(found);

            ros = ReadOnlySpan<char>.Empty;
            found = ros.Contains((char)0);
            Assert.IsFalse(found);

            found = string.Empty.Contains((char)0);
            Assert.IsFalse(found);
        }

        public static void Contains_MultipleMatches_Char()
        {
            for (int length = 2; length < 32; length++)
            {
                var ca = new char[length];
                for (int i = 0; i < length; i++)
                {
                    ca[i] = (char)(i + 1);
                }

                ca[length - 1] = (char)200;
                ca[length - 2] = (char)200;

                // Span
                var span = new Span<char>(ca);
                bool found = span.Contains((char)200);
                Assert.IsTrue(found);

                // ReadOnlySpan
                var ros = new ReadOnlySpan<char>(ca);
                found = ros.Contains((char)200);
                Assert.IsTrue(found);

                // String
                var str = new string(ca);
                found = str.Contains((char)200);
                Assert.IsTrue(found);
            }
        }

        public static void ImplicitCast_ResultingSpanMatches_Tests()
        {
            ImplicitCast_ResultingSpanMatches("");
            ImplicitCast_ResultingSpanMatches("a");
            ImplicitCast_ResultingSpanMatches("\0");
            ImplicitCast_ResultingSpanMatches("abc");
        }

        public static unsafe void ImplicitCast_ResultingSpanMatches(string s)
        {
            ReadOnlySpan<char> span = s;
            Assert.AreEqual(s.Length, span.Length);
            fixed (char* stringPtr = s)
            fixed (char* spanPtr = &MemoryMarshal.GetReference(span))
            {
                Assert.AreEqual((IntPtr)stringPtr, (IntPtr)spanPtr);
            }
        }

        public static void ImplicitCast_NullString_ReturnsDefaultSpan()
        {
            ReadOnlySpan<char> span = (string)null;
            Assert.IsTrue(span == default);
        }
    }
}
