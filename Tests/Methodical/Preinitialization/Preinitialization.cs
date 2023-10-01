using System;
using System.Runtime.CompilerServices;

namespace Preinitialization
{
    public static class Test
    {
        public static int Main()
        {
            TestConstants.Run();
            TestComplexConstructor.Run();

            return 0;
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
            Assert.AreEquals(25, _value);
        }
    }

    public class TestConstants
    {
        private static bool _b = true;
        private static int _s = 3;

        public static void Run()
        {
            Assert.IsPreinitialized<TestConstants>();
            Assert.AreEquals(true, _b);
            Assert.AreEquals(3, _s);
        }
    }

    public class Assert
    {
        public static void IsPreinitialized<T>()
        {
            if (RuntimeHelpers.HasCctor<T>())
            {
                Environment.Exit(1);
            }
        }

        public static void IsLazyInitialized<T>()
        {
            if (!RuntimeHelpers.HasCctor<T>())
            {
                Environment.Exit(1);
            }
        }

        public static void AreEquals(bool expected, bool actual)
        {
            if (expected != actual)
            {
                Environment.Exit(1);
            }
        }

        public static void AreEquals(int expected, int actual)
        {
            if (expected != actual)
            {
                Environment.Exit(1);
            }
        }
    }
}