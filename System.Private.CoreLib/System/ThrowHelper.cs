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

        internal static void ThrowInvalidOperationException_InvalidOperation_NoValue()
        {
            throw new InvalidOperationException();
        }

        internal static void ThrowArgumentException_DestinationTooShort()
        {
            throw new ArgumentException("DestinationTooShort");
        }

        internal static void ThrowArgumentOutOfRangeException()
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}
