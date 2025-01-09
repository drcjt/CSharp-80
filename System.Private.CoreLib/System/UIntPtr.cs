﻿namespace System
{
    public struct UIntPtr
    {
        private readonly nuint _value;

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
