using System;

namespace CoreLib
{
    internal static class Assert
    {
        public static void Equals(bool expected, bool actual)
        {
            if (expected != actual)
            {
                Environment.Exit(1);
            }
        }

        public static void Equals(int expected, int actual)
        {
            if (expected != actual)
            {
                Environment.Exit(1);
            }
        }
    }
}
