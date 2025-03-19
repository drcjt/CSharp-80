namespace MDArrays
{
    public static class Test
    {
        public static int Main()
        {
            int result = Simple_MDArray_Test();
            return result;
        }

        private static int Simple_MDArray_Test()
        {
            int[,] array = new int[3, 5];

            int value = 1;
            for (int x = 0; x < array.GetLength(0); x++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    array[x, y] = value++;
                }
            }

            value = 1;
            for (int x = 0; x < array.GetLength(0); x++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    if (array[x, y] != value++)
                    {
                        return 1;
                    }
                }
            }

            return 0;
        }
    }
}