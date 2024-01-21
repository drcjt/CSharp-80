using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

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
            context.InstructionsBuilder.Push(IX);
            context.InstructionsBuilder.Pop(HL);

            CodeGeneratorHelper.AddHLFromDE(context.InstructionsBuilder, (short)(-offset));

            // Push address
            context.InstructionsBuilder.Push(HL);
        }
    }
}
