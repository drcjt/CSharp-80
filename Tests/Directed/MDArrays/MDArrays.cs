using System;

public static class Simple_MDArray_Test
{
    public static int Main()
    {
        int SIZE = 10;

        Int32[,,] foo = new Int32[SIZE, SIZE, SIZE];
        int i, j, k, m;
        int sum = 0;

        for (i = 0; i < SIZE; i++)
        {
            for (j = 0; j < SIZE; j++)
            {
                for (k = 0; k < SIZE; k++)
                {
                    foo[i, j, k] = i * j * k;
                }
            }
        }

        for (i = 0; i < SIZE; i++)
        {
            for (j = 0; j < i; j++)
            {
                for (k = 0; k < j; k++)
                {
                    for (m = 0; m < k; m++)
                    {
                        sum += foo[i, j, k];
                    }
                }
            }
        }
        if (sum == 35958)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }
}