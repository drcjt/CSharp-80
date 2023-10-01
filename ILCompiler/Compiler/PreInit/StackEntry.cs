namespace ILCompiler.Compiler.PreInit
{
    public class StackEntry
    {
        public readonly StackValueKind ValueKind;
        public readonly Value Value;

        public StackEntry(StackValueKind valueKind, Value value)
        {
            ValueKind = valueKind;
            Value = value;
        }
    }
}
