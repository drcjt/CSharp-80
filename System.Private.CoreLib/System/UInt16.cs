namespace System
{
    public struct UInt16
    {
        private readonly ushort m_value;

        public override bool Equals(object? obj)
        {
            if (obj is not ushort)
                return false;
            return m_value == ((ushort)obj).m_value;
        }
    }
}
