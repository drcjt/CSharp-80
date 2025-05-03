namespace EarlyValuePropagation
{
    public static class SimpleArrayLength
    {
        public static int Main()
        {
            var result = Test();

            if (result != 78)
                return result;

            return 0;
        }

        public static int Test()
        {
            int[] a = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
            int s = 0;
            for (int i = 0; i < a.Length; i++)
            {
                s += a[i];
            }

            return s;
        }
    }
}