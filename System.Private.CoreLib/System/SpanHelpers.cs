using System.Runtime.CompilerServices;
using Internal.Runtime.CompilerServices;

namespace System
{
    internal static class SpanHelpers
    {
        public static unsafe void Fill<T>(ref T refData, nuint length, T value)
        {
            if (length == 0)
                return;

            for (nuint i = 0; i < length; i++)
            {
                Unsafe.Add(ref refData, (nint)i) = value;
            }
        }

        [Intrinsic]
        internal static unsafe void Memmove(ref byte dest, ref byte src, nuint len)
        {
            if ((nuint)Unsafe.ByteOffset(ref src, ref dest) >= len)
            {
                for (nuint i = 0; i < len; i++)
                {
                    Unsafe.Add(ref dest, (nint)i) = Unsafe.Add(ref src, (nint)i);
                }
            }
            else
            {
                for (nuint i = len; i > 0; i--)
                {
                    Unsafe.Add(ref dest, (nint)(i - 1)) = Unsafe.Add(ref src, (nint)(i - 1));
                }
            }
        }

        public static bool SequenceEqual<T>(ref T first, ref T second, int length) where T : IEquatable<T>
        {
            int index = 0;
            T lookUp0;
            T lookUp1;

            while (length > 0)
            {
                lookUp0 = Unsafe.Add(ref first, index);
                lookUp1 = Unsafe.Add(ref second, index);
                if (!lookUp0.Equals(lookUp1))
                    return false;
                index += 1;
                length--;
            }

            return true;
        }
    }
}
