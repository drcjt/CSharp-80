using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class FieldAddressCodeGenerator : ICodeGenerator<FieldAddressEntry>
    {
        public void GenerateCode(FieldAddressEntry entry, CodeGeneratorContext context)
        {
            var fieldOffset = entry.Offset;

            // Get address of object
            context.Assembler.Pop(R16.HL);      // LSW

            // Calculate field address
            context.Assembler.Ld(R16.DE, (short)fieldOffset);
            context.Assembler.Add(R16.HL, R16.DE);

            // Push field address onto the stack msw first, lsw second
            context.Assembler.Ld(R16.DE, 0);
            context.Assembler.Push(R16.HL);
        }
    }
}
