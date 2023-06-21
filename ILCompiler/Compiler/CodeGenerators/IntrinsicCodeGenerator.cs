using ILCompiler.Compiler.EvaluationStack;
using Microsoft.Extensions.Configuration;
using static ILCompiler.Compiler.Emit.Registers;

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
                        context.Emitter.Pop(HL);    // chars are stored on stack as int32 so remove MSW

                        context.Emitter.Push(BC);     // save registers
                        context.Emitter.Push(DE);
                        context.Emitter.Push(HL);

                        context.Emitter.Ld(C, 2);      // Call CPM BDOS C_WRITE console write
                        context.Emitter.Ld(E, L);   // character to write goes into E
                        context.Emitter.Call(0x05);       // call BDOS C_WRITE

                        context.Emitter.Pop(HL);      // restore registers
                        context.Emitter.Pop(DE);
                        context.Emitter.Pop(BC);
                    }
                    else if (context.Configuration.TargetArchitecture == TargetArchitecture.TRS80)
                    {
                        context.Emitter.Pop(HL);    // chars are stored on stack as int32 so remove MSW
                        context.Emitter.Ld(A, L); // Load low byte of argument 1 into A
                        context.Emitter.Call(0x0033); // ROM routine to display character at current cursor position
                    } 
                    else if (context.Configuration.TargetArchitecture == TargetArchitecture.ZXSpectrum)
                    {
                        context.Emitter.Pop(HL);    // chars are stored on stack as int32 so remove MSW
                        context.Emitter.Ld(A, L); // Load low byte of argument 1 into A
                        context.Emitter.Rst(0x0010); // ROM routine to display character at current cursor position
                        context.Emitter.Ld(A, 255);
                        context.Emitter.Ld(__[23692], A);
                    }
                    break;

                case "Exit":
                    context.Emitter.Jp("EXITRETCODE");
                    break;
            }
        }
    }
}