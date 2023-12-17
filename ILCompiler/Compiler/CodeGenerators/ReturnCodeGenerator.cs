using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class ReturnCodeGenerator : ICodeGenerator<ReturnEntry>
    {
        public void GenerateCode(ReturnEntry entry, CodeGeneratorContext context)
        {
            var targetType = entry.Return;
            var hasReturnValue = targetType != null && targetType.Type != VarType.Void;

            if (hasReturnValue)
            {
                if (entry.ReturnBufferArgIndex.HasValue)
                {
                    // Returning a struct

                    // Load address of return buffer into HL
                    var variable = context.LocalVariableTable[entry.ReturnBufferArgIndex.Value];
                    context.InstructionsBuilder.Ld(H, __[IX + (short)-(variable.StackOffset - 1)]);
                    context.InstructionsBuilder.Ld(L, __[IX + (short)-(variable.StackOffset - 0)]);

                    context.InstructionsBuilder.Push(IX); // save IX to BC
                    context.InstructionsBuilder.Pop(BC);

                    context.InstructionsBuilder.Push(HL); // Move HL to IX
                    context.InstructionsBuilder.Pop(IX);

                    // Copy struct to the return buffer
                    var returnTypeExactSize = entry.ReturnTypeExactSize ?? 0;
                    CopyHelper.CopyFromStackToIX(context.InstructionsBuilder, returnTypeExactSize);

                    context.InstructionsBuilder.Push(BC); // restore IX
                    context.InstructionsBuilder.Pop(IX);
                }
                else
                {
                    context.InstructionsBuilder.Pop(DE);            // Copy return value into DE/DE'

                    if (targetType?.Type.IsInt() ?? false)
                    {
                        context.InstructionsBuilder.Exx();
                        context.InstructionsBuilder.Pop(DE);
                        context.InstructionsBuilder.Exx();
                    }
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

            if (context.ParamsCount > 0 || (context.LocalsCount + tempCount) > 0 || context.Configuration.ExceptionSupport)
            {
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

                if (context.ParamsCount > 0)
                {
                    // Calculate size of parameters
                    var totalParametersSize = 0;
                    foreach (var local in context.LocalVariableTable)
                    {
                        if (local.IsParameter)
                        {
                            totalParametersSize += local.ExactSize;
                        }
                    }

                    // TODO: consider optimising simple cases to just use Pop to remove the parameters.
                    // will probably be better for 1 or maybe 2 32bit parameters.

                    // Work out start of params so we can reset SP after removing return address
                    context.InstructionsBuilder.Ld(HL, 0);
                    context.InstructionsBuilder.Add(HL, SP);
                    context.InstructionsBuilder.Ld(BC, (short)(2 + totalParametersSize));
                    context.InstructionsBuilder.Add(HL, BC);
                }

                context.InstructionsBuilder.Pop(BC);      // Store return address in BC

                if (context.ParamsCount > 0)
                {
                    // Remove parameters from stack
                    context.InstructionsBuilder.Ld(SP, HL);
                }
            }
            else
            {
                context.InstructionsBuilder.Pop(BC);      // Store return address in BC
            }

            if (hasReturnValue && !entry.ReturnBufferArgIndex.HasValue)
            {
                if (targetType?.Type.IsInt() ?? false)
                {
                    context.InstructionsBuilder.Exx();
                    context.InstructionsBuilder.Push(DE);
                    context.InstructionsBuilder.Exx();
                }
                context.InstructionsBuilder.Push(DE);
            }

            context.InstructionsBuilder.Push(BC);
            context.InstructionsBuilder.Ret();
        }
    }
}
