using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class LoadArgImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IImporter importer)
        {
            int index;
            switch (instruction.Opcode)
            {
                case ILOpcode.ldarg_0:
                case ILOpcode.ldarg_1:
                case ILOpcode.ldarg_2:
                case ILOpcode.ldarg_3:
                    index = instruction.Opcode - ILOpcode.ldarg_0;
                    break;

                case ILOpcode.ldarg:
                case ILOpcode.ldarg_s:
                    var parameter = (ParameterDefinition)instruction.Operand;
                    index = parameter.Index;
                    break;

                default:
                    return false;
            }

            if (context.Inlining)
            {
                // Need to get arg from inline info.
                var node = importer.InlineFetchArgument(index);
                importer.Push(node);
            }
            else
            {
                var lclNum = MapIlArgNum(index, importer.ReturnBufferArgIndex);

                var argument = importer.LocalVariableTable[lclNum];
                var node = new LocalVariableEntry(lclNum, argument.Type, argument.ExactSize);
                importer.Push(node);
            }

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
