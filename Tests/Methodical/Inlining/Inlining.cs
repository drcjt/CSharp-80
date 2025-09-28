using System.Runtime.CompilerServices;

namespace Inlining
{
    public static class Test
    {
        private static int _parameter = 0;        
        private static string _str = "Foo";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InlineMethodWithParameters(int i, string s)
        {
            int j = i + 1;
            _parameter = j;
            _str = s;
        }

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

        private static int _unused = 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InlineMethodWithMoreThanOneBasicBlock(int n)
        {
            if (n < 0)
            {
                _unused = 1;
            }
        }

        public static void InlineMethodAtEndOfBasicBlock()
        {
            if (_result != 1)
            {
                InlineMethodWithMoreThanOneBasicBlock(42);
            }
        }

        private static int _result = 1;

        public static int Main()
        {
            InlineMethod();

            string strParameter = "Test";
            InlineMethodWithParameters(1, strParameter);

            if (_parameter != 2) return 2;
            if (!ReferenceEquals(_str, strParameter)) return 3;

            InlineMethodAtEndOfBasicBlock();

            return _result;
        }
    }
}
