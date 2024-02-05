using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadStringImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code != Code.Ldstr) return false;

            var node = context.NodeFactory.SerializedStringObject(instruction.OperandAs<string>(), context.CorLibModuleProvider);

            importer.PushExpression(new ExpressionEntry(VarType.Ptr, node.Label));

            return true;
        }
    }
}
