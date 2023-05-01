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
            context.Emitter.Push(IX);
            context.Emitter.Pop(HL);

            context.Emitter.Ld(DE, (short)(-offset));
            context.Emitter.Add(HL, DE);

            // Push address
            context.Emitter.Push(HL);
        }
    }
}
