﻿using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class FieldAddressCodeGenerator : ICodeGenerator<FieldAddressEntry>
    {
        public void GenerateCode(FieldAddressEntry entry, CodeGeneratorContext context)
        {
            var fieldOffset = entry.Offset;

            // Get address of object
            context.InstructionsBuilder.Pop(HL);      // LSW

            // Calculate field address
            CodeGeneratorHelper.AddHLFromDE(context.InstructionsBuilder, (short)fieldOffset);

            // Push field address onto the stack msw first, lsw second
            context.InstructionsBuilder.Ld(DE, 0);
            context.InstructionsBuilder.Push(HL);
        }
    }
}
