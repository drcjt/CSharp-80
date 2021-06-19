using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    // StackEntry and subclasses represent the tree oriented high level intermediate representation
    // which will be the main output of the importer
    public abstract class StackEntry
    {
        public StackValueKind Kind { get; }

        public StackEntry Next { get; set; }

        protected StackEntry(StackValueKind kind)
        {
            Kind = kind;
        }
    }
}
