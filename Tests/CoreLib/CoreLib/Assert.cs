using System;

namespace CoreLib
{
    internal static class Assert
    {
        public static void AreEqual(bool expected, bool actual)
        {
            if (expected != actual)
            {
                Environment.Exit(1);
            }
        }

        public static void AreEqual(int expected, int actual)
        {
            if (expected != actual)
            {
                Environment.Exit(1);
            }
        }

        public static void AreEqual(RuntimeTypeHandle expected, RuntimeTypeHandle actual)
        {
            if (!expected.Equals(actual))
            {
                Environment.Exit(1);
            }
        }

        public static void AreEqual(object? expected, object? actual)
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
