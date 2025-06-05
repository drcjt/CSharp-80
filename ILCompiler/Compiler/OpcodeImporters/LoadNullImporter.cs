using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class LoadNullImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.ldnull) return false;

            var node = new NativeIntConstantEntry(0, VarType.Ref);
            importer.Push(node);

            return true;
        }
    }
}
