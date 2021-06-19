using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILCompiler.Compiler.EvaluationStack
{
    public interface IVisitableStackEntry
    {
        public void Accept(IStackEntryVisitor visitor);
    }
}
