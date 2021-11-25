namespace System
{
    public static class Math
    {
        public static int Abs(int value)
        {
            if (value >= 0)
                return value;
            else
                return -value;
        }

        public static int Sign(int value)
        {
            if (value < 0)
                return -1;
            else if (value > 0)
                return 1;
            else
                return 0;
        }

        public static int Max(int val1, int val2)
        {
            return (val1 >= val2) ? val1 : val2;
        }

        public static int Min(int val1, int val2)
        {
            return (val1 <= val2) ? val1 : val2;
        }
    }
}
