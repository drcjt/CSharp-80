using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class FieldAddressCodeGenerator : ICodeGenerator<FieldAddressEntry>
    {
        public void GenerateCode(FieldAddressEntry entry, CodeGeneratorContext context)
        {
            var fieldOffset = entry.Offset;

            // Get address of object
            context.Emitter.Pop(R16.HL);      // LSW

            // Calculate field address
            context.Emitter.Ld(R16.DE, (short)fieldOffset);
            context.Emitter.Add(R16.HL, R16.DE);

            // Push field address onto the stack msw first, lsw second
            context.Emitter.Ld(R16.DE, 0);
            context.Emitter.Push(R16.HL);
        }
    }
}
