using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Tests
{
    internal static class Assert
    {
        public static void AreEnumerablesEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            Assert.AreEqual(expected.Count(), actual.Count());

            var expectedEnumerator = expected.GetEnumerator();
            var actualEnumerator = actual.GetEnumerator();
            while (expectedEnumerator.MoveNext() && actualEnumerator.MoveNext())
            {
                Assert.AreEqual(expectedEnumerator.Current, actualEnumerator.Current);
            }
        }

        public static void AreEqual(RuntimeTypeHandle expected, RuntimeTypeHandle actual)
        {
            if (!expected.Equals(actual))
            {
                Environment.Exit(1);
            }
        }

        public static void AreEqual<T>(T expected, T actual)
        {
            if (expected is null)
            {
                if (actual is not null)
                {
                    Environment.Exit(1);
                }
            }
            else if (!expected.Equals(actual))
            {
                Environment.Exit(1);
            }
        }

        public static void AreNotEqual(RuntimeTypeHandle expected, RuntimeTypeHandle actual)
        {
            if (expected.Equals(actual))
            {
                Environment.Exit(1);
            }
        }

        public static void IsFalse(bool condition)
        {
            if (condition)
            {
                Environment.Exit(1);
            }
        }

        public static void IsTrue(bool condition)
        {
            if (!condition)
            {
                Environment.Exit(1);
            }
        }

        public static void IsNotNull(object obj)
        {
            if (obj == null)
            {
                Environment.Exit(1);
            }
        }
    }
}
