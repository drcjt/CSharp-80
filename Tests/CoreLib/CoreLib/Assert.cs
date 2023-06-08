using System;

namespace CoreLib
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

        public static void AreNotEquals(int expected, int actual)
        {
            if (expected == actual)
            {
                Environment.Exit(1);
            }
        }
    }
}
