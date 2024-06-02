using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class NewarrImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code != Code.Newarr) return false;

            var op2 = importer.PopExpression();

            var elemTypeDesc = context.Module.Create((ITypeDefOrRef)instruction.Operand, context.Method.Instantiation);
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
