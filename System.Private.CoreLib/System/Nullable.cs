namespace System
{
    public struct Nullable<T> where T : struct
    {
        public readonly bool HasValue { get; }
        public readonly T Value { get; }

        public Nullable(T value)
        {
            Value = value;
            HasValue = true;
        }

        public override bool Equals(object? obj)
        {
            if (!HasValue) return obj == null;
            if (obj == null) return false;
            return Value.Equals(obj);
        }

        public readonly T GetValueOrDefault() => Value;
        public readonly T GetValueOrDefault(T defaultValue) => HasValue ? Value : defaultValue;
        public override int GetHashCode()
        {
            if (HasValue)
            {
                return Value.GetHashCode();
            }
            return 0;

            // TODO: This doesn't work properly
            //return HasValue ? Value.GetHashCode() : 0;
        }

        public override string ToString()
        { 
            if (HasValue)
            {
                return Value.ToString();
            }
            return "";
        }

        public static explicit operator T(Nullable<T> value) => value!.Value;
        public static explicit operator T?(T value) => new T?(value);
    }
}
