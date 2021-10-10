using System;

namespace ILCompiler.Common.TypeSystem
{
    public enum WellKnownType
    {
        Unknown,

        // Primitive types are first - keep in sync with type flags
        Void,
        Boolean,
        Char,
        SByte,
        Byte,
        Int16,
        UInt16,
        Int32,
        UInt32,
        Int64,
        UInt64,
        IntPtr,
        UIntPtr,
        Single,
        Double,

        ValueType,
        Enum,
        Nullable,

        Object,
        String,
        Array,
        MulticastDelegate,

        RuntimeTypeHandle,
        RuntimeMethodHandle,
        RuntimeFieldHandle,

        Exception,

        TypedReference,
        ByReferenceOfT,
    }

    public static class WellKnownTypeExtensions
    {
        public static int GetWellKnownTypeSize(this WellKnownType type)
        {
            return type switch
            {
                WellKnownType.SByte => 1,
                WellKnownType.Int16 => 2,
                WellKnownType.Int32 => 4,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
