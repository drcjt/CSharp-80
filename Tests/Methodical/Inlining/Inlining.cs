using System.Runtime.CompilerServices;

namespace Inlining
{
    public static class Test
    {
        private static int _parameter = 0;
        
        // This is not initialized as this causes all od the static method
        // calls to be imported as comma nodes to call the cctor before
        // the method. 
        private static string _str;

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

        private static int _result = 1;

        public static int Main()
        {
            InlineMethod();

            string strParameter = "Test";
            InlineMethodWithParameters(1, strParameter);

            if (_parameter != 2) return 2;
            if (!ReferenceEquals(_str, strParameter)) return 3;

            return _result;
        }
    }
}