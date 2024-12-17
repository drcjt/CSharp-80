using System;

namespace Regression
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

        public static void AreEqual(nuint expected, nuint actual)
        {
            if (expected != actual)
            {
                Environment.Exit(1);
            }
        }
    }
}
