using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class StoreArgImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Starg || code == Code.Starg_S;

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var value = importer.PopExpression();
            var node = new StoreLocalVariableEntry((instruction.OperandAs<Parameter>()).Index, true, value);
            importer.ImportAppendTree(node);
        }
    }
}
