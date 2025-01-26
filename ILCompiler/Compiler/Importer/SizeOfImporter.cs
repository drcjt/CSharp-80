using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    internal class SizeOfImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.Opcode != ILOpcode.sizeof_) return false;

            var typeDesc = (TypeDesc)instruction.Operand;

            var elemType = typeDesc.VarType;
            int elemSize = typeDesc.GetElementSize().AsInt;

            importer.PushExpression(new Int32ConstantEntry(checked((int)elemSize)));

            return true;
        }
    }
}