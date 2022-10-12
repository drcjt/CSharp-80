using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class StoreVarImporter : IOpcodeImporter
    {
        public bool CanImport(Code code)
        {
            return code == Code.Stloc ||
                   code == Code.Stloc_S ||
                   code == Code.Stloc_0 ||
                   code == Code.Stloc_1 ||
                   code == Code.Stloc_2 ||
                   code == Code.Stloc_3;
        }

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            int index = GetIndex(instruction);

            var value = importer.PopExpression();

            var localNumber = importer.ParameterCount + index;
            var localVariable = importer.LocalVariableTable[localNumber];
            var node = new StoreLocalVariableEntry(localNumber, false, value);
            importer.ImportAppendTree(node, true);
        }

        private static int GetIndex(Instruction instruction)
        {
            var index = instruction.OpCode.Code switch
            {
                Code.Stloc or Code.Stloc_S => (instruction.OperandAs<Local>()).Index,
                _ => instruction.OpCode.Code - Code.Stloc_0,
            };
            return index;
        }
    }
}
