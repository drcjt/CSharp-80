﻿using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class StoreVarImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            int index;
            switch (instruction.Opcode)
            {
                case ILOpcode.stloc_0:
                case ILOpcode.stloc_1:
                case ILOpcode.stloc_2:
                case ILOpcode.stloc_3:
                    index = instruction.Opcode - ILOpcode.stloc_0;
                    break;
                case ILOpcode.stloc:
                case ILOpcode.stloc_s:
                    var localVariableDefinition = (LocalVariableDefinition)instruction.Operand;
                    index = localVariableDefinition.Index;
                    break;

                default:
                    return false;
            }

            var localNumber = index;

            if (importer.Inlining)
            {
                localNumber = importer.InlineFetchLocal(localNumber);
            }
            else
            {
                localNumber += importer.ParameterCount;
            }

            var value = importer.Pop();

            StackEntry node = importer.NewTempStore(localNumber, value);
            importer.ImportAppendTree(node, true);

            return true;
        }
    }
}