using System;

namespace GenericConstrainedCall
{
    internal static class TestRunner
    {
        public static int Main()
        {
            var myCounter = new MyCounter<MyInt>(new MyInt());
            myCounter.Increment();
            if (myCounter.Value != 100)
            {
                Console.WriteLine("FAILED 1");
                Console.WriteLine($"Expected: 100, Actual: {myCounter.Value}");

                return 1;
            }

            myCounter.Decrement();
            if (myCounter.Value != 0)
            {
                Console.WriteLine("FAILED 2");
                Console.WriteLine($"Expected: 100, Actual: {myCounter.Value}");

                return 2;
            }

            var myArrayCounter = new MyArrayCounter<MyInt>(new MyInt[1]);
            myArrayCounter.Increment(0, new MyInt());
            if (myArrayCounter.Value(0) != 100)
            {
                Console.WriteLine("FAILED 3");
                Console.WriteLine($"Expected: 100, Actual: {myCounter.Value}");

                return 3;
            }

            myArrayCounter.Decrement(0);
            if (myArrayCounter.Value(0) != 0)
            {
                Console.WriteLine("FAILED 4");
                Console.WriteLine($"Expected: 100, Actual: {myCounter.Value}");

                return 4;
            }

            var mi = new MyInt();
            myCounter.Increment(mi);
            if (myCounter.Value != 100)
            {
                Console.WriteLine("FAILED 5");
                Console.WriteLine($"Expected: 100, Actual: {myCounter.Value}");

                return 5;
            }

            myCounter.Decrement(mi);
            if (myCounter.Value != 0)
            {
                Console.WriteLine("FAILED 6");
                Console.WriteLine($"Expected: 100, Actual: {myCounter.Value}");

                return 6;
            }

            Console.WriteLine("PASSED");
            return 0;
        }
    }
}
