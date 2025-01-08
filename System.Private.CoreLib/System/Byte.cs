namespace System
{
    public struct Byte
    {
        private readonly byte m_value;

        public override bool Equals(object? obj)
        {
            if (obj is not byte)
                return false;
            return m_value == ((byte)obj).m_value;
        }

        public override int GetHashCode() => m_value;
    }
}
