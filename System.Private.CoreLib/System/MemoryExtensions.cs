using System.Runtime.InteropServices;

namespace System
{
    public static class MemoryExtensions
    {
        public static bool SequenceEqual<T>(this Span<T> span, ReadOnlySpan<T> other) where T: IEquatable<T>
        {
            return SequenceEqual((ReadOnlySpan<T>)span, other);
        }

        public static unsafe bool SequenceEqual<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other) where T: IEquatable<T>
        {
            int length = span.Length;
            int otherLength = other.Length;

            return length == otherLength && SpanHelpers.SequenceEqual(ref MemoryMarshal.GetReference(span), ref MemoryMarshal.GetReference(other), length);
        }
    }
}
