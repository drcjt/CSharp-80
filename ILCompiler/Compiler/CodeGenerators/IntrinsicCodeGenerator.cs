using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class IntrinsicCodeGenerator
    {
        public static void GenerateCode(IntrinsicEntry entry, Assembler assembler)
        {
            // TODO: Most of this should be done through MethodImplOptions.InternalCall instead
            var methodToCall = entry.TargetMethod;
            switch (methodToCall)
            {
                case "WriteString":
                    assembler.Pop(R16.DE);    // put argument 1 into HL
                    assembler.Pop(R16.HL);
                    assembler.Call("PRINT");
                    break;

                case "WriteInt32":
                    assembler.Pop(R16.DE);
                    assembler.Pop(R16.HL);
                    assembler.Call("LTOA");
                    break;

                case "WriteUInt32":
                    assembler.Pop(R16.DE);
                    assembler.Pop(R16.HL);
                    assembler.Call("ULTOA");
                    break;

                case "WriteChar":
                    assembler.Pop(R16.DE);    // chars are stored on stack as int32 so remove MSW
                    assembler.Pop(R16.HL);    // put argument 1 into HL
                    assembler.Ld(R8.A, R8.L); // Load low byte of argument 1 into A
                    assembler.Call(0x0033); // ROM routine to display character at current cursor position
                    break;
            }
        }
    }
}
