using Internal.Runtime.CompilerServices;

namespace System
{
    internal class Buffer
    {
        internal static void Memmove<T>(ref T destination, ref T source, nuint elementCount)
        {
            Memmove(ref Unsafe.As<T, byte>(ref destination), ref Unsafe.As<T, byte>(ref source), elementCount * (nuint)Unsafe.SizeOf<T>());
        }

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
