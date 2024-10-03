using ILCompiler.Compiler.LinearIR;
using System.Diagnostics.CodeAnalysis;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class IndexRefEntry : StackEntry
    {
        public StackEntry IndexOp { get; set; }
        public StackEntry ArrayOp { get; set; }

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

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);

        public override StackEntry Duplicate()
        {
            return new IndexRefEntry(IndexOp.Duplicate(), ArrayOp.Duplicate(), ElemSize, Type, FirstElementOffset, BoundsCheck);
        }

        public override bool TryGetUse(StackEntry operand, [NotNullWhen(true)] out Edge<StackEntry>? edge)
        {
            if (operand == IndexOp)
            {
                edge = new Edge<StackEntry>(() => IndexOp, x => IndexOp = x);
                return true;
            }
            if (operand == ArrayOp)
            {
                edge = new Edge<StackEntry>(() => ArrayOp, x => ArrayOp = x);
                return true;
            }

            return base.TryGetUse(operand, out edge);
        }
    }
}
