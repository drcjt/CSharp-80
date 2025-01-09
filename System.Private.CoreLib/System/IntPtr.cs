namespace System
{
    public readonly struct IntPtr
    {
        private readonly nint _value;

        public unsafe IntPtr(void* value)
        {
            _value = (nint)value;
        }

        public static unsafe explicit operator nint(void* value) => (nint)value;
        public static unsafe explicit operator void*(nint value) => (void*)value;

        public static unsafe bool operator ==(nint value1, nint value2) => value1._value == value2._value;
        public static unsafe bool operator !=(nint value1, nint value2) => value1._value != value2._value;

        public override bool Equals(object? obj) => (obj is nint other) && Equals(other);
        public bool Equals(nint other) => _value == other;

        public override unsafe int GetHashCode() => (int)_value;
    }
}