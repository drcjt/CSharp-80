using dnlib.DotNet.Writer;
using Internal.Runtime.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MiniBCL
{
    public class GenericArrayEnumerable<T> : IEnumerable<T>
    {
        private T[] _items;
        public GenericArrayEnumerable(T[] items)
        {
            _items = items;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var index = 0; index < _items.Length; index++)
            {
                yield return _items[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class Gen<T>
    {
        public T? Field1;

        public bool EqualNull(T? t)
        {
            Field1 = t;
            return ((object?)Field1 == null);
        }
    }


    /*
        public class Gen<T>
        {
            public T Assign(T t)
            {
                T Field1 = t;
                return Field1;
            }
        }
    */
    public class Generic<T>
    {
        /// <summary>
        /// public T[] Items { get; set; }
        /// </summary>
        /// <param name="x"></param>
        //public void Frob(T x) => Console.WriteLine("Frob");
        public T[] Frob(T x)
        {
            var a = new T[10];
            a[0] = x;
            return a;
            //Console.WriteLine(x.ToString());
        }
        /*
        {
            Console.WriteLine(x.ToString());
            //Console.WriteLine("Frob");
        }
        */
    }

    public class GenericList<T> where T : new()
    {
        private T _item;

        public GenericList()
        {
            //_item = new T();
        }
    }

    public class Measure
    {
        public static int a = 0xCC;
    }

    // This is not beforefieldinit
    public class TestClass
    {
        static TestClass()
        {
            Measure.a = 8;
        }
    }

    public class TestComplexConstructor
    {
        private static int _value;

        static TestComplexConstructor()
        {
            _value = GetValue();
        }

        private static int GetValue()
        {
            return 25;
        }

        public static void Run()
        {
            Assert.IsLazyInitialized<TestComplexConstructor>();
        }
    }

    public class Assert
    {
        public static void IsLazyInitialized<T>()
        {
            if (!RuntimeHelpers.HasCctor<T>())
            {
                Environment.Exit(1);
            }
        }
    }

    public class Foo
    {
        public int i, j, k;
    }

    class GenericFoo<T>
    {
        public GenericFoo(T i1)
        {
        }
    }

    public static class Program
    {
        private static int _counter = 0;
        private static bool _result = true;

        public static void Eval(bool exp)
        {
            _counter++;
            if (!exp)
            {
                _result = exp;
                Console.Write("InstanceAssignmentClass failed at location: ");
                Console.WriteLine(_counter);
            }
        }

        public static int I4_DivPow2_2(int i4) // => i4 / 2;
        {
            int result = i4 / 2;
            return result;
        }

        public static int Count<T>(T[] arrayOfT)
        {
            return arrayOfT.Length;
        }

        private static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        private static void SwapIndirect<T>(ref T a, ref T b)
        {
            Swap<T>(ref a, ref b);
        }

        static object newarr<T>()
        {
            object o = new T[10];
            return o;
        }

        public static int test_0_vt_newarr()
        {
            object o = newarr<Foo>();
            //if (!(o is Foo[]))
            return 1;
            //return 0;
        }

        static GenericFoo<T> Newobj<T>(T t1)
        {
            return new GenericFoo<T>(t1);
        }


        public static int Main()
        {
            var s = "hello";
            Newobj(s);
            //test_0_vt_newarr();
            //new Gen<string>().EqualNull("string");
            //Eval(!new Gen<string>().EqualNull(_string));

            //TestComplexConstructor.Run();

            /*
            if (!RuntimeHelpers.HasCctor<Measure>())
            {
                Console.WriteLine("HasCctor");
            }
            */

            /*
            byte b = 0x0f;

            if (Measure.a != 0xCC)
            {
                Console.WriteLine("Not 0xCC at start");
            }

            // This should trigger the cctor as ctor is an instance method
            TestClass t = new TestClass();

            if (Measure.a != 8)
            {
                Console.WriteLine("Not 8 at end");
            }
            */

            /*
            var testArray = new int[1];
            var enumerable = new GenericArrayEnumerable<int>(testArray);
            var enumerator = enumerable.GetEnumerator();
            */

            /*
            int index = 0;
            foreach (var item in enumerable)
            {
                Console.WriteLine(testArray[index++]);
                //Assert.AreEquals(testArray[index++], item);
            }
            */

            /*
            Console.ReadLine();

            string _string = "string";
            var result = new Gen<string>().Assign(_string);
            Console.WriteLine(result);
            */

            //var s = new GenericList<Object>();


            //Console.WriteLine(Unsafe.SizeOf<sbyte>());
            /*
            int x = 12;
            int y = 17;

            SwapIndirect<int>(ref x, ref y);
            */

            //object o = "Spong";
            //Console.WriteLine(o.ToString());

            //var x = new Generic<object>();
            //var y = new Generic<string>();

            //var o = new object();

            //x.Frob(x);
            //x.Items[0] = x;

            //var r = y.Frob("Frobber");
            //Console.WriteLine(r[0]);

            //var s = x.Frob("foo");
            //Console.WriteLine(s[0].ToString());

            //y.Items[0] = "Wibble";
            //Console.WriteLine(y.Items[0]);
            //int x = -43;

            //int result = I4_DivPow2_2(42);
            //int result = x / 2;

            //Console.WriteLine(result);
            /*
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

            */
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
