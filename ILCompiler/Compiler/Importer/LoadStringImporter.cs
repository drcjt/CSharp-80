using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadStringImporter : SingleOpcodeImporter
    {
        protected override Code Code { get; } = Code.Ldstr;

        protected override void ImportOpcode(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            importer.PushExpression(new StringConstantEntry(instruction.OperandAs<string>()));
        }
    }
}
