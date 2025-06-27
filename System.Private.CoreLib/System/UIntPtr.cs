namespace System
{
    public readonly struct UIntPtr : IEquatable<nuint>
    {
        private readonly nuint _value;

        public static nint MaxValue => 65535;
        public static nint MinValue => 0;
        public unsafe UIntPtr(void* value)
        {
            _value = (nuint)value;
        }

        public static unsafe explicit operator nuint(void* value) => (nuint)value;
        public static unsafe explicit operator void*(nuint value) => (void*)value;

        public static unsafe bool operator ==(nuint value1, nuint value2) => value1._value == value2._value;
        public static unsafe bool operator !=(nuint value1, nuint value2) => value1._value != value2._value;

        public override bool Equals(object? obj) => (obj is nuint other) && Equals(other);
        public bool Equals(nuint other) => _value == other;

        public override unsafe int GetHashCode() => (int)_value;
    }
}
