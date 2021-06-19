namespace ILCompiler.Compiler.EvaluationStack
{
    // STOREIND
    public class IndEntry : StackEntry
    {
        public IndEntry(StackEntry addr) : base(addr.Kind)
        {
        }
    }
}
