using dnlib.DotNet;

namespace ILCompiler.Common.TypeSystem.Common
{
    public static class TypeDefExtensions
    {
        private static IDictionary<TypeDef, ComputedInstanceFieldLayout> _fieldLayoutsByType = new Dictionary<TypeDef, ComputedInstanceFieldLayout>();
        public static ComputedInstanceFieldLayout InstanceFieldLayout(this TypeDef typedef, MetadataFieldLayoutAlgorithm layoutAlgorithm)
        {
            if (!_fieldLayoutsByType.TryGetValue(typedef, out var fieldLayout))
            {
                fieldLayout = layoutAlgorithm.ComputeInstanceLayout(typedef);
                _fieldLayoutsByType.Add(typedef, fieldLayout);
            }

            return fieldLayout;
        }

        public static LayoutInt InstanceFieldSize(this TypeDef typedef, MetadataFieldLayoutAlgorithm fieldLayoutAlgorithm) 
        {
            return typedef.InstanceFieldLayout(fieldLayoutAlgorithm).FieldSize;
        }

        public static LayoutInt InstanceFieldAlignment(this TypeDef typedef, MetadataFieldLayoutAlgorithm fieldLayoutAlgorithm)
        {
            return typedef.InstanceFieldLayout(fieldLayoutAlgorithm).FieldAlignment;
        }

        public static LayoutInt InstanceByteCountUnaligned(this TypeDef typedef, MetadataFieldLayoutAlgorithm fieldLayoutAlgorithm)
        {
            return typedef.InstanceFieldLayout(fieldLayoutAlgorithm).ByteCountUnaligned;
        }
        public static LayoutInt InstanceByteAlignment(this TypeDef typedef, MetadataFieldLayoutAlgorithm fieldLayoutAlgorithm)
        {
            return typedef.InstanceFieldLayout(fieldLayoutAlgorithm).ByteCountAlignment;
        }
    }
}
