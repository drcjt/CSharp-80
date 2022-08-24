using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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
                case ElementType.Boolean: 
                    return VarType.Bool;

                case ElementType.Char:
                case ElementType.I1:
                    return VarType.Byte;
                case ElementType.U1:
                    return VarType.SByte;

                case ElementType.I2:
                    return VarType.Short;
                case ElementType.U2:
                    return VarType.UShort;

                case ElementType.I4:
                    return VarType.Int;
                case ElementType.U4:
                    return VarType.UInt;

                case ElementType.Ptr:
                case ElementType.I:
                    return VarType.Ptr;

                case ElementType.ValueType:
                    return VarType.Struct;

                case ElementType.Class:
                case ElementType.String:
                case ElementType.Array:
                case ElementType.SZArray:
                    return VarType.Ref;

                case ElementType.ByRef:
                    return VarType.ByRef;

                default:
                    throw new NotSupportedException($"ElementType : {typeSig.ElementType} cannot be converted to VarType");
            }
        }

        public static int GetExactSize(this TypeSig type)
        {
            if (type.ElementType == ElementType.ValueType)
            {
                var typeDefOrRef = type.TryGetTypeDefOrRef();
                var typeDef = typeDefOrRef.ResolveTypeDef();
                if (typeDef != null)
                {
                    var typeSize = 0;

                    if (typeDef.IsEnum)
                    {
                        typeSize = TypeList.GetExactSize(typeDef.GetEnumUnderlyingType().GetStackValueKind());
                    }
                    else
                    {
                        foreach (var field in typeDef.Fields)
                        {
                            if (field.FieldOffset.HasValue)
                            {
                                Debug.Assert(typeSize == field.FieldOffset.Value);
                            }
                            field.FieldOffset = (uint)typeSize;
                            typeSize += GetExactSize(field.FieldType);
                        }
                    }

                    return typeSize;
                }
                else
                {
                    throw new Exception("Could not resolve type def");
                }
            }
            else
            {
                if (type.ElementType == ElementType.Class)
                {
                    var typeDefOrRef = type.TryGetTypeDefOrRef();
                    var typeDef = typeDefOrRef.ResolveTypeDef();
                    if (typeDef != null)
                    {
                        typeDef.ClassSize = 0;
                        foreach (var field in typeDef.Fields)
                        {
                            field.FieldOffset = typeDef.ClassSize;
                            typeDef.ClassSize += (uint)GetExactSize(field.FieldType);
                        }
                    }
                }
                return TypeList.GetExactSize(type.GetStackValueKind());
            }
        }
    }
}
