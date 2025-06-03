using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    internal class LoadLengthImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.ldlen) return false;

            var addr = importer.Pop();
            var node = new ArrayLengthEntry(addr);
            importer.Push(node);
            return true;
        }
    }
}
