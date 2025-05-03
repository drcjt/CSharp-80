namespace EarlyValuePropagation
{
    public static class SimpleArrayLength
    {
        public static int Main()
        { 
            int[] array = new int[10];
            if (array.Length != 10)
            {
                return 1;
            }

            return 0;
        }
    }
}