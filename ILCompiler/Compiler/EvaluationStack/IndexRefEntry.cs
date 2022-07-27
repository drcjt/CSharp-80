using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class IndexRefEntry : StackEntry
    {
        public StackEntry IndexOp { get; }
        public StackEntry ArrayOp { get; }

        public int ElemSize { get; }

        public IndexRefEntry(StackEntry indexOp, StackEntry arrayOp, int elemSize, StackValueKind kind) : base(kind)
        {
            IndexOp = indexOp;
            ArrayOp = arrayOp;
            ElemSize = elemSize;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override StackEntry Duplicate()
        {
            return new IndexRefEntry(IndexOp.Duplicate(), ArrayOp.Duplicate(), ElemSize, Kind);
        }
    }
}
