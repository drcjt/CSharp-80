using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILCompiler.Compiler.EvaluationStack
{
    public interface IStackEntryVisitor
    {
        public void Visit(ConstantEntry entry);
        public void Visit(StoreIndEntry entry);
        public void Visit(JumpTrueEntry entry);
        public void Visit(JumpEntry entry);
        public void Visit(ReturnEntry entry);
        public void Visit(BinaryOperator entry);
        public void Visit(LocalVariableEntry entry);
        public void Visit(StoreLocalVariableEntry entry);
        public void Visit(CallEntry entry);
        public void Visit(IntrinsicEntry entry);

    }
}
