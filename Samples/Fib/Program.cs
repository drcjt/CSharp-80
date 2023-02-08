using System;

namespace MiniBCL
{
    public static class Program
    {
        public static void Main()
        {
            Console.Clear();
            for (short i = 1; i < 50; i++)
            {
                Console.WriteLine(Fibonacci(i));  // should display 55
            }

            for (short i = 1; i < 50; i++)
            {
                Console.WriteLine(FibInt32(i));  // should display 55
            }
        }

        private static int FibInt32(int n)
        {
            int a = 0;
            int b = 1;
            for (int i = 0; i < n; i++)
            {
                int temp = a;
                a = b;
                b = temp + b;
            }
            return a;
        }

        private static short Fibonacci(short n)
        {
            short result = 0;
            short b = 1;
            // In N steps, compute Fibonacci sequence iteratively.
            for (short i = 0; i < n; i++)
            {
                short temp = result;
                result = b;
                b = (short)(temp + b);
            }
            return (short)result;
        }
    }
}
