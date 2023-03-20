namespace Fibonacci
{
    public static class Tests
    {
        public static int Fibonacci(int n)
        {
            if (n < 2)
            {
                return 1;
            }
            return Fibonacci(n - 2) + Fibonacci(n - 1);
        }

        public static int Main()
        {
            if (Fibonacci(16) != 1597)
            {
                return 1;
            }

            return 0;
        }
    }
}