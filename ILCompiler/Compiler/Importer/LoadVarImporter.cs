using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadVarImporter : IOpcodeImporter
    {
        public bool CanImport(Code code)
        {
            return code == Code.Ldloc ||
                   code == Code.Ldloc_S ||
                   code == Code.Ldloc_0 ||
                   code == Code.Ldloc_1 ||
                   code == Code.Ldloc_2 ||
                   code == Code.Ldloc_3;
        }

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            int index = GetIndex(instruction);
            var localNumber = importer.ParameterCount + index;
            var localVariable = importer.LocalVariableTable[localNumber];
            var node = new LocalVariableEntry(localNumber, localVariable.Kind, localVariable.ExactSize);
            node.Type = localVariable.Type;
            importer.PushExpression(node);
        }

        private static int GetIndex(Instruction instruction)
        {
            int index = instruction.OpCode.Code switch
            {
                Code.Ldloc or Code.Ldloc_S => (instruction.OperandAs<Local>()).Index,
                _ => instruction.OpCode.Code - Code.Ldloc_0,
            };
            return index;
        }
    }
}
