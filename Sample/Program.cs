using System;

namespace MiniBCL
{
    public class Program
    {
		public static int Main(string[] args)
		{
			// Very simple program!
			HelloWorld();

			return 42;
		}

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
    }
}
