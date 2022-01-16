using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class IntrinsicCodeGenerator : ICodeGenerator<IntrinsicEntry>
    {
        public void GenerateCode(IntrinsicEntry entry, CodeGeneratorContext context)
        {
            // TODO: Most of this should be done through MethodImplOptions.InternalCall instead
            var methodToCall = entry.TargetMethod;
            switch (methodToCall)
            {
                case "WriteString":
                    context.Assembler.Pop(R16.HL);    // put argument 1 into HL
                    context.Assembler.Pop(R16.DE);
                    context.Assembler.Call("PRINT");
                    break;

                case "WriteInt32":
                    context.Assembler.Pop(R16.HL);
                    context.Assembler.Pop(R16.DE);
                    context.Assembler.Call("LTOA");
                    break;

                case "WriteUInt32":
                    context.Assembler.Pop(R16.HL);
                    context.Assembler.Pop(R16.DE);
                    context.Assembler.Call("ULTOA");
                    break;

                case "WriteChar":
                    if (context.Configuration.TargetCpm)
                    {
                        context.Assembler.Pop(R16.HL);    // chars are stored on stack as int32 so remove MSW
                        context.Assembler.Pop(R16.DE);    // put argument 1 into HL

                        context.Assembler.Push(R16.BC);     // save registers
                        context.Assembler.Push(R16.DE);
                        context.Assembler.Push(R16.HL);

                        context.Assembler.Ld(R8.C, 2);      // Call CPM BDOS C_WRITE console write
                        context.Assembler.Ld(R8.E, R8.L);   // character to write goes into E
                        context.Assembler.Call(0x05);       // call BDOS C_WRITE

                        context.Assembler.Pop(R16.HL);      // restore registers
                        context.Assembler.Pop(R16.DE);
                        context.Assembler.Pop(R16.BC);
                    }
                    else
                    {
                        context.Assembler.Pop(R16.HL);    // chars are stored on stack as int32 so remove MSW
                        context.Assembler.Pop(R16.DE);    // put argument 1 into HL
                        context.Assembler.Ld(R8.A, R8.L); // Load low byte of argument 1 into A
                        context.Assembler.Call(0x0033); // ROM routine to display character at current cursor position
                    }
                    break;
            }
        }
    }
}
