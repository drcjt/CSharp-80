namespace System.Collections.Generic
{
    internal static class EqualOnlyComparerHelper
    {
        public static bool Equals(sbyte x, sbyte y) => x == y;
        public static bool Equals(byte x, byte y) => x == y;
        public static bool Equals(Int16 x, Int16 y) => x == y;
        public static bool Equals(UInt16 x, UInt16 y) => x == y;
        public static bool Equals(Int32 x, Int32 y) => x == y;
        public static bool Equals(UInt32 x, UInt32 y) => x == y;
    }

    internal class EqualOnlyComparer<T>
    {
        public static bool Equals(T x, T y)
        {
            /*
            if (typeof(T) == typeof(sbyte))
                return EqualOnlyComparerHelper.Equals((sbyte)(Object)x, (sbyte)(Object)y);
            else if (typeof(T) == typeof(byte))
                return EqualOnlyComparerHelper.Equals((byte)(Object)x, (byte)(Object)y);
            else if (typeof(T) == typeof(Int16))
                return EqualOnlyComparerHelper.Equals((Int16)(Object)x, (Int16)(Object)y);
            else if (typeof(T) == typeof(UInt16))
                return EqualOnlyComparerHelper.Equals((UInt16)(Object)x, (UInt16)(Object)y);
            else if (typeof(T) == typeof(Int32))
                return EqualOnlyComparerHelper.Equals((Int32)(Object)x, (Int32)(Object)y);
            else if (typeof(T) == typeof(UInt32))
                return EqualOnlyComparerHelper.Equals((UInt32)(Object)x, (UInt32)(Object)y);
            */

            if (x == null)
                return y == null;

            if (y == null)
                return false;

            return x.Equals(y);
        }
    }
}
