using dnlib.DotNet;

namespace ILCompiler.Common.TypeSystem.IL
{
    public static class DnlibExtensions
    {
        private const string CompilerIntrinsicAttribute = "System.Runtime.CompilerServices.IntrinsicAttribute";

        public static bool IsIntrinsic(this IMethodDefOrRef method)
        {
            return method.HasCustomAttributes && method.CustomAttributes.IsDefined(CompilerIntrinsicAttribute);
        }
        public static bool IsStruct(this TypeSig typeSig)
        {
            var typeDef = typeSig.TryGetTypeDef();
            if (typeDef == null)
            {
                return typeSig.IsValueType && !typeSig.IsPrimitive;
            }
            else
            {
                return typeDef.IsValueType && !typeDef.IsPrimitive && !typeDef.IsEnum;
            }
        }

        public static int GetHeapValueSize(this TypeSig typeSig)
        {
            var typeDefOrRef = typeSig.TryGetTypeDefOrRef();
            var typeDef = typeDefOrRef.ResolveTypeDef();

            if (typeDef != null)
            {
                if (typeDef.IsEnum)
                {
                    return GetHeapValueSize(typeDef.GetEnumUnderlyingType());
                }

                switch (typeSig.ElementType)
                {
                    case ElementType.Boolean:
                    case ElementType.Char:
                    case ElementType.I1:
                    case ElementType.U1:
                        return 1;
                    case ElementType.I2:
                    case ElementType.U2:
                        return 2;
                    case ElementType.I4:
                    case ElementType.U4:
                    case ElementType.R4:
                        return 4;
                    case ElementType.I8:
                    case ElementType.U8:
                    case ElementType.R8:
                        return 8;
                    case ElementType.Ptr:
                    case ElementType.I:
                        return 2;
                    case ElementType.ValueType:
                        var typeSize = 0;
                        foreach (var field in typeDef.Fields)
                        {
                            typeSize += GetHeapValueSize(field.FieldType);
                        }
                        return typeSize;
                    case ElementType.Class:
                    case ElementType.Array:
                    case ElementType.SZArray:
                    case ElementType.String:
                        return 2;
                    case ElementType.ByRef:
                        return 2;
                    default:
                        return -1;
                }
            }
            else
            {
                throw new Exception("Could not resolve type def");
            }
        }
    }
}