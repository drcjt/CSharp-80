using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class LoadArgAddressImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            switch (instruction.Opcode)
            {
                case ILOpcode.ldarga:
                case ILOpcode.ldarga_s:
                    var parameter = (ParameterDefinition)instruction.Operand;
                    var index = parameter.Index;
                    var localNumber = MapIlArgNum(index, importer.ReturnBufferArgIndex);

                    importer.PushExpression(new LocalVariableAddressEntry(localNumber));

                    importer.LocalVariableTable[localNumber].AddressExposed = true;

                    return true;

                default:
                    return false;
            }
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