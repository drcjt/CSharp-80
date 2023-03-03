using ILCompiler.Compiler.EvaluationStack;
using System.Xml;
using Z80Assembler;

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
                    context.Assembler.Ld(R8.H, I16.IX, (short)-(variable.StackOffset - 1));
                    context.Assembler.Ld(R8.L, I16.IX, (short)-(variable.StackOffset - 0));

                    context.Assembler.Push(I16.IX); // save IX to BC
                    context.Assembler.Pop(R16.BC);

                    context.Assembler.Push(R16.HL); // Move HL to IX
                    context.Assembler.Pop(I16.IX);

                    // Copy struct to the return buffer
                    var returnTypeExactSize = entry.ReturnTypeExactSize ?? 0;
                    CopyHelper.CopyFromStackToIX(context.Assembler, returnTypeExactSize);

                    context.Assembler.Push(R16.BC); // restore IX
                    context.Assembler.Pop(I16.IX);
                }
                else
                {
                    context.Assembler.Pop(R16.DE);            // Copy return value into DE/DE'

                    if (targetType?.Type.IsInt() ?? false)
                    {
                        context.Assembler.Exx();
                        context.Assembler.Pop(R16.DE);
                        context.Assembler.Exx();
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

                        // TODO: also need to factor in any localalloc space allocated
                        // this is why localloc integration test fails with this code enabled

                        context.Assembler.Ld(R16.HL, (short)localsSize);
                        context.Assembler.Add(R16.HL, R16.SP);

                        context.Assembler.Push(I16.IX);
                        context.Assembler.Pop(R16.BC);

                        context.Assembler.Sbc(R16.HL, R16.BC);

                        var unwindLabel = context.NameMangler.GetUniqueName();
                        context.Assembler.Jp(Condition.Zero, unwindLabel);

                        context.Assembler.Halt();

                        context.Assembler.AddInstruction(new LabelInstruction(unwindLabel));
                    }

                    context.Assembler.Ld(R16.SP, I16.IX);     // Move SP to before locals
                }
                context.Assembler.Pop(I16.IX);            // Remove IX

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
                    context.Assembler.Ld(R16.HL, 0);
                    context.Assembler.Add(R16.HL, R16.SP);
                    context.Assembler.Ld(R16.BC, (short)(2 + totalParametersSize));
                    context.Assembler.Add(R16.HL, R16.BC);
                }

                context.Assembler.Pop(R16.BC);      // Store return address in BC

                if (context.ParamsCount > 0)
                {
                    // Remove parameters from stack
                    context.Assembler.Ld(R16.SP, R16.HL);
                }
            }
            else
            {
                context.Assembler.Pop(R16.BC);      // Store return address in BC
            }

            if (hasReturnValue && !entry.ReturnBufferArgIndex.HasValue)
            {
                if (targetType?.Type.IsInt() ?? false)
                {
                    context.Assembler.Exx();
                    context.Assembler.Push(R16.DE);
                    context.Assembler.Exx();
                }
                context.Assembler.Push(R16.DE);
            }

            context.Assembler.Push(R16.BC);
            context.Assembler.Ret();
        }
    }
}
