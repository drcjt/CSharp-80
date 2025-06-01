using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class StoreVarImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            int index;
            switch (instruction.Opcode)
            {
                case ILOpcode.stloc_0:
                case ILOpcode.stloc_1:
                case ILOpcode.stloc_2:
                case ILOpcode.stloc_3:
                    index = instruction.Opcode - ILOpcode.stloc_0;
                    break;
                case ILOpcode.stloc:
                case ILOpcode.stloc_s:
                    var localVariableDefinition = (LocalVariableDefinition)instruction.Operand;
                    index = localVariableDefinition.Index;
                    break;

                default:
                    return false;
            }

            var localNumber = importer.ParameterCount + index;

            if (context.Inlining)
            {
                localNumber = importer.InlineFetchLocal(index);
            }

            var value = importer.PopExpression();

            var node = new StoreLocalVariableEntry(localNumber, false, value);
            importer.ImportAppendTree(node, true);

            return true;
        }
    }
}