using Internal.Runtime.CompilerServices;
using System.Runtime.CompilerServices;

namespace System
{
    internal class Buffer
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
        private static void Memmove(ref byte dest, ref byte src, nuint len)
        {
            _Memmove(ref dest, ref src, len);
        }

        private static unsafe void _Memmove(ref byte dest, ref byte src, nuint len)
        {
            fixed (byte* pDest = &dest)
            fixed (byte* pSrc = &src)
            {
                __Memmove(pDest, pSrc, len);
            }
        }

        internal static unsafe void __Memmove(byte* dest, byte* src, nuint len)
        {
            for (nuint i = 0; i < len; i++)
            {
                *dest = *src;
                dest++;
                src++;
            }
        }

        internal static unsafe void Memmove(byte* dest, byte* src, int len)
        {
            for (int i = 0; i < len; i++) 
            {
                *dest = *src;
                dest++;
                src++;
            }
        }
    }
}
