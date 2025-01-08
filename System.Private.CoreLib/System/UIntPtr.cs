namespace System
{
    public struct UIntPtr
    {
        private readonly nuint _value;

        public unsafe UIntPtr(void* value)
        {
            _value = (nuint)value;
        }

        public static unsafe explicit operator UIntPtr(void* value) => new(value);
        public static unsafe explicit operator void*(UIntPtr value) => (void*)value._value;

        public static unsafe bool operator ==(UIntPtr value1, UIntPtr value2) => value1._value == value2._value;
        public static unsafe bool operator !=(UIntPtr value1, UIntPtr value2) => value1._value != value2._value;

        public override bool Equals(object? obj) => (obj is nuint other) && Equals(other);
        public bool Equals(nuint other) => _value == other;

        public override unsafe int GetHashCode() => (int)_value;
    }
}
