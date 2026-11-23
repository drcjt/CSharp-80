using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Text
{
    public static partial class Ascii
    {

        /// <summary>
        /// Copies text from a source buffer to a destination buffer, converting
        /// from UTF-16 to ASCII during the copy.
        /// </summary>
        /// <param name="source">The source buffer from which UTF-16 text is read.</param>
        /// <param name="destination">The destination buffer to which ASCII text is written.</param>
        /// <param name="bytesWritten">The number of bytes actually written to <paramref name="destination"/>. It's the same as the number of chars actually read from <paramref name="source"/>.</param>
        /// <returns>An <see cref="OperationStatus"/> describing the result of the operation.</returns>
        public static unsafe OperationStatus FromUtf16(ReadOnlySpan<char> source, Span<byte> destination, out int bytesWritten)
        {
            int numElementsToConvert;
            OperationStatus statusToReturnOnSuccess;

            if (source.Length <= destination.Length)
            {
                numElementsToConvert = source.Length;
                statusToReturnOnSuccess = OperationStatus.Done;
            }
            else
            {
                numElementsToConvert = destination.Length;
                statusToReturnOnSuccess = OperationStatus.DestinationTooSmall;
            }

            fixed (char* pSource = &MemoryMarshal.GetReference(source))
            fixed (byte* pDestination = &MemoryMarshal.GetReference(destination))
            {
                int numElementsActuallyConverted = NarrowUtf16ToAscii(pSource, pDestination, numElementsToConvert);
                Debug.Assert(numElementsActuallyConverted <= numElementsToConvert);

                bytesWritten = numElementsActuallyConverted;
                return (numElementsToConvert == numElementsActuallyConverted) ? statusToReturnOnSuccess : OperationStatus.InvalidData;
            }
        }
    }
}
