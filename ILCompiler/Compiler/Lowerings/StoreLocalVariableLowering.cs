using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.Lowerings
{
    internal class StoreLocalVariableLowering : ILowering<StoreLocalVariableEntry>
    {
        public StackEntry? Lower(StoreLocalVariableEntry entry)
        {
            if (entry.Op1.IsIntCnsOrI())
            {
                // When storing constant mark as contained as we can embed the constant
                // into the instructions to store to the local variable directly
                entry.Op1.Contained = true;
            }

            return null;
        }
    }
}
