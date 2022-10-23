using System;

public static class AckermannTest
{
    public static int Main()
    {
        int NUM = 8;
        var ackermannValue = Ackermann(3, 3);

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
        return Ackermann(m -1, Ackermann(m, n - 1));
    }
}