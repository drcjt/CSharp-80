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
        }

        private static short Fibonacci(short n)
        {
            // TODO: this fails to work correctly in release configuration
            // result is 512 instead of 55 with n = 10??
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
