namespace System
{
    public struct Int32
    {
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
    }
}
