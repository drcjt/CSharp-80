using ILCompiler.Compiler.EvaluationStack;
using System.Collections.Generic;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    public class LocalVariableAddressCodeGenerator
    {
        public static void GenerateCode(LocalVariableAddressEntry entry, Assembler assembler, IList<LocalVariableDescriptor> localVariableTable)
        {
            // Loading address of a local variable/argument
            var localVariable = localVariableTable[entry.LocalNumber];
            var offset = localVariable.StackOffset;

            // Calculate and push the actual 16 bit address
            assembler.Push(I16.IX);
            assembler.Pop(R16.HL);

            assembler.Ld(R16.DE, (short)(-offset));
            assembler.Add(R16.HL, R16.DE);

            // Push address
            assembler.Push(R16.HL);

            // Push 0 to makeup full 32 bit value
            assembler.Ld(R16.HL, 0);
            assembler.Push(R16.HL);
        }
    }
}
