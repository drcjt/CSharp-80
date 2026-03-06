using System;

namespace NullableTypes
{
    interface BaseInter { }
    interface GenInter<T> { }

    struct Struct { }
    struct ImplStruct : BaseInter { }
    struct OpenGenImplStruct<T> : GenInter<T> { }
    struct CloseGenImplStruct : GenInter<int> { }

    class Foo { }

    class NullableTest1
    {
        public static int exceptionCounter = 0;
        //Nullable types with ?
        static int? i;
        static Struct? s;
        static ImplStruct? imps;
        static OpenGenImplStruct<Foo>? genfoo;
        static CloseGenImplStruct? genint;

        public static void Run()
        {
            try
            {
                Console.WriteLine(i.Value);
                Console.WriteLine($"Test_nullabletypes Failed at location {exceptionCounter}");
                exceptionCounter++;
            }
            catch (System.InvalidOperationException e) { }

            /*
            try
            {
                Console.WriteLine(s.Value);
                Console.WriteLine($"Test_nullabletypes Failed at location {exceptionCounter}");
                exceptionCounter++;
            }
            catch (System.InvalidOperationException e) { }
            */

            /*
            try
            {
                Console.WriteLine(imps.Value);
                Console.WriteLine($"Test_nullabletypes Failed at location {exceptionCounter}");
                exceptionCounter++;
            }
            catch (System.InvalidOperationException e) { }

            try
            {
                Console.WriteLine(genfoo.Value);
                Console.WriteLine($"Test_nullabletypes Failed at location {exceptionCounter}");
                exceptionCounter++;
            }
            catch (System.InvalidOperationException e) { }

            try
            {
                Console.WriteLine(genint.Value);
                Console.WriteLine($"Test_nullabletypes Failed at location {exceptionCounter}");
                exceptionCounter++;
            }
            catch (System.InvalidOperationException e) { }
            */
        }
    }

    class NullableTest3
    {
        //Nullable types with ?
        static int? i = default(int);
        static Struct? s = new Struct();
        static ImplStruct? imps = new ImplStruct();
        static OpenGenImplStruct<Foo>? genfoo = new OpenGenImplStruct<Foo>();
        static CloseGenImplStruct? genint = new CloseGenImplStruct();

        public static void Run()
        {
            Test_nullabletypes.Eval(i.Value, default(int));
            Test_nullabletypes.Eval(s.Value, default(Struct));
            Test_nullabletypes.Eval(imps.Value, default(ImplStruct));
            Test_nullabletypes.Eval(genfoo.Value, default(OpenGenImplStruct<Foo>));
            Test_nullabletypes.Eval(genint.Value, default(CloseGenImplStruct));
        }
    }

    public static class Test_nullabletypes
    {
        public static int counter = 0;

        internal static void Eval(object obj1, object obj2)
        {
            counter++;

            if (!((obj1 != null) && (obj2 != null) && /*(obj1.GetType().Equals(obj2.GetType())) && */ obj1.Equals(obj2)))
            {
                throw new Exception($"Failure while Comparing {obj1} to {obj2}");
            }
        }
    }

    public static class Tests
    {
        public static int Main()
        {
            NullableTest1.Run();
            NullableTest3.Run();

            return NullableTest1.exceptionCounter != 0 ? 1 : 0;
        }
    }
}
