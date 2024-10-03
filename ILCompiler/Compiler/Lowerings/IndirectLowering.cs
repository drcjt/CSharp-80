using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.Lowerings
{
    internal class IndirectLowering : ILowering<IndirectEntry>
    {
        public StackEntry? Lower(IndirectEntry entry, BasicBlock block, LocalVariableTable locals)
        {
            if (entry.Op1 is LocalVariableAddressEntry lvaAddress)
            {
                // Mark as contained so calculation of local variable address
                // can be combined with indirect offset and size
                lvaAddress.Contained = true;
            }

            return null;
        }
    }
}
