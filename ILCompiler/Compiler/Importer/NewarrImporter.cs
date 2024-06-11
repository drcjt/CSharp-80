using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class NewarrImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.Opcode != ILOpcode.newarr) return false;

            var op2 = importer.PopExpression();

            var elemTypeDesc = (TypeDesc)instruction.GetOperandAs<TypeDesc>();
            var arrayType = new ArrayType(elemTypeDesc, -1);

            var arrayElementSize = elemTypeDesc.GetElementSize().AsInt;

            var mangledEETypeName = context.NameMangler.GetMangledTypeName(arrayType);
            var eeTypeNode = new NativeIntConstantEntry(mangledEETypeName);

            var args = new List<StackEntry>() { op2, new Int32ConstantEntry(arrayElementSize), eeTypeNode };
            var node = new CallEntry("NewArray", args, VarType.Ref, 2);
            importer.PushExpression(node);

            return true;
        }
    }
}