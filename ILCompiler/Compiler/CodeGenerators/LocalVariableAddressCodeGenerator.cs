using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;

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
            context.Emitter.Push(I16.IX);
            context.Emitter.Pop(R16.HL);

            context.Emitter.Ld(R16.DE, (short)(-offset));
            context.Emitter.Add(R16.HL, R16.DE);

            // Push address
            context.Emitter.Push(R16.HL);
        }
    }
}
