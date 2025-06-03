using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class CpblkImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.cpblk) return false;

            var size = importer.Pop(); // Size (unsigned int 32)
            var sourceAddress = importer.Pop(); // Source address (native int or &)
            var destinationAddress = importer.Pop(); // Destination address (native int or &)

            var args = new List<StackEntry>() { size, sourceAddress, destinationAddress };

            var node = new CallEntry("Memcpy", args, VarType.Void, null);
            importer.ImportAppendTree(node);

            return true;
        }
    }
}
