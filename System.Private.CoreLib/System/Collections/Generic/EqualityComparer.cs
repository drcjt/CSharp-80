﻿using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    public abstract class EqualityComparer<T> : IEqualityComparer, IEqualityComparer<T>
    {
        [Intrinsic]
        private static EqualityComparer<T> Create()
        {
            throw new PlatformNotSupportedException();
        }

        public static EqualityComparer<T> Default { get; } = Create();
        public abstract bool Equals(T? x, T? y);
        public abstract int GetHashCode(T obj);

        int IEqualityComparer.GetHashCode(object obj)
        {
            if (obj is null) return 0;
            if (obj is T objAsT) return GetHashCode(objAsT);
            return 0;
        }
        bool IEqualityComparer.Equals(object? x, object? y)
        {
            if (x == y) return true;
            if (x == null || y == null) return false;
            if ((x is T) && (y is T)) return Equals((T)x, (T)y);
            return false;
        }
    }

    public sealed class ObjectEqualityComparer<T> : EqualityComparer<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(T x, T y)
        {
            if (x is not null)
            {
                if (y is not null) return x.Equals(y);
                return false;
            }
            if (y is not null) return false;
            return true;
        }

        public override bool Equals(object? obj) => throw new NotImplementedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode(T obj)
        {
            if (obj is not null)
                return obj.GetHashCode();

            return 0;
        }

        public override int GetHashCode() => throw new NotImplementedException();
    }

    public sealed class GenericEqualityComparer<T> : EqualityComparer<T> where T : IEquatable<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(T x, T y)
        {
            if (x is not null)
            {
                if (y is not null) return x.Equals(y);
                return false;
            }
            if (y is not null) return false;

            return true;
        }

        public override bool Equals(object? obj) => throw new NotImplementedException();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode(T obj)
        {
            if (obj is not null)
                return obj.GetHashCode();

            return 0;
        }

        public override int GetHashCode() => throw new NotImplementedException();
    }
}
