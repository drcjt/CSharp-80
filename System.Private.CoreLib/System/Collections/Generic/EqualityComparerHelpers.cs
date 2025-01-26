using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    public static class EqualityComparerHelpers
    {
        private static bool StructOnlyNormalEquals<T>(T left, T right) where T : notnull
        {
            return left.Equals(right);
        }

        private static bool StructOnlyEqualsIEquatable<T>(T x, T y) where T : IEquatable<T>
        {
            return x.Equals(y);
        }

        [Intrinsic]
        public static bool StructOnlyEquals<T>(T left, T right)
        {
            // No support for nullable currently so can just redirect to the non nullable struct equals
            return left!.Equals(right);
        }

        [Intrinsic]
        public static EqualityComparer<T> GetComparerForReferenceTypesOnly<T>()
        {
            return EqualityComparer<T>.Default;
        }
    }
}