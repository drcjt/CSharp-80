using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class LocalVariableAddressCodeGenerator : ICodeGenerator<LocalVariableAddressEntry>
    {
        public void GenerateCode(LocalVariableAddressEntry entry, CodeGeneratorContext context)
        {
            // Loading address of a local variable/argument
            var localVariable = context.LocalVariableTable[entry.LocalNumber];
            var offset = localVariable.StackOffset;

            // Calculate and push the actual 16 bit address
            context.Assembler.Push(I16.IX);
            context.Assembler.Pop(R16.HL);

            context.Assembler.Ld(R16.DE, (short)(-offset));
            context.Assembler.Add(R16.HL, R16.DE);

            // Push address
            context.Assembler.Push(R16.HL);

            // Push 0 to makeup full 32 bit value
            context.Assembler.Ld(R16.HL, 0);
            context.Assembler.Push(R16.HL);
        }
    }
}
