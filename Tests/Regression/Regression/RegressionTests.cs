﻿namespace Regression
{
    public static class Regression
    {
        public static int Main()
        {
            Bug87();

            nuint testValue = 123;
            Assert.AreEqual(testValue, MethodCall_WithNuintParameter_CompilesWithoutErrors(testValue));

            Assert.AreEqual(1, Bug210_SpillStack());

            Assert.AreEqual(0, Bug206(0));

            Assert.AreEqual(2, new SpillImportAppendTests().SpillOnStFldImport());

            Assert.AreEqual(10, Bug545Method<int>().Length);

            return 0;
        }

        public unsafe static nuint Bug206(int count)
        {
            byte* b = stackalloc byte[count];
            return ((nuint)b);
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

        private static int Bug210_SpillStack()
        {
            int x = 0;
            int y = 1;
            x += y == 1 ? 1 : 0;

            return x;
        }

        private static nuint MethodCall_WithNuintParameter_CompilesWithoutErrors(nuint n)
        {
            return n;
        }

        public static T[] Bug545Method<T>()
        {
            var t = new Bug545Test<T>();
            return t.ToArray();
        }

    }

    public class Bug545Test<T>()
    {
        public T[] ToArray()
        {
            var array = new T[10];
            return array;
        }
    }
}