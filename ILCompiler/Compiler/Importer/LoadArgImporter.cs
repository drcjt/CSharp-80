using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadArgImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            int index;
            switch (instruction.OpCode.Code)
            {
                case Code.Ldarg_0:
                case Code.Ldarg_1:
                case Code.Ldarg_2:
                case Code.Ldarg_3:
                    index = instruction.OpCode.Code - Code.Ldarg_0;
                    break;

                case Code.Ldarg:
                case Code.Ldarg_S:
                    index = (instruction.OperandAs<Parameter>()).Index;
                    break;

                default:
                    return false;
            }

            var lclNum = MapIlArgNum(index, importer.ReturnBufferArgIndex);

            var argument = importer.LocalVariableTable[lclNum];
            var node = new LocalVariableEntry(lclNum, argument.Type, argument.ExactSize);
            importer.PushExpression(node);

            return true;
        }

        /// <summary>
        /// Map IL arg num to account for hidden parameters
        /// </summary>
        /// <param name="ilArgNum"></param>
        /// <returns></returns>
        private static int MapIlArgNum(int ilArgNum, int? returnBufferArgIndex)
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
    }
}
