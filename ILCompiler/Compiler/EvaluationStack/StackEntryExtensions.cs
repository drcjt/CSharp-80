namespace ILCompiler.Compiler.EvaluationStack
{
    public static class StackEntryExtensions
    {
        public static bool IsIntCns(this StackEntry entry) => entry is Int32ConstantEntry;

        public static bool IsContainedInt(this StackEntry entry) => entry.Contained && entry.IsIntCns();

        public static bool IsIntegralConstant(this StackEntry entry, int constantValue)
        {
            if (entry is Int32ConstantEntry && entry.As<Int32ConstantEntry>().Value == constantValue)
            {
                return true;
            }
            if (entry is NativeIntConstantEntry && entry.As<NativeIntConstantEntry>().Value == constantValue)
            {
                return true;
            }
            return false;
        }
    }
}
