using Internal.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Memory.Tests
{
    public static partial class SpanTests
    {
        public static void CtorPointerInt()
        {
            unsafe
            {
                int[] a = { 90, 91, 92 };
                fixed (int* pa = a)
                {
                    Span<int> span = new Span<int>(pa, 3);
                    span.Validate(90, 91, 92);
                    Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef<int>(pa), ref MemoryMarshal.GetReference(span)));
                }
            }
        }
    }
}
