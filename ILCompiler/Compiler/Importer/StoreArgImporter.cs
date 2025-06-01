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
                    var parameter = (ParameterDefinition)instruction.Operand;
                    int localNumber = parameter.Index;

                    if (context.Inlining)
                    {
                        var node = importer.InlineFetchArgument(parameter.Index);
                        localNumber = ((LocalVariableEntry)node).LocalNumber;
                    }

                    var value = importer.PopExpression();
                    var store = new StoreLocalVariableEntry(localNumber, true, value);
                    importer.ImportAppendTree(store);

                    return true;

                default:
                    return false;
            }
        }
    }
}