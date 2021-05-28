using System;

namespace MiniBCL
{
    public static class Program
    {
		public static int Main(string[] args)
		{
            HelloWorld();
            /*
            // Very simple program!
            int i = 0;
            while (i < 5)
            {
                HelloWorld();
                i++;
            }
            */
			return 42;
		}

		private static int HelloWorld()
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

            return 0;
		}

        private static void Display(short i)
        {
            Console.Write(i);
        }
    }
}
