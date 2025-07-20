using Internal.Runtime.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;

namespace MiniBCL
{
    public static class TestHelpers
    {
        public static void Validate<T>(this Span<T> span, params T[] expected) where T : struct, IEquatable<T>
        {
            Assert.True(span.SequenceEqual(expected));
        }

        public static void Validate<T>(this ReadOnlySpan<T> span, params T[] expected) where T : struct, IEquatable<T>
        {
            Assert.True(span.SequenceEqual(expected));
        }

        public static void ValidateReferenceType<T>(this Span<T> span, params T[] expected) where T : class
        {
            Assert.Equal(span.Length, expected.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                T actual = span[i];
                Assert.Same(expected[i], actual);
            }

            bool exceptionThrown = false;
            try
            {
                _ = span[expected.Length];
            }
            catch (IndexOutOfRangeException)
            {
                exceptionThrown = true;
            }

            Assert.True(exceptionThrown);
        }

        public static void ValidateReferenceType<T>(this ReadOnlySpan<T> span, params T[] expected) where T : class
        {
            Assert.Equal(span.Length, expected.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                T actual = span[i];
                Assert.Same(expected[i], actual);
            }

            bool exceptionThrown = false;
            try
            {
                _ = span[expected.Length];
            }
            catch (IndexOutOfRangeException)
            {
                exceptionThrown = true;
            }

            Assert.True(exceptionThrown);
        }
    }

    public struct GenericWrapper<T>
    {
        public T Field;

        public GenericWrapper(T field)
        {
            Field = field;
        }
    }

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

    public class StaticTest
    {
        public static byte B;
        public static int X;
        public static int Y;
    }

    public class Bug545Test<T>()
    {
        public T[] ToArray()
        {
            var array = new T[10];
            return array;
        }
    }

