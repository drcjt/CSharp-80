using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class StaticFieldCodeGenerator : ICodeGenerator<StaticFieldEntry>
    {
        public void GenerateCode(StaticFieldEntry entry, CodeGeneratorContext context)
        {
            var mangledFieldName = entry.Name;

            context.Emitter.Ld(R16.HL, mangledFieldName);
            context.Emitter.Push(R16.HL);
        }
    }
}