using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.LinearIR;

namespace ILCompiler.Compiler.Lowerings
{
    internal class ArrayLengthLowering : ILowering<ArrayLengthEntry>
    {
        public StackEntry? Lower(ArrayLengthEntry entry, BasicBlock block, LocalVariableTable locals)
        {
            var blockRange = block;
            if (!blockRange.TryGetUse(entry, out Use? use))
            {
                return null;
            }

            // Create the expression "*(array_address + length_offset)"
            var arraySizeOffset = new NativeIntConstantEntry(2);
            var addr = new BinaryOperator(Operation.Add, isComparison: false, entry.ArrayReference, arraySizeOffset, VarType.Ptr);
            var node = new IndirectEntry(addr, VarType.Ptr, 2);

            // Insert new nodes after existing ArrayLengthEntry
            blockRange.InsertAfter(entry.ArrayReference, arraySizeOffset, addr, node);

            // Remove the original ArrayLengthEntry
            blockRange.Remove(entry);

            // Replace the use of the original ArrayLengthEntry with the new node
            use.ReplaceWith(node);

            return node;
        }
    }
}