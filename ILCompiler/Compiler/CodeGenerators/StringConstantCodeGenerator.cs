using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    public  class StringConstantCodeGenerator
    {
        public static void GenerateCode(StringConstantEntry entry, Assembler assembler)
        {
            // TODO: Currently obj refs can only be strings
            assembler.Ld(R16.HL, (entry as StringConstantEntry).Label);
            assembler.Push(R16.HL);
            assembler.Ld(R16.HL, 0);
            assembler.Push(R16.HL);
        }
    }
}
