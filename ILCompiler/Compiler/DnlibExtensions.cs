using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using System;
using System.Diagnostics;

namespace ILCompiler.Compiler
{
    public static class DnlibExtensions
    {
        public static T OperandAs<T>(this Instruction instruction)
        {
            return (T)instruction.Operand;
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
                return TypeList.GetExactSize(type.GetStackValueKind());
            }
        }
    }
}
