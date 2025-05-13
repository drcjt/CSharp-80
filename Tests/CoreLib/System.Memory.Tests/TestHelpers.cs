using Xunit;

namespace System.Memory.Tests
{
    public static class TestHelpers
    {
        public static void Validate<T>(this Span<T> span, params T[] expected) where T : struct, IEquatable<T>
        {
            Assert.True(span.SequenceEqual(expected));
        }

        public static void Validate<T>(this ReadOnlySpan<T> span, params T[] expected) where T : struct, IEquatable<T>
        {
            Assert.True(span.SequenceEqual(expected));
        }

        public static void ValidateReferenceType<T>(this Span<T> span, params T[] expected) where T : class
        {
            Assert.Equal(span.Length, expected.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                T actual = span[i];
                Assert.Same(expected[i], actual);
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

            Assert.True(exceptionThrown);
        }

        public static void ValidateReferenceType<T>(this ReadOnlySpan<T> span, params T[] expected) where T : class
        {
            Assert.Equal(span.Length, expected.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                T actual = span[i];
                Assert.Same(expected[i], actual);
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

            Assert.True(exceptionThrown);
        }
    }
}
