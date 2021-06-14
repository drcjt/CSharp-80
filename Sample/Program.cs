using System;

namespace MiniBCL
{
    public static class Program
    {
        public static int Main()
		{
            // Test subroutine call
            HelloWorld();

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

            // Test calling assembly code via pinvoke
            Console.WriteCh('Z');

            // Test implementation of write char completely written in C#
            WriteChar(0, 0, 48); // Write 0 to top left corner of screen

            return 42;
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
                Console.Write('T');
            }
            else
            {
                Console.Write('F');
            }
            Console.Write('\n');
        }

        // Note Roslyn inverts the condition so this produces a bne instruction in the IL
        private static void TestEqualBranching()
        {
            short a = 4;
            short b = 5;
            if (a != b)
            {
                Console.Write('T');
            }
            else
            {
                Console.Write('F');
            }
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
