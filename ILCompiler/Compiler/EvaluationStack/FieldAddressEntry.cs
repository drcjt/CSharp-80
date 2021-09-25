using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class FieldAddressEntry : StackEntry
    {
        public uint Offset => FieldDef.FieldOffset.Value;
        public int Size => FieldDef.FieldType.GetExactSize(false);

        public StackEntry Op1 { get; }

        public FieldDef FieldDef { get; }

        public FieldAddressEntry(StackEntry op1, FieldDef fieldDef) : base(StackValueKind.ObjRef, fieldDef.FieldType)
        {
            Operation = Operation.FieldAddress;
            Op1 = op1;
            FieldDef = fieldDef;
        }

        public override FieldAddressEntry Duplicate()
        {
            return new FieldAddressEntry(Op1.Duplicate(), FieldDef);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }

    }
}
