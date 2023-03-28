using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class LocalHeapCodeGenerator : ICodeGenerator<LocalHeapEntry>
    {
        private const int StackAlign = 4;
        public void GenerateCode(LocalHeapEntry entry, CodeGeneratorContext context)
        {
            // Use the 16 bit size on top of the stack to determine the amount of localloc to do
            context.Assembler.Pop(R16.HL);

            // Round up number of bytes to allocate to a StackAlign boundary
            // BC = (HL + (StackAlign - 1)) & ~(StackAlign - 1)
            context.Assembler.Ld(R16.DE, (short)(StackAlign - 1));
            context.Assembler.Add(R16.HL, R16.DE);
            context.Assembler.Ld(R16.DE, (short)~(StackAlign - 1));
            context.Assembler.Ld(R8.A, R8.H);
            context.Assembler.And(R8.D);
            context.Assembler.Ld(R8.B, R8.A);
            context.Assembler.Ld(R8.A, R8.L);
            context.Assembler.And(R8.E);
            context.Assembler.Ld(R8.C, R8.A);

            if (context.Method.Body.InitLocals)
            {
                // zero space on stack
                context.Assembler.Ld(R16.HL, 0);

                // Divide BC by 2
                context.Assembler.ShiftRightLogical(R8.B);
                context.Assembler.RotateRight(R8.C);

                // Start of Zeroing loop
                var initLoopLabel = context.NameMangler.GetUniqueName();
                context.Assembler.AddInstruction(new LabelInstruction(initLoopLabel));

                // Zero a byte
                context.Assembler.Push(R16.HL);

                // Decrement byte count
                context.Assembler.Dec(R16.BC);

                context.Assembler.Ld(R8.A, R8.B);
                context.Assembler.Or(R8.C);

                // More bytes to zero then loop
                context.Assembler.Jp(Condition.NonZero, initLoopLabel);

                context.Assembler.Ld(R16.HL, 0);
                context.Assembler.Add(R16.HL, R16.SP);
            }
            else
            {
                context.Assembler.Ld(R16.HL, 0);
                context.Assembler.Add(R16.HL, R16.SP);

                context.Assembler.And(R8.A);
                context.Assembler.Sbc(R16.HL, R16.BC);

                context.Assembler.Ld(R16.SP, R16.HL);
            }

            // Push address of newly allocated space
            context.Assembler.Push(R16.HL);
        }
    }
}
