namespace System
{
    public readonly struct Int16
        : IComparable,
          IEquatable<short>,
          IComparable<short>
    {
        private readonly short m_value;

        public const short MaxValue = (short)0x7FFF;
        public const short MinValue = unchecked((short)0x8000);

        public override int GetHashCode() => m_value;

        public override bool Equals(object? obj)
        {
            if (obj is not short)
                return false;

            return m_value == ((short)obj).m_value;
        }

        public bool Equals(short obj)
        {
            return m_value == obj;
        }

        public int CompareTo(object? obj)
        {
            if (obj is null)
            {
                return 1;
            }

            if (obj is short otherValue)
            {
                return m_value - otherValue.m_value;
            }

            throw new ArgumentException();
        }

        public int CompareTo(short value)
        {
            return m_value - value;
        }
    }
}
