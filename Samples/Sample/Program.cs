using System;

namespace MiniBCL
{
    public static class Program
    {
        public static int Main()
		{
            Console.Clear();

            // Test subroutine call
            HelloWorld();

            // Test writing int16 positive value
            Console.Write((short)589);
            Console.WriteLine();

            // Test writing int16 negative value
            Console.Write((short)-8537);
            Console.WriteLine();

            // Test boolean comparisons
            TestBooleanComparison();

            // Test Int16 addition
            TestAddition();

            // Test less than branching
            TestLessThanBranching();

            // Test greater than branching
            TestGreaterThanBranching();

            // Test less than or equal branching
            TestLessThanOrEqualBranching();

            // Test greater than or equal branching
            TestGreaterThanOrEqualBranching();

            // Test equal branching
            TestEqualBranching();

            // Test not equal branching
            TestNotEqualBranching();

            short a = 3;
            short b = 4;
            short c = 5;
            TestArguments(a, b, c);

            // Test implementation of write char completely written in C#
            WriteChar(0, 0, 48); // Write 0 to top left corner of screen

            Console.Write(TestFibonacci(10));  // should display 55

            return 42;
		}

        private static short TestFibonacci(short n)
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

        private static void TestArguments(short a, short b, short c)
        {
            Console.Write(48 + a);
            Console.Write(48 + b);
            Console.Write(48 + c);
            Console.Write('\n');
        }

        private static void TestBooleanComparison()
        {
            bool b = true;
            if (b)
            {
                Console.Write("Bool Test\n");
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

        private static void TestGreaterThanBranching()
        {
            short i = 5;
            while (i > 0)
            {
                Console.Write(48 + i);
                i--;
            }
            Console.Write('\n');
        }

        private static void TestLessThanOrEqualBranching()
        {
            short i = 0;
            while (i <= 5)
            {
                Console.Write(48 + i);
                i++;
            }
            Console.Write('\n');
        }

        private static void TestGreaterThanOrEqualBranching()
        {
            short i = 5;
            while (i >= 0)
            {
                Console.Write(48 + i);
                i--;
            }
            Console.Write('\n');
        }

        // Note Roslyn inverts the condition so this produces a beq instruction in the IL
        private static void TestNotEqualBranching()
        {
            short a = 4;
            short b = 4;
            if (a == b)
            {
                Console.WriteLine("True");
            }
            else
            {
                Console.WriteLine("False");
            }
        }

        // Note Roslyn inverts the condition so this produces a bne instruction in the IL
        private static void TestEqualBranching()
        {
            short a = 4;
            short b = 5;
            if (a != b)
            {
                Console.WriteLine("True");
            }
            else
            {
                Console.WriteLine("False");
            }
        }

        private static void HelloWorld()
		{
            Console.WriteLine("Hello World");
		}

        // Experiment to see if we can write routine to update video memory completely in C#!!
        // ...
        // and it worked!!
        // although codegen z80 is very inefficient
        private unsafe static void WriteChar(short x, short y, byte ch)
        {
            byte* screenMemory = (byte*)0x3c00;
            short offset = 0;
            for (int i = 0; i < y; i++)
            {
                offset += 64;
            }

            screenMemory[offset + x] = ch;
        }
    }
}
