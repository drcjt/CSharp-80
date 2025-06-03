using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class InitblkImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.initblk) return false;

            var size = importer.Pop(); // Size (unsigned int 32)
            var initValue = importer.Pop(); // Value (unsigned int 8)
            var destinationAddress = importer.Pop(); // Destination address (native int or &)

            var args = new List<StackEntry>() { size, initValue, destinationAddress };

            var node = new CallEntry("Memset", args, VarType.Void, null);
            importer.ImportAppendTree(node);

            return true;
        }
    }
}
