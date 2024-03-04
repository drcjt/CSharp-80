using dnlib.DotNet;
using dnlib.DotNet.Emit;
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
            typeSig = context.Method.ResolveType(typeSig);
            int elemSize = typeSig.GetInstanceFieldSize();

            var size = new Int32ConstantEntry(elemSize);
            var initValue = new Int32ConstantEntry(0);
            var args = new List<StackEntry>() { size, initValue, address };

            var node = new CallEntry("Memset", args, VarType.Void, null);
            importer.ImportAppendTree(node);

            return true;
        }
    }
}
