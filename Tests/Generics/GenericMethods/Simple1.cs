﻿using System;

namespace SimpleGenericMethod
{
    public static class Tests
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

        private static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        private static void SwapIndirect<T>(ref T a, ref T b)
        {
            Swap<T>(ref a, ref b);
        }

        private static void SwapInts<T>(ref T a, ref T b)
        {
            int x = 15;
            int y = 20;

            Swap<int>(ref x, ref y);

        }

        public static int Main()
        {
            int x = 12;
            int y = 17;

            char c = 'a';
            char d = 'b';

            SwapInts<char>(ref c, ref d);

            Swap<int>(ref x, ref y);
            if (x != 17) return 1;
            if (y != 12) return 1;

            SwapIndirect<int>(ref x, ref y);
            if (x != 12) return 1;
            if (y != 17) return 1;

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
}