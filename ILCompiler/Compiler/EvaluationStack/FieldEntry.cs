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
        // Some information about field goes here

        public FieldEntry(StackValueKind kind) : base(kind)
        {
        }

        public override FieldEntry Duplicate()
        {
            return new FieldEntry(Kind);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
