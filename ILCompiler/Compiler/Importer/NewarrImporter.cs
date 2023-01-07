using dnlib.DotNet;
using dnlib.DotNet.Emit;
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

            var typeSig = (instruction.Operand as ITypeDefOrRef).ToTypeSig();
            typeSig = context.Method.ResolveType(typeSig);
            var arrayElementSize = typeSig.GetInstanceFieldSize();

            // Instead of creating new node type specifically for arrays
            // could leverage existing CallEntry node to call arbitrary helper functions
            // can use this then for other helper functions too

            var args = new List<StackEntry>() { op2, new Int32ConstantEntry(arrayElementSize) };
            var node = new CallEntry("NewArr", args, VarType.Ref, 2);
            importer.PushExpression(node);

            return true;
        }
    }
}
