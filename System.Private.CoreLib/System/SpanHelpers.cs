using Internal.Runtime.CompilerServices;

namespace System
{
    internal static class SpanHelpers
    {
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
