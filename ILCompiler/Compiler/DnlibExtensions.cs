using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;
using System.Diagnostics;

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
                    return VarType.Bool;

                case ElementType.I1:
                    return VarType.SByte;
                case ElementType.U1:
                case ElementType.Char:
                    return VarType.Byte;

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
                case ElementType.Object:
                    return VarType.Ref;

                case ElementType.ByRef:
                    return VarType.ByRef;

                default:
                    throw new NotSupportedException($"ElementType : {typeSig.ElementType} cannot be converted to VarType");
            }
        }

        public static int GetExactSize(this TypeSig type)
        {
            var target = new TargetDetails(Common.TypeSystem.Common.TargetArchitecture.Z80);
            var fieldLayoutAlgorithm = new MetadataFieldLayoutAlgorithm(target);

            var typeDefOrRef = type.TryGetTypeDefOrRef();
            var typeDef = typeDefOrRef.ResolveTypeDef();

            if (typeDef != null) 
            {
                var computedLayout = fieldLayoutAlgorithm.ComputeInstanceLayout(typeDef);

                if (computedLayout.Offsets != null)
                {
                    foreach (var fieldAndOffset in computedLayout.Offsets)
                    {
                        Debug.Assert(fieldAndOffset.Field.DeclaringType == typeDef);
                        fieldAndOffset.Field.FieldOffset = (uint)fieldAndOffset.Offset.AsInt;
                    }
                }

                if (type.ElementType == ElementType.Class)
                {
                    typeDef.ClassSize = (uint)computedLayout.FieldSize.AsInt;
                }

                return computedLayout.FieldSize.AsInt;
            }
            else
            {
                return target.GetWellKnownTypeSize(type).AsInt;
            }
        }
    }
}
