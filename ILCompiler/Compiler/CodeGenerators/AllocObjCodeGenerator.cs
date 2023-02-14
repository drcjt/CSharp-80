using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class AllocObjCodeGenerator : ICodeGenerator<AllocObjEntry>
    {
        public void GenerateCode(AllocObjEntry entry, CodeGeneratorContext context)
        {
            // Allocate memory on the heap using simple zero GC/increment a pointer approach

            context.Assembler.Ld(R16.HL, (short)entry.Size);
            context.Assembler.Push(R16.HL);
            context.Assembler.Call("NewObject");
        }
    }
}
