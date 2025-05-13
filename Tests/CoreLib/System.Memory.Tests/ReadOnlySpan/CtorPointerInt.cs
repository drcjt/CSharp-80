using Internal.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Memory.Tests
{
    public static partial class ReadOnlySpanTests
    {
        public static void CtorPointerInt()
        {
            unsafe
            {
                int[] a = { 90, 91, 92 };
                fixed (int* pa = a)
                {
                    ReadOnlySpan<int> span = new ReadOnlySpan<int>(pa, 3);
                    span.Validate(90, 91, 92);
                    Assert.True(Unsafe.AreSame(ref Unsafe.AsRef<int>(pa), ref Unsafe.AsRef(in MemoryMarshal.GetReference(span))));
                }
            }
        }
    }
}
