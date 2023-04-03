using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;

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
                    context.Emitter.Ld(R8.H, I16.IX, (short)-(variable.StackOffset - 1));
                    context.Emitter.Ld(R8.L, I16.IX, (short)-(variable.StackOffset - 0));

                    context.Emitter.Push(I16.IX); // save IX to BC
                    context.Emitter.Pop(R16.BC);

                    context.Emitter.Push(R16.HL); // Move HL to IX
                    context.Emitter.Pop(I16.IX);

                    // Copy struct to the return buffer
                    var returnTypeExactSize = entry.ReturnTypeExactSize ?? 0;
                    CopyHelper.CopyFromStackToIX(context.Emitter, returnTypeExactSize);

                    context.Emitter.Push(R16.BC); // restore IX
                    context.Emitter.Pop(I16.IX);
                }
                else
                {
                    context.Emitter.Pop(R16.DE);            // Copy return value into DE/DE'

                    if (targetType?.Type.IsInt() ?? false)
                    {
                        context.Emitter.Exx();
                        context.Emitter.Pop(R16.DE);
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

                        context.Emitter.Ld(R16.HL, (short)localsSize);
                        context.Emitter.Add(R16.HL, R16.SP);

                        context.Emitter.Push(I16.IX);
                        context.Emitter.Pop(R16.BC);

                        context.Emitter.Sbc(R16.HL, R16.BC);

                        var unwindLabel = context.NameMangler.GetUniqueName();
                        context.Emitter.Jp(Condition.Zero, unwindLabel);

                        context.Emitter.Halt();

                        context.Emitter.EmitInstruction(new LabelInstruction(unwindLabel));
                    }

                    context.Emitter.Ld(R16.SP, I16.IX);     // Move SP to before locals
                }
                context.Emitter.Pop(I16.IX);            // Remove IX

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
                    context.Emitter.Ld(R16.HL, 0);
                    context.Emitter.Add(R16.HL, R16.SP);
                    context.Emitter.Ld(R16.BC, (short)(2 + totalParametersSize));
                    context.Emitter.Add(R16.HL, R16.BC);
                }

                context.Emitter.Pop(R16.BC);      // Store return address in BC

                if (context.ParamsCount > 0)
                {
                    // Remove parameters from stack
                    context.Emitter.Ld(R16.SP, R16.HL);
                }
            }
            else
            {
                context.Emitter.Pop(R16.BC);      // Store return address in BC
            }

            if (hasReturnValue && !entry.ReturnBufferArgIndex.HasValue)
            {
                if (targetType?.Type.IsInt() ?? false)
                {
                    context.Emitter.Exx();
                    context.Emitter.Push(R16.DE);
                    context.Emitter.Exx();
                }
                context.Emitter.Push(R16.DE);
            }

            context.Emitter.Push(R16.BC);
            context.Emitter.Ret();
        }
    }
}
