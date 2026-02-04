namespace System
{
    public struct Nullable<T> where T : struct
    {
        public readonly bool HasValue { get; }
        internal T value;

        public Nullable(T value)
        {
            this.value = value;
            HasValue = true;
        }

        public readonly T Value => value;

        public override bool Equals(object? obj)
        {
            if (!HasValue) return obj == null;
            if (obj == null) return false;
            return Value.Equals(obj);
        }

        public readonly T GetValueOrDefault() => value;
        public readonly T GetValueOrDefault(T defaultValue) => HasValue ? value : defaultValue;
        public override int GetHashCode()
        {
            if (HasValue)
            {
                return value.GetHashCode();
            }
            return 0;

            // TODO: This doesn't work properly
            //return HasValue ? Value.GetHashCode() : 0;
        }

        public override string ToString()
        { 
            if (HasValue)
            {
                return value.ToString();
            }
            return "";
        }

        public static explicit operator T(Nullable<T> value) => value!.Value;
        public static explicit operator T?(T value) => new T?(value);
    }
}
