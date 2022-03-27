﻿using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class IndirectCodeGenerator : ICodeGenerator<IndirectEntry>
    {
        public void GenerateCode(IndirectEntry entry, CodeGeneratorContext context)
        {
            if (entry.Kind == StackValueKind.Int32 || entry.Kind == StackValueKind.ValueType || entry.Kind == StackValueKind.NativeInt)
            {
                // Save IX to BC
                context.Assembler.Push(I16.IX);
                context.Assembler.Pop(R16.BC);

                // Get indirect address from stack into IX
                context.Assembler.Pop(I16.IX);


                var size = entry.ExactSize ?? 4; // TODO: is 4 the right default size?
                CopyHelper.CopyFromIXToStack(context.Assembler, size, (short)entry.Offset);

                // Restore IX
                context.Assembler.Push(R16.BC);
                context.Assembler.Pop(I16.IX);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
