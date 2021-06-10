using System;

namespace MiniBCL
{
    public static class Program
    {
		public static int Main(string[] args)
		{
            /*
            bool b = true;
            if (b)
            {
                HelloWorld();
            }
            */

            /*
            HelloWorld();
            // Very simple program!
            short a = 50;
            short b = 3;
            Console.Write(a + b);
            */

            short i = 0;
            while (i < 5)
            {
                HelloWorld();
                i++;
            }

			return 42;
		}

		private static void HelloWorld()
		{
            Console.Write('H');
            Console.Write('e');
            Console.Write('l');
            Console.Write('l');
            Console.Write('o');

            //Display(32); // Show space

            Console.Write(' ');
            Console.Write('W');
            Console.Write('o');
            Console.Write('r');
            Console.Write('l');
            Console.Write('d');
            Console.Write('\n');
		}

        private static void Display(short i)
        {
            Console.Write(i);
        }
    }
}
