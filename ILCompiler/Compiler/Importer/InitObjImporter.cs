using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class InitobjImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.Opcode != ILOpcode.initobj) return false;

            var address = importer.PopExpression();

            // TODO: Support shared generics here

            var typeDesc = (TypeDesc)instruction.Operand;
            int elemSize = typeDesc.GetElementSize().AsInt;

            var size = new Int32ConstantEntry(elemSize);
            var initValue = new Int32ConstantEntry(0);
            var args = new List<StackEntry>() { size, initValue, address };

            var node = new CallEntry("Memset", args, VarType.Void, null);
            importer.ImportAppendTree(node);

            return true;
        }
    }
}
