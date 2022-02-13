using ILCompiler.Common.TypeSystem.IL;
using System;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class AllocObjEntry : StackEntry
    {
        public int Size { get; }

        public AllocObjEntry(int objSize, StackValueKind objKind) : base(objKind)
        {
            Size = objSize;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Vist(this);
        }

        public override StackEntry Duplicate()
        {
            return new AllocObjEntry(Size, Kind);
        }
    }
}
