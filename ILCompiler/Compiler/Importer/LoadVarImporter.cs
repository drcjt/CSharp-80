using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadVarImporter : IOpcodeImporter
    {
        private readonly IILImporter _importer;
        public LoadVarImporter(IILImporter importer)
        {
            _importer = importer;
        }
        
        public bool CanImport(Code opcode)
        {
            return opcode == Code.Ldloc ||
                   opcode == Code.Ldloc_S ||
                   opcode == Code.Ldloc_0 ||
                   opcode == Code.Ldloc_1 ||
                   opcode == Code.Ldloc_2 ||
                   opcode == Code.Ldloc_3;
        }

        public void Import(Instruction instruction, ImportContext context)
        {
            var index = 0;
            switch (instruction.OpCode.Code)
            {
                case Code.Ldloc:
                case Code.Ldloc_S:
                    index = (instruction.Operand as Local).Index;
                    break;

                default:
                    index = instruction.OpCode.Code - Code.Ldloc_0;
                    break;
            }

            var localNumber = _importer.ParameterCount + index;
            var localVariable = _importer.LocalVariableTable[localNumber];
            var node = new LocalVariableEntry(localNumber, localVariable.Kind);
            _importer.PushExpression(node);
        }
    }
}
