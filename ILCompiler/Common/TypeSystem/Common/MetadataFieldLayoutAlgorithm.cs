﻿using dnlib.DotNet;
using System.Diagnostics;

namespace ILCompiler.Common.TypeSystem.Common
{
    public class MetadataFieldLayoutAlgorithm
    {
        private readonly TargetDetails _target;
        public MetadataFieldLayoutAlgorithm(TargetDetails target)
        {
            _target = target;
        }

        public ComputedInstanceFieldLayout ComputeInstanceLayout(TypeDef type)
        {
            Debug.Assert(type != null, "Cannot compute instance layout for null type def");
            // Count the number of instance fields in advance
            int numInstanceFields = 0;
            foreach (var field in type.Fields)
            {
                if (field.IsStatic)
                    continue;

                TypeSig fieldType = field.FieldType;

                if (fieldType.IsByRef)
                {
                    throw new InvalidOperationException("ByRef Instance fields are not allowed");
                }

                numInstanceFields++;
            }

            if (type.IsEnum && numInstanceFields != 1)
            {
                throw new InvalidOperationException("Enum types must have a single instance field");
            }

            if (type.IsPrimitive)
            {
                if (numInstanceFields > 1)
                {
                    throw new InvalidOperationException("Primitive type cannot have more than one field");
                }

                SizeAndAlignment instanceByteSizeAndAlignment;
                var sizeAndAlignment = ComputeInstanceSize(
                    type,
                    _target.GetWellKnownTypeSize(type),
                    _target.GetWellKnownTypeAlignment(type),
                    0,
                    out instanceByteSizeAndAlignment);

                ComputedInstanceFieldLayout result = new ComputedInstanceFieldLayout
                {
                    ByteCountUnaligned = instanceByteSizeAndAlignment.Size,
                    ByteCountAlignment = instanceByteSizeAndAlignment.Alignment,
                    FieldAlignment = sizeAndAlignment.Alignment,
                    FieldSize = sizeAndAlignment.Size,
                };

                if (numInstanceFields > 0)
                {
                    FieldDef? instanceField = null;
                    foreach (var field in type.Fields)
                    {
                        if (!field.IsStatic)
                        {
                            Debug.Assert(instanceField == null, "Unexpected extra instance field");
                            instanceField = field;
                        }
                    }

                    Debug.Assert(instanceField != null, "Null instance field");

                    result.Offsets = new FieldAndOffset[] { new FieldAndOffset(instanceField, LayoutInt.Zero) };
                }
                else
                {
                    result.Offsets = Array.Empty<FieldAndOffset>();
                }

                return result;
            }

            return ComputeInstanceFieldLayout(type, numInstanceFields);
        }

        protected ComputedInstanceFieldLayout ComputeInstanceFieldLayout(TypeDef type, int numInstanceFields)
        {
            return ComputeSequentialFieldLayout(type, numInstanceFields);
        }

        protected ComputedInstanceFieldLayout ComputeSequentialFieldLayout(TypeDef type, int numInstanceFields)
        {
            var offsets = new FieldAndOffset[numInstanceFields];

            // For types inheriting from another type, field offsets continue on from where they left off
            LayoutInt cumulativeInstanceFieldPos = ComputeBytesUsedInParentType(type);

            LayoutInt largestAlignmentRequirement = LayoutInt.One;
            int fieldOrdinal = 0;
            int packingSize = ComputePackingSize(type);

            foreach (var field in type.Fields)
            {
                if (field.IsStatic)
                    continue;

                var fieldSizeAndAlignment = ComputeFieldSizeAndAlignment(field.FieldType, packingSize);

                largestAlignmentRequirement = LayoutInt.Max(fieldSizeAndAlignment.Alignment, largestAlignmentRequirement);

                cumulativeInstanceFieldPos = LayoutInt.AlignUp(cumulativeInstanceFieldPos, fieldSizeAndAlignment.Alignment, _target);
                offsets[fieldOrdinal] = new FieldAndOffset(field, cumulativeInstanceFieldPos);
                cumulativeInstanceFieldPos = checked(cumulativeInstanceFieldPos + fieldSizeAndAlignment.Size);

                fieldOrdinal++;
            }

            SizeAndAlignment instanceByteSizeAndAlignment;
            var instanceSizeAndAlignment = ComputeInstanceSize(type, cumulativeInstanceFieldPos, largestAlignmentRequirement, (int)(type?.ClassSize ?? 0), out instanceByteSizeAndAlignment);

            ComputedInstanceFieldLayout computedLayout = new ComputedInstanceFieldLayout();
            computedLayout.FieldAlignment = instanceSizeAndAlignment.Alignment;
            computedLayout.FieldSize = instanceSizeAndAlignment.Size;
            computedLayout.ByteCountUnaligned = instanceByteSizeAndAlignment.Size;
            computedLayout.ByteCountAlignment = instanceByteSizeAndAlignment.Alignment;
            computedLayout.Offsets = offsets;

            return computedLayout;
        }

