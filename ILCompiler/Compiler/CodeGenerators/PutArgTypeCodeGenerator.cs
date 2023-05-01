using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class PutArgTypeCodeGenerator : ICodeGenerator<PutArgTypeEntry>
    {
        public void GenerateCode(PutArgTypeEntry entry, CodeGeneratorContext context)
        {
            // We have 4 bytes on the stack representing a small data type, e.g int16, uint16, int8, uint8, bool
            // For all of these cases we drop the MSWs

            context.Emitter.Pop(HL);      // LSW
            context.Emitter.Pop(DE);      // MSW

            context.Emitter.Push(HL);
        }
    }
}
