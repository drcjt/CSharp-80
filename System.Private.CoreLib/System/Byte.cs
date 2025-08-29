namespace System
{
    public readonly struct Byte
        : IComparable,
          IEquatable<byte>,
          IComparable<byte>

    {
        private readonly byte m_value;

        public const byte MaxValue = (byte)0xFF;
        public const byte MinValue = 0;

        public override int GetHashCode() => m_value;

        public override bool Equals(object? obj)
        {
            if (obj is not byte)
                return false;
            return m_value == ((byte)obj).m_value;
        }

        public bool Equals(byte obj) => m_value == obj;

        public int CompareTo(object? obj)
        {
            if (obj is null)
            {
                return 1;
            }
            if (obj is byte b)
            {
                return m_value - b.m_value;
            }
            throw new ArgumentException();
        }

        public int CompareTo(byte value) => m_value - value;
    }
}
