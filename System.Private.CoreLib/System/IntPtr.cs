namespace System
{
    public readonly struct IntPtr
    {
        private readonly nint _value;

        public unsafe IntPtr(void* value)
        {
            _value = (nint)value;
        }

        public static unsafe explicit operator IntPtr(void* value) => new(value);
        public static unsafe explicit operator void*(IntPtr value) => (void*)value._value;

        public static unsafe bool operator ==(IntPtr value1, IntPtr value2) => value1._value == value2._value;
        public static unsafe bool operator !=(IntPtr value1, IntPtr value2) => value1._value != value2._value;

        public override bool Equals(object? obj) => (obj is nint other) && Equals(other);
        public bool Equals(nint other) => _value == other;

        public override unsafe int GetHashCode() => (int)_value;
    }
}