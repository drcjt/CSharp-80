using System.Runtime.CompilerServices;

namespace System
{
    public struct EETypePtr
    {
        private IntPtr _value;

        internal EETypePtr(IntPtr value)
        {
            _value = value;
        }

        /* TODO : Need to implement this
        [Intrinsic]
        internal static EETypePtr EETypePtrOf<T>()
        {
            throw new Exception();
        }
        */
    }
}
