using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadVarImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            int index;
            switch (instruction.OpCode.Code)
            {
                case Code.Ldloc_0:
                case Code.Ldloc_1:
                case Code.Ldloc_2:
                case Code.Ldloc_3:
                    index = instruction.OpCode.Code - Code.Ldloc_0;
                    break;

                case Code.Ldloc:
                case Code.Ldloc_S:
                    index = (instruction.OperandAs<Local>()).Index;
                    break;

                default:
                    return false;
            }

            var localNumber = importer.ParameterCount + index;
            var localVariable = importer.LocalVariableTable[localNumber];
            var node = new LocalVariableEntry(localNumber, localVariable.Type, localVariable.ExactSize);
            importer.PushExpression(node);

            return true;
        }
    }
}
