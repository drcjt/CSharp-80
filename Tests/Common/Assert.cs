using System;
using System.Collections;
using System.Collections.Generic;

namespace Xunit
{
    public static class Assert
    {
        public static void Same(object expected, object actual)
        {
            if (!Object.ReferenceEquals(expected, actual))
            {
                Assert.HandleFail("Assert.Same", "");
            }
        }

        public static void NotSame(object notExpected, object actual)
        {
            if (Object.ReferenceEquals(notExpected, actual))
            {
                Assert.HandleFail("Assert.NotSame", "");
            }
        }

        public static void Null(object? value)
        {
            if (value is not null)
            {
                Assert.HandleFail("Assert.Null", "");
            }
        }

        public static void Equal<T>(T expected, T actual)
        {
            if (expected is IEnumerable expectedEnumerable && actual is IEnumerable actualEnumerable)
            {
                SequenceEqual(expectedEnumerable, actualEnumerable);
                return;
            }

            var comparer = EqualityComparerHelpers.GetComparerForReferenceTypesOnly<T>();

            bool result;
            if (comparer != null)
            {
                result = comparer.Equals(expected, actual);
            }
            else
            {
                result = EqualityComparerHelpers.StructOnlyEquals<T>(expected, actual);
            }

            if (!result)
            {
                Assert.HandleFail("Assert.Equal", "");
            }
        }

        private static void SequenceEqual(IEnumerable expected, IEnumerable actual)
        {
            var expectedEnumerator = expected.GetEnumerator();
            var actualEnumerator = actual.GetEnumerator();

            while (expectedEnumerator.MoveNext())
            {
                Assert.True(actualEnumerator.MoveNext());
                Assert.Equal(expectedEnumerator.Current, actualEnumerator.Current);
            }

            Assert.False(actualEnumerator.MoveNext());
        }

        public static void NotEqual<T>(T notExpected, T actual)
        {
            if (Object.Equals(notExpected, actual))
            {
                Assert.HandleFail("Assert.NotEqual", "");
            }
        }

        public static void False(bool condition, string message = "")
        {
            if (condition)
            {
                Assert.HandleFail("Assert.False", message);
            }
        }

        public static void True(bool condition, string message = "")
        {
            if (!condition)
            {
                Assert.HandleFail("Assert.True", message);
            }
        }

        public static void NotNull(object value)
        {
            if (value == null)
            {
                Assert.HandleFail("Assert.NotNull", "");
            }
        }

        internal static void HandleFail(string assertionName, string message)
        {
            throw new XunitException($"{assertionName}: {message}");
        }
    }

    public class XunitException : Exception
    {
        public XunitException(string message) : base(message)
        {
        }

        public XunitException() : base()
        {
        }
    }
}
