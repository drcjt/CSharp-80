using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class CpblkImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code != Code.Cpblk) return false;

            var size = importer.PopExpression(); // Size (unsigned int 32)
            var sourceAddress = importer.PopExpression(); // Source address (native int or &)
            var destinationAddress = importer.PopExpression(); // Destination address (native int or &)

            var args = new List<StackEntry>() { size, sourceAddress, destinationAddress };

            var node = new CallEntry("Memcpy", args, VarType.Void, null);
            importer.ImportAppendTree(node);

            return true;
        }
    }
}
