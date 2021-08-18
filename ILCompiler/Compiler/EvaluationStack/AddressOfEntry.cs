using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class AddressOfEntry : StackEntry
    {
        public StackEntry Op1 { get; }

        // TODO: Should kind be nativeint or byref?
        public AddressOfEntry(StackEntry op1) : base(op1.Kind)
        {
            Operation = Operation.AddressOf;
            Op1 = op1;
        }

        public override StackEntry Duplicate()
        {
            return new AddressOfEntry(Op1.Duplicate());
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
