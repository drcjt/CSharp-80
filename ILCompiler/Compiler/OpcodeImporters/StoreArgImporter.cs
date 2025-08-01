﻿using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class StoreArgImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            switch (instruction.Opcode)
            {
                case ILOpcode.starg:
                case ILOpcode.starg_s:
                    var parameter = (ParameterDefinition)instruction.Operand;
                    var localNumber = importer.MapIlArgNum(parameter.Index);

                    if (importer.Inlining)
                    {
                        var node = importer.InlineFetchArgument(parameter.Index);
                        localNumber = ((LocalVariableEntry)node).LocalNumber;
                    }

                    var value = importer.Pop();
                    var store = new StoreLocalVariableEntry(localNumber, true, value);
                    importer.ImportAppendTree(store);

                    return true;

                default:
                    return false;
            }
        }
    }
}