using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class NewarrImporter : SingleOpcodeImporter
    {
        protected override Code Code { get; } = Code.Newarr;

        protected override void ImportOpcode(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var op2 = importer.PopExpression();

            var typeSig = (instruction.Operand as ITypeDefOrRef).ToTypeSig();
            var arrayElementSize = typeSig.GetExactSize();

            // Instead of creating new node type specifically for arrays
            // could leverage existing CallEntry node to call arbitrary helper functions
            // can use this then for other helper functions too

            var args = new List<StackEntry>() { op2, new Int32ConstantEntry(arrayElementSize) };
            var node = new CallEntry("NewArr", args, VarType.Ref, 2);
            importer.PushExpression(node);
        }
    }
}
