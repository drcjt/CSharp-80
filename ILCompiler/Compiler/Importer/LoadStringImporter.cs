using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadStringImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Ldstr;

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            importer.PushExpression(new StringConstantEntry(instruction.OperandAs<string>()));
        }
    }
}
