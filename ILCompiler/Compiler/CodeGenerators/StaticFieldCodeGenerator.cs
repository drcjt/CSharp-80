using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class StaticFieldCodeGenerator : ICodeGenerator<StaticFieldEntry>
    {
        public void GenerateCode(StaticFieldEntry entry, CodeGeneratorContext context)
        {
            var mangledFieldName = entry.Name;

            context.Assembler.Ld(R16.HL, mangledFieldName);
            context.Assembler.Push(R16.HL);
        }
    }
}