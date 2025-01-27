namespace System
{
    public readonly struct Byte : IEquatable<byte>
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
    }
}
