using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class StoreArgImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            switch (instruction.OpCode.Code)
            {
                case Code.Starg:
                case Code.Starg_S:
                    var value = importer.PopExpression();
                    var node = new StoreLocalVariableEntry((instruction.OperandAs<Parameter>()).Index, true, value);
                    importer.ImportAppendTree(node);
                    return true;

                default:
                    return false;
            }
        }
    }
}
