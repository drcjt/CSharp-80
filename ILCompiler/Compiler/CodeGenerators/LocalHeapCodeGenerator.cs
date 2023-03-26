using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class LocalHeapCodeGenerator : ICodeGenerator<LocalHeapEntry>
    {
        public void GenerateCode(LocalHeapEntry entry, CodeGeneratorContext context)
        {
            if (context.Method.Body.InitLocals)
            {
                // Reserve and zero space on stack
                // Use the 16 bit size on top of the stack to determine the amount of localloc to do
                context.Assembler.Pop(R16.BC);

                // Move SP to HL
                context.Assembler.Ld(R16.HL, 0);
                context.Assembler.Add(R16.HL, R16.SP);

                // Start of Zeroing loop
                var initLoopLabel = context.NameMangler.GetUniqueName();
                context.Assembler.AddInstruction(new LabelInstruction(initLoopLabel));

                // Zero a byte
                context.Assembler.LdInd(R16.HL, 0);

                // Move to next byte to zero remembering that stack grows downwards
                context.Assembler.Dec(R16.HL);

                // Decrement byte count
                context.Assembler.Dec(R16.BC);

                // More bytes to zero then loop
                context.Assembler.Jp(Condition.NonZero, initLoopLabel);

                // Set SP to resulting HL
                context.Assembler.Ld(R16.SP, R16.HL);
            }
            else
            {
                // Use the 16 bit size on top of the stack to determine the amount of localloc to do
                context.Assembler.Pop(R16.HL);

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
