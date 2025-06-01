using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class LoadVarImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            int index;
            switch (instruction.Opcode)
            {
                case ILOpcode.ldloc_0:
                case ILOpcode.ldloc_1:
                case ILOpcode.ldloc_2:
                case ILOpcode.ldloc_3:
                    index = instruction.Opcode - ILOpcode.ldloc_0;
                    break;

                case ILOpcode.ldloc:
                case ILOpcode.ldloc_s:
                    var localVariableDefinition = (LocalVariableDefinition)instruction.Operand;
                    index = localVariableDefinition.Index;
                    break;

                default:
                    return false;
            }

            var localNumber = importer.ParameterCount + index;
            var localVariable = importer.LocalVariableTable[localNumber];

            if (context.Inlining)
            {
                localNumber = importer.InlineFetchLocal(index);
            }

            var node = new LocalVariableEntry(localNumber, localVariable.Type, localVariable.ExactSize);
            importer.PushExpression(node);

            return true;
        }
    }
}
