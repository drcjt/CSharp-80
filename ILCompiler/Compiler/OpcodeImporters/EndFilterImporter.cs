using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class EndFilterImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.endfilter) return false;

            var returnValue = importer.Pop();

            ReturnEntry returnEntry = new(returnValue, isFilterReturn: true);
            importer.ImportAppendTree(returnEntry);

            return true;
        }
    }
}
