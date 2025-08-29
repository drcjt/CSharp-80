namespace System
{
    public readonly struct UInt32
        : IComparable,
          IEquatable<uint>,
          IComparable<uint>
    {
        private readonly uint m_value;

        public const uint MaxValue = 0xffffffff;
        public const uint MinValue = 0U;

        public override int GetHashCode() => (int)m_value;

        public override bool Equals(object? obj)
        {
            if (obj is not uint)
                return false;
            return m_value == ((uint)obj).m_value;
        }

        public bool Equals(uint obj)
        {
            return m_value == obj;
        }

        public int CompareTo(object? obj)
        {
            if (obj is null)
            {
                return 1;
            }
            if (obj is uint otherValue)
            {
                return CompareTo(otherValue);
            }
            throw new ArgumentException();
        }

        public int CompareTo(uint value)
        {
            if (m_value < value) return -1;
            if (m_value > value) return 1;
            return 0;
        }
    }
}
