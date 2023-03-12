namespace ILCompiler.Compiler.EvaluationStack
{
    public class IndexRefEntry : StackEntry
    {
        public StackEntry IndexOp { get; }
        public StackEntry ArrayOp { get; }

        public short FirstElementOffset { get; }

        public int ElemSize { get; }

        public IndexRefEntry(StackEntry indexOp, StackEntry arrayOp, int elemSize, VarType type, short firstElementOffset) : base(type)
        {
            IndexOp = indexOp;
            ArrayOp = arrayOp;
            ElemSize = elemSize;
            FirstElementOffset = firstElementOffset;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override StackEntry Duplicate()
        {
            return new IndexRefEntry(IndexOp.Duplicate(), ArrayOp.Duplicate(), ElemSize, Type, FirstElementOffset);
        }
    }
}
