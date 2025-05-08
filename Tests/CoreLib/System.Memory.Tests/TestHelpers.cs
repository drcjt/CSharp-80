namespace System.Memory.Tests
{
    public static class TestHelpers
    {
        public static void Validate<T>(this Span<T> span, params T[] expected) where T : struct, IEquatable<T>
        {
            Assert.IsTrue(span.SequenceEqual(expected));
        }

        public static void Validate<T>(this ReadOnlySpan<T> span, params T[] expected) where T : struct, IEquatable<T>
        {
            Assert.IsTrue(span.SequenceEqual(expected));
        }

        public static void ValidateReferenceType<T>(this Span<T> span, params T[] expected) where T : class
        {
            Assert.AreEqual(span.Length, expected.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                T actual = span[i];
                Assert.AreSame(expected[i], actual);
            }

            bool exceptionThrown = false;
            try
            {
                _ = span[expected.Length];
            }
            catch (IndexOutOfRangeException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }

        public static void ValidateReferenceType<T>(this ReadOnlySpan<T> span, params T[] expected) where T : class
        {
            Assert.AreEqual(span.Length, expected.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                T actual = span[i];
                Assert.AreSame(expected[i], actual);
            }

            bool exceptionThrown = false;
            try
            {
                _ = span[expected.Length];
            }
            catch (IndexOutOfRangeException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }
    }
}
