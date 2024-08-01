namespace ILCompiler.Compiler.FlowgraphHelpers
{
    public class FlowEdge
    {
        public BasicBlock Source { get; }
        public BasicBlock Target { get; }

        public FlowEdge? NextPredEdge { get; }
        
        public FlowEdge(BasicBlock source, BasicBlock target, FlowEdge? nextPredEdge = null)
        {
            Source = source;
            Target = target;
            NextPredEdge = nextPredEdge;
        }
    }
}
