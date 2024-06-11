using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class StoreArgImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            switch (instruction.Opcode)
            {
                case ILOpcode.starg:
                case ILOpcode.starg_s:
                    var value = importer.PopExpression();
                    var parameter = (ParameterDefinition)instruction.GetOperand();
                    var node = new StoreLocalVariableEntry(parameter.Index, true, value);
                    importer.ImportAppendTree(node);
                    return true;

                default:
                    return false;
            }
        }
    }
}