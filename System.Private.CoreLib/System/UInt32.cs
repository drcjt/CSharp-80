namespace System
{
    public struct UInt32
    {
        private readonly uint m_value;

        public override bool Equals(object? obj)
        {
            if (obj is not uint)
                return false;
            return m_value == ((uint)obj).m_value;
        }
    }
}
