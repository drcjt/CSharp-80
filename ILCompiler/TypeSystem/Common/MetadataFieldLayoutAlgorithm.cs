using System.Diagnostics;

namespace ILCompiler.TypeSystem.Common
{
    public class MetadataFieldLayoutAlgorithm
    {
        private readonly TargetDetails _target;
        public MetadataFieldLayoutAlgorithm(TargetDetails target)
        {
            _target = target;
        }

        public ComputedStaticFieldLayout ComputeStaticFieldLayout(DefType defType)
        {
            MetadataType type = (MetadataType)defType;
            int numStaticFields = 0;

            foreach (var field in type.GetFields())
            {
                if (!field.IsStatic || field.IsLiteral)
                    continue;

                numStaticFields++;
            }

            ComputedStaticFieldLayout result = new ComputedStaticFieldLayout();
            result.Size = new LayoutInt(0);

            if (numStaticFields == 0)
            {
                result.Offsets = Array.Empty<FieldAndOffset>();
                return result;
            }

            result.Offsets = new FieldAndOffset[numStaticFields];

            int index = 0;
            foreach (var field in type.GetFields())
            {
                if (!field.IsStatic || field.IsLiteral)
                    continue;

                var fieldType = field.FieldType;
                SizeAndAlignment sizeAndAlignment = ComputeFieldSizeAndAlignment(fieldType, 1);

                result.Size = LayoutInt.AlignUp(result.Size, sizeAndAlignment.Alignment, _target);
                result.Offsets[index] = new FieldAndOffset(field, result.Size);
                result.Size += sizeAndAlignment.Size;

                index++;
            }
            return result;
        }

        public ComputedInstanceFieldLayout ComputeInstanceLayout(DefType defType)
        {
            MetadataType type = (MetadataType)defType;
            Debug.Assert(type != null, "Cannot compute instance layout for null type def");
            // Count the number of instance fields in advance
            int numInstanceFields = 0;
            foreach (var field in type.GetFields())
            {
                if (field.IsStatic)
                    continue;

                TypeDesc fieldType = field.FieldType;

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

                var result = new ComputedInstanceFieldLayout
                {
                    ByteCountUnaligned = instanceByteSizeAndAlignment.Size,
                    ByteCountAlignment = instanceByteSizeAndAlignment.Alignment,
                    FieldAlignment = sizeAndAlignment.Alignment,
                    FieldSize = sizeAndAlignment.Size,
                };

                if (numInstanceFields > 0)
                {
                    FieldDesc? instanceField = null;
                    foreach (var field in type.GetFields())
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

        protected ComputedInstanceFieldLayout ComputeInstanceFieldLayout(MetadataType type, int numInstanceFields)
        {
            // TODO: implement auto layout https://github.com/drcjt/CSharp-80/issues/162
            // For now just use SequentialFieldLayout for everything
            return ComputeSequentialFieldLayout(type, numInstanceFields);
        }

        protected ComputedInstanceFieldLayout ComputeSequentialFieldLayout(MetadataType type, int numInstanceFields)
        {
            var offsets = new FieldAndOffset[numInstanceFields];

            // For types inheriting from another type, field offsets continue on from where they left off
            LayoutInt cumulativeInstanceFieldPos = ComputeBytesUsedInParentType(type);

            var layoutMetadata = type.GetClassLayout();

            LayoutInt largestAlignmentRequirement = LayoutInt.One;
            int fieldOrdinal = 0;
            int packingSize = ComputePackingSize(type);

            foreach (var field in type.GetFields())
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
            var instanceSizeAndAlignment = ComputeInstanceSize(type, cumulativeInstanceFieldPos, largestAlignmentRequirement, layoutMetadata.Size, out instanceByteSizeAndAlignment);

            var computedLayout = new ComputedInstanceFieldLayout();
            computedLayout.FieldAlignment = instanceSizeAndAlignment.Alignment;
            computedLayout.FieldSize = instanceSizeAndAlignment.Size;
            computedLayout.ByteCountUnaligned = instanceByteSizeAndAlignment.Size;
            computedLayout.ByteCountAlignment = instanceByteSizeAndAlignment.Alignment;
            computedLayout.Offsets = offsets;

            return computedLayout;
        }

        private int ComputePackingSize(DefType def)
        {
            return _target.DefaultPackingSize;
        }

        private SizeAndAlignment ComputeInstanceSize(DefType type, LayoutInt instanceSize, LayoutInt alignment, int classLayoutSize, out SizeAndAlignment byteCount)
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
                    parentSize = type.BaseType!.InstanceByteCountUnaligned;
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

                if (type.HasBaseType)
                {
                    alignment = LayoutInt.Max(alignment, type.BaseType!.InstanceByteAlignment);
                }
            }

            alignment = _target.GetObjectAlignment(alignment);

            byteCount.Size = instanceSize;
            byteCount.Alignment = alignment;

            return result;
        }

        private LayoutInt ComputeBytesUsedInParentType(DefType type)
        {
            LayoutInt cumulativeInstanceFieldPos = LayoutInt.Zero;

            if (!type.IsValueType && type.HasBaseType)
            {
                cumulativeInstanceFieldPos = type.BaseType!.InstanceByteCountUnaligned;
            }

            return cumulativeInstanceFieldPos;
        }

        private SizeAndAlignment ComputeFieldSizeAndAlignment(TypeDesc fieldType, int packingSize)
        {
            SizeAndAlignment result;

            if (fieldType.IsDefType)
            {
                if (fieldType.IsValueType)
                {
                    var defType = (DefType)fieldType;
                    result.Size = defType.InstanceFieldSize;
                    result.Alignment = defType.InstanceFieldAlignment;
                }
                else
                {
                    result.Size = fieldType.Context.Target.LayoutPointerSize;
                    result.Alignment = fieldType.Context.Target.LayoutPointerSize;
                }
            }
            else if (fieldType.IsArray)
            {
                result.Size = fieldType.Context.Target.LayoutPointerSize;
                result.Alignment = fieldType.Context.Target.LayoutPointerSize;
            }
            else
            {
                Debug.Assert(fieldType.IsPointer || fieldType.IsFunctionPointer);

                result.Size = _target.LayoutPointerSize;
                result.Alignment = _target.LayoutPointerSize;

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
