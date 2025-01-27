namespace System
{
    public readonly struct Char : IEquatable<char>
    {
        private readonly char m_value;

        public static bool IsBetween(char c, char minInclusive, char maxInclusive) => (uint)(c - minInclusive) <= (uint)(maxInclusive - minInclusive);

        public static bool IsAsciiDigit(char c) => IsBetween(c, '0', '9');

        public static char ToLower(char c) => (char)(c | 32);
        public static char ToUpper(char c) => (char)(c & ~32);

        public override int GetHashCode() => (int)m_value | ((int)m_value << 16);

        public override bool Equals(object? obj)
        {
            if (obj is not char)
                return false;
            return m_value == ((char)obj).m_value;
        }


        public bool Equals(char obj)
        {
            return m_value == obj;
        }
    }
}
