using Internal.Runtime;
using System.Runtime;

namespace System
{
    internal static class Number
    {
        public unsafe static string Int32ToDecStr(int value) 
            => value >= 0 ? 
                UInt32ToDecStr((uint)value) : 
                NegativeInt32ToDecStr(value);

        private static unsafe string UInt32ToDecStr(uint value)
        {
            if (value == 0)
                return "0";

            nuint bufferLength = CountDigits(value);

            string result = RuntimeImports.NewString(EEType.Of<string>(), bufferLength);
            fixed (char* buffer = result)
            {
                char* p = buffer + (int)(nint)bufferLength;
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
            nuint bufferLength = CountDigits((uint)-value) + 1;
            string result = RuntimeImports.NewString(EEType.Of<string>(), bufferLength);

            fixed (char* buffer = result)
            {
                char* p = buffer + (int)(nint)bufferLength;
                p = UInt32ToDecStr(p, (uint)-value);

                *(p-1) = '-';
            }

            return result;
        }

        private static nuint CountDigits(uint value)
        {
            nuint count = 0;

            while (value > 0)
            {
                value /= 10;
                count++;
            }

            return count;
        }
    }
}
