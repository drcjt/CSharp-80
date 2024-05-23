using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace ILCompiler.Compiler
{
    public static class DnlibExtensions
    {
        public static T OperandAs<T>(this Instruction instruction)
        {
            return (T)instruction.Operand;
        }

        public static VarType GetVarType(this TypeSig typeSig)
        {
            var typeDefOrRef = typeSig.TryGetTypeDefOrRef();
            var typeDef = typeDefOrRef.ResolveTypeDef();

            if (typeDef != null && typeDef.IsEnum)
            {
                return GetVarType(typeDef.GetEnumUnderlyingType());
            }

            switch (typeSig.ElementType)
            {
                case ElementType.Void:
                    return VarType.Void;

                case ElementType.Boolean: 
                    return VarType.Byte;

                case ElementType.I1:
                    return VarType.SByte;
                case ElementType.U1:
                    return VarType.Byte;

                case ElementType.I2:
                    return VarType.Short;
                case ElementType.U2:
                case ElementType.Char:
                    return VarType.UShort;

                case ElementType.I4:
                    return VarType.Int;
                case ElementType.U4:
                    return VarType.UInt;

                case ElementType.Ptr:
                case ElementType.I:
                case ElementType.U:
                    return VarType.Ptr;

                case ElementType.ValueType:
                    return VarType.Struct;

                case ElementType.Class:
                case ElementType.String:
                case ElementType.Array:
                case ElementType.SZArray:
                case ElementType.Object:
                    return VarType.Ref;

                case ElementType.Pinned:
                    return GetVarType(typeSig.Next);

                case ElementType.ByRef:
                    return VarType.ByRef;

                default:
                    throw new NotSupportedException($"ElementType : {typeSig.ElementType} cannot be converted to VarType");
            }
        }
    }
}
