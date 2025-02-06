using Internal.Runtime;
using System.Runtime;

namespace System
{
    internal static class Number
    {
        public unsafe static string Int32ToDecStr(int value)
        {
            if (value >= 0)
                return UInt32ToDecStr((uint)value);
            else
                return NegativeInt32ToDecStr(value);
        }

        private static unsafe string UInt32ToDecStr(uint value)
        {
            if (value == 0)
                return "0";

            int bufferLength = CountDigits(value);

            string result = RuntimeImports.NewString(EEType.Of<string>(), bufferLength);
            fixed (char* buffer = result)
            {
                char* p = buffer + bufferLength;
                UInt32ToDecStr(p, value);
            }
            return result;
        }

        private static unsafe char* UInt32ToDecStr(char* bufferEnd, uint value)
        {
            while (value != 0)
            {
                *(--bufferEnd) = (char)((value % 10) + '0');
                value /= 10;
            }

            return bufferEnd;
        }
        private static unsafe string NegativeInt32ToDecStr(int value)
        {
            int bufferLength = CountDigits((uint)-value) + 1;
            string result = RuntimeImports.NewString(EEType.Of<string>(), bufferLength);

            fixed (char* buffer = result)
            {
                char* p = buffer + bufferLength;
                p = UInt32ToDecStr(p, (uint)-value);

                *(p-1) = '-';
            }

            return result;
        }

        private static int CountDigits(uint value)
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