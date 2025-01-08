namespace System
{
    public struct Int16 
    {
        private readonly short m_value;

        public override bool Equals(object? obj)
        {
            if (obj is not short)
                return false;
            return m_value == ((short)obj).m_value;
        }

        public override int GetHashCode() => m_value;
    }
}
