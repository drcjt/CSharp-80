using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        public static void EqualEnumerable<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            Assert.Equal(expected.Count(), actual.Count());

            var expectedEnumerator = expected.GetEnumerator();
            var actualEnumerator = actual.GetEnumerator();
            while (expectedEnumerator.MoveNext() && actualEnumerator.MoveNext())
            {
                Assert.Equal(expectedEnumerator.Current, actualEnumerator.Current);
            }
        }

        public static void Equal<T>(T expected, T actual)
        {
            if (!Object.Equals(expected, actual))
            {
                Assert.HandleFail("Assert.Equal", "");
            }
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
            // TODO this fails compile with CS0656
            //throw new XunitException(assertionName + ": " + message);
            throw new XunitException(String.Concat(String.Concat(assertionName, ": "), message));
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
