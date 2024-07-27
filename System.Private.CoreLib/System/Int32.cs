namespace System
{
    public readonly struct Int32
    {
        private readonly int m_value;

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

        public override bool Equals(object obj)
        {
            return m_value == ((int)obj).m_value;
        }
    }
}
