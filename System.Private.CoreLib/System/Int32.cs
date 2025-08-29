namespace System
{
    public readonly struct Int32
        : IComparable,
          IEquatable<int>,
          IComparable<int>
    {
        private readonly int m_value;

        public const int MaxValue = 2147483647;
        public const int MinValue = -2147483648;

        public static int Parse(string s)
        {
            int result = 0;
            for (int i = 0; i < s.Length; i++) 
            { 
                char c = s[i];
                if (char.IsAsciiDigit(c))
                {
                    result = result * 10 + (c - '0');
                }
            }

            return result;
        }

        public override string ToString()
        {
            return Number.Int32ToDecStr(m_value);
        }

        public override int GetHashCode() => m_value;

        public override bool Equals(object? obj)
        {
            if (obj is not int)
                return false;
            return m_value == ((int)obj).m_value;
        }

        public bool Equals(int obj)
        {
            return m_value == obj;
        }

        public int CompareTo(object? obj)
        {
            if (obj is null)
            {
                return 1;
            }

            if (obj is int i)
            {
                return CompareTo(i);
            }

            throw new ArgumentException();
        }

        public int CompareTo(int other)
        {
            if (m_value < other) return -1;
            if (m_value > other) return 1;
            return 0;
        }
    }
}
