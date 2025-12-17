using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.ValueNumbering;

namespace ILCompiler.Compiler.Ssa
{
    public class LocalSsaVariableDescriptor
    {
        public BasicBlock Block { get; set; }

        public bool HasGlobalUse { get; private set; } = false;
        public bool HasPhiUse { get; private set; } = false;

        public ValueNumber ValueNumber { get; set; }

        public int NumberOfUses { get; private set; } = 0;

        public LocalVariableCommon? DefNode { get; private set; }

        public LocalSsaVariableDescriptor(BasicBlock block)
        {
            Block = block;
        }

        public LocalSsaVariableDescriptor(BasicBlock block, LocalVariableCommon defNode)
        {
            Block = block;
            DefNode = defNode;
        }

        public void AddUse(BasicBlock block)
        {
            if (block != Block)
            {
                HasGlobalUse = true;
            }

            NumberOfUses++;
        }

        public void AddPhiUse(BasicBlock block)
        {
            HasPhiUse = true;
            AddUse(block);
        }
    }
}
