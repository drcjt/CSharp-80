namespace System
{
    public readonly struct Char
        : IComparable,
          IEquatable<char>,
          IComparable<char>
    {
        private readonly char m_value;

        public const char MaxValue = (char)0xFFFF;
        public const char MinValue = (char)0x00;

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

        public int CompareTo(object? obj)
        {
            if (obj is null)
            {
                return 1;
            }
            if (obj is char c)
            {
                return m_value - c.m_value;
            }
            throw new ArgumentException();
        }

        public int CompareTo(char value) => m_value - value;
    }
}
