﻿using System;

namespace MiniBCL
{
    public static class Program
    {
        public static int Main()
        {
            Console.Clear();

            TestImplicitCasting();

            Int32 returnedInt32 = TestReturnInt32();
            Console.WriteLine(returnedInt32);

            TestDiv();
            TestRem();

            // Test writing out Int32 value
            TestPassingInt32(123456);

            // Test subroutine call
            HelloWorld();

            // Test writing int16 positive value
            Console.WriteLine((short)589);

            // Test writing int16 negative value
            Console.WriteLine((short)-8537);

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
            //WriteChar(0, 0, 48); // Write 0 to top left corner of screen

            return 42;
        }

        private static void TestImplicitCasting()
        {
            short x = 3;
            short y = 7;

            // When code is generated we should have an implicit int16 to int32 cast
            // inserted as roslyn will be targetting the WriteLine(Int32) method here
            // yet the result of x + y will be an int16

            Console.WriteLine(x + y);
        }

        private static Int32 TestReturnInt32()
        {
            Int32 result = 123456;
            return result;
        }

        private static void TestPassingInt32(int value)
        {
            Console.Write(value);
            Console.WriteLine();
        }

        private static void TestDiv()
        {
            // Unsigned division
            uint a = 100;
            uint b = 5;
            Console.WriteLine(a / b);

            // signed divison
            int x = -90;
            int y = 3;
            Console.WriteLine(x / y);
        }

        private static void TestRem()
        {
            // Unsigned remainder
            uint a = 100;
            uint b = 3;
            Console.WriteLine(a % b);

            // signed remainder
            int x = -100;
            int y = 7;
            Console.WriteLine(x % y);
        }


        private static void TestArguments(short a, short b, short c)
        {
            Console.Write((char)(48 + a));
            Console.Write((char)(48 + b));
            Console.Write((char)(48 + c));
            Console.WriteLine();
        }

        private static void TestBooleanComparison()
        {
            bool b = true;
            if (b)
            {
                Console.WriteLine("Bool Test");
            }
        }

        private static void TestAddition()
        {
            short a = 50;
            short b = 3;
            Console.Write((short)(a + b));
            Console.WriteLine();
        }

        private static void TestLessThanBranching()
        {
            short i = 0;
            while (i < 5)
            {
                Console.Write((char)(48 + i));
                i++;
            }
            Console.WriteLine();
        }

        private static void TestGreaterThanBranching()
        {
            short i = 5;
            while (i > 0)
            {
                Console.Write((char)(48 + i));
                i--;
            }
            Console.WriteLine();
        }

        private static void TestLessThanOrEqualBranching()
        {
            short i = 0;
            while (i <= 5)
            {
                Console.Write((char)(48 + i));
                i++;
            }
            Console.WriteLine();
        }

        private static void TestGreaterThanOrEqualBranching()
        {
            short i = 5;
            while (i >= 0)
            {
                Console.Write((char)(48 + i));
                i--;
            }
            Console.WriteLine();
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
            for (short i = 0; i < y; i++)
            {
                offset += 64;
            }

            screenMemory[offset + x] = ch;
        }

        // Experiment to write short to int widening conversion in pure C# code vs z80 assembly
        // TODO: This requires conv.i2 to be implemented in the AOT/CodeGenerator
        private unsafe static int Widen(short s)
        {
            Int32 retval;
            Int16* ptr = (Int16*)&retval;

            *ptr++ = (short)(s & 0x7F);
            *ptr = (short)(s & 0x80);

            return retval;
        }
    }
}