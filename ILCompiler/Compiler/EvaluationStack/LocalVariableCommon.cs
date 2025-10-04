namespace ILCompiler.Compiler.EvaluationStack
{
    public abstract class LocalVariableCommon(VarType type, int? exactSize = null) : StackEntry(type, exactSize)
    {
        public int LocalNumber { get; init; }
        public int SsaNumber { get; set; }

        public StackEntry Data => ((StoreLocalVariableEntry)this).Op1;
    }
}
