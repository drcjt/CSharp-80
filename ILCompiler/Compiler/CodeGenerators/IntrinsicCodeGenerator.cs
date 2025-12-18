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
                        context.InstructionsBuilder.Pop(HL);    // chars are stored on stack as int32 so remove MSW

                        context.InstructionsBuilder.Push(BC);     // save registers
                        context.InstructionsBuilder.Push(DE);
                        context.InstructionsBuilder.Push(HL);

                        context.InstructionsBuilder.Ld(C, 2);      // Call CPM BDOS C_WRITE console write
                        context.InstructionsBuilder.Ld(E, L);   // character to write goes into E
                        context.InstructionsBuilder.Call(0x05);       // call BDOS C_WRITE

                        context.InstructionsBuilder.Pop(HL);      // restore registers
                        context.InstructionsBuilder.Pop(DE);
                        context.InstructionsBuilder.Pop(BC);
                    }
                    else if (context.Configuration.TargetArchitecture == TargetArchitecture.TRS80)
                    {
                        context.InstructionsBuilder.Pop(HL);    // chars are stored on stack as int32 so remove MSW
                        context.InstructionsBuilder.Ld(A, L); // Load low byte of argument 1 into A
                        context.InstructionsBuilder.Call(0x0033); // ROM routine to display character at current cursor position
                    } 
                    else if (context.Configuration.TargetArchitecture == TargetArchitecture.ZXSpectrum)
                    {
                        context.InstructionsBuilder.Pop(HL);    // chars are stored on stack as int32 so remove MSW
                        context.InstructionsBuilder.Ld(A, L); // Load low byte of argument 1 into A
                        context.InstructionsBuilder.Rst(0x0010); // ROM routine to display character at current cursor position
                        context.InstructionsBuilder.Ld(A, 255);
                        context.InstructionsBuilder.Ld(__[23692], A);
                    }
                    break;

                case "Exit":
                    context.InstructionsBuilder.Jp("EXITRETCODE");
                    break;

                case "DebugBreak":
                    if (context.Configuration.TargetArchitecture == TargetArchitecture.TRS80)
                    {
                        context.InstructionsBuilder.Call(0x440d);
                    }
                    else
                    {
                        // TODO: Implement for other architectures
                    }
                    break;
            }
        }
    }
}
