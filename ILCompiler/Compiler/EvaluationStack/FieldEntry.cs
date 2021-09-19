using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class FieldEntry : StackEntry
    {
        public uint Offset => FieldDef.FieldOffset.Value;
        public int Size => FieldDef.FieldType.GetExactSize(false);

        public StackEntry Op1 { get; }

        public FieldDef FieldDef { get; }

        public FieldEntry(StackEntry op1, FieldDef fieldDef, StackValueKind kind) : base(kind, fieldDef.FieldType)
        {
            Operation = Operation.Field;
            Op1 = op1;
            FieldDef = fieldDef;
        }

        public override FieldEntry Duplicate()
        {
            return new FieldEntry(Op1.Duplicate(), FieldDef, Kind);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
