namespace ILCompiler.Compiler.EvaluationStack
{
    public class WalkData(Func<Edge<StackEntry>, WalkData, WalkResult>? preOrderVisitorFunction = null, Func<Edge<StackEntry>, WalkData, WalkResult>? postOrderVisitorFunction = null)
    {
        public Func<Edge<StackEntry>, WalkData, WalkResult>? PreOrderVisitorFunction { get; init; } = preOrderVisitorFunction;
        public Func<Edge<StackEntry>, WalkData, WalkResult>? PostOrderVisitorFunction { get; init; } = postOrderVisitorFunction;
        public StackEntry? Parent { get; set; }
    }

    public class StackEntryWalker(WalkData walkData) : StackEntryVisitor
    {
        private readonly WalkData _walkData = walkData;

        public override WalkResult PreOrderVisit(Edge<StackEntry> use, StackEntry? user)
        {           
            _walkData.Parent = user;
            return _walkData.PreOrderVisitorFunction!(use, _walkData);
        }

        public override WalkResult PostOrderVisit(Edge<StackEntry> use, StackEntry? user)
        {
            _walkData.Parent = user;
            return _walkData.PostOrderVisitorFunction!(use, _walkData);
        }
    }
}
