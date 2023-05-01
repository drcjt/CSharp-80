using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class StaticFieldCodeGenerator : ICodeGenerator<StaticFieldEntry>
    {
        public void GenerateCode(StaticFieldEntry entry, CodeGeneratorContext context)
        {
            var mangledFieldName = entry.Name;

            context.Emitter.Ld(HL, mangledFieldName);
            context.Emitter.Push(HL);
        }
    }
}