    public class ArrayEnumerable(int[] array) : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var index = 0;
            while (index < array.Length)
            {
                yield return array[index++];
            }
        }
    }

    internal class DefaultClass
    {
        public class Gen<T>
        {
            public bool DefaultTest(bool status)
            {
                T? t = default(T);
                return ((object?)t == null) == status;
            }
        }

        private static int _counter = 0;
        private static bool _result = true;
        public static void Eval(bool exp)
        {
            _counter++;
            if (!exp)
            {
                _result = exp;
                Console.Write("DefaultClass failed at location: ");
                Console.WriteLine(_counter);
            }
        }

        public static bool RunTests()
        {
            Eval(new Gen<int>().DefaultTest(false));

            return _result;
        }
    }

    internal class DefaultStruct
    {
        public struct Gen<T>
        {
            public bool DefaultTest(bool status)
            {
                T? t = default(T);
                return (((object?)t == null) == status);
            }
        }

        private static int _counter = 0;
        private static bool _result = true;
        public static void Eval(bool exp)
        {
            _counter++;
            if (!exp)
            {
                _result = exp;
                Console.Write("DefaultStruct failed at location: ");
                Console.WriteLine(_counter);
            }
        }

        public static bool RunTests()
        {
            Eval(new Gen<int>().DefaultTest(false));

            return _result;
        }
    }

    public struct vcClass
    {
        public int Field1;
        public char Field2;

        public void Init()
        {
            Field1 = 255;
            Field2 = 'T';
        }

        public override string ToString()
        {
            return "Some Test String";
        }
    }

    /*
    public class TestClass
    {
        private int field1;
        private bool field2;
        private bool field3;
    }
    */

    public class Maze
    {
        private int[,] maze;
        private int width, height;

        public Maze(int width, int height)
        {
            this.width = width;
            this.height = height;
            maze = new int[width, height];
        }
    }

    public class Measure
    {
        public static int a = 0xCC;
    }

    public class TestClass
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void f(ref byte b)
        {
            return;
        }

        static TestClass()
        {
            //int a = GetInitValue();
            Measure.a = 8;
        }

        private static int GetInitValue() => 8;
    }

    public struct MyStruct
    {
        public string str;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MyStruct MakeStruct(string s)
        {
            MyStruct ms;
            ms.str = s;
            return ms;
        }
    }

    public class A
    {
        private int _prop;
        public int Prop
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (_prop != 100) ? _prop : 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value == 1)
                {
                    _prop = value + 99;
                }
                else if (value == 2)
                {
                    _prop = value + 98;
                }
                else
                {
                    _prop = value;
                }
            }
        }
    }


    public class Stack
    {
        private object?[] _array;
        private int _size;

        public Stack()
        {
            _array = new object[100];
            _size = 0;

            _array[0] = 25; // Initialize the first element
        }

        public virtual object? Pop()
        {
            if (_size == 0)
                throw new InvalidOperationException("Stack is empty");

            object? value = _array[--_size];
            _array[_size] = null; // Clear the reference
            return value;
        }
    }

    public class Program
    {
        public static object PopTest()
        {
            object[] arr = new object[100];
            arr[0] = 25;

            int size = 1;

            object value = arr[--size];
            arr[size] = null;
            return value;
        }

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

        [DllImport("C:\\Users\\drcjt\\source\\repos\\CSharp-80-main\\Samples\\Sample\\bin\\Trs80\\Debug\\net9.0\\foobar.asm", EntryPoint = "TestDllImport")]
        public static unsafe extern void TestDllImport();

        public static int GetZero()
        {
            return 0;
        }

        public static int GetValue(int v)
        {
            return v;
        }

        public static T[] Bug545Method<T>()
        {
            var t = new Bug545Test<T>();
            return t.ToArray();
        }

        public static vcClass vc;

        /*
        public static void Validate<T>(this Span<T> span, params T[] expected) where T : struct, IEquatable<T>
        {
            Debug.Assert(span.SequenceEqual(expected));
        }
        */

        private static void Contains_Match_Char_For_Length(int length)
        {
            var start = DateTime.Now;

            char[] ca = new char[length];
            for (int i = 0; i < length; i++)
            {
                ca[i] = (char)(i + 1);
            }

            //var span = new Span<char>(ca);
            //var ros = new ReadOnlySpan<char>(ca);
            var str = new string(ca);

            var end = DateTime.Now;
            Console.WriteLine(end.TotalSeconds - start.TotalSeconds);

            for (var targetIndex = 0; targetIndex < length; targetIndex++)
            {
                start = DateTime.Now;
                //char target = ca[targetIndex];

                /*
                bool found = span.Contains(target);
                Assert.IsTrue(found);

                found = ros.Contains(target);
                Assert.IsTrue(found);
                */

                //bool found = str.Contains(target);
                //Assert.IsTrue(found);
                end = DateTime.Now;
                Console.WriteLine(end.TotalSeconds - start.TotalSeconds);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int InlineTest()
        {
            /*
            while (inlineeLocalVar > 0)
            {
                inlineeLocalVar--;
            }
            */

            return InlineTest2() * 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int InlineTest2()
        {
            return 25;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InlineTest3<T>(T j)
        {
            Console.WriteLine(j.ToString());
            //return DateTime.Now;
        }

        private static string MockReadLine()
        {
            return "6";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int InlineTest4(int a, int b)
        {
            return a + b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int InlineTest5(int a, int b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        public unsafe static void Bug617()
        {
            byte value = 42;
            Unsafe.Add<byte>(ref value, 0);
        }

        private static nuint MethodCall_WithNuintParameter_CompilesWithoutErrors(nuint n)
        {
            return n;
        }

        public static void Bug87()
        {
            char[] test = new char[1] { 'a' };
            Bug87_Method(test[0]);
        }

        private static char Bug87_Method(char ch)
        {
            return ch;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int BarInline(int v)
        {
            Console.WriteLine("Entering BarInline: v=");
            int ret = v * 2;
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FooInline(int v)
        {
            int ret = BarInline(v + 1) * 4;
            Console.WriteLine(ret);
            return ret;
        }

        public static void FirstMethod(int a)
        {
            SecondMethod(a, 2);
        }

        public static void SecondMethod(int x, int y)
        {
            throw new Exception();
        }

        private static int s_c = 1;

        public static void Pop_EmptyStack_ThrowsInvalidOperationException()
        {
            bool exceptionThrown = false;
            var stack = new Stack();
            try
            {
                stack.Pop();
            }
            catch (InvalidOperationException)
            {
                exceptionThrown = true;
            }
            Assert.True(exceptionThrown);
        }

        //public static int[,] testArray = { { 1, 2, 3 }, { 4, 5, 6 } };
        public unsafe static int Main(string[] args)
        {
         //   Pop_EmptyStack_ThrowsInvalidOperationException();
            try
            {
                FirstMethod(25);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught exception: " + ex.Message);
            }

            /*
            bool exceptionThrown = false;
            var stack = new Stack();
            try
            {
                stack.Pop();
            }
            catch (InvalidOperationException)
            {
                exceptionThrown = true;
            }

            Console.WriteLine(exceptionThrown ? "Exception Thrown" : "Exception not thrown");
            Assert.True(exceptionThrown);

            */

            return 0;

            //return (int)PopTest();
            //int v = (int)PopTest();

            /*
            var s = new Stack();
            int v = (int)s.Pop();
            */

            //Console.WriteLine(v);

            //return 0;

            /*
            var r = new Random();

            if (r.Next() > 10)
            {
                Console.WriteLine("Random number is greater than 10");
            }
            else
            {
                Console.WriteLine("Random number is 10 or less");
            }

            return 0;
            */

            //char[] test = new char[1] { 'a' };
            //var array = test as Array;
            //ref byte element = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetArrayDataReference(array), 2);
            //var ch = array[0];


            /*
            object o1 = new object();
            object o2 = new object();
            object[] a = { o1, o2 };
            Span<object> span;

            span = new Span<object>(a);

            if (span[1] != o2)
            {
                Console.WriteLine("Span[1] != o2");
                return 1;
            }

            return 0;
            */

            /*
            Bug87();

            nuint testValue = 123;
            Assert.Equal(testValue, MethodCall_WithNuintParameter_CompilesWithoutErrors(testValue));
            */

            //return 0;
            //Bug617();

            //return 0;
            /*
            A a = new A();
            a.Prop = 1;
            int retval = a.Prop;

            return retval;
            */
            //var result = InlineTest5(10, 5);
            //Console.WriteLine(result);
            /*
            char[] array = new char[5];
            var s = new string(array);

            Console.WriteLine(s);
            */

            /*
            var str1 = new object();
            var str2 = new object();

            if (str1.GetEEType().Equals(str2.GetEEType()))
            {
                Console.WriteLine("EEType is the same for both string instances.");
            }
            else
            {
                Console.WriteLine("EEType is different for the string instances.");
            }
            */

            //MyStruct ms = MyStruct.MakeStruct("Hello, World!");

            //Assert.Equal("Hello, World!", ms.str);
            //Console.WriteLine(ms.str);

            //Console.WriteLine("Inlining test passed.");

            //return 0;

            /*
            var chars = new char[25];
            string newString = new String(chars);
            */
            /*
                        byte b = 0x0f;

                        if (Measure.a != 0xCC)
                        {
                            Console.WriteLine("0");
                        }

                        // Check when method is inlined that cctor is still called
                        TestClass.f(ref b);

                        if (Measure.a != 8)
                        {
                            Console.WriteLine("1");
                        }
            */

            /*
            int a = 25;
            var result = InlineTest4(25, 17);
            Console.WriteLine(result);

            int b = a + 1;
            Console.WriteLine(a);
            */

            //var p = new Program();

            //Console.Clear();
            //Console.WriteLine("Calculate Pi");
            //Console.Write("How many digits of Pi do you wish to calculate? ");
            //var digits = int.Parse(Console.ReadLine());

            //var startTime = DateTime.Now;
            //var endTime = DateTime.Now;
            //var elapsedTime = endTime.TotalSeconds; // - startTime.TotalSeconds;

            //Console.WriteLine($"Elapsed Time (seconds): {elapsedTime}");

            //var p = new Program();

            //p.InlineTest3(25);
            //Console.WriteLine(result);
            /*
            Console.Clear();
            Console.WriteLine("Calculate Pi");
            Console.Write("How many digits of Pi do you wish to calculate? ");
            var digits = int.Parse(Console.ReadLine()); // Console.ReadLine());
            var startTime = DateTime.Now;
            */

            //Console.WriteLine(digits);
            //var a = InlineTest();
            //Console.WriteLine(a);

            /*
            if (args == null)
            {
                Console.WriteLine("Args is null");
            }
            */
            /*
            var a = "foobar";
            var b = "wibble";

            var s = $"{a}{b}";
            Xunit.Assert.True(true);

            */
            /*
            var arr = new int[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Span<int> bytes = arr;

            Console.WriteLine(bytes[2]);
            */

            /*
            byte* sb = stackalloc byte[2];

            sb[0] = 42;

            Console.WriteLine(sb[0]);
            */

            //byte* sb = stackalloc byte[2];
            //Span<byte> bytes2 = new Span<byte>(sb, 2);

            // FAILS due to localalloc happening in the setup of the call arguments to Span ctor
            /*
            Span<byte> bytes2 = stackalloc byte[2];
            bytes2[0] = 42;
            bytes2[1] = 43;

            Console.WriteLine("After assignment");
            Console.ReadLine();

            Console.WriteLine(bytes2[0]);
            Console.WriteLine(bytes2[1]);

            Console.ReadLine();
            */

            /*
            var a = new int[10];

            var b = a;

            Console.WriteLine(b.Length);
            */

            //return 42;

            //Maze maze = new Maze(10, 10);
            /*
            int size = 10;
            int i, j, k;
            int locationNumber = 0;

            GenericWrapper<int>[,,] GenArray = new GenericWrapper<int>[size, size, size];

            var item = GenArray[10, 20, 10];

            for (i = 0; (i < size); i++)
            {
                for (j = 0; (j < size); j++)
                {
                    for (k = 0; (k < size); k++)
                    {
                        GenArray[i, j, k] = new GenericWrapper<int>(locationNumber);
                        locationNumber++;
                    }
                }
            }

            int sum = 0;
            for (i = 0; (i < size); i++)
            {
                for (j = 0; (j < size); j++)
                {
                    for (k = 0; (k < size); k++)
                    {
                        sum += GenArray[i, j, k].Field;
                        locationNumber++;
                    }
                }
            }
            */


            //int[][,] ja1 = new int[2][,];
            //ja1[0] = new int[,] { { 0, -1 }, { 0, 0 } };


            //var testArray = new int[2,3] { { 1, 2, 3 }, { 4, 5, 6 } };
            /*
            Console.WriteLine(testArray.Length);

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    var value = x + y;
                    Console.WriteLine(value);
                    testArray[x, y] = value;
                }
            }
            */

            /*
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    Console.Write(testArray[x, y]);
                    Console.Write(',');
                }
                Console.WriteLine();
            }
            */

            //testArray[1, 2] = 25;
            //var x = testArray[1, 2];

            //Console.WriteLine(x);

            //var str1 = new String(new char[5]);
            //var class1 = new TestClass();

            //var str1EEtype = str1.GetEEType();
            //var class1EEtype = class1.GetEEType();

            //Console.WriteLine(str1EEtype.ToString());
            //Console.WriteLine(class1EEtype.ToString());

            //if (str1.GetEEType().Equals(class1.GetEEType()))
            //Console.WriteLine("FAILED");

            /*
            vc.Init();
            if (vc.Field1 == 255 && vc.Field2.ToString().Equals("Some Test String"))
                Console.WriteLine("vcClass test passed");
*/

            /*
            var s = new DefaultStruct.Gen<int>();

            var result = DefaultClass.RunTests();
            result &= DefaultStruct.RunTests();
            */

            /*
            int i = 25;
            object o = i;

            if (!i.Equals(25))
                return 4;
            */

            /*
            String s = "Foo";
            String n = "Bar";
            if (s.Equals(n))
                Console.WriteLine("Foo");
            */

            /*
            var testArray = new int[] { 1, 2, 3 };
            foreach (var item in testArray)
            {
                Console.WriteLine(item.ToString());
            }
            */


            /*
            var testArray = new int[] { 1, 2, 3 };
            var enumerable = new ArrayEnumerable(testArray);

            int index = 0;
            foreach (var item in enumerable)
            {
                Console.WriteLine(item.ToString());
            }
            */

            //int result = Bug545Method<int>().Length;
            //int[] szArray = new int[4];
            //int[,] mdarray = new int[2, 3];

            /*
            StaticTest.X = 25;
            StaticTest.Y = 72;

            Console.WriteLine(StaticTest.X);
            Console.WriteLine(StaticTest.Y);

            fixed (int* b = &StaticTest.Y)
            {
                int v = *b;
                Console.WriteLine(v);
            }
            */

            //Console.Clear();
            //TestDllImport();
            //int[][] jagged = new int[3][];
            //int[,] mdarray = new int[2, 3];
            //int[] array2 = new int[] { 1, 2, 3, 4, 5 };

            //var result = ArrayEquals(array1, array2);
            //Console.WriteLine(result ? "Equal" : "Not-Equal");

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

            //return 42;
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