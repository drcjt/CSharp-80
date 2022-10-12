namespace ILCompiler.Compiler.EvaluationStack
{
    public class IndexRefEntry : StackEntry
    {
        public StackEntry IndexOp { get; }
        public StackEntry ArrayOp { get; }

        public int ElemSize { get; }

        public IndexRefEntry(StackEntry indexOp, StackEntry arrayOp, int elemSize, VarType type) : base(type)
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
            return new IndexRefEntry(IndexOp.Duplicate(), ArrayOp.Duplicate(), ElemSize, Type);
        }
    }
}
