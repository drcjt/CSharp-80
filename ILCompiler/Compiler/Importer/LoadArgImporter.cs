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
            var lclNum = MapIlArgNum(index, importer.ReturnBufferArgIndex);

            var argument = importer.LocalVariableTable[lclNum];
            var node = new LocalVariableEntry(lclNum, argument.Kind, argument.ExactSize);
            importer.PushExpression(node);
        }

        /// <summary>
        /// Map IL arg num to account for hidden parameters
        /// </summary>
        /// <param name="ilArgNum"></param>
        /// <returns></returns>
        private int MapIlArgNum(int ilArgNum, int? returnBufferArgIndex)
        {
            if (returnBufferArgIndex.HasValue)
            {
                if (ilArgNum >= returnBufferArgIndex)
                {
                    ilArgNum++;
                }
            }

            return ilArgNum;
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
