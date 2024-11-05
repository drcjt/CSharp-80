namespace ILCompiler.Compiler.EvaluationStack
{
    public class AllocObjEntry : StackEntry
    {
        public int Size { get; }
        public StackEntry EETypeNode { get; }

        public AllocObjEntry(StackEntry eeTypeNode, int objSize, VarType objType) : base(objType)
        {
            EETypeNode = eeTypeNode;
            Size = objSize;
        }

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);

        public override StackEntry Duplicate()
        {
            return new AllocObjEntry(EETypeNode, Size, Type);
        }
    }
}
