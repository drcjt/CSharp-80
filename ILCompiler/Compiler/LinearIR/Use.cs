using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.LinearIR
{
    /// <summary>
    /// Represents a use <-> def edge between two nodes
    /// in a range of LinearIR.
    /// </summary>
    public class Use
    {
        private readonly Range _range;
        private readonly Edge<StackEntry> _edge;
        private StackEntry User { get; init; }

        public Use(Range range, Edge<StackEntry> edge, StackEntry user)
        {
            _range = range;
            _edge = edge;
            User = user;
        }

        public StackEntry Def => _edge.Get();

        public void ReplaceWith(StackEntry replacement)
        {
            StackEntry.ReplaceOperand(_edge, replacement);
        }

        public StackEntry ReplaceWithLclVar(LocalVariableTable locals)
        {
            var node = _edge.Get();

            var type = Def.Type;
            var lclNum = locals.GrabTemp(node.Type, node.Type.GetTypeSize());
            var store = new StoreLocalVariableEntry(lclNum, false, node);
            var load = new LocalVariableEntry(store.LocalNumber, store.Type, type.GetTypeSize());

            _range.InsertAfter(node, store, load);

            ReplaceWith(load);

            return Def;
        }
    }
}
