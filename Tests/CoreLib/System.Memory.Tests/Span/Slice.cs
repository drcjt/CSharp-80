using Internal.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Memory.Tests
{
    public static partial class SpanTests
    {
        [Fact]
        public static void SliceInt()
        {
            int[] a = { 90, 91, 92, 93, 94, 95, 96, 97, 98, 99 };
            Span<int> span = new Span<int>(a).Slice(6);
            Assert.Equal(4, span.Length);
            Assert.True(Unsafe.AreSame(ref a[6], ref MemoryMarshal.GetReference(span)));
        }

        [Fact]
        public static void SliceIntRangeChecksd()
        {
            int[] a = { 90, 91, 92, 93, 94, 95, 96, 97, 98, 99 };

            int firstValue = 0;
            bool result = false;
            try
            {
                Span<int> slice = new Span<int>(a).Slice(a.Length + 1);
                firstValue = slice[0];
            }
            catch (ArgumentOutOfRangeException)
            {
                result = true;
            }
            Assert.True(result);
        }
    }
}
