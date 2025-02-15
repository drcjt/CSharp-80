using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.Importer;
using ILCompiler.Compiler.PreInit;
using ILCompiler.TypeSystem.Common;
using Internal.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MiniBCL
{
    public class ArrayEnumerable<T>(T[] array) : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            var index = 0;
            while (index < array.Length)
            {
                yield return array[index++];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class Test<T>()
    {
        public void ToColin()
        {
            var array = new T[10];
        }
    }

    public struct UnboxingStruct
    {
        public int m_value;

        public UnboxingStruct(int value)
        {
            m_value = value;
        }

        public override int GetHashCode() => m_value;
    }

    interface ITester<T>
    {
        void Test();
    }

    struct MyTester<T> : ITester<T>
    {
        public int Value;

        public void Test()
        {
            Value = 12345;
        }
    }

    class TesterClass<T> : ITester<T>
    {
        public int Value;

        public void Test()
        {
            Value = 12345;
        }
    }

    class TesterBase<T>
    {
        public int Value;

        public void Test()
        {
            Value = 12345;
        }
    }

    /*
    struct MyTester2<T> : TesterBase<T>
    {
    }
    */

    interface IInterface<T>
    {
        string Method();
    }

    class Base<T> : IInterface<T>
    {
        string IInterface<T>.Method() => "Base<T>.Method<X>()";
    }

    class Derived<T, U> : Base<T>, IInterface<U>
    {
        string IInterface<U>.Method() => "Derived<T,U>.Method<X>()";
    }

    class Atom1 { }
    class Atom2 { }
    class Atom3 { }


    public interface ITestable
    {
        public abstract void Test();
    }

    public class TestableClass : ITestable
    {
        public void Test() { }
    }

    public class MyTester3<T> where T : ITestable
    {
        public void RunTest(T t)
        {
            t.Test();
        }
    }

    public static class GVMFake<T>
    {
        public static RuntimeTypeHandle M(T t)
        {
            return t.GetEEType();
        }
    }

    public class SomeBaseClass
    {
        public static void MyStaticMethod() { }
    }

    public class SomeDerivedClass : SomeBaseClass
    {

    }

    interface IWibble
    {
        void Method();
    }

    class Wibble : IWibble
    {
        public void Method() { }
    }

    static class WibbleStatic<T> where T : IWibble
    {
        public static void Method() { }
    }

    public class EquatableObject : IEquatable<EquatableObject>
    {
        public bool Equals(EquatableObject? other)
        {
            Console.WriteLine("EquatableObject.Equals");
            return false;
        }
    }

    public static class Program
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        static string ConstrainedCall<T, U, X>(ref T instance) where T : IInterface<U>
        {
            return instance.Method();
        }


        public static void Method<T>()
        {
            var t = new Test<T>();
            t.ToColin();
        }

        public static void GetEnumerator()
        {
            var realArray = new int[] { 1, 2, 3 };
            Array array = realArray;
            //Assert.IsTrue(array.GetEnumerator() != array.GetEnumerator());

            Console.WriteLine("Enumerating");

            IEnumerator enumerator = array.GetEnumerator();
            //for (int i = 0; i < 2; i++)
            {
                int counter = 0;
                while (enumerator.MoveNext())
                {
                    Console.WriteLine("MovedNext");
                    object current = enumerator.Current;
                    Console.WriteLine("Got Current");

                    //Console.WriteLine(array.GetValue(counter).ToString());

                    //Console.WriteLine(array[counter].ToString());
                    //Console.WriteLine(enumerator.Current.ToString());

                    //Assert.AreEqual(array[counter], enumerator.Current);
                    counter++;
                }

                Console.WriteLine("Finished enumeration");

                //Assert.IsFalse(enumerator.MoveNext());
                //Assert.AreEqual(array.Length, counter);

                enumerator.Reset();
            }
        }

        public static bool Compare<T>(T left, T right) where T : IEquatable<T>
        {
            return left.Equals(right);
        }

        private static void DoConstrainedCall<T, U>(ref T t) where T : ITester<U>
        {
            t.Test();
        }

        private static void DoConstrainedCall2<T, U>(ref T t) where T : TesterBase<U>
        {
            t.Test();
        }

        private static void DoConstrainedCall3<T>(T t) where T : ITestable
        {
            t.Test();
        }

        private static void DoConstrainedCall4<T>() where T : SomeBaseClass
        {
            SomeBaseClass.MyStaticMethod();
        }

        private static void PrintName<T>(string prefix) where T : class
        {
            
            Console.WriteLine(prefix);
        }

        struct NoToString
        {
            //int X;

            public NoToString(/*int x*/) { /* X = x; */ }

            public override string ToString() => "Bar";
        }

        public static void TestSwitch(int i)
        {
            switch (i)
            {
                case 1:
                    Console.WriteLine("1");
                    break;

                case 2:
                    Console.WriteLine("2");
                    break;

                case 3:
                    Console.WriteLine("3");
                    break;

                default:
                    Console.WriteLine("Default");
                    break;
            }
        }

        public static int TestValue(int value) => value > 0 ? 1 : 0;

        static bool ArrayEquals(Array x, Array y)
        {
            if (x.Length != y.Length)
                return false;

            for (int i = 0; i < x.Length; i++)
            {
                var vx = x.GetValue(i);
                var vy = y.GetValue(i);
                Console.WriteLine(vx.ToString());
                Console.WriteLine(vy.ToString());

                if (object.Equals(x.GetValue(i), y.GetValue(i)))
                    return false;
            }

            return true;
        }

        public static int Main()
        {
            int[] array1 = new int[] { 1,2,3,4,5 };
            int[] array2 = new int[] { 1, 2, 3, 4, 5 };

            var result = ArrayEquals(array1, array2);
            Console.WriteLine(result ? "Equal" : "Not-Equal");


            /*
            foreach (var i in array)
                Console.WriteLine(i);
            */

            /*
            object m = new object();
            object n = new object();

            object[] objarr = new object[] { m, n };
            var index = Array.IndexOf<object>(objarr, n, 0, 2);
            Console.WriteLine(index);

            var x = new EquatableObject();
            var y = new EquatableObject();
            var duparr = new EquatableObject[] { x, y };
            var index2 = Array.IndexOf<EquatableObject>(duparr, y, 0, 2);
            Console.WriteLine(index2);
            */

            //nint min = -32768;
            //nint one = +1;
            //nint _one = -1;
            //nint max = +32767;
            //nint even = -21846;

            //nuint one = 1;
            //nuint even = 21846;

            //var result = one > even;   // Should be false

            /*
            if (result)
            {
                Console.WriteLine("_one > even");
            }
            */

            //if (TestValue(int.MaxValue) != 0)
                //Console.WriteLine("Failed");
            /*
            TestValue(int.MinValue+1);
            TestValue(0);
            TestValue(int.MaxValue);
            TestValue(int.MaxValue - 1);
            */

            /*
            Console.WriteLine(Number.Int32ToDecStr(int.MinValue));
            Console.WriteLine(Number.Int32ToDecStr(int.MinValue + 1));
            Console.WriteLine(Number.Int32ToDecStr(12345));
            Console.WriteLine(Number.Int32ToDecStr(int.MaxValue));
            Console.WriteLine(Number.Int32ToDecStr(-12345));

            Console.WriteLine(Number.Int32ToDecStr(0));
            */

            /*
            int[] arr = new int[3] { 0, 1, 2 };
            Array a = arr;
            IList l = a;

            // THIS ISN'T WORKING!!
            Console.WriteLine(l.Count);
            */

            /*
            int[] arr = new int[3] { 0, 1, 2 };
            IEnumerable<int> enumerable = arr;

            foreach (var item in enumerable)
            {
                Console.WriteLine(item);
            } 

            var list = arr.ToList<int>();

            Console.WriteLine("After ToList<int>");

            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
            */

            //var result = Unsafe.SizeOf<sbyte>();
            //Console.WriteLine(result);

            //Console.WriteLine(result ? "Equal" : "Not Equal");

            //int[] arr = new int[3] { 0, 1, 2 };
            //var result = Array.IndexOf<int>(arr, 1, 0, 3);
            //Console.WriteLine(result);

            //TestSwitch(1);
            //var x = System.Collections.Generic.EqualityComparerHelpers.GetComparerForReferenceTypesOnly<string>();
            //var result = x.Equals("foo", "foo");
            //Console.WriteLine(result ? "Equal" : "Not Equal");
            //Console.WriteLine(x is null ? "Null" : "not null");

            //var y = System.Collections.Generic.EqualityComparerHelpers.StructOnlyEquals<char>('b', 'b');
            //Console.WriteLine(y ? "Equal" : "Not Equal");

            //PrintName<TestableClass>("foo");
            //var wibble = new Wibble();
            //WibbleStatic<Wibble>.Method();

            //var sbc = new SomeDerivedClass();
            //DoConstrainedCall4<SomeDerivedClass>();

            //var t = new TestableClass();
            //DoConstrainedCall3<TestableClass>(t);
            //var t = new TesterBase<int>();
            //DoConstrainedCall2<TesterBase<int>, int>(ref t);
            //GVMFake<string>.M("hello");
            //var result = GVMFake<int>.M(123);
            //Console.WriteLine(result.ToString());
            //if (!result.Equals(123.GetEEType()))
            //{
            //Console.WriteLine("Different");
            //}

            //Console.WriteLine("Same");

            /*
            NoToString nts = new NoToString();
            nts.GetEEType();
            var result = nts.ToString();
            Console.WriteLine(result);
            */

            //TestableClass tc = new TestableClass();
            //MyTester3<TestableClass> mt = new MyTester3<TestableClass>();
            //mt.RunTest(tc);

            /*
            int i = 43;
            Compare<int>(i, 25);
            var result = i.Equals(43);
            */

            //var derived = new Derived<Atom1, Atom2>();
            //ConstrainedCall<Derived<Atom1, Atom2>, Atom2, Atom3 > (ref derived);


            //var foo = new MyTester<int>();
            //DoConstrainedCall<MyTester<int>, int>(ref foo);
            //var foo = new TesterClass<int>();
            //DoConstrainedCall<TesterClass<int>, int>(ref foo);
            //var foo = new MyTester2<int>();
            //DoConstrainedCall2<MyTester2<int>, int>(ref foo);

            //int? n = new int?(42);

            //int? n = default(int?);
            //int? e = 42;



            //var result = e.Equals(n);
            //Console.WriteLine(result ? "Equal" : "Not Equal");


            /*
                        int[] a = new int[] { 1, 2, 3, 4, 5 };
                        var index = Array.IndexOf<int>(a, 4, 0, 5);
                        Console.WriteLine(index);
            */
            /*
            object m = new object();
            object n = new object();

            object[] objarr = new object[] { m, n };
            index = Array.IndexOf<object>(objarr, n, 0, 2);
            Console.WriteLine(index);
            */

            /*
            int i = 25;
            int j = 25;

            int start = GC.GetTotalMemory();

            //var force = i.Equals(25);

            var result = Compare<int>(i, j);

            int end = GC.GetTotalMemory();

            Console.WriteLine(result ? "True" : "False");

            Console.WriteLine(end - start);
            */
            /*
            int i = 25;
            int j = 25;
            var result = Compare<int>(i, j);
            //var result = i.Equals(j); // Compare<int>(i, j);
            Console.WriteLine(result ? "True" : "False");
            */
            /*
            object?[] a = new object?[] { 25 };
            Array arr = a;
            var elem = a.GetValue(0);
            int i = (int)elem;
            Console.WriteLine(i);
            */

            /*
            var index = Array.IndexOf(arr, 1);
            Console.WriteLine(index);
            */

            /*
            ArrayList list = new ArrayList();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            var index = list.IndexOf(1);
            Console.WriteLine(index);
            */

            //UIntPtr ptr = (UIntPtr)3;
            /*
            int[] array = [7, 7, 8, 8, 9, 9];
            IList iList = array;
            Console.WriteLine(iList.IndexOf(8));
            Console.WriteLine(iList.Contains(8) ? "Yes" : "No");
            */

            /*
            int[] sourceArray = new int[] { 1, 2, 3 };
            Array a = sourceArray;
            a[0] = 25;
            Console.WriteLine(sourceArray[0]);
            */

            //int i = -2147483648;
            //Console.WriteLine(i.ToString());
            //var obj1 = new object();
            //var obj2 = new object();

            //Equals(obj1, obj2);

            //Equals(obj1, obj2);

            //GetEnumerator();

            //object boxed = 25;
            //Console.WriteLine("Start");
            //int[] sourceArray = new int[] { 1, 2, 3 };
            //Array a = sourceArray;

            //Debugger.Break();

            /*
            Console.WriteLine(a.Length);

            int elementSize = (int)(a.ElementSize);
            Console.WriteLine(elementSize);

            int s = (int)a.InternalGetValue(0);
            Console.WriteLine("After InternalGetValue");
            Console.WriteLine(s);
            */

            /*
            IEnumerable enumerable = sourceArray;
            foreach (var item in enumerable)
            {
                //int i = (int)item;
                Console.WriteLine(item.ToString());
            }
            */

            /*
            int[] resultArray = sourceArray.ToArray();
            Console.WriteLine("After ToArray");

            Debug.Assert(sourceArray != resultArray);
            Debug.Assert(sourceArray.Equals(resultArray));
            */

            /*var sarray = new string[] { "abc", "defg", "hi" };
            var newList = sarray.ToList<string>();
            Console.WriteLine("After ToList");
            foreach (var item in newList)
                Console.WriteLine(item);
            */

            /*
            var list = new List<int> { 1,2,3 };

            var gcStart = GC.GetTotalMemory();

            Console.WriteLine(list.IndexOf(5));

            var gcEnd = GC.GetTotalMemory();
            Console.WriteLine($"Heap allocated = {gcEnd - gcStart}");

            */
            /*
             * var array = list.ToArray<string>();

            foreach (var item in array)
                Console.WriteLine(item);


            Console.WriteLine(list.Count<string>());
            Console.WriteLine(list.Any<string>() ? "Not Empty" : "Empty");
            */

            //int[] array = new int[1] { 25 };

            //if (array[0].Equals(1))
            //Console.WriteLine("Equals Test");

            //if (Array.IndexOf<int>(array, 25, 0, 1) >= 0)
            //Console.WriteLine("One is in list");

            //Method<int>();
            //Test<int> t = default;
            //t.ToArray();

            /*
            var items = new List<string>() { "One", "Two" };
            Console.WriteLine(items.Count);

            int iterations = 0;
            IEnumerator enumerator = items.GetEnumerator();
            while (enumerator.MoveNext())
            {
                iterations++;
            }
            Console.WriteLine(iterations);

            iterations = 0;
            foreach (var i in items)
                foreach (var i2 in items)
                    foreach(var i3 in items)
                        iterations++;
            Console.WriteLine(iterations);

            */

            /*
            for (int i = 0; i < items.Count; i++)
            {
                Console.WriteLine(items[i]);
            }
            */
            /*
            foreach (var item in items)
            {
                Console.WriteLine(item);
            }
            */

            /*
            var item = Console.ReadLine();
            while (item[0] != 'X')
            {
                items.Add(item);
                item = Console.ReadLine();
            }
            */


            /*
            foreach (var i in items)
            {
                Console.WriteLine(i);
            
            }
            */

            //var genlist = new List<int>() { 1, 2, 3, 4, 5 };

            /*
            var genlist = new List<int>();

            genlist.Add(1);
            genlist.Add(2);
            genlist.Add(3);
            */

            /*foreach (var gen in genlist)
            {
                Console.WriteLine(gen);
            }
            */

            /*
            var sourceArray = new char[] { 'a', 'b', 'c', 'd', 'e', 'f' };
            var sourceEnumerable = new ArrayEnumerable<char>(sourceArray);
            var resultArray = sourceEnumerable.ToArray();

            foreach (char i in resultArray)
                Console.WriteLine(i);
            */

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