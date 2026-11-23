using Xunit;

namespace System.Memory.Tests
{
    public static partial class ReadOnlySpanTests
    {
        [Fact]
        public static void CopyToSingleArray()
        {
            ReadOnlySpan<int> src = new int[1] { 1 };
            Span<int> dst = new int[1] { 99 };

            src.CopyTo(dst);

            Assert.Equal<int>(src, dst.ToArray());
        }

        [Fact]
        public static void CopyToArray()
        {
            ReadOnlySpan<int> src = new int[3] { 1, 2, 3 };
            Span<int> dst = new int[3] { 99, 100, 101 };

            src.CopyTo(dst);

            Assert.Equal<int>(src, dst.ToArray());
        }
    }
}
