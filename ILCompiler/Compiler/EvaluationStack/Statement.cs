namespace ILCompiler.Compiler.EvaluationStack
{
    public class Statement
    {
        public uint StartOffset { get; set; }
        public uint EndOffset { get; set; }
        public StackEntry RootNode { get; set; }

        public List<StackEntry> TreeList { get; set; } = [];

        public Statement(StackEntry rootNode)
        {
            RootNode = rootNode;
        }

        public bool IsPhiDefinition => RootNode is StoreLocalVariableEntry store && store.Op1 is PhiNode;
    }
}
