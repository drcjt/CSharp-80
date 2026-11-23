namespace System.Text
{
    public static partial class Ascii
    {
        /// <summary>
        /// Copies as many ASCII characters (U+0000..U+007F) as possible from <paramref name="pUtf16Buffer"/>
        /// to <paramref name="pAsciiBuffer"/>, stopping when the first non-ASCII character is encountered
        /// or once <paramref name="elementCount"/> elements have been converted. Returns the total number
        /// of elements that were able to be converted.
        /// </summary>
        internal static unsafe int NarrowUtf16ToAscii(char* pUtf16Buffer, byte* pAsciiBuffer, int elementCount)
        {
            int currentOffset = 0;

            do
            {
                uint utf16Data32BitsHigh = pUtf16Buffer[currentOffset];
                if (utf16Data32BitsHigh > 0x007Fu)
                {
                    break;
                }
                pAsciiBuffer[currentOffset] = (byte)utf16Data32BitsHigh;
                currentOffset++;
            } while (currentOffset < elementCount);

            return currentOffset;
        }
    }
}
