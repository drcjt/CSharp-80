using dnlib.DotNet;

namespace ILCompiler.Common.TypeSystem.Common
{
    public struct ComputedInstanceFieldLayout
    {
        public LayoutInt FieldSize { get; set; }
        public LayoutInt FieldAlignment { get; set; }
        public LayoutInt ByteCountUnaligned { get; set; }
        public LayoutInt ByteCountAlignment { get; set; }

        public FieldAndOffset[] Offsets { get; set; }
    }
    public readonly struct FieldAndOffset
    {
        public static readonly LayoutInt InvalidOffset = new(int.MaxValue);

        public readonly FieldDef Field;
        public readonly LayoutInt Offset;

        public FieldAndOffset(FieldDef field, LayoutInt offset)
        {
            Field = field;
            Offset = offset;
        }
    }
}