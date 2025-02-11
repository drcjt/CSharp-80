namespace System.Tests
{
    internal static class Assert
    {
        public static void AreSame<T>(T[] expected, T[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
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
