using dnlib.DotNet;

namespace ILCompiler.Common.TypeSystem.IL
{
    public static class DnlibExtensions
    {
        private const string CompilerIntrinsicAttribute = "System.Runtime.CompilerServices.IntrinsicAttribute";

        public static bool IsIntrinsic(this MethodDef method)
        {
            return method.HasCustomAttributes && method.CustomAttributes.IsDefined(CompilerIntrinsicAttribute);
        }

        public static bool IsUnsigned(this TypeSig typeSig)
        {
            switch (typeSig.ElementType)
            {
                case ElementType.U1:
                case ElementType.U2:
                case ElementType.U4:
                case ElementType.U8:
                    return true;
            }

            return false;
        }
        public static StackValueKind GetStackValueKind(this TypeSig typeSig)
        {
            switch (typeSig.ElementType)
            {
                case ElementType.Boolean:
                case ElementType.Char:
                case ElementType.I1:
                case ElementType.U1:
                case ElementType.I2:
                case ElementType.U2:
                    return StackValueKind.Int16;
                case ElementType.I4:
                case ElementType.U4:
                    return StackValueKind.Int32;
                case ElementType.I8:
                case ElementType.U8:
                    return StackValueKind.Int64;
                case ElementType.R4:
                case ElementType.R8:
                    return StackValueKind.Float;
                case ElementType.Ptr:
                    return StackValueKind.NativeInt;
                case ElementType.ValueType:
                    return StackValueKind.ValueType;
                case ElementType.Class:
                case ElementType.Array:
                case ElementType.SZArray:
                case ElementType.String:
                    return StackValueKind.ObjRef;
                case ElementType.ByRef:
                    return StackValueKind.ByRef;
                default:
                    return StackValueKind.Unknown;
            }
        }
    }
}