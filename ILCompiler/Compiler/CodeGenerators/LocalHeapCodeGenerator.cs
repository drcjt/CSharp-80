using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class LocalHeapCodeGenerator : ICodeGenerator<LocalHeapEntry>
    {
        private const int StackAlign = 4;
        public void GenerateCode(LocalHeapEntry entry, CodeGeneratorContext context)
        {
            // Use the 16 bit size on top of the stack to determine the amount of localloc to do
            context.InstructionsBuilder.Pop(HL);

            var endLabel = context.NameMangler.GetUniqueName();

            // Test if size is 0 and return null if it is
            context.InstructionsBuilder.Ld(A, H);
            context.InstructionsBuilder.Or(L);
            context.InstructionsBuilder.Jp(Condition.Z, endLabel);

            // Round up number of bytes to allocate to a StackAlign boundary
            // BC = (HL + (StackAlign - 1)) & ~(StackAlign - 1)
            context.InstructionsBuilder.Ld(DE, (short)(StackAlign - 1));
            context.InstructionsBuilder.Add(HL, DE);
            context.InstructionsBuilder.Ld(DE, (short)~(StackAlign - 1));
            context.InstructionsBuilder.Ld(A, H);
            context.InstructionsBuilder.And(D);
            context.InstructionsBuilder.Ld(B, A);
            context.InstructionsBuilder.Ld(A, L);
            context.InstructionsBuilder.And(E);
            context.InstructionsBuilder.Ld(C, A);

            if (context.Method.MethodIL!.IsInitLocals)
            {
                // zero space on stack
                context.InstructionsBuilder.Ld(HL, 0);

                // Divide BC by 2
                context.InstructionsBuilder.Srl(B);
                context.InstructionsBuilder.Rr(C);
                context.InstructionsBuilder.Srl(B);
                context.InstructionsBuilder.Rr(C);

                // Start of Zeroing loop
                var initLoopLabel = context.NameMangler.GetUniqueName();
                context.InstructionsBuilder.Label(initLoopLabel);

                // Zero two zero bytes
                context.InstructionsBuilder.Push(HL);
                context.InstructionsBuilder.Push(HL);

                // Decrement byte count
                context.InstructionsBuilder.Dec(BC);

                context.InstructionsBuilder.Ld(A, B);
                context.InstructionsBuilder.Or(C);

                // More bytes to zero then loop
                context.InstructionsBuilder.Jp(Condition.NZ, initLoopLabel);

                context.InstructionsBuilder.Ld(HL, 0);
                context.InstructionsBuilder.Add(HL, SP);
            }
            else
            {
                context.InstructionsBuilder.Ld(HL, 0);
                context.InstructionsBuilder.Add(HL, SP);

                context.InstructionsBuilder.And(A);
                context.InstructionsBuilder.Sbc(HL, BC);

                context.InstructionsBuilder.Ld(SP, HL);
            }

            context.InstructionsBuilder.Label(endLabel);

            // Push address of newly allocated space
            context.InstructionsBuilder.Push(HL);
        }
    }
}
