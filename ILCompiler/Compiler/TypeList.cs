using ILCompiler.Common.TypeSystem.IL;

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
    }
}
