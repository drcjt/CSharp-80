using System;

namespace Internal.Runtime.CompilerHelpers
{
    internal static class ThrowHelpers
    {
        private static void ThrowIndexOutOfRangeException()
        {
            throw new IndexOutOfRangeException();
        }

        private static void ThrowNullReferenceException()
        {
            throw new NullReferenceException();
        }
    }
}
