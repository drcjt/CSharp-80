namespace System
{
    public readonly struct UInt16
        : IComparable,
          IEquatable<ushort>,
          IComparable<ushort>
    {
        private readonly ushort m_value;

        public const ushort MaxValue = (ushort)0xFFFF;
        public const ushort MinValue = 0;

        public override int GetHashCode() => m_value;

        public override bool Equals(object? obj)
        {
            if (obj is not ushort)
                return false;
            return m_value == ((ushort)obj).m_value;
        }

        public bool Equals(ushort obj)
        {
            return m_value == obj;
        }

        public int CompareTo(object? obj)
        {
            if (obj is null)
            {
                return 1;
            }
            if (obj is ushort otherValue)
            {
                return (int)m_value - (int)otherValue.m_value;
            }
            throw new ArgumentException();
        }

        public int CompareTo(ushort value) => (int)m_value - (int)value;
    }
}
