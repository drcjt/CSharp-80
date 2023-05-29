namespace ILCompiler.Compiler.EvaluationStack
{
    public class AllocObjEntry : StackEntry
    {
        public int Size { get; }
        public string MangledEETypeName { get; }

        public AllocObjEntry(string mangledEETypeName, VarType objType) : base(objType)
        {
            MangledEETypeName = mangledEETypeName;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override StackEntry Duplicate()
        {
            return new AllocObjEntry(MangledEETypeName, Type);
        }
    }
}
