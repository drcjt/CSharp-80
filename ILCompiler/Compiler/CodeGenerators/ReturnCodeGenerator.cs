﻿using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class ReturnCodeGenerator : ICodeGenerator<ReturnEntry>
    {
        private static string GetEpilogLabel(CodeGeneratorContext context) => $"{context.NameMangler.GetMangledMethodName(context.Method)}_EPILOG";

        public void GenerateCode(ReturnEntry entry, CodeGeneratorContext context)
        {
            var epilogLabel = GetEpilogLabel(context);
            if (context.GeneratedEpilog)
            {
                // Don't generate more than one epilog just jump to
                // previously generate epilog
                context.InstructionsBuilder.Jp(epilogLabel);
                return;
            }

            context.GeneratedEpilog = true;
            context.InstructionsBuilder.Label(epilogLabel);

            var targetType = entry.Return;
            var hasReturnValue = targetType != null && targetType.Type != VarType.Void;

            if (hasReturnValue)
            {
                context.InstructionsBuilder.Pop(DE);            // Copy return value into DE/DE'

                if (targetType?.Type.IsInt() ?? false)
                {
                    context.InstructionsBuilder.Exx();
                    context.InstructionsBuilder.Pop(DE);
                    context.InstructionsBuilder.Exx();
                }
            }

            // Unwind stack frame
            var localsSize = 0;
            var tempCount = 0;
            foreach (var localVariable in context.LocalVariableTable)
            {
                if (localVariable.IsTemp)
                {
                    tempCount++;
                }
                if (!localVariable.IsParameter)
                {
                    localsSize += localVariable.ExactSize;
                }
            }

            if (context.LocalsCount + tempCount > 0)
            {
                if (context.Configuration.IntegrationTests && !context.Method.LocallocUsed)
                {
                    // Validate we haven't got any under/over flow on stack
                    // Assert that localsSize + SP - IX = 0

                    // TODO: also need to factor in any localloc space allocated
                    // this is why localloc integration test fails with this code enabled

                    context.InstructionsBuilder.Ld(HL, (short)localsSize);
                    context.InstructionsBuilder.Add(HL, SP);

                    context.InstructionsBuilder.Push(IX);
                    context.InstructionsBuilder.Pop(BC);

                    context.InstructionsBuilder.Sbc(HL, BC);

                    var unwindLabel = context.NameMangler.GetUniqueName();
                    context.InstructionsBuilder.Jp(Condition.Z, unwindLabel);

                    context.InstructionsBuilder.Halt();

                    context.InstructionsBuilder.Label(unwindLabel);
                }

                context.InstructionsBuilder.Ld(SP, IX);     // Move SP to before locals
            }
            context.InstructionsBuilder.Pop(IX);            // Remove IX

            context.InstructionsBuilder.Pop(BC);      // Store return address in BC

            // Calculate size of parameters
            var totalParametersSize = 0;
            foreach (var local in context.LocalVariableTable)
            {
                if (local.IsParameter)
                {
                    totalParametersSize += local.ExactSize;
                }
            }

            if (totalParametersSize > 0)
            {
                // Remove parameters from stack
                CodeGeneratorHelper.AddSPFromHL(context.InstructionsBuilder, (short)(totalParametersSize));
            }

            if (hasReturnValue)
            {
                if (targetType?.Type.IsInt() ?? false)
                {
                    context.InstructionsBuilder.Exx();
                    context.InstructionsBuilder.Push(DE);
                    context.InstructionsBuilder.Exx();
                }
                context.InstructionsBuilder.Push(DE);
            }

            // Could do this instead - it's a bit faster, but 1 byte longer
            //context.InstructionsBuilder.Ld(H, B);
            //context.InstructionsBuilder.Ld(L, C);
            //context.InstructionsBuilder.Jp(__[HL]);

            context.InstructionsBuilder.Push(BC);
            context.InstructionsBuilder.Ret();
        }
    }
}
