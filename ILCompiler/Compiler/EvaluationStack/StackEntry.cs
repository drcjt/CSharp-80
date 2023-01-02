namespace ILCompiler.Compiler.EvaluationStack
{
    public enum Operation
    {
        Neg,
        Not,

        Add,
        Sub,
        Mul,
        Div,
        Div_Un,
        Rem,
        Rem_Un,
        And,
        Or,

        Eq,
        Ge,
        Gt,
        Le,
        Lt,
        Ne,
        Lsh,
        Rsh,

        Gt_Un,
        Lt_Un,
    }

    // StackEntry and subclasses represent the tree oriented high level intermediate representation
    // which will be the main output of the importer
    public abstract class StackEntry : IVisitableStackEntry
    {
        public VarType Type { get; set; }

        public int? ExactSize { get; }

        public StackEntry? Next { get; set; }
        public int TreeID { get; }

        private static int _treeID = 0;

        public bool Contained { get; set; }

        protected StackEntry(VarType type, int? exactSize = null)
        {
            Type = type;
            ExactSize = exactSize;
            TreeID = _treeID++;
        }

        // TODO: Consider using a visitor to do the duplication
        public abstract StackEntry Duplicate();

        abstract public void Accept(IStackEntryVisitor visitor);

        public T As<T>() where T : StackEntry
        {
            return (T)this;
        }
    }
}
