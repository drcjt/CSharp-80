using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class InitblkImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.Opcode != ILOpcode.initblk) return false;

            var size = importer.PopExpression(); // Size (unsigned int 32)
            var initValue = importer.PopExpression(); // Value (unsigned int 8)
            var destinationAddress = importer.PopExpression(); // Destination address (native int or &)

            var args = new List<StackEntry>() { size, initValue, destinationAddress };

            var node = new CallEntry("Memset", args, VarType.Void, null);
            importer.ImportAppendTree(node);

            return true;
        }
    }
}
