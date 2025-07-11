﻿using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class PopImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.pop) return false;

            var op1 = importer.Pop();

            // Need to spill result removed from stack to a temp that will never be used
            var localNumber = importer.GrabTemp(op1.Type, op1.ExactSize);
            StackEntry store = new StoreLocalVariableEntry(localNumber, false, op1);
            importer.ImportAppendTree(store);

            return true;
        }
    }
}