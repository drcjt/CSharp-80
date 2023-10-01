namespace System
{
    public struct IntPtr
    {
        private readonly unsafe void* _value;

        public unsafe IntPtr(void* value)
        {
            _value = value;
        }

        public static unsafe explicit operator IntPtr(void* value) => new IntPtr(value);
        public static unsafe explicit operator void*(IntPtr value) => value._value;
    }
}
