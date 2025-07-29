using Internal.Runtime.CompilerServices;
using Xunit;

namespace Regression
{
    public static class Regression
    {
        public static int Main()
        {
            Bug87();

            nuint testValue = 123;
            Assert.Equal(testValue, MethodCall_WithNuintParameter_CompilesWithoutErrors(testValue));

            Assert.Equal(1, Bug210_SpillStack());
            Assert.Equal<nuint>(0, Bug206(0));

            Assert.Equal(2, new SpillImportAppendTests().SpillOnStFldImport());
            Assert.Equal(37, SpillImportStElemTests.Test());

            Assert.Equal(10, Bug545Method<int>().Length);

            Bug617();

            Bug660Test();

            return 0;
        }

        public unsafe static void Bug617()
        {
            byte value = 42;
            Unsafe.Add<byte>(ref value, 0);
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

        public static void Bug660Test()
        {
            int[] balls = new int[10];

            bool bouncing = true;
            while (bouncing)
            {
                for (int i = 0; i < balls.Length; i++)
                {
                    if (i == 5)
                    {
                        bouncing = false;
                        break;
                    }
                }
            }
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
