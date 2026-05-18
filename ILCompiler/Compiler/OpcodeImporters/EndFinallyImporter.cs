using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class EndFinallyImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.endfinally) return false;

            ReturnEntry returnEntry = new(returnValue: null, isFinallyReturn: true);
            importer.ImportAppendTree(returnEntry);

            return true;
        }
    }
}
