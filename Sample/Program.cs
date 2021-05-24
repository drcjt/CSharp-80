using System;

namespace MiniBCL
{
    public static class Program
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

            //DisplayA(32, 10); // Show space

            Console.Write(' ');
            Console.Write('W');
            Console.Write('o');
            Console.Write('r');
            Console.Write('l');
            Console.Write('d');
		}

        /*
        private static void DisplayA(short i, short j)
        {
            Console.Write(i);
        }
        */
    }
}
