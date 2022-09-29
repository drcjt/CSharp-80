using dnlib.DotNet;

namespace ILCompiler.Common.TypeSystem.Common
{
    public struct ComputedInstanceFieldLayout
    {
        public LayoutInt FieldSize;
        public LayoutInt FieldAlignment;
        public LayoutInt ByteCountUnaligned;
        public LayoutInt ByteCountAlignment;

        public FieldAndOffset[] Offsets;
    }
    public struct FieldAndOffset
    {
        public static readonly LayoutInt InvalidOffset = new LayoutInt(int.MaxValue);

        public readonly FieldDef Field;
        public readonly LayoutInt Offset;

        public FieldAndOffset(FieldDef field, LayoutInt offset)
        {
            Field = field;
            Offset = offset;
        }
    }
}