using ILCompiler.Compiler.LinearIR;
using System.Diagnostics.CodeAnalysis;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class FieldAddressEntry : StackEntry
    {
        public uint Offset { get; }

        public StackEntry Op1 { get; set; }

        public string Name { get; }

        public FieldAddressEntry(String name, StackEntry op1, uint offset) : base(VarType.Ptr, 2)
        {
            Name = name;
            Op1 = op1;
            Offset = offset;
        }

        public override FieldAddressEntry Duplicate()
        {
            return new FieldAddressEntry(Name, Op1.Duplicate(), Offset);
        }

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);

        public override bool TryGetUse(StackEntry operand, [NotNullWhen(true)] out Edge<StackEntry>? edge)
        {
            if (operand == Op1)
            {
                edge = new Edge<StackEntry>(() => Op1, x => Op1 = x);
                return true;
            }
            return base.TryGetUse(operand, out edge);
        }
    }
}
