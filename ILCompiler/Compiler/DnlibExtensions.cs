using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.IL;
using System;

namespace ILCompiler.Compiler
{
    public static class DnlibExtensions
    {
        public static int GetExactSize(this TypeSig type, bool setFieldOffset = false)
        {
            if (type.ElementType == ElementType.ValueType)
            {
                var typeDefOrRef = type.TryGetTypeDefOrRef();
                var typeDef = typeDefOrRef.ResolveTypeDef();
                if (typeDef != null)
                {
                    var typeSize = 0;
                    foreach (var field in typeDef.Fields)
                    {
                        if (setFieldOffset)
                        {
                            field.FieldOffset = (uint)typeSize;
                        }
                        typeSize += GetExactSize(field.FieldType);
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
