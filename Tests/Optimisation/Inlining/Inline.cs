using System;
using System.Runtime.CompilerServices;

namespace Inlining
{
    public static class Inline
    {
        private static int s_c = 1;

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

        public static int Main()
        {
            try
            {
                Console.WriteLine(FooInline(s_c));  // 16
                Console.WriteLine(FooInline(s_c) + 1 + FooInline(s_c));     // 16 + 1 + 16 = 33
                Console.WriteLine(FooInline(s_c) + 1 + FooInline(s_c) + 2 + FooInline(s_c)); 
                Console.WriteLine("Test Passed");
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Test Failed: {e.Message}");
                // Return a non-zero value to indicate failure
                return 1;
            }
        }
    }
}