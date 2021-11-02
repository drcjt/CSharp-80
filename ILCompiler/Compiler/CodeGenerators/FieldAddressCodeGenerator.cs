using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    public class FieldAddressCodeGenerator
    {
        public static void GenerateCode(FieldAddressEntry entry, Assembler assembler)
        {
            var fieldOffset = entry.Offset;

            // Get address of object
            assembler.Pop(R16.DE);      // lsw will be ignored
            assembler.Pop(R16.HL);

            // Calculate field address
            assembler.Ld(R16.DE, (short)fieldOffset);
            assembler.Add(R16.HL, R16.DE);

            // Push field address onto the stack
            assembler.Push(R16.HL);
            assembler.Ld(R16.DE, 0);
            assembler.Push(R16.DE);
        }
    }
}
