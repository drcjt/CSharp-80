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

        /// <summary>
        /// The number of bytes required when allocating this type on the heap
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int GetInstanceByteCount(this TypeSig type)
        {
            var target = new TargetDetails(Common.TypeSystem.Common.TargetArchitecture.Z80);
            var fieldLayoutAlgorithm = new MetadataFieldLayoutAlgorithm(target);

            var typeDefOrRef = type.ToTypeDefOrRef();
            var typeDef = typeDefOrRef.ResolveTypeDef();

            var computedLayout = typeDef.InstanceFieldLayout(fieldLayoutAlgorithm);

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

            var instanceByteCountUnaligned = computedLayout.ByteCountUnaligned;
            var instanceByteAlignment = computedLayout.ByteCountAlignment;
            var instanceByteCount = LayoutInt.AlignUp(instanceByteCountUnaligned, instanceByteAlignment, target);

            return instanceByteCount.AsInt;
        }


        /// <summary>
        /// The number of bytes required to hold a field of this type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int GetInstanceFieldSize(this TypeSig type)
        {
            var target = new TargetDetails(Common.TypeSystem.Common.TargetArchitecture.Z80);
            var fieldLayoutAlgorithm = new MetadataFieldLayoutAlgorithm(target);
          
            var typeDefOrRef = type.TryGetTypeDefOrRef();
            var typeDef = typeDefOrRef.ResolveTypeDef();

            if (typeDef != null) 
            {
                var computedLayout = typeDef.InstanceFieldLayout(fieldLayoutAlgorithm);
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
