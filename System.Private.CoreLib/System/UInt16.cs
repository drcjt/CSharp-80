namespace System
{
    public readonly struct UInt16 : IEquatable<ushort>
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
    }
}
