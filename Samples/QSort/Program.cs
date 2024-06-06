using System;
using System.Drawing;

[module: System.Runtime.CompilerServices.SkipLocalsInit]

namespace MiniBCL
{
    public static class Program
    {
        public static int Main()
        {
            var array = CreateRandomArray(Graphics.ScreenWidth);

            DrawData(array);

            var sortedArray = SortArray(array, 0, array.Length - 1);

            var lastItem = -1;
            foreach (var item in sortedArray)
            {
                if (item <= lastItem)
                    return 1;

                lastItem = item;
            }

            return 0;
        }

        public static void DrawData(int[] array)
        {
            Console.Clear();
            for (int x = 0; x < array.Length; x++)
            {
                Graphics.DrawLine(Pens.White, x, 0, x, array[x]);
            }
        }

        public static int[] CreateRandomArray(int size)
        {
            var array = new int[size];
            var rand = new Random();
            for (int i = 0; i < size; i++)
                array[i] = rand.Next(Graphics.ScreenHeight);
            return array;
        }

        public static int[] SortArray(int[] array, int leftIndex, int rightIndex)
        {
            var i = leftIndex;
            var j = rightIndex;
            var pivot = array[leftIndex];
            while (i <= j)
            {
                while (array[i] < pivot)
                {
                    i++;
                }

                while (array[j] > pivot)
                {
                    j--;
                }
                if (i <= j)
                {
                    Graphics.DrawLine(Pens.Black, i, 0, i, array[i]);
                    Graphics.DrawLine(Pens.Black, j, 0, j, array[j]);
                    int temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                    Graphics.DrawLine(Pens.White, i, 0, i, array[i]);
                    Graphics.DrawLine(Pens.White, j, 0, j, array[j]);
                    i++;
                    j--;
                }
            }

            if (leftIndex < j)
                SortArray(array, leftIndex, j);
            if (i < rightIndex)
                SortArray(array, i, rightIndex);
            return array;
        }
    }
}
