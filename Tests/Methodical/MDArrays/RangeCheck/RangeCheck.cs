using System;

namespace RangeCheck
{
    public static class Test
    {
        public static int Main()
        {
            int result = ReadBeyondEndOfFirstDimension_Test(); if (result != 0) return result;
            result = ReadBeyondEndOfSecondDimension_Test(); if (result != 0) return result;
            result = WriteBeyondEndOfFirstDimension_Test(); if (result != 0) return result;
            result = WriteBeyondEndOfSecondDimension_Test(); if (result != 0) return result;

            return result;
        }

        private static int ReadBeyondEndOfFirstDimension_Test()
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

        private static int ReadBeyondEndOfSecondDimension_Test()
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

        private static int WriteBeyondEndOfFirstDimension_Test()
        {
            int[,] array = new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
            try
            {
                array[2, 0] = 10; // This should throw an IndexOutOfRangeException
                return 1; // If we reach here, the test failed
            }
            catch (IndexOutOfRangeException)
            {
                return 0; // Test passed
            }
        }

        private static int WriteBeyondEndOfSecondDimension_Test()
        {
            int[,] array = new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
            try
            {
                array[0, 3] = 10; // This should throw an IndexOutOfRangeException
                return 1; // If we reach here, the test failed
            }
            catch (IndexOutOfRangeException)
            {
                return 0; // Test passed
            }
        }
    }
}