using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    internal class LoadLengthImporter : SingleOpcodeImporter
    {
        protected override Code Code { get; } = Code.Ldlen;

        protected override void ImportOpcode(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var op1 = importer.PopExpression();
            var node = new IndirectEntry(op1, VarType.UShort, 2);
            importer.PushExpression(node);
        }
    }
}
