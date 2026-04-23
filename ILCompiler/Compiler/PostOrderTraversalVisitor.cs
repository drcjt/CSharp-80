using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler
{
    public class PostOrderTraversalVisitor : StackEntryVisitor
    {
        public StackEntry? Current { get; private set; }
        public StackEntry? First { get; set; }

        public List<StackEntry> PostOrderNodes { get; set; } = new List<StackEntry>();

        public PostOrderTraversalVisitor(StackEntry? current)
        {
            Current = current;
        }

        public override WalkResult PostOrderVisit(Edge<StackEntry> use, StackEntry? user)
        {
            SetNext(use.Get());
            return WalkResult.Continue;
        }
        private void SetNext(StackEntry entry)
        {
            PostOrderNodes.Add(entry);
            if (Current != null)
            {
                Current.Next = entry;
                entry.Prev = Current;
            }
            Current = entry;
            if (First == null)
            {
                First = Current;
            }
        }
    }
}
