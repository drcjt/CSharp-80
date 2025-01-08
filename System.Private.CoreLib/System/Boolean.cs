namespace System
{
    public struct Boolean
    {
        private readonly bool m_value;

        public override bool Equals(object? obj)
        {
            if (obj is not bool)
                return false;
            return m_value == ((bool)obj).m_value;
        }
    }
}
