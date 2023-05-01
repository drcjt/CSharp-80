using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class FieldAddressCodeGenerator : ICodeGenerator<FieldAddressEntry>
    {
        public void GenerateCode(FieldAddressEntry entry, CodeGeneratorContext context)
        {
            var fieldOffset = entry.Offset;

            // Get address of object
            context.Emitter.Pop(HL);      // LSW

            // Calculate field address
            context.Emitter.Ld(DE, (short)fieldOffset);
            context.Emitter.Add(HL, DE);

            // Push field address onto the stack msw first, lsw second
            context.Emitter.Ld(DE, 0);
            context.Emitter.Push(HL);
        }
    }
}
