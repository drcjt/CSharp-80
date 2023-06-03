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

        [Intrinsic]
        public static EETypePtr EETypePtrOf<T>()
        {
            throw new Exception();
        }
    }
}
