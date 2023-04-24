using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class IntrinsicCodeGenerator : ICodeGenerator<IntrinsicEntry>
    {
        public void GenerateCode(IntrinsicEntry entry, CodeGeneratorContext context)
        {
            var methodToCall = entry.TargetMethod;
            switch (methodToCall)
            {
                case "WriteChar":
                    if (context.Configuration.TargetArchitecture == TargetArchitecture.CPM)
                    {
                        context.Emitter.Pop(R16.HL);    // chars are stored on stack as int32 so remove MSW

                        context.Emitter.Push(R16.BC);     // save registers
                        context.Emitter.Push(R16.DE);
                        context.Emitter.Push(R16.HL);

                        context.Emitter.Ld(R8.C, 2);      // Call CPM BDOS C_WRITE console write
                        context.Emitter.Ld(R8.E, R8.L);   // character to write goes into E
                        context.Emitter.Call(0x05);       // call BDOS C_WRITE

                        context.Emitter.Pop(R16.HL);      // restore registers
                        context.Emitter.Pop(R16.DE);
                        context.Emitter.Pop(R16.BC);
                    }
                    else if (context.Configuration.TargetArchitecture == TargetArchitecture.TRS80)
                    {
                        context.Emitter.Pop(R16.HL);    // chars are stored on stack as int32 so remove MSW
                        context.Emitter.Ld(R8.A, R8.L); // Load low byte of argument 1 into A

                        context.Emitter.LdFromMemory(R16.HL, 0x4020);
                        context.Emitter.LdToMemory(R16.HL, R8.A);
                    } else if (context.Configuration.TargetArchitecture == TargetArchitecture.ZXSpectrum)
                    {
                        context.Emitter.Pop(R16.HL);    // chars are stored on stack as int32 so remove MSW
                        context.Emitter.Ld(R8.A, R8.L); // Load low byte of argument 1 into A
                        context.Emitter.Rst(0x0010); // ROM routine to display character at current cursor position
                    }
                    break;

                case "Exit":
                    context.Emitter.Jp("EXITRETCODE");
                    break;
            }
        }
    }
}