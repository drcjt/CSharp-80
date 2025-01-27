namespace System
{
    public sealed partial class String
    {
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is not string str)
                return false;

            if (this.Length != str.Length)
                return false;

            // Compare actual characters in the strings
            return EqualsHelper(this, str);
        }

        public bool Equals(String? value)
        {
            if (ReferenceEquals(this, value))
                return true;

            if (value == null)
                return false;

            if (this.Length != value.Length)
                return false;

            return EqualsHelper(this, value);
        }

        private static unsafe bool EqualsHelper(string strA, string strB)
        {
            fixed (char* strABuffer = &strA._firstChar) fixed (char* strBBuffer = &strB._firstChar)
            {
                char* pA = strABuffer;
                char* pB = strBBuffer;

                var offset = 0;
                while (offset < strA.Length)
                {
                    if (*(pA++) != *(pB++))
                    {
                        return false;
                    }
                    offset++;
                }
            }

            return true;
        }

        public static bool operator ==(string a, string b) => a.Equals(b);
        public static bool operator !=(string a, string b) => !a.Equals(b);

        public override int GetHashCode()
        {
            // TODO: Implement hash algorithm
            return 0;
        }
    }
}
