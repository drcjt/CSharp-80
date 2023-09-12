namespace ILCompiler.Compiler.Ssa
{
    public class LocalSsaVariableDescriptor
    {
        public BasicBlock Block { get; set; }

        public bool HasGlobalUse { get; private set; } = false;
        public bool HasPhiUse { get; private set; } = false;

        public int NumberOfUses { get; private set; } = 0;

        public LocalSsaVariableDescriptor(BasicBlock block)
        {
            Block = block;
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
