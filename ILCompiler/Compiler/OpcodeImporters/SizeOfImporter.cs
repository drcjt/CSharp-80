using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    internal class SizeOfImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.sizeof_) return false;

            var typeDesc = (TypeDesc)instruction.Operand;

            int elemSize = typeDesc.GetElementSize().AsInt;

            importer.Push(new Int32ConstantEntry(checked(elemSize)));

            return true;
        }
    }
}