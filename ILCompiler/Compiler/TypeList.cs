namespace ILCompiler.Compiler
{
    public enum VarType
    {
        Void,

        Bool,
        Byte,
        SByte,

        Short,
        UShort,

        Int,
        UInt,

        //Long,
        //ULong,

        //Float,
        //Double,

        Ref,        // Object references
        ByRef,
        Struct,     // Custom Value Types

        Ptr,        // Unmanaged pointers, NativeInt, NativeUInt
    }

    public static class VarTypeExtensions
    {
        public static bool IsSmall(this VarType type)
        {
            return type >= VarType.Bool && type <= VarType.UShort;
        }

        public static bool IsByte(this VarType type)
        {
            return type >= VarType.Bool && type <= VarType.SByte;
        }

        public static bool IsShort(this VarType type)
        {
            return type == VarType.Short || type == VarType.UShort;
        }

        public static bool IsSmallInt(this VarType type)
        {
            return type >= VarType.Byte && type <= VarType.UShort;
        }

        public static bool IsUnsigned(this VarType type)
        {
            return type == VarType.Bool || type == VarType.Byte || type == VarType.UShort || type == VarType.UInt;
        }

        public static bool IsInt(this VarType type)
        {
            return type >= VarType.Bool && type <= VarType.UInt;
        }

        public static int GetTypeSize(this VarType type)
        {
            switch (type)
            {
                case VarType.Bool:
                case VarType.Byte:
                case VarType.SByte:
                    return 1;

                case VarType.Short:
                case VarType.UShort:
                    return 2;

                case VarType.Int:
                case VarType.UInt:
                    return 4;

                case VarType.Ref:
                case VarType.ByRef:
                case VarType.Ptr:
                    return 2;

                default:
                    throw new NotImplementedException($"GetTypeSize for {type} is not implemented");
            }
        }
    }
}
