using Xunit;

namespace System.Memory.Tests
{
    public static partial class SpanTests
    {
        [Fact]
        public static void ClearEmpty()
        {
            var span = Span<string>.Empty;
            span.Clear();
        }

        [Fact]
        public static void ClearBytes()
        {
            const byte initial = 5;
            var actual = new byte[128];
            for (int i = 0; i < actual.Length; i++)
            {
                actual[i] = initial;
            }
            var expected = new byte[actual.Length];

            var span = new Span<byte>(actual);
            span.Clear();
            Assert.Equal<byte>(expected, actual);
        }
    }
}
