using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class NegImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Neg;

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            importer.PushExpression(new UnaryOperator(Operation.Neg, importer.PopExpression()));
        }
    }
}
