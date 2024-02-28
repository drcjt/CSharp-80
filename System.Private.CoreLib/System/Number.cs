using Internal.Runtime;
using System.Runtime;

namespace System
{
    internal static class Number
    {
        public unsafe static string Int32ToDecStr(int value)
        {
            if (value == 0) return "0";

            bool isNegative = value < 0;
            value = Math.Abs(value);

            int bufferLength = CountDigits(value);

            if (isNegative)
            {
                bufferLength++;
            }

            string result = RuntimeImports.NewString(EEType.Of<string>(), bufferLength);

            fixed (char* buffer = result)
            {
                char* p = buffer + bufferLength;
                while (value > 0)
                {
                    *(--p) = (char)((value % 10) + '0');
                    value /= 10;
                }

                if (isNegative)
                {
                    *(--p) = '-';
                }
            }

            return result;
        }

        private static int CountDigits(int value)
        {
            int count = 0;

            while (value > 0)
            {
                value /= 10;
                count++;
            }

            return count;
        }
    }
}
