using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class LocalHeapCodeGenerator : ICodeGenerator<LocalHeapEntry>
    {
        public void GenerateCode(LocalHeapEntry entry, CodeGeneratorContext context)
        {
            // Use the 16 bit size on top of the stack to determine the amount of localloc to do
            context.Assembler.Pop(R16.BC);

            context.Assembler.Ld(R16.HL, 0);
            context.Assembler.Add(R16.HL, R16.SP);

            if (!context.Method.Body.InitLocals)
            {
                context.Assembler.And(R8.A);
                context.Assembler.Sbc(R16.HL, R16.BC);
            }
            else
            {
                // zero space on stack

                // Start of Zeroing loop
                var initLoopLabel = context.NameMangler.GetUniqueName();
                context.Assembler.AddInstruction(new LabelInstruction(initLoopLabel));

                // Move to next byte to zero remembering that stack grows downwards
                context.Assembler.Dec(R16.HL);

                // Zero a byte
                context.Assembler.LdInd(R16.HL, 0);

                // Decrement byte count
                context.Assembler.Dec(R16.BC);

                context.Assembler.Ld(R8.A, R8.B);
                context.Assembler.Or(R8.C);

                // More bytes to zero then loop
                context.Assembler.Jp(Condition.NonZero, initLoopLabel);
            }

            context.Assembler.Ld(R16.SP, R16.HL);

            // Push address of newly allocated space
            context.Assembler.Push(R16.HL);

        }
    }
}
