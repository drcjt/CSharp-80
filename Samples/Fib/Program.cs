using System;

namespace MiniBCL
{
    public static class Program
    {
        public static void Main()
		{
            Console.Clear();          
            Console.WriteLine(Fibonacci(10));  // should display 55
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
