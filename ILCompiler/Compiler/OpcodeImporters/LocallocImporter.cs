﻿using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class LocallocImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.localloc) return false;

            var op2 = importer.Pop();
            if (op2.Type != VarType.Ptr)
            {
                // Insert cast from int32 to nativeint as cannot localloc more
                // space than the processor can address :)
                var cast = new CastEntry(op2, VarType.Ptr);
                op2 = CodeFolder.FoldExpression(cast);
            }

            if (op2.Type != VarType.Ptr)
            {
                throw new NotSupportedException("Localloc requires native int size");
            }

            var op1 = new LocalHeapEntry(op2);

            importer.Push(op1);

            // The frame pointer may not be back to the original value at the end of the method
            // even if the frame size is 0 as localloc may have modified it so we will have to
            // reset it.
            importer.Method.LocallocUsed = true;

            return true;
        }
    }
}
