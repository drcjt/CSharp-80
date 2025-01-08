namespace System
{
    public struct SByte
    {
        private readonly sbyte m_value;

        public override bool Equals(object? obj)
        {
            if (obj is not sbyte)
                return false;
            return m_value == ((sbyte)obj).m_value;
        }
    }
}
