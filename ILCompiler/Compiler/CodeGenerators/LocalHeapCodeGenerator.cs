using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class LocalHeapCodeGenerator : ICodeGenerator<LocalHeapEntry>
    {
        public void GenerateCode(LocalHeapEntry entry, CodeGeneratorContext context)
        {
            // Use the 16 bit size on top of the stack to determine the amount of localloc to do

            context.Assembler.Pop(R16.HL);  // amount of space to localloc

            if (context.Method.Body.InitLocals)
            {
                // Reserve and zero space on stack
                context.Assembler.Push(R16.HL);
                context.Assembler.Pop(R16.BC);
                context.Assembler.Ld(R16.HL, 0);

                var initLoopLabel = context.NameMangler.GetUniqueName();
                context.Assembler.AddInstruction(new LabelInstruction(initLoopLabel));

                context.Assembler.Push(R16.HL);
                context.Assembler.Dec(R16.BC);
                context.Assembler.Dec(R16.BC);      // TODO: Divide BC by 2 earlier to avoid having to do this
                context.Assembler.Ld(R8.A, R8.B);
                context.Assembler.Or(R8.C);
                context.Assembler.Jp(Condition.NonZero, initLoopLabel);

                context.Assembler.Ld(R16.HL, 0);
                context.Assembler.Add(R16.HL, R16.SP);

                // TODO: Need to Zero last byte if size to allocate is odd number of bytes
            }
            else
            {
                // Negate HL i.e. HL = -HL
                context.Assembler.Ld(R8.A, R8.L);
                context.Assembler.Cpl();
                context.Assembler.Ld(R8.L, R8.A);
                context.Assembler.Ld(R8.A, R8.H);
                context.Assembler.Cpl();
                context.Assembler.Ld(R8.H, R8.A);

                // TODO: Use sbc instead to avoid need to negate HL
                context.Assembler.Add(R16.HL, R16.SP);
                context.Assembler.Ld(R16.SP, R16.HL);
            }

            // Push address of newly allocated space
            context.Assembler.Push(R16.HL);
        }
    }
}
