using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    internal class LoadElemImporter : IOpcodeImporter
    {
        public bool CanImport(Code code)
        {
            return code == Code.Ldelem_I4;
        }

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var op1 = importer.PopExpression();
            var op2 = importer.PopExpression();

            importer.PushExpression(new IndexRefEntry(op1, op2, 4, StackValueKind.Int32));
        }
    }
}
