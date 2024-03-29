﻿using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadArgAddressImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            switch (instruction.OpCode.Code)
            {
                case Code.Ldarga:
                case Code.Ldarga_S:
                    var index = (instruction.OperandAs<Parameter>()).Index;
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
