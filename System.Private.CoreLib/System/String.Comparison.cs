namespace System
{
    public sealed partial class String
    {
        public override bool Equals(object other)
        {
            if (this == other)
                return true;

            if (other is not string str)
                return false;

            if (this.Length != str.Length)
                return false;

            // Compare actual characters in the strings
            return EqualsHelper(this, str);
        }

        private unsafe bool EqualsHelper(string strA, string strB)
        {
            fixed (char* strABuffer = strA)
            {
                fixed (char* strBBuffer = strB)
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
            }

            return true;
        }
    }
}
