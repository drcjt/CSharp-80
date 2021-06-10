using System;

namespace MiniBCL
{
    public static class Program
    {
		public static int Main(string[] args)
		{
            // Test subroutine call
            HelloWorld();

            // Test boolean comparisons
            TestBooleanComparison();

            // Test Int16 addition
            TestAddition();

            // Test less than branching
            TestLessThanBranching();

			return 42;
		}

        private static void TestBooleanComparison()
        {
            bool b = true;
            if (b)
            {
                Console.Write('B');
                Console.Write('o');
                Console.Write('o');
                Console.Write('l');
                Console.Write(' ');
                Console.Write('T');
                Console.Write('e');
                Console.Write('s');
                Console.Write('t');
                Console.Write('\n');
            }
        }

        private static void TestAddition()
        {
            short a = 50;
            short b = 3;
            Console.Write(a + b);
            Console.Write('\n');
        }

        private static void TestLessThanBranching()
        {
            short i = 0;
            while (i < 5)
            {
                Console.Write(48 + i);
                i++;
            }
            Console.Write('\n');
        }

        private static void HelloWorld()
		{
            Console.Write('H');
            Console.Write('e');
            Console.Write('l');
            Console.Write('l');
            Console.Write('o');

            Console.Write(' ');
            Console.Write('W');
            Console.Write('o');
            Console.Write('r');
            Console.Write('l');
            Console.Write('d');
            Console.Write('\n');
		}
    }
}
