using ILCompiler.Common.TypeSystem.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class FieldEntry : StackEntry
    {
        public uint FieldOffset { get; }
        public int Size { get; }

        public StackEntry Op1 { get; }

        public FieldEntry(StackEntry op1, uint? fieldOffset, int size, StackValueKind kind) : base(kind)
        {
            Operation = Operation.Field;
            Op1 = op1;
            Size = size;
            FieldOffset = fieldOffset.Value;
        }

        public override FieldEntry Duplicate()
        {
            return new FieldEntry(Op1.Duplicate(), FieldOffset, Size, Kind);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
