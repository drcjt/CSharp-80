using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadArgImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => IsLdArg(code) || IsLdArgN(code);

        private static bool IsLdArg(Code code) => code == Code.Ldarg || code == Code.Ldarg_S;
        private static bool IsLdArgN(Code code) => code == Code.Ldarg_0 || code == Code.Ldarg_1 || code == Code.Ldarg_2 || code == Code.Ldarg_3;
        

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            var index = GetIndex(instruction);
            var argument = importer.LocalVariableTable[index];
            var node = new LocalVariableEntry(index, argument.Kind);
            importer.PushExpression(node);
        }

        private int GetIndex(Instruction instruction)
        {
            var code = instruction.OpCode.Code;
            int index = code - Code.Ldarg_0;
            if (IsLdArg(code))
            {
                index = (instruction.Operand as Parameter).Index;
            }

            return index;
        }
    }
}
