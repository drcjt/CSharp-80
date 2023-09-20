namespace ILCompiler.Compiler.EvaluationStack
{
    public class IndexRefEntry : StackEntry
    {
        public StackEntry IndexOp { get; }
        public StackEntry ArrayOp { get; }

        public short FirstElementOffset { get; }

        public int ElemSize { get; }
        public bool BoundsCheck { get; }

        public IndexRefEntry(StackEntry indexOp, StackEntry arrayOp, int elemSize, VarType type, short firstElementOffset, bool boundsCheck) : base(type, elemSize)
        {
            IndexOp = indexOp;
            ArrayOp = arrayOp;
            ElemSize = elemSize;
            FirstElementOffset = firstElementOffset;
            BoundsCheck = boundsCheck;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override StackEntry Duplicate()
        {
            return new IndexRefEntry(IndexOp.Duplicate(), ArrayOp.Duplicate(), ElemSize, Type, FirstElementOffset, BoundsCheck);
        }
    }
}
