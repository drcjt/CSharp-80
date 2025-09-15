using System.Runtime;
using System.Runtime.CompilerServices;
using Internal.Runtime.CompilerServices;

namespace System
{
    public static class Buffer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Memmove<T>(ref T destination, ref T source, nuint elementCount)
        {
            // Since we don't have any GC it's safe to just use SpanHelpers.Memmove
            SpanHelpers.Memmove(
                ref Unsafe.As<T, byte>(ref destination), 
                ref Unsafe.As<T, byte>(ref source), 
                elementCount * (nuint)Unsafe.SizeOf<T>());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void MemoryCopy(void* source, void* destination, nuint sourceBytesToCopy)
        {
            Memmove(ref *(byte*)destination, ref *(byte*)source, sourceBytesToCopy);
        }
    }
}
