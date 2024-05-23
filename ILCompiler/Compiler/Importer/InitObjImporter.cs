using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class InitobjImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code != Code.Initobj) return false;

            var address = importer.PopExpression();

            var typeSig = (instruction.Operand as ITypeDefOrRef).ToTypeSig();
            var typeDesc = context.TypeSystemContext.Create(typeSig, context.Method.Instantiation);
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
