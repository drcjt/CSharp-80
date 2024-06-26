﻿using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class PopImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.Opcode != ILOpcode.pop) return false;

            var op1 = importer.PopExpression();

            // Need to spill result removed from stack to a temp that will never be used
            var lclNum = importer.GrabTemp(op1.Type, op1.ExactSize);
            var node = new StoreLocalVariableEntry(lclNum, false, op1);

            // ctor has no return type so just append the tree
            importer.ImportAppendTree(node);

            return true;
        }
    }
}