namespace ILCompiler.Compiler.EvaluationStack
{
    public class Statement
    {
        public StackEntry RootNode { get; init; }

        public List<StackEntry> TreeList { get; set; } = [];

        public Statement(StackEntry rootNode)
        {
            RootNode = rootNode;
        }

        public bool IsPhiDefinition => RootNode is StoreLocalVariableEntry store && store.Op1 is PhiNode;
    }
}
