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
                    context.Assembler.Pop(R16.DE);    // put argument 1 into HL
                    context.Assembler.Pop(R16.HL);
                    context.Assembler.Call("PRINT");
                    break;

                case "WriteInt32":
                    context.Assembler.Pop(R16.DE);
                    context.Assembler.Pop(R16.HL);
                    context.Assembler.Call("LTOA");
                    break;

                case "WriteUInt32":
                    context.Assembler.Pop(R16.DE);
                    context.Assembler.Pop(R16.HL);
                    context.Assembler.Call("ULTOA");
                    break;

                case "WriteChar":
                    context.Assembler.Pop(R16.DE);    // chars are stored on stack as int32 so remove MSW
                    context.Assembler.Pop(R16.HL);    // put argument 1 into HL
                    context.Assembler.Ld(R8.A, R8.L); // Load low byte of argument 1 into A
                    context.Assembler.Call(0x0033); // ROM routine to display character at current cursor position
                    break;
            }
        }
    }
}
