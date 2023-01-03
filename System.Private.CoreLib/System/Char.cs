namespace System
{
    public struct Char
    {
        public static bool IsBetween(char c, char minInclusive, char maxInclusive) => (uint)(c - minInclusive) <= (uint)(maxInclusive - minInclusive);

        public static bool IsAsciiDigit(char c) => IsBetween(c, '0', '9');
    }
}
