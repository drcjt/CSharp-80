using Xunit;

namespace System.Memory.Tests
{
    public static partial class ReadOnlySpanTests
    {
        [Fact]
        public static void ToArray1()
        {
            int[] a = { 91, 92, 93 };
            var span = new ReadOnlySpan<int>(a);
            int[] copy = span.ToArray();
            Assert.Equal<int>(a, copy);
            Assert.NotSame(a, copy);
        }
    }
}
