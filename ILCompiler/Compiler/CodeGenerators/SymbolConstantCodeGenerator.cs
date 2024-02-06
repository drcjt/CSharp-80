using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class SymbolConstantCodeGenerator : ICodeGenerator<SymbolConstantEntry>
    {
        public void GenerateCode(SymbolConstantEntry entry, CodeGeneratorContext context)
        {
            var mangledFieldName = entry.Value;

            context.InstructionsBuilder.Ld(HL, mangledFieldName);
            context.InstructionsBuilder.Push(HL);
        }
    }
}