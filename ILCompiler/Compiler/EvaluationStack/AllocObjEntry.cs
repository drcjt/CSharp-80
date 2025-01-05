namespace ILCompiler.Compiler.EvaluationStack
{
    public class AllocObjEntry : StackEntry
    {
        public StackEntry EETypeNode { get; }

        public AllocObjEntry(StackEntry eeTypeNode, VarType objType) : base(objType)
        {
            EETypeNode = eeTypeNode;
        }

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);

        public override StackEntry Duplicate()
        {
            return new AllocObjEntry(EETypeNode, Type);
        }
    }
}
