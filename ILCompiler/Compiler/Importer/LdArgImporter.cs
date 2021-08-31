using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LdArgImporter : IOpcodeImporter
    {
        private readonly IILImporter _importer;

        public LdArgImporter(IILImporter importer)
        {
            _importer = importer;
        }

        public bool CanImport(Code opcode)
        {
            return opcode == Code.Ldarg ||
                   opcode == Code.Ldarg_S ||
                   opcode == Code.Ldarg_0 ||
                   opcode == Code.Ldarg_1 ||
                   opcode == Code.Ldarg_2 ||
                   opcode == Code.Ldarg_3;
        }

        public void Import(Instruction instruction, ImportContext context)
        {
            var opcode = instruction.OpCode.Code;
            int index = opcode - Code.Ldarg_0;
            switch (opcode)
            {
                case Code.Ldarg:
                case Code.Ldarg_S:
                    var parameter = instruction.Operand as Parameter;
                    index = parameter.Index;
                    break;                
            }            

            var argument = _importer.LocalVariableTable[index];
            var node = new LocalVariableEntry(index, argument.Kind);
            _importer.PushExpression(node);
        }
    }
}
