﻿namespace Regression
{
    internal static class SpillImportStElemTests
    {
        public static int Test()
        {
            object?[] arr = new object[1];
            arr[0] = 37;

            int size = 1;

            object value = arr[--size]!;
            arr[size] = null;

            return (int)value;
        }
    }
}
