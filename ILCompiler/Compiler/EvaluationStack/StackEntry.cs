using ILCompiler.Compiler.LinearIR;
using System.Diagnostics.CodeAnalysis;

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
        Xor,

        Eq,
        Ge,
        Gt,
        Le,
        Lt,
        Ne_Un,
        Ge_Un,
        Gt_Un,
        Le_Un,
        Lt_Un,

        Lsh,
        Rsh,
    }

    // StackEntry and subclasses represent the tree oriented high level intermediate representation
    // which will be the main output of the importer
    public abstract class StackEntry : IVisitableStackEntry
    {
        public VarType Type { get; }

        public int? ExactSize { get; }

        public StackEntry? Next { get; set; }
        public StackEntry? Prev { get; set; }

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

        public virtual bool TryGetUse(StackEntry operand, [NotNullWhen(returnValue: true)] out Edge<StackEntry>? edge)
        {
            edge = null;
            return false;
        }

        public static void ReplaceOperand(Edge<StackEntry> useEdge, StackEntry replacement) => useEdge.Set(replacement);

        public void ReplaceWith(StackEntry newTree, BasicBlock block)
        {
            if (block.TryGetUse(this, out Use? use))
            {
                use.ReplaceWith(newTree);
            }
        }
    }
}
