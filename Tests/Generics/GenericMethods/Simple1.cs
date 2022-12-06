using System;

public static class Simple_Generic_Method_Test
{
    private static void Reverse<T>(T[] arr)
    {
        int i = 0;
        int j = arr.Length - 1;
        while (i < j)
        {
            T temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
            i++;
            j--;
        }
    }

    private static T[] GetInitialisedArray<T>(T value, int size)
    {
        var arr = new T[size];
        for (int i = 0; i < size; i++)
        {
            arr[i] = value;
        }

        return arr;
    }

    public static int Main()
    {
        var iarray = new int[5] { 1, 2, 3, 4, 5 };
        Reverse<int>(iarray);

        if (iarray[0] != 5) return 1;
        if (iarray[1] != 4) return 1;
        if (iarray[2] != 3) return 1;
        if (iarray[3] != 2) return 1;
        if (iarray[4] != 1) return 1;

        var jarray = GetInitialisedArray<byte>(12, 7);

        for (int j = 0; j < jarray.Length; j++)
        {
            if (jarray[j] != 12)
            {
                return 1;
            }
        }

        var barray = new byte[3] { 64, 65, 66 };
        Reverse<byte>(barray);

        if (barray[0] != 66) return 1;
        if (barray[1] != 65) return 1;
        if (barray[2] != 64) return 1;

        return 0;
    }
}