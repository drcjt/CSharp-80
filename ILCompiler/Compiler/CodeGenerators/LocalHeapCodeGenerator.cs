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
            context.Emitter.Pop(HL);

            var endLabel = context.NameMangler.GetUniqueName();

            // Test if size is 0 and return null if it is
            context.Emitter.Ld(A, H);
            context.Emitter.Or(L);
            context.Emitter.Jp(Condition.Z, endLabel);

            // Round up number of bytes to allocate to a StackAlign boundary
            // BC = (HL + (StackAlign - 1)) & ~(StackAlign - 1)
            context.Emitter.Ld(DE, (short)(StackAlign - 1));
            context.Emitter.Add(HL, DE);
            context.Emitter.Ld(DE, (short)~(StackAlign - 1));
            context.Emitter.Ld(A, H);
            context.Emitter.And(D);
            context.Emitter.Ld(B, A);
            context.Emitter.Ld(A, L);
            context.Emitter.And(E);
            context.Emitter.Ld(C, A);

            if (context.Method.Body.InitLocals)
            {
                // zero space on stack
                context.Emitter.Ld(HL, 0);

                // Divide BC by 2
                context.Emitter.Srl(B);
                context.Emitter.Rr(C);
                context.Emitter.Srl(B);
                context.Emitter.Rr(C);

                // Start of Zeroing loop
                var initLoopLabel = context.NameMangler.GetUniqueName();
                context.Emitter.CreateLabel(initLoopLabel);

                // Zero two zero bytes
                context.Emitter.Push(HL);
                context.Emitter.Push(HL);

                // Decrement byte count
                context.Emitter.Dec(BC);

                context.Emitter.Ld(A, B);
                context.Emitter.Or(C);

                // More bytes to zero then loop
                context.Emitter.Jp(Condition.NZ, initLoopLabel);

                context.Emitter.Ld(HL, 0);
                context.Emitter.Add(HL, SP);
            }
            else
            {
                context.Emitter.Ld(HL, 0);
                context.Emitter.Add(HL, SP);

                context.Emitter.And(A);
                context.Emitter.Sbc(HL, BC);

                context.Emitter.Ld(SP, HL);
            }

            context.Emitter.CreateLabel(endLabel);

            // Push address of newly allocated space
            context.Emitter.Push(HL);
        }
    }
}
