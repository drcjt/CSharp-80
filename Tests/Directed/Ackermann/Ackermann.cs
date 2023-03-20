using System;

namespace Ackermann
{
    public static class Test
    {
        public static int Main()
        {
            int NUM = 3;
            var ackermannValue = Ackermann(3, NUM);

            if (ackermannValue == 61)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public static int Ackermann(int m, int n)
        {
            if (m == 0)
            {
                return n + 1;
            }
            if (n == 0)
            {
                return Ackermann(m - 1, 1);
            }
            return Ackermann(m - 1, Ackermann(m, n - 1));
        }
    }
}