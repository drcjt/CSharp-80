using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System
{
    public static class MemoryExtensions
    {
        public static bool SequenceEqual<T>(this Span<T> span, ReadOnlySpan<T> other) where T : IEquatable<T>
        {
            return SequenceEqual((ReadOnlySpan<T>)span, other);
        }

        public static unsafe bool SequenceEqual<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other) where T : IEquatable<T>
        {
            int length = span.Length;
            int otherLength = other.Length;

            return length == otherLength && SpanHelpers.SequenceEqual(ref MemoryMarshal.GetReference(span), ref MemoryMarshal.GetReference(other), length);
        }

        public static bool Contains<T>(this Span<T> span, T value) where T : IEquatable<T>?
        {
            return Contains((ReadOnlySpan<T>)span, value);
        }

        public static bool Contains<T>(this ReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer = null)
        {
            return IndexOf(span, value, comparer) >= 0;
        }

        public static unsafe int IndexOf<T>(this ReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer = null)
        {
            if (comparer == null)
            {
                comparer = EqualityComparerHelpers.GetComparerForReferenceTypesOnly<T>();
            }

            if (comparer != null)
            {
                for (int i = 0; i < span.Length; i++)
                {
                    if (comparer.Equals(span[i], value))
                        return i;
                }
            }
            else
            {
                for (int i = 0; i < span.Length; i++)
                {
                    if (EqualityComparerHelpers.StructOnlyEquals<T>(span[i], value))
                        return i;
                }
            }

            return -1;
        }
    }
}
