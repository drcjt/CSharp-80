namespace System
{
    public readonly struct SByte : IEquatable<sbyte>
    {
        private readonly sbyte m_value;
        
        public const sbyte MaxValue = (sbyte)0x7F;
        public const sbyte MinValue = unchecked((sbyte)0x80);

        public override int GetHashCode() => m_value;

        public override bool Equals(object? obj)
        {
            if (obj is not sbyte)
                return false;
            return m_value == ((sbyte)obj).m_value;
        }

        public bool Equals(sbyte obj) => m_value == obj;
    }
}
