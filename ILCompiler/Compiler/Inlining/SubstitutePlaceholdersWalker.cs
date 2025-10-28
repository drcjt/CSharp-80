using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.OpcodeImporters;

namespace ILCompiler.Compiler.Inlining
{
    public class SubstitutePlaceholdersWalker : StackEntryVisitor
    {
        private readonly CodeFolder _codeFolder;
        public SubstitutePlaceholdersWalker(CodeFolder codeFolder) : base()
        {
            _codeFolder = codeFolder;
        }

        public Statement WalkStatement(Statement statement)
        {
            WalkTree(new Edge<StackEntry>(() => statement.RootNode, x => { statement.RootNode = x; }), null);

            return statement;
        }

        public override WalkResult PreOrderVisit(Edge<StackEntry> use, StackEntry? user)
        {
            var node = use.Get();
            if (node is ReturnExpressionEntry)
            {
                UpdateInlineReturnExpressionPlaceHolder(use, user, _codeFolder);
            }
            return WalkResult.Continue;
        }

        public override WalkResult PostOrderVisit(Edge<StackEntry> use, StackEntry? user)
        {
            use.Set(_codeFolder.FoldExpression(use.Get()!));
            return WalkResult.Continue;
        }

        private static void UpdateInlineReturnExpressionPlaceHolder(Edge<StackEntry> use, StackEntry? user, CodeFolder codeFolder)
        {
            while (use.Get() is ReturnExpressionEntry)
            {
                var tree = use.Get();
                var inlineCandidate = tree;

                do
                {
                    var returnExpression = inlineCandidate as ReturnExpressionEntry;
                    inlineCandidate = returnExpression!.SubstitutionExpression;
                } while (inlineCandidate is ReturnExpressionEntry);

                inlineCandidate = codeFolder.FoldExpression(inlineCandidate!);

                use.Set(inlineCandidate!);
            }
        }
    }
}
