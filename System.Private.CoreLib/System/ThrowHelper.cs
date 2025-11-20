namespace System
{
    internal static class ThrowHelper
    {
        internal static void ThrowIndexOutOfRangeException()
        {
            throw new IndexOutOfRangeException();
        }

        internal static void ThrowNotSupportedException()
        {
            throw new NotSupportedException();
        }
    }
}
