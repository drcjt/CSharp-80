using ILCompiler.Interfaces;

namespace ILCompiler.Compiler
{
    internal class ObjectAllocator : IObjectAllocator
    {
        public void DoPhase(IList<BasicBlock> blocks)
        {
            MorphAllocObjNodes(blocks);
        }

        private void MorphAllocObjNodes(IList<BasicBlock> blocks)
        {
            // Morph each AllocObj node into either an allocation helper call or stack allocation
        }
    }
}
