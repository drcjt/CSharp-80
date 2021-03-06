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

            // Negate HL i.e. HL = -HL
            context.Assembler.Ld(R8.A, R8.L);
            context.Assembler.Cpl();
            context.Assembler.Ld(R8.L, R8.A);
            context.Assembler.Ld(R8.A, R8.H);
            context.Assembler.Cpl();
            context.Assembler.Ld(R8.H, R8.A);

            context.Assembler.Add(R16.HL, R16.SP);
            context.Assembler.Ld(R16.SP, R16.HL);

            // Push address of newly allocated space
            context.Assembler.Push(R16.HL);
        }
    }
}
