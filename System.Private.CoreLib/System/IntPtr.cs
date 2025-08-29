namespace System
{
    public readonly struct IntPtr
        : IComparable,
          IEquatable<nint>,
          IComparable<nint>
    {
        private readonly nint _value;

        public static nint MaxValue => 32767;
        public static nint MinValue => -32768;

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
        public override string ToString() => ((int)_value).ToString();

        public int CompareTo(object? obj)
        {
            if (obj is nint other)
            {
                return CompareTo(other);
            }
            else if (obj is null)
            {
                return 1;
            }

            throw new ArgumentException();
        }

        public int CompareTo(nint value)
        {
            if (_value < value) return -1;
            if (_value > value) return 1;
            return 0;
        }
    }
}
