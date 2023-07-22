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
                    context.Emitter.Ld(H, __[IX + (short)-(variable.StackOffset - 1)]);
                    context.Emitter.Ld(L, __[IX + (short)-(variable.StackOffset - 0)]);

                    context.Emitter.Push(IX); // save IX to BC
                    context.Emitter.Pop(BC);

                    context.Emitter.Push(HL); // Move HL to IX
                    context.Emitter.Pop(IX);

                    // Copy struct to the return buffer
                    var returnTypeExactSize = entry.ReturnTypeExactSize ?? 0;
                    CopyHelper.CopyFromStackToHL(context.Emitter, returnTypeExactSize);

                    context.Emitter.Push(BC); // restore IX
                    context.Emitter.Pop(IX);
                }
                else
                {
                    context.Emitter.Pop(DE);            // Copy return value into DE/DE'

                    if (targetType?.Type.IsInt() ?? false)
                    {
                        context.Emitter.Exx();
                        context.Emitter.Pop(DE);
                        context.Emitter.Exx();
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

            if (context.ParamsCount > 0 || (context.LocalsCount + tempCount) > 0)
            {
                if (context.LocalsCount + tempCount > 0)
                {
                    if (context.Configuration.IntegrationTests && !context.Method.LocallocUsed)
                    {
                        // Validate we haven't got any under/over flow on stack
                        // Assert that localsSize + SP - IX = 0

                        // TODO: also need to factor in any localloc space allocated
                        // this is why localloc integration test fails with this code enabled

                        context.Emitter.Ld(HL, (short)localsSize);
                        context.Emitter.Add(HL, SP);

                        context.Emitter.Push(IX);
                        context.Emitter.Pop(BC);

                        context.Emitter.Sbc(HL, BC);

                        var unwindLabel = context.NameMangler.GetUniqueName();
                        context.Emitter.Jp(Condition.Z, unwindLabel);

                        context.Emitter.Halt();

                        context.Emitter.CreateLabel(unwindLabel);
                    }

                    context.Emitter.Ld(SP, IX);     // Move SP to before locals
                }
                context.Emitter.Pop(IX);            // Remove IX

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
                    context.Emitter.Ld(HL, 0);
                    context.Emitter.Add(HL, SP);
                    context.Emitter.Ld(BC, (short)(2 + totalParametersSize));
                    context.Emitter.Add(HL, BC);
                }

                context.Emitter.Pop(BC);      // Store return address in BC

                if (context.ParamsCount > 0)
                {
                    // Remove parameters from stack
                    context.Emitter.Ld(SP, HL);
                }
            }
            else
            {
                context.Emitter.Pop(BC);      // Store return address in BC
            }

            if (hasReturnValue && !entry.ReturnBufferArgIndex.HasValue)
            {
                if (targetType?.Type.IsInt() ?? false)
                {
                    context.Emitter.Exx();
                    context.Emitter.Push(DE);
                    context.Emitter.Exx();
                }
                context.Emitter.Push(DE);
            }

            context.Emitter.Push(BC);
            context.Emitter.Ret();
        }
    }
}
