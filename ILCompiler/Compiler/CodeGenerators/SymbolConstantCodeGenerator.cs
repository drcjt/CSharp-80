using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class SymbolConstantCodeGenerator : ICodeGenerator<SymbolConstantEntry>
    {
        public void GenerateCode(SymbolConstantEntry entry, CodeGeneratorContext context)
        {
            var mangledFieldName = entry.Value;

            if (entry.Offset != 0)
            {
                context.InstructionsBuilder.Ld(HL, $"{mangledFieldName} + {entry.Offset}");
            }
            else
            {
                context.InstructionsBuilder.Ld(HL, mangledFieldName);
            }
            context.InstructionsBuilder.Push(HL);
        }
    }
}