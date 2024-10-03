namespace ILCompiler.Compiler.EvaluationStack
{
    public class AllocObjEntry : StackEntry
    {
        public int Size { get; }
        public string MangledEETypeName { get; }

        public AllocObjEntry(string mangledEETypeName, int objSize, VarType objType) : base(objType)
        {
            MangledEETypeName = mangledEETypeName;
            Size = objSize;
        }

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);

        public override StackEntry Duplicate()
        {
            return new AllocObjEntry(MangledEETypeName, Size, Type);
        }
    }
}
