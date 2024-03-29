﻿using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class StoreIndCodeGenerator : ICodeGenerator<StoreIndEntry>
    {
        public void GenerateCode(StoreIndEntry entry, CodeGeneratorContext context)
        {
            var exactSize = entry.ExactSize ?? 0;
            if (exactSize > 0)
            {
                context.InstructionsBuilder.Pop(HL);        // Address to store to
                short offset = (short)entry.FieldOffset;

                if (entry.Type.IsSmall())
                {
                    CopyHelper.CopyStackToHLSmall(context.InstructionsBuilder, exactSize, offset);
                }
                else
                {
                    CopyHelper.CopyFromStackToHL(context.InstructionsBuilder, exactSize, offset);
                }
            }
        }
    }
}
