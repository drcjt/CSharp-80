using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public enum Operation
    {
        Call,
        Return,
        Add,
        Sub,
        Mul,
        Div,
        Eq,
        Ne,
        Lt,
        Le,
        Gt,
        Ge,
        Constant_Int16,
        Constant_Int32,
        Constant_String,
        Intrinsic,
        Jump,
        JumpTrue,
        LocalVariable,
        StoreIndirect,
        StoreLocalVariable,
    }

    // StackEntry and subclasses represent the tree oriented high level intermediate representation
    // which will be the main output of the importer
    public abstract class StackEntry : IVisitableStackEntry
    {
        public StackValueKind Kind { get; }

        public StackEntry Next { get; set; }

        public Operation Operation { get; set; }

        protected StackEntry(StackValueKind kind)
        {
            Kind = kind;
        }

        abstract public void Accept(IStackEntryVisitor visitor);
    }
}
