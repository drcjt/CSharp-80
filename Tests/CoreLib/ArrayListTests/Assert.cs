namespace System.Collections.Tests
{
    internal static class Assert
    {
        public static void AreEquals(bool expected, bool actual)
        {
            if (expected != actual)
            {
                Environment.Exit(1);
            }
        }

        public static void AreEquals(int expected, int actual)
        {
            if (expected != actual)
            {
                Environment.Exit(1);
            }
        }

        public static void AreEquals(RuntimeTypeHandle expected, RuntimeTypeHandle actual)
        {
            if (!expected.Equals(actual))
            {
                Environment.Exit(1);
            }
        }

        public static void AreNotEquals(RuntimeTypeHandle expected, RuntimeTypeHandle actual)
        {
            if (expected.Equals(actual))
            {
                Environment.Exit(1);
            }
        }

        public static void Equal(object? expected, object? actual)
        {
            if (expected != actual)
            {
                Environment.Exit(1);
            }
        }

        public static void AreEquals(object expected, object actual)
        {
            if (!expected.Equals((object)actual))
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
