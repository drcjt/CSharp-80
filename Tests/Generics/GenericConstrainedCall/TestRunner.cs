using System;
using static GenericConstrainedCall.InterfaceConstrainedCaller;

namespace GenericConstrainedCall
{
    internal static class TestRunner
    {
        public static int Main()
        {
            // Perform a constrained interface call which should be resolved
            // to a direct call at compile time.
            var foo = new MyTester<int>();
            DoConstrainedCall<MyTester<int>, int>(ref foo);

            // If the value doesn't change then box+interface call must have happened.
            if (foo.Value != 12345)
            {
                Console.WriteLine("FAILED 1");
                Console.WriteLine($"Expected: 12345, Actual: {foo.Value}");
                return 1;
            }

            // Perform a constrained virtual method call
            var bar = new ToStringOverrider();
            var result = bar.ToString();

            if (!result.Equals("Bar"))
            {
                return 1;
            }

            // Perform a constrained instance method call on System.Object method
            var eeType = ObjectInstanceMethodCaller<int>.M(123);
            var expectedEEType = 123.GetEEType();
            if (!eeType.Equals(expectedEEType))
            {
                Console.WriteLine("FAILED 2");
                Console.WriteLine($"Expected: {expectedEEType}, Actual: {eeType}");

                return 1;
            }

            var myCounter = new MyCounter<MyInt>(new MyInt());
            myCounter.Increment();
            if (myCounter.Value != 100)
            {
                Console.WriteLine("FAILED 3");
                Console.WriteLine($"Expected: 100, Actual: {myCounter.Value}");

                return 1;
            }

            myCounter.Decrement();
            if (myCounter.Value != 0)
            {
                Console.WriteLine("FAILED 4");
                Console.WriteLine($"Expected: 100, Actual: {myCounter.Value}");

                return 2;
            }

            var myArrayCounter = new MyArrayCounter<MyInt>(new MyInt[1]);
            myArrayCounter.Increment(0, new MyInt());
            if (myArrayCounter.Value(0) != 100)
            {
                Console.WriteLine("FAILED 5");
                Console.WriteLine($"Expected: 100, Actual: {myCounter.Value}");

                return 3;
            }

            myArrayCounter.Decrement(0);
            if (myArrayCounter.Value(0) != 0)
            {
                Console.WriteLine("FAILED 6");
                Console.WriteLine($"Expected: 100, Actual: {myCounter.Value}");

                return 4;
            }

            var mi = new MyInt();
            myCounter.Increment(mi);
            if (myCounter.Value != 100)
            {
                Console.WriteLine("FAILED 7");
                Console.WriteLine($"Expected: 100, Actual: {myCounter.Value}");

                return 5;
            }

            myCounter.Decrement(mi);
            if (myCounter.Value != 0)
            {
                Console.WriteLine("FAILED 8");
                Console.WriteLine($"Expected: 100, Actual: {myCounter.Value}");

                return 6;
            }

            Console.WriteLine("PASSED");
            return 0;
        }
    }
}
