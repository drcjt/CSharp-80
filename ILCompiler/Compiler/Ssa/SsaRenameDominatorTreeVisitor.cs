namespace ILCompiler.Compiler.Ssa
{
    internal class SsaRenameDominatorTreeVisitor : DominatorTreeVisitor
    {
        private readonly SsaRenameState _renameStack;
        public SsaRenameDominatorTreeVisitor(DominatorTreeNode root, SsaRenameState renameStack) : base(root)
        {
            _renameStack = renameStack;
        }

        public override void PostOrderVisit(BasicBlock block)
        {
            _renameStack.PopBlockStacks(block);
        }

        public override void PreOrderVisit(BasicBlock block)
        {
            // BlockRenameVariables(block);
            // AddPhiArgsToSuccessors(block);
        }
    }
}
