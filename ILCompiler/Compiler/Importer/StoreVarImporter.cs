using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class StoreVarImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            int index;
            switch (instruction.OpCode.Code)
            {
                case Code.Stloc_0:
                case Code.Stloc_1:
                case Code.Stloc_2:
                case Code.Stloc_3:
                    index = instruction.OpCode.Code - Code.Stloc_0;
                    break;
                case Code.Stloc:
                case Code.Stloc_S:
                    index = (instruction.OperandAs<Local>()).Index;
                    break;

                default:
                    return false;
            }

            var value = importer.PopExpression();

            var localNumber = importer.ParameterCount + index;
            var localVariable = importer.LocalVariableTable[localNumber];
            var node = new StoreLocalVariableEntry(localNumber, false, value);
            importer.ImportAppendTree(node, true);

            return true;
        }
    }
}
