using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class LocalHeapCodeGenerator : ICodeGenerator<LocalHeapEntry>
    {
        private const int StackAlign = 4;
        public void GenerateCode(LocalHeapEntry entry, CodeGeneratorContext context)
        {
            // Use the 16 bit size on top of the stack to determine the amount of localloc to do
            context.Emitter.Pop(R16.HL);

            var endLabel = context.NameMangler.GetUniqueName();

            // Test if size is 0 and return null if it is
            context.Emitter.Ld(R8.A, R8.H);
            context.Emitter.Or(R8.L);
            context.Emitter.Jp(Condition.Zero, endLabel);

            // Round up number of bytes to allocate to a StackAlign boundary
            // BC = (HL + (StackAlign - 1)) & ~(StackAlign - 1)
            context.Emitter.Ld(R16.DE, (short)(StackAlign - 1));
            context.Emitter.Add(R16.HL, R16.DE);
            context.Emitter.Ld(R16.DE, (short)~(StackAlign - 1));
            context.Emitter.Ld(R8.A, R8.H);
            context.Emitter.And(R8.D);
            context.Emitter.Ld(R8.B, R8.A);
            context.Emitter.Ld(R8.A, R8.L);
            context.Emitter.And(R8.E);
            context.Emitter.Ld(R8.C, R8.A);

            if (context.Method.Body.InitLocals)
            {
                // zero space on stack
                context.Emitter.Ld(R16.HL, 0);

                // Divide BC by 2
                context.Emitter.ShiftRightLogical(R8.B);
                context.Emitter.RotateRight(R8.C);
                context.Emitter.ShiftRightLogical(R8.B);
                context.Emitter.RotateRight(R8.C);

                // Start of Zeroing loop
                var initLoopLabel = context.NameMangler.GetUniqueName();
                context.Emitter.EmitInstruction(new LabelInstruction(initLoopLabel));

                // Zero two zero bytes
                context.Emitter.Push(R16.HL);
                context.Emitter.Push(R16.HL);

                // Decrement byte count
                context.Emitter.Dec(R16.BC);

                context.Emitter.Ld(R8.A, R8.B);
                context.Emitter.Or(R8.C);

                // More bytes to zero then loop
                context.Emitter.Jp(Condition.NonZero, initLoopLabel);

                context.Emitter.Ld(R16.HL, 0);
                context.Emitter.Add(R16.HL, R16.SP);
            }
            else
            {
                context.Emitter.Ld(R16.HL, 0);
                context.Emitter.Add(R16.HL, R16.SP);

                context.Emitter.And(R8.A);
                context.Emitter.Sbc(R16.HL, R16.BC);

                context.Emitter.Ld(R16.SP, R16.HL);
            }

            context.Emitter.EmitInstruction(new LabelInstruction(endLabel));

            // Push address of newly allocated space
            context.Emitter.Push(R16.HL);
        }
    }
}
