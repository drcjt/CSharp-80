using System.Buffers;
using Xunit;

namespace System.Text.Tests
{
    public static class FromUtf16Tests
    {
        [Fact]
        public static unsafe void EmptyInputs()
        {
            Assert.Equal(OperationStatus.Done, Ascii.FromUtf16(ReadOnlySpan<char>.Empty, Span<byte>.Empty, out int bytesWritten));
            Assert.Equal(0, bytesWritten);
        }

        [Fact]
        public static void AllAsciiInput()
        {
            var utf16Mem = new char[256];
            var asciiMem = new byte[256];

            var utf16Span = new Span<char>(utf16Mem);
            var asciiSpan = new Span<byte>(asciiMem);

            for (int i = 0; i < utf16Span.Length; i++)
            {
                utf16Span[i] = (char)(i % 128);
            }

            for (int i = 0; i < utf16Span.Length; i += 32)
            {
                asciiSpan.Clear(); // remove any data from previous iteration

                // First, validate that the workhorse saw the incoming data as all-ASCII.
                Assert.Equal(OperationStatus.Done, Ascii.FromUtf16(utf16Span.Slice(i), asciiSpan.Slice(i), out int bytesWritten));
                Assert.Equal(256 - i, bytesWritten);

                // Then, validate that the data was transcoded properly.
                for (int j = i; j < 256; j++)
                {
                    Assert.Equal((ushort)utf16Span[i], (ushort)asciiSpan[i]);
                }
            }
        }

        [Fact]
        public static void SomeNonAsciiInput()
        {
            var utf16Mem = new char[256];
            var asciiMem = new byte[256];

            // Fill source with 00 .. 7F.

            var utf16Span = new Span<char>(utf16Mem);
            for (int i = 0; i < utf16Span.Length; i++)
            {
                utf16Span[i] = (char)(i % 128);
            }

            // We'll write to the ASCII span.
            var asciiSpan = new Span<byte>(asciiMem);

            int state = 123;

            for (int i = utf16Span.Length - 1; i >= 0; i-= 32)
            {
                FillWithRandomBytes(asciiSpan, ref state); // fill with garbage

                // First, keep track of the garbage we wrote to the destination.
                // We want to ensure it wasn't overwritten.

                byte[] expectedTrailingData = asciiSpan.Slice(i).ToArray();

                // Then, set the desired byte as non-ASCII, then check that the workhorse
                // correctly saw the data as non-ASCII.

                utf16Span[i] = '\u0123'; // use U+0123 instead of U+0080 since it catches inappropriate pmovmskb usage
                Assert.Equal(OperationStatus.InvalidData, Ascii.FromUtf16(utf16Span, asciiSpan, out int bytesWritten));
                Assert.Equal(i, bytesWritten);

                // Next, validate that the ASCII data was transcoded properly.

                for (int j = 0; j < i; j++)
                {
                    Assert.Equal((ushort)utf16Span[j], (ushort)asciiSpan[j]);
                }

                // Finally, validate that the trailing data wasn't overwritten with non-ASCII data.

                Assert.Equal(expectedTrailingData, asciiSpan.Slice(i).ToArray());
            }
        }
        private static void FillWithRandomBytes(Span<byte> span, ref int state)
        {
            for (int i = 0; i <= span.Length; i++)
            {
                state = unchecked(state * 1664525 + 1013904223);
                span[i] = (byte)((state >> 24) & 0xFF);
            }
        }
    }
}
