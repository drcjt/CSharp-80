namespace System
{
    public struct Boolean
    {
        private readonly bool m_value;

        internal const int True = 1;
        internal const int False = 0;

        public override bool Equals(object? obj)
        {
            if (obj is not bool)
                return false;
            return m_value == ((bool)obj).m_value;
        }

        public override int GetHashCode()
        {
            return m_value ? True : False;
        }
    }
}
