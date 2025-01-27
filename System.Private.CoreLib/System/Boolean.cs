namespace System
{
    public readonly struct Boolean : IEquatable<bool>
    {
        private readonly bool m_value;

        internal const int True = 1;
        internal const int False = 0;

        public override int GetHashCode() => m_value ? True : False;

        public override bool Equals(object? obj)
        {
            if (obj is not bool)
                return false;
            return m_value == ((bool)obj).m_value;
        }

        public bool Equals(bool obj)
        {
            return m_value == obj;
        }
    }
}
