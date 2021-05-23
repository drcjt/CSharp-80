using System;

namespace MiniBCL
{
    public static class Program
    {
		public static int Main(string[] args)
		{
            //Console.WriteLine("Hello World");

			// Very simple program!
			//HelloWorld();

            Console.Write((short)(Fibonacci(0) + 48));

			return 42;
		}

        /*
		private static void HelloWorld()
		{
            Console.Write('H');
            Console.Write('e');
            Console.Write('l');
            Console.Write('l');
            Console.Write('o');

            Display(32, 10); // Show space

            Console.Write(' ');
            Console.Write('W');
            Console.Write('o');
            Console.Write('r');
            Console.Write('l');
            Console.Write('d');
		}

        private static void Display(short i, short j)
        {
            Console.Write(i);
        }
        */

        private static int Fibonacci(int n)
        {
            if (n == 0 || n == 1)
            {
                return 1;
            }
            else
            {
                return Fibonacci(n-1) + Fibonacci(n-2);
            }
        }
    }
}
