using ILCompiler.Common.TypeSystem.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILCompiler.Compiler.EvaluationStack
{
    // LCL_VAR
    public class LocalVariableEntry : StackEntry
    {
        public int LocalNumber { get; }
        public LocalVariableEntry(int localNumber, StackValueKind kind) : base(kind)
        {
            LocalNumber = localNumber;
        }
    }
}
