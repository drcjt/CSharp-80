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
            var arraySizeOffset = new NativeIntConstantEntry(2);
            addr = new BinaryOperator(Operation.Add, isComparison: false, addr, arraySizeOffset, VarType.Ptr);
            var node = new IndirectEntry(addr, VarType.Ptr, 2);
            importer.PushExpression(node);
            return true;
        }
    }
}
