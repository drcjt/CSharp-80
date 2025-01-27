namespace System
{
    public readonly struct UInt32 : IEquatable<uint>
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
    }
}
