using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    internal class LoadLengthImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.Opcode != ILOpcode.ldlen) return false;

            var addr = importer.PopExpression();
            var node = new ArrayLengthEntry(addr);
            importer.PushExpression(node);
            return true;
        }
    }
}
