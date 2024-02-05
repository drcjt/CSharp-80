using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class ExpressionCodeGenerator : ICodeGenerator<ExpressionEntry>
    {
        public void GenerateCode(ExpressionEntry entry, CodeGeneratorContext context)
        {
            var mangledFieldName = entry.Name;

            context.InstructionsBuilder.Ld(HL, mangledFieldName);
            context.InstructionsBuilder.Push(HL);
        }
    }
}