        private int ComputePackingSize(TypeDef def)
        {
            return _target.DefaultPackingSize;
        }

        private SizeAndAlignment ComputeInstanceSize(TypeDef type, LayoutInt instanceSize, LayoutInt alignment, int classLayoutSize, out SizeAndAlignment byteCount)
        {
            SizeAndAlignment result;

            if (type.IsValueType && instanceSize == LayoutInt.Zero)
            {
                instanceSize = LayoutInt.One;
            }

            if (classLayoutSize > 0)
            {
                LayoutInt parentSize;
                if (type.IsValueType)
                {
                    parentSize = LayoutInt.Zero;
                }
                else
                {
                    var baseType = type.GetBaseType(true);
                    var baseTypeDef = baseType.ResolveTypeDef();
                    parentSize = new LayoutInt(baseTypeDef?.ClassSize ?? 0); //parentSize = type.BaseType.InstanceByteCountUnaligned;
                }
                LayoutInt specifiedInstanceSize = parentSize + new LayoutInt(classLayoutSize);
                instanceSize = LayoutInt.Max(specifiedInstanceSize, instanceSize);
            }
            else
            {
                if (type.IsValueType)
                {
                    instanceSize = LayoutInt.AlignUp(instanceSize, alignment, _target);
                }
            }

            if (type.IsValueType)
            {
                result.Size = instanceSize;
                result.Alignment = alignment;
            }
            else
            {
                result.Size = _target.LayoutPointerSize;
                result.Alignment = _target.LayoutPointerSize;
                /*
                // TODO: work out what this should be using dnlib
                if (type.HasBaseType)
                {
                    alignment = LayoutInt.Max(alignment, type.BaseType.InstanceByteAlignment);
                }
                */
            }

            alignment = _target.GetObjectAlignment(alignment);

            byteCount.Size = instanceSize;
            byteCount.Alignment = alignment;

            return result;
        }

        private static LayoutInt ComputeBytesUsedInParentType(TypeDef type)
        {
            LayoutInt cumulativeInstanceFieldPos = LayoutInt.Zero;

            // TODO: Work out how to do this using dnlib
            /*
            if (!type.IsValueType && type.HasBaseType)
            {
                cumulativeInstanceFieldPos = type.BaseType.InstanceByteCountUnaligned;
            }
            */

            return cumulativeInstanceFieldPos;
        }

        private SizeAndAlignment ComputeFieldSizeAndAlignment(TypeSig fieldTypeSig, int packingSize)
        {
            SizeAndAlignment result;

            if (fieldTypeSig.IsTypeDefOrRef)
            {
                if (fieldTypeSig.IsValueType)
                {
                    var fieldTypeDefOrRef = fieldTypeSig.TryGetTypeDefOrRef();
                    var fieldType = fieldTypeDefOrRef.ResolveTypeDef();

                    if (fieldType != null)
                    {
                        var computedLayout = ComputeInstanceLayout(fieldType);
                        result.Size = computedLayout.FieldSize;
                        result.Alignment = computedLayout.FieldAlignment;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Failed to resolve type def for field {fieldTypeDefOrRef.Name}");
                    }
                }
                else
                {
                    result.Size = _target.LayoutPointerSize;
                    result.Alignment = _target.LayoutPointerSize;
                }
            }
            else
            {
                if (fieldTypeSig.IsSZArray)
                {
                    result.Size = _target.LayoutPointerSize;
                    result.Alignment = _target.LayoutPointerSize;
                }
                else
                {
                    Debug.Assert(fieldTypeSig.IsPointer || fieldTypeSig.IsFunctionPointer);

                    result.Size = _target.LayoutPointerSize;
                    result.Alignment = _target.LayoutPointerSize;
                }
            }

            result.Alignment = LayoutInt.Min(result.Alignment, new LayoutInt(packingSize));

            return result;
        }

        private struct SizeAndAlignment
        {
            public LayoutInt Size;
            public LayoutInt Alignment;
        }
    }
}
