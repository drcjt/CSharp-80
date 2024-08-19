namespace Benchmarks
{
    public static class Fib
    {
        const int Number = 24;

        static int Fibonacci(int x)
        {
            if (x > 2)
            {
                return (Fibonacci(x - 1) + Fibonacci(x - 2));
            }
            else
            {
                return 1;
            }
        }

        static bool Bench()
        {
            int fib = Fibonacci(Number);
            return (fib == 46368);
        }

        public static int Main()
        {
            return Bench() ? 0 : 1;
        }
    }
}