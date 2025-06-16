using System.Runtime.CompilerServices;

namespace Inlining
{
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
            Measure.a = 8;
        }
    }

    public static class Program
    {
        public static int Main()
        {
            byte b = 0x0f;

            if (Measure.a != 0xCC)
            {
                return 1;
            }

            // Check when method is inlined that cctor is still called
            TestClass.f(ref b);

            if (Measure.a != 8)
            {
                return 1;
            }

            return 0;
        }
    }
}