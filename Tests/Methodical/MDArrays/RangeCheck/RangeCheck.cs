using System;

namespace RangeCheck
{
    public static class Test
    {
        public static int Main()
        {
            int result = IndexOutOfRangeException_Test_FirstDimension(); if (result != 0) return result;
            result = IndexOutOfRangeException_Test_SecondDimension(); if (result != 0) return result;

            return result;
        }

        private static int IndexOutOfRangeException_Test_FirstDimension()
        {
            int[,] array = new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
            try
            {
                int value = array[2, 0]; // This should throw an IndexOutOfRangeException
                return 1; // If we reach here, the test failed
            }
            catch (IndexOutOfRangeException)
            {
                return 0; // Test passed
            }
        }

        private static int IndexOutOfRangeException_Test_SecondDimension()
        {
            int[,] array = new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
            try
            {
                int value = array[0, 3]; // This should throw an IndexOutOfRangeException
                return 1; // If we reach here, the test failed
            }
            catch (IndexOutOfRangeException)
            {
                return 0; // Test passed
            }
        }
    }
}