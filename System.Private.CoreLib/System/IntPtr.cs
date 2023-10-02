namespace System
{
    public readonly struct IntPtr
    {
        private readonly unsafe void* _value;

        public unsafe IntPtr(void* value)
        {
            _value = value;
        }

        public static unsafe explicit operator IntPtr(void* value) => new(value);
        public static unsafe explicit operator void*(IntPtr value) => value._value;

        public static unsafe bool operator ==(IntPtr value1, IntPtr value2) => value1._value == value2._value;
        public static unsafe bool operator !=(IntPtr value1, IntPtr value2) => value1._value != value2._value;

        public override bool Equals(object obj) => (obj is nint other) && Equals(other);
        public override unsafe int GetHashCode() => (int)_value;
    }
}