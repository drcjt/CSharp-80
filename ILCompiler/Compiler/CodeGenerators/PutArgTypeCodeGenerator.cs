using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class PutArgTypeCodeGenerator : ICodeGenerator<PutArgTypeEntry>
    {
        public void GenerateCode(PutArgTypeEntry entry, CodeGeneratorContext context)
        {
            // We have 4 bytes on the stack representing a small data type, e.g int16, uint16, int8, uint8, bool
            // For all of these cases we drop the MSWs

            context.Emitter.Pop(R16.HL);      // LSW
            context.Emitter.Pop(R16.DE);      // MSW

            context.Emitter.Push(R16.HL);
        }
    }
}
