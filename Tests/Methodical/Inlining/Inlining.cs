using System.Runtime.CompilerServices;

namespace Inlining
{
    public static class Test
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InlineMethod2()
        {
            _result = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InlineMethod()
        {
            InlineMethod2();
        }

        private static int _result = 1;

        public static int Main()
        {
            InlineMethod();

            return _result;
        }
    }